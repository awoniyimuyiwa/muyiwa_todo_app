using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Web.Services.Abstracts;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    /// <summary>
    /// Data for page layouts
    /// </summary>
    public class LayoutViewModel
    {
        /// <summary>App Name</summary>
        public string AppName { get; set; }

        /// <summary>Title</summary>
        public string Title { get; set; }

        /// <summary>Indicates if the page is being displayed for an authenticated user or not</summary>
        public bool IsUserAuthenticated { get; set; }

        /// <summary>Name of the currently authenticated user</summary>
        public string UserName { get; set; }
    }

    /// <summary>
    /// Generic version of LayoutViewModel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LayoutViewModel<T> : LayoutViewModel
    {
        /// <summary>View model for the page</summary>
        public T PageViewModel { get; private set; }

        /// <summary>
        /// Initializes properties
        /// </summary>
        /// <param name="pageViewModel"></param>
        /// <param name="title"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Initialize(T pageViewModel, string title, HttpContext httpContext)
        {
            AppName = "Todo App"; 
            PageViewModel = pageViewModel;
            Title = title;

            if (httpContext.User != null && httpContext.User.Identity.IsAuthenticated)
            {
                IsUserAuthenticated = true;

                var accessToken = await httpContext.GetTokenAsync("access_token");
                var oidcUserInfoService = (IOidcUserInfoService)httpContext.RequestServices.GetService(typeof(IOidcUserInfoService));
                UserName = await oidcUserInfoService.GetUserName(accessToken);
            }
        }
    }
}
