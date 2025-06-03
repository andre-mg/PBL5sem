using System.Data.SqlClient;
using System.Data;
using PBL5sem.Models;
using System.Diagnostics;

namespace PBL5sem.DAO
{
    public abstract class PadraoDAO<T> where T : PadraoViewModel
    {
        public PadraoDAO()
        {
            SetTabela();
        }

        protected string Tabela { get; set; }
        protected string NomeSpListagem { get; set; } = "spListagem";
        protected string NomeSpConsulta { get; set; } = "spConsulta";
        protected abstract SqlParameter[] CriaParametros(T model);
        protected abstract T MontaModel(DataRow registro);
        protected abstract void SetTabela();

        public virtual void Insert(T model)
        {
            HelperDAO.ExecutaProc("spInsert_" + Tabela, CriaParametros(model));
        }
        public virtual void Update(T model)
        {

            HelperDAO.ExecutaProc("spUpdate_" + Tabela, CriaParametros(model));
        }
        public virtual void Delete(int id)
        {
            try
            {
                var p = new SqlParameter[]
                {
                    new SqlParameter("id", id),
                    new SqlParameter("tabela", Tabela)
                };
                HelperDAO.ExecutaProc("spDelete", p);
            }
            catch (SqlException ex)
            {
                // Log do erro completo
                Debug.WriteLine($"ERRO AO DELETAR: {ex.Message}");
                throw new Exception($"Não foi possível excluir o registro. Erro: {ex.Message}");
            }
        }
        public virtual int ProximoId()
        {
            var p = new SqlParameter[]
            {
                new SqlParameter("tabela", Tabela)
            };
            var tabela = HelperDAO.ExecutaProcSelect("spProximoId", p);
            return Convert.ToInt32(tabela.Rows[0][0]);
        }
        public virtual List<T> Listagem(string ordem = "1")
        {
            SqlParameter[] parametros = null;

            // Se não foi definida uma SP customizada, usa a padrão com parâmetros
            if (string.IsNullOrEmpty(NomeSpListagem) || NomeSpListagem == "spListagem")
            {
                parametros = new SqlParameter[]
                {
                    new SqlParameter("tabela", Tabela),
                    new SqlParameter("ordem", ordem)
                };
                // Garante que usa o nome padrão
                NomeSpListagem = "spListagem";
            }
            // Se foi definida uma SP customizada, não envia parâmetros

            var tabela = HelperDAO.ExecutaProcSelect(NomeSpListagem, parametros);
            List<T> lista = new List<T>();

            foreach (DataRow registro in tabela.Rows)
                lista.Add(MontaModel(registro));

            return lista;
        }

        public virtual T Consulta(int id)
        {
            SqlParameter[] parametros;

            if (string.IsNullOrEmpty(NomeSpConsulta) || NomeSpConsulta == "spConsulta")
            {
                parametros = new SqlParameter[]
                {
                    new SqlParameter("id", id),
                    new SqlParameter("tabela", Tabela)
                };
            }
            else
            {
                parametros = new SqlParameter[] { new SqlParameter("id", id) };
            }

            try
            {
                var tabela = HelperDAO.ExecutaProcSelect(NomeSpConsulta, parametros);

                if (tabela.Rows.Count == 0)
                {
                    return null;
                }

                return MontaModel(tabela.Rows[0]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERRO na consulta: {ex.Message}");
                throw;
            }
        }
    }
}
