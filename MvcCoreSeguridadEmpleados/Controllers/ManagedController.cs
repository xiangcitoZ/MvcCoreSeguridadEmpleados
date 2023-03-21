using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcCoreSeguridadEmpleados.Models;
using MvcCoreSeguridadEmpleados.Repositories;
using System.Security.Claims;

namespace MvcCoreSeguridadEmpleados.Controllers
{
    public class ManagedController : Controller
    {
        private RepositoryEmpleados repo;
        public ManagedController(RepositoryEmpleados repo)
        {
            this.repo = repo;
        }
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn
            (string username, string password)
        {
            Empleado empleado =
                await this.repo.ExisteEmpleado(username, int.Parse(password));
            if (empleado != null)
            {
                ClaimsIdentity identity =
                    new ClaimsIdentity
                    (CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimTypes.Name, ClaimTypes.Role);

                Claim claimName = new Claim(ClaimTypes.Name, username);
                identity.AddClaim(claimName);

                Claim claimId = new Claim(ClaimTypes.NameIdentifier, 
                    empleado.IdEmpleado.ToString());
                identity.AddClaim(claimId);

                Claim claimOfICIO = new Claim(ClaimTypes.Role,
                   empleado.Oficio);
                identity.AddClaim(claimOfICIO);

                Claim claimSalario = new Claim("Salario",
                   empleado.Salario.ToString());
                identity.AddClaim(claimSalario);

                Claim claimDepartamento = new Claim("Departamento",
                   empleado.Departamento.ToString());
                identity.AddClaim(claimDepartamento);

                ClaimsPrincipal userPrincipal =
                    new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync
                    (CookieAuthenticationDefaults.AuthenticationScheme
                    , userPrincipal);
                string controller = TempData["controller"].ToString();
                string action = TempData["action"].ToString();


                return RedirectToAction("PerfilEmpleado", "Empleados");
            }
            else
            {
                ViewData["MENSAJE"] = "Usuario/Password incorrectos";
                return View();
            }
        }

        public IActionResult ErrorAcceso()
        {
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync
                (CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("PerfilEmpleado", "Empleados");
        }

    }
}
