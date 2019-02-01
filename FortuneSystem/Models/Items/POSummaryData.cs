﻿using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Item;
using FortuneSystem.Models.Items;
using FortuneSystem.Models.Packing;
using FortuneSystem.Models.Pedidos;
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
        PackingData objPacking = new PackingData();
        PedidosData objPedido = new PedidosData();
        ItemTallaData objTallas = new ItemTallaData();
        //Muestra la lista de PO Summary Por PO
        public IEnumerable<POSummary> ListaItemsPorPO(int? id)
          {
                Conexion conn = new Conexion();
            List<POSummary> listSummary = new List<POSummary>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;              
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
                    Desc.Descripcion = leer["DESCRIPCION_ITEM"].ToString();
                    colores.CodigoColor = leer["CODIGO_COLOR"].ToString();
                    colores.DescripcionColor = leer["DESCRIPCION"].ToString();
                    ItemSummary.EstiloItem = leer["ITEM_STYLE"].ToString();
                    ItemSummary.Cantidad = Convert.ToInt32(leer["QTY"]);
                    ItemSummary.Price = leer["PRICE"].ToString();
                    ItemSummary.Total = leer["TOTAL"].ToString();
                    ItemSummary.IdItems = Convert.ToInt32(leer["ID_PO_SUMMARY"]);
                    ItemSummary.CatColores = colores;
                    ItemSummary.ItemDescripcion = Desc;
                    listSummary.Add(ItemSummary);

                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }          
              return listSummary;
          }

        public IEnumerable<POSummary> ListadoInfEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<POSummary> listSummary = new List<POSummary>();
            OrdenesCompra listaPO = new OrdenesCompra();
            List<ItemTalla> listaTallas = new List<ItemTalla>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Listar_Item_Por_Pedido";//Info_Estilo
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();

                while (leer.Read())
                {
                    POSummary ItemSummary = new POSummary();
                    ItemDescripcion Desc = new ItemDescripcion();
                    CatColores colores = new CatColores();
                    CatEspecialidades Especialidad = new CatEspecialidades();
                    Desc.Descripcion = leer["DESCRIPCION_ITEM"].ToString();
                    colores.CodigoColor = leer["CODIGO_COLOR"].ToString();
                    colores.DescripcionColor = leer["DESCRIPCION"].ToString();
                    Especialidad.Especialidad = leer["SPECIALTIES"].ToString();
                    ItemSummary.EstiloItem = leer["ITEM_STYLE"].ToString();
                    ItemSummary.Cantidad = Convert.ToInt32(leer["QTY"]);
                    ItemSummary.Price = leer["PRICE"].ToString();
                    ItemSummary.Total = leer["TOTAL"].ToString();
                    ItemSummary.IdItems = Convert.ToInt32(leer["ID_PO_SUMMARY"]);
                    ItemSummary.CatColores = colores;
                    ItemSummary.ItemDescripcion = Desc;
                    ItemSummary.CatEspecialidades = Especialidad;
                    ItemSummary.PedidosId = Convert.ToInt32(leer["ID_PEDIDOS"]);
                    listaPO = objPedido.ConsultarListaPO(ItemSummary.PedidosId);
                    listaTallas = objTallas.ListadoTallasPorEstilo(ItemSummary.IdItems).ToList();
                    ItemSummary.Pedidos = listaPO;
                    ItemSummary.ListarTallasPorEstilo = listaTallas;         
                    listSummary.Add(ItemSummary);

                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
            return listSummary;
        }

        //Muestra la lista estilos de Por PO
        public IEnumerable<POSummary> ListaEstilosPorPO(int? id)
        {
            Conexion conn = new Conexion();
            List<POSummary> listSummary = new List<POSummary>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;               
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
                    tipoCamiseta.TipoProducto = leer["PRODUCT_TYPE_CODE"].ToString();
                    ItemSummary.IdTela = Convert.ToInt32(leer["ID_TELA"]);
                    ItemSummary.CatColores = colores;
                    ItemSummary.CatTipoCamiseta = tipoCamiseta;
                    ItemSummary.CatGenero = genero;
                    listSummary.Add(ItemSummary);

                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }         

            return listSummary;
        }

        //Permite consultar la informacion de un estilo por ID
        public POSummary ConsultarListaEstilos(int? id)
        {
            Conexion conexion = new Conexion();
            POSummary estilos = new POSummary();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;             

                com.Connection = conexion.AbrirConexion();
                com.CommandText = "Lista_Estilos_Por_Id";
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", id);

                leerF = com.ExecuteReader();
                while (leerF.Read())
                {

                    estilos.IdItems = Convert.ToInt32(leerF["ITEM_ID"]);
                    estilos.ColorId = Convert.ToInt32(leerF["ID_COLOR"]);
                    estilos.Id_Genero = Convert.ToInt32(leerF["ID_GENDER"]);
                    estilos.IdTela = Convert.ToInt32(leerF["ID_TELA"]);
                    estilos.IdCamiseta = Convert.ToInt32(leerF["ID_PRODUCT_TYPE"]);
                    estilos.Cantidad = Convert.ToInt32(leerF["QTY"]);
                    estilos.PedidosId = Convert.ToInt32(leerF["ID_PEDIDOS"]);
                    estilos.Precio = Convert.ToDouble(leerF["PRICE"]);


                }
                leerF.Close();
            }
            finally
            {
                conexion.CerrarConexion();
                conexion.Dispose();
            } 
            
            return estilos;

        }

        public void AgregarItems(POSummary items)
        {
            Conexion conex = new Conexion();
            try
            {
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
                comando.Parameters.AddWithValue("@IdEspecialidad", items.IdEspecialidad);

                comando.ExecuteNonQuery();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }          

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
            finally {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return 0;
        }

        //Permite obtener el total de piezas de los estilos por Id
        public int ObtenerPiezasTotalesEstilos(int? id)
        {
            Conexion conex = new Conexion();
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            int suma = 0;
            try
            {
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "SELECT QTY FROM PO_SUMMARY WHERE ID_PEDIDOS='"+id+"' ";
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    suma += Convert.ToInt32(reader["QTY"]);
                    
                }                
                conex.CerrarConexion();
               
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return suma;
        }


        //Permite obtener el total de piezas por tipo de empaque 
        public int ObtenerPiezasTotalesPorPack(int? id)
        {
            Conexion conex = new Conexion();
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            int suma = 0;
            int pedidoId = Convert.ToInt32(id);
            string query = objPacking.ObtenerEstilosPorIdPedido(pedidoId);
            try
            {
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "SELECT DISTINCT P.ID_TALLA, P.ID_SUMMARY, P.CANT_BOX, P.TOTAL_PIECES, PT.TYPE_PACKING  FROM packing_type_size PT, PACKING P " +
                                  "WHERE PT.ID_SUMMARY in(" + query + ") AND PT.TYPE_PACKING IN(1,2) AND P.ID_SUMMARY=PT.ID_SUMMARY";
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    suma += Convert.ToInt32(reader["TOTAL_PIECES"]);

                }
                conex.CerrarConexion();

            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return suma;
        }

        //Permite obtener el total de piezas por tipo de empaque para Assort
        public int ObtenerPiezasTotalesPorPackAssort(int? id)
        {
            Conexion conex = new Conexion();
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            int suma = 0;
            try
            {
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "SELECT DISTINCT P.ID_TALLA, P.ID_SUMMARY, P.CANT_BOX, P.TOTAL_PIECES, PT.TYPE_PACKING  FROM packing_type_size PT, PACKING P " +
                                  "WHERE PT.ID_SUMMARY='"+id+"' AND PT.TYPE_PACKING IN(3) AND P.ID_SUMMARY=PT.ID_SUMMARY";
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    suma += Convert.ToInt32(reader["TOTAL_PIECES"]);

                }
                conex.CerrarConexion();

            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return suma;
        }
        //Permite actualiza la informacion de un estilo
        public void ActualizarEstilos(POSummary items)
        {
             Conexion conex = new Conexion();
            try
            {
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
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
                      
        }

        //Permite eliminar la informacion de un estilo 
        public void EliminarEstilos(int? id)
        {
            Conexion conex = new Conexion();           
            try
            {
                SqlCommand comando = new SqlCommand();
                comando.Connection = conex.AbrirConexion();
                comando.CommandText = "EliminarEstilo";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                comando.ExecuteNonQuery();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
        }

        //Permite eliminar la informacion de un estilo 
        public void EliminarTallasEstilo(int? id)
        {
            Conexion conex = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                comando.Connection = conex.AbrirConexion();
                comando.CommandText = "EliminarTallasEstilo";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                comando.ExecuteNonQuery();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
        }
    }
}