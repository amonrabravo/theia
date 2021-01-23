using Microsoft.AspNetCore.Identity;

namespace Theia.Data
{
    public class Role: IdentityRole<int>
    {
        public string DiplayName { get; set; }
    }
}