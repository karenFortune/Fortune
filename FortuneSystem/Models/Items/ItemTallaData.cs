using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Item
{
    public class ItemTallaData
    {
            
       public void RegistroTallas(ItemTalla tallas)
        {
            Conexion conn = new Conexion();
            SqlCommand comando = new SqlCommand();

        comando.Connection = conn.AbrirConexion();
            try
            {
                comando.CommandText = "INSERT INTO  ITEM_SIZE (TALLA_ITEM,CANTIDAD,EXTRAS,EJEMPLOS,ID_SUMMARY) " +
                    " VALUES((SELECT ID FROM CAT_ITEM_SIZE WHERE TALLA ='"+ tallas.Talla + "'),'" + tallas.Cantidad + "','"+ tallas.Extras + "','" + tallas.Ejemplos + "','" + tallas.IdSummary + "')";
                comando.ExecuteNonQuery();
            }
            finally { conn.CerrarConexion(); }
        }

        public void RegistroTallasUPC(UPC tallas)
        {
            Conexion conn = new Conexion();
            SqlCommand comando = new SqlCommand();

            comando.Connection = conn.AbrirConexion();
            try
            {
                comando.CommandText = "INSERT INTO  UPC (IdTalla,IdSummary,UPC) " +
                    " VALUES((SELECT ID FROM CAT_ITEM_SIZE WHERE TALLA ='" + tallas.Talla + "'),'" + tallas.IdSummary + "','" + tallas.UPC1 + "')";
                comando.ExecuteNonQuery();
            }
            finally { conn.CerrarConexion(); }
        }



        //Muestra la lista de tallas por estilo
        public IEnumerable<ItemTalla> ListaTallasPorEstilo(int? id)
        {
            Conexion conn = new Conexion();
            SqlCommand comando = new SqlCommand();
            SqlDataReader leer = null;
            List<ItemTalla> listTallas = new List<ItemTalla>();
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "Lista_Tallas_Por_Estilo";
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("@Id", id);
            leer = comando.ExecuteReader();

            while (leer.Read())
            {
                ItemTalla tallas = new ItemTalla()
                {
                    Talla = leer["TALLA"].ToString(),
                    Cantidad = Convert.ToInt32(leer["CANTIDAD"]),
                    Extras = Convert.ToInt32(leer["EXTRAS"]),
                    Ejemplos= Convert.ToInt32(leer["EJEMPLOS"]),
                    Estilo = leer["ITEM_STYLE"].ToString()

                };

                listTallas.Add(tallas);
            }
            leer.Close();
            conn.CerrarConexion();

            return listTallas;
        }

        //Muestra la lista de tallas de Staging por estilo
        public IEnumerable<ItemTalla> ListaTallasStagingPorEstilo(int? id)
        {
            Conexion conn = new Conexion();
            SqlCommand comando = new SqlCommand();
            SqlDataReader leer = null;
            List<ItemTalla> listTallas = new List<ItemTalla>();
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "Lista_Tallas_Staging_Por_Estilo";
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("@Id", id);
            leer = comando.ExecuteReader();

            while (leer.Read())
            {
                ItemTalla tallas = new ItemTalla()
                {
                    Talla = leer["TALLA"].ToString(),
                    Cantidad = Convert.ToInt32(leer["CANTIDAD"]),
                    Estilo = leer["ITEM_STYLE"].ToString()

                };

                listTallas.Add(tallas);
            }
            leer.Close();
            conn.CerrarConexion();

            return listTallas;
        }
        //Muestra la lista de tallas por summary
        public IEnumerable<ItemTalla> ListaTallasPorSummary(int? id)
        {
                Conexion conexion = new Conexion();
             SqlCommand com = new SqlCommand();
            SqlDataReader leerF = null;
        List<ItemTalla> listTallas = new List<ItemTalla>();
            com.Connection = conexion.AbrirConexion();
            com.CommandText = "Lista_Tallas_Por_Summary";
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Id", id);
            leerF = com.ExecuteReader();

            while (leerF.Read())
            {
                ItemTalla tallas = new ItemTalla()
                {
                    Talla = leerF["TALLA"].ToString(),
                    Cantidad = Convert.ToInt32(leerF["CANTIDAD"]),
                    Extras = Convert.ToInt32(leerF["EXTRAS"]),
                    Ejemplos = Convert.ToInt32(leerF["EJEMPLOS"]),
                    IdSummary = Convert.ToInt32(leerF["ID_SUMMARY"])

                };

                listTallas.Add(tallas);
            }
            leerF.Close();
            conexion.CerrarConexion();

            return listTallas;
        }

        //Permite actualiza la informacion de un usuario
        public void Actualizar_Tallas_Estilo(ItemTalla tallas)
        {
            Conexion conexion = new Conexion();
            SqlCommand com = new SqlCommand();
            com.Connection = conexion.AbrirConexion();
            com.CommandText = "Actualizar_Tallas_Estilo";
            com.CommandType = CommandType.StoredProcedure;

            com.Parameters.AddWithValue("@Id", tallas.Id);
            com.Parameters.AddWithValue("@IdTalla", tallas.IdTalla);
            com.Parameters.AddWithValue("@Cantidad", tallas.Cantidad);
            com.Parameters.AddWithValue("@Extras", tallas.Extras);
            com.Parameters.AddWithValue("@Ejemplos", tallas.Ejemplos);
            com.Parameters.AddWithValue("@IdSummary", tallas.IdSummary);

            com.ExecuteNonQuery();
            conexion.CerrarConexion();
        }

        public int ObtenerIdTalla(string talla, int idEstilo)
        {
            int idTalla = 0;
            Conexion conex = new Conexion();
            SqlCommand coman = new SqlCommand();
            SqlDataReader leerF = null;
            coman.Connection = conex.AbrirConexion();
            coman.CommandText = "SELECT I.TALLA_ITEM FROM ITEM_SIZE I " +
                    "INNER JOIN CAT_ITEM_SIZE TALLA ON I.TALLA_ITEM=TALLA.ID " +
                    "INNER JOIN PO_SUMMARY ESTILOS ON I.ID_SUMMARY=ESTILOS.ID_PO_SUMMARY "+
                    "where I.ID_SUMMARY='" + idEstilo + "'and TALLA.TALLA='" + talla + "'";
            leerF = coman.ExecuteReader();
            while (leerF.Read())
            {
                idTalla += Convert.ToInt32(leerF["TALLA_ITEM"]);
            }
            leerF.Close();
            conex.CerrarConexion();
            return idTalla;
        }

        public int ObtenerIdTallaEstilo(string talla, int idEstilo)
        {
            int idTalla = 0;
            Conexion conex = new Conexion();
            SqlCommand coman = new SqlCommand();
            SqlDataReader leerF = null;
            coman.Connection = conex.AbrirConexion();
            coman.CommandText = "SELECT I.ID_TALLA FROM ITEM_SIZE I " +
                    "INNER JOIN CAT_ITEM_SIZE TALLA ON I.TALLA_ITEM=TALLA.ID " +
                    "INNER JOIN PO_SUMMARY ESTILOS ON I.ID_SUMMARY=ESTILOS.ID_PO_SUMMARY " +
                    "where I.ID_SUMMARY='" + idEstilo + "'and TALLA.TALLA='" + talla + "'";
            leerF = coman.ExecuteReader();
            while (leerF.Read())
            {
                idTalla += Convert.ToInt32(leerF["ID_TALLA"]);
            }
            leerF.Close();
            conex.CerrarConexion();
            return idTalla;
        }



    }
}