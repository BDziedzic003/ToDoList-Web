using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ToDoList.Api.Controllers;

namespace ToDoList.Api.Tests
{
    public class TaskControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient client;

        public TaskControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            this.client = factory.CreateClient();
        }
        private string UniqueUsername() => $"user_{Guid.NewGuid()}";
        private async Task<string> RegisterAndLoginAsync(string username, string password)
        {
            var registerResponse = await client.PostAsJsonAsync("/api/Auth/register", new { username, password });
            var loginResponse = await client.PostAsJsonAsync("/api/Auth/login", new { username, password });
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResult>();
            return loginResult.Token;
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WhenTokenProvided()
        {
            var token = await RegisterAndLoginAsync(UniqueUsername(), "testpass");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = await client.GetAsync("/api/Task");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task GetAll_ReturnsUnauthorized_WhenNoTokenProvided()
        {
            var result = await client.GetAsync("/api/Task");

            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }
        [Fact]
        public async Task GetAll_ReturnsUnauthorized_WhenTokenIsInvalid()
        {
            var token = await RegisterAndLoginAsync(UniqueUsername(), "testpass");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "falsetoken");

            var result = await client.GetAsync("/api/Task");
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }
        [Fact]
        public async Task Add_ReturnsOk_WhenValidTokenAndValidData()
        {
            var token = await RegisterAndLoginAsync(UniqueUsername(), "testpass");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = await client.PostAsJsonAsync("api/Task", new TaskController.TaskRequest { Text = "mleko", Priority = Models.Priority.Low });
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
        [Fact]
        public async Task Add_ReturnsBadRequest_WhenInvalidData()
        {
            var token = await RegisterAndLoginAsync(UniqueUsername(), "testpass");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = await client.PostAsJsonAsync("api/Task", new TaskController.TaskRequest { Text = "", Priority = Models.Priority.Low });
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
        [Fact]
        public async Task Remove_ReturnsOk_WhenTaskDeletedSuccessfully()
        {
            var token = await RegisterAndLoginAsync(UniqueUsername(), "testpass");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var postResult = await client.PostAsJsonAsync("api/Task", new TaskController.TaskRequest { Text = "mleko", Priority = Models.Priority.Low });
            var addedTask = await postResult.Content.ReadFromJsonAsync<AddTaskResult>();
            var deleteResult = await client.DeleteAsync($"api/Task/{addedTask.Id}");

            Assert.Equal(HttpStatusCode.OK, deleteResult.StatusCode);
        }
        [Fact]
        public async Task Remove_ReturnsNotFound_WhenTaskBelongsToAnotherUser()
        {
            var token = await RegisterAndLoginAsync(UniqueUsername(), "testpass");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var postResult = await client.PostAsJsonAsync("api/Task", new TaskController.TaskRequest { Text = "mleko", Priority = Models.Priority.Low });
            var addedTask = await postResult.Content.ReadFromJsonAsync<AddTaskResult>();
            token = await RegisterAndLoginAsync(UniqueUsername(), "testpass"); 
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var deleteResult = await client.DeleteAsync($"api/Task/{addedTask.Id}"); 

            Assert.Equal(HttpStatusCode.NotFound, deleteResult.StatusCode);
        }

        public class LoginResult
        {
            public string Token { get; set; }
        }

        public class AddTaskResult
        {
            public int Id { get; set; }
        }
    }
}
