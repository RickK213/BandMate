namespace BandMate.Migrations
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BandMate.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BandMate.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            //Seed the subscription types
            SubscriptionType monthlySubscription = new SubscriptionType();
            monthlySubscription.Name = "Monthly Subscription";
            monthlySubscription.Price = 9.99d;

            SubscriptionType annualSubscription = new SubscriptionType();
            annualSubscription.Name = "Annual Subscription";
            annualSubscription.Price = 110.00d;

            context.SubscriptionTypes.Add(monthlySubscription);
            context.SubscriptionTypes.Add(annualSubscription);

            context.SaveChanges();


        }
    }
}
