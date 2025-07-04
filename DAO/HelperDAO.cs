﻿using System.Data.SqlClient;
using System.Data;

namespace PBL5sem.DAO
{
    public static class HelperDAO
    {
        public static void ExecutaSQL(string sql, SqlParameter[] parametros)

        {
            using (SqlConnection conexao = ConexaoBD.GetConexao())

            {
                using (SqlCommand comando = new SqlCommand(sql, conexao))

                {
                    if (parametros != null
                   )
                        comando.Parameters.AddRange(parametros);
                    comando.ExecuteNonQuery();

                }
                conexao.Close();

            }

        }


        public static DataTable ExecutaSelect(string sql, SqlParameter[] parametros)
        {
            using (SqlConnection conexao = ConexaoBD.GetConexao())
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(sql, conexao))
                {
                    if (parametros != null)
                        adapter.SelectCommand.Parameters.AddRange(parametros);
                    DataTable tabela = new DataTable();
                    adapter.Fill(tabela);
                    conexao.Close();
                    return tabela;
                }

            }

        }



        public static void ExecutaProc(string nomeSP, SqlParameter[] parametros)
        {
            using (SqlConnection conexao = ConexaoBD.GetConexao())
            {
                using (SqlCommand comando = new SqlCommand(nomeSP, conexao))
                {
                    comando.CommandType = CommandType.StoredProcedure;
                    if (parametros != null)
                        comando.Parameters.AddRange(parametros);
                    comando.ExecuteNonQuery();
                }
                conexao.Close();
            }
        }


        public static DataTable ExecutaProcSelect(string nomeSP, SqlParameter[] parametros)

        {
            using (SqlConnection conexao = ConexaoBD.GetConexao())
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(nomeSP, conexao))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    if (parametros != null)
                        adapter.SelectCommand.Parameters.AddRange(parametros);
                    DataTable tabela = new DataTable();
                    adapter.Fill(tabela);
                    conexao.Close();
                    return tabela;
                }
            }
        }


    }
}
