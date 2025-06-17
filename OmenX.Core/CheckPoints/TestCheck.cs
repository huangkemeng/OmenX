using OmenX.Contracts;

namespace OmenX.Core.CheckPoints;

[CheckPointMetadata(Title = "Test check", Description = "Test check description")]
public class TestCheck : IOmenXCheckPoint
{
    public Task CheckAsync(OmeXCheckPointContext checkContext)
    {
        checkContext.Success(true, "test pass");
        return Task.CompletedTask;
    }
}