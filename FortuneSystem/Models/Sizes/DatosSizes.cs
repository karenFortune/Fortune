using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
namespace FortuneSystem.Models.Sizes
{
    public class DatosSizes
    {
        private Conexion conn = new Conexion();
        private SqlCommand comando = new SqlCommand();
        private SqlDataReader leerFilas = null;
        public int id_recibo,id_po_summary;
        public string estilo, descripcion;

        public List<Size> ListaTallas( int id_recibo)
        {
            List<Size> listTalla = new List<Size>();
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "SELECT ris.id_size,ris.cantidad,cis.TALLA FROM recibos_item_size ris, CAT_ITEM_SIZE cis where ris.id_recibo='"+id_recibo+"' and cis.ID=ris.id_size";
            leerFilas = comando.ExecuteReader();
            while (leerFilas.Read())
            {
                Size t = new Size();
                t.cantidad = Convert.ToInt32(leerFilas["cantidad"]);
                t.talla = leerFilas["TALLA"].ToString();
                t.id_size= Convert.ToInt32(leerFilas["id_size"]);
                listTalla.Add(t);
            }
            leerFilas.Close();
            conn.CerrarConexion();
            return listTalla;
        }

        public List<Size> ListaTallasPorOrden(int ips)
        {
            List<Size> listTalla = new List<Size>();
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "SELECT CIS.TALLA,ITS.EXTRAS,ITS.EJEMPLOS, ITS.CANTIDAD, IDS.ITEM_STYLE,IDS.DESCRIPTION FROM ITEM_SIZE ITS,PO_SUMMARY PS, CAT_ITEM_SIZE CIS,ITEM_DESCRIPTION IDS WHERE PS.ID_PO_SUMMARY=ITS.ID_SUMMARY AND ITS.TALLA_ITEM=CIS.ID AND IDS.ITEM_ID=PS.ITEM_ID AND PS.ID_PO_SUMMARY='" + ips + "'";
            leerFilas = comando.ExecuteReader();
            while (leerFilas.Read())
            {
                Size t = new Size();
                t.cantidad = Convert.ToInt32(leerFilas["CANTIDAD"]);
                t.talla = leerFilas["TALLA"].ToString();
                t.estilo = leerFilas["ITEM_STYLE"].ToString() +" - "+ leerFilas["DESCRIPTION"].ToString();
                t.extras= Convert.ToInt32(leerFilas["EXTRAS"]);
                t.ejemplos= Convert.ToInt32(leerFilas["EJEMPLOS"]);
                t.totales = t.cantidad + t.extras + t.ejemplos;
                t.total_talla = 0;
                listTalla.Add(t);
            }
            leerFilas.Close();
            conn.CerrarConexion();
            return listTalla;
        }

        public void buscar_estilo( int id)
        {
            //Session["id_po_summary"]
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "SELECT IDS.ITEM_STYLE,ID.ITEM_STYLE,ID.DESCRIPTION FROM PO_SUMMARY PO,ITEM_DESCRIPTION ID WHERE PO.ITEM_ID=ID.ITEM_ID AND PO.ID_PO_SUMMARY='"+id+"' ";
            leerFilas = comando.ExecuteReader();
            while (leerFilas.Read()){
                estilo = Convert.ToString(leerFilas["ITEM_STYLE"]);
                descripcion = Convert.ToString(leerFilas["DESCRIPTION"]);
            }
            leerFilas.Close();
            conn.CerrarConexion();
        }
        


    }//No
}//No