using Microsoft.AspNetCore.Mvc;
using PBL5sem.Controllers;
using PBL5sem.DAO;
using PBL5sem.Models;

public class MonitoramentoController : Controller
{
    private readonly MonitoramentoDAO _monitoramentoDAO = new();
    private readonly EstufaDAO _estufaDAO = new();

    public IActionResult Index(int id)
    {
        if (!HelperControllers.VerificaUserLogado(HttpContext.Session))
            return RedirectToAction("Index", "Login");

        var estufa = _estufaDAO.Consulta(id);
        if (estufa == null) return NotFound();

        return View(estufa);
    }

    [HttpPost]
    public IActionResult SalvarMonitoramento(int estufaId, decimal temperatura)
    {
        try
        {
            if (!HelperControllers.VerificaUserLogado(HttpContext.Session))
                return Json(new { success = false, error = "Acesso negado" });

            var model = new MonitoramentoViewModel
            {
                EstufaId = estufaId,
                DataHora = DateTime.UtcNow,
                Temperatura = temperatura
            };

            // Garante que o ID seja gerado antes da inserção
            model.Id = _monitoramentoDAO.ProximoId();

            _monitoramentoDAO.Insert(model);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = ex.Message });
        }
    }
}