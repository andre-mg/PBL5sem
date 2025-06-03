using PBL5sem.Models;
using System.Data;
using System.Data.SqlClient;

namespace PBL5sem.DAO
{
    public class EstufaDAO : PadraoDAO<EstufaViewModel>
    {
        protected override SqlParameter[] CriaParametros(EstufaViewModel model)
        {
            return new SqlParameter[]
            {
                new SqlParameter("Id", model.Id),
                new SqlParameter("Nome", model.Nome),
                new SqlParameter("TemperaturaIdeal", model.TemperaturaIdeal),
                new SqlParameter("AwsIP", model.AwsIP),
                new SqlParameter("EspId", model.EspId)
            };
        }

        protected override EstufaViewModel MontaModel(DataRow registro)
        {

            var model = new EstufaViewModel
            {
                Id = Convert.ToInt32(registro["Id"]),
                Nome = registro["Nome"].ToString(),
                TemperaturaIdeal = Convert.ToDecimal(registro["TemperaturaIdeal"]),
                AwsIP = registro["AwsIP"].ToString(),
                EspId = Convert.ToInt32(registro["EspId"]),
                EspEnderecoIP = registro["EspEnderecoIP"].ToString()
            };
            return model;
        }

        protected override void SetTabela()
        {
            Tabela = "Estufa";
            NomeSpListagem = "spListagemEstufaEsp";
            NomeSpConsulta = "spConsultaEstufa";
        }

    }
}