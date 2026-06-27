using Microsoft.AspNetCore.Mvc;

namespace MainApp.Infrastructure.Authentication
{
    public class BimodalAuthentifyAttribute : TypeFilterAttribute
    {
        public BimodalAuthentifyAttribute()
        : base(typeof(BimodalAuthentify))
        {

        }
    }
}
