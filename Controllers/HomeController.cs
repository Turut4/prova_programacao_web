using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Prova.Models;

namespace Prova.Controllers;

public class HomeController(IBanco<Cliente> banco) : Controller
{

    public IActionResult Index()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (User.Identity.Name == "Admin")
        {
            var todosClientes = banco.Listar();
            ViewBag.IsAdmin = true;
            return View(todosClientes);
        }
        else
        {
            var clienteLogado = banco.Listar().FirstOrDefault(c => c.CodigoFiscal == User.Identity.Name);
            ViewBag.IsAdmin = false;

            var lista = new List<Cliente>();
            if (clienteLogado != null)
            {
                lista.Add(clienteLogado);
            }
            return View(lista);
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
