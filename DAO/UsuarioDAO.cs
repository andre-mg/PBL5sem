using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Org.BouncyCastle.Crypto.Generators;
using PBL5sem.Models;

namespace PBL5sem.DAO
{
    public class UsuarioDAO : PadraoDAO<UsuarioViewModel>
    {
        protected override SqlParameter[] CriaParametros(UsuarioViewModel model)
        {
            return new SqlParameter[]
            {
                new SqlParameter("Id", model.Id),
                new SqlParameter("Nome", model.Nome),
                new SqlParameter("Email", model.Email),
                new SqlParameter("Senha", BCrypt.Net.BCrypt.HashPassword(model.Senha)),
                new SqlParameter("EmpresaId", model.EmpresaId),
                new SqlParameter("CargoId", model.CargoId)
            };
        }

        protected override UsuarioViewModel MontaModel(DataRow registro)
        {
            return new UsuarioViewModel
            {
                Id = Convert.ToInt32(registro["Id"]),
                Nome = registro["Nome"].ToString(),
                Email = registro["Email"].ToString(),
                Senha = registro["Senha"].ToString(),
                EmpresaId = Convert.ToInt32(registro["EmpresaId"]),
                CargoId = Convert.ToInt32(registro["CargoId"]),
                NomeEmpresa = registro["NomeEmpresa"].ToString(),
                LogoEmpresa = registro["LogoEmpresa"] != DBNull.Value ? (byte[])registro["LogoEmpresa"] : null,
                NomeCargo = registro["NomeCargo"].ToString()
            };
        }


        protected override void SetTabela()
        {
            Tabela = "Usuario";
            NomeSpListagem = "spListagemUsuarioEmpresa";
        }
       
        public UsuarioViewModel ConsultaPorEmail(string email)
        {
            // Debug: mostra qual procedure está sendo usada
            Debug.WriteLine($"Consultando usuário por email: {email}");

            var parametros = new SqlParameter[] { new SqlParameter("Email", email) };
            var tabela = HelperDAO.ExecutaProcSelect("spConsultaUsuarioPorEmail", parametros);

            // Debug: mostra informações sobre o resultado
            Debug.WriteLine($"Resultado obtido - {tabela.Rows.Count} linhas");
            if (tabela.Rows.Count > 0)
            {
                Debug.WriteLine("Colunas disponíveis no resultado:");
                foreach (DataColumn col in tabela.Columns)
                {
                    Debug.WriteLine($"- {col.ColumnName} ({col.DataType})");
                }
            }

            return tabela.Rows.Count == 0 ? null : MontaModel(tabela.Rows[0]);
        }
    }
}
