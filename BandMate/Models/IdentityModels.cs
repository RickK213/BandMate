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

        public Subscription Subscription { get; set; }
        public int? SubscriptionId { get; set; }

        //note: the 2 properties below will not be saved in the database but will change depending on what band the user is viewing
        public string Title { get; set; }
        public int BandMemberId { get; set; }
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

        public System.Data.Entity.DbSet<BandMate.Models.Subscription> Subscriptions { get; set; }

        public System.Data.Entity.DbSet<BandMate.Models.Band> Bands { get; set; }

        public System.Data.Entity.DbSet<BandMate.Models.Store> Stores { get; set; }

        public System.Data.Entity.DbSet<BandMate.Models.Invitation> Invitations { get; set; }

        public System.Data.Entity.DbSet<BandMate.Models.BandMember> BandMembers { get; set; }

        public System.Data.Entity.DbSet<BandMate.Models.Tour> Tours { get; set; }

        public System.Data.Entity.DbSet<BandMate.Models.Venue> Venues { get; set; }

        public System.Data.Entity.DbSet<BandMate.Models.Address> Addresses { get; set; }

        public System.Data.Entity.DbSet<BandMate.Models.City> Cities { get; set; }

        public System.Data.Entity.DbSet<BandMate.Models.State> States { get; set; }

        public System.Data.Entity.DbSet<BandMate.Models.ZipCode> ZipCodes { get; set; }

        public System.Data.Entity.DbSet<BandMate.Models.SetList> SetLists { get; set; }

        public System.Data.Entity.DbSet<BandMate.Models.Song> Songs { get; set; }

        public System.Data.Entity.DbSet<BandMate.Models.SetListSong> SetListSongs { get; set; }

        public System.Data.Entity.DbSet<BandMate.Models.Event> Events { get; set; }

        public System.Data.Entity.DbSet<BandMate.Models.TourDate> TourDates { get; set; }

    }
}