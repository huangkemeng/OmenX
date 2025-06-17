using OmenX.Contracts;

namespace OmenX.Core.CheckPoints;

public class TestCheck : IOmenXCheckPoint
{
    public async Task CheckAsync(OmeXCheckPointContext checkContext)
    {
        checkContext.Success(true, "Good");
    }
}