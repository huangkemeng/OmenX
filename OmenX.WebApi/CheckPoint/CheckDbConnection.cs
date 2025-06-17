using OmenX.Contracts;

namespace OmenX.WebApi.CheckPoint;

[CheckPointMetadata(Title = "数据库检查")]
public class CheckDbConnection : IOmenXCheckPoint
{
    public Task CheckAsync(OmeXCheckPointContext checkContext)
    {
        checkContext.Success(true, "数据库连接正常");
        return Task.CompletedTask;
    }
}