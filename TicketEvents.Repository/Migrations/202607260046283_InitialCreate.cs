namespace TicketEvents.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Eventoes",
                c => new
                    {
                        EventoId = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Descripcion = c.String(),
                        Categoria = c.String(),
                        FechaHora = c.DateTime(nullable: false),
                        Ubicacion = c.String(),
                        Precio = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AforoTotal = c.Int(nullable: false),
                        EntradasDisponibles = c.Int(nullable: false),
                        Estado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.EventoId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Eventoes");
        }
    }
}
