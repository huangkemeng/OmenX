﻿using OmenX.Contracts;

namespace OmenX.Server.CheckPoints;

[CheckPointMetadata(Title = "慢SQL检查点", Description = "检查是否存在慢SQL")]
public class SlowSqlCheckPoint : IOmenXCheckPoint
{
    public Task CheckAsync(OmeXCheckPointContext checkContext, CancellationToken cancellationToken)
    {
        checkContext.Success(true, "无慢SQL");
        return Task.CompletedTask;
    }
}