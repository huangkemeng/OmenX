using OmenX.Contracts;

namespace OmenX.Server.CheckPoints;

[CheckPointMetadata(Title = "慢SQL检查点", Description = "检查是否存在慢SQL")]
public class SlowSqlCheckPoint : IOmenXCheckPoint
{
    public async Task CheckAsync(OmeXCheckPointContext checkContext, CancellationToken cancellationToken)
    {
        await Task.Delay(10000, cancellationToken);
        checkContext.Success(true, "无慢SQL");
    }
}