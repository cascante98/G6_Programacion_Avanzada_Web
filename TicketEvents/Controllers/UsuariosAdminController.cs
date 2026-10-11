using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TicketEvents.Entities.Models;
using TicketEvents.Services.Services;

namespace TicketEvents.Controllers
{
    /// <summary>
    /// CRUD de usuarios. Los datos se leen y guardan mediante IUsuarioService (capa Service →
    /// Repository); UserManager de ASP.NET Identity se usa solo para contraseñas y roles.
    /// </summary>
    [Authorize(Roles = "Administrador")]
    public class UsuariosAdminController : Controller
    {
        public static readonly string[] RolesDisponibles = { "Asociado", "Administrador" };

        private readonly IUsuarioService _usuarioService;
        private UserManager<Usuario> UserManager { get { return HttpContext.GetOwinContext().GetUserManager<UserManager<Usuario>>(); } }

        public UsuariosAdminController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public ActionResult Index()
        {
            var usuarios = _usuarioService.ObtenerTodos().ToList();
            ViewBag.Roles = usuarios.ToDictionary(u => u.Id, u => string.Join(", ", UserManager.GetRoles(u.Id)));
            return View("~/Views/UsuarioAdmin/Index.cshtml", usuarios);
        }

        public ActionResult Create()
        {
            ViewBag.RolesDisponibles = new SelectList(RolesDisponibles);
            return View("~/Views/UsuarioAdmin/Create.cshtml", new CrearUsuarioViewModel { Rol = "Asociado" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CrearUsuarioViewModel model)
        {
            if (model != null && !RolesDisponibles.Contains(model.Rol))
                ModelState.AddModelError("Rol", "Seleccione un rol válido.");

            if (ModelState.IsValid)
            {
                var usuario = new Usuario { UserName = model.Email, Email = model.Email, Nombre = model.Nombre, Estado = true };
                var result = await UserManager.CreateAsync(usuario, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(usuario.Id, model.Rol);
                    TempData["Mensaje"] = "Usuario creado correctamente.";
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors) ModelState.AddModelError("", error);
            }

            ViewBag.RolesDisponibles = new SelectList(RolesDisponibles, model == null ? null : model.Rol);
            return View("~/Views/UsuarioAdmin/Create.cshtml", model);
        }

        public ActionResult Details(string id)
        {
            var usuario = _usuarioService.ObtenerUsuarioPorId(id);
            if (usuario == null) return HttpNotFound();
            ViewBag.Rol = string.Join(", ", UserManager.GetRoles(id));
            return View("~/Views/UsuarioAdmin/Details.cshtml", usuario);
        }

        public ActionResult Edit(string id)
        {
            var usuario = _usuarioService.ObtenerUsuarioPorId(id);
            if (usuario == null) return HttpNotFound();
            ViewBag.RolesDisponibles = new SelectList(RolesDisponibles, UserManager.GetRoles(id).FirstOrDefault());
            return View("~/Views/UsuarioAdmin/Edit.cshtml", usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Usuario usuario, string rol)
        {
            var esUsuarioActual = usuario.Id == User.Identity.GetUserId();
            if (!RolesDisponibles.Contains(rol))
                ModelState.AddModelError("", "Seleccione un rol válido.");
            if (esUsuarioActual && (!usuario.Estado || rol != "Administrador"))
                ModelState.AddModelError("", "No puede desactivarse ni quitarse el rol de Administrador a sí mismo.");

            if (ModelState.IsValid)
            {
                var usuarioDB = _usuarioService.ObtenerUsuarioPorId(usuario.Id);
                if (usuarioDB == null) return HttpNotFound();

                usuarioDB.Nombre = usuario.Nombre;
                usuarioDB.Email = usuario.Email;
                usuarioDB.UserName = usuario.Email;
                usuarioDB.Estado = usuario.Estado;
                _usuarioService.ActualizarUsuario(usuarioDB);

                var rolesActuales = UserManager.GetRoles(usuario.Id);
                if (!rolesActuales.Contains(rol))
                {
                    if (rolesActuales.Any())
                        UserManager.RemoveFromRoles(usuario.Id, rolesActuales.ToArray());
                    UserManager.AddToRole(usuario.Id, rol);
                }

                TempData["Mensaje"] = "Usuario actualizado correctamente.";
                return RedirectToAction("Index");
            }

            ViewBag.RolesDisponibles = new SelectList(RolesDisponibles, rol);
            return View("~/Views/UsuarioAdmin/Edit.cshtml", usuario);
        }

        public ActionResult Delete(string id)
        {
            var usuario = _usuarioService.ObtenerUsuarioPorId(id);
            if (usuario == null) return HttpNotFound();
            return View("~/Views/UsuarioAdmin/Delete.cshtml", usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (id == User.Identity.GetUserId())
            {
                TempData["Error"] = "No puede eliminar su propia cuenta.";
                return RedirectToAction("Index");
            }

            var usuario = UserManager.FindById(id);
            if (usuario == null)
                return HttpNotFound();

            if (_usuarioService.TieneOrdenes(id))
            {
                TempData["Error"] = "No se puede eliminar el usuario porque tiene órdenes asociadas. Puede desactivarlo desde Editar.";
                return RedirectToAction("Index");
            }

            try
            {
                var result = UserManager.Delete(usuario);
                if (!result.Succeeded)
                {
                    TempData["Error"] = string.Join(" ", result.Errors);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "No se pudo eliminar el usuario porque tiene información asociada (por ejemplo, reseñas). Puede desactivarlo desde Editar.";
                return RedirectToAction("Index");
            }

            TempData["Mensaje"] = "Usuario eliminado correctamente.";
            return RedirectToAction("Index");
        }
    }

    public class CrearUsuarioViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio."), Display(Name = "Nombre completo")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio."), EmailAddress(ErrorMessage = "Correo no válido."), Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria."), StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [DataType(DataType.Password), Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required, Display(Name = "Rol")]
        public string Rol { get; set; }
    }
}
