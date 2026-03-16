namespace BookSwap.Aggregator.Helpers
{
    public static class CityValidator
    {
        public static bool IsValidCityName(string city)
        {
            if (string.IsNullOrWhiteSpace(city)) return false;

            return city.Length >= 2 && !city.Any(char.IsDigit);
        }
    }
}