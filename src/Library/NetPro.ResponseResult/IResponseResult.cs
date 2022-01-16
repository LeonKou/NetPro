namespace NetPro.ResponseResult
{
    public interface IResponseResult
    {
        public string Message { get; set; }

        public string Result { get; set; }

        public string Code { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BaseModel : IResponseResult
    {
        public BaseModel(string message, string result, string code)
        {
            Message = message;
            Result = result;
            Code = code;
        }
        public string Message { get; set; }
        public string Result { get; set; }
        public string Code { get; set; }
    }
}
