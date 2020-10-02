using Application.Services.Abstracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Web.Extensions;

namespace Web.Controllers.API.v1.Admin
{
    /// <summary>
    /// Base class for admin TodoItemControllers
    /// </summary>
    public abstract class TodoItemsControllerBase : ControllerBase
    {
        readonly ITodoItemService TodoItemService;

        /// <summary>
        /// Initializes fields
        /// </summary>
        /// <param name="todoItemService"></param>
        public TodoItemsControllerBase(ITodoItemService todoItemService)
        {
            TodoItemService = todoItemService;
        }

        /// <summary>
        /// Returns list of todo-items
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="dateRangeString"></param>
        /// <returns></returns>
        [NonAction]
        protected virtual async Task<IActionResult> DoIndex(
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
        [NonAction]
        protected virtual async Task<IActionResult> DoView(string slug)
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
