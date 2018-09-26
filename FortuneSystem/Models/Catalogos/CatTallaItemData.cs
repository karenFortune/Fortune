using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatTallaItemData
    {
        private Conexion conn = new Conexion();
        private SqlCommand comando = new SqlCommand();
        private SqlDataReader leer = null;

        //Muestra la lista de tallas
        public IEnumerable<CatTallaItem> ListaTallas()
        {
            List<CatTallaItem> listTallas = new List<CatTallaItem>();
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "Listar_Tallas";
            comando.CommandType = CommandType.StoredProcedure;
            leer = comando.ExecuteReader();

            while (leer.Read())
            {
                CatTallaItem tallas = new CatTallaItem()
                {
                    Id = Convert.ToInt32(leer["ID"]),
                    Talla = leer["TALLA"].ToString()
                };

                listTallas.Add(tallas);
            }
            leer.Close();
            conn.CerrarConexion();

            return listTallas;
        }
        

        public List<String> Lista_tallas()
        {
            Conexion con = new Conexion();
            SqlCommand com = new SqlCommand();
            SqlDataReader leer = null;
            //string listaStag="";
            List<String> Lista = new List<String>();
            com.Connection = con.AbrirConexion();
            com.CommandText = "SELECT TALLA from CAT_ITEM_SIZE order by TALLA asc ";
            leer = com.ExecuteReader();
            while (leer.Read())
            {

                Lista.Add(leer["TALLA"].ToString());
            }
            leer.Close();
            con.CerrarConexion();
            //return listaStag;
            return Lista;


        }


        //Permite crear una nueva talla
        public void AgregarTallas(CatTallaItem tallas)
        {
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "AgregarTalla";
            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue("@Talla", tallas.Talla);

            comando.ExecuteNonQuery();
            conn.CerrarConexion();

        }

        //Permite consultar los detalles de una talla
        public CatTallaItem ConsultarListaTallas(int? id)
        {
            CatTallaItem tallas = new CatTallaItem();

            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "Listar_Talla_Por_Id";
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("@Id", id);

            leer = comando.ExecuteReader();
            while (leer.Read())
            {

                tallas.Id = Convert.ToInt32(leer["ID"]);
                tallas.Talla = leer["TALLA"].ToString();

            }
            return tallas;

        }

        //Permite actualiza la informacion de una talla
        public void ActualizarTallas(CatTallaItem tallas)
        {
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "Actualizar_Talla";
            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue("@Id", tallas.Id);
            comando.Parameters.AddWithValue("@Talla", tallas.Talla);

            comando.ExecuteNonQuery();
            conn.CerrarConexion();
        }

        //Permite eliminar la informacion de una talla
        public void EliminarTallas(int? id)
        {
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "EliminarTalla";
            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue("@Id", id);

            comando.ExecuteNonQuery();
            conn.CerrarConexion();
        }


    }
}

