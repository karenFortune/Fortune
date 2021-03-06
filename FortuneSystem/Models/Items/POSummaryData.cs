﻿using FortuneSystem.Controllers;
using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Item;
using FortuneSystem.Models.Items;
using FortuneSystem.Models.Packing;
using FortuneSystem.Models.Pedidos;
using FortuneSystem.Models.Usuarios;
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
		readonly PackingData objPacking = new PackingData();
		readonly PedidosData objPedido = new PedidosData();
		//readonly ItemTallaData objTallas = new ItemTallaData();
		readonly CatUsuarioData objUsr = new CatUsuarioData();
		readonly CatTypeFormPackData objFormaPacking = new CatTypeFormPackData();


		//Muestra la lista de PO Summary Por PO
		public IEnumerable<POSummary> ListaItemsPorPO(int? id)
          {
                Conexion conn = new Conexion();
			ItemTallaData objTallas = new ItemTallaData();
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
                    CatEspecialidades Especialidad = new CatEspecialidades();
					CatTypeFormPack TipoFormPack = new CatTypeFormPack();

                    Especialidad.Especialidad = leer["SPECIALTIES"].ToString();                    
                    Desc.Descripcion = leer["DESCRIPCION_ITEM"].ToString();
                    colores.CodigoColor = leer["CODIGO_COLOR"].ToString();
                    colores.DescripcionColor = leer["DESCRIPCION"].ToString();
                    ItemSummary.EstiloItem = leer["ITEM_STYLE"].ToString();
                    ItemSummary.Cantidad = Convert.ToInt32(leer["QTY"]);
                    ItemSummary.Price = leer["PRICE"].ToString();
                    ItemSummary.Total = leer["TOTAL"].ToString();
                    ItemSummary.IdItems = Convert.ToInt32(leer["ID_PO_SUMMARY"]);
                    ItemSummary.CatEspecialidades = Especialidad;
                    ItemSummary.CatColores = colores;
                    ItemSummary.ItemDescripcion = Desc;
                    ItemSummary.IdEstilo= Convert.ToInt32(leer["ITEM_ID"]);
					ItemSummary.NumCliente = Convert.ToInt32(leer["CUSTOMER"]);
					

					if (!Convert.IsDBNull(leer["PO_FANTASY"]))
					{
						ItemSummary.POFantasy = leer["PO_FANTASY"].ToString();
					}

					if (!Convert.IsDBNull(leer["ID_FORM_PACK"]))
					{
						ItemSummary.IdTipoFormPack = Convert.ToInt32(leer["ID_FORM_PACK"]);
					}

					if (!Convert.IsDBNull(leer["TYPE_IMP"]))
					{
						ItemSummary.TipoImpresion = leer["TYPE_IMP"].ToString();
					}
					List<string> separadas =  new List<string>();
					/*if(separadas.Count != 0)
					{*/
						separadas = ItemSummary.TipoImpresion.Split(',').ToList();

					for (int i = separadas.Count - 1; i >= 0; i--)
					{
						if (separadas[i].StartsWith(""))
							separadas.Remove(separadas[i]);
					}

					//	int i = 0;
					/*foreach (string item in separadas)
					{
						separadas.Remove("");
					}*/
					ItemSummary.ListaTipoImpresion = separadas;
						//int x = i - 1;

					//}


					ItemSummary.IdTipoFormPack = (ItemSummary.IdTipoFormPack == 0 ? 1 : ItemSummary.IdTipoFormPack);
					ItemSummary.CatTipoFormPack = objFormaPacking.ConsultarListatipoFormPack(ItemSummary.IdTipoFormPack);

					if (!Convert.IsDBNull(leer["ID_USUARIO"]))
                    {
                        ItemSummary.IdUsuario = Convert.ToInt32(leer["ID_USUARIO"]);
                    }             
                    if (ItemSummary.IdUsuario != 0)
                    {
                        ItemSummary.NombreUsuario = objUsr.Obtener_Nombre_Usuario_PorID(ItemSummary.IdUsuario);
                    }
                    else
                    {
                        ItemSummary.NombreUsuario = "-";
                    }

					ItemSummary.HistorialPacking = objPacking.ObtenerNumeroPacking(ItemSummary.IdItems);
					//ItemSummary.Cantidad = objTallas.ObtenerTotalTallas(ItemSummary.IdItems);
					//total = objTallas.ObtenerTotalTallas(ItemSummary.IdItems);
					//ItemSummary.NombreEstilo = leer["DESCRIPTION"].ToString();
					//total = objTallas.ObtenerTotalTallas(ItemSummary.IdItems);
					ItemSummary.CantidadGeneral = objTallas.ObtenerTotalTallasPrimeraCalidad(ItemSummary.IdItems, ItemSummary.Cantidad);
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
			ArteController arteCont = new ArteController();
			ItemTallaData objTallas = new ItemTallaData();
			PedidosData objPedido = new PedidosData();
			ItemDescripcionData objDesc = new ItemDescripcionData();
			MyDbContext db = new MyDbContext();
			Conexion conn = new Conexion();
			List<POSummary> listSummary = new List<POSummary>();
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

					IMAGEN_ARTE_ESTILO arteEstilo = new IMAGEN_ARTE_ESTILO();

					POSummary ItemSummary = new POSummary();
					ItemDescripcion Desc = new ItemDescripcion();
					CatColores colores = new CatColores();
					CatEspecialidades Especialidad = new CatEspecialidades();
					CatTela Tela = new CatTela
					{
						Tela = leer["FABRIC"].ToString()
					};
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
					ItemSummary.CatTela = Tela;
					ItemSummary.PedidosId = Convert.ToInt32(leer["ID_PEDIDOS"]);
					OrdenesCompra listaPO = objPedido.ConsultarListaPO(ItemSummary.PedidosId);
					//List<ItemTalla> listaTallas = objTallas.ListadoTallasPorEstilo(ItemSummary.IdItems).ToList();
					List<ItemTalla> listaTallas = objTallas.ListadoTallasDetallesPorEstilos(ItemSummary.IdItems).ToList();
					
					string descripcion = ItemSummary.EstiloItem.TrimEnd() + "_" + colores.CodigoColor.TrimEnd();
					int idEstilo = objDesc.ObtenerIdEstilo(ItemSummary.EstiloItem);
					var arte = db.ImagenArte.Where(x => x.IdEstilo == idEstilo).FirstOrDefault();
					ObtenerExtensionArte(arteCont, arteEstilo, ItemSummary, descripcion, arte);
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

		public static void ObtenerExtensionArte(ArteController arteCont, IMAGEN_ARTE_ESTILO arteEstilo, POSummary ItemSummary, string descripcion, IMAGEN_ARTE arte)
		{
			if (arte != null && arte.extensionArte != "")
			{
				int tam_var = arte.extensionArte.Length;
				string nombreEstiloArt = arte.extensionArte.Substring(0, tam_var - 4);
				if (descripcion == nombreEstiloArt && arte.extensionArte != null && arte.extensionArte != "")
				{
					ItemSummary.nombreArte = arte.extensionArte;
				}
				else
				{
					arteCont.BuscarRutaImagenEstilo(descripcion, arteEstilo);
					if (arteEstilo != null && arteEstilo.extensionArt != null)
					{
						int tam_var2 = arteEstilo.extensionArt.Length;
						string nomEsdesctiloArt = arteEstilo.extensionArt.Substring(0, tam_var2 - 4);
						if (descripcion == nomEsdesctiloArt && arteEstilo.extensionArt != null)
						{
							ItemSummary.nombreArte = arteEstilo.extensionArt;
						}
						else
						{
							ItemSummary.nombreArte = arte.extensionArte;
						}
					}
					else
					{
						ItemSummary.nombreArte = arte.extensionArte;
					}
				}
			}
			else
			{
				arteCont.BuscarRutaImagenEstilo(descripcion, arteEstilo);
				if (arteEstilo != null && arteEstilo.extensionArt != null)
				{
					int tam_var2 = arteEstilo.extensionArt.Length;
					string nomEsdesctiloArt = arteEstilo.extensionArt.Substring(0, tam_var2 - 4);
					if (descripcion == nomEsdesctiloArt && arteEstilo.extensionArt != null)
					{
						ItemSummary.nombreArte = arteEstilo.extensionArt;
					}
				}
				else
				{
					ItemSummary.nombreArte = arte.extensionArte;
				}
			}
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
					CatEspecialidades especial = new CatEspecialidades
					{
						Especialidad = leer["SPECIALTIES"].ToString(),
						IdEspecialidad = Convert.ToInt32(leer["ID_SPECIALTIES"])
					};
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
                    ItemSummary.CatEspecialidades = especial;
                    ItemSummary.CatGenero = genero;
					ItemSummary.IdEstado = Convert.ToInt32(leer["ID_ESTADO"]);
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
					estilos.CantidadT = leerF["QTY"].ToString();
					estilos.PedidosId = Convert.ToInt32(leerF["ID_PEDIDOS"]);
                    estilos.Precio = Convert.ToDouble(leerF["PRICE"]);
                    estilos.IdEspecialidad = Convert.ToInt32(leerF["ID_SPECIALTIES"]);
					estilos.TipoImpresion = leerF["TYPE_IMP"].ToString();

					if (!Convert.IsDBNull(leerF["ID_FORM_PACK"]))
					{
						estilos.IdTipoFormPack = Convert.ToInt32(leerF["ID_FORM_PACK"]);
					}

					if (!Convert.IsDBNull(leerF["PO_FANTASY"]))
					{
						estilos.POFantasy =leerF["PO_FANTASY"].ToString();
					}



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
				SqlCommand comando = new SqlCommand
				{
					Connection = conex.AbrirConexion(),
					CommandText = "AgregarItem",
					CommandType = CommandType.StoredProcedure
				};

				comando.Parameters.AddWithValue("@Item", items.EstiloItem);
                comando.Parameters.AddWithValue("@Color", items.IdColor);
                comando.Parameters.AddWithValue("@Qty", items.Cantidad);
                comando.Parameters.AddWithValue("@Price", items.Precio);
                comando.Parameters.AddWithValue("@IdPedidos", items.PedidosId);
                comando.Parameters.AddWithValue("@IdGenero", items.IdGenero);
                comando.Parameters.AddWithValue("@IdTela", items.IdTela);
                comando.Parameters.AddWithValue("@TipoCamiseta", items.TipoCamiseta);
                comando.Parameters.AddWithValue("@IdEspecialidad", items.IdEspecialidad);
                comando.Parameters.AddWithValue("@IdUsuario", items.IdUsuario);
                comando.Parameters.AddWithValue("@FechaUCC", DBNull.Value);
                comando.Parameters.AddWithValue("@IdEstado", items.IdEstado);
				comando.Parameters.AddWithValue("@IdSucursal", items.IdSucursal);
				comando.Parameters.AddWithValue("@IdFormPack", items.IdTipoFormPack);
				comando.Parameters.AddWithValue("@TipoImpresion", items.TipoImpresion);
				comando.Parameters.AddWithValue("@POFant", items.POFantasy);

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

        public int Obtener_Utlimo_Id_Arte_Pnl()
        {
            Conexion conex = new Conexion();
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            try
            {
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "SELECT IdImgArtePNL FROM IMAGEN_ARTE_PNL WHERE IdImgArtePNL = (SELECT MAX(IdImgArtePNL) FROM IMAGEN_ARTE_PNL) ";
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    return Convert.ToInt32(reader["IdImgArtePNL"]);
                }
                conex.CerrarConexion();
            }
            finally
            {
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
				if (!reader.Read())
				{
					//Response.Write("Wrong Details");
				}
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
				SqlCommand comando = new SqlCommand
				{
					Connection = conex.AbrirConexion(),
					CommandText = "Actualizar_Estilos",
					CommandType = CommandType.StoredProcedure
				};

				comando.Parameters.AddWithValue("@Id", items.IdItems);
                comando.Parameters.AddWithValue("@Item", items.IdEstilo);
                comando.Parameters.AddWithValue("@Color", items.ColorId);
                comando.Parameters.AddWithValue("@Qty", items.Cantidad);
                comando.Parameters.AddWithValue("@Price", items.Precio);
                comando.Parameters.AddWithValue("@IdPedidos", items.PedidosId);
                comando.Parameters.AddWithValue("@IdGenero", items.Id_Genero);
                comando.Parameters.AddWithValue("@IdTela", items.IdTela);
                comando.Parameters.AddWithValue("@TipoCamiseta", items.IdCamiseta);
                comando.Parameters.AddWithValue("@IdEspecialidad", items.IdEspecialidad);
				comando.Parameters.AddWithValue("@IdTipoForm", items.IdTipoFormPack);
				comando.Parameters.AddWithValue("@TipoImpresion", items.TipoImpresion);
				comando.Parameters.AddWithValue("@POFant", items.POFantasy);

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
				SqlCommand comando = new SqlCommand
				{
					Connection = conex.AbrirConexion(),
					CommandText = "EliminarEstilo",
					CommandType = CommandType.StoredProcedure
				};
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
				SqlCommand comando = new SqlCommand
				{
					Connection = conex.AbrirConexion(),
					CommandText = "EliminarTallasEstilo",
					CommandType = CommandType.StoredProcedure
				};
				comando.Parameters.AddWithValue("@Id", id);
                comando.ExecuteNonQuery();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
        }

        //Muestra la lista estilos y tallas por ID pedido y ID Estilo
       /* public IEnumerable<POSummarys> ListaSummaryBlanksId(int? id, int? idEstilo)
        {
            Conexion conn = new Conexion();
            List<POSummarys> listSummary = new List<POSummarys>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT PO.ID_PO_SUMMARY, S.TALLA_ITEM, S.CANTIDAD,S.EJEMPLOS FROM PO_SUMMARY PO " +
                    "INNER JOIN ITEM_SIZE S ON S.ID_SUMMARY=PO.ID_PO_SUMMARY " +
                    "WHERE PO.ID_PEDIDOS='" + id + "' AND PO.ITEM_ID='" + idEstilo + "' ";
                comando.CommandType = CommandType.Text; 
                leer = comando.ExecuteReader();

                while (leer.Read())
                {
                    POSummarys ItemSummary = new POSummarys();
                    ItemTalla talla = new ItemTalla();
                    talla.IdTalla = Convert.ToInt32(leer["TALLA_ITEM"]);  
                    talla.Ejemplos = Convert.ToInt32(leer["EJEMPLOS"]);
                    ItemSummary.IdItems = Convert.ToInt32(leer["ID_PO_SUMMARY"]);
                    talla.Cantidad = Convert.ToInt32(leer["CANTIDAD"]) + Convert.ToInt32(leer["EJEMPLOS"]);
                    ItemSummary.ItemTalla = talla;
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
        }*/

        //Obtener el tipo de especialidad de estilo por Id Summary
        public int ObtenerEspecialidadPorIdSummary(int? idSummary)        {
        
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT ID_SPECIALTIES FROM PO_SUMMARY " +
                                     "WHERE ID_PO_SUMMARY='" + idSummary + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                 
                    if (!Convert.IsDBNull(leerF["ID_SPECIALTIES"]))
                    {
                        return Convert.ToInt32(leerF["ID_SPECIALTIES"]);
                    }
                   
                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

            return 0;
        }

        //Permite registrar la fechaUPCC al estilo
        public void AgregarFechaUCC(POSummary poSummary)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            Conexion conex = new Conexion();
            try
            {
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "UPDATE PO_SUMMARY SET FECHA_UCC ='" + poSummary.FechaUCC + "' WHERE ID_PO_SUMMARY='" + poSummary.IdItems + "'";
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                conex.CerrarConexion();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

        }

		//Muestra la lista de tipo packing por Estilo
		public IEnumerable<CatTypePackItem> ListaPackPorEstilo(int? id)
		{
			Conexion conn = new Conexion();
			List<CatTypePackItem> listPack = new List<CatTypePackItem>();
			try
			{
				SqlCommand comando = new SqlCommand();
				SqlDataReader leer = null;
				comando.Connection = conn.AbrirConexion();
				comando.CommandText = "SELECT ID_PACK_STYLE, DESC_PACK FROM CAT_TYPE_PACK_STYLE WHERE ID_SUMMARY='" + id + "' ";
				comando.CommandType = CommandType.Text;
				leer = comando.ExecuteReader();

				while (leer.Read())
				{
					CatTypePackItem typePack = new CatTypePackItem()
					{
						IdPackStyle = Convert.ToInt32(leer["ID_PACK_STYLE"]),
						DescripcionPack = leer["DESC_PACK"].ToString()

				    };

				listPack.Add(typePack);

			}
				leer.Close();
		}
			finally
			{
				conn.CerrarConexion();
				conn.Dispose();
			}

			return listPack;
		}



	}
}