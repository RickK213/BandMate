namespace BandMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tours", "Band_BandId", "dbo.Bands");
            DropIndex("dbo.Tours", new[] { "Band_BandId" });
            RenameColumn(table: "dbo.Tours", name: "Band_BandId", newName: "BandId");
            AlterColumn("dbo.Tours", "BandId", c => c.Int(nullable: false));
            CreateIndex("dbo.Tours", "BandId");
            AddForeignKey("dbo.Tours", "BandId", "dbo.Bands", "BandId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tours", "BandId", "dbo.Bands");
            DropIndex("dbo.Tours", new[] { "BandId" });
            AlterColumn("dbo.Tours", "BandId", c => c.Int());
            RenameColumn(table: "dbo.Tours", name: "BandId", newName: "Band_BandId");
            CreateIndex("dbo.Tours", "Band_BandId");
            AddForeignKey("dbo.Tours", "Band_BandId", "dbo.Bands", "BandId");
        }
    }
}
