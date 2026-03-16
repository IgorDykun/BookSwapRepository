using BookSwap.Aggregator.Controllers;
using BookSwap.Aggregator.Helpers;
using Xunit;

namespace BookSwap.Tests
{
    public class AggregatorDataTests
    {
        [Fact]
        public void UserProfileUpdate_ShouldStoreDataCorrectly()
        {
            var update = new ExternalDataController.UserProfileUpdate
            {
                UserId = 123456789,
                FavoriteCity = "Київ",
                Name = "Олексій"
            };

            Assert.Equal(123456789, update.UserId);
            Assert.Equal("Київ", update.FavoriteCity);
            Assert.Equal("Олексій", update.Name);
        }

        [Fact]
        public void UserRequest_ShouldHandleEmptyMessage()
        {
            var request = new ExternalDataController.UserRequest
            {
                UserId = 987,
                Message = ""
            };

            Assert.NotNull(request);
            Assert.Equal("", request.Message);
        }

        [Theory]
        [InlineData("Київ", true)]
        [InlineData("Львів", true)]
        [InlineData("К", false)] 
        [InlineData("Київ123", false)] 
        [InlineData("", false)] 
        public void CityValidator_ShouldValidateCorrectly(string city, bool expected)
        {
            var result = CityValidator.IsValidCityName(city);
            Assert.Equal(expected, result);
        }
    }
}