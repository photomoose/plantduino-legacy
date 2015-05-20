namespace Rumr.Plantduino.Worker
{
    public interface ITwilioAccount
    {
        string AccountSid { get; }
        string AuthToken { get; }
    }
}