using System.Data;
using System.Data.SqlClient;
using PBL5sem.Models;

namespace PBL5sem.DAO
{
    public class EmpresaDAO : PadraoDAO<EmpresaViewModel>
    {
        protected override SqlParameter[] CriaParametros(EmpresaViewModel model)
        {
            var logoParam = new SqlParameter("Logo", SqlDbType.VarBinary);
            logoParam.Value = (object)model.Logo ?? DBNull.Value;

            return new SqlParameter[]
            {
                new SqlParameter("Id", model.Id),
                new SqlParameter("Nome", model.Nome),
                logoParam
            };
        }


        protected override EmpresaViewModel MontaModel(DataRow registro)
        {
            return new EmpresaViewModel
            {
                Id = Convert.ToInt32(registro["Id"]),
                Nome = registro["Nome"].ToString(),
                Logo = registro["Logo"] != DBNull.Value ? (byte[])registro["Logo"] : null
            };
        }

        protected override void SetTabela()
        {
            Tabela = "Empresa";
        }

        public override void Delete(int id)
        {
            HelperDAO.ExecutaProc("spDeleteEmpresaComUsuarios",
                new SqlParameter[] { new SqlParameter("EmpresaId", id) });

            base.Delete(id);
        }
    }
}