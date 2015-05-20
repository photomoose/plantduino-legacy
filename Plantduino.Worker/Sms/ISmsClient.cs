namespace Rumr.Plantduino.Worker.Sms
{
    public interface ISmsClient
    {
        void Send(string from, string to, string text);
    }
}