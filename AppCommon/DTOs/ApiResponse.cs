namespace AppCommon.DTOs
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public static ApiResponse<T> Success(T data) => new() { IsSuccess = true, Data = data };
        public static ApiResponse<T> Fail(string errorCode, string errorMessage) => new()
        {
            IsSuccess = false,
            ErrorCode = errorCode,
            ErrorMessage = errorMessage
        };
    }


    public static class ErrorCodes
    {
        public const string ValidationError = "VAL_001";
        public const string NotFound = "NF_001";
        public const string Unauthorized = "AUTH_001";
        public const string InternalServerError = "SRV_001";
        public const string BadRequest = "REQ_001";
        public const string Forbidden = "AUTH_002";
        public const string DuplicateEntry = "VAL_002";
        public const string InvalidOperation = "OPR_001";
    }


    public static class ErrorMessages
    {
        public const string SystemPropertyDeleteMessage = "SystemPropertyDeleteMessage";
    }
}
