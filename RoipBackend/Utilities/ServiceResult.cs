using RoipBackend.Models;

namespace RoipBackend.Utilities
{
    public class ServiceResult<T>
    {
        public T Data { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }

        public string Error { get; set; }

        public int StatusCode { get; set; }

        public string RoipShekemStoreJWT { get; set; }        
    }
}
