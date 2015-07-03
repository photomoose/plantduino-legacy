using System.Threading.Tasks;

namespace Rumr.Plantduino.Domain.Repositories
{
    public interface IColdSpellRepository
    {
        Task<ColdSpell> GetAsync(string deviceId, string sensorId);
        Task SaveAsync(ColdSpell coldSpell);
    }
}