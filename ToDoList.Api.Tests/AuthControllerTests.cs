using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Api.Controllers;
using ToDoList.Data;
using ToDoList.Repositories;
using ToDoList.Security;

namespace ToDoList.Api.Tests
{
    public class AuthControllerTests
    {
        private TodoContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new TodoContext(options);
        }

        private AuthController CreateController(TodoContext context)
        {
            var userRepository = new UserRepository(context);
            var tokenGenerator = new JwtTokenGenerator(
                key: "TestKeyMinimum32CharactersLongForHmacSha256",
                issuer: "TestIssuer",
                audience: "TestAudience");
            return new AuthController(userRepository, tokenGenerator);
        }

        [Fact]
        public void Register_ReturnsOk_WhenUsernameIsFree()
        {
            var context = CreateInMemoryContext();
            var controller = CreateController(context);

            var result = controller.Register(new RegisterRequest { Username = "test", Password = "testpass" });

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Register_ReturnsBadRequest_WhenUsernameTaken()
        {
            var context = CreateInMemoryContext();
            var controller = CreateController(context);
            controller.Register(new RegisterRequest { Username = "test", Password = "testpass" });

            var result = controller.Register(new RegisterRequest { Username = "test", Password = "testpass" });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Register_ReturnsBadRequest_WhenUsernameEmpty()
        {
            var context = CreateInMemoryContext();
            var controller = CreateController(context);

            var result = controller.Register(new RegisterRequest { Username = "", Password = "testpass" });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Register_ReturnsBadRequest_WhenPasswordEmpty()
        {
            var context = CreateInMemoryContext();
            var controller = CreateController(context);

            var result = controller.Register(new RegisterRequest { Username = "test", Password = "" });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Login_ReturnsOkWithToken_WhenCredentialsCorrect()
        {
            var context = CreateInMemoryContext();
            var controller = CreateController(context);
            controller.Register(new RegisterRequest { Username = "jan", Password = "haslo123" });

            var result = controller.Login(new LoginRequest { Username = "jan", Password = "haslo123" }) as OkObjectResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void Login_ReturnsUnauthorized_WhenPasswordWrong()
        {
            var context = CreateInMemoryContext();
            var controller = CreateController(context);
            controller.Register(new RegisterRequest { Username = "jan", Password = "haslo123" });

            var result = controller.Login(new LoginRequest { Username = "jan", Password = "zle" });

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public void Login_ReturnsUnauthorized_WhenLoginEmpty()
        {
            var context = CreateInMemoryContext();
            var controller = CreateController(context);
            controller.Register(new RegisterRequest { Username = "jan", Password = "haslo123" });

            var result = controller.Login(new LoginRequest { Username = "jan", Password = "zle" });

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public void Login_ReturnsUnauthorized_WhenPasswordEmpty()
        {
            var context = CreateInMemoryContext();
            var controller = CreateController(context);
            controller.Register(new RegisterRequest { Username = "jan", Password = "haslo123" });

            var result = controller.Login(new LoginRequest { Username = "jan", Password = "zle" });

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}