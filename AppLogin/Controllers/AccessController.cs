using Microsoft.AspNetCore.Mvc;
using AppLogin.Context;
using AppLogin.Models;
using Microsoft.EntityFrameworkCore;
using AppLogin.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AppLogin.Controllers
{
    public class AccessController : Controller
    {
        private readonly AppDBContext _dbContext;
        public AccessController(AppDBContext appDBContext)
        {
            _dbContext = appDBContext;
        }

        [HttpGet]
        public IActionResult Registrarse()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registrarse(UsuarioVM modelo)
        {
            if (modelo.Password != modelo.ConfirmarPassword) {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }
            Usuario usuario = new Usuario()
            {
                Name = modelo.Name,
                Email = modelo.Email,
                Password= modelo.Password,
                Username = modelo.Username,
            };
            await _dbContext.Usuarios.AddAsync(usuario);
            await _dbContext.SaveChangesAsync();

            if (usuario.Id != 0)  return RedirectToAction("Login", "Access");
            ViewData["Mensaje"] = "No se pudo crear el usuario";
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated) 
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM modelo)
        {
            Usuario? usuario = await _dbContext.Usuarios
                .Where(u =>
                u.Username == modelo.Username &&
                u.Password == modelo.Password)
                .FirstOrDefaultAsync();
            if (usuario == null) 
            {
                ViewData["Mensaje"] = "El usuario o la contraseña es incorrecta";
                return View();
            }
            
            List<Claim> claims = new List<Claim>() 
            {
                new Claim(ClaimTypes.Name, usuario.Name)

            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), properties
                );
            return RedirectToAction("Index", "Home");
          
        }
    }
}
