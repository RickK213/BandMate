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

            //Seed the product types
            ProductType standardProductType = new ProductType();
            standardProductType.Name = "Standard Product";

            ProductType garmentProductType = new ProductType();
            garmentProductType.Name = "Garment";

            context.ProductTypes.Add(standardProductType);
            context.ProductTypes.Add(garmentProductType);

            context.SaveChanges();

            //Seed the sizes
            Size small = new Size();
            small.Name = "Small";
            small.Abbreviation = "S";

            Size medium = new Size();
            medium.Name = "Medium";
            medium.Abbreviation = "M";

            Size large = new Size();
            large.Name = "Large";
            large.Abbreviation = "L";

            Size xLarge = new Size();
            xLarge.Name = "X-Large";
            xLarge.Abbreviation = "XL";

            Size xxLarge = new Size();
            xxLarge.Name = "XX-Large";
            xxLarge.Abbreviation = "XXL";

            context.Sizes.Add(small);
            context.Sizes.Add(medium);
            context.Sizes.Add(large);
            context.Sizes.Add(xLarge);
            context.Sizes.Add(xxLarge);

            context.SaveChanges();

            //Seed notification preferences
            NotificationPreference emailPreference = new NotificationPreference();
            emailPreference.Name = "Email";

            NotificationPreference textPreference = new NotificationPreference();
            textPreference.Name = "Text";

            context.NotificationPreferences.Add(emailPreference);
            context.NotificationPreferences.Add(textPreference);

            context.SaveChanges();

        }
    }
}
