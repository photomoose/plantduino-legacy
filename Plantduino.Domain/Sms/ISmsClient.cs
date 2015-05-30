namespace Rumr.Plantduino.Domain.Sms
{
    public interface ISmsClient
    {
        void Send(string from, string to, string text);
    }
}