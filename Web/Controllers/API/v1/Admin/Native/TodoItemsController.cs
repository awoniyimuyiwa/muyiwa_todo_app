using Application.Services.Abstracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Web.Utils;

namespace Web.Controllers.API.v1.Admin.Native
{
    /// <summary>
    /// Handles todo-item requests from non-browser native clients that can securely store access tokens 
    /// for users with admin permissions (assigned to them on the identity server) for this API, 
    /// </summary>
    /// <remarks>
    /// The endpoints on this contoller use the bearer authentication scheme and no csrf protection. 
    /// </remarks>
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiVersion("1.0")]
    [Route("api")] // The /admin/native path should have been included in this attribute but it was moved to actions to get past swagger's unique method/path restriction
    [Route("api/v{version:apiVersion}")]
    public class TodoItemsController : TodoItemsControllerBase
    {
        /// <summary>
        /// Initializes fields
        /// </summary>
        /// <param name="todoItemService"></param>
        public TodoItemsController(ITodoItemService todoItemService) : base(todoItemService) { }

        /// <summary>Returns list of todo-items</summary>
        /// <param name="search">The search string</param>
        /// <param name="page">The page number</param>
        /// <param name="pageSize">The maximum number of items per page</param>
        /// <param name="dateRangeString">String in the format: dd MMMM yyyy-dd MMMM yyyy</param>
        /// <response code="200">When there are no errors</response>
        /// <response code="400">When validation of any of the query string parameters fails</response>
        /// <response code="401">When authentication fails</response>
        /// <response code="403">When none of the permission claim for the user has value: "View TodoItem"</response> 
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = Constants.AuthorizationPolicyNames.ViewTodoItemPermission)]
        [HttpGet("admin/native/todo-items")]
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
            return await DoIndex(search, page, pageSize, dateRangeString);
        }

        /// <summary>Returns details of a todo-item</summary>
        /// <param name="slug">Slug for identifying the todo-item</param>
        /// <response code="200">When there are no errors</response>
        /// <response code="401">When authentication fails</response>
        /// <response code="403">When none of the permission claim for the user has value: "View TodoItem"</response>
        /// <response code="404">When a todo-item with the slug specified doesn't exist</response>       
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = Constants.AuthorizationPolicyNames.ViewTodoItemPermission)]
        [HttpGet("admin/native/todo-items/{slug}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> View(string slug)
        {
            return await DoView(slug);
        }
    }
}
