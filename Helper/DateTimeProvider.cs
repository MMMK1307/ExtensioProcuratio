using ExtensioProcuratio.Helper.Interfaces;

namespace ExtensioProcuratio.Helper
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetBrazil()
        {
            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(GetUtc(), cstZone);
        }

        public DateTime GetUtc()
        {
            return DateTime.UtcNow;
        }
    }
}