using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoList.Models;
using ToDoList.Repositories;


namespace ToDoList.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly TaskRepository taskRepository;

        public TaskController(TaskRepository taskRepository)
        {
            this.taskRepository = taskRepository;
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(idClaim!.Value);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var taskList = taskRepository.GetAll(GetCurrentUserId());
            return Ok(taskList);
        }

        [HttpPost]
        public IActionResult AddTask([FromBody] TaskRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest("Task text is required");

            if (!Enum.IsDefined(typeof(Priority), request.Priority))
                return BadRequest("Invalid priority value");

            int id = taskRepository.Add(request.Text, request.Priority, GetCurrentUserId());
            return Ok(new { id });
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveTask(int id)
        {
            bool successful = taskRepository.Remove(id, GetCurrentUserId());
            if (!successful) 
                return NotFound("Task not found");
            return Ok("Task deleted"); 
        }

        [HttpPut("{id}/text")]
        public IActionResult UpdateText(int id, [FromBody] UpdateTextRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest("Text is required");
            bool successful = taskRepository.UpdateText(id, request.Text, GetCurrentUserId());
            if (!successful) 
                return NotFound("Task not found");
            return Ok("Task text updated");
        }

        [HttpPut("{id}/priority")]
        public IActionResult UpdatePriority(int id, [FromBody] UpdatePriorityRequest request)
        {
            if (!Enum.IsDefined(typeof(Priority), request.Priority))
                return BadRequest("Property value is required");
            bool successful = taskRepository.UpdatePriority(id, request.Priority, GetCurrentUserId()); 
            if (!successful)
                return NotFound("Task not found");
            return Ok("Task priority updated");
        }
        [HttpPut("{id}/done")]
        public IActionResult UpdateDone(int id, [FromBody] UpdateDoneRequest request)
        {
            if (request.Done == null)
                return BadRequest("Done value is required");
            bool successful = taskRepository.UpdateDone(id, request.Done.Value, GetCurrentUserId());
            if (!successful)
                return NotFound("Task not found");
            return Ok("Task status updated");
        }

        public class TaskRequest
        {
            public string Text { get; set; }
            public Priority Priority { get; set; }
        }

        public class UpdateTextRequest
        {
            public string Text { get; set; }
        }

        public class UpdatePriorityRequest
        {
            public Priority Priority { get; set; }
        }

        public class UpdateDoneRequest
        {
            public bool? Done { get; set; }
        }
    }
}
