/*
 * IMPLEMENTED BY VRCGalleryManager
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace VRChat.API.Model
{
    /// <summary>
    /// Print
    /// </summary>
    [DataContract(Name = "Print")]
    public partial class Print : IEquatable<Print>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Print" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected Print() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Print" /> class.
        /// </summary>
        /// <param name="authorId">L'ID dell'autore (richiesto).</param>
        /// <param name="authorName">Il nome dell'autore (richiesto).</param>
        /// <param name="createdAt">Data di creazione (richiesto).</param>
        /// <param name="files">I file associati, rappresentati da un oggetto PrintFile (richiesto).</param>
        /// <param name="id">L'ID della print (richiesto).</param>
        /// <param name="note">La nota (richiesto).</param>
        /// <param name="ownerId">L'ID del proprietario (richiesto).</param>
        /// <param name="timestamp">Timestamp della print (richiesto).</param>
        /// <param name="worldId">L'ID del mondo (richiesto).</param>
        /// <param name="worldName">Il nome del mondo (richiesto).</param>
        public Print(
            string authorId,
            string authorName,
            DateTime createdAt,
            PrintFile files,
            string id,
            string note,
            string ownerId,
            DateTime timestamp,
            string worldId,
            string worldName)
        {
            // Verifica eventuali parametri obbligatori qui se necessario.
            this.AuthorId = authorId;
            this.AuthorName = authorName;
            this.CreatedAt = createdAt;
            this.Files = files;
            this.Id = id;
            this.Note = note;
            this.OwnerId = ownerId;
            this.Timestamp = timestamp;
            this.WorldId = worldId;
            this.WorldName = worldName;
        }

        /// <summary>
        /// Gets or Sets AuthorId
        /// </summary>
        [DataMember(Name = "authorId", IsRequired = true, EmitDefaultValue = true)]
        public string AuthorId { get; set; }

        /// <summary>
        /// Gets or Sets AuthorName
        /// </summary>
        [DataMember(Name = "authorName", IsRequired = true, EmitDefaultValue = true)]
        public string AuthorName { get; set; }

        /// <summary>
        /// Gets or Sets CreatedAt
        /// </summary>
        [DataMember(Name = "createdAt", IsRequired = true, EmitDefaultValue = true)]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or Sets Files
        /// </summary>
        [DataMember(Name = "files", IsRequired = true, EmitDefaultValue = true)]
        public PrintFile Files { get; set; }

        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id", IsRequired = true, EmitDefaultValue = true)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or Sets Note
        /// </summary>
        [DataMember(Name = "note", IsRequired = true, EmitDefaultValue = true)]
        public string Note { get; set; }

        /// <summary>
        /// Gets or Sets OwnerId
        /// </summary>
        [DataMember(Name = "ownerId", IsRequired = true, EmitDefaultValue = true)]
        public string OwnerId { get; set; }

        /// <summary>
        /// Gets or Sets Timestamp
        /// </summary>
        [DataMember(Name = "timestamp", IsRequired = true, EmitDefaultValue = true)]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or Sets WorldId
        /// </summary>
        [DataMember(Name = "worldId", IsRequired = true, EmitDefaultValue = true)]
        public string WorldId { get; set; }

        /// <summary>
        /// Gets or Sets WorldName
        /// </summary>
        [DataMember(Name = "worldName", IsRequired = true, EmitDefaultValue = true)]
        public string WorldName { get; set; }

        /// <summary>
        /// Returns the string presentation of the object.
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Print {\n");
            sb.Append("  AuthorId: ").Append(AuthorId).Append("\n");
            sb.Append("  AuthorName: ").Append(AuthorName).Append("\n");
            sb.Append("  CreatedAt: ").Append(CreatedAt).Append("\n");
            sb.Append("  Files: ").Append(Files).Append("\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  Note: ").Append(Note).Append("\n");
            sb.Append("  OwnerId: ").Append(OwnerId).Append("\n");
            sb.Append("  Timestamp: ").Append(Timestamp).Append("\n");
            sb.Append("  WorldId: ").Append(WorldId).Append("\n");
            sb.Append("  WorldName: ").Append(WorldName).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object.
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal.
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as Print);
        }

        /// <summary>
        /// Returns true if Print instances are equal.
        /// </summary>
        /// <param name="input">Instance of Print to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Print input)
        {
            if (input == null)
                return false;

            return
                (
                    this.AuthorId == input.AuthorId ||
                    (this.AuthorId != null && this.AuthorId.Equals(input.AuthorId))
                ) &&
                (
                    this.AuthorName == input.AuthorName ||
                    (this.AuthorName != null && this.AuthorName.Equals(input.AuthorName))
                ) &&
                (
                    this.CreatedAt == input.CreatedAt ||
                    this.CreatedAt.Equals(input.CreatedAt)
                ) &&
                (
                    this.Files == input.Files ||
                    (this.Files != null && this.Files.Equals(input.Files))
                ) &&
                (
                    this.Id == input.Id ||
                    (this.Id != null && this.Id.Equals(input.Id))
                ) &&
                (
                    this.Note == input.Note ||
                    (this.Note != null && this.Note.Equals(input.Note))
                ) &&
                (
                    this.OwnerId == input.OwnerId ||
                    (this.OwnerId != null && this.OwnerId.Equals(input.OwnerId))
                ) &&
                (
                    this.Timestamp == input.Timestamp ||
                    this.Timestamp.Equals(input.Timestamp)
                ) &&
                (
                    this.WorldId == input.WorldId ||
                    (this.WorldId != null && this.WorldId.Equals(input.WorldId))
                ) &&
                (
                    this.WorldName == input.WorldName ||
                    (this.WorldName != null && this.WorldName.Equals(input.WorldName))
                );
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow è consentito, si avvolge
            {
                int hashCode = 41;
                if (this.AuthorId != null)
                    hashCode = hashCode * 59 + this.AuthorId.GetHashCode();
                if (this.AuthorName != null)
                    hashCode = hashCode * 59 + this.AuthorName.GetHashCode();
                hashCode = hashCode * 59 + this.CreatedAt.GetHashCode();
                if (this.Files != null)
                    hashCode = hashCode * 59 + this.Files.GetHashCode();
                if (this.Id != null)
                    hashCode = hashCode * 59 + this.Id.GetHashCode();
                if (this.Note != null)
                    hashCode = hashCode * 59 + this.Note.GetHashCode();
                if (this.OwnerId != null)
                    hashCode = hashCode * 59 + this.OwnerId.GetHashCode();
                hashCode = hashCode * 59 + this.Timestamp.GetHashCode();
                if (this.WorldId != null)
                    hashCode = hashCode * 59 + this.WorldId.GetHashCode();
                if (this.WorldName != null)
                    hashCode = hashCode * 59 + this.WorldName.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Validates all properties of the instance.
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Inserisci qui eventuali controlli di validazione
            yield break;
        }
    }

    /// <summary>
    /// PrintFile
    /// </summary>
    [DataContract(Name = "PrintFile")]
    public partial class PrintFile : IEquatable<PrintFile>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrintFile" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected PrintFile() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintFile" /> class.
        /// </summary>
        /// <param name="fileId">L'ID del file (richiesto).</param>
        /// <param name="image">L'URL dell'immagine (richiesto).</param>
        public PrintFile(string fileId, string image)
        {
            this.FileId = fileId;
            this.Image = image;
        }

        /// <summary>
        /// Gets or Sets FileId
        /// </summary>
        [DataMember(Name = "fileId", IsRequired = true, EmitDefaultValue = true)]
        public string FileId { get; set; }

        /// <summary>
        /// Gets or Sets Image
        /// </summary>
        [DataMember(Name = "image", IsRequired = true, EmitDefaultValue = true)]
        public string Image { get; set; }

        /// <summary>
        /// Returns the string presentation of the object.
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class PrintFile {\n");
            sb.Append("  FileId: ").Append(FileId).Append("\n");
            sb.Append("  Image: ").Append(Image).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object.
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal.
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as PrintFile);
        }

        /// <summary>
        /// Returns true if PrintFile instances are equal.
        /// </summary>
        /// <param name="input">Instance of PrintFile to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(PrintFile input)
        {
            if (input == null)
                return false;

            return
                (
                    this.FileId == input.FileId ||
                    (this.FileId != null && this.FileId.Equals(input.FileId))
                ) &&
                (
                    this.Image == input.Image ||
                    (this.Image != null && this.Image.Equals(input.Image))
                );
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 41;
                if (this.FileId != null)
                    hashCode = hashCode * 59 + this.FileId.GetHashCode();
                if (this.Image != null)
                    hashCode = hashCode * 59 + this.Image.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Validates all properties of the instance.
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Inserisci qui eventuali controlli di validazione
            yield break;
        }
    }
}
