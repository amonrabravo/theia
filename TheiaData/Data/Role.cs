using Microsoft.AspNetCore.Identity;

namespace TheiaData.Data
{
    public class Role: IdentityRole<int>
    {
        public string DiplayName { get; set; }
    }
}