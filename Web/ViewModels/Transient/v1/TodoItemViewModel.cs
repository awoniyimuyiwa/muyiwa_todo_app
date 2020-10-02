using Application.Services.Abstracts;
using Domain.Core.Dtos;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Transient.v1
{
    /// <summary>
    /// For receieving user input for a todo-item   
    /// </summary>
    public class TodoItemViewModel
    {
        /// <summary>
        /// The Name for the todo-item
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Converts to a TodoItemDto
        /// </summary>
        public TodoItemDto ToDto()
        {
            return new TodoItemDto()
            {
                Name = Name
            };
        }
    }

    /// <summary>
    /// Fluent validation for TodoItemViewModel
    /// </summary>
    class TodoItemViewModelValidator : AbstractValidator<TodoItemViewModel>
    {
        /// <summary>
        /// Contsructor for TodItemViewModelValidator
        /// </summary>
        /// <param name="todoItemService"></param>
        /// <param name="validationMessageLocalizer"></param>
        // When writing regular expression in C#, it is recommended that verbatim strings should be used instead of regular strings.
        // Verbatim strings begin with a special prefix (@) and signal to the compiler not to interpret backslashes and special metacharacters in the string, 
        // thereby making it possible to pass them through directly to the regular expression engine. 
        // This means that a pattern like "\n\w" will not be interpreted and can be written as @"\n\w" instead of "\\n\\w" as in other languages, 
        // which is much easier to read. https://regexone.com/references/csharp
        public TodoItemViewModelValidator(
            ITodoItemService todoItemService,
            IStringLocalizer<Validation> validationMessageLocalizer)
        {
            RuleFor(vm => vm.Name)
                .NotEmpty()
                .WithName("Name")
                .WithMessage(vm => validationMessageLocalizer["Required"])
                .Length(3, 250)                
                .Matches(@"^[A-Za-z0-9\s]+$").WithMessage(vm => validationMessageLocalizer["AlphaNumericSpace"])
                .MustAsync(async (vm, name, context, cancellationToken) =>
                {
                    var todoItem = await todoItemService.TodoItemRepository.FindOneBy(
                        t => t.Slug != (string)context.ParentContext.RootContextData[RootContextDataKeys.SlugKey] &&
                        t.NormalizedName == name.ToUpperInvariant() &&
                        t.User.Id == (int)context.ParentContext.RootContextData[RootContextDataKeys.UserIdKey]);

                    return todoItem == null;
                }).WithMessage(vm => validationMessageLocalizer["Unique"]);
        }

        internal static class RootContextDataKeys
        {
            internal const string SlugKey = "slug";
            internal const string UserIdKey = "userId";
        }
    }
}
