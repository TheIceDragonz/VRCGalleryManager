using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VRCGalleryManager.Core.Helpers
{
    /// <summary>
    /// High-performance PNG metadata reader and injector in pure C#.
    /// Writes standard PNG iTXt chunks fully compatible with VRCX and VRChat.
    /// </summary>
    public static class PngMetadataWriter
    {
        private static readonly byte[] PngSignature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        private static readonly uint[] Crc32Table;

        static PngMetadataWriter()
        {
            Crc32Table = new uint[256];
            for (uint i = 0; i < 256; i++)
            {
                uint c = i;
                for (int j = 0; j < 8; j++)
                {
                    if ((c & 1) != 0)
                        c = 0xEDB88320 ^ (c >> 1);
                    else
                        c >>= 1;
                }
                Crc32Table[i] = c;
            }
        }

        public static uint CalculateCrc32(ReadOnlySpan<byte> typeSpan, ReadOnlySpan<byte> dataSpan)
        {
            uint crc = 0xFFFFFFFF;
            foreach (byte b in typeSpan)
            {
                crc = Crc32Table[(crc ^ b) & 0xFF] ^ (crc >> 8);
            }
            foreach (byte b in dataSpan)
            {
                crc = Crc32Table[(crc ^ b) & 0xFF] ^ (crc >> 8);
            }
            return crc ^ 0xFFFFFFFF;
        }

        /// <summary>
        /// Reads an iTXt text chunk by keyword (e.g. "Description" or "XML:com.adobe.xmp") from a PNG file.
        /// </summary>
        public static string? ReadTextChunk(string filePath, string targetKeyword)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return null;

            try
            {
                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096);
                if (fs.Length < 33) return null;

                Span<byte> sig = stackalloc byte[8];
                if (fs.Read(sig) < 8 || !sig.SequenceEqual(PngSignature))
                    return null;

                Span<byte> header = stackalloc byte[8];
                while (fs.Position < fs.Length)
                {
                    if (fs.Read(header) < 8) break;

                    uint length = BinaryPrimitives.ReadUInt32BigEndian(header.Slice(0, 4));
                    string chunkType = Encoding.ASCII.GetString(header.Slice(4, 4));

                    // Stop if we hit image data
                    if (chunkType == "IDAT" || chunkType == "IEND")
                        break;

                    if (chunkType == "iTXt" && length > 0 && length < 10 * 1024 * 1024)
                    {
                        byte[] chunkData = ArrayPool<byte>.Shared.Rent((int)length);
                        try
                        {
                            fs.ReadExactly(chunkData, 0, (int)length);
                            fs.Seek(4, SeekOrigin.Current); // skip CRC

                            var span = chunkData.AsSpan(0, (int)length);
                            int nullIdx = span.IndexOf((byte)0);
                            if (nullIdx > 0 && nullIdx <= 79)
                            {
                                string keyword = Encoding.UTF8.GetString(span.Slice(0, nullIdx));
                                if (string.Equals(keyword, targetKeyword, StringComparison.OrdinalIgnoreCase))
                                {
                                    // iTXt structure:
                                    // keyword + null(1) + compFlag(1) + compMethod(1) + langTag + null(1) + transKeyword + null(1) + text
                                    int offset = nullIdx + 3;
                                    // skip lang tag
                                    while (offset < span.Length && span[offset] != 0) offset++;
                                    offset++; // skip null
                                    // skip translated keyword
                                    while (offset < span.Length && span[offset] != 0) offset++;
                                    offset++; // skip null

                                    if (offset <= span.Length)
                                    {
                                        return Encoding.UTF8.GetString(span.Slice(offset));
                                    }
                                }
                            }
                            continue;
                        }
                        finally
                        {
                            ArrayPool<byte>.Shared.Return(chunkData);
                        }
                    }

                    // Skip chunk data + 4-byte CRC
                    fs.Seek((long)length + 4, SeekOrigin.Current);
                }
            }
            catch
            {
                // Ignored
            }

            return null;
        }

        /// <summary>
        /// Generates an uncompressed iTXt chunk byte array ready to write into a PNG stream.
        /// </summary>
        public static byte[] BuildITXtChunk(string keyword, string text)
        {
            byte[] keywordBytes = Encoding.UTF8.GetBytes(keyword);
            byte[] textBytes = Encoding.UTF8.GetBytes(text);

            // Chunk data: Keyword + 0x00 + flag(0x00) + method(0x00) + langTag(empty) + 0x00 + transKeyword(empty) + 0x00 + textBytes
            int dataLength = keywordBytes.Length + 5 + textBytes.Length;
            byte[] chunkBuffer = new byte[4 + 4 + dataLength + 4]; // Length (4) + Type (4) + Data + CRC (4)

            // Length (big-endian)
            BinaryPrimitives.WriteUInt32BigEndian(chunkBuffer.AsSpan(0, 4), (uint)dataLength);

            // Type ("iTXt")
            Encoding.ASCII.GetBytes("iTXt", chunkBuffer.AsSpan(4, 4));

            // Data
            int dataOffset = 8;
            keywordBytes.CopyTo(chunkBuffer, dataOffset);
            dataOffset += keywordBytes.Length;
            chunkBuffer[dataOffset++] = 0x00; // Null separator
            chunkBuffer[dataOffset++] = 0x00; // Compression flag (0 = uncompressed)
            chunkBuffer[dataOffset++] = 0x00; // Compression method (0 = deflate)
            chunkBuffer[dataOffset++] = 0x00; // Language tag null separator
            chunkBuffer[dataOffset++] = 0x00; // Translated keyword null separator
            textBytes.CopyTo(chunkBuffer, dataOffset);

            // CRC32
            ReadOnlySpan<byte> typeSpan = chunkBuffer.AsSpan(4, 4);
            ReadOnlySpan<byte> dataSpan = chunkBuffer.AsSpan(8, dataLength);
            uint crc = CalculateCrc32(typeSpan, dataSpan);
            BinaryPrimitives.WriteUInt32BigEndian(chunkBuffer.AsSpan(8 + dataLength, 4), crc);

            return chunkBuffer;
        }

        /// <summary>
        /// Atomically injects or replaces an iTXt "Description" chunk containing the photo metadata JSON into a PNG file.
        /// </summary>
        public static bool InjectPhotoMetadata(string filePath, MetaDataImageReader.PhotoMetadata metadata)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath) || metadata == null)
                return false;

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false
            };
            string jsonString = JsonSerializer.Serialize(metadata, options);
            return InjectTextChunk(filePath, "Description", jsonString);
        }

        /// <summary>
        /// Atomically injects or replaces an iTXt chunk with the given keyword and text into a PNG file.
        /// Uses streaming copy with atomic temp file replacement to guarantee zero file corruption and low memory usage.
        /// </summary>
        public static bool InjectTextChunk(string filePath, string keyword, string text)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return false;

            string tempFile = filePath + ".vrcgm_tmp";

            try
            {
                byte[] newChunkBytes = BuildITXtChunk(keyword, text);
                bool chunkInserted = false;

                const int bufferSize = 64 * 1024;
                byte[] transferBuffer = ArrayPool<byte>.Shared.Rent(bufferSize);

                try
                {
                    using (var src = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize))
                    using (var dst = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize))
                    {
                        // Verify PNG Signature
                        Span<byte> sig = stackalloc byte[8];
                        if (src.Read(sig) < 8 || !sig.SequenceEqual(PngSignature))
                            return false;

                        dst.Write(PngSignature, 0, 8);

                        Span<byte> header = stackalloc byte[8];
                        Span<byte> crcBuf = stackalloc byte[4];

                        while (src.Position < src.Length)
                        {
                            if (src.Read(header) < 8) break;

                            uint length = BinaryPrimitives.ReadUInt32BigEndian(header.Slice(0, 4));
                            string chunkType = Encoding.ASCII.GetString(header.Slice(4, 4));

                            // If we hit IDAT or IEND and haven't inserted the new chunk yet, insert it right before!
                            if (!chunkInserted && (chunkType == "IDAT" || chunkType == "IEND"))
                            {
                                dst.Write(newChunkBytes, 0, newChunkBytes.Length);
                                chunkInserted = true;
                            }

                            // If this chunk is an existing iTXt chunk with the SAME keyword, skip it to avoid duplicates
                            if (chunkType == "iTXt" && length < 5 * 1024 * 1024)
                            {
                                byte[] candidateData = new byte[length];
                                src.ReadExactly(candidateData, 0, (int)length);
                                src.ReadExactly(crcBuf);

                                int nullIdx = Array.IndexOf(candidateData, (byte)0);
                                if (nullIdx > 0 && nullIdx <= 79)
                                {
                                    string existingKeyword = Encoding.UTF8.GetString(candidateData, 0, nullIdx);
                                    if (string.Equals(existingKeyword, keyword, StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Skip old version of this metadata chunk
                                        continue;
                                    }
                                }

                                // Not our keyword, write it to destination
                                dst.Write(header);
                                dst.Write(candidateData, 0, candidateData.Length);
                                dst.Write(crcBuf);
                                continue;
                            }

                            // Write chunk header (length + type)
                            dst.Write(header);

                            // Copy chunk data in stream buffers
                            long remaining = length;
                            while (remaining > 0)
                            {
                                int toRead = (int)Math.Min(remaining, transferBuffer.Length);
                                int bytesRead = src.Read(transferBuffer, 0, toRead);
                                if (bytesRead <= 0) break;
                                dst.Write(transferBuffer, 0, bytesRead);
                                remaining -= bytesRead;
                            }

                            // Copy CRC
                            src.ReadExactly(crcBuf);
                            dst.Write(crcBuf);
                        }

                        // Fallback: If somehow IDAT wasn't found before end, append before close
                        if (!chunkInserted)
                        {
                            dst.Write(newChunkBytes, 0, newChunkBytes.Length);
                            chunkInserted = true;
                        }
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(transferBuffer);
                }

                // Replace destination file atomically with retry in case another process (e.g. VRCX, thumbnailer) has a transient lock
                bool replaced = false;
                for (int attempt = 0; attempt < 5; attempt++)
                {
                    try
                    {
                        File.Move(tempFile, filePath, overwrite: true);
                        replaced = true;
                        break;
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(100);
                    }
                }

                if (!replaced)
                {
                    try { if (File.Exists(tempFile)) File.Delete(tempFile); } catch { }
                    return false;
                }

                return true;
            }
            catch
            {
                try
                {
                    if (File.Exists(tempFile))
                        File.Delete(tempFile);
                }
                catch { }

                return false;
            }
        }
    }
}
