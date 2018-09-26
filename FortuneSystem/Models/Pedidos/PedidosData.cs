using FortuneSystem.Models.Catalogos;
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
        private Conexion conn = new Conexion();
        private SqlCommand comando = new SqlCommand();
        private SqlDataReader leer = null;
        int IdPedido;
        RevisionesData objRevision = new RevisionesData();

        //Muestra la lista de PO
        public IEnumerable<OrdenesCompra> ListaOrdenCompra()
        {
            List<OrdenesCompra> listPedidos = new List<OrdenesCompra>();
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "Listar_Pedidos";
            comando.CommandType = CommandType.StoredProcedure;
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
                    NombreCliente = leer["NAME"].ToString()
                };

                pedidos.Historial = objRevision.ObtenerPedidoRevisiones(pedidos.IdPedido);

                pedidos.CatStatus = status;
                pedidos.CatClienteFinal = clienteFinal;
   

                listPedidos.Add(pedidos);
            }
            leer.Close();
            conn.CerrarConexion();

            return listPedidos;
        }

        //Muestra la lista de PO por fechas
        public IEnumerable<OrdenesCompra> ListaOrdenCompraPorFechas(DateTime? fechaCanc, DateTime? fechaOrden)
        {
                Conexion conex = new Conexion();
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;
        List<OrdenesCompra> listPedidos = new List<OrdenesCompra>();
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
                    NombreCliente = leerF["NAME"].ToString()
                };

                pedidos.CatStatus = status;
                pedidos.CatClienteFinal = clienteFinal;


                listPedidos.Add(pedidos);
            }
            leerF.Close();
            conex.CerrarConexion();

            return listPedidos;
        }

        //Permite consultar los detalles de un PO
        public OrdenesCompra ConsultarListaPO(int? id)
        {
             Conexion conexion = new Conexion();
             SqlCommand com = new SqlCommand();
             SqlDataReader leerF = null;
        OrdenesCompra pedidos = new OrdenesCompra();

            com.Connection = conexion.AbrirConexion();
            com.CommandText = "Lista_Pedido_Por_Id";
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Id", id);

            leerF = com.ExecuteReader();
            while (leerF.Read())
            {

                pedidos.PO = leerF["PO"].ToString();
                pedidos.VPO = Convert.ToInt32(leerF["VPO"]);

                pedidos.Cliente = Convert.ToInt32(leerF["CUSTOMER"]);
                pedidos.ClienteFinal = Convert.ToInt32(leerF["CUSTOMER_FINAL"]);
                pedidos.FechaCancel = Convert.ToDateTime(leerF["DATE_CANCEL"]);
                pedidos.FechaOrden = Convert.ToDateTime(leerF["DATE_ORDER"]);
                pedidos.TotalUnidades = Convert.ToInt32(leerF["TOTAL_UNITS"]);
                pedidos.IdStatus = Convert.ToInt32(leerF["ID_STATUS"]);

            }
            leerF.Close();
            conexion.CerrarConexion();
            return pedidos;

        }


        //Permite crear un nuevo PO
        public void AgregarPO(OrdenesCompra ordenCompra)
        {
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
                 
            comando.ExecuteNonQuery();
            conn.CerrarConexion();

        }

        //Permite actualiza la informacion de un PO
        public void ActualizarPedidos(OrdenesCompra ordenCompra)
        {
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
            conn.CerrarConexion();
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
                conex.CerrarConexion();
            }
            finally { conex.CerrarConexion(); }
            return 0;
        }      

       
    }
}