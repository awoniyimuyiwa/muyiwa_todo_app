using Application.Exceptions;
using Application.Services.Abstracts;
using Domain.Generic;
using Domain.Generic.Auth;
using Domain.Generic.Auth.Dtos;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Exceptions;

namespace Web.Extensions
{
    /// <summary>
    /// Extension methods for controllers
    /// </summary>
    internal static class ContollerExtensions
    {
        /// <summary>
        /// Retrieve local user
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="userService"></param>
        /// <returns>User if it exists</returns>
        /// <exception cref="UserNotFoundException">Thrown when user cannot be found or created</exception>
        internal async static Task<User> GetLocalUser(
            this ControllerBase controller, IUserService userService)
        {
            var userIdClaimFromIdp = controller.User.FindFirst(claim =>
            claim.Type == JwtClaimTypes.Subject || claim.Type == ClaimTypes.NameIdentifier);

            if (userIdClaimFromIdp != null) 
            {
                var user = await userService.FindOrCreate(userIdClaimFromIdp.Value, new UserDto());
                if (user != null) { return user; } 
            }

            throw new UserNotFoundException();
        }

        [NonAction]
        internal static object ListResponseBody<T>(
            this ControllerBase controller,
            PaginatedList<T> paginatedList, DateRange dateRange)
        {
            return new
            {
                Data = paginatedList,
                Meta = new
                {
                    paginatedList.Page,
                    paginatedList.PerPage,
                    paginatedList.Total,
                    paginatedList.TotalPages,
                    paginatedList.HasNext,
                    paginatedList.HasPrevious,
                    DateRange = dateRange.ToString()
                }
            };
        }

        [NonAction]
        internal static object ListResponseBody<T>(PaginatedList<T> paginatedList)
        {
            return new
            {
                Data = paginatedList,
                Meta = new
                {
                    paginatedList.Page,
                    paginatedList.PerPage,
                    paginatedList.Total,
                    paginatedList.TotalPages,
                    paginatedList.HasNext,
                    paginatedList.HasPrevious
                }
            };
        }

        internal static DateRange ValidateDateRange(
            this ControllerBase controller,
            string dateRangeString, bool setDefaults = false)
        {
            try
            {
                return DateRange.Parse(dateRangeString, setDefaults);
            }
            catch (Exception ex)
            {
                throw new DateRangeValidationException(innerException: ex);
            }
        }
    }
}
