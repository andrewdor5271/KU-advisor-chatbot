using Microsoft.AspNetCore.Mvc;

namespace MainApp.Infrastructure.Authentication
{
    public class BimodalCheckAuthenticationAttribute : TypeFilterAttribute
    {
        public BimodalCheckAuthenticationAttribute()
            : base(typeof(BimodalCheckAuthentication))
        {
                
        }
    }
}
