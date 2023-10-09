namespace PlanePal.Model.Shared
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;

        public ServiceResponse()
        { }

        public ServiceResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public ServiceResponse(T data, bool success, string message)
        {
            Data = data;
            Success = success;
            Message = message;
        }

        public ServiceResponse(T data, bool success)
        {
            Data = data;
            Success = success;
        }
    }
}