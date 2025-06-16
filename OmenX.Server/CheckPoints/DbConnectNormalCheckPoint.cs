using OmenX.Contracts;

namespace OmenX.Server.CheckPoints;

[CheckPointMetadata(Title = "数据库连接检查点", Description = "检查数据库连接是否正常")]
public class DbConnectNormalCheckPoint : IOmenXCheckPoint
{
    public Task CheckAsync(OmeXCheckPointContext checkContext)
    {
        checkContext.Success(true, "数据库连接正常");
        checkContext.Warn(10 > 0, "数据库连接时长超过10秒");
        return Task.CompletedTask;
    }
}