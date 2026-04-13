using System.Text.RegularExpressions;

namespace BookSwap.Aggregator.Services
{
    public class ReminderParser
    {
        public (DateTime? time, string? reminderText) Parse(string input)
        {
            var timeMatch = Regex.Match(input, @"\b(\d{1,2})[:.-](\d{2})\b");
            if (!timeMatch.Success) return (null, null);

            int hours = int.Parse(timeMatch.Groups[1].Value);
            int minutes = int.Parse(timeMatch.Groups[2].Value);

            DateTime targetDate = DateTime.Today;

            if (input.Contains("завтра", StringComparison.OrdinalIgnoreCase))
            {
                targetDate = targetDate.AddDays(1);
            }

            var finalDateTime = targetDate.AddHours(hours).AddMinutes(minutes);

            string cleanText = input.Replace("Нагадай", "").Trim();

            return (finalDateTime, cleanText);
        }
    }
}