using Microsoft.AspNetCore.Identity;

namespace Punim_Diplome.Models
{
    public class AdminOrdersViewModel
    {
       
            public List<OrderProduct> Orders { get; set; }
            public List<IdentityUser> Users { get; set; } 
        

    }
}
