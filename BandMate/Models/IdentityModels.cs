using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace BandMate.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        //Extended Properties
        public ICollection<Band> Bands { get; set; }

        public NotificationPreference NotificationPreference { get; set; }
        public int? NotificationPreferenceId { get; set; }

        public string Title { get; set; }

        public Subscription Subscription { get; set; }
        public int? SubscriptionId { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<BandMate.Models.SubscriptionType> SubscriptionTypes { get; set; }

        public System.Data.Entity.DbSet<BandMate.Models.ProductType> ProductTypes { get; set; }

        public System.Data.Entity.DbSet<BandMate.Models.Size> Sizes { get; set; }

        public System.Data.Entity.DbSet<BandMate.Models.NotificationPreference> NotificationPreferences { get; set; }

        public System.Data.Entity.DbSet<BandMate.Models.State> States { get; set; }

    }
}