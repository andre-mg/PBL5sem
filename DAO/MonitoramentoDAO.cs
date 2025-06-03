using PBL5sem.Models;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace PBL5sem.DAO
{
    public class MonitoramentoDAO : PadraoDAO<MonitoramentoViewModel>
    {
        public MonitoramentoDAO()
        {
            // Configura o nome da tabela e procedures personalizadas
            NomeSpListagem = "spListagemMonitoramentoPorEstufa";
        }

        protected override void SetTabela()
        {
            Tabela = "MonitoramentoEstufa";
        }

        protected override SqlParameter[] CriaParametros(MonitoramentoViewModel model)
        {
            // Garante que o ID seja gerado corretamente
            if (model.Id == 0)
                model.Id = ProximoId();

            return new SqlParameter[]
            {
            new SqlParameter("@Id", model.Id),
            new SqlParameter("@EstufaId", model.EstufaId),
            new SqlParameter("@DataHora", model.DataHora),
            new SqlParameter("@Temperatura", model.Temperatura)
            };
        }

        protected override MonitoramentoViewModel MontaModel(DataRow registro)
        {
            return new MonitoramentoViewModel
            {
                Id = Convert.ToInt32(registro["Id"]),
                EstufaId = Convert.ToInt32(registro["EstufaId"]),
                DataHora = Convert.ToDateTime(registro["DataHora"]),
                Temperatura = Convert.ToDecimal(registro["Temperatura"])
            };
        }

        public List<MonitoramentoViewModel> Listagem(int estufaId, int ultimosRegistros = 50)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
            new SqlParameter("@EstufaId", estufaId),
            new SqlParameter("@UltimosRegistros", ultimosRegistros)
                };

                var tabela = HelperDAO.ExecutaProcSelect("spListagemMonitoramentoPorEstufa", parametros);
                var lista = new List<MonitoramentoViewModel>();

                foreach (DataRow registro in tabela.Rows)
                {
                    lista.Add(new MonitoramentoViewModel
                    {
                        Id = Convert.ToInt32(registro["Id"]),
                        EstufaId = Convert.ToInt32(registro["EstufaId"]),
                        DataHora = Convert.ToDateTime(registro["DataHora"]),
                        Temperatura = Convert.ToDecimal(registro["Temperatura"])
                    });
                }

                return lista.OrderBy(m => m.DataHora).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao listar monitoramento: {ex.Message}");
                return new List<MonitoramentoViewModel>();
            }
        }

        public MonitoramentoViewModel UltimaLeitura(int estufaId)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@EstufaId", estufaId)
            };

            var tabela = HelperDAO.ExecutaProcSelect("spUltimaLeituraMonitoramento", parametros);

            if (tabela.Rows.Count == 0)
                return null;

            return MontaModel(tabela.Rows[0]);
        }


        public bool ExisteRegistroComDataHora(int estufaId, DateTime dataHora)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@EstufaId", estufaId),
                new SqlParameter("@DataHora", dataHora)
            };

            var tabela = HelperDAO.ExecutaProcSelect("spVerificaExistenciaDataHora", parametros);
            return tabela.Rows.Count > 0;
        }
    }
}