namespace BandMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BandMembers",
                c => new
                    {
                        BandMemberId = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        Title = c.String(),
                        Band_BandId = c.Int(),
                    })
                .PrimaryKey(t => t.BandMemberId)
                .ForeignKey("dbo.Bands", t => t.Band_BandId)
                .Index(t => t.Band_BandId);
            
            CreateTable(
                "dbo.Bands",
                c => new
                    {
                        BandId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        StoreId = c.Int(nullable: false),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.BandId)
                .ForeignKey("dbo.Stores", t => t.StoreId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.StoreId)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        EventId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        EventDate = c.DateTime(nullable: false),
                        AppearanceFee = c.Double(),
                        FeeCollectedOn = c.DateTime(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        SetList_SetListId = c.Int(),
                        Venue_VenueId = c.Int(),
                        Band_BandId = c.Int(),
                        Tour_TourId = c.Int(),
                    })
                .PrimaryKey(t => t.EventId)
                .ForeignKey("dbo.SetLists", t => t.SetList_SetListId)
                .ForeignKey("dbo.Venues", t => t.Venue_VenueId)
                .ForeignKey("dbo.Bands", t => t.Band_BandId)
                .ForeignKey("dbo.Tours", t => t.Tour_TourId)
                .Index(t => t.SetList_SetListId)
                .Index(t => t.Venue_VenueId)
                .Index(t => t.Band_BandId)
                .Index(t => t.Tour_TourId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        ImageFileName = c.String(),
                        Price = c.Double(nullable: false),
                        QuantityAvailable = c.Int(nullable: false),
                        IsSoldOut = c.Boolean(nullable: false),
                        ProductType_ProductTypeId = c.Int(),
                        TourDate_EventId = c.Int(),
                        Store_StoreId = c.Int(),
                        Transaction_TransactionId = c.Int(),
                    })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("dbo.ProductTypes", t => t.ProductType_ProductTypeId)
                .ForeignKey("dbo.Events", t => t.TourDate_EventId)
                .ForeignKey("dbo.Stores", t => t.Store_StoreId)
                .ForeignKey("dbo.Transactions", t => t.Transaction_TransactionId)
                .Index(t => t.ProductType_ProductTypeId)
                .Index(t => t.TourDate_EventId)
                .Index(t => t.Store_StoreId)
                .Index(t => t.Transaction_TransactionId);
            
            CreateTable(
                "dbo.ProductTypes",
                c => new
                    {
                        ProductTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ProductTypeId);
            
            CreateTable(
                "dbo.Sizes",
                c => new
                    {
                        SizeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Abbreviation = c.String(nullable: false),
                        UpCharge = c.Double(),
                        QuantityAvailable = c.Int(nullable: false),
                        Product_ProductId = c.Int(),
                    })
                .PrimaryKey(t => t.SizeId)
                .ForeignKey("dbo.Products", t => t.Product_ProductId)
                .Index(t => t.Product_ProductId);
            
            CreateTable(
                "dbo.SetLists",
                c => new
                    {
                        SetListId = c.Int(nullable: false, identity: true),
                        Band_BandId = c.Int(),
                    })
                .PrimaryKey(t => t.SetListId)
                .ForeignKey("dbo.Bands", t => t.Band_BandId)
                .Index(t => t.Band_BandId);
            
            CreateTable(
                "dbo.Songs",
                c => new
                    {
                        SongId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        SetList_SetListId = c.Int(),
                    })
                .PrimaryKey(t => t.SongId)
                .ForeignKey("dbo.SetLists", t => t.SetList_SetListId)
                .Index(t => t.SetList_SetListId);
            
            CreateTable(
                "dbo.Venues",
                c => new
                    {
                        VenueId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ContactFirstName = c.String(),
                        ContactLastName = c.String(),
                        ContactPhoneNumber = c.String(),
                        ContactEmail = c.String(),
                        Address_AddressId = c.Int(),
                        Band_BandId = c.Int(),
                    })
                .PrimaryKey(t => t.VenueId)
                .ForeignKey("dbo.Addresses", t => t.Address_AddressId)
                .ForeignKey("dbo.Bands", t => t.Band_BandId)
                .Index(t => t.Address_AddressId)
                .Index(t => t.Band_BandId);
            
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        AddressId = c.Int(nullable: false, identity: true),
                        StreetOne = c.String(nullable: false),
                        CityId = c.Int(nullable: false),
                        StateId = c.Int(nullable: false),
                        ZipCodeId = c.Int(nullable: false),
                        Lat = c.Single(),
                        Lng = c.Single(),
                    })
                .PrimaryKey(t => t.AddressId)
                .ForeignKey("dbo.Cities", t => t.CityId, cascadeDelete: true)
                .ForeignKey("dbo.States", t => t.StateId, cascadeDelete: true)
                .ForeignKey("dbo.ZipCodes", t => t.ZipCodeId, cascadeDelete: true)
                .Index(t => t.CityId)
                .Index(t => t.StateId)
                .Index(t => t.ZipCodeId);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        CityId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.CityId);
            
            CreateTable(
                "dbo.States",
                c => new
                    {
                        StateId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Abbreviation = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.StateId);
            
            CreateTable(
                "dbo.ZipCodes",
                c => new
                    {
                        ZipCodeId = c.Int(nullable: false, identity: true),
                        Number = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ZipCodeId);
            
            CreateTable(
                "dbo.Invitations",
                c => new
                    {
                        InvitationId = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false),
                        InvitedBy = c.String(),
                        BandName = c.String(),
                        Title = c.String(),
                        BandId = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        IsAccepted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.InvitationId)
                .ForeignKey("dbo.Bands", t => t.BandId, cascadeDelete: true)
                .Index(t => t.BandId);
            
            CreateTable(
                "dbo.Stores",
                c => new
                    {
                        StoreId = c.Int(nullable: false, identity: true),
                        PlaylistId = c.String(),
                    })
                .PrimaryKey(t => t.StoreId);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        TransactionId = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        TotalPrice = c.Double(nullable: false),
                        Store_StoreId = c.Int(),
                    })
                .PrimaryKey(t => t.TransactionId)
                .ForeignKey("dbo.Stores", t => t.Store_StoreId)
                .Index(t => t.Store_StoreId);
            
            CreateTable(
                "dbo.Tours",
                c => new
                    {
                        TourId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Band_BandId = c.Int(),
                    })
                .PrimaryKey(t => t.TourId)
                .ForeignKey("dbo.Bands", t => t.Band_BandId)
                .Index(t => t.Band_BandId);
            
            CreateTable(
                "dbo.NotificationPreferences",
                c => new
                    {
                        NotificationPreferenceId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.NotificationPreferenceId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Subscriptions",
                c => new
                    {
                        SubscriptionId = c.Int(nullable: false, identity: true),
                        SubscriptionTypeId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        AutoRenewal = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.SubscriptionId)
                .ForeignKey("dbo.SubscriptionTypes", t => t.SubscriptionTypeId, cascadeDelete: true)
                .Index(t => t.SubscriptionTypeId);
            
            CreateTable(
                "dbo.SubscriptionTypes",
                c => new
                    {
                        SubscriptionTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Price = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.SubscriptionTypeId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        NotificationPreferenceId = c.Int(),
                        SubscriptionId = c.Int(),
                        Title = c.String(),
                        BandMemberId = c.Int(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NotificationPreferences", t => t.NotificationPreferenceId)
                .ForeignKey("dbo.Subscriptions", t => t.SubscriptionId)
                .Index(t => t.NotificationPreferenceId)
                .Index(t => t.SubscriptionId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "SubscriptionId", "dbo.Subscriptions");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "NotificationPreferenceId", "dbo.NotificationPreferences");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Bands", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Subscriptions", "SubscriptionTypeId", "dbo.SubscriptionTypes");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Venues", "Band_BandId", "dbo.Bands");
            DropForeignKey("dbo.Tours", "Band_BandId", "dbo.Bands");
            DropForeignKey("dbo.Events", "Tour_TourId", "dbo.Tours");
            DropForeignKey("dbo.Bands", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.Transactions", "Store_StoreId", "dbo.Stores");
            DropForeignKey("dbo.Products", "Transaction_TransactionId", "dbo.Transactions");
            DropForeignKey("dbo.Products", "Store_StoreId", "dbo.Stores");
            DropForeignKey("dbo.SetLists", "Band_BandId", "dbo.Bands");
            DropForeignKey("dbo.Invitations", "BandId", "dbo.Bands");
            DropForeignKey("dbo.Events", "Band_BandId", "dbo.Bands");
            DropForeignKey("dbo.Events", "Venue_VenueId", "dbo.Venues");
            DropForeignKey("dbo.Venues", "Address_AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Addresses", "ZipCodeId", "dbo.ZipCodes");
            DropForeignKey("dbo.Addresses", "StateId", "dbo.States");
            DropForeignKey("dbo.Addresses", "CityId", "dbo.Cities");
            DropForeignKey("dbo.Events", "SetList_SetListId", "dbo.SetLists");
            DropForeignKey("dbo.Songs", "SetList_SetListId", "dbo.SetLists");
            DropForeignKey("dbo.Products", "TourDate_EventId", "dbo.Events");
            DropForeignKey("dbo.Sizes", "Product_ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "ProductType_ProductTypeId", "dbo.ProductTypes");
            DropForeignKey("dbo.BandMembers", "Band_BandId", "dbo.Bands");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "SubscriptionId" });
            DropIndex("dbo.AspNetUsers", new[] { "NotificationPreferenceId" });
            DropIndex("dbo.Subscriptions", new[] { "SubscriptionTypeId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Tours", new[] { "Band_BandId" });
            DropIndex("dbo.Transactions", new[] { "Store_StoreId" });
            DropIndex("dbo.Invitations", new[] { "BandId" });
            DropIndex("dbo.Addresses", new[] { "ZipCodeId" });
            DropIndex("dbo.Addresses", new[] { "StateId" });
            DropIndex("dbo.Addresses", new[] { "CityId" });
            DropIndex("dbo.Venues", new[] { "Band_BandId" });
            DropIndex("dbo.Venues", new[] { "Address_AddressId" });
            DropIndex("dbo.Songs", new[] { "SetList_SetListId" });
            DropIndex("dbo.SetLists", new[] { "Band_BandId" });
            DropIndex("dbo.Sizes", new[] { "Product_ProductId" });
            DropIndex("dbo.Products", new[] { "Transaction_TransactionId" });
            DropIndex("dbo.Products", new[] { "Store_StoreId" });
            DropIndex("dbo.Products", new[] { "TourDate_EventId" });
            DropIndex("dbo.Products", new[] { "ProductType_ProductTypeId" });
            DropIndex("dbo.Events", new[] { "Tour_TourId" });
            DropIndex("dbo.Events", new[] { "Band_BandId" });
            DropIndex("dbo.Events", new[] { "Venue_VenueId" });
            DropIndex("dbo.Events", new[] { "SetList_SetListId" });
            DropIndex("dbo.Bands", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Bands", new[] { "StoreId" });
            DropIndex("dbo.BandMembers", new[] { "Band_BandId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.SubscriptionTypes");
            DropTable("dbo.Subscriptions");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.NotificationPreferences");
            DropTable("dbo.Tours");
            DropTable("dbo.Transactions");
            DropTable("dbo.Stores");
            DropTable("dbo.Invitations");
            DropTable("dbo.ZipCodes");
            DropTable("dbo.States");
            DropTable("dbo.Cities");
            DropTable("dbo.Addresses");
            DropTable("dbo.Venues");
            DropTable("dbo.Songs");
            DropTable("dbo.SetLists");
            DropTable("dbo.Sizes");
            DropTable("dbo.ProductTypes");
            DropTable("dbo.Products");
            DropTable("dbo.Events");
            DropTable("dbo.Bands");
            DropTable("dbo.BandMembers");
        }
    }
}
