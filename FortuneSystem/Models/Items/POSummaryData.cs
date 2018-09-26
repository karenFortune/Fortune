using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Items;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.POSummary
{
    public class DescripcionItemData
    {

       
        //Muestra la lista de PO Summary Por PO
          public IEnumerable<POSummary> ListaItemsPorPO(int? id)
          {
                Conexion conn = new Conexion();
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;

              List<POSummary> listSummary = new List<POSummary>();
              comando.Connection = conn.AbrirConexion();
              comando.CommandText = "Listar_Item_Por_Pedido";
              comando.CommandType = CommandType.StoredProcedure;
              comando.Parameters.AddWithValue("@Id", id);
              leer = comando.ExecuteReader();

              while (leer.Read())
              {
                 POSummary ItemSummary = new POSummary();
                 ItemDescripcion Desc = new ItemDescripcion();
                 CatColores colores = new CatColores();
                 Desc.Descripcion= leer["DESCRIPCION_ITEM"].ToString();
                 colores.CodigoColor = leer["CODIGO_COLOR"].ToString();
                 colores.DescripcionColor = leer["DESCRIPCION"].ToString();
                 ItemSummary.EstiloItem = leer["ITEM_STYLE"].ToString();                
                 ItemSummary.Cantidad = Convert.ToInt32(leer["QTY"]);
                 ItemSummary.Price = leer["PRICE"].ToString();
                 ItemSummary.Total = leer["TOTAL"].ToString();
                 ItemSummary.IdItems= Convert.ToInt32(leer["ID_PO_SUMMARY"]);
                 ItemSummary.CatColores = colores;
                 ItemSummary.ItemDescripcion = Desc;
                 listSummary.Add(ItemSummary);

              }
              leer.Close();
              conn.CerrarConexion();

              return listSummary;
          }


        //Muestra la lista estilos de Por PO
        public IEnumerable<POSummary> ListaEstilosPorPO(int? id)
        {
            Conexion conn = new Conexion();
            SqlCommand comando = new SqlCommand();
            SqlDataReader leer = null;
            List<POSummary> listSummary = new List<POSummary>();
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "Listar_Estilos_Por_Pedido";
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("@Id", id);
            leer = comando.ExecuteReader();

            while (leer.Read())
            {
                POSummary ItemSummary = new POSummary();
                CatColores colores = new CatColores();
                CatTipoCamiseta tipoCamiseta = new CatTipoCamiseta();
                CatGenero genero = new CatGenero();
                colores.CodigoColor = leer["CODIGO_COLOR"].ToString();
                ItemSummary.EstiloItem = leer["ITEM_STYLE"].ToString();
                ItemSummary.Cantidad = Convert.ToInt32(leer["QTY"]);
                genero.GeneroCode = leer["GENERO_CODE"].ToString();
                ItemSummary.Precio = Convert.ToDouble(leer["PRICE"]);
                ItemSummary.IdItems = Convert.ToInt32(leer["ID_PO_SUMMARY"]);
                ItemSummary.PedidosId = Convert.ToInt32(leer["ID_PEDIDOS"]);
                tipoCamiseta.TipoProducto= leer["PRODUCT_TYPE_CODE"].ToString();
                ItemSummary.IdTela = Convert.ToInt32(leer["ID_TELA"]);
                ItemSummary.CatColores = colores;
                ItemSummary.CatTipoCamiseta = tipoCamiseta;
                ItemSummary.CatGenero = genero;
                listSummary.Add(ItemSummary);
               
            }
            leer.Close();
            conn.CerrarConexion();

            return listSummary;
        }

        //Permite consultar la informacion de un estilo por ID
        public POSummary ConsultarListaEstilos(int? id)
        {
            Conexion conexion = new Conexion();
            SqlCommand com = new SqlCommand();
            SqlDataReader leerF = null;
            POSummary estilos = new POSummary();

            com.Connection = conexion.AbrirConexion();
            com.CommandText = "Lista_Estilos_Por_Id";
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Id", id);

            leerF = com.ExecuteReader();
            while (leerF.Read())
            {

                estilos.IdItems = Convert.ToInt32(leerF["ITEM_ID"]);
                estilos.ColorId= Convert.ToInt32(leerF["ID_COLOR"]);
                estilos.Id_Genero = Convert.ToInt32(leerF["ID_GENDER"]);
                estilos.IdTela = Convert.ToInt32(leerF["ID_TELA"]);
                estilos.IdCamiseta = Convert.ToInt32(leerF["ID_PRODUCT_TYPE"]);
                estilos.Cantidad = Convert.ToInt32(leerF["QTY"]);
                estilos.PedidosId = Convert.ToInt32(leerF["ID_PEDIDOS"]);
                estilos.Precio = Convert.ToDouble(leerF["PRICE"]);
               

            }
            leerF.Close();
            conexion.CerrarConexion();
            return estilos;

        }

        public void AgregarItems(POSummary items)
        {
            Conexion conex = new Conexion();
            SqlCommand comando = new SqlCommand();
            comando.Connection = conex.AbrirConexion();
            comando.CommandText = "AgregarItem";
            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue("@Item", items.EstiloItem);
            comando.Parameters.AddWithValue("@Color", items.IdColor);
            comando.Parameters.AddWithValue("@Qty", items.Cantidad);
            comando.Parameters.AddWithValue("@Price", items.Precio);
            comando.Parameters.AddWithValue("@IdPedidos", items.PedidosId);
            comando.Parameters.AddWithValue("@IdGenero", items.IdGenero);
            comando.Parameters.AddWithValue("@IdTela", items.IdTela);
            comando.Parameters.AddWithValue("@TipoCamiseta", items.TipoCamiseta);

            comando.ExecuteNonQuery();
            conex.CerrarConexion();

        }

        public int Obtener_Utlimo_Item()
        {
            Conexion conex = new Conexion();
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            try
            {
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "SELECT ID_PO_SUMMARY FROM PO_SUMMARY WHERE ID_PO_SUMMARY = (SELECT MAX(ID_PO_SUMMARY) FROM PO_SUMMARY) ";
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    return Convert.ToInt32(reader["ID_PO_SUMMARY"]);
                }
                conex.CerrarConexion();
            }
            finally { conex.CerrarConexion(); }
            return 0;
        }

        //Permite actualiza la informacion de un estilo
        public void ActualizarEstilos(POSummary items)
        {
             Conexion conex = new Conexion();
            SqlCommand comando = new SqlCommand();
            comando.Connection = conex.AbrirConexion();
            comando.CommandText = "Actualizar_Estilos";
            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue("@Id", items.IdItems);
            comando.Parameters.AddWithValue("@Item", items.IdEstilo);
            comando.Parameters.AddWithValue("@Color", items.ColorId);
            comando.Parameters.AddWithValue("@Qty", items.Cantidad);
            comando.Parameters.AddWithValue("@Price", items.Precio);
            comando.Parameters.AddWithValue("@IdPedidos", items.PedidosId);
            comando.Parameters.AddWithValue("@IdGenero", items.Id_Genero);
            comando.Parameters.AddWithValue("@IdTela", items.IdTela);
            comando.Parameters.AddWithValue("@TipoCamiseta", items.IdCamiseta);

            comando.ExecuteNonQuery();
            conex.CerrarConexion();
        }







    }
}