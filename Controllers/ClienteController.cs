using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Prova.Models;

namespace Prova.Controllers;

public class ClienteController(IBanco<Cliente> banco) : Controller
{

    public IActionResult Index()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (User.Identity.Name != "Admin")
        {
            return Forbid();
        }

        var lista = banco.Listar();
        return View(lista);
    }


    [HttpGet]
    public IActionResult Cadastro()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated && User.Identity.Name != "Admin")
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Cadastrar(Cliente model)
    {
        if (User.Identity != null && User.Identity.IsAuthenticated && User.Identity.Name != "Admin")
        {
            return RedirectToAction("Index", "Home");
        }

        var clienteExistente = banco.Listar().FirstOrDefault(c => c.CodigoFiscal == model.CodigoFiscal);
        if (clienteExistente != null)
        {
            ModelState.AddModelError("CodigoFiscal", "Já existe um cliente cadastrado com este CPF/CNPJ.");
        }

        if (!string.IsNullOrEmpty(model.InscricaoEstadual))
        {
            var ieExistente = banco.Listar().FirstOrDefault(c => c.InscricaoEstadual == model.InscricaoEstadual);
            if (ieExistente != null)
            {
                ModelState.AddModelError("InscricaoEstadual", "Já existe um cliente cadastrado com esta Inscrição Estadual.");
            }
        }


        if (!string.IsNullOrEmpty(model.Senha))
        {
            var hash = new PasswordHasher<object>();
            model.Senha = hash.HashPassword(new object(), model.Senha);
        }
        else
        {
            var hash = new PasswordHasher<object>();
            model.Senha = hash.HashPassword(new object(), "123");
        }

        if (!ModelState.IsValid)
        {
            return View("Cadastro", model);
        }

        if (model.Imagem != null && model.Imagem.Length > 0)
        {
            if (!ValidarExtensaoImagem(model.Imagem))
            {
                ModelState.AddModelError("Imagem", "Apenas imagens nos formatos .jpg, .jpeg, .png ou .gif são permitidas.");
                return View("Cadastro", model);
            }

            model.ImagemCaminho = SalvarImagemNoServidor(model.Imagem);
        }

        var listaClientes = banco.Listar();
        model.Id = listaClientes.Count > 0 ? listaClientes.Max(c => c.Id) + 1 : 1;

        banco.Adicionar(model);

        if (User.Identity == null || !User.Identity.IsAuthenticated || User.Identity.Name != "Admin")
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, model.CodigoFiscal)
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

        return RedirectToAction("Index", "Cliente");
    }


    [HttpGet]
    public IActionResult Editar(int id)
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        var cliente = banco.Busca(id);
        if (cliente == null)
        {
            return NotFound();
        }

        if (cliente.CodigoFiscal != User.Identity.Name && User.Identity.Name != "Admin")
        {
            return Forbid();
        }

        return View("Cadastro", cliente);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(Cliente model, string? NovaSenha)
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        var clienteAntigo = banco.Busca(model.Id);
        if (clienteAntigo == null)
        {
            return NotFound();
        }

        if (clienteAntigo.CodigoFiscal != User.Identity.Name && User.Identity.Name != "Admin")
        {
            return Forbid();
        }

        var codigoFiscalExistente = banco.Listar().FirstOrDefault(c => c.CodigoFiscal == model.CodigoFiscal && c.Id != model.Id);
        if (codigoFiscalExistente != null)
        {
            ModelState.AddModelError("CodigoFiscal", "Este CPF/CNPJ já está sendo utilizado por outro cliente.");
        }

        if (!string.IsNullOrEmpty(model.InscricaoEstadual))
        {
            var ieExistente = banco.Listar().FirstOrDefault(c => c.InscricaoEstadual == model.InscricaoEstadual && c.Id != model.Id);
            if (ieExistente != null)
            {
                ModelState.AddModelError("InscricaoEstadual", "Esta Inscrição Estadual já está sendo utilizada por outro cliente.");
            }
        }

        if (!string.IsNullOrEmpty(NovaSenha))
        {
            var hash = new PasswordHasher<object>();
            model.Senha = hash.HashPassword(new object(), NovaSenha);
        }
        else
        {
            model.Senha = clienteAntigo.Senha;
        }

        if (!ModelState.IsValid)
        {
            return View("Cadastro", model);
        }

        if (model.Imagem != null && model.Imagem.Length > 0)
        {
            if (!ValidarExtensaoImagem(model.Imagem))
            {
                ModelState.AddModelError("Imagem", "Apenas imagens nos formatos .jpg, .jpeg, .png ou .gif são permitidas.");
                return View("Cadastro", model);
            }

            model.ImagemCaminho = SalvarImagemNoServidor(model.Imagem);
        }
        else
        {
            model.ImagemCaminho = clienteAntigo.ImagemCaminho;
        }

        banco.Alterar(model.Id, model);

        if (User.Identity.Name != "Admin")
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, model.CodigoFiscal)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        return RedirectToAction("Index", "Home");
    }


    [HttpPost]
    public async Task<IActionResult> Excluir(int id)
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        var cliente = banco.Busca(id);
        if (cliente == null)
        {
            return NotFound();
        }

        if (cliente.CodigoFiscal != User.Identity.Name && User.Identity.Name != "Admin")
        {
            return Forbid();
        }

        banco.Excluir(id);

        if (cliente.CodigoFiscal == User.Identity.Name)
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }

        return RedirectToAction("Index", "Home");
    }

    private bool ValidarExtensaoImagem(IFormFile imagem)
    {
        var extensao = Path.GetExtension(imagem.FileName).ToLowerInvariant();
        var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        return extensoesPermitidas.Contains(extensao);
    }

    private string SalvarImagemNoServidor(IFormFile imagem)
    {
        var extensao = Path.GetExtension(imagem.FileName).ToLowerInvariant();
        string pastaImagem = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagens");

        if (!Directory.Exists(pastaImagem))
        {
            Directory.CreateDirectory(pastaImagem);
        }

        string nomeUnico = Guid.NewGuid().ToString() + extensao;
        string caminhoCompleto = Path.Combine(pastaImagem, nomeUnico);

        using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
        {
            imagem.CopyTo(stream);
        }

        return $"/imagens/{nomeUnico}";
    }
}
