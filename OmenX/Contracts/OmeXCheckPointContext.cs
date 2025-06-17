using System.Collections.Generic;

namespace OmenX.Contracts
{
    public class OmeXCheckPointContext
    {
        internal List<OmeXCheckResult> Results { get; set; } = new List<OmeXCheckResult>();

        public OmeXCheckPointContext Warn(bool predication, string message = null)
        {
            if (predication)
            {
                Results.Add(OmeXCheckResult.Warn(message));
            }

            return this;
        }

        public OmeXCheckPointContext Error(bool predication, string message = null)
        {
            if (predication)
            {
                Results.Add(OmeXCheckResult.Error(message));
            }

            return this;
        }

        public OmeXCheckPointContext Success(bool predication, string message = null)
        {
            if (predication)
            {
                Results.Add(OmeXCheckResult.Success(message));
            }

            return this;
        }
    }
}