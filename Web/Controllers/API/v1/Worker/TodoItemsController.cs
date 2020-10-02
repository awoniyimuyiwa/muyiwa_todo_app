using Application.Services.Abstracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Web.Extensions;
using Web.Utils;

namespace Web.Controllers.API.v1.Worker
{
    /// <summary>
    /// Handles todo-item requests for administrative tasks 
    /// from non browser based clients like background workers on other servers
    /// </summary>
    /// <remarks>
    /// The endpoints on this contoller use the bearer authentication scheme and no csrf protection. 
    /// </remarks>
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiVersion("1.0")]
    [Route("api")] // The /worker path should have been included in this attribute but it was moved to actions to get past swagger's unique method/path restriction
    [Route("api/v{version:apiVersion}")]
    public class TodoItemsController : ControllerBase
    {
        readonly ITodoItemService TodoItemService;
       
        /// <summary>
        /// Initializes fields
        /// </summary>
        /// <param name="todoItemService"></param>
        public TodoItemsController(ITodoItemService todoItemService)
        {
            TodoItemService = todoItemService;
        }

        /// <summary>Returns list of todo-items</summary>
        /// <param name="search">The search string</param>
        /// <param name="page">The page number</param>
        /// <param name="pageSize">The maximum number of items per page</param>
        /// <param name="dateRangeString">String in the format: dd MMMM yyyy-dd MMMM yyyy</param>
        /// <response code="200">When there are no errors</response>
        /// <response code="400">When validation of any of the query string parameters fails</response>
        /// <response code="401">When authentication fails</response>
        /// <response code="403">When the access token doesn't have worker.todoitem scope</response>
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = Constants.AuthorizationPolicyNames.WorkerTodoItemScope)]
        [HttpGet("worker/todo-items")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Index(
            string search = null,
            int page = 1,
            [FromQuery(Name = "page_size")] int pageSize = 15,
            [FromQuery(Name = "date_range")] string dateRangeString = null) 
        {
            var dateRange = this.ValidateDateRange(dateRangeString, setDefaults: true);
            
            var todoItemDtos = await TodoItemService.TodoItemRepository.FilterBy(
                search,
                page,
                pageSize,
                todoItem => todoItem.CreatedAt >= dateRange.Min && todoItem.CreatedAt <= dateRange.Max);

            var responseBody = this.ListResponseBody(todoItemDtos, dateRange);

            return Ok(responseBody);
        }

        /// <summary>Returns details of a todo-item</summary>
        /// <param name="slug">Slug for identifying the todo-item</param>
        /// <response code="200">When there are no errors</response>
        /// <response code="401">When authentication fails</response>
        /// <response code="403">When the access token doesn't have worker.todoitem scope</response>
        /// <response code="404">When a todo-item with the slug specified doesn't exist</response>        
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = Constants.AuthorizationPolicyNames.WorkerTodoItemScope)]
        [HttpGet("worker/todo-items/{slug}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> View(string slug)
        {
            var todoItem = await TodoItemService.TodoItemRepository.FindOneBy(todoItem => todoItem.Slug == slug);
            if (todoItem == null) { return NotFound(); }

            var responseBody = new
            {
                Data = todoItem.ToDto()
            };

            return Ok(responseBody);
        }
    }
}
