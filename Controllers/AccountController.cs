using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WAMVC.Data;
using WAMVC.Models;

namespace WAMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ArtesaniasDBContext _context;
        private readonly PasswordHasher<Usuario> _passwordHasher;

        public AccountController(ArtesaniasDBContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Usuario>();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Email y contraseña son obligatorios.";
                return View();
            }

            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email && u.Activo);
            if (user == null)
            {
                ViewBag.Error = "Email o contraseña incorrectos";
                return View();
            }

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            var passwordMatches = verifyResult == PasswordVerificationResult.Success
                                  || string.Equals(user.Password, password); // fallback si en la DB hay contraseñas en claro

            if (!passwordMatches)
            {
                ViewBag.Error = "Email o contraseña incorrectos";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, string.IsNullOrWhiteSpace(user.NombreCompleto) ? user.Email : user.NombreCompleto),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Rol ?? "Usuario")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string email, string password, string nombreCompleto)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Email y contraseña son obligatorios.");
                return View();
            }

            var exists = await _context.Usuarios.AnyAsync(u => u.Email == email);
            if (exists)
            {
                ModelState.AddModelError(string.Empty, "Ya existe un usuario con ese email.");
                return View();
            }

            var user = new Usuario
            {
                Email = email,
                NombreCompleto = nombreCompleto ?? string.Empty,
                Rol = "Usuario",
                Activo = true
            };

            user.Password = _passwordHasher.HashPassword(user, password);

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, string.IsNullOrWhiteSpace(user.NombreCompleto) ? user.Email : user.NombreCompleto),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Rol ?? "Usuario")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
