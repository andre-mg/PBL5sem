using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using PBL5sem.DAO;
using PBL5sem.Filters;
using PBL5sem.Models;

namespace PBL5sem.Controllers
{
    [AutorizacaoAdmin]
    public class EmpresaController : PadraoController<EmpresaViewModel>
    {
        public EmpresaController()
        {
            DAO = new EmpresaDAO();
            GeraProximoId = true;
            ExigeAutenticacao = true;
        }

        protected override void ValidaDados(EmpresaViewModel model, string operacao)
        {
            base.ValidaDados(model, operacao);

            if (string.IsNullOrEmpty(model.Nome))
                ModelState.AddModelError("Nome", "Preencha o nome da empresa");
        }

        // AJAX - Listar usuários
        [HttpGet]
        public IActionResult GetUsuariosAssociados(int id)
        {
            var usuarios = HelperDAO.ExecutaProcSelect("spVerificaUsuariosPorEmpresa",
                new SqlParameter[] { new SqlParameter("EmpresaId", id) });

            var result = new List<object>();
            foreach (DataRow row in usuarios.Rows)
            {
                result.Add(new
                {
                    id = row["Id"],
                    nome = row["Nome"],
                    email = row["Email"]
                });
            }

            return Json(result);
        }


    }
}