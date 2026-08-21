using System.Net;
using System.Net.Http.Json; 

namespace ToDoList.Api.Tests
{
    public class AuthControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient client;

        public AuthControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            this.client = factory.CreateClient();
        }
        private string UniqueUsername() => $"user_{Guid.NewGuid()}";

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenUsernameAlreadyTaken()
        {
            var username = UniqueUsername(); 
            await client.PostAsJsonAsync("/api/Auth/register", new { username, password = "haslo123" });

            var secondAttempt = await client.PostAsJsonAsync("/api/Auth/register", new { username, password = "inne_haslo" });

            Assert.Equal(HttpStatusCode.BadRequest, secondAttempt.StatusCode);
        }
    }
}
