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
using FortuneSystem.Models.Almacen;

namespace FortuneSystem.Models.Staging
{
    public class DatosStaging
    {
        FuncionesInventarioGeneral consultas = new FuncionesInventarioGeneral();
        StagingGeneral stag = new StagingGeneral();
        /*******************************************************************************************************************/

        public List<pedido_staging> buscar_pedidos_recibo(int sucursal) {
            List<pedido_staging> lista = new List<pedido_staging>();
            Conexion con = new Conexion();           
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT r.id_inventario,r.total, i.id_pedido,i.id_estilo,i.descripcion,rg.fecha from recibos_items r,inventario i,recibos rg where "+
                    "  r.id_inventario=i.id_inventario and i.id_sucursal='"+sucursal+"' and r.id_recibo=rg.id_recibo and i.id_estilo!=0 and i.id_pedido!=0 and i.id_categoria_inventario=1 order by rg.fecha desc";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    pedido_staging ps = new pedido_staging();
                    ps.id_pedido = Convert.ToInt32(leer["id_pedido"]);
                    ps.id_estilo= Convert.ToInt32(leer["id_estilo"]);
                    ps.descripcion = leer["descripcion"].ToString();
                    /*ps.id_customer= Convert.ToInt32(leer["id_customer"]);
                    ps.id_customer_final= Convert.ToInt32(leer["id_customer_final"]);*/
                    ps.estilo_nombre = consultas.buscar_descripcion_estilo(ps.id_estilo);
                    ps.po = consultas.obtener_po_id((ps.id_pedido).ToString());
                    ps.total= Convert.ToInt32(leer["total"]);
                    ps.estilo = consultas.obtener_estilo(ps.id_estilo);
                    ps.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    ps.fecha = (Convert.ToDateTime(leer["fecha"])).ToString("MMM dd yyyy");
                    lista.Add(ps);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }

        public List<pedido_staging> lista_papeleta(int inventario,int turno)
        {
            List<pedido_staging> lista = new List<pedido_staging>();
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT r.id_inventario,r.total, i.id_pedido,i.id_estilo,i.descripcion,rg.fecha from recibos_items r,inventario i,recibos rg where " +
                    "  r.id_inventario=i.id_inventario and i.id_inventario='" + inventario + "' and r.id_recibo=rg.id_recibo order by rg.fecha desc";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    pedido_staging ps = new pedido_staging();
                    ps.id_pedido = Convert.ToInt32(leer["id_pedido"]);
                    ps.id_estilo = Convert.ToInt32(leer["id_estilo"]);
                    ps.descripcion = leer["descripcion"].ToString();
                    ps.estilo_nombre = consultas.buscar_descripcion_estilo(ps.id_estilo);
                    ps.po = consultas.obtener_po_id((ps.id_pedido).ToString());
                    ps.total = Convert.ToInt32(leer["total"]);
                    ps.estilo = consultas.obtener_estilo(ps.id_estilo);
                    ps.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    ps.fecha = (Convert.ToDateTime(leer["fecha"])).ToString("MMM dd yyyy");
                    if (turno == 1) { ps.turno = "PRIMER TURNO"; } else { ps.turno = "SEGUNDO TURNO"; }                    
                    lista.Add(ps);
                }
                leer.Close();
            }
            finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }

        public int buscar_id_summary(string po,string estilo){
            int temp = 0;
            Conexion conx = new Conexion();
            try{
                SqlCommand comx = new SqlCommand();
                SqlDataReader leerx = null;
                comx.Connection = conx.AbrirConexion();
                comx.CommandText = "SELECT ID_PO_SUMMARY FROM PO_SUMMARY WHERE ID_PEDIDOS='" + po + "' AND ITEM_ID='"+estilo+"'  ";
                leerx = comx.ExecuteReader();
                while (leerx.Read()){
                    temp = Convert.ToInt32(leerx["ID_PO_SUMMARY"]);
                }
                leerx.Close();
            }finally{ conx.CerrarConexion(); conx.Dispose(); }
            return temp;
        }

        public void guardar_stag_bd(string pedido,string estilo,int total,int usuario, int summary,string comentarios){
            Conexion con_r = new Conexion();
            try{
                SqlCommand com_r = new SqlCommand();
                com_r.Connection = con_r.AbrirConexion();
                com_r.CommandText = "INSERT INTO staging(id_pedido,id_estilo,total,id_usuario_captura,id_summary,comentarios,fecha) values('" + 
                    pedido + "','" + estilo + "','" + total + "','" + usuario + "','" + summary + "','" + comentarios + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                com_r.ExecuteNonQuery();
            }finally{
                con_r.CerrarConexion(); con_r.Dispose();
            }
        }

        public int obtener_ultimo_stag(){
            int temporal = 0;
            Conexion con_u_r_i = new Conexion();
            try{
                SqlCommand com_u_r_i = new SqlCommand();
                SqlDataReader leer_u_r_i = null;
                com_u_r_i.Connection = con_u_r_i.AbrirConexion();
                com_u_r_i.CommandText = "SELECT TOP 1 id_staging FROM staging order by id_staging desc ";
                leer_u_r_i = com_u_r_i.ExecuteReader();
                while (leer_u_r_i.Read()){
                    temporal = Convert.ToInt32(leer_u_r_i["id_staging"]);
                }
                leer_u_r_i.Close();
            }finally{
                con_u_r_i.CerrarConexion();
                con_u_r_i.Dispose();
            }
            return temporal;
        }

        public void guardar_stag_conteos(int staging,int talla,int pais,int color, int porcentaje, int total,int usuario)
        {
            Conexion con_r = new Conexion();
            try
            {
                SqlCommand com_r = new SqlCommand();
                com_r.Connection = con_r.AbrirConexion();
                com_r.CommandText = "INSERT INTO staging_count(id_staging,id_talla,id_pais,id_color,id_porcentaje,total,id_empleado) values('" +
                    staging + "','" + talla + "','" + pais + "','" + color + "','" +porcentaje + "','" + total + "','"+usuario+"') ";
                com_r.ExecuteNonQuery();
            }
            finally
            {
                con_r.CerrarConexion(); con_r.Dispose();
            }
        }
        //lista_papeleta_staging
        public List<stag_conteo> lista_papeleta_staging(int stag,int turno)
        {
            List<stag_conteo> lista = new List<stag_conteo>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT s.id_pedido,s.id_estilo,s.comentarios,s.fecha,s.id_usuario_captura,sc.id_talla,sc.id_pais,sc.id_color,sc.id_porcentaje,sc.total,sc.id_empleado  " +
                    " from staging_count sc,staging s where sc.id_staging=s.id_staging and sc.id_staging='"+stag+"' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    stag_conteo ps = new stag_conteo();
                    ps.estilo_nombre = consultas.buscar_descripcion_estilo(Convert.ToInt32(leer["id_estilo"]));
                    ps.po = consultas.obtener_po_id(leer["id_pedido"].ToString());
                    ps.estilo = consultas.obtener_estilo(Convert.ToInt32(leer["id_estilo"]));
                    ps.fecha = (Convert.ToDateTime(leer["fecha"])).ToString("MMM dd yyyy");
                    ps.color= consultas.obtener_color_id(Convert.ToString(leer["id_color"]))+"-"+consultas.obtener_descripcion_color_id(Convert.ToString(leer["id_color"]));
                    ps.talla= consultas.obtener_size_id(Convert.ToString(leer["id_talla"]));
                    ps.porcentaje = consultas.obtener_fabric_percent_id(Convert.ToString(leer["id_porcentaje"]));
                    ps.pais= consultas.obtener_pais_id(Convert.ToString(leer["id_pais"]));
                    ps.cantidad = Convert.ToString(leer["total"]);
                    ps.usuario_conteo = obtener_nombre_empleado(Convert.ToInt32(leer["id_empleado"]));
                    ps.observaciones = leer["comentarios"].ToString();
                    ps.usuario = (consultas.buscar_nombre_usuario(leer["id_usuario_captura"].ToString())).ToUpper();
                    if (turno == 1) { ps.turno = "PRIMER TURNO"; } else { ps.turno = "SEGUNDO TURNO"; }
                    lista.Add(ps);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public string obtener_nombre_empleado(int cadena)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT nombre_empleado from empleados where id_empleado ='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["nombre_empleado"]).ToUpper();
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }

        public List<stag_conteo> obtener_staging_inicio()
        {
            List<stag_conteo> lista = new List<stag_conteo>();
            Conexion con = new Conexion();
            int i = 0;
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT  s.id_staging,s.id_pedido,s.id_estilo,s.comentarios,s.fecha,s.id_usuario_captura,s.total  " +
                    " from staging s order by s.id_staging desc";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    stag_conteo ps = new stag_conteo();
                    ps.id_staging = Convert.ToInt32(leer["id_staging"]);
                    ps.estilo_nombre = consultas.buscar_descripcion_estilo(Convert.ToInt32(leer["id_estilo"]));
                    ps.po = consultas.obtener_po_id(leer["id_pedido"].ToString());
                    ps.estilo = consultas.obtener_estilo(Convert.ToInt32(leer["id_estilo"]));
                    ps.fecha = ((Convert.ToDateTime(leer["fecha"])).ToString("MMM dd yyyy")).ToUpper();
                    ps.cantidad = Convert.ToString(leer["total"]);
                    ps.usuario = (consultas.buscar_nombre_usuario(leer["id_usuario_captura"].ToString())).ToUpper();
                    lista.Add(ps);
                }
                leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }






    }//No
}//No