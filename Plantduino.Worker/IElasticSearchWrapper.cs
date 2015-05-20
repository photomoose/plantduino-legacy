using Worker;

namespace Rumr.Plantduino.Worker
{
    public interface IElasticSearchWrapper
    {
        void Index(string index, string type, object body);
    }
}