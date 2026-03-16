using BookSwap.Aggregator.Controllers;
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
    }
}