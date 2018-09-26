using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatColoresData
    {
        private Conexion conn = new Conexion();
        private SqlCommand comando = new SqlCommand();
        private SqlDataReader leer = null;

        //Muestra la lista de colores
        public IEnumerable<CatColores> ListaColores()
        {
            List<CatColores> listColores= new List<CatColores>();
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "Listar_Color";
            comando.CommandType = CommandType.StoredProcedure;
            leer = comando.ExecuteReader();

            while (leer.Read())
            {
                CatColores colores = new CatColores();
                colores.IdColor= Convert.ToInt32(leer["ID_COLOR"]);
                colores.CodigoColor = leer["CODIGO_COLOR"].ToString();
                colores.DescripcionColor = leer["DESCRIPCION"].ToString();


                listColores.Add(colores);
            }
            leer.Close();
            conn.CerrarConexion();

            return listColores;
        }

        /*public DataTable ListColores()
        {
            DataTable tabla = new DataTable();
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "Listar_Color";
            comando.CommandType = CommandType.StoredProcedure;
            leer = comando.ExecuteReader();
            tabla.Load(leer);
            leer.Close();
            conn.CerrarConexion();
            return tabla;

        }*/

        //Permite crear un nuevo color
        public void AgregarColores(CatColores colores)
        {
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "AgregarColor";
            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue("@ColorStyle", colores.CodigoColor);
            comando.Parameters.AddWithValue("@ColorDesc", colores.DescripcionColor);
         
            comando.ExecuteNonQuery();
            conn.CerrarConexion();

        }

        //Permite consultar los detalles de un color
        public CatColores ConsultarListaColores(int? id)
        {
            CatColores colores = new CatColores();

            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "Listar_Color_Por_Id";
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("@Id", id);

            leer = comando.ExecuteReader();
            while (leer.Read())
            {

                colores.IdColor = Convert.ToInt32(leer["ID_COLOR"]);
                colores.CodigoColor = leer["CODIGO_COLOR"].ToString();
                colores.DescripcionColor = leer["DESCRIPCION"].ToString();

            }
            return colores;

        }

        //Permite actualiza la informacion de un color
        public void ActualizarColores(CatColores colores)
        {
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "Actualizar_Colores";
            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue("@Id", colores.IdColor);
            comando.Parameters.AddWithValue("@ColorStyle", colores.CodigoColor);
            comando.Parameters.AddWithValue("@ColorDesc", colores.DescripcionColor);
        
            comando.ExecuteNonQuery();
            conn.CerrarConexion();
        }

        //Permite eliminar la informacion de un color
        public void EliminarColores(int? id)
        {
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "EliminarColores";
            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue("@Id", id);

            comando.ExecuteNonQuery();
            conn.CerrarConexion();
        }

        public int ObtenerIdColor(string color)
        {
            int idColor = 0;
            Conexion conex = new Conexion();
            SqlCommand coman = new SqlCommand();
            SqlDataReader leerF = null;
            coman.Connection = conex.AbrirConexion();
            coman.CommandText = "SELECT ID_COLOR FROM CAT_COLORES " +
                                 "WHERE CODIGO_COLOR='" + color + "' ";
            leerF = coman.ExecuteReader();
            while (leerF.Read())
            {
                idColor += Convert.ToInt32(leerF["ID_COLOR"]);
            }
            leerF.Close();
            conex.CerrarConexion();
            return idColor;
        }


    }
}


