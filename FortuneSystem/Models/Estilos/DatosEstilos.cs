using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace FortuneSystem.Models.Estilos
{
    public class DatosEstilos
    {
        private Conexion conn = new Conexion();
        private SqlCommand comando = new SqlCommand();
        private SqlDataReader leerFilas = null;

        public int total_unidades, total_pendientes, total_recibidas,id_po_summary,id_pedido,id_usuario,id_recibo,id_talla;
        public string fecha, usuario, id_po, lista_po_autocompletado;
        public DateTime fecha_cancelacion;

        public List<Estilo> ListaEstilos(){
            List<Estilo> listEstilos = new List<Estilo>();
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "SELECT PS.ID_PO_SUMMARY, ID.ITEM_STYLE, ID.DESCRIPTION,CC.CODIGO_COLOR,CC.DESCRIPCION,PS.QTY,PS.PRICE,CG.GENERO  "+
                " FROM PO_SUMMARY PS,ITEM_DESCRIPTION ID,CAT_COLORES CC,CAT_GENDER CG WHERE PS.ID_PEDIDOS='"+ id_pedido + "' AND PS.ITEM_ID=ID.ITEM_ID AND "+
                " CC.ID_COLOR=PS.ID_COLOR AND PS.ID_GENDER=CG.ID_GENDER  ";
            leerFilas = comando.ExecuteReader();
            while (leerFilas.Read()){
                Estilo style = new Estilo();
                style.id_po_summary = leerFilas["ID_PO_SUMMARY"].ToString().ToUpper();
                style.id_estilo = leerFilas["ITEM_STYLE"].ToString().ToUpper();
                style.estilo = leerFilas["DESCRIPTION"].ToString().ToUpper();
                style.id_color = Convert.ToString(leerFilas["CODIGO_COLOR"]).ToUpper();
                style.color = Convert.ToString(leerFilas["DESCRIPCION"]).ToUpper();
                style.cantidad = Math.Round(Convert.ToDouble(leerFilas["QTY"]), 2, MidpointRounding.AwayFromZero);
                style.precio = Math.Round(Convert.ToDouble(leerFilas["PRICE"]), 2, MidpointRounding.AwayFromZero);
                style.total_precio = Math.Round((style.cantidad*style.precio), 2, MidpointRounding.AwayFromZero);
                style.genero = leerFilas["GENERO"].ToString().ToUpper();
                style.totales_recibidos = buscar_totales_por_po_summary(style.id_po_summary);
                listEstilos.Add(style);
            }
            leerFilas.Close();
            conn.CerrarConexion();
            return listEstilos;
        }

        public void Obtener_informacion_po(int id){
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "SELECT PO,DATE_CANCEL,TOTAL_UNITS FROM PEDIDO WHERE ID_PEDIDO='" + id + "' ";
            leerFilas = comando.ExecuteReader();
            while (leerFilas.Read()){
                id_po = Convert.ToString(leerFilas["PO"]);
                fecha_cancelacion = Convert.ToDateTime(leerFilas["DATE_CANCEL"]);
                total_unidades = Convert.ToInt32(leerFilas["TOTAL_UNITS"]);
            }
            leerFilas.Close();
            conn.CerrarConexion();
        }

        public void registrar_recibo(string packing,string origen,string material,string notas,int total)
        {
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "INSERT INTO recibos_item (id_po_summary,total,packing,fecha,origen,material,notas,id_usuario,id_pedido) "+
                " VALUES('"+id_po_summary+"','"+total+"','"+packing+"','"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"','"+origen+"','"+material+"','"+notas+"','"+id_usuario+"','"+id_pedido+"')";
            comando.ExecuteNonQuery();
            conn.CerrarConexion();
        }

        public int obtener_ultimo_recibo()
        {
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "SELECT TOP 1 id_recibo FROM recibos_item order by id_recibo desc ";
            leerFilas = comando.ExecuteReader();
            while (leerFilas.Read())
            {
                id_recibo= Convert.ToInt32(leerFilas["id_recibo"]);
            }
            leerFilas.Close();
            conn.CerrarConexion();
            return id_recibo;
        }

        public void registrar_tallas_recibo(int recibo,int talla,string cantidad)
        {
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "INSERT INTO recibos_item_size (id_recibo,id_size,cantidad) VALUES('"+recibo+"','"+talla+"','"+cantidad+"') ";
            comando.ExecuteNonQuery();
            conn.CerrarConexion();
        }
        public int buscar_talla(string talla)
        {
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "SELECT ID FROM CAT_ITEM_SIZE WHERE TALLA='"+talla+"' ";
            leerFilas = comando.ExecuteReader();
            while (leerFilas.Read())
            {
                id_talla = Convert.ToInt32(leerFilas["ID"]);
            }
            leerFilas.Close();
            conn.CerrarConexion();
            return id_talla;
        }

        public int buscar_totales_por_po_summary(string po_summary)
        {
            int tempo = 0;
            Conexion con = new Conexion();
            SqlCommand com = new SqlCommand();
            SqlDataReader leer = null;
            com.Connection = con.AbrirConexion();
            com.CommandText = "SELECT total FROM recibos_item WHERE id_po_summary='" + po_summary + "' ";
            leer = com.ExecuteReader();
            while (leer.Read()){
                tempo +=Convert.ToInt32(leer["total"]);
            }
            leer.Close();
            con.CerrarConexion();
            return tempo;
        }

        public int buscar_estado_estilo_pedido(int po_summary) {
            int cantidad = 0;
            int recibidos = buscar_totales_por_po_summary(Convert.ToString(po_summary));
            Conexion con = new Conexion();
            SqlCommand com = new SqlCommand();
            SqlDataReader leer = null;
            com.Connection = con.AbrirConexion();
            com.CommandText = "SELECT QTY FROM PO_SUMMARY WHERE ID_PO_SUMMARY='" + po_summary + "' ";
            leer = com.ExecuteReader();
            while (leer.Read()){
                cantidad = Convert.ToInt32(leer["QTY"]);
            }
            leer.Close();
            con.CerrarConexion();
            if (recibidos >= cantidad) return 1;
            else return 0;
        }






    }
}