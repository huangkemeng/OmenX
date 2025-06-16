using OmenX.Contracts;

namespace OmenX.Server.CheckPoints;

[CheckPointMetadata(Title = "慢SQL检查点", Description = "检查是否存在慢SQL")]
public class SlowSqlCheckPoint : IOmenXCheckPoint
{
    public Task CheckAsync(OmeXCheckPointContext checkContext)
    {
        checkContext.Error(10 > 0, "存在SQL执行时间大于10s的");
        return Task.CompletedTask;
    }
}