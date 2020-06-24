using System.ComponentModel.DataAnnotations;
using System.Net;

namespace WebApi.Models
{
    public class S3Response
    {
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
    }
}
