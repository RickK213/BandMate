namespace BandMate.Migrations
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualBasic.FileIO;
    using System.Web;
    using System.Web.Hosting;
    using System.Reflection;

    internal sealed class Configuration : DbMigrationsConfiguration<BandMate.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BandMate.Models.ApplicationDbContext context)
        {

            //Seed the subscription types
            SubscriptionType monthlySubscription = new SubscriptionType();
            monthlySubscription.Name = "Monthly Subscription";
            monthlySubscription.Price = 9.99d;

            SubscriptionType annualSubscription = new SubscriptionType();
            annualSubscription.Name = "Annual Subscription";
            annualSubscription.Price = 108.00d;

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

            ////Seed the sizes
            //Size small = new Size();
            //small.Name = "Small";
            //small.Abbreviation = "S";

            //Size medium = new Size();
            //medium.Name = "Medium";
            //medium.Abbreviation = "M";

            //Size large = new Size();
            //large.Name = "Large";
            //large.Abbreviation = "L";

            //Size xLarge = new Size();
            //xLarge.Name = "X-Large";
            //xLarge.Abbreviation = "XL";

            //Size xxLarge = new Size();
            //xxLarge.Name = "XX-Large";
            //xxLarge.Abbreviation = "XXL";

            //context.Sizes.Add(small);
            //context.Sizes.Add(medium);
            //context.Sizes.Add(large);
            //context.Sizes.Add(xLarge);
            //context.Sizes.Add(xxLarge);

            //context.SaveChanges();

            //Seed notification preferences
            NotificationPreference emailPreference = new NotificationPreference();
            emailPreference.Name = "Email";

            NotificationPreference textPreference = new NotificationPreference();
            textPreference.Name = "Text";

            context.NotificationPreferences.Add(emailPreference);
            context.NotificationPreferences.Add(textPreference);

            context.SaveChanges();

            //Seed States table with all US States from CSV
            string seedFile = "~/CSV/SeedData/";//states.csv removed from end of seedFile
            string filePath = GetMapPath(seedFile);

            bool fileExists = File.Exists(filePath + "states.csv");
            if (fileExists)
            {
                List<State> states = new List<State>();
                using (TextFieldParser parser = new TextFieldParser(filePath + "states.csv"))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    State state;
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        if (fields.Any(x => x.Length == 0))
                        {
                            Console.WriteLine("We found an empty value in your CSV. Please check your file and try again.\nPress any key to return to main menu.");
                            Console.ReadKey(true);
                        }
                        state = new State();
                        state.Name = fields[0];
                        state.Abbreviation = fields[1];
                        states.Add(state);
                    }
                }
                context.States.AddOrUpdate(c => c.Abbreviation, states.ToArray());
                context.SaveChanges();
            }
        }

        private string GetMapPath(string seedFile)
        {
            if (HttpContext.Current != null)
                return HostingEnvironment.MapPath(seedFile);

            var absolutePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath; //was AbsolutePath but didn't work with spaces according to comments
            var directoryName = Path.GetDirectoryName(absolutePath);
            var path = Path.Combine(directoryName, ".." + seedFile.TrimStart('~').Replace('/', '\\'));

            return path;
        }

    }
}
