using Application.Services.Abstracts;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Web.Extensions;
using Web.ViewModels.Transient.v1;

namespace Web.Controllers.API.v1
{
    /// <summary>
    /// Base class for todo-item controllers
    /// </summary>
    public abstract class TodoItemsControllerBase : ControllerBase
    {
        readonly ITodoItemService TodoItemService;
        readonly IUserService UserService;
        readonly ILogger<TodoItemsControllerBase> Logger;
        readonly IStringLocalizer<Status> StatusMessageLocalizer;
        readonly IStringLocalizer<Validation> ValidationMessageLocalizer;

        /// <summary>
        /// Initializes fields
        /// </summary>
        /// <param name="todoItemService"></param>
        /// <param name="userService"></param>
        /// <param name="logger"></param>
        /// <param name="statusMessageLocalizer"></param>
        /// <param name="validationMessageLocalizer"></param>
        protected TodoItemsControllerBase(
            ITodoItemService todoItemService,
            IUserService userService,
            ILogger<TodoItemsControllerBase> logger,
            IStringLocalizer<Status> statusMessageLocalizer,
            IStringLocalizer<Validation> validationMessageLocalizer)
        {
            TodoItemService = todoItemService;
            UserService = userService;
            Logger = logger;
            StatusMessageLocalizer = statusMessageLocalizer;
            ValidationMessageLocalizer = validationMessageLocalizer;
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
        protected virtual async Task<ActionResult> DoIndex(
            string search = null,
            int page = 1,
            int pageSize = 15,
            string dateRangeString = null)
        {
            var user = await this.GetLocalUser(UserService);
            var dateRange = this.ValidateDateRange(dateRangeString, setDefaults: true);

            var todoItemDtos = await TodoItemService.TodoItemRepository.FilterBy(
                search,
                page,
                pageSize,
                t => t.User.Id == user.Id &&
                t.CreatedAt >= dateRange.Min && t.CreatedAt <= dateRange.Max);

            var responseBody = this.ListResponseBody(todoItemDtos, dateRange);

            return Ok(responseBody);
        }

        /// <summary>Creates a new todo-item</summary>
        /// <param name="viewModel">The view model for accepting form input and on which validation will be done</param>
        /// <param name="createdUrlPath"></param>
        [NonAction]
        protected virtual async Task<IActionResult> DoCreate(TodoItemViewModel viewModel, string createdUrlPath)
        {
            var user = await this.GetLocalUser(UserService);

            await Validate(viewModel, user.Id);
            if (!ModelState.IsValid) { return ValidationProblem(); }

            var todoItem = await TodoItemService.Create(user, viewModel.ToDto());

            var responseBody = new
            {
                Message = StatusMessageLocalizer["TodoItemCreated"].Value,
                Data = todoItem.ToDto()
            };

            return Created($"{createdUrlPath}/{todoItem.Slug}", responseBody);
            //return CreatedAtAction(nameof(View), ControllerContext.ActionDescriptor.ControllerName, new { slug = todoItem.Slug }, responseBody);
        }

        /// <summary>Returns details of a todo-item</summary>
        /// <param name="slug">Slug for identifying the todo-item</param>
        [NonAction]
        protected virtual async Task<IActionResult> DoView(string slug)
        {
            var user = await this.GetLocalUser(UserService);

            var todoItem = await TodoItemService.TodoItemRepository
                .FindOneBy(t => t.User.Id == user.Id && t.Slug == slug);
            if (todoItem == null) { return NotFound(); }

            var responseBody = new
            {
                Data = todoItem.ToDto()
            };

            return Ok(responseBody);
        }

        /// <summary>Updates a todo-item</summary>
        /// <param name="slug">Slug for identifying the todo-item</param>
        /// <param name="viewModel">The view model for accepting form input and on which validation will be done</param>
        [NonAction]
        protected virtual async Task<IActionResult> DoUpdate(string slug, TodoItemViewModel viewModel)
        {
            var user = await this.GetLocalUser(UserService);

            var todoItem = await TodoItemService.TodoItemRepository
                .FindOneBy(t => t.User.Id == user.Id && t.Slug == slug);
            if (todoItem == null) { return NotFound(); }

            await Validate(viewModel, user.Id, slug);
            if (!ModelState.IsValid) { return ValidationProblem(); }

            await TodoItemService.Update(todoItem, viewModel.ToDto());
            var responseBody = new
            {
                Message = StatusMessageLocalizer["TodoItemUpdated"].Value,
                Data = todoItem.ToDto()
            };

            return Ok(responseBody);
        }

        /// <summary>Marks a todo-item as completed</summary>
        /// <param name="slug">Slug for identifying the todo-item</param>
        [NonAction]
        protected virtual async Task<IActionResult> DoMarkAsCompleted(string slug)
        {
            var user = await this.GetLocalUser(UserService);

            var todoItem = await TodoItemService.TodoItemRepository
                .FindOneBy(t => t.User.Id == user.Id && t.Slug == slug);
            if (todoItem == null) { return NotFound(); }

            await TodoItemService.UpdateIsCompleted(todoItem);

            var responseBody = new
            {
                Message = StatusMessageLocalizer["TodoItemCompleted"].Value
            };

            return Ok(responseBody);
        }

        /// <summary>Marks a todo-item as un-completed</summary>
        /// <param name="slug">Slug for identifying the todo-item</param>
        [NonAction]
        protected virtual async Task<IActionResult> DoMarkAsUncompleted(string slug)
        {
            var user = await this.GetLocalUser(UserService);

            var todoItem = await TodoItemService.TodoItemRepository
                .FindOneBy(t => t.User.Id == user.Id && t.Slug == slug);
            if (todoItem == null) { return NotFound(); }

            await TodoItemService.UpdateIsCompleted(todoItem, false);

            var responseBody = new
            {
                Message = StatusMessageLocalizer["TodoItemUnCompleted"].Value
            };

            return Ok(responseBody);
        }

        /// <summary>Deletes a todo-item</summary>
        /// <param name="slug">Slug for identifying the todo-item</param>
        [NonAction]
        protected virtual async Task<IActionResult> DoDelete(string slug)
        {
            var user = await this.GetLocalUser(UserService);

            var todoItem = await TodoItemService.TodoItemRepository
                .FindOneBy(t => t.User.Id == user.Id && t.Slug == slug);
            if (todoItem == null) { return NotFound(); }

            await TodoItemService.Delete(todoItem);

            var responseBody = new
            {
                Message = StatusMessageLocalizer["TodoItemDeleted"].Value
            };

            return Ok(responseBody);
        }

        /// <summary>
        /// Validates a TodoItemViewModel asynchronously 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="userId"></param>
        /// <param name="slug"></param>
        /// <returns></returns>
        [NonAction]
        protected virtual async Task Validate(TodoItemViewModel viewModel, int userId, string slug = null)
        {
            var validator = new TodoItemViewModelValidator(TodoItemService, ValidationMessageLocalizer);

            var validationContext = new ValidationContext<TodoItemViewModel>(viewModel);
            validationContext.RootContextData[TodoItemViewModelValidator.RootContextDataKeys.UserIdKey] = userId;
            validationContext.RootContextData[TodoItemViewModelValidator.RootContextDataKeys.SlugKey] = slug;

            var validationResult = await validator.ValidateAsync(validationContext);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState, null);
            }
        }
    }
}
