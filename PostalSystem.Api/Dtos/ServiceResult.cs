namespace PostalSystem.Api.Dtos
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; } // Now Data is specifically type T, not just 'object'

        public ServiceResult(bool isSuccess, string message, T? data = default)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
        }
    }

    // non-generic version for methods that don't return any data
    public class ServiceResult : ServiceResult<object>
    {
        public ServiceResult(
            bool isSuccess, 
            string message, 
            object? data = null
        ) : base(isSuccess, message, data) { }
    }
}