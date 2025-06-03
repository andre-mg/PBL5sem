using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using PBL5sem.DAO;
using PBL5sem.Filters;
using PBL5sem.Models;

namespace PBL5sem.Controllers
{
    public class EspDeviceController : PadraoController<EspDeviceViewModel>
    {
        public EspDeviceController()
        {
            DAO = new EspDeviceDAO();
            GeraProximoId = true;
        }

        [AutorizacaoAdmin]
        public override IActionResult Create()
        {
            return base.Create();
        }

        protected override void PreencheDadosParaView(string Operacao, EspDeviceViewModel model)
        {
            base.PreencheDadosParaView(Operacao, model);

            // Carrega estufas disponíveis para associação
            var estufaDAO = new EstufaDAO();
            ViewBag.EstufasDisponiveis = estufaDAO.Listagem();
        }

        protected override void ValidaDados(EspDeviceViewModel model, string operacao)
        {
            base.ValidaDados(model, operacao);
            ModelState.Clear();

            if (string.IsNullOrWhiteSpace(model.Nome))
                ModelState.AddModelError("Nome", "Preencha o nome do ESP");

            if (string.IsNullOrWhiteSpace(model.EnderecoIP))
                ModelState.AddModelError("EnderecoIp", "Preencha o endereço IP");
        }

        // AJAX -> Buscar estufas associadas ao ESP em casode delete
        [HttpGet]
        public IActionResult GetEstufasAssociadas(int id)
        {
            var estufas = HelperDAO.ExecutaProcSelect("spVerificaEstufasPorEsp",
                new SqlParameter[] { new SqlParameter("EspId", id) });

            var result = new List<object>();
            foreach (DataRow row in estufas.Rows)
            {
                result.Add(new
                {
                    id = row["Id"],
                    nome = row["Nome"]
                });
            }

            return Json(result);
        }
    }
}