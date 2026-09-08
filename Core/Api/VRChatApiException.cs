using System;

namespace VRCGalleryManager.Core.Api
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }
        public int ErrorCode => StatusCode;
        public string? RawContent { get; }

        public ApiException(int statusCode, string message, string? rawContent = null)
            : base(message)
        {
            StatusCode = statusCode;
            RawContent = rawContent;
        }

        public ApiException(int statusCode, string message, Exception innerException, string? rawContent = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            RawContent = rawContent;
        }
    }

    public class VRChatApiException : ApiException
    {
        public VRChatApiException(int statusCode, string message, string? rawContent = null)
            : base(statusCode, message, rawContent)
        {
        }

        public VRChatApiException(int statusCode, string message, Exception innerException, string? rawContent = null)
            : base(statusCode, message, innerException, rawContent)
        {
        }
    }
}
