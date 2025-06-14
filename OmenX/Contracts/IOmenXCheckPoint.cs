namespace OmenX.Contracts;

public interface IOmenXCheckPoint
{
    public Task CheckAsync(OmeXCheckPointContext checkPointContext);
}