using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace BookSwap.Aggregator
{
    public class CalendarManager
    {
        private static string[] Scopes = { CalendarService.Scope.Calendar };
        private static string ApplicationName = "Virtual Assistant Calendar";
        private CalendarService _service;

        public async Task AuthenticateAsync()
        {
            UserCredential credential;

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true));
            }

            _service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public async Task<string> CreateEventFromTextAsync(string userInput)
        {
            try
            {
                var timeMatch = Regex.Match(userInput, @"\b([0-1]?[0-9]|2[0-3]):[0-5][0-9]\b");
                if (!timeMatch.Success)
                    return "Не можу знайти час. Будь ласка, вкажіть час у форматі, наприклад, 14:00.";

                DateTime eventDate = GetDateFromText(userInput);
                TimeSpan eventTime = TimeSpan.Parse(timeMatch.Value);

                DateTime startDateTime = eventDate.Date + eventTime;
                DateTime endDateTime = startDateTime.AddHours(1); 

                string summary = "Нове нагадування";
                var titleMatch = Regex.Match(userInput, @"(?:подію|нагадування)\s+(.*?)\s+(?:на|о|об)", RegexOptions.IgnoreCase);
                if (titleMatch.Success) summary = titleMatch.Groups[1].Value.Trim();

                Event newEvent = new Event()
                {
                    Summary = summary,
                    Start = new EventDateTime() { DateTimeDateTimeOffset = startDateTime, TimeZone = "Europe/Kiev" },
                    End = new EventDateTime() { DateTimeDateTimeOffset = endDateTime, TimeZone = "Europe/Kiev" }
                };

                EventsResource.InsertRequest request = _service.Events.Insert(newEvent, "primary");
                Event createdEvent = await request.ExecuteAsync();

                return $"Подію '{summary}' успішно створено на {startDateTime:dd.MM.yyyy HH:mm}!\nПосилання: {createdEvent.HtmlLink}";
            }
            catch (Exception ex)
            {
                return $"Помилка при створенні події: {ex.Message}";
            }
        }

        private DateTime GetDateFromText(string text)
        {
            text = text.ToLower();
            DateTime today = DateTime.Today;

            if (text.Contains("сьогодні")) return today;
            if (text.Contains("завтра")) return today.AddDays(1);
            if (text.Contains("післязавтра")) return today.AddDays(2);

            Dictionary<string, DayOfWeek> days = new Dictionary<string, DayOfWeek>
            {
                { "неділ", DayOfWeek.Sunday }, { "понеділ", DayOfWeek.Monday },
                { "вівтор", DayOfWeek.Tuesday }, { "серед", DayOfWeek.Wednesday },
                { "четвер", DayOfWeek.Thursday }, { "п'ятниц", DayOfWeek.Friday },
                { "субот", DayOfWeek.Saturday }
            };

            foreach (var day in days)
            {
                if (text.Contains(day.Key))
                {
                    int daysToAdd = ((int)day.Value - (int)today.DayOfWeek + 7) % 7;
                    if (daysToAdd == 0) daysToAdd = 7;
                    return today.AddDays(daysToAdd);
                }
            }

            return today;
        }

        public async Task<string> GetUpcomingEventsAsync(int maxEvents = 5)
        {
            try
            {
                EventsResource.ListRequest request = _service.Events.List("primary");
                request.TimeMinDateTimeOffset = DateTime.Now;
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.MaxResults = maxEvents;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                Events events = await request.ExecuteAsync();

                if (events.Items != null && events.Items.Count > 0)
                {
                    string result = "Ваші найближчі події:\n";
                    foreach (var eventItem in events.Items)
                    {
                        string when = eventItem.Start.DateTimeDateTimeOffset?.ToString("dd.MM.yyyy HH:mm") ?? "Весь день";
                        result += $"- {eventItem.Summary} ({when})\n";
                    }
                    return result;
                }
                else
                {
                    return "Найближчих подій не знайдено.";
                }
            }
            catch (Exception ex)
            {
                return $"Помилка при отриманні подій: {ex.Message}";
            }
        }
    }
}
