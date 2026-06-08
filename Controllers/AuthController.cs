using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Prova.Models;

namespace exercicio3.Controllers;

public class AuthController(ILogger<AuthController> logger, IBanco<Cliente> banco) : Controller
{
    [HttpGet]
    public IActionResult Login()
    {


        var adminExistente = banco.Listar().FirstOrDefault(p => p.CodigoFiscal == "Admin");
        if (adminExistente == null)
        {
            var hash = new PasswordHasher<object>();
            var dummy = new object();
            var senhaHash = hash.HashPassword(dummy, "123");

            var usr = new Cliente
            {
                CodigoFiscal = "Admin",
                Nome = "Administrador",
                Endereco = "N/A",
                Numero = "0",
                Bairro = "N/A",
                Cidade = "N/A",
                Estado = "SP",
                DataNascimento = DateTime.Now,
                Senha = senhaHash
            };

            banco.Adicionar(usr);
            logger.LogInformation("Usuário Administrador padrão criado com sucesso.");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string Login, string Senha)
    {
        var usuario = banco.Listar().FirstOrDefault(p => p.CodigoFiscal == Login);

        if (usuario == null)
        {
            ViewBag.Erro = "Usuário Invalido";
            return View();
        }

        var hash = new PasswordHasher<object>();
        var dummy = new object();

        var result = hash.VerifyHashedPassword(dummy, usuario.Senha, Senha);

        if (result == PasswordVerificationResult.Success)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, Login)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );
            return RedirectToAction("Index", "Home");
        }
        else
        {
            ViewBag.Erro = "Usuário Invalido";
            return View();
        }

    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        logger.LogInformation("Usuário deslogado.");
        return RedirectToAction("Login");
    }
}
