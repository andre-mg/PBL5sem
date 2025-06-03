using Microsoft.AspNetCore.Mvc;
using PBL5sem.DAO;
using PBL5sem.Models;
using System;

namespace PBL5sem.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FazLogin(string email, string senha)
        {
            try
            {
                var usuarioDAO = new UsuarioDAO();
                var usuario = usuarioDAO.ConsultaPorEmail(email);

                if (usuario != null && BCrypt.Net.BCrypt.Verify(senha, usuario.Senha))
                {
                    HttpContext.Session.SetString("Logado", "true");
                    HttpContext.Session.SetString("UsuarioEmail", usuario.Email);
                    HttpContext.Session.SetString("UsuarioNome", usuario.Nome);
                    HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
                    HttpContext.Session.SetInt32("UsuarioCargoId", usuario.CargoId);

                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Erro = "Usuário ou senha inválidos!";
                return View("Index");
            }
            catch (Exception erro)
            {
                ViewBag.Erro = erro.Message;
                return View("Index");
            }
        }

        public IActionResult LogOff()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        public IActionResult AcessoNegado()
        {
            return View(); // CRIAR uma view com mensagem amigável
        }
    }
}