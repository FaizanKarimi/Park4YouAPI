using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Helpers
{
    public static class HttpContextExtensionMethods
    {
        /// <summary>
        /// Get userId from the claim.
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <returns></returns>
        public static string GetCurrentUserId(this IHttpContextAccessor httpContextAccessor)
        {
            string userId = string.Empty;
            HttpContext context = httpContextAccessor.HttpContext;
            if (context != null)
            {
                bool hasUserId = httpContextAccessor.HttpContext.User.HasClaim(x => x.Type == ClaimTypes.NameIdentifier);
                if (hasUserId)
                {
                    userId = httpContextAccessor?.HttpContext?.User?.FindFirst(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? null;
                }
            }
            return userId;
        }

        /// <summary>
        /// Get username from the claim.
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <returns></returns>
        public static string GetCurrentUserName(this IHttpContextAccessor httpContextAccessor)
        {
            string userName = string.Empty;
            HttpContext context = httpContextAccessor.HttpContext;
            if (context != null)
            {
                bool hasUserName = httpContextAccessor.HttpContext.User.HasClaim(x => x.Type == ClaimTypes.Name);
                if (hasUserName)
                {
                    userName = httpContextAccessor?.HttpContext?.User?.FindFirst(x => x.Type == ClaimTypes.Name)?.Value ?? null;
                }
            }
            return userName;
        }

        /// <summary>
        /// Get user email from the claim.
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <returns></returns>
        public static string GetCurrentUserEmail(this IHttpContextAccessor httpContextAccessor)
        {
            string userEmail = string.Empty;
            HttpContext context = httpContextAccessor.HttpContext;
            if (context != null)
            {
                bool hasUserEmail = httpContextAccessor.HttpContext.User.HasClaim(x => x.Type == ClaimTypes.Email);
                if (hasUserEmail)
                {
                    userEmail = httpContextAccessor?.HttpContext?.User?.FindFirst(x => x.Type == ClaimTypes.Email)?.Value ?? null;
                }
            }
            return userEmail;
        }

        /// <summary>
        /// Get user role from the claim.
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <returns></returns>
        public static string GetCurrentUserRole(this IHttpContextAccessor httpContextAccessor)
        {
            string userRole = string.Empty;
            HttpContext context = httpContextAccessor.HttpContext;
            if (context != null)
            {
                bool hasUserRole = httpContextAccessor.HttpContext.User.HasClaim(x => x.Type == ClaimTypes.Role);
                if (hasUserRole)
                {
                    userRole = httpContextAccessor?.HttpContext?.User?.FindFirst(x => x.Type == ClaimTypes.Role)?.Value ?? null;
                }
            }
            return userRole;
        }
    }
}