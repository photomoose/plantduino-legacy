namespace Plantduino.Infrastructure.Twilio
{
    public interface ITwilioAccount
    {
        string AccountSid { get; }
        string AuthToken { get; }
        bool IsSmsEnabled { get; }
    }
}