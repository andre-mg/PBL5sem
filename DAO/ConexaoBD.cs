using System.Data.SqlClient;

namespace PBL5sem.DAO
{
    static class ConexaoBD
    {
        public static SqlConnection GetConexao()
        {
            string strCon = "Server=localhost;Database=PBL5sem;User Id=sa;Password=123456;TrustServerCertificate=True;";
            SqlConnection conexao = new SqlConnection(strCon);
            conexao.Open();
            return conexao;
        }
    }
}

