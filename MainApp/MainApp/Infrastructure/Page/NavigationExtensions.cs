using Microsoft.AspNetCore.Mvc.Rendering;

namespace MainApp.Infrastructure.Page
{
    public static class NavigationExtensions
    {
        // Cool tech I learned from an LLM
        public static string IsActive(this ViewContext context, string page)
        {
            var current = context.RouteData.Values["page"]?.ToString();
            if (current == "/Index")
            {
                return current == page || page == "/" ? "active" : "";
            }
            return current == page ? "active" : "";
        }
    }
}
