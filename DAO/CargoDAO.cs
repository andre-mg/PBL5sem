using System.Data;
using System.Data.SqlClient;
using PBL5sem.Models;

namespace PBL5sem.DAO
{
    public class CargoDAO : PadraoDAO<CargoViewModel>
    {
        protected override SqlParameter[] CriaParametros(CargoViewModel model)
        {
            return new SqlParameter[]
            {
                new SqlParameter("Id", model.Id),
                new SqlParameter("Nome", model.Nome)
            };
        }

        protected override CargoViewModel MontaModel(DataRow registro)
        {
            return new CargoViewModel
            {
                Id = Convert.ToInt32(registro["Id"]),
                Nome = registro["Nome"].ToString()
            };
        }

        protected override void SetTabela()
        {
            Tabela = "Cargo";
        }
    }
}