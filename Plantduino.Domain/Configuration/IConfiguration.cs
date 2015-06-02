namespace Rumr.Plantduino.Domain.Configuration
{
    public interface IConfiguration
    {
        double ColdSpellTemp { get; }
        string SmsFrom { get; }
        string SmsTo { get; }
    }
}