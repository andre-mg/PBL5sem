using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PBL5sem.DAO;
using PBL5sem.Filters;
using PBL5sem.Models;

namespace PBL5sem.Controllers
{
    [AutorizacaoAdmin]
    public class UsuarioController : PadraoController<UsuarioViewModel>
    {
        public UsuarioController()
        {
            DAO = new UsuarioDAO();
            GeraProximoId = true;
            ExigeAutenticacao = true;
        }

        protected override void PreencheDadosParaView(string Operacao, UsuarioViewModel model)
        {
            base.PreencheDadosParaView(Operacao, model);

            if (model.EmpresaId > 0)
            {
                var empresaDAO = new EmpresaDAO();
                var empresa = empresaDAO.Consulta(model.EmpresaId);

                if (empresa != null)
                {
                    model.NomeEmpresa = empresa.Nome ?? model.NomeEmpresa;
                    model.LogoEmpresa = empresa.Logo;
                }
            }

            // Completa dados do Cargo
            if (model.CargoId > 0)
            {
                var cargoDAO = new CargoDAO();
                var cargo = cargoDAO.Consulta(model.CargoId);
                model.NomeCargo = cargo?.Nome ?? model.NomeCargo;
            }

            ViewBag.Empresas = new EmpresaDAO().Listagem();
            ViewBag.Cargos = new CargoDAO().Listagem();
        }

        protected override void ValidaDados(UsuarioViewModel model, string operacao)
        {
            base.ValidaDados(model, operacao);

            if (string.IsNullOrEmpty(model.Nome))
                ModelState.AddModelError("Nome", "Preencha o nome completo");

            if (string.IsNullOrEmpty(model.Email))
                ModelState.AddModelError("Email", "Preencha o e-mail");

            if (string.IsNullOrEmpty(model.Senha) && operacao == "I")
                ModelState.AddModelError("Senha", "Defina uma senha");

            if (model.EmpresaId <= 0)
                ModelState.AddModelError("EmpresaId", "Selecione uma empresa");

            if (model.CargoId <= 0)
                ModelState.AddModelError("CargoId", "Selecione um cargo");
        }
    }
}