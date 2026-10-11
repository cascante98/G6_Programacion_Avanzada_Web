namespace TicketEvents.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregarIdentityYEntidades : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DetalleOrdens",
                c => new
                    {
                        DetalleOrdenId = c.Int(nullable: false, identity: true),
                        OrdenId = c.Int(nullable: false),
                        EventoId = c.Int(nullable: false),
                        Cantidad = c.Int(nullable: false),
                        PrecioUnitario = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.DetalleOrdenId)
                .ForeignKey("dbo.Eventoes", t => t.EventoId, cascadeDelete: true)
                .ForeignKey("dbo.Ordens", t => t.OrdenId, cascadeDelete: true)
                .Index(t => t.OrdenId)
                .Index(t => t.EventoId);
            
            CreateTable(
                "dbo.Ordens",
                c => new
                    {
                        OrdenId = c.Int(nullable: false, identity: true),
                        Fecha = c.DateTime(nullable: false),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UsuarioId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.OrdenId)
                .ForeignKey("dbo.AspNetUsers", t => t.UsuarioId)
                .Index(t => t.UsuarioId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Nombre = c.String(nullable: false),
                        UltimaConexion = c.DateTime(),
                        Estado = c.Boolean(nullable: false),
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
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Imagens",
                c => new
                    {
                        ImagenId = c.Int(nullable: false, identity: true),
                        DatosImagen = c.Binary(nullable: false),
                        EventoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ImagenId)
                .ForeignKey("dbo.Eventoes", t => t.EventoId, cascadeDelete: true)
                .Index(t => t.EventoId);
            
            CreateTable(
                "dbo.Resenas",
                c => new
                    {
                        ResenaId = c.Int(nullable: false, identity: true),
                        Comentario = c.String(nullable: false),
                        Calificacion = c.Int(nullable: false),
                        FechaCreacion = c.DateTime(nullable: false),
                        Estado = c.String(),
                        UsuarioId = c.String(maxLength: 128),
                        EventoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ResenaId)
                .ForeignKey("dbo.Eventoes", t => t.EventoId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UsuarioId)
                .Index(t => t.UsuarioId)
                .Index(t => t.EventoId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            AlterColumn("dbo.Eventoes", "Nombre", c => c.String(nullable: false, maxLength: 150));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Resenas", "UsuarioId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Resenas", "EventoId", "dbo.Eventoes");
            DropForeignKey("dbo.Imagens", "EventoId", "dbo.Eventoes");
            DropForeignKey("dbo.Ordens", "UsuarioId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DetalleOrdens", "OrdenId", "dbo.Ordens");
            DropForeignKey("dbo.DetalleOrdens", "EventoId", "dbo.Eventoes");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Resenas", new[] { "EventoId" });
            DropIndex("dbo.Resenas", new[] { "UsuarioId" });
            DropIndex("dbo.Imagens", new[] { "EventoId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Ordens", new[] { "UsuarioId" });
            DropIndex("dbo.DetalleOrdens", new[] { "EventoId" });
            DropIndex("dbo.DetalleOrdens", new[] { "OrdenId" });
            AlterColumn("dbo.Eventoes", "Nombre", c => c.String());
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Resenas");
            DropTable("dbo.Imagens");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Ordens");
            DropTable("dbo.DetalleOrdens");
        }
    }
}
