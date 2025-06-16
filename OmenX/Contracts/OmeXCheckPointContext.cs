using System.Collections.Generic;

namespace OmenX.Contracts
{
    public class OmeXCheckPointContext
    {
        internal List<OmeXCheckResult> Results { get; set; } = new List<OmeXCheckResult>();

        public void Warn(bool predication, string message = null)
        {
            if (predication)
            {
                Results.Add(OmeXCheckResult.Warn(message));
            }
        }

        public void Error(bool predication, string message = null)
        {
            if (predication)
            {
                Results.Add(OmeXCheckResult.Error(message));
            }
        }

        public void Success(bool predication, string message = null)
        {
            if (predication)
            {
                Results.Add(OmeXCheckResult.Success(message));
            }
        }
    }
}