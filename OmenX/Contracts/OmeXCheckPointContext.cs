using System.Collections.Generic;

namespace OmenX.Contracts
{
    public class OmeXCheckPointContext
    {
        internal List<OmeXCheckResult> CheckResults { get; set; } = new List<OmeXCheckResult>();

        public OmeXCheckPointContext Success(string message = null)
        {
            CheckResults.Add(OmeXCheckResult.Success(message));
            return this;
        }

        public OmeXCheckPointContext Warn(string message = null)
        {
            CheckResults.Add(OmeXCheckResult.Warn(message));
            return this;
        }

        public OmeXCheckPointContext Error(string message = null)
        {
            CheckResults.Add(OmeXCheckResult.Error(message));
            return this;
        }

        public OmeXCheckPointContext Success(bool predication, string message = null)
        {
            if (predication)
            {
                Success(message);
            }

            return this;
        }

        public OmeXCheckPointContext Warn(bool predication, string message = null)
        {
            if (predication)
            {
                Warn(message);
            }

            return this;
        }

        public OmeXCheckPointContext Error(bool predication, string message = null)
        {
            if (predication)
            {
                Error(message);
            }

            return this;
        }
    }
}