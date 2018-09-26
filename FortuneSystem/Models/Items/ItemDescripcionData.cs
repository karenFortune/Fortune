using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Items
{
    public class ItemDescripcionData
    {

        private Conexion conn = new Conexion();
        private SqlCommand comando = new SqlCommand();
        private SqlDataReader leerFilas = null;


        //Muestra la lista de Items
        public IEnumerable<ItemDescripcion> ListaItems()
        {
            List<ItemDescripcion> listItems = new List<ItemDescripcion>();
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "Listar_Item_Desc";
            comando.CommandType = CommandType.StoredProcedure;
            leerFilas = comando.ExecuteReader();

            while (leerFilas.Read())
            {
                ItemDescripcion items = new ItemDescripcion();
                items.ItemId = Convert.ToInt32(leerFilas["ITEM_ID"]);
                items.ItemEstilo = leerFilas["ITEM_STYLE"].ToString();
                items.Descripcion = leerFilas["DESCRIPTION"].ToString();
                listItems.Add(items);

            }
            leerFilas.Close();
            conn.CerrarConexion();

            return listItems;
        }


        //Permite crear un nuevo Item descripcion
        public void AgregarItemDescripcion(ItemDescripcion itemDesc)
        {
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "Agregar_Item_Desc";
            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue("@Style", itemDesc.ItemEstilo);
            comando.Parameters.AddWithValue("@Descripcion", itemDesc.Descripcion);

            comando.ExecuteNonQuery();
            conn.CerrarConexion();

        }

        //Permite consultar los detalles de un Item Desc
        public ItemDescripcion ConsultarListaItemDesc(int? id)
        {
            Conexion conex = new Conexion();
            SqlCommand coma = new SqlCommand();
            SqlDataReader leer = null;
        ItemDescripcion itemDesc = new ItemDescripcion();

            coma.Connection = conex.AbrirConexion();
            coma.CommandText = "Listar_EstiloDesc_Por_Id";
            coma.CommandType = CommandType.StoredProcedure;
            coma.Parameters.AddWithValue("@Id", id);

            leer = coma.ExecuteReader();
            while (leer.Read())
            {
                string descripcion = leer["DESCRIPTION"].ToString();
                itemDesc.ItemId = Convert.ToInt32(leer["ITEM_ID"]);
                itemDesc.ItemEstilo = leer["ITEM_STYLE"].ToString();
                itemDesc.Descripcion = descripcion.TrimEnd(' '); 

            }
            return itemDesc;

        }

        public int ObtenerIdEstilo(string estilo)
        {
            int idEstilo = 0;
            Conexion conex = new Conexion();
            SqlCommand coman = new SqlCommand();
            SqlDataReader leerF = null;
            coman.Connection = conex.AbrirConexion();
            coman.CommandText = "SELECT ITEM_ID FROM ITEM_DESCRIPTION " +
                                 "WHERE ITEM_STYLE='" + estilo + "' ";
            leerF = coman.ExecuteReader();
            while (leerF.Read())
            {
                idEstilo += Convert.ToInt32(leerF["ITEM_ID"]);
            }
            leerF.Close();
            conex.CerrarConexion();
            return idEstilo;
        }


        public string Verificar_Item_CD(string cadena)
        {
            string texto = null;
            cadena = cadena.TrimEnd();
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "VerificarItem";
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("@Cadena", cadena);
            texto = (string)comando.ExecuteScalar();
            comando.Parameters.Clear();
            conn.CerrarConexion();
            if (texto != null) texto = texto.TrimEnd();
            return texto;
        }
    }
}