using System;

namespace VRCGalleryManager.Core.Api
{
    public class VRChatApiException : Exception
    {
        public int StatusCode { get; }
        public int ErrorCode => StatusCode;
        public string? RawContent { get; }

        public VRChatApiException(int statusCode, string message, string? rawContent = null)
            : base(message)
        {
            StatusCode = statusCode;
            RawContent = rawContent;
        }

        public VRChatApiException(int statusCode, string message, Exception innerException, string? rawContent = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            RawContent = rawContent;
        }
    }

    public class ApiException : VRChatApiException
    {
        public ApiException(int statusCode, string message, string? rawContent = null)
            : base(statusCode, message, rawContent)
        {
        }

        public ApiException(int statusCode, string message, Exception innerException, string? rawContent = null)
            : base(statusCode, message, innerException, rawContent)
        {
        }
    }
}
