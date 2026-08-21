using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Api.Controllers;
using ToDoList.Data;
using ToDoList.Models;
using ToDoList.Repositories;
using ToDoList.Security;

namespace ToDoList.Api.Tests
{
    public class TasksControllerTests
    {
        private TodoContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new TodoContext(options);
        }

        private TaskController CreateController(TodoContext context, int userId)
        {
            var taskRepository = new TaskRepository(context);
            var taskController = new TaskController(taskRepository);
            ControllerTestHelper.SetUser(taskController, userId);
            return taskController;
        }

        [Fact]
        public void GetAll_ReturnsTasksForLoggedUser()
        {
            var context = CreateInMemoryContext();
            var taskController = CreateController(context, 1);
            var userRepository = new UserRepository(context);
            var user = userRepository.Register("user", "1234");
            var taskRepository = new TaskRepository(context);

            taskRepository.Add("mleko", Models.Priority.Low, user.Id);
            var result = taskController.GetAll() as OkObjectResult;

            Assert.NotNull(result);
            var taskList = result.Value as List<TaskItem>;
            Assert.Single(taskList);
        }
        [Fact]
        public void GetAll_ReturnsForLoggedUser()
        {
            var context = CreateInMemoryContext();
            var taskController = CreateController(context, 1);
            var userRepository = new UserRepository(context);
            var user = userRepository.Register("user", "1234");
            var taskRepository = new TaskRepository(context);

            taskRepository.Add("mleko", Models.Priority.Low, user.Id);
            var result = taskController.GetAll() as OkObjectResult;

            Assert.NotNull(result);
            var taskList = result.Value as List<TaskItem>;
            Assert.Single(taskList);
        }
        [Fact]
        public void Add_ReturnsOk_WhenInputIsCorrect()
        {
            var context = CreateInMemoryContext();
            var taskController = CreateController(context, 1);

            var result = taskController.AddTask(new TaskController.TaskRequest { Text = "text", Priority = Priority.Low });

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Add_ReturnsBadRequest_WhenInputIsWrong()
        {
            var context = CreateInMemoryContext();
            var taskController = CreateController(context, 1);

            var resultBadText = taskController.AddTask(new TaskController.TaskRequest { Text = "", Priority = Priority.Low });
            var resultBadPriority = taskController.AddTask(new TaskController.TaskRequest { Text = "text", Priority = (Priority)99 });

            Assert.IsType<BadRequestObjectResult>(resultBadText);
            Assert.IsType<BadRequestObjectResult>(resultBadPriority);
        }

        [Fact]
        public void Remove_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            var context = CreateInMemoryContext();
            var taskController = CreateController(context, 1);

            var result = taskController.RemoveTask(3);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void Remove_ReturnsOk_WhenTaskDeletedSuccesfully()
        {
            var context = CreateInMemoryContext();
            var taskRepository = new TaskRepository(context);
            var taskController = CreateController(context, 1);

            var taskId = taskRepository.Add("text", Priority.Low, 1);
            var result = taskController.RemoveTask(taskId);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Remove_ReturnsNotFound_WhenTaskBelongsToDifferentUser()
        {
            var context = CreateInMemoryContext();
            var taskRepository = new TaskRepository(context);
            var taskController = CreateController(context, 1);

            var taskId = taskRepository.Add("text", Priority.Low, 2);
            var result = taskController.RemoveTask(taskId);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void UpdateDone_ReturnsBadRequest_WhenDoneIsNull()
        {
            var context = CreateInMemoryContext();
            var taskRepository = new TaskRepository(context);
            int id = taskRepository.Add("text", Priority.Low, 1);
            var taskController = CreateController(context, 1);

            var result = taskController.UpdateDone(id, new TaskController.UpdateDoneRequest { Done = null });

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
