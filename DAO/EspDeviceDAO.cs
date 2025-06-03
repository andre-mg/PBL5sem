using PBL5sem.Models;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace PBL5sem.DAO
{
    public class EspDeviceDAO : PadraoDAO<EspDeviceViewModel>
    {
        protected override SqlParameter[] CriaParametros(EspDeviceViewModel model)
        {
            return new SqlParameter[]
            {
                new SqlParameter("Id", model.Id),
                new SqlParameter("Nome", model.Nome),
                new SqlParameter("EnderecoIP", model.EnderecoIP),
            };
        }

        protected override EspDeviceViewModel MontaModel(DataRow registro)
        {
            return new EspDeviceViewModel
            {
                Id = Convert.ToInt32(registro["Id"]),
                Nome = registro["Nome"].ToString(),
                EnderecoIP = registro["EnderecoIP"].ToString()
            };
        }

        public override void Delete(int id)
        {
            // Verifica se há estufas associadas (apenas para mostrar na mensagem)
            var estufasAssociadas = HelperDAO.ExecutaProcSelect("spVerificaEstufasPorEsp",
                new SqlParameter[] { new SqlParameter("EspId", id) });

            // Chama a nova procedure
            HelperDAO.ExecutaProc("spDeleteEspDeviceComEstufas",
                new SqlParameter[] { new SqlParameter("EspId", id) });
        }

        protected override void SetTabela()
        {
            Tabela = "EspDevice";
        }
    }
}