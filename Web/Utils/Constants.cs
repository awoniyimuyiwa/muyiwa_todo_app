namespace Web.Utils
{
    internal static class Constants
    {
        internal static class AuthorizationPolicyNames
        {
            // Admin user permissions
            internal const string ViewTodoItemPermission = "View TodoItem";

            // Api scopes
            internal const string DeleteTodoItemScope = "delete.todoitem";
            internal const string ReadTodoItemScope = "read.todoitem";
            internal const string WorkerTodoItemScope = "worker.todoitem";
            internal const string WriteTodoItemScope = "write.todoitem";
        }

        internal static class CustomClaimTypes
        {
            internal const string LocalUserId = "local_user_id";
            internal const string Permission = "permission";
        }

        internal static class HttpClientNames
        {
            internal const string ClientWithClientAccessToken = "Client";
            internal const string ClientWithUserAccessToken = "User Client";
        }

        internal static class PollyPolicyNames
        {
            internal const string RetryWithJitter = "Retry With Jitter";
        }

        internal static class SwaggerSecurityDefinitionNames
        {
            internal const string OAuth2AuthCodeNative = "oauth2_auth_code_native";
            internal const string OAuth2AuthCodeAdminNative = "oauth2_auth_code_admin_native";
            internal const string OAuth2ClientCredentials = "oauth2_client_credentials";
        }
    }
}
