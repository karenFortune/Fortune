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
        //Permite obtener el id de un talla
        public int ObtenerIdTalla(string talla)
        {
            int idTalla = 0;
            Conexion conex = new Conexion();
            SqlCommand coman = new SqlCommand();
            SqlDataReader leerF = null;
            coman.Connection = conex.AbrirConexion();
            coman.CommandText = "select ID from CAT_ITEM_SIZE WHERE TALLA='" + talla + "'";
            leerF = coman.ExecuteReader();
            while (leerF.Read())
            {
                idTalla += Convert.ToInt32(leerF["ID"]);
            }
            leerF.Close();
            conex.CerrarConexion();
            return idTalla;
        }

        //Permite eliminar la informacion de una talla por estilo
        public void EliminarTallasIdEstilo(int idEstilo, int idTalla)
        {

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            Conexion conex = new Conexion();
            try
            {
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "DELETE FROM ITEM_SIZE WHERE ID_SUMMARY='" + idEstilo + "' AND TALLA_ITEM='" + idTalla + "' ";
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                conex.CerrarConexion();
            }
            finally { conex.CerrarConexion(); }

        }


    }
}

