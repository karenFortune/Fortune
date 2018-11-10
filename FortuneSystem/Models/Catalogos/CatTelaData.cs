using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatTelaData
    {
        private Conexion conn = new Conexion();
        private SqlCommand comando = new SqlCommand();
        private SqlDataReader leer = null;


        public IEnumerable<CatTela> ListaTela()
        {
            List<CatTela> listTela = new List<CatTela>();
            try
            {
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Listar_Tela";
                comando.CommandType = CommandType.StoredProcedure;
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    CatTela tela = new CatTela()
                    {
                        Id_Tela = Convert.ToInt32(leer["ID"]),
                        Tela = leer["FABRIC"].ToString(),
                        CodigoTela = leer["CODE"].ToString()
                    };
                    listTela.Add(tela);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listTela;
        }

        //Permite crear una nueva tela
        public void AgregarTelas(CatTela telas)
        {
            try
            {
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "AgregarTela";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Tela", telas.Tela);
                comando.Parameters.AddWithValue("@Codigo", telas.CodigoTela);
                comando.ExecuteNonQuery();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

        }

        //Permite consultar los detalles de una tela
        public CatTela ConsultarListaTelas(int? id)
        {
            CatTela telas = new CatTela();
            try
            {
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Listar_Tela_Por_Id";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    telas.Id_Tela = Convert.ToInt32(leer["ID"]);
                    telas.Tela = leer["FABRIC"].ToString();
                    telas.CodigoTela = leer["CODE"].ToString();
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
            return telas;

        }

        //Permite actualiza la informacion de una tela
        public void ActualizarTelas(CatTela telas)
        {
            try
            {
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Actualizar_Tela";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", telas.Id_Tela);
                comando.Parameters.AddWithValue("@Tela", telas.Tela);
                comando.Parameters.AddWithValue("@Codigo", telas.CodigoTela);
                comando.ExecuteNonQuery();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
        }

        //Permite eliminar la informacion de una tela
        public void EliminarTelas(int? id)
        {
            try
            {
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "EliminarTela";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                comando.ExecuteNonQuery();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
        }

        public int ObtenerIdTela(string tela)
        {
            int idTela = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT ID FROM CAT_FABRIC_CODES " +
                                     "WHERE FABRIC='" + tela + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    idTela += Convert.ToInt32(leerF["ID"]);
                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return idTela;
        }
    }
}