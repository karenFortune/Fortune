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
namespace FortuneSystem.Models.Staging
{
    public class DatosStaging
    {
        private Conexion conn = new Conexion();
        private SqlCommand comando = new SqlCommand();
        private SqlDataReader leerFilas = null;

        public int id_recibo,id_po_summary,id_pedido,id_staging;
        public string estilo, descripcion,po;

        public List<Staging> Lista_recibos_index( )
         {
             List<Staging> listaStag = new List<Staging>();
             comando.Connection = conn.AbrirConexion();
             comando.CommandText = "SELECT ri.id_recibo,ri.total,ri.fecha,p.PO from recibos_item ri, PEDIDO p, PO_SUMMARY ps where  ri.id_po_summary=ps.ID_PO_SUMMARY and ps.ID_PEDIDOS=p.ID_PEDIDO order by ri.fecha desc";
             leerFilas = comando.ExecuteReader();
             while (leerFilas.Read())
             {
                 Staging s = new Staging();
                 s.id_recibo = Convert.ToInt32(leerFilas["id_recibo"]);
                 s.total = Convert.ToInt32(leerFilas["total"]);
                 s.po = leerFilas["PO"].ToString();
                 s.fecha= (Convert.ToDateTime(leerFilas["fecha"])).ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX"));
                listaStag.Add(s);
             }
             leerFilas.Close();
             conn.CerrarConexion();
             return listaStag;
         }
        public List<Staging> Lista_stag_pedido( int id){
            List<Staging> listaStag = new List<Staging>();
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "SELECT p.PO,s.id_staging,s.fecha,u.Nombres,u.Apellidos,c.TALLA,s.pais,s.color,s.porcentaje,s.id_recibo from staging s,Usuarios u,CAT_ITEM_SIZE c,PEDIDO p  where s.id_pedido='"+id+"' and s.id_usuario=u.Id and s.id_talla=c.ID and p.ID_PEDIDO=s.id_pedido ";
            leerFilas = comando.ExecuteReader();
            while (leerFilas.Read()){
                Staging s = new Staging();
                s.fecha = (Convert.ToDateTime(leerFilas["fecha"])).ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX"));
                s.nombre_usuario = leerFilas["Nombres"].ToString() + " " + leerFilas["Apellidos"].ToString();
                s.talla = leerFilas["TALLA"].ToString();
                s.pais = leerFilas["pais"].ToString();
                s.color_description = leerFilas["color"].ToString();
                s.porcentaje= leerFilas["porcentaje"].ToString();
                s.total = contar_total_tallas_stag(Convert.ToInt32(leerFilas["id_staging"]));
                s.po= leerFilas["PO"].ToString();
                s.id_staging = Convert.ToInt32(leerFilas["id_staging"]);
                buscar_po_estilo_recibo(Convert.ToInt32(leerFilas["id_recibo"]));
                s.estilo = estilo;
                listaStag.Add(s);
            }
            leerFilas.Close();
            conn.CerrarConexion();
            return listaStag;
        }

        public int contar_total_tallas_stag(int stag)
        {
            int total = 0;
            Conexion con = new Conexion();
            SqlCommand com = new SqlCommand();
            SqlDataReader leer = null;
            com.Connection = con.AbrirConexion();
            com.CommandText = " select cantidad from cajas_staging where id_staging='" + stag + "' ";
            leer = com.ExecuteReader();
            while (leer.Read()){
                total += Convert.ToInt32(leer["cantidad"]);
            }
            leer.Close();
            con.CerrarConexion();
            return total;
        }

        public List<Staging> Lista_tallas_recibo(int id_recibo){
            List<Staging> listaTallas = new List<Staging>();
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "SELECT ris.id_recibo,ris.id_size,CIS.TALLA from CAT_ITEM_SIZE CIS, recibos_item_size ris where CIS.ID=ris.id_size and ris.id_recibo='"+id_recibo+"' and ris.staging=0 ";
            leerFilas = comando.ExecuteReader();
            while (leerFilas.Read()){
                Staging s = new Staging();
                s.id_size= Convert.ToInt32(leerFilas["id_size"]);
                s.id_recibo= Convert.ToInt32(leerFilas["id_recibo"]);
                s.talla = Convert.ToString(leerFilas["TALLA"]);
                buscar_po_estilo_recibo(s.id_recibo);               
                s.po = po;
                s.estilo = estilo;
                listaTallas.Add(s);
            }
            leerFilas.Close();
            conn.CerrarConexion();
            return listaTallas;
        }

        public void buscar_po_estilo_recibo(int id){
            Conexion con = new Conexion();
            SqlCommand com = new SqlCommand();
            SqlDataReader leer = null;
            com.Connection = con.AbrirConexion();
            com.CommandText = "SELECT  p.ID_PEDIDO,p.PO,itd.ITEM_STYLE,itd.DESCRIPTION from PEDIDO P,ITEM_DESCRIPTION itd,PO_SUMMARY ps,recibos_item ri where ri.id_po_summary=ps.ID_PO_SUMMARY and "+
                " ps.ID_PEDIDOS=p.ID_PEDIDO and ps.ITEM_ID=itd.ITEM_ID and ri.id_recibo='" + id + "' ";
            leer = com.ExecuteReader();
            while (leer.Read()){
                po = Convert.ToString(leer["PO"]);
                estilo= Convert.ToString(leer["ITEM_STYLE"])+" "+ Convert.ToString(leer["DESCRIPTION"]);
                id_pedido= Convert.ToInt32(leer["ID_PEDIDO"]);
            }
            leer.Close();
            con.CerrarConexion();
        }

        public void buscar_estado_stag_recibo(int recibo)
        {
            int i = 0,s;
            Conexion con = new Conexion();
            SqlCommand com = new SqlCommand();
            SqlDataReader leer = null;
            com.Connection = con.AbrirConexion();
            com.CommandText = "SELECT staging FROM recibos_item_size where id_recibo='" + recibo + "' ";
            leer = com.ExecuteReader();
            while (leer.Read()){
                s = Convert.ToInt32(leer["staging"]);
                if (s == 0) {
                    i++;
                }
            }leer.Close();
            con.CerrarConexion();
            if (i == 0) marcar_recibo(recibo);
        }

        public void marcar_recibo(int recibo){
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "UPDATE recibos_item set staging=1 where id_recibo='"+recibo+"' ";
            comando.ExecuteNonQuery();
            conn.CerrarConexion();
        }

        public void marcar_talla_recibo(int recibo, string talla){
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "UPDATE recibos_item_size set staging=1 where id_recibo='" + recibo + "' and id_size='"+talla+"' ";
            comando.ExecuteNonQuery();
            conn.CerrarConexion();
        }

        public int guardar_stag(int recibo, int usuario,string porcentaje, string talla,string color,string pais){
            buscar_po_estilo_recibo(recibo);
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "INSERT INTO staging (id_recibo,id_pedido,fecha,id_usuario,id_talla,pais,color,porcentaje) VALUES "+
                "('"+recibo+"','"+id_pedido+"','"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"','"+usuario+"','"+talla+"','"+pais+"','"+color+"','"+porcentaje+"') ";
            comando.ExecuteNonQuery();
            conn.CerrarConexion();
          return buscar_ultimo_stag();
        }
        public int buscar_ultimo_stag()
        {
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "SELECT TOP 1 id_staging FROM staging order by id_staging desc ";
            leerFilas = comando.ExecuteReader();
            while (leerFilas.Read())
            {
                id_staging = Convert.ToInt32(leerFilas["id_staging"]);
            }
            leerFilas.Close();
            conn.CerrarConexion();
            return id_staging;
        }
        public void guardar_cajas_stag(int staging,string cantidad){
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "INSERT INTO cajas_staging (id_staging,cantidad) VALUES ('"+staging+"','"+cantidad+"') ";
            comando.ExecuteNonQuery();
            conn.CerrarConexion();
        }

        public List<String> Lista_colores()
        {
            Conexion con = new Conexion();
            SqlCommand com = new SqlCommand();
            SqlDataReader leer = null;
            List<String> Lista= new List<String>();
            com.Connection = con.AbrirConexion();
            com.CommandText = "SELECT DESCRIPCION from CAT_COLORES order by DESCRIPCION asc ";
            leer = com.ExecuteReader();
            while (leer.Read())     {
                
                Lista.Add(leer["DESCRIPCION"].ToString());
            }
            leer.Close();
            con.CerrarConexion();
            //return listaStag;
            return Lista;

            
        }

        public List<Staging> Lista_stag_id(int id)
        {
            List<Staging> listaStag = new List<Staging>();
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "SELECT p.PO,s.id_staging,s.fecha,u.Nombres,u.Apellidos,c.TALLA,s.pais,s.color,s.porcentaje,s.id_recibo from staging s,Usuarios u,CAT_ITEM_SIZE c,PEDIDO p  where  s.id_staging='"+id+"' and s.id_usuario=u.Id and s.id_talla=c.ID and p.ID_PEDIDO=s.id_pedido ";
            leerFilas = comando.ExecuteReader();
            while (leerFilas.Read())
            {
                Staging s = new Staging();
                s.fecha = (Convert.ToDateTime(leerFilas["fecha"])).ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX")).ToUpper();
                s.nombre_usuario = leerFilas["Nombres"].ToString() + " " + leerFilas["Apellidos"].ToString();
                s.talla = leerFilas["TALLA"].ToString();
                s.pais = leerFilas["pais"].ToString();
                s.color_description = leerFilas["color"].ToString();
                s.porcentaje = leerFilas["porcentaje"].ToString();
                s.total = contar_total_tallas_stag(Convert.ToInt32(leerFilas["id_staging"]));
                s.po = leerFilas["PO"].ToString();
                s.id_staging = Convert.ToInt32(leerFilas["id_staging"]);
                buscar_po_estilo_recibo(Convert.ToInt32(leerFilas["id_recibo"]));
                s.estilo = estilo;
                listaStag.Add(s);
            }
            leerFilas.Close();
            conn.CerrarConexion();
            return listaStag;
        }








    }//No
}//No