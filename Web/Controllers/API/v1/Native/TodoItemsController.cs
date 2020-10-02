using Application.Services.Abstracts;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Web.Utils;
using Web.ViewModels.Transient.v1;

namespace Web.Controllers.API.v1.Native
{
    /// <summary>
    /// Handles todo-item requests for a user from non-browser native clients that can securely store access tokens
    /// </summary>
    /// <remarks>
    /// The endpoints on this contoller use the bearer authentication scheme and no csrf protection. 
    /// </remarks>
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiVersion("1.0")]
    [Route("api")] // The /native path should have been included in this attribute but it was moved to actions to get past swagger's unique method/path restriction
    [Route("api/v{version:apiVersion}")]
    public class TodoItemsController : TodoItemsControllerBase
    {
        /// <summary>
        /// Initializes fields
        /// </summary>
        /// <param name="todoItemService"></param>
        /// <param name="userService"></param>
        /// <param name="logger"></param>
        /// <param name="statusMessageLocalizer"></param>
        /// <param name="validationMessageLocalizer"></param>
        public TodoItemsController(
            ITodoItemService todoItemService, 
            IUserService userService,
            ILogger<TodoItemsController> logger,
            IStringLocalizer<Status> statusMessageLocalizer,
            IStringLocalizer<Validation> validationMessageLocalizer) : base(
                todoItemService, 
                userService,
                logger, 
                statusMessageLocalizer, 
                validationMessageLocalizer) {}

        /// <summary>Returns list of todo-items</summary>
        /// <param name="search">The search string</param>
        /// <param name="page">The page number</param>
        /// <param name="pageSize">The maximum number of items per page</param>
        /// <param name="dateRangeString">String in the format: dd MMMM yyyy-dd MMMM yyyy</param>
        /// <response code="200">When there are no errors</response>
        /// <response code="400">
        /// 1) When no user is indicated via the accesss token 
        /// 2) When validation of any of the query string parameters fails
        /// </response>
        /// <response code="401">When authentication fails</response>
        /// <response code="403">When the access token doesn't have read.todoitem scope</response>
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = Constants.AuthorizationPolicyNames.ReadTodoItemScope)]
        [HttpGet("native/todo-items")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> NativeIndex(
            string search = null,
            int page = 1,
            [FromQuery(Name = "page_size")] int pageSize = 15,
            [FromQuery(Name = "date_range")] string dateRangeString = null) 
        {
            return await DoIndex(search, page, pageSize, dateRangeString);
        }

        /// <summary>Creates a new todo-item</summary>
        /// <param name="viewModel">The model for accepting form input and on which validation will be done</param>        
        /// <response code="201">When there are no errors</response>
        /// <response code="400">
        /// 1) When no user is indicated via the accesss token 
        /// 2) When input validation fails
        /// </response>
        /// <response code="401">When authentication fails</response>
        /// <response code="403">When the access token doesn't have write.todoitem scope</response>
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = Constants.AuthorizationPolicyNames.WriteTodoItemScope)]
        [HttpPost("native/todo-items")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([CustomizeValidator(Skip = true)] TodoItemViewModel viewModel)
        {
            return await DoCreate(viewModel, "/api/v1/native/todo-items");
        }

        /// <summary>Returns details of a todo-item</summary>
        /// <param name="slug">Slug for identifying the todo-item</param>
        /// <response code="200">When there are no errors</response>
        /// <response code="400">When no user is indicated via the accesss token</response>
        /// <response code="401">When authentication fails</response>
        /// <response code="403">When the access token doesn't have read.todoitem scope</response>        
        /// <response code="404">When the user indicated via the access token doesn't have a todo-item with the slug specified</response>        
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = Constants.AuthorizationPolicyNames.ReadTodoItemScope)]
        [HttpGet("native/todo-items/{slug}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> View(string slug)
        {
            return await DoView(slug);
        }

        /// <summary>Updates a todo-item</summary>
        /// <param name="slug">Slug for identifying the todo-item</param>
        /// <param name="viewModel">The model for accepting form input and on which validation will be done</param>
        /// <response code="200">When there are no errors</response>
        /// <response code="400">
        /// 1) When no user is indicated via the accesss token
        /// 2) When input validation fails
        /// </response>
        /// <response code="401">When authentication fails</response>
        /// <response code="403">When the access token doesn't have write.todoitem scope</response>        
        /// <response code="404">When the user indicated via the access token doesn't have a todo-item with the slug specified</response>        
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = Constants.AuthorizationPolicyNames.WriteTodoItemScope)]
        [HttpPut("native/todo-items/{slug}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(string slug, [CustomizeValidator(Skip = true)] TodoItemViewModel viewModel)
        {
            return await DoUpdate(slug, viewModel);
        }

        /// <summary>Marks a todo-item as completed</summary>
        /// <param name="slug">Slug for identifying the todo-item</param>
        /// <response code="200">When there are no errors</response>
        /// <response code="400">When no user is indicated via the accesss token</response>
        /// <response code="401">When authentication fails</response>
        /// <response code="403">When the access token doesn't have write.todoitem scope</response>        
        /// <response code="404">When the user indicated via the access token doesn't have a todo-item with the slug specified</response>        
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = Constants.AuthorizationPolicyNames.WriteTodoItemScope)]
        [HttpPut("native/todo-items/{slug}/mark-as-completed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsCompleted(string slug)
        {
            return await DoMarkAsCompleted(slug);
        }

        /// <summary>Marks a todo-item as un-completed</summary>
        /// <param name="slug">Slug for identifying the todo-item</param>
        /// <response code="200">When there are no errors</response>
        /// <response code="400">When no user is indicated via the accesss token</response>
        /// <response code="401">When authentication fails</response>
        /// <response code="403">When the access token doesn't have write.todoitem scope</response>        
        /// <response code="404">When the user indicated via the access token doesn't have a todo-item with the slug specified</response>        
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = Constants.AuthorizationPolicyNames.WriteTodoItemScope)]
        [HttpPut("native/todo-items/{slug}/mark-as-uncompleted")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsUncompleted(string slug)
        {
            return await DoMarkAsUncompleted(slug);
        }

        /// <summary>Deletes a todo-item</summary>
        /// <param name="slug">Slug for identifying the todo-item</param>
        /// <response code="200">When there are no errors</response>
        /// <response code="400">When no user is indicated via the accesss token</response>
        /// <response code="401">When authentication fails</response>
        /// <response code="403">When the access token doesn't have delete.todoitem scope</response>        
        /// <response code="404">When the user indicated via the access token doesn't have a todo-item with the slug specified</response>        
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = Constants.AuthorizationPolicyNames.DeleteTodoItemScope)]
        [HttpDelete("native/todo-items/{slug}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string slug)
        {
            return await DoDelete(slug);
        }
    }
}
