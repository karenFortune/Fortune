using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Items;
using FortuneSystem.Models.Revisiones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Pedidos
{
    public class PedidosData
    {
       
        RevisionesData objRevision = new RevisionesData();

        //Muestra la lista de PO
        public IEnumerable<OrdenesCompra> ListaOrdenCompra()
        {
            Conexion conn = new Conexion();
            List<OrdenesCompra> listPedidos = new List<OrdenesCompra>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                // comando.CommandText = "Listar_Pedidos";
                comando.CommandText = "SELECT P.ID_PEDIDO, P.PO, P.VPO, P.CUSTOMER, P.CUSTOMER_FINAL, C.NAME_FINAL, " +
                    "P.DATE_CANCEL, P.DATE_ORDER, P.TOTAL_UNITS, P.ID_STATUS, S.ESTADO FROM PEDIDO P, CAT_STATUS S, " +
                    "CAT_CUSTOMER_PO C WHERE P.ID_STATUS = S.ID_STATUS AND P.CUSTOMER_FINAL = C.CUSTOMER_FINAL AND " +
                    "P.ID_STATUS = 1 ORDER BY P.DATE_ORDER DESC ";
                comando.CommandType = CommandType.Text;
                leer = comando.ExecuteReader();

                while (leer.Read())
                {

                    OrdenesCompra pedidos = new OrdenesCompra()
                    {
                        IdPedido = Convert.ToInt32(leer["ID_PEDIDO"]),
                        PO = leer["PO"].ToString(),
                        VPO = Convert.ToInt32(leer["VPO"]),
                        Cliente = Convert.ToInt32(leer["CUSTOMER"]),
                        ClienteFinal = Convert.ToInt32(leer["CUSTOMER_FINAL"]),
                        FechaCancel = Convert.ToDateTime(leer["DATE_CANCEL"]),
                        FechaOrden = Convert.ToDateTime(leer["DATE_ORDER"]),
                        TotalUnidades = Convert.ToInt32(leer["TOTAL_UNITS"]),
                        IdStatus = Convert.ToInt32(leer["ID_STATUS"]),

                    };
                    CatStatus status = new CatStatus()
                    {
                        Estado = leer["ESTADO"].ToString()
                    };
                    CatClienteFinal clienteFinal = new CatClienteFinal()
                    {
                        NombreCliente = leer["NAME_FINAL"].ToString()
                    };

                    pedidos.Historial = objRevision.ObtenerPedidoRevisiones(pedidos.IdPedido);

                    pedidos.CatStatus = status;
                    pedidos.CatClienteFinal = clienteFinal;


                    listPedidos.Add(pedidos);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

           

            return listPedidos;
        }

        //Muestra la lista de estilos por IdPedido
        public IEnumerable<ItemDescripcion> ListaEstilosPorIdPedido(int? id)
        {
            int pedido = Convert.ToInt32(id);
            Conexion conn = new Conexion();
            List<ItemDescripcion> listEstilos = new List<ItemDescripcion>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                // comando.CommandText = "Listar_Pedidos";
                comando.CommandText = "select  ITD.ITEM_ID, ITD.ITEM_STYLE, PS.ID_PO_SUMMARY from po_summary PS " +
                    "INNER JOIN item_description ITD ON PS.ITEM_ID = ITD.ITEM_ID " +
                    "where  PS.id_pedidos= '" + pedido + "'"; 
                comando.CommandType = CommandType.Text;
                leer = comando.ExecuteReader();

                while (leer.Read())
                {

                    ItemDescripcion estilos = new ItemDescripcion() {

                      ItemEstilo = leer["ITEM_STYLE"].ToString(),
                      ItemId = Convert.ToInt32(leer["ITEM_ID"]),
                      IdSummary=Convert.ToInt32(leer["ID_PO_SUMMARY"])
                };



                    listEstilos.Add(estilos);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
            return listEstilos;
        }

        //Muestra la lista de estilos por IdPedido
        public IEnumerable<ItemDescripcion> ListaItemEstilosPorIdPedido(int? id)
        {
            int pedido = Convert.ToInt32(id);
            Conexion conn = new Conexion();
            List<ItemDescripcion> listEstilos = new List<ItemDescripcion>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select PS.ID_PO_SUMMARY, PS.ITEM_ID, RTRIM(ITD.ITEM_STYLE) as ITEM_STYLE, RTRIM(ITD.DESCRIPTION) as DESCRIPTION, PS.ID_COLOR, CONCAT(RTRIM(C.CODIGO_COLOR),'  ',C.DESCRIPCION ) AS DESCRIPCION, RTRIM(C.CODIGO_COLOR) AS CODIGO from po_summary PS " +
                    "INNER JOIN item_description ITD ON PS.ITEM_ID = ITD.ITEM_ID "+
                    "INNER JOIN cat_colores C ON PS.ID_COLOR = C.ID_COLOR "+
                    "where PS.id_pedidos= '" + pedido + "'";
                comando.CommandType = CommandType.Text;
                leer = comando.ExecuteReader();

                while (leer.Read())
                {

                    ItemDescripcion estilos = new ItemDescripcion()
                    {

                        ItemId = Convert.ToInt32(leer["ITEM_ID"]),
                        ItemEstilo = leer["ITEM_STYLE"].ToString(),
                        Descripcion = leer["DESCRIPTION"].ToString(),
                        IdSummary = Convert.ToInt32(leer["ID_PO_SUMMARY"])
                    };

                    CatColores CatColores = new CatColores() {
                        DescripcionColor = leer["DESCRIPCION"].ToString(),
                        IdColor = Convert.ToInt32(leer["ID_COLOR"]),
                        CodigoColor = leer["CODIGO"].ToString()
                    };


                    estilos.CatColores = CatColores;



                    listEstilos.Add(estilos);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
            return listEstilos;
        }

        //Muestra la lista de PO
        public IEnumerable<OrdenesCompra> ListaRevisionesPO(int idEstilo)
        {

            int resultado=2;
            int temp = idEstilo;
            
            List<OrdenesCompra> listPedidos = new List<OrdenesCompra>();

            Conexion conex = new Conexion();
            try
            {
                while (resultado != 0)
                {
                    SqlCommand com = new SqlCommand();
                    SqlDataReader leerF = null;
                    com.Connection = conex.AbrirConexion();
                    com.CommandText = "Listar_Pedidos_Revisiones";
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@Id", temp);
                    leerF = com.ExecuteReader();
                    resultado = objRevision.ObtenerNoPedidoRevisiones(temp);
                    while (leerF.Read())
                    {

                        OrdenesCompra pedidos = new OrdenesCompra()
                        {
                            IdPedido = Convert.ToInt32(leerF["ID_PEDIDO"]),
                            PO = leerF["PO"].ToString(),
                            VPO = Convert.ToInt32(leerF["VPO"]),
                            Cliente = Convert.ToInt32(leerF["CUSTOMER"]),
                            ClienteFinal = Convert.ToInt32(leerF["CUSTOMER_FINAL"]),
                            FechaCancel = Convert.ToDateTime(leerF["DATE_CANCEL"]),
                            FechaOrden = Convert.ToDateTime(leerF["DATE_ORDER"]),
                            TotalUnidades = Convert.ToInt32(leerF["TOTAL_UNITS"]),
                            IdStatus = Convert.ToInt32(leerF["ID_STATUS"]),

                        };

                        Revision revision = new Revision()
                        {
                            Id = Convert.ToInt32(leerF["ID"]),
                            IdPedido = Convert.ToInt32(leerF["ID_PEDIDO"]),
                            IdRevisionPO = Convert.ToInt32(leerF["ID_REVISION_PO"]),
                            FechaRevision = Convert.ToDateTime(leerF["FECHA_REVISION"])
                        };

                        CatClienteFinal clienteFinal = new CatClienteFinal()
                        {
                            NombreCliente = leerF["NAME_FINAL"].ToString()
                        };

                        CatCliente cliente = new CatCliente()
                        {
                            Nombre = leerF["NAME"].ToString()
                        };

                        temp = revision.IdPedido;

                        pedidos.Revision = revision;
                        pedidos.CatCliente = cliente;
                        pedidos.CatClienteFinal = clienteFinal;

                        listPedidos.Add(pedidos);
                    }
                    leerF.Close();           
                 }
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }      
           return listPedidos;
        }

        //Muestra la lista de PO por fechas
        public IEnumerable<OrdenesCompra> ListaOrdenCompraPorFechas(DateTime? fechaCanc, DateTime? fechaOrden)
        {
                Conexion conex = new Conexion();
            List<OrdenesCompra> listPedidos = new List<OrdenesCompra>();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;               
                com.Connection = conex.AbrirConexion();
                com.CommandText = "Listar_Pedidos_Por_Fechas";
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@FechaCanc", fechaCanc);
                com.Parameters.AddWithValue("@FechaOrden", fechaOrden);
                leerF = com.ExecuteReader();

                while (leerF.Read())
                {

                    OrdenesCompra pedidos = new OrdenesCompra()
                    {
                        IdPedido = Convert.ToInt32(leerF["ID_PEDIDO"]),
                        PO = leerF["PO"].ToString(),
                        VPO = Convert.ToInt32(leerF["VPO"]),
                        Cliente = Convert.ToInt32(leerF["CUSTOMER"]),
                        ClienteFinal = Convert.ToInt32(leerF["CUSTOMER_FINAL"]),
                        FechaCancel = Convert.ToDateTime(leerF["DATE_CANCEL"]),
                        FechaOrden = Convert.ToDateTime(leerF["DATE_ORDER"]),
                        TotalUnidades = Convert.ToInt32(leerF["TOTAL_UNITS"]),
                        IdStatus = Convert.ToInt32(leerF["ID_STATUS"])

                    };
                    CatStatus status = new CatStatus()
                    {
                        Estado = leerF["ESTADO"].ToString()
                    };
                    CatClienteFinal clienteFinal = new CatClienteFinal()
                    {
                        NombreCliente = leerF["NAME_FINAL"].ToString()
                    };

                    pedidos.CatStatus = status;
                    pedidos.CatClienteFinal = clienteFinal;


                    listPedidos.Add(pedidos);
                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return listPedidos;
        }

        //Permite consultar los detalles de un PO
        public OrdenesCompra ConsultarListaPO(int? id)
        {
             Conexion conexion = new Conexion();
            OrdenesCompra pedidos = new OrdenesCompra();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;           

                com.Connection = conexion.AbrirConexion();
                com.CommandText = "Lista_Pedido_Por_Id";
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", id);

                leerF = com.ExecuteReader();
                while (leerF.Read())
                {

                    pedidos.IdPedido = Convert.ToInt32(leerF["ID_PEDIDO"]);
                    pedidos.PO = leerF["PO"].ToString();
                    pedidos.VPO = Convert.ToInt32(leerF["VPO"]);
                    pedidos.Cliente = Convert.ToInt32(leerF["CUSTOMER"]);
                    pedidos.ClienteFinal = Convert.ToInt32(leerF["CUSTOMER_FINAL"]);
                    pedidos.FechaCancel = Convert.ToDateTime(leerF["DATE_CANCEL"]);
                    pedidos.FechaOrden = Convert.ToDateTime(leerF["DATE_ORDER"]);
                    pedidos.TotalUnidades = Convert.ToInt32(leerF["TOTAL_UNITS"]);
                    pedidos.IdStatus = Convert.ToInt32(leerF["ID_STATUS"]);
                    pedidos.Usuario = Convert.ToInt32(leerF["ID_USUARIO"]);
                    DateTime fecha = pedidos.FechaCancel;
                    DateTime dt = fecha;
                    for (int k = 0; k < 2; k++)
                    {
                        dt = dt.AddDays(-1);
                    }
                    if (dt.DayOfWeek == DayOfWeek.Saturday) { dt = dt.AddDays(-1); }
                    if (dt.DayOfWeek == DayOfWeek.Sunday) { dt = dt.AddDays(-2); }
                    pedidos.FechaFinalOrden = dt;
                }
                leerF.Close();
            }
            finally
            {
                conexion.CerrarConexion();
                conexion.Dispose();
            }
           
            return pedidos;

        }


        //Permite crear un nuevo PO
        public void AgregarPO(OrdenesCompra ordenCompra)
        {
            Conexion conn = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "AgregarPedido";
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.AddWithValue("@idPO", ordenCompra.PO);
                comando.Parameters.AddWithValue("@idPOF", ordenCompra.VPO);
                comando.Parameters.AddWithValue("@Customer", ordenCompra.Cliente);
                comando.Parameters.AddWithValue("@CustomerF", ordenCompra.ClienteFinal);
                comando.Parameters.AddWithValue("@datecancel", ordenCompra.FechaCancel);
                comando.Parameters.AddWithValue("@datePO", ordenCompra.FechaOrden);
                comando.Parameters.AddWithValue("@totUnid", ordenCompra.TotalUnidades);
                comando.Parameters.AddWithValue("@idStatus", ordenCompra.IdStatus);
                comando.Parameters.AddWithValue("@idUser", ordenCompra.Usuario);

                comando.ExecuteNonQuery();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }           

        }

        //Permite actualiza la informacion de un PO
        public void ActualizarPedidos(OrdenesCompra ordenCompra)
        {
            Conexion conn = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Actualizar_Pedido";
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.AddWithValue("@idPO", ordenCompra.PO);
                comando.Parameters.AddWithValue("@idPOF", ordenCompra.VPO);
                comando.Parameters.AddWithValue("@Customer", ordenCompra.Cliente);
                comando.Parameters.AddWithValue("@CustomerF", ordenCompra.ClienteFinal);
                comando.Parameters.AddWithValue("@datecancel", ordenCompra.FechaCancel);
                comando.Parameters.AddWithValue("@datePO", ordenCompra.FechaOrden);
                comando.Parameters.AddWithValue("@totUnid", ordenCompra.TotalUnidades);
                comando.Parameters.AddWithValue("@idStatus", ordenCompra.IdStatus);

                comando.ExecuteNonQuery();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }            
        }

        public int Obtener_Utlimo_po() {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            Conexion conex = new Conexion();
            try
            {
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "SELECT ID_PEDIDO FROM PEDIDO WHERE ID_PEDIDO = (SELECT MAX(ID_PEDIDO) FROM PEDIDO) ";
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    return Convert.ToInt32(reader["ID_PEDIDO"]);
                }
            
            }
            finally {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return 0;
        }



        public void ActualizarEstadoPO(int id)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            Conexion conex = new Conexion();
            try
            {
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "UPDATE PEDIDO SET ID_STATUS =5 WHERE ID_PEDIDO='" + id + "'";
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                          conex.CerrarConexion();
            }
            finally {
                conex.CerrarConexion();
                conex.Dispose();
            }
         
        }

        public void ActualizarEstadoPOCancelado(int id)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            Conexion conex = new Conexion();
            try
            {
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "UPDATE PEDIDO SET ID_STATUS =6 WHERE ID_PEDIDO='" + id + "'";
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                conex.CerrarConexion();
            }
            finally {
                conex.CerrarConexion();
                conex.Dispose();
            }

        }

    }
}