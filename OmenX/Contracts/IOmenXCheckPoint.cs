using System.Threading.Tasks;

namespace OmenX.Contracts
{
    public interface IOmenXCheckPoint
    {
        Task CheckAsync(OmeXCheckPointContext checkContext);
    }
}