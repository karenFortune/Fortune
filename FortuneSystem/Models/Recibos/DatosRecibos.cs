using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace FortuneSystem.Models.Recibos
{
    public class DatosRecibos
    {
        private Conexion conn = new Conexion();
        private SqlCommand comando = new SqlCommand();
        private SqlDataReader leerFilas = null;
        public int total_unidades,total_pendientes,total_recibidas;
        public string fecha, usuario, id_po, lista_po_autocompletado;
        public DateTime fecha_cancelacion;

        public IEnumerable<Recibo> ListaRecibos()
        {
            List<Recibo> listRecibos = new List<Recibo>();
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "SELECT ID_PEDIDO,DATE_ORDER,TOTAL_UNITS,PO FROM PEDIDO";
            leerFilas = comando.ExecuteReader();
            while (leerFilas.Read())
            {
                Recibo invoices = new Recibo();
                invoices.id_po = leerFilas["PO"].ToString();
                invoices.fecha =Convert.ToDateTime(leerFilas["DATE_ORDER"]).ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX"));
                invoices.total = Convert.ToInt32(leerFilas["TOTAL_UNITS"]);
                invoices.id_pedido = Convert.ToInt32(leerFilas["ID_PEDIDO"]);
                listRecibos.Add(invoices);
            }
            leerFilas.Close();
            conn.CerrarConexion();
            return listRecibos;
        }

        

        
        













    }
}