using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using PBL5sem.DAO;
using PBL5sem.Filters;
using PBL5sem.Models;

namespace PBL5sem.Controllers
{
    public interface IFileUpload
    {
        void ProcessFile(IFormFile file);
    }
    public abstract class PadraoController<T> : Controller where T : PadraoViewModel
    {
        protected PadraoDAO<T> DAO { get; set; }
        protected bool GeraProximoId { get; set; }
        protected string NomeViewIndex { get; set; } = "index";
        protected string NomeViewForm { get; set; } = "form";
        protected bool ExigeAutenticacao { get; set; } = true;

        public virtual IActionResult Index()
        {
            try
            {
                var lista = DAO.Listagem();

                return View(NomeViewIndex, lista);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }
        public virtual IActionResult Create()
        {
            try
            {

                ViewBag.Operacao = "I";
                T model = Activator.CreateInstance<T>();
                PreencheDadosParaView("I", model);
                return View(NomeViewForm, model);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }
        protected virtual void PreencheDadosParaView(string Operacao, T model)
        {
            if (GeraProximoId && Operacao == "I")
                model.Id = DAO.ProximoId();
        }

        public virtual IActionResult Save(T model, string Operacao)
        {
            try
            {
                // Processa arquivo se houver e se o modelo implementar IFileUpload
                if (model is IFileUpload fileUploadModel)
                {
                    var file = Request.Form.Files.FirstOrDefault();
                    if (file != null && file.Length > 0)
                    {
                        fileUploadModel.ProcessFile(file);
                    }
                }

                ValidaDados(model, Operacao);

                if (ModelState.IsValid)
                {
                    if (Operacao == "I")
                        DAO.Insert(model);
                    else
                        DAO.Update(model);

                    // Verifica se é uma requisição AJAX
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true });
                    }
                    return RedirectToAction("Index"); // Adicionado retorno para requisições não-AJAX
                }
                else
                {
                    ViewBag.Operacao = Operacao;
                    PreencheDadosParaView(Operacao, model);

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        var errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage);
                        return Json(new { success = false, errors });
                    }
                    return View(NomeViewForm, model);
                }
            }
            catch (Exception erro)
            {
                Debug.WriteLine($"O erro foi {erro}");

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, error = erro.Message });
                }
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        protected virtual void ValidaDados(T model, string operacao)
        {
            ModelState.Clear();
            if (operacao == "I" && DAO.Consulta(model.Id) != null)
                ModelState.AddModelError("Id", "Código já está em uso!");
            if (operacao == "A" && DAO.Consulta(model.Id) == null)
                ModelState.AddModelError("Id", "Este registro não existe!");
            if (model.Id <= 0)
                ModelState.AddModelError("Id", "Id inválido!");
        }

        [AutorizacaoAdmin]
        public IActionResult Edit(int id)
        {
            try
            {
                ViewBag.Operacao = "A";
                var model = DAO.Consulta(id);
                if (model == null)
                    return RedirecionaParaIndex(model);
                else
                {
                    PreencheDadosParaView("A", model);
                    return View(NomeViewForm, model);
                }
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }


        protected virtual IActionResult RedirecionaParaIndex(T model)
        {
            return RedirectToAction(NomeViewIndex);
        }

        [AutorizacaoAdmin]
        public IActionResult Delete(int id)
        {
            try
            {

                var model = DAO.Consulta(id);
                return RedirectToAction("Index");
            }
            catch (Exception erro)
            {
                Debug.WriteLine($"ERRO: {erro}"); // Log 5
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (ExigeAutenticacao && !HelperControllers.VerificaUserLogado(HttpContext.Session))
                context.Result = RedirectToAction("Index", "Login");
            else
            {
                ViewBag.Logado = true;
                ViewBag.UsuarioNome = HttpContext.Session.GetString("UsuarioNome");
                base.OnActionExecuting(context);
            }
        }
        protected IActionResult JsonSaveResult(bool success, string errorMessage = null)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success, error = errorMessage });
            }
            return success ? RedirectToAction("Index") : View(NomeViewForm);
        }
    }
}
