using System.Net;

namespace Application.DTOs.Cloud.AWS
{
    public class ResponseStatus
    {
        public string Message { get; set; } = string.Empty;
        public HttpStatusCode Status { get; set; }
    }
}