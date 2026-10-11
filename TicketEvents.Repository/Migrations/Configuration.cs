using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using TicketEvents.Entities.Models;
using TicketEvents.Repository.Context;

namespace TicketEvents.Repository.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<TicketEventsContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(TicketEventsContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!roleManager.RoleExists("Administrador"))
                roleManager.Create(new IdentityRole("Administrador"));

            if (!roleManager.RoleExists("Asociado"))
                roleManager.Create(new IdentityRole("Asociado"));

            var userManager = new UserManager<Usuario>(new UserStore<Usuario>(context));

            const string adminEmail = "admin@ticketevents.com";
            if (userManager.FindByName(adminEmail) == null)
            {
                var admin = new Usuario
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Nombre = "Administrador Sistema",
                    Estado = true,
                    UltimaConexion = DateTime.Now
                };

                if (userManager.Create(admin, "Admin123#").Succeeded)
                    userManager.AddToRole(admin.Id, "Administrador");
            }

            const string asociadoEmail = "asociado@test.com";
            if (userManager.FindByName(asociadoEmail) == null)
            {
                var asociado = new Usuario
                {
                    UserName = asociadoEmail,
                    Email = asociadoEmail,
                    Nombre = "Asociado Prueba",
                    Estado = true,
                    UltimaConexion = DateTime.Now
                };

                if (userManager.Create(asociado, "Asoc123#").Succeeded)
                    userManager.AddToRole(asociado.Id, "Asociado");
            }

            // Segundo asociado para demostrar compras concurrentes (dos navegadores a la vez).
            const string asociado2Email = "asociado2@test.com";
            if (userManager.FindByName(asociado2Email) == null)
            {
                var asociado2 = new Usuario
                {
                    UserName = asociado2Email,
                    Email = asociado2Email,
                    Nombre = "Asociado Prueba 2",
                    Estado = true,
                    UltimaConexion = DateTime.Now
                };

                if (userManager.Create(asociado2, "Asoc123#").Succeeded)
                    userManager.AddToRole(asociado2.Id, "Asociado");
            }

            context.SaveChanges();

            AgregarEventoSiNoExiste(context, new Evento
            {
                Nombre = "Concierto Filarmónico",
                Descripcion = "Una noche especial con música sinfónica para toda la familia.",
                Categoria = "Concierto",
                FechaHora = DateTime.Today.AddDays(30).AddHours(20),
                Ubicacion = "Teatro Nacional",
                Precio = 15000m,
                AforoTotal = 100,
                EntradasDisponibles = 100,
                Estado = true
            });

            AgregarEventoSiNoExiste(context, new Evento
            {
                Nombre = "Festival Sonidos del Valle",
                Descripcion = "Festival con artistas nacionales, luces y música en vivo.",
                Categoria = "Música",
                FechaHora = DateTime.Today.AddDays(37).AddHours(19),
                Ubicacion = "Parque Viva",
                Precio = 22000m,
                AforoTotal = 350,
                EntradasDisponibles = 350,
                Estado = true
            });

            AgregarEventoSiNoExiste(context, new Evento
            {
                Nombre = "La Casa de los Enigmas",
                Descripcion = "Una obra de teatro llena de humor, misterio y sorpresas.",
                Categoria = "Teatro",
                FechaHora = DateTime.Today.AddDays(44).AddHours(18),
                Ubicacion = "Teatro Nacional",
                Precio = 18500m,
                AforoTotal = 120,
                EntradasDisponibles = 120,
                Estado = true
            });

            AgregarEventoSiNoExiste(context, new Evento
            {
                Nombre = "Final Nacional de Baloncesto",
                Descripcion = "La gran final nacional en una noche llena de emoción deportiva.",
                Categoria = "Deportes",
                FechaHora = DateTime.Today.AddDays(51).AddHours(19),
                Ubicacion = "Gimnasio Nacional",
                Precio = 12000m,
                AforoTotal = 500,
                EntradasDisponibles = 500,
                Estado = true
            });

            AgregarEventoSiNoExiste(context, new Evento
            {
                Nombre = "Tech Innovation Summit 2026",
                Descripcion = "Charlas y talleres sobre innovación, tecnología y desarrollo de software.",
                Categoria = "Tecnología",
                FechaHora = DateTime.Today.AddDays(58).AddHours(9),
                Ubicacion = "Centro de Convenciones",
                Precio = 25000m,
                AforoTotal = 800,
                EntradasDisponibles = 800,
                Estado = true
            });

            AgregarEventoSiNoExiste(context, new Evento
            {
                Nombre = "Taller de Fotografía Nocturna",
                Descripcion = "Taller práctico con cupo limitado: ideal para probar el control de aforo y la concurrencia.",
                Categoria = "Taller",
                FechaHora = DateTime.Today.AddDays(12).AddHours(18),
                Ubicacion = "Centro Cultural de Tibás",
                Precio = 8000m,
                AforoTotal = 10,
                EntradasDisponibles = 10,
                Estado = true
            });

            context.SaveChanges();

            // El enunciado exige al menos tres imágenes por evento almacenadas en BD.
            // Se usan las fotos de Content/EventImages; si no se encuentran (por ejemplo al
            // ejecutar Update-Database desde la consola), se usa una imagen mínima de respaldo.
            var fotos = CargarFotosDeEjemplo();
            var placeholder = CrearImagenPredeterminada();
            var indice = 0;

            foreach (var evento in context.Eventos.Include(e => e.Imagenes).OrderBy(e => e.EventoId).ToList())
            {
                if (evento.Imagenes == null)
                    evento.Imagenes = new List<Imagen>();

                // Sustituye imágenes de relleno de versiones anteriores por fotos reales.
                if (fotos.Count > 0)
                {
                    foreach (var img in evento.Imagenes.Where(i => i.DatosImagen != null && i.DatosImagen.SequenceEqual(placeholder)).ToList())
                        context.Imagenes.Remove(img);
                }

                while (evento.Imagenes.Count < 3)
                {
                    var datos = fotos.Count > 0 ? fotos[indice++ % fotos.Count] : placeholder;
                    evento.Imagenes.Add(new Imagen { DatosImagen = datos });
                }
            }

            context.SaveChanges();
        }

        private static List<byte[]> CargarFotosDeEjemplo()
        {
            var fotos = new List<byte[]>();
            try
            {
                var carpeta = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "EventImages");
                if (!System.IO.Directory.Exists(carpeta))
                    return fotos;

                foreach (var archivo in System.IO.Directory.GetFiles(carpeta).OrderBy(f => f))
                {
                    var info = new System.IO.FileInfo(archivo);
                    var ext = info.Extension.ToLowerInvariant();
                    // Se omiten archivos diminutos (relleno) y fotos de más de 1 MB.
                    if ((ext == ".jpg" || ext == ".jpeg" || ext == ".png") && info.Length > 1024 && info.Length < 1024 * 1024)
                        fotos.Add(System.IO.File.ReadAllBytes(archivo));
                }
            }
            catch (Exception)
            {
                // Si no se puede leer la carpeta se usará la imagen de respaldo.
            }
            return fotos;
        }

        private static void AgregarEventoSiNoExiste(TicketEventsContext context, Evento evento)
        {
            if (!context.Eventos.Any(e => e.Nombre == evento.Nombre))
                context.Eventos.Add(evento);
        }

        private static byte[] CrearImagenPredeterminada()
        {
            // PNG válido almacenado como binario. Cada registro Imagen queda en SQL Server.
            const string pngBase64 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAJCAIAAAC0SDtlAAAAGklEQVR4nGPM9frPQApgIkk1w6gG4gDJwQoA/+AByL25rWIAAAAASUVORK5CYII=";
            return Convert.FromBase64String(pngBase64);
        }
    }
}
