namespace OmenX.Contracts
{
    public class OmeXCheckResult
    {
        private OmeXCheckResult(string result)
        {
            Result = result;
        }

        public string Result { get; set; }

        public string Message { get; set; }

        public static OmeXCheckResult Success(string message = null)
        {
            return new OmeXCheckResult("Success")
            {
                Message = message
            };
        }

        public static OmeXCheckResult Warn(string message = null)
        {
            return new OmeXCheckResult("Warning")
            {
                Message = message
            };
        }

        public static OmeXCheckResult Error(string message = null)
        {
            return new OmeXCheckResult("Error")
            {
                Message = message
            };
        }
    }
}