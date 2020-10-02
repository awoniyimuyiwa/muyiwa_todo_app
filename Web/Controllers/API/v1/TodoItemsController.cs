using Application.Services.Abstracts;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Web.Utils;
using Web.ViewModels.Transient.v1;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Web.Controllers.API.v1
{
    /// <summary>
    /// Handles todo-item requests from browser based clients/single page applications
    /// </summary>
    /// <remarks> 
    /// Endpoints exposed by this controller use cookie authentication scheme.
    /// Endpoints are protected from CSRF because they are intended for use from a "web browser"
    /// where a stored access token is vulnerable to theft via XSS and cookies can be exploited by CSRF. 
    /// </remarks>
    // The ApiController attribute on the controller enables automatic validation and conversion of responses to json with camel case formatting 
    // and a response with 400 status code and error list will be sent when validation errors occur.
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiVersion("1.0")] // Use: [ApiVersion("1.0", Deprecated = true)] to advertise that an API version that is still supported will be deprecated soon
    [Route("api/todo-items")] // Serves for when no version is specified. Always remmeber to move this to the controllers for the latest version of your API 
    [Route("api/v{version:apiVersion}/todo-items")] // Serves for when v1 is specified
    [ValidateAntiForgeryToken]
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
                validationMessageLocalizer) { }

        /// <summary>Returns list of todo-items</summary>
        /// <param name="search">The search string</param>
        /// <param name="page">The page number</param>
        /// <param name="pageSize">The maximum number of items per page</param>
        /// <param name="dateRangeString">String in the format: dd MMMM yyyy-dd MMMM yyyy</param>
        /// <response code="200">When there are no errors</response>
        /// <response code="400">
        /// 1) When csrf token validation fails
        /// 2) When no user is indicated via the accesss token 
        /// 3) When input validation fails
        /// </response>
        /// <response code="401">When authentication fails</response>
        /// <response code="403">When the access token doesn't have read.todoitem scope</response>
        /// <remarks>
        /// This endpoint has CSRF protection because it uses cookie based authentication and is expected to be accessed from web browsers. 
        /// Visit /auth/login to authenticate
        /// </remarks>        
        [Authorize(Policy = Constants.AuthorizationPolicyNames.ReadTodoItemScope)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> Index(
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
        /// 1) When csrf token validation fails
        /// 2) When no user is indicated via the accesss token 
        /// 3) When input validation fails
        /// </response>
        /// <response code="401">When authentication fails</response>
        /// <response code="403">When the access token doesn't have write.todoitem scope</response>
        /// <remarks>
        /// This endpoint has CSRF protection because it uses cookie based authentication and is expected to be accessed from web browsers. 
        /// Visit /auth/login to authenticate.
        /// </remarks>        
        // The CustomizeValidator attribute on viewModel is used to prevent automatic validation
        // because of the need to perform validation asynchronously (calling the db to check if Name doesn't already exist e.t.c) 
        // which has to be done manually since MVC’s validation pipeline is not asynchronous
        [Authorize(Policy = Constants.AuthorizationPolicyNames.WriteTodoItemScope)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([CustomizeValidator(Skip = true)] TodoItemViewModel viewModel)
        {
            return await DoCreate(viewModel, "/api/v1/todo-items");            
        }

        /// <summary>Returns details of a todo-item</summary>
        /// <param name="slug">Slug for identifying the todo-item</param>
        /// <response code="200">When there are no errors</response>
        /// <response code="400">
        /// 1) When csrf token validation fails
        /// 2) When no user is indicated via the accesss token 
        /// 3) When input validation fails
        /// </response>
        /// <response code="401">When authentication fails</response>
        /// <response code="403">When the access token doesn't have the read.todoitem scope</response>        
        /// <response code="404">When the user indicated via the access token doesn't have a todo-item with the slug specified</response>        
        /// <remarks>
        /// This endpoint has CSRF protection because it uses cookie based authentication and is expected to be accessed from web browsers. 
        /// Visit /auth/login to authenticate
        /// </remarks>
        [Authorize(Policy = Constants.AuthorizationPolicyNames.ReadTodoItemScope)]
        [HttpGet("{slug}")]
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
        /// 1) When csrf token validation fails
        /// 2) When no user is indicated via the accesss token 
        /// 3) When input validation fails
        /// </response>
        /// <response code="401">When authentication fails</response>
        /// <response code="403">When the access token doesn't have write.todoitem scope</response>        
        /// <response code="404">When the user indicated via the access token doesn't have a todo-item with the slug specified</response>        
        /// <remarks>
        /// This endpoint has CSRF protection because it uses cookie based authentication and is expected to be accessed from web browsers. 
        /// Visit /auth/login to authenticate
        /// </remarks>       
        [Authorize(Policy = Constants.AuthorizationPolicyNames.WriteTodoItemScope)]
        [HttpPut("{slug}")]
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
        /// <response code="400">
        /// 1) When csrf token validation fails
        /// 2) When no user is indicated via the accesss token 
        /// 3) When input validation fails
        /// </response>
        /// <response code="401">When authentication fails</response>
        /// <response code="403">When the access token doesn't have write.todoitem scope</response>        
        /// <response code="404">When the user indicated via the access token doesn't have a todo-item with the slug specified</response>        
        /// <remarks>
        /// This endpoint has CSRF protection because it uses cookie based authentication and is expected to be accessed from web browsers. 
        /// Visit /auth/login to authenticate
        /// </remarks>        
        [Authorize(Policy = Constants.AuthorizationPolicyNames.WriteTodoItemScope)]
        [HttpPut("{slug}/mark-as-completed")]
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
        /// <response code="400">
        /// 1) When csrf token validation fails
        /// 2) When no user is indicated via the accesss token 
        /// 3) When input validation fails
        /// </response>
        /// <response code="401">When authentication fails</response>
        /// <response code="403">When the access token doesn't have write.todoitem scope</response>        
        /// <response code="404">When the user indicated via the access token doesn't have a todo-item with the slug specified</response>        
        /// <remarks>
        /// This endpoint has CSRF protection because it uses cookie based authentication and is expected to be accessed from web browsers. 
        /// Visit /auth/login to authenticate
        /// </remarks>        
        [Authorize(Policy = Constants.AuthorizationPolicyNames.WriteTodoItemScope)]
        [HttpPut("{slug}/mark-as-uncompleted")]
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
        /// <response code="400">
        /// 1) When csrf token validation fails
        /// 2) When no user is indicated via the accesss token 
        /// 3) When input validation fails
        /// </response>
        /// <response code="401">When authentication fails</response>
        /// <response code="403">When the access token doesn't have delete.todoitem scope</response>        
        /// <response code="404">When the user indicated via the access token doesn't have a todo-item with the slug specified</response>        
        /// <remarks>
        /// This endpoint has CSRF protection because it uses cookie based authentication and is expected to be accessed from web browsers. 
        /// Visit /auth/login to authenticate
        /// </remarks>        
        [Authorize(Policy = Constants.AuthorizationPolicyNames.DeleteTodoItemScope)]
        [HttpDelete("{slug}")]
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
