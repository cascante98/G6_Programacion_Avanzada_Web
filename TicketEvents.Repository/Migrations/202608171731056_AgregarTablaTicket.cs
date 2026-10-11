namespace TicketEvents.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregarTablaTicket : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        TicketId = c.Int(nullable: false, identity: true),
                        Codigo = c.String(nullable: false, maxLength: 20),
                        DetalleOrdenId = c.Int(nullable: false),
                        Usado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TicketId)
                .ForeignKey("dbo.DetalleOrdens", t => t.DetalleOrdenId, cascadeDelete: true)
                .Index(t => t.Codigo, unique: true, name: "IX_Ticket_Codigo")
                .Index(t => t.DetalleOrdenId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "DetalleOrdenId", "dbo.DetalleOrdens");
            DropIndex("dbo.Tickets", new[] { "DetalleOrdenId" });
            DropIndex("dbo.Tickets", "IX_Ticket_Codigo");
            DropTable("dbo.Tickets");
        }
    }
}
