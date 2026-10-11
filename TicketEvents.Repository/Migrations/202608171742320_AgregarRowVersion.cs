namespace TicketEvents.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregarRowVersion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Eventoes", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Eventoes", "RowVersion");
        }
    }
}
