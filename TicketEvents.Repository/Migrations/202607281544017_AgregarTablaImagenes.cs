namespace TicketEvents.Repository.Migrations
{
    using System.Data.Entity.Migrations;
    public partial class AgregarTablaImagenes : DbMigration
    {
        public override void Up()
        {
            Sql(@"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Imagen_EventoId_Entrega' AND object_id = OBJECT_ID('dbo.Imagens'))
                  CREATE INDEX IX_Imagen_EventoId_Entrega ON dbo.Imagens(EventoId)");
        }
        public override void Down()
        {
            Sql(@"IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Imagen_EventoId_Entrega' AND object_id = OBJECT_ID('dbo.Imagens'))
                  DROP INDEX IX_Imagen_EventoId_Entrega ON dbo.Imagens");
        }
    }
}
