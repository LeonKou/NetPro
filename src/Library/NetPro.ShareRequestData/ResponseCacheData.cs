namespace NetPro.ShareRequestBody
{
    public class ResponseCacheData
    {
        /// <summary>
        /// StatusCode
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// ContentType
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Body
        /// </summary>
        public string Body { get; set; }
    }

    public class RequestCacheData
    {
        /// <summary>
        /// Body
        /// </summary>
        public string Body { get; set; }
    }
}
