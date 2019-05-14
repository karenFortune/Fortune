using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FortuneSystem.Models.Almacen;
using FortuneSystem.Models.Shipping;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FortuneSystem.Models.Trims{
    public class DatosTrim{
        DatosInventario di = new DatosInventario();
        DatosReportes dr = new DatosReportes();
        DatosShipping ds = new DatosShipping();
        FuncionesInventarioGeneral consultas = new FuncionesInventarioGeneral();
        DatosTransferencias dt = new DatosTransferencias();
        public string query = "";

        public List<recibo> obtener_lista_recibos_trim(string busqueda) {
            if (busqueda != ""){
                string[] fecha = busqueda.Split('*');
                query = "SELECT top 50 r.id_recibo,r.fecha,r.total,r.mp_number,r.mill_po,r.po_reference, u.Nombres,u.Apellidos, " +
                    "s.sucursal from recibos r,Usuarios u,sucursales s WHERE r.id_usuario=u.Id and r.id_sucursal=s.id_sucursal  "+
                " AND r.fecha between '" +fecha[0]+" 00:00:00' and '"+fecha[1]+ " 23:59:59' order by r.id_recibo desc ";
            }else {
                query = "SELECT top 30 r.id_recibo,r.fecha,r.total,r.mp_number,r.mill_po,r.po_reference, u.Nombres,u.Apellidos, " +
                    "s.sucursal from recibos r,Usuarios u,sucursales s " +
                " where r.id_usuario=u.Id and r.id_sucursal=s.id_sucursal order by r.id_recibo desc ";
            }
            //cargar_recibos_trims
            List<recibo> lista = new List<recibo>();
            Conexion con_ltf = new Conexion();
            try{
                SqlCommand com_ltf = new SqlCommand();
                SqlDataReader leer_ltf = null;
                com_ltf.Connection = con_ltf.AbrirConexion();
                com_ltf.CommandText = query;
                leer_ltf = com_ltf.ExecuteReader();
                while (leer_ltf.Read()){
                    int trim= buscar_trims_recibo(Convert.ToInt32(leer_ltf["id_recibo"]));
                    if (trim != 0)
                    {
                        recibo l = new recibo();
                        l.id_recibo = Convert.ToInt32(leer_ltf["id_recibo"]);
                        l.fecha = (Convert.ToDateTime(leer_ltf["fecha"])).ToString("MMM dd yyyy");
                        l.usuario = leer_ltf["Nombres"].ToString() + " " + leer_ltf["apellidos"].ToString();
                        l.total = Convert.ToInt32(leer_ltf["total"]);
                        l.sucursal = leer_ltf["sucursal"].ToString();
                        l.lista_recibos_item = obtener_lista_items_trims(l.id_recibo);
                        l.mp_number = leer_ltf["mp_number"].ToString();
                        l.mill_po = leer_ltf["mill_po"].ToString();
                        l.po_referencia = leer_ltf["po_reference"].ToString();
                    
                        lista.Add(l);
                    }
                }leer_ltf.Close();
            }finally{con_ltf.CerrarConexion(); con_ltf.Dispose();}
            return lista;
        }
        public List<recibos_item> obtener_lista_items_trims(int id_recibo){
            List<recibos_item> lista = new List<recibos_item>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "Select  ri.id_inventario,ri.total,ri.id_recibo_item from recibos_items ri where ri.id_recibo='" + id_recibo + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    recibos_item ri = new recibos_item();
                    ri.id_recibo_item = Convert.ToInt32(leer["id_recibo_item"]);
                    ri.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    ri.total = Convert.ToInt32(leer["total"]);
                    ri.item =di.obtener_inventario(ri.id_inventario);                    
                    lista.Add(ri);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public int buscar_trims_recibo(int salida){
            int trim = 0;
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT i.id_categoria_inventario from inventario i,recibos_items s where s.id_recibo='" + salida + "' and s.id_inventario=i.id_inventario ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    if (Convert.ToInt32(leer_ltd["id_categoria_inventario"]) == 2) { trim++; }
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return trim;
        }
        public int buscar_trims_salida(int salida) {
            int trim = 0;
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT i.id_categoria_inventario from inventario i,salidas_items s where s.id_salida='" + salida + "' and s.id_inventario=i.id_inventario ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    if (Convert.ToInt32(leer_ltd["id_categoria_inventario"]) == 2) { trim++; }
                }leer_ltd.Close();
            }finally{con_ltd.CerrarConexion(); con_ltd.Dispose();}
            return trim;
        }
        public List<salidas_item> obtener_lista_items(int salida){
            List<salidas_item> lista = new List<salidas_item>();
            Conexion connn = new Conexion();
            try{
                SqlCommand commm = new SqlCommand();
                SqlDataReader leerrr = null;
                commm.Connection = connn.AbrirConexion();
                commm.CommandText = "SELECT s.id_inventario,s.cantidad,i.mill_po,i.descripcion,s.id_inventario,s.id_pedido,s.id_estilo,s.codigo,s.id_salida_item from salidas_items s,inventario i where s.id_salida='" + salida + "' and s.id_inventario=i.id_inventario and i.id_categoria_inventario=2";
                leerrr = commm.ExecuteReader();
                while (leerrr.Read()){
                    salidas_item l = new salidas_item();
                    l.id_salida_item= Convert.ToInt32(leerrr["id_salida_item"]);
                    l.id_inventario = Convert.ToInt32(leerrr["id_inventario"]);
                    l.cantidad = Convert.ToInt32(leerrr["cantidad"]);
                    l.descripcion = consultas.buscar_descripcion_item(Convert.ToInt32(leerrr["id_inventario"]));
                    l.po = consultas.buscar_po_item(Convert.ToInt32(leerrr["id_inventario"]));
                    l.estilo = consultas.obtener_estilo(Convert.ToInt32(leerrr["id_inventario"]));
                    l.summary = consultas.obtener_po_summary(Convert.ToInt32(leerrr["id_pedido"]), Convert.ToInt32(leerrr["id_estilo"]));
                    l.po_number = buscar_po_number_summary(l.summary);
                    l.codigo = Convert.ToString(leerrr["codigo"]);
                    l.mp_number = buscar_mp_number_inventario(Convert.ToInt32(leerrr["id_inventario"]));
                    lista.Add(l);
                }leerrr.Close();
            }finally { connn.CerrarConexion(); connn.Dispose(); }
            return lista;
        }
        public string buscar_mp_number_inventario(int inventario){
            string trim = "0";
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT r.mp_number from recibos r,recibos_items ri where r.id_recibo=ri.id_recibo and ri.id_inventario='" + inventario + "'   ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    trim = Convert.ToString(leer_ltd["mp_number"]);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return trim;
        }
        public string buscar_po_number_summary(int summary){
            string trim = "0";
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT NUMBER_PO FROM PACKING_TYPE_SIZE WHERE ID_SUMMARY='" + summary + "'   ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    trim = Convert.ToString(leer_ltd["NUMBER_PO"]);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return trim;
        }
        public List<Pedido_t> lista_ordenes(){
            List<Pedido_t> listar = new List<Pedido_t>();
            Conexion conn = new Conexion();
            try{
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT ID_PEDIDO,PO from PEDIDO where ID_STATUS!=7 AND ID_STATUS!=6 ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()){
                    Pedido_t l = new Pedido_t();
                    l.id_pedido = Convert.ToInt32(leerFilas["ID_PEDIDO"]);
                    l.pedido = leerFilas["PO"].ToString();
                    listar.Add(l);
                }
                leerFilas.Close();
            }
            finally { conn.CerrarConexion(); conn.Dispose(); }
            return listar;
        }
        public List<Estilos_t> lista_estilos_dropdown(string po){
            List<Estilos_t> lista = new List<Estilos_t>();
            Conexion con_led = new Conexion();
            try{
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT ID_PO_SUMMARY,ITEM_ID FROM PO_SUMMARY WHERE ID_PEDIDOS='"+po+ "'  AND ID_ESTADO!=6 AND ID_ESTADO!=7  ";                
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read()){
                    Estilos_t i = new Estilos_t();
                    i.id_po_summary= Convert.ToInt32(leer_led["ID_PO_SUMMARY"]);
                    i.id_estilo = Convert.ToInt32(leer_led["ITEM_ID"]);
                    i.estilo = (consultas.obtener_estilo(i.id_estilo)).Trim();
                    i.descripcion = (consultas.buscar_descripcion_estilo(i.id_estilo)).Trim();
                    lista.Add(i);
                }leer_led.Close();
            }finally{con_led.CerrarConexion(); con_led.Dispose();}
            return lista;
        }
        public List<Talla_t> lista_tallas_dropdown(string estilo){
            string[] estilos = estilo.Split('*');
            List<Talla_t> lista = new List<Talla_t>();
            for (int ii=0; ii<estilos.Length; ii++) {
                Conexion con_led = new Conexion();
                try{
                    SqlCommand com_led = new SqlCommand();
                    SqlDataReader leer_led = null;
                    com_led.Connection = con_led.AbrirConexion();
                    com_led.CommandText = " SELECT TALLA_ITEM FROM ITEM_SIZE WHERE ID_SUMMARY='" + estilos[ii] + "' ";
                    leer_led = com_led.ExecuteReader();
                    while (leer_led.Read()){
                        Talla_t i = new Talla_t();
                        i.id_talla = Convert.ToInt32(leer_led["TALLA_ITEM"]);
                        i.talla = consultas.obtener_size_id(Convert.ToString(leer_led["TALLA_ITEM"]));
                        lista.Add(i);
                    }leer_led.Close();
                }finally { con_led.CerrarConexion(); con_led.Dispose(); }
            }            
            return (lista).Distinct().ToList();
        }
        public List<Item_t> lista_trim_items(){
            List<Item_t> lista = new List<Item_t>();
            Conexion con_led = new Conexion();
            try{
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT item_id,descripcion FROM items_catalogue where tipo=2 ";
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read()){
                    Item_t i = new Item_t();
                    i.id_item = Convert.ToInt32(leer_led["item_id"]);
                    i.descripcion = Convert.ToString(leer_led["descripcion"]);
                    lista.Add(i);
                }leer_led.Close();
            }finally { con_led.CerrarConexion(); con_led.Dispose(); }
            return lista;
        }
        public int obtener_total_estilo(string summary,string talla){
            string[] Estilos = summary.Split('*');
            int total = 0;
            for (int ii = 0; ii < Estilos.Length; ii++){
                if (talla == "0"){
                    query = "SELECT CANTIDAD FROM ITEM_SIZE WHERE ID_SUMMARY='" + Estilos[ii] + "'";
                }else{
                    query = "SELECT CANTIDAD FROM ITEM_SIZE WHERE ID_SUMMARY='" + Estilos[ii] + "' and TALLA_ITEM='" + talla + "'";
                }
                Conexion con_ltd = new Conexion();
                try{
                    SqlCommand com_ltd = new SqlCommand();
                    SqlDataReader leer_ltd = null;
                    com_ltd.Connection = con_ltd.AbrirConexion();
                    com_ltd.CommandText = query;
                    leer_ltd = com_ltd.ExecuteReader();
                    while (leer_ltd.Read()){
                        total += Convert.ToInt32(leer_ltd["CANTIDAD"]);
                    }leer_ltd.Close();
                }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            }
            return total;
        }
        public string obtener_family_trim_item(string item){
            string trim = "";
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT fabric_type from items_catalogue where item_id='"+item+"'";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    trim = Convert.ToString(leer_ltd["fabric_type"]);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return trim;
        }
        public void guardar_request(string total, string estilo, string item, string talla,int usuario,string cantidad,string blank){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO trim_requests(id_po_summary,id_size,id_item,total,restante,revision,usuario,fecha,cantidad,blanks,id_status) VALUES "+
                    "('" + estilo + "','" + talla + "','" + item + "','" + total + "','" + total + "','0','" + usuario + "','"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','"+cantidad+"','"+blank+"','1')";
                com_c.ExecuteNonQuery();
            }finally{con_c.CerrarConexion(); con_c.Dispose();}
        }
        public void modificar_request(string request,string total, int usuario, string cantidad, string blank){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "UPDATE trim_requests SET usuario='"+usuario+"',cantidad='"+cantidad+ "',blanks='" + blank + "'," +
                    "total='" + total + "',fecha='"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                    " WHERE id_request='"+request+"' ";
              com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }

        public void eliminar_trim_request(string request){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText ="DELETE FROM trim_requests WHERE id_request='"+request+"'";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public List<Trim> lista_trims_por_pedir() {
            List<Trim> lista = new List<Trim>();
            Conexion con= new Conexion();
            try{
                SqlCommand com= new SqlCommand();
                SqlDataReader leer= null;
                com.Connection = con.AbrirConexion();
                com.CommandText = " SELECT id_request,id_po_summary,id_size,id_item,restante,usuario,fecha from trim_requests where revision=0 ";
                leer= com.ExecuteReader();
                while (leer.Read()){
                    Trim t = new Trim();
                    t.estilo=buscar_informacion_estilo( Convert.ToInt32(leer["id_po_summary"]));
                    t.po= buscar_informacion_pedido(Convert.ToInt32(leer["id_po_summary"]));                    
                    t.usuario = consultas.buscar_nombre_usuario(Convert.ToString(leer["usuario"]));
                    t.item = buscar_informacion_item(Convert.ToInt32(leer["id_request"]));
                    lista.Add(t);
                    marcar_revision_item(Convert.ToInt32(leer["id_request"]));
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public string obtener_componente_item(int item){
            string lista = "";
            Conexion con_led = new Conexion();
            try{
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT item FROM items_catalogue where item_id='"+item+"' ";
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read()){
                    lista= Convert.ToString(leer_led["item"]);
                }leer_led.Close();
            }finally { con_led.CerrarConexion(); con_led.Dispose(); }
            return lista;
        }
        public string obtener_descripcion_item(int item){
            string lista = "";
            Conexion con_led = new Conexion();
            try{
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT descripcion FROM items_catalogue where item_id='" + item + "' ";
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read()){
                    lista = Convert.ToString(leer_led["descripcion"]);
                }leer_led.Close();
            }finally { con_led.CerrarConexion(); con_led.Dispose(); }
            return lista;
        }
        public void marcar_revision_item(int item){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE trim_requests SET revision=1 WHERE id_request='" + item + "' ";
                com_s.ExecuteNonQuery();
            }finally{con_s.CerrarConexion(); con_s.Dispose();}
        }
        public Estilos_t buscar_informacion_estilo(int summary) {
            Estilos_t e = new Estilos_t();
            e.id_po_summary = summary;
            e.id_estilo = consultas.obtener_estilo_summary(e.id_po_summary);
            e.estilo = consultas.obtener_estilo(e.id_estilo);
            e.descripcion = consultas.buscar_descripcion_estilo(e.id_estilo);
            return e;
        }
        public Pedido_t buscar_informacion_pedido(int summary){
            Pedido_t p = new Pedido_t();
            p.id_pedido = consultas.obtener_id_pedido_summary(summary);
            p.pedido = consultas.obtener_po_id(Convert.ToString(p.id_pedido));
            return p;
        }
        public Item_t buscar_informacion_item(int request){
            Item_t i = new Item_t();
            Conexion con_led = new Conexion();
            try{
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT id_request,id_po_summary,id_size,id_item,restante,usuario,fecha from trim_requests where id_request='"+request+"' ";
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read()){
                    i.id_item = Convert.ToInt32(leer_led["id_item"]);
                    i.total = Convert.ToInt32(leer_led["restante"]);
                    i.componente = obtener_componente_item(i.id_item);
                    i.descripcion = obtener_descripcion_item(i.id_item);
                    i.fecha = (Convert.ToDateTime(leer_led["fecha"])).ToString("MMM dd yyyy");
                }leer_led.Close();
            }finally { con_led.CerrarConexion(); con_led.Dispose(); }            
            return i;
        }
        //
        public List<Trim_inventario> lista_trim_recibidos(){
            List<Trim_inventario> lista = new List<Trim_inventario>();
            Conexion con_ltf = new Conexion();
            try{
                SqlCommand com_ltf = new SqlCommand();
                SqlDataReader leer_ltf = null;
                com_ltf.Connection = con_ltf.AbrirConexion();
                //com_ltf.CommandText = "SELECT id_salida,fecha,total,id_usuario,id_origen,id_destino,estado_aprobacion,estado_entrega,sello,responsable,id_envio,fecha_recibo,driver,pallet from salidas where estado_entrega=1 and id_destino=1 "; 
                com_ltf.CommandText = "SELECT  i.id_inventario,i.id_pedido,i.id_estilo,i.descripcion,i.id_family_trim,i.id_unit,i.id_trim,i.total,i.id_item" +
                    " FROM inventario i,PEDIDO P where P.ID_PEDIDO=i.id_pedido AND P.ID_STATUS!=6 AND P.ID_STATUS!=7 and i.id_categoria_inventario=2 and i.auditoria=0"; 
                leer_ltf = com_ltf.ExecuteReader();
                while (leer_ltf.Read()){
                    Trim_inventario i = new Trim_inventario();
                    i.id_inventario= Convert.ToInt32(leer_ltf["id_inventario"]);
                    i.id_pedido= Convert.ToInt32(leer_ltf["id_pedido"]);
                    i.pedido = consultas.obtener_po_id(Convert.ToString(i.id_pedido));
                    i.id_estilo= Convert.ToInt32(leer_ltf["id_estilo"]);
                    i.estilo = consultas.obtener_estilo(i.id_estilo) + " " + consultas.buscar_descripcion_estilo(i.id_estilo);
                    i.id_family_trim= Convert.ToInt32(leer_ltf["id_family_trim"]);
                    i.family_trim = consultas.obtener_family_id((i.id_family_trim).ToString());
                    i.id_unit= Convert.ToInt32(leer_ltf["id_unit"]);
                    i.unit = consultas.obtener_unit_id(Convert.ToString(i.id_unit));
                    i.id_item= Convert.ToInt32(leer_ltf["id_item"]);
                    i.descripcion = Convert.ToString(leer_ltf["descripcion"]);
                    i.total = Convert.ToInt32(leer_ltf["total"]);
                    lista.Add(i);                   

                }
                leer_ltf.Close();
            }finally { con_ltf.CerrarConexion(); con_ltf.Dispose(); }
            return lista;
        }
        //
        public salidas_item obtener_id_inventario_salida(int salida){
            salidas_item s = new salidas_item();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT id_inventario,id_salida,cantidad,id_salida_item,id_pedido,id_estilo FROM salidas_items WHERE id_salida_item='"+salida+"'";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    s.id_inventario = Convert.ToInt32(leer_ltd["id_inventario"]);
                    s.id_salida= Convert.ToInt32(leer_ltd["id_salida"]);
                    s.cantidad = Convert.ToInt32(leer_ltd["cantidad"]);
                    s.id_salida_item = Convert.ToInt32(leer_ltd["id_salida_item"]);
                    s.summary = consultas.obtener_po_summary(Convert.ToInt32(leer_ltd["id_pedido"]), Convert.ToInt32(leer_ltd["id_estilo"]));
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return s;
        }
        

        public void cambiar_cantidad_salida_item(int salida, int cantidad){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE salidas_items SET cantidad='" + cantidad + "' WHERE id_salida_item='" + salida + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public void actualizar_cantidad_salida(int salida){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE salidas SET total=(SELECT SUM(cantidad) FROM salidas_items WHERE id_salida='"+salida+"') WHERE id_salida='" + salida + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public void sumar_cantidad_item_request(int summary, int cantidad){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE trim_requests SET restante=restante+'" + cantidad + "' WHERE id_po_summary='" + summary + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public void restar_cantidad_item_request(int summary, int cantidad){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE trim_requests SET restante=restante-'" + cantidad + "' WHERE id_po_summary='" + summary + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public List<Estilos_t> obtener_lista_summary(int pedido){
            List<Estilos_t> lista = new List<Estilos_t>();
            Conexion con_led = new Conexion();
            try{
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT ID_PO_SUMMARY,ITEM_ID,ID_GENDER FROM PO_SUMMARY WHERE ID_PEDIDOS='" + pedido + "'  AND ID_ESTADO!=6 AND ID_ESTADO!=7 ";
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read()){
                    Estilos_t i = new Estilos_t();
                    i.id_po_summary = Convert.ToInt32(leer_led["ID_PO_SUMMARY"]);
                    i.id_estilo = Convert.ToInt32(leer_led["ITEM_ID"]);
                    i.estilo = consultas.obtener_estilo(i.id_estilo);
                    i.descripcion = consultas.buscar_descripcion_estilo(i.id_estilo);
                    lista.Add(i);
                }leer_led.Close();
            }finally { con_led.CerrarConexion(); con_led.Dispose(); }
            return lista;
        }
        public List<Estilos_t> obtener_lista_generos(int pedido){
            List<Estilos_t> lista = new List<Estilos_t>();
            Conexion con_led = new Conexion();
            try{
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT distinct ID_GENDER FROM PO_SUMMARY WHERE ID_PEDIDOS='" + pedido + "'  AND ID_ESTADO!=6 AND ID_ESTADO!=7  ";
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read()){
                    Estilos_t i = new Estilos_t();
                    i.genero = consultas.obtener_sigla_genero(Convert.ToString(leer_led["ID_GENDER"]));
                    lista.Add(i);
                }leer_led.Close();
            }finally { con_led.CerrarConexion(); con_led.Dispose(); }
            return lista;
        }
        public List<Trim> obtener_datos_trim_card(string pedido,string usuario) {
            List<Trim> lista = new List<Trim>();
            int id_pedido = consultas.buscar_pedido(pedido);
            Trim t = new Trim();
            t.lista_estilos= obtener_lista_summary(id_pedido);
            t.lista_generos= obtener_lista_generos(id_pedido);
            t.id_pedido = id_pedido;
            t.pedido = Regex.Replace(pedido, @"\s+", " "); 
            t.usuario = consultas.buscar_nombre_usuario(usuario);
            t.customer= Regex.Replace(consultas.obtener_customer_id(Convert.ToString(consultas.buscar_cliente_final_po(pedido))), @"\s+", " ");
            lista.Add(t);                
            return lista;
        }

        public void actualizar_cantidad_inventario(int inventario, int cantidad){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE inventario SET total='" + cantidad + "' WHERE id_inventario='" + inventario + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }


        public List<Trim_item> lista_descripciones_trims(){
            List<Trim_item> lista = new List<Trim_item>();
            Conexion con_led = new Conexion();
            try{
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT item_id,descripcion from items_catalogue where tipo=2 ";
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read()){
                    Trim_item ti = new Trim_item();
                    ti.id_item = Convert.ToInt32(leer_led["item_id"]);
                    ti.descripcion = Convert.ToString(leer_led["descripcion"]);
                    lista.Add(ti);
                }leer_led.Close();
            }finally { con_led.CerrarConexion(); con_led.Dispose(); }
            return lista;
        }

        public List<Trim_item> informacion_editar_item_trim(string id){
            List<Trim_item> lista = new List<Trim_item>();
            Conexion con_led = new Conexion();
            try{
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT item_id,item,body_type,descripcion,fabric_type,unit,minimo from items_catalogue where item_id='"+id+"' ";
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read()){
                    Trim_item ti = new Trim_item();
                    ti.id_item = Convert.ToInt32(leer_led["item_id"]);
                    ti.item = Convert.ToString(leer_led["item"]);
                    ti.descripcion = Convert.ToString(leer_led["descripcion"]);
                    ti.family = Convert.ToString(leer_led["fabric_type"]);
                    ti.unit = Convert.ToString(leer_led["unit"]);
                    ti.minimo = Convert.ToInt32(leer_led["minimo"]);
                    lista.Add(ti);
                }leer_led.Close();
            }finally { con_led.CerrarConexion(); con_led.Dispose(); }
            return lista;
        }

        public void editar_informacion_trim(string id,string item,string minimo,string descripcion,string family,string unidad){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE items_catalogue SET item='" + item + "', descripcion='"+descripcion+ "',body_type='" + unidad + "', " +
                    " unit='" + unidad + "', fabric_type='" + family + "', minimo='"+minimo+"' " +
                    " WHERE item_id='" + id + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }

        public List<Trim_item> lista_trims_tabla_inicio(){
            List<Trim_item> lista = new List<Trim_item>();
            Conexion con_led = new Conexion();
            try{
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT item_id,item,body_type,descripcion,fabric_type,unit,minimo from items_catalogue where minimo!=0 and tipo=2 ";
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read()){
                    Trim_item ti = new Trim_item();
                    ti.id_item = Convert.ToInt32(leer_led["item_id"]);
                    ti.item = Convert.ToString(leer_led["item"]);
                    ti.descripcion = Convert.ToString(leer_led["descripcion"]);
                    ti.family = Convert.ToString(leer_led["fabric_type"]);
                    ti.unit = Convert.ToString(leer_led["unit"]);
                    ti.minimo = Convert.ToInt32(leer_led["minimo"]);
                    ti.total = buscar_totales_item(ti.id_item);
                    if (ti.total < ti.minimo) {
                        lista.Add(ti);
                    }                    
                }leer_led.Close();
            }finally { con_led.CerrarConexion(); con_led.Dispose(); }
            return lista;
        }
        public int buscar_totales_item(int id){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT total from inventario where id_item='" + id + "'"; //AQUI QUEDA PENDIENTE QUE INVENTARIO VOY A REVISAR
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp += Convert.ToInt32(leer["total"]);
                }leer.Close();
            }finally{ con.CerrarConexion(); con.Dispose(); }
            return temp;
        }

        public int obtener_cajas_estilo(string summary,string talla) {
            int cajas = 0,contador_ppk=0;
            List<ratio_tallas> lista = obtener_ratio_estilo(summary, talla);
            foreach (ratio_tallas r in lista) {
                if (r.tipo_empaque == 1) {
                    cajas += r.total / r.piezas;
                } else {
                    if (contador_ppk == 0) {
                        cajas += r.total / r.ratio;
                    }
                    contador_ppk++;
                }
            }
            return cajas;
        }
        public List<ratio_tallas> obtener_ratio_estilo(string summary, string talla) {
            string[] Estilos = summary.Split('*');
            List<ratio_tallas> lista = new List<ratio_tallas>();
            for (int i = 0; i < Estilos.Length; i++) {
                string query;
                if (talla == "0"){
                    query = "SELECT PIECES,RATIO,ID_TALLA,TYPE_PACKING FROM PACKING_TYPE_SIZE WHERE ID_SUMMARY='" + Estilos[i] + "'";
                }else{
                    query = "SELECT PIECES,RATIO,ID_TALLA,TYPE_PACKING FROM PACKING_TYPE_SIZE WHERE ID_SUMMARY='" + Estilos[i] + "' and ID_TALLA='" + talla + "'";
                }
                Conexion con_ltd = new Conexion();
                try{
                    SqlCommand com_ltd = new SqlCommand();
                    SqlDataReader leer_ltd = null;
                    com_ltd.Connection = con_ltd.AbrirConexion();
                    com_ltd.CommandText = query;
                    leer_ltd = com_ltd.ExecuteReader();
                    while (leer_ltd.Read()){
                        ratio_tallas r = new ratio_tallas();
                        r.ratio = Convert.ToInt32(leer_ltd["RATIO"]);
                        r.id_talla = Convert.ToInt32(leer_ltd["ID_TALLA"]);
                        r.piezas = Convert.ToInt32(leer_ltd["PIECES"]);
                        r.tipo_empaque = Convert.ToInt32(leer_ltd["TYPE_PACKING"]);
                        r.total = obtener_total_estilo(summary, talla);
                        lista.Add(r);
                    }leer_ltd.Close();
                }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            }
            return lista;
        }

        public List<Trim_requests> obtener_trims_anteriores(string summary) {
            List<Trim_requests> lista = new List<Trim_requests>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT entregado,id_request,id_po_summary,id_size,id_item,total,restante,revision,usuario,fecha,id_recibo,cantidad,blanks from trim_requests where id_po_summary='"+summary+"'";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Trim_requests tr = new Trim_requests();
                    tr.id_request = Convert.ToInt32(leer_ltd["id_request"]);
                    tr.id_summary = Convert.ToInt32(leer_ltd["id_po_summary"]);
                    tr.id_talla = Convert.ToInt32(leer_ltd["id_size"]);
                    tr.id_item = Convert.ToInt32(leer_ltd["id_item"]);
                    tr.item = consultas.buscar_amt_item(Convert.ToString(tr.id_item))+" "+consultas.buscar_descripcion_item(Convert.ToString(tr.id_item));
                    tr.tipo_item = consultas.buscar_tipo_trim_item(Convert.ToString(tr.id_item));
                    tr.total = Convert.ToInt32(leer_ltd["total"]);
                    tr.cantidad = Convert.ToInt32(leer_ltd["cantidad"]);
                    tr.blanks = Convert.ToInt32(leer_ltd["blanks"]);
                    tr.restante = Convert.ToInt32(leer_ltd["restante"]);
                    tr.fecha=Convert.ToDateTime(leer_ltd["fecha"]).ToString("dd-MM-yyyy");
                    tr.recibo = Convert.ToInt32(leer_ltd["id_recibo"]);
                    tr.id_usuario= Convert.ToInt32(leer_ltd["id_recibo"]);
                    tr.usuario = consultas.buscar_nombre_usuario(Convert.ToString(tr.id_usuario));
                    tr.talla = consultas.obtener_size_id(Convert.ToString(tr.id_talla));                    
                    tr.id_estilo = consultas.obtener_estilo_summary(tr.id_summary);
                    tr.estilo =consultas.obtener_estilo((tr.id_estilo))+" " + consultas.buscar_descripcion_estilo(tr.id_estilo);
                    tr.entregado = Convert.ToInt32(leer_ltd["entregado"]);
                    lista.Add(tr);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }


        public List<Estilos_t> obtener_trims_anteriores_pedido(string pedido){
            List<Estilos_t> lista = new List<Estilos_t>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT ID_PO_SUMMARY FROM PO_SUMMARY WHERE ID_PEDIDOS='"+pedido+ "'  AND ID_ESTADO!=6 AND ID_ESTADO!=7  ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Estilos_t tr = new Estilos_t();
                    tr.id_po_summary = Convert.ToInt32(leer_ltd["ID_PO_SUMMARY"]);
                    tr.id_estilo = consultas.obtener_estilo_summary(tr.id_po_summary);
                    tr.estilo = consultas.obtener_estilo(tr.id_estilo) + " " + consultas.buscar_descripcion_estilo(tr.id_estilo);
                    tr.lista_requests = obtener_trims_anteriores(Convert.ToString(tr.id_po_summary));
                    lista.Add(tr);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }

        public List<Trim_requests> comparacion_inventario_trim(string salida){
            List<Trim_requests> lista = new List<Trim_requests>();
            string[] salidas = salida.Split('*');
            for (int i = 1; i < salidas.Length; i++) {
                int summary=0, item=0, total=0,pedido=0;
                Conexion con_ltd = new Conexion();
                try{
                    SqlCommand com_ltd = new SqlCommand();
                    SqlDataReader leer_ltd = null;
                    com_ltd.Connection = con_ltd.AbrirConexion();
                    com_ltd.CommandText = "SELECT id_inventario,total,id_estilo,id_pedido,id_item from inventario where id_inventario='" + salidas[i] + "'";
                    leer_ltd = com_ltd.ExecuteReader();
                    while (leer_ltd.Read()){
                        summary = consultas.obtener_po_summary(Convert.ToInt32(leer_ltd["id_pedido"]), Convert.ToInt32(leer_ltd["id_estilo"]));
                        item= Convert.ToInt32(leer_ltd["id_item"]);
                        pedido= Convert.ToInt32(leer_ltd["id_pedido"]);
                        total+= Convert.ToInt32(leer_ltd["total"]);
                    }leer_ltd.Close();
                }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
                Trim_requests tr = obtener_trim_comparacion(summary, item, total,pedido);
                if (tr != null) {
                    lista.Add(tr);
                }
            }   
            return lista;
        }

        public Trim_requests obtener_trim_comparacion(int summary,int item, int total,int pedido){
            //Trim_requests lista = new Trim_requests();
            Trim_requests tr = new Trim_requests();            
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT id_request,id_po_summary,id_size,id_item,total,restante,revision,usuario," +
                    " fecha,id_recibo,cantidad,blanks from trim_requests " +
                    " where id_po_summary='" + summary + "' and id_item='" + item + "'";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    tr.id_request = Convert.ToInt32(leer_ltd["id_request"]);
                    tr.id_summary = Convert.ToInt32(leer_ltd["id_po_summary"]);
                    tr.id_talla = Convert.ToInt32(leer_ltd["id_size"]);
                    tr.id_item = Convert.ToInt32(leer_ltd["id_item"]);
                    tr.item = consultas.buscar_amt_item(Convert.ToString(tr.id_item)) + " " + consultas.buscar_descripcion_item(Convert.ToString(tr.id_item));
                    tr.tipo_item = consultas.buscar_tipo_trim_item(Convert.ToString(tr.id_item));
                    tr.total = Convert.ToInt32(leer_ltd["total"]);
                    //tr.cantidad = Convert.ToInt32(leer_ltd["cantidad"]);
                    tr.cantidad =total; //LO QUE LLEGÓ
                    tr.blanks = Convert.ToInt32(leer_ltd["blanks"]);
                    tr.restante = Convert.ToInt32(leer_ltd["restante"]);                    
                    tr.recibo = Convert.ToInt32(leer_ltd["id_recibo"]);
                    //tr.fecha = Convert.ToDateTime(leer_ltd["fecha"]).ToString("dd-MM-yyyy");
                    tr.fecha = buscar_fecha_recibo_item(tr.recibo);
                    tr.id_usuario = Convert.ToInt32(leer_ltd["id_recibo"]);
                    tr.usuario = consultas.buscar_nombre_usuario(Convert.ToString(tr.id_usuario));
                    tr.talla = consultas.obtener_size_id(Convert.ToString(tr.id_talla));
                    tr.id_estilo = consultas.obtener_estilo_summary(tr.id_summary);
                    tr.estilo = consultas.obtener_estilo((tr.id_estilo)) + " " + consultas.buscar_descripcion_estilo(tr.id_estilo);
                    tr.pedido = consultas.obtener_po_id(Convert.ToString(pedido));                    
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return tr;
        }

        public string buscar_fecha_recibo_item(int recibo){
            string temp = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT fecha from recibos where id_recibo='" + recibo + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToDateTime(leer["fecha"]).ToString("dd-MM-yyyy");
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return temp;
        }
        public void cambiar_estado_auditoria_inventario(string inventario){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE inventario SET auditoria=1 WHERE id_inventario='" + inventario + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
                    
        public List<Pedido_t> obtener_lista_ordenes_estados(string busqueda) {
            string query = "";
            if (busqueda == "0"){
                query = "SELECT TOP 30 ID_PEDIDO,PO FROM PEDIDO WHERE ID_STATUS!=6 AND ID_STATUS!=7";
            }else {
                query = "SELECT TOP 30 ID_PEDIDO,PO FROM PEDIDO WHERE ID_STATUS!=6 AND ID_STATUS!=7 AND PO LIKE'%"+busqueda+"%'";
            }
            List<Pedido_t> lista = new List<Pedido_t>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                //com.CommandText = "SELECT TOP 25 ID_PEDIDO,PO FROM PEDIDO WHERE ID_STATUS!=6 AND ID_STATUS!=7";
                com.CommandText =query;
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Pedido_t p = new Pedido_t();
                    p.id_pedido= Convert.ToInt32(leer["ID_PEDIDO"]);
                    p.pedido= (Convert.ToString(leer["PO"])).Trim();
                    p.lista_estilos= obtener_trims_anteriores_pedido(Convert.ToString(leer["ID_PEDIDO"]));
                    lista.Add(p);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }

        public List<Inventario> obtener_lista_trims_inicio(string busqueda,string fecha) {
            List<Inventario> lista = new List<Inventario>();
            if (busqueda == "0" && fecha=="0"){
                lista.AddRange(lista_trims_inicio_default());                
            }else{
                if (busqueda == "0" && fecha != "0") {
                    lista.AddRange(lista_trims_inicio_fecha(busqueda, fecha));
                } else {
                    lista.AddRange(lista_trims_inicio_trim(busqueda, fecha));
                    lista.AddRange(lista_trims_inicio_mp(busqueda, fecha));
                    lista.AddRange(lista_trims_inicio_po(busqueda, fecha));
                }                
            }            
            return lista;
        }
        public List<Inventario> lista_trims_inicio_default() {
            List<Inventario> lista = new List<Inventario>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT DISTINCT r.fecha,i.id_inventario,i.id_pedido,i.id_estilo,i.total,i.id_item,i.auditoria,ic.descripcion,ic.fabric_type " +
                    "  FROM recibos r,inventario i,recibos_items ri,items_catalogue ic" +
                    " WHERE i.id_inventario=ri.id_inventario AND ic.item_id=i.id_item AND ri.id_recibo=r.id_recibo " +
                    " AND r.fecha between '" + DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00' AND '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +//AND r.fecha<'2019-03-01 12:00:00'
                    "AND ic.tipo=2 ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Inventario i = new Inventario();
                    i.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer["id_pedido"]));
                    i.estilo = consultas.obtener_estilo(Convert.ToInt32(leer["id_estilo"])) + " " + consultas.buscar_descripcion_estilo(Convert.ToInt32(leer["id_estilo"]));
                    i.total = Convert.ToInt32(leer["total"]);
                    i.auditoria = Convert.ToInt32(leer["auditoria"]);
                    i.descripcion = Convert.ToString(leer["descripcion"]) + " " + Convert.ToString(leer["fabric_type"]);
                    i.fecha = Convert.ToDateTime(leer["fecha"]).ToString("dd-MM-yyyy");
                    lista.Add(i);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<Inventario> lista_trims_inicio_trim(string busqueda, string fecha){
            string query = "SELECT DISTINCT r.fecha,i.id_inventario,i.id_pedido,i.id_estilo,i.total,i.id_item,i.auditoria,ic.descripcion,ic.fabric_type " +
                    "  FROM recibos r,inventario i,recibos_items ri,items_catalogue ic" +
                    " WHERE i.id_inventario=ri.id_inventario AND ic.item_id=i.id_item AND ri.id_recibo=r.id_recibo AND ic.tipo=2 ";                    
            if (busqueda != "0") { query+= " AND i.descripcion like '%" + busqueda + "%'  "; }
            if (fecha != "0") { query+= " AND r.fecha between '" + fecha + " 00:00:00' and  '" + fecha + " 23:59:59'  "; }
            List<Inventario> lista = new List<Inventario>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = query;                  
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Inventario i = new Inventario();
                    i.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer["id_pedido"]));
                    i.estilo = consultas.obtener_estilo(Convert.ToInt32(leer["id_estilo"])) + " " + consultas.buscar_descripcion_estilo(Convert.ToInt32(leer["id_estilo"]));
                    i.total = Convert.ToInt32(leer["total"]);
                    i.auditoria = Convert.ToInt32(leer["auditoria"]);
                    i.descripcion = Convert.ToString(leer["descripcion"]) + " " + Convert.ToString(leer["fabric_type"]);
                    i.fecha = Convert.ToDateTime(leer["fecha"]).ToString("dd-MM-yyyy");
                    lista.Add(i);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<Inventario> lista_trims_inicio_mp(string busqueda, string fecha){
            string query = "SELECT DISTINCT r.fecha,i.id_inventario,i.id_pedido,i.id_estilo,i.total,i.id_item,i.auditoria,ic.descripcion,ic.fabric_type " +
                    "  FROM recibos r,inventario i,recibos_items ri,items_catalogue ic" +
                    " WHERE i.id_inventario=ri.id_inventario AND ic.item_id=i.id_item AND ri.id_recibo=r.id_recibo AND ic.tipo=2  ";
            if (busqueda != "0") { query += " AND r.mp_number like '%" + busqueda + "%'  "; }
            if (fecha != "0") { query += " AND r.fecha between '" + fecha + " 00:00:00' and  '" + fecha + " 23:59:59'  "; }
            List<Inventario> lista = new List<Inventario>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = query;
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Inventario i = new Inventario();
                    i.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer["id_pedido"]));
                    i.estilo = consultas.obtener_estilo(Convert.ToInt32(leer["id_estilo"])) + " " + consultas.buscar_descripcion_estilo(Convert.ToInt32(leer["id_estilo"]));
                    i.total = Convert.ToInt32(leer["total"]);
                    i.auditoria = Convert.ToInt32(leer["auditoria"]);
                    i.descripcion = Convert.ToString(leer["descripcion"]) + " " + Convert.ToString(leer["fabric_type"]);
                    i.fecha = Convert.ToDateTime(leer["fecha"]).ToString("dd-MM-yyyy");
                    lista.Add(i);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<Inventario> lista_trims_inicio_po(string busqueda, string fecha){
            string query = "SELECT DISTINCT r.fecha,i.id_inventario,i.id_pedido,i.id_estilo,i.total,i.id_item,i.auditoria,ic.descripcion,ic.fabric_type " +
                    "  FROM recibos r,inventario i,recibos_items ri,items_catalogue ic,PEDIDO p" +
                    " WHERE i.id_inventario=ri.id_inventario AND ic.item_id=i.id_item AND ri.id_recibo=r.id_recibo AND i.id_pedido=p.ID_PEDIDO AND ic.tipo=2 ";
            if (busqueda != "0") { query += "  AND p.PO like '%" + busqueda + "%' "; }
            if (fecha != "0") { query += " AND r.fecha between '" + fecha + " 00:00:00' and  '" + fecha + " 23:59:59'  "; }
            List<Inventario> lista = new List<Inventario>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = query;
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Inventario i = new Inventario();
                    i.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer["id_pedido"]));
                    i.estilo = consultas.obtener_estilo(Convert.ToInt32(leer["id_estilo"])) + " " + consultas.buscar_descripcion_estilo(Convert.ToInt32(leer["id_estilo"]));
                    i.total = Convert.ToInt32(leer["total"]);
                    i.auditoria = Convert.ToInt32(leer["auditoria"]);
                    i.descripcion = Convert.ToString(leer["descripcion"]) + " " + Convert.ToString(leer["fabric_type"]);
                    i.fecha = Convert.ToDateTime(leer["fecha"]).ToString("dd-MM-yyyy");
                    lista.Add(i);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<Inventario> lista_trims_inicio_fecha(string busqueda, string fecha){        
            List<Inventario> lista = new List<Inventario>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT DISTINCT r.fecha,i.id_inventario,i.id_pedido,i.id_estilo,i.total,i.id_item,i.auditoria,ic.descripcion,ic.fabric_type " +
                    "  FROM recibos r,inventario i,recibos_items ri,items_catalogue ic " +
                    " WHERE i.id_inventario=ri.id_inventario AND ic.item_id=i.id_item AND ri.id_recibo=r.id_recibo " +
                    " AND r.fecha between '" + fecha + " 00:00:00' and  '" + fecha + " 23:59:59'  " +
                    "AND ic.tipo=2 ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Inventario i = new Inventario();
                    i.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer["id_pedido"]));
                    i.estilo = consultas.obtener_estilo(Convert.ToInt32(leer["id_estilo"])) + " " + consultas.buscar_descripcion_estilo(Convert.ToInt32(leer["id_estilo"]));
                    i.total = Convert.ToInt32(leer["total"]);
                    i.auditoria = Convert.ToInt32(leer["auditoria"]);
                    i.descripcion = Convert.ToString(leer["descripcion"]) + " " + Convert.ToString(leer["fabric_type"]);
                    i.fecha = Convert.ToDateTime(leer["fecha"]).ToString("dd-MM-yyyy");
                    lista.Add(i);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        /*******************************************************************************************************************************************************/
        public int obtener_id_recibo_inventario(int inventario){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT TOP 1 id_recibo from recibos_items where id_inventario='" + inventario + "' order by id_recibo_item desc ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["id_recibo"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }

        public int obtener_id_recibo_inventario_detalles(int inventario, int total)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT TOP 1 id_recibo from recibos_items where id_inventario='" + inventario + "' and total='" + total + "' order by id_recibo_item desc ";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["id_recibo"]);
                }
                leer.Close();
            }
            finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }
        public List<recibo> buscar_mp_recibos_hoy() {
            List<recibo> lista = new List<recibo>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT distinct r.mp_number from recibos r,recibos_items ri,inventario i,items_catalogue ic where" +
                    " r.id_recibo=ri.id_recibo AND ri.id_inventario=i.id_inventario AND i.id_item=ic.item_id " +
                    " AND r.fecha between '" + DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00' AND '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +//AND r.fecha<'2019-03-01 12:00:00'
                    "AND ic.tipo=2 ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    recibo i = new recibo();
                    i.mp_number = Convert.ToString(leer["mp_number"]);                  
                    lista.Add(i);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public Trim_requests buscar_trim( int summary,int item) {
            Trim_requests tr = new Trim_requests();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT id_request,id_po_summary,id_size,id_item,total,restante,revision,usuario," +
                    " fecha,id_recibo,cantidad,blanks from trim_requests " +
                    " where id_po_summary='" + summary + "' and id_item='" + item + "'";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    tr.id_request = Convert.ToInt32(leer_ltd["id_request"]);
                    tr.id_summary = Convert.ToInt32(leer_ltd["id_po_summary"]);
                    tr.id_talla = Convert.ToInt32(leer_ltd["id_size"]);
                    tr.id_item = Convert.ToInt32(leer_ltd["id_item"]);
                    tr.item = consultas.buscar_amt_item(Convert.ToString(tr.id_item)) + " " + consultas.buscar_descripcion_item(Convert.ToString(tr.id_item));
                    tr.tipo_item = consultas.buscar_tipo_trim_item(Convert.ToString(tr.id_item));
                    tr.total = Convert.ToInt32(leer_ltd["total"]);
                    tr.cantidad = Convert.ToInt32(leer_ltd["cantidad"]);
                    tr.blanks = Convert.ToInt32(leer_ltd["blanks"]);
                    tr.restante = Convert.ToInt32(leer_ltd["restante"]);
                    tr.recibo = Convert.ToInt32(leer_ltd["id_recibo"]);
                    tr.fecha = Convert.ToDateTime(leer_ltd["fecha"]).ToString("dd-MM-yyyy");
                    //tr.fecha = buscar_fecha_recibo_item(tr.recibo);
                    tr.id_usuario = Convert.ToInt32(leer_ltd["id_recibo"]);
                    tr.usuario = consultas.buscar_nombre_usuario(Convert.ToString(tr.id_usuario));
                    tr.talla = consultas.obtener_size_id(Convert.ToString(tr.id_talla));                    
                    //tr.id_estilo = consultas.obtener_estilo_summary(tr.id_summary);
                    //tr.estilo = consultas.obtener_estilo((tr.id_estilo)) + " " + consultas.buscar_descripcion_estilo(tr.id_estilo);
                    
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return tr;
        }
        public void update_trim_request(int request, int cantidad, int inventario,int recibo){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE trim_requests SET restante=restante-'" + cantidad + "',id_inventario='" + inventario + 
                    "',id_recibo='"+recibo+"' WHERE id_request='" + request + "'  ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public void cambiar_estado_trim_request(int request, int estado){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE trim_requests SET id_status='" + estado + "' WHERE id_request='" + request + "'  ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public void revisar_estados_cantidades_trim(int inventario,int cantidad,int recibo) {
            Inventario inv = dt.consultar_item(inventario);
            Trim_requests trim = buscar_trim(inv.id_summary, inv.id_item);
            update_trim_request(trim.id_request,cantidad,inv.id_inventario,recibo);
            trim.restante = trim.restante - cantidad;
            if (trim.restante <= 0) {
                cambiar_estado_trim_request(trim.id_request,3);
            } else {
                cambiar_estado_trim_request(trim.id_request, 2);
            }           
        }
        public void revisar_status_trim_request(string request,string total) {
            Conexion con_u_i = new Conexion();
            try{
                SqlCommand com_u_i = new SqlCommand();
                SqlDataReader leer_u_i = null;
                com_u_i.Connection = con_u_i.AbrirConexion();
                com_u_i.CommandText = "SELECT i.total from inventario i,trim_requests tr where tr.id_inventario=i.id_inventario" +
                    " AND tr.id_request='"+request+"' ";
                leer_u_i = com_u_i.ExecuteReader();
                while (leer_u_i.Read()){
                    if (Convert.ToInt32(leer_u_i["total"]) >= Convert.ToInt32(total)){
                        cambiar_estado_trim_request(Convert.ToInt32(request), 3);
                    }else {
                        cambiar_estado_trim_request(Convert.ToInt32(request), 2);
                    }
                }leer_u_i.Close();
            }finally { con_u_i.CerrarConexion(); con_u_i.Dispose(); }
        }
        public void guardar_entrega(string pedido, string entrega, string recibe){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO trim_entregas(entrega,recibe,fecha,id_pedido) VALUES " +
                    "('" + entrega + "','" + recibe + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + pedido + "')";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public int obtener_ultima_entrega(){
            int temp = 0;
            Conexion con_u_i = new Conexion();
            try{
                SqlCommand com_u_i = new SqlCommand();
                SqlDataReader leer_u_i = null;
                com_u_i.Connection = con_u_i.AbrirConexion();
                com_u_i.CommandText = "SELECT TOP 1 id_entrega FROM trim_entregas order by id_entrega desc ";
                leer_u_i = com_u_i.ExecuteReader();
                while (leer_u_i.Read()){
                    temp = Convert.ToInt32(leer_u_i["id_entrega"]);
                }leer_u_i.Close();
            }finally { con_u_i.CerrarConexion(); con_u_i.Dispose(); }
            return temp;
        }
        public void guardar_entrega_item(int entrega, string request, string cantidad){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO trim_entrega_items(id_entrega,id_request,total) VALUES " +
                    "('" + entrega + "','" + request + "','" + cantidad + "')";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public void actualizar_estado_entrega_request(string request, string cantidad){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE trim_requests SET entregado=entregado+'" + cantidad + "' " +
                    " WHERE id_request='" + request + "'  ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public int obtener_id_inventario_request(string request){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_inventario from trim_requests where id_request='" + request + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["id_inventario"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }

        public void actualizar_inventario(int inventario, string cantidad){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE inventario SET total=total-" + cantidad + " " +
                    " WHERE id_inventario='" + inventario + "'  ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public List<Trim_entregas> obtener_lista_entregas_fechas(string inicio,string final) {
            List<Trim_entregas> lista = new List<Trim_entregas>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_entrega,entrega,recibe,fecha,id_pedido from trim_entregas  where" +
                    "  fecha between '" + inicio + " 00:00:00' AND '" + final + " 23:59:59'  ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Trim_entregas i = new Trim_entregas();
                    i.id_entrega = Convert.ToInt32(leer["id_entrega"]);
                    i.fecha = Convert.ToDateTime(leer["fecha"]).ToString("MM/dd/yyyy hh:mm:ss");//dd-MM-yyyy
                    i.entrega = Convert.ToString(leer["entrega"]);
                    i.recibe = Convert.ToString(leer["recibe"]);
                    i.lista_request = obtener_lista_entregas_fechas(i.id_entrega);
                    i.pedido = consultas.obtener_po_id(Convert.ToString(leer["id_pedido"]));
                    lista.Add(i);
                }leer.Close();
            }
            finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<Trim_requests> obtener_lista_entregas_fechas(int entrega){
            List<Trim_requests> lista = new List<Trim_requests>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT tr.id_request,tr.id_po_summary,tr.id_size,tr.id_item,revision,tr.usuario," +
                    " tr.fecha,tr.id_recibo,tr.cantidad,tei.total,tr.total as 'total_orden' from trim_requests tr,trim_entrega_items tei " +
                    " where tei.id_entrega='" + entrega + "' and tei.id_request=tr.id_request";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Trim_requests tr = new Trim_requests();
                    tr.id_request = Convert.ToInt32(leer_ltd["id_request"]);
                    tr.id_summary = Convert.ToInt32(leer_ltd["id_po_summary"]);
                    tr.id_talla = Convert.ToInt32(leer_ltd["id_size"]);
                    tr.id_item = Convert.ToInt32(leer_ltd["id_item"]);
                    tr.item = consultas.buscar_amt_item(Convert.ToString(tr.id_item)) + " " + consultas.buscar_descripcion_item(Convert.ToString(tr.id_item));
                    tr.tipo_item = consultas.buscar_tipo_trim_item(Convert.ToString(tr.id_item));
                    tr.total = Convert.ToInt32(leer_ltd["total"]);     
                    tr.cantidad= Convert.ToInt32(leer_ltd["total_orden"]);
                    tr.talla = consultas.obtener_size_id(Convert.ToString(tr.id_talla));
                    tr.id_estilo = consultas.obtener_estilo_summary(tr.id_summary);
                    tr.estilo = consultas.obtener_estilo((tr.id_estilo)) + " " + consultas.buscar_descripcion_estilo(tr.id_estilo);
                    lista.Add(tr);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }
        public Pedidos_trim buscar_estado_total_pedido(int pedido) {
            Pedidos_trim p = new Pedidos_trim();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT P.PO,P.CUSTOMER,P.DATE_ORDER,CC.NAME from " +
                    " PEDIDO P, CAT_CUSTOMER CC where P.ID_PEDIDO='" + pedido + "' and CC.CUSTOMER=P.CUSTOMER ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    p.id_pedido = pedido;
                    p.pedido = Convert.ToString(leer["PO"]);
                    p.id_customer = Convert.ToInt32(leer["CUSTOMER"]);                    
                    p.customer = Convert.ToString(leer["NAME"]);
                    p.ship_date= Convert.ToDateTime(leer["DATE_ORDER"]).ToString("dd-MMM");
                    List<int> temporal = consultas.Lista_generos_po(pedido);
                    p.gender = "";
                    foreach (int x in temporal) {
                        p.gender += consultas.obtener_genero_id(Convert.ToString(x));
                    }
                    p.gender= Regex.Replace(p.gender, @"\s+", " ");
                    p.lista_families = obtener_lista_familias(pedido);
                    p.lista_empaque = lista_tipos_empaque(Convert.ToString(pedido));
                    p.lista_assort = ds.lista_assortments_pedido(pedido);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return p;
        }
        public List<Family_trim> obtener_lista_familias(int pedido) {
            List<Family_trim> lista = new List<Family_trim>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT id_family_trim,family_trim FROM family_trims order by family_trim ";
                    //" SELECT id_family_trim,family_trim FROM family_trims order by family_trim ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Family_trim ft = new Family_trim();
                    ft.id_family_trim = Convert.ToInt32(leer_ltd["id_family_trim"]);
                    ft.family_trim = Convert.ToString(leer_ltd["family_trim"]);
                    ft.lista_requests = buscar_trims_pedido_familia(pedido,ft.family_trim);
                    lista.Add(ft);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }
        public List<Trim_requests> buscar_trims_pedido_familia(int pedido,string familia){
            List<Trim_requests> lista = new List<Trim_requests>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT tr.entregado,tr.id_request,tr.id_po_summary,tr.id_size,tr.id_item,tr.total,tr.restante, " +
                    " tr.revision,tr.usuario,tr.fecha,tr.id_recibo,tr.id_inventario,tr.id_status,tr.id_inventario,ts.status " +
                    " FROM trim_requests tr,items_catalogue ic,PO_SUMMARY ps, trim_status ts " +
                    " WHERE ps.ID_PEDIDOS='"+pedido+"' AND ps.ID_PO_SUMMARY=tr.id_po_summary AND ic.item_id=tr.id_item " +
                    " AND ic.fabric_type='"+familia+"' AND ts.id_status=tr.id_status ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Trim_requests tr = new Trim_requests();
                    tr.id_request = Convert.ToInt32(leer_ltd["id_request"]);
                    tr.id_summary = Convert.ToInt32(leer_ltd["id_po_summary"]);
                    tr.id_talla = Convert.ToInt32(leer_ltd["id_size"]);
                    tr.id_item = Convert.ToInt32(leer_ltd["id_item"]);
                    tr.item = consultas.buscar_descripcion_item(Convert.ToString(tr.id_item)); 
                    tr.tipo_item = consultas.buscar_tipo_trim_item(Convert.ToString(tr.id_item));
                    tr.total = Convert.ToInt32(leer_ltd["total"]);                                     
                    tr.restante = Convert.ToInt32(leer_ltd["restante"]);
                    tr.cantidad = tr.total-tr.restante;//cuanto ha llegado
                    tr.fecha = Convert.ToDateTime(leer_ltd["fecha"]).ToString("dd-MM");
                    if (tr.id_talla != 0){
                        tr.talla = consultas.obtener_size_id(Convert.ToString(tr.id_talla));
                    }else {
                        tr.talla = "";
                    }
                    tr.id_estilo = consultas.obtener_estilo_summary(tr.id_summary);
                    tr.estilo = consultas.buscar_descripcion_estilo(tr.id_estilo); 
                    tr.entregado = Convert.ToInt32(leer_ltd["entregado"]);
                    tr.id_inventario = Convert.ToInt32(leer_ltd["id_inventario"]);
                    tr.recibo = buscar_id_recibo_inventario(tr.id_inventario);
                    if (tr.recibo != 0) { tr.recibo_item = di.lista_recibo_detalles(Convert.ToString(tr.recibo)); }
                    tr.id_estado = Convert.ToInt32(leer_ltd["id_status"]);
                    tr.estado = Convert.ToString(leer_ltd["status"]);
                    lista.Add(tr);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }
        public string buscar_status_trims(int estado){
            string trim = "";
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT status from trim_status where id_status='" + estado + "'   ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    trim = Convert.ToString(leer_ltd["status"]);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return trim;
        }
        public int buscar_id_recibo_inventario(int inventario){
            int trim = 0;
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT r.id_recibo FROM recibos r,recibos_items ri WHERE" +
                    " r.id_recibo=ri.id_recibo AND ri.id_inventario='" + inventario + "'   ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    trim = Convert.ToInt32(leer_ltd["id_recibo"]);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return trim;
        }
        public int buscar_estado_auditoria(int inventario){
            int trim = 0;
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT auditoria FROM inventario WHERE id_inventario='" + inventario + "'   ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    trim = Convert.ToInt32(leer_ltd["auditoria"]);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return trim;
        }
        public List<Pedidos_trim> obtener_pedidos_fechas(string inicio,string final) {
            List<Pedidos_trim> lista = new List<Pedidos_trim>();            
                Conexion con = new Conexion();
                try{
                    SqlCommand com = new SqlCommand();
                    SqlDataReader leer = null;
                    com.Connection = con.AbrirConexion();
                    com.CommandText = "SELECT P.ID_PEDIDO,P.PO,P.CUSTOMER,P.DATE_ORDER,CC.NAME from " +
                        " PEDIDO P, CAT_CUSTOMER CC where CC.CUSTOMER=P.CUSTOMER " +
                        " and P.DATE_ORDER BETWEEN '" + inicio + " 00:00:00' AND '" + final + " 23:59:59' ";                    
                    leer = com.ExecuteReader();
                    while (leer.Read()){
                        Pedidos_trim p = new Pedidos_trim();
                        p.id_pedido = Convert.ToInt32(leer["ID_PEDIDO"]);
                        p.pedido = Convert.ToString(leer["PO"]);
                        p.id_customer = Convert.ToInt32(leer["CUSTOMER"]);
                        p.customer = Convert.ToString(leer["NAME"]);
                        p.ship_date = Convert.ToDateTime(leer["DATE_ORDER"]).ToString("dd-MMM");
                        List<int> temporal = consultas.Lista_generos_po(p.id_pedido);
                        p.gender = "";
                        foreach (int x in temporal){
                            p.gender += consultas.obtener_genero_id(Convert.ToString(x));
                        }
                        p.gender = Regex.Replace(p.gender, @"\s+", " ");
                        p.lista_families = obtener_lista_familias(p.id_pedido);
                        p.lista_empaque = lista_tipos_empaque(Convert.ToString(p.id_pedido));
                        p.lista_assort = ds.lista_assortments_pedido(p.id_pedido);
                    lista.Add(p);
                    }leer.Close();
                }finally { con.CerrarConexion(); con.Dispose(); }            
            return lista;
        }
        public List<Family_trim> obtener_familias(){
            List<Family_trim> lista = new List<Family_trim>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT id_family_trim,family_trim FROM family_trims order by family_trim ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Family_trim ft = new Family_trim();
                    ft.id_family_trim = Convert.ToInt32(leer_ltd["id_family_trim"]);
                    ft.family_trim = Convert.ToString(leer_ltd["family_trim"]);                    
                    lista.Add(ft);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }
        public List<Empaque> lista_tipos_empaque(string id_pedido){
            List<Empaque> lista = new List<Empaque>();
            Conexion conn = new Conexion();
            try{
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select distinct ITEM_ID from PO_SUMMARY where ID_PEDIDOS='" + id_pedido + "'  AND ID_ESTADO!=6 AND ID_ESTADO!=7  ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()){
                    estilo_shipping l = new estilo_shipping();
                    l.id_estilo = Convert.ToInt32(leerFilas["ITEM_ID"]);
                    l.id_summary = consultas.obtener_po_summary(Convert.ToInt32(id_pedido), l.id_estilo);
                    l.descripcion = consultas.buscar_descripcion_estilo(l.id_estilo); ;
                    List<Empaque> lista_e = new List<Empaque>();
                    List<string> tipo_empaque_temporal = consultas.buscar_tipo_empaque(l.id_summary);
                    foreach (string s in tipo_empaque_temporal){
                        Empaque e = new Empaque();
                        e.tipo_empaque = Convert.ToInt32(s);
                        if (s == "1") { e.lista_ratio = ds.obtener_lista_tallas_estilo(l.id_summary, l.id_estilo); }
                        if (s == "2") { e.lista_ratio = ds.obtener_lista_ratio(l.id_summary, l.id_estilo, 2); }
                        lista.Add(e);
                    }
                }leerFilas.Close();
            }finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        //*********************************************************************************************************************************
        public List<Estilos_t> obtener_trims_pedido_locaciones(string pedido){
            List<Estilos_t> lista = new List<Estilos_t>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT ID_PO_SUMMARY FROM PO_SUMMARY WHERE ID_PEDIDOS='" + pedido + "'  AND ID_ESTADO!=6 AND ID_ESTADO!=7  ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Estilos_t tr = new Estilos_t();
                    tr.id_po_summary = Convert.ToInt32(leer_ltd["ID_PO_SUMMARY"]);
                    tr.id_estilo = consultas.obtener_estilo_summary(tr.id_po_summary);
                    tr.estilo = consultas.obtener_estilo(tr.id_estilo) + " " + consultas.buscar_descripcion_estilo(tr.id_estilo);
                    tr.lista_requests = obtener_trims_sin_locacion(Convert.ToString(tr.id_po_summary));
                    lista.Add(tr);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }
        public List<Trim_requests> obtener_trims_sin_locacion(string summary)
        {
            List<Trim_requests> lista = new List<Trim_requests>();
            Conexion con_ltd = new Conexion();
            try
            {
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT entregado,id_request,id_po_summary,id_size,id_item,total,restante,revision,usuario,fecha,id_recibo,cantidad,blanks from trim_requests where id_po_summary='" + summary + "'";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read())
                {
                    Trim_requests tr = new Trim_requests();
                    tr.id_request = Convert.ToInt32(leer_ltd["id_request"]);
                    tr.id_summary = Convert.ToInt32(leer_ltd["id_po_summary"]);
                    tr.id_talla = Convert.ToInt32(leer_ltd["id_size"]);
                    tr.id_item = Convert.ToInt32(leer_ltd["id_item"]);
                    tr.item = consultas.buscar_amt_item(Convert.ToString(tr.id_item)) + " " + consultas.buscar_descripcion_item(Convert.ToString(tr.id_item));
                    tr.tipo_item = consultas.buscar_tipo_trim_item(Convert.ToString(tr.id_item));
                    tr.total = Convert.ToInt32(leer_ltd["total"]);
                    tr.cantidad = Convert.ToInt32(leer_ltd["cantidad"]);
                    tr.blanks = Convert.ToInt32(leer_ltd["blanks"]);
                    tr.restante = Convert.ToInt32(leer_ltd["restante"]);
                    tr.fecha = Convert.ToDateTime(leer_ltd["fecha"]).ToString("dd-MM-yyyy");
                    tr.recibo = Convert.ToInt32(leer_ltd["id_recibo"]);
                    tr.id_usuario = Convert.ToInt32(leer_ltd["id_recibo"]);
                    tr.usuario = consultas.buscar_nombre_usuario(Convert.ToString(tr.id_usuario));
                    tr.talla = consultas.obtener_size_id(Convert.ToString(tr.id_talla));
                    tr.id_estilo = consultas.obtener_estilo_summary(tr.id_summary);
                    tr.estilo = consultas.obtener_estilo((tr.id_estilo)) + " " + consultas.buscar_descripcion_estilo(tr.id_estilo);
                    tr.entregado = Convert.ToInt32(leer_ltd["entregado"]);
                    lista.Add(tr);
                }
                leer_ltd.Close();
            }
            finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }
        public List<Estilos_t> obtener_trims_anteriores_pedido_location(string pedido){
            List<Estilos_t> lista = new List<Estilos_t>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT ID_PO_SUMMARY FROM PO_SUMMARY WHERE ID_PEDIDOS='" + pedido + "'  AND ID_ESTADO!=6 AND ID_ESTADO!=7  ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Estilos_t tr = new Estilos_t();
                    tr.id_po_summary = Convert.ToInt32(leer_ltd["ID_PO_SUMMARY"]);
                    tr.id_estilo = consultas.obtener_estilo_summary(tr.id_po_summary);
                    tr.estilo =  consultas.buscar_descripcion_estilo(tr.id_estilo);//consultas.obtener_estilo(tr.id_estilo) + " " +
                    tr.lista_requests = obtener_trims_anteriores_location(Convert.ToString(tr.id_po_summary));
                    lista.Add(tr);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }
        public List<Trim_requests> obtener_trims_anteriores_location(string summary){
            List<Trim_requests> lista = new List<Trim_requests>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT entregado,id_request,id_po_summary,id_size,id_item,total,restante,revision,usuario,fecha,id_recibo,cantidad,blanks from trim_requests where id_po_summary='" + summary + "'";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Trim_requests tr = new Trim_requests();
                    tr.id_request = Convert.ToInt32(leer_ltd["id_request"]);
                    tr.id_summary = Convert.ToInt32(leer_ltd["id_po_summary"]);
                    tr.id_talla = Convert.ToInt32(leer_ltd["id_size"]);
                    tr.id_item = Convert.ToInt32(leer_ltd["id_item"]);
                    tr.item = consultas.buscar_descripcion_item(Convert.ToString(tr.id_item));//consultas.buscar_amt_item(Convert.ToString(tr.id_item)) + " " + 
                    tr.tipo_item = consultas.buscar_tipo_trim_item(Convert.ToString(tr.id_item));
                    tr.total = Convert.ToInt32(leer_ltd["total"]);
                    //tr.restante = Convert.ToInt32(leer_ltd["restante"]);
                    tr.cantidad = tr.total-(buscar_cantidad_guardada(tr.id_request));
                    /*tr.fecha = Convert.ToDateTime(leer_ltd["fecha"]).ToString("dd-MM-yyyy");
                    tr.recibo = Convert.ToInt32(leer_ltd["id_recibo"]);
                    tr.id_usuario = Convert.ToInt32(leer_ltd["id_recibo"]);
                    tr.usuario = consultas.buscar_nombre_usuario(Convert.ToString(tr.id_usuario));*/
                    tr.talla = consultas.obtener_size_id(Convert.ToString(tr.id_talla));
                    tr.id_estilo = consultas.obtener_estilo_summary(tr.id_summary);
                    tr.estilo = consultas.obtener_estilo((tr.id_estilo)) + " " + consultas.buscar_descripcion_estilo(tr.id_estilo);
                    //tr.entregado = Convert.ToInt32(leer_ltd["entregado"]);
                    //tr.total = tr.total - tr.cantidad;
                    if (tr.cantidad > 0) {
                        lista.Add(tr);
                    }
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }
        public int buscar_cantidad_guardada(int request){
            int trim = 0;
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT total FROM trim_locations_items WHERE id_request='" + request + "'   ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    trim += Convert.ToInt32(leer_ltd["total"]);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return trim;
        }
        public void guardar_locacion(string locacion, int usuario){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO trim_locations(localizacion,id_usuario,fecha) VALUES " +
                    "('" + locacion + "','" + usuario + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public int obtener_id_locacion(){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT TOP 1 id_local from trim_locations order by id_local desc ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["id_local"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }
        public void guardar_locacion_item(int location, string request, string total){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO trim_locations_items(id_local,total,id_request) VALUES " +
                    "('" + location + "','" + total + "','" + request + "')";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }

        public Pedidos_trim buscar_estado_total_pedido_locacion(int pedido){
            Pedidos_trim p = new Pedidos_trim();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT P.PO,P.CUSTOMER,P.DATE_ORDER,CC.NAME from " +
                    " PEDIDO P, CAT_CUSTOMER CC where P.ID_PEDIDO='" + pedido + "' and CC.CUSTOMER=P.CUSTOMER ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    p.id_pedido = pedido;
                    p.pedido = Convert.ToString(leer["PO"]);
                    p.id_customer = Convert.ToInt32(leer["CUSTOMER"]);
                    p.customer = Convert.ToString(leer["NAME"]);
                    p.ship_date = Convert.ToDateTime(leer["DATE_ORDER"]).ToString("dd-MMM");
                    List<int> temporal = consultas.Lista_generos_po(pedido);
                    p.gender = "";
                    foreach (int x in temporal){
                        p.gender += consultas.obtener_genero_id(Convert.ToString(x));
                    }
                    p.gender = Regex.Replace(p.gender, @"\s+", " ");
                    p.lista_families = obtener_lista_familias_locations(pedido);
                    //p.lista_empaque = lista_tipos_empaque(Convert.ToString(pedido));
                    //p.lista_assort = ds.lista_assortments_pedido(pedido);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return p;
        }
        public List<Family_trim> obtener_lista_familias_locations(int pedido){
            List<Family_trim> lista = new List<Family_trim>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT id_family_trim,family_trim FROM family_trims";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Family_trim ft = new Family_trim();
                    ft.id_family_trim = Convert.ToInt32(leer_ltd["id_family_trim"]);
                    ft.family_trim = Convert.ToString(leer_ltd["family_trim"]);
                    ft.lista_requests = buscar_trims_pedido_familia_locaciones(pedido, ft.family_trim);
                    lista.Add(ft);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }
        public List<Trim_requests> buscar_trims_pedido_familia_locaciones(int pedido, string familia){
            List<Trim_requests> lista = new List<Trim_requests>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT DISTINCT tr.entregado,tr.id_request,tr.id_po_summary,tr.id_size,tr.id_item,tr.total,tr.restante,tr.revision " +
                    " FROM trim_requests tr,items_catalogue ic,PO_SUMMARY ps " +
                    " WHERE ps.ID_PEDIDOS='" + pedido + "' AND ps.ID_PO_SUMMARY=tr.id_po_summary AND ic.item_id=tr.id_item " +
                    " AND ic.fabric_type='" + familia + "'  ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Trim_requests tr = new Trim_requests();
                    tr.id_request = Convert.ToInt32(leer_ltd["id_request"]);
                    tr.id_summary = Convert.ToInt32(leer_ltd["id_po_summary"]);
                    tr.id_talla = Convert.ToInt32(leer_ltd["id_size"]);
                    tr.id_item = Convert.ToInt32(leer_ltd["id_item"]);
                    tr.item = consultas.buscar_descripcion_item(Convert.ToString(tr.id_item));
                    tr.tipo_item = consultas.buscar_tipo_trim_item(Convert.ToString(tr.id_item));
                    tr.total = Convert.ToInt32(leer_ltd["total"]);
                   // tr.cantidad = buscar_cantidad_guardada(tr.id_request);//cuanto se ha guardado
                    tr.lista_locations = buscar_locacion_request(tr.id_request);                    
                    if (tr.id_talla != 0){
                        tr.talla = consultas.obtener_size_id(Convert.ToString(tr.id_talla));
                    }else{ tr.talla = ""; }
                    tr.id_estilo = consultas.obtener_estilo_summary(tr.id_summary);
                    int id_color = consultas.obtener_color_id_item_cat(tr.id_summary);
                    tr.estilo = consultas.obtener_estilo(tr.id_estilo) + " " +consultas.buscar_descripcion_estilo(tr.id_estilo)+" ("+consultas.obtener_color_id(id_color.ToString())+")";
                    tr.entregado = Convert.ToInt32(leer_ltd["entregado"]);
                    tr.lista_entrega = buscar_lista_entregas(tr.id_request);
                    bool isEmpty = !tr.lista_entrega.Any();
                    if (isEmpty){
                        tr.entrega = 0;
                    }else{
                        tr.entrega = 1;
                    }
                    bool isEmptyL = !tr.lista_locations.Any();
                    if (isEmptyL){
                        tr.location = 0;
                    }else{
                        tr.location = 1;
                    }
                    lista.Add(tr);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }
        public List<Trim_location> buscar_locacion_request(int request) {
            List<Trim_location> lista = new List<Trim_location>();            
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT tli.total,tl.localizacion,u.Nombres,u.Apellidos,tl.fecha FROM " +
                    " trim_locations tl,trim_locations_items tli,Usuarios u WHERE " +
                    " tl.id_usuario=u.Id AND tli.id_local=tl.id_local AND tli.id_request='"+request+"'";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Trim_location location = new Trim_location();
                    location.total = Convert.ToInt32(leer_ltd["total"]);
                    location.usuario = Convert.ToString(leer_ltd["Nombres"])+" " + Convert.ToString(leer_ltd["Apellidos"]);
                    location.fecha= (Convert.ToDateTime(leer_ltd["fecha"])).ToString("MMM dd yyyy HH:mm:ss");//yyyy-MM-dd HH:mm:ss
                    location.location = Convert.ToString(leer_ltd["localizacion"]);
                    lista.Add(location);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }
        public List<Trim_entregas> buscar_lista_entregas(int request){
            List<Trim_entregas> lista = new List<Trim_entregas>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT te.entrega,te.recibe,te.fecha,tei.total FROM " +
                    " trim_entregas te,trim_entrega_items tei WHERE " +
                    " te.id_entrega=tei.id_entrega AND tei.id_request='" + request + "'";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Trim_entregas te = new Trim_entregas();
                    te.total = Convert.ToInt32(leer_ltd["total"]);
                    te.recibe = Convert.ToString(leer_ltd["recibe"]);
                    te.entrega = Convert.ToString(leer_ltd["entrega"]);
                    te.fecha = (Convert.ToDateTime(leer_ltd["fecha"])).ToString("MMM dd yyyy HH:mm:ss");//yyyy-MM-dd HH:mm:ss
                    lista.Add(te);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }
        public List<Trim_entregas> obtener_lista_entregas_estilos(string summary){
            List<Trim_entregas> lista = new List<Trim_entregas>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT distinct te.id_entrega,te.entrega,te.recibe,te.fecha,te.id_pedido from trim_entregas te,trim_requests tr,trim_entrega_items tei  where" +
                    "  tr.id_request=tei.id_request and tei.id_entrega=te.id_entrega and tr.id_po_summary='" + summary + "'  ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Trim_entregas i = new Trim_entregas();
                    i.id_entrega = Convert.ToInt32(leer["id_entrega"]);
                    i.fecha = Convert.ToDateTime(leer["fecha"]).ToString("MM/dd/yyyy hh:mm:ss");//dd-MM-yyyy
                    i.entrega = Convert.ToString(leer["entrega"]);
                    i.recibe = Convert.ToString(leer["recibe"]);
                    i.lista_request = obtener_lista_entregas_estilos(i.id_entrega,summary);
                    i.pedido = consultas.obtener_po_id(Convert.ToString(leer["id_pedido"]));
                    lista.Add(i);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<Trim_requests> obtener_lista_entregas_estilos(int entrega,string summary){
            List<Trim_requests> lista = new List<Trim_requests>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT tr.id_request,tr.total as 'orden',tr.id_po_summary,tr.id_size,tr.id_item,revision,tr.usuario," +
                    " tr.fecha,tr.id_recibo,tr.cantidad,tei.total from trim_requests tr,trim_entrega_items tei " +
                    " where tei.id_entrega='" + entrega + "' and tei.id_request=tr.id_request and tr.id_po_summary='"+summary+"'";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Trim_requests tr = new Trim_requests();
                    tr.id_request = Convert.ToInt32(leer_ltd["id_request"]);
                    tr.id_summary = Convert.ToInt32(leer_ltd["id_po_summary"]);
                    tr.id_talla = Convert.ToInt32(leer_ltd["id_size"]);
                    tr.id_item = Convert.ToInt32(leer_ltd["id_item"]);
                    tr.item = consultas.buscar_amt_item(Convert.ToString(tr.id_item)) + " " + consultas.buscar_descripcion_item(Convert.ToString(tr.id_item));
                    tr.tipo_item = consultas.buscar_tipo_trim_item(Convert.ToString(tr.id_item));
                    tr.total = Convert.ToInt32(leer_ltd["total"]);
                    tr.cantidad = Convert.ToInt32(leer_ltd["orden"]);
                    tr.talla = consultas.obtener_size_id(Convert.ToString(tr.id_talla));
                    tr.id_estilo = consultas.obtener_estilo_summary(tr.id_summary);
                    tr.estilo = consultas.obtener_estilo((tr.id_estilo)) + " " + consultas.buscar_descripcion_estilo(tr.id_estilo);
                    lista.Add(tr);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }

        public List<registro_price_tickets> buscar_registro_price_ticket_pedido(int pedido){
            List<registro_price_tickets> lista = new List<registro_price_tickets>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "select ID_PO_SUMMARY,ID_COLOR,ITEM_ID from PO_SUMMARY where ID_PEDIDOS='" + pedido + "'  AND ID_ESTADO!=6 AND ID_ESTADO!=7   ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    registro_price_tickets rpt = new registro_price_tickets();
                    rpt.id_pedido = pedido;
                    rpt.id_estilo = Convert.ToInt32(leer["ITEM_ID"]);
                    rpt.id_summary = Convert.ToInt32(leer["ID_PO_SUMMARY"]);
                    rpt.id_color = Convert.ToInt32(leer["ID_COLOR"]);
                    rpt.estilo = (consultas.obtener_estilo((rpt.id_estilo))).Trim();
                    rpt.descripcion_estilo = (consultas.buscar_descripcion_estilo(rpt.id_estilo)).Trim();
                    rpt.color = ((consultas.obtener_color_id((rpt.id_color).ToString())).Trim()).Remove(0, 2);
                    List<ratio_tallas> lista_tallas = obtener_cantidades_tallas_estilo(rpt.id_summary);
                    foreach (ratio_tallas r in lista_tallas) {
                        rpt.id_talla = r.id_talla;
                        rpt.talla =(r.talla).Trim();
                        rpt.total = (Convert.ToString(r.total)).Trim();
                        rpt.upc = (Convert.ToString(buscar_upc(rpt.id_summary, r.id_talla))).Trim();
                        lista.Add(rpt);
                    }                    
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<ratio_tallas> obtener_cantidades_tallas_estilo(int posummary){
            List<ratio_tallas> lista = new List<ratio_tallas>();
            Conexion conn = new Conexion();
            try{
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select TALLA_ITEM,CANTIDAD from ITEM_SIZE where ID_SUMMARY='" + posummary + "' ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()){
                    ratio_tallas e = new ratio_tallas();
                    e.id_talla = Convert.ToInt32(leerFilas["TALLA_ITEM"]);
                    e.talla = Regex.Replace(consultas.obtener_size_id(Convert.ToString(leerFilas["TALLA_ITEM"])), @"\s+", " ");
                    e.total = Convert.ToInt32(leerFilas["CANTIDAD"]);
                    lista.Add(e);
                }leerFilas.Close();
            }finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public int buscar_upc(int summary,int talla){
            int temp = 0;
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "select UPC from UPC where IdSummary='" + summary + "' and IdTalla='" + talla +"'  ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    temp = Convert.ToInt32(leer_ltd["UPC"]);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return temp;
        }
        public void guardar_price_tickets(string pedido, string total, string estilo, string upc, string descripcion, string color, string talla, string ticket, string dept, string clas, string sub, string retail, string cl,string usuario){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO price_tickets(id_pedido,total, estilo, upc, descripcion, color, talla, ticket, dept,class, sub, retail, cl,id_usuario,fecha) VALUES " +
                    "('" + pedido + "','" + total + "','" + estilo + "','" + upc + "','" + descripcion + "','"+color+"','" + talla + "'," +
                    "'" +ticket  + "','" + dept + "','" + clas + "','"+sub+ "','" + retail + "','" + cl + "','" + usuario + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                com_c.ExecuteNonQuery();
            }
            finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public List<registro_price_tickets> buscar_price_tickets_pedido(int pedido){
            List<registro_price_tickets> lista = new List<registro_price_tickets>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_price_ticket,total,estilo,upc,descripcion,color,talla,ticket,dept,class,sub,retail,cl,id_usuario,fecha " +
                    " FROM price_tickets WHERE id_pedido='" + pedido + "' ";                   
                leer = com.ExecuteReader();
                while (leer.Read()){
                    registro_price_tickets rpt = new registro_price_tickets();
                    rpt.id_pedido = pedido;
                    rpt.pedido = consultas.obtener_po_id(Convert.ToString(pedido));
                    rpt.id_price_ticket = Convert.ToInt32(leer["id_price_ticket"]);
                    rpt.total = Convert.ToString(leer["total"]);
                    rpt.estilo = Convert.ToString(leer["estilo"]);                    
                    rpt.upc = Convert.ToString(leer["upc"]);
                    rpt.descripcion_estilo = Convert.ToString(leer["descripcion"]);
                    rpt.color = Convert.ToString(leer["color"]);
                    rpt.talla = Convert.ToString(leer["talla"]);
                    rpt.tickets = Convert.ToString(leer["ticket"]);
                    rpt.dept = Convert.ToString(leer["dept"]);
                    rpt.clas = Convert.ToString(leer["class"]);
                    rpt.sub = Convert.ToString(leer["sub"]);
                    rpt.retail = Convert.ToString(leer["retail"]);
                    rpt.cl = Convert.ToString(leer["cl"]);
                    rpt.fecha = (Convert.ToDateTime(leer["fecha"])).ToString("MMM dd yyyy");
                    rpt.usuario = consultas.buscar_nombre_usuario(Convert.ToString(leer["id_usuario"]));
                    lista.Add(rpt);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public void marcar_price_tickets_pedido_impreso(int pedido){
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "select ID_PO_SUMMARY from PO_SUMMARY where ID_PEDIDOS='" + pedido + "'  AND ID_ESTADO!=6 AND ID_ESTADO!=7  ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    marcar_estilo_talla_impreso(Convert.ToInt32(leer["ID_PO_SUMMARY"]),0);
                    List<Trim_requests> lista_items = obtener_total_inventario_pt(Convert.ToInt32(leer["ID_PO_SUMMARY"]),0);
                    foreach (Trim_requests t in lista_items) {
                        actualizar_inventario(t.id_inventario, Convert.ToString(t.total));
                        //actualizar_inventario(int inventario, string cantidad)
                    }
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
           
        }
        public void marcar_estilo_talla_impreso(int summary,int talla){
            string query = "";
            if (talla == 0) {
                query = "UPDATE trim_requests SET impreso=1 WHERE id_po_summary='"+summary+"' ";
            } else {
                query = "UPDATE trim_requests SET impreso=1 WHERE id_po_summary='" + summary + "' AND id_size='"+talla+"' ";
            }
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = query;
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public List<Trim_requests> obtener_total_inventario_pt(int summary,int talla){
            List<Trim_requests> lista = new List<Trim_requests>();
            string query = "";
            if (talla == 0){
                query = "SELECT tr.id_request,tr.id_po_summary,tr.id_size,tr.total,tr.id_inventario,tr.id_item FROM " +
                    " trim_requests tr,items_catalogue ic WHERE ic.item_id=tr.id_item AND tr.id_po_summary='" + summary + "' " +
                    " AND ic.fabric_type='PRICE TICKETS'";
            }else{
                query = "SELECT tr.id_po_summary,tr.id_size,tr.total,tr.id_inventario,tr.id_item FROM " +
                    " trim_requests tr,items_catalogue ic WHERE ic.item_id=tr.id_item AND tr.id_po_summary='" + summary + "' " +
                    " AND ic.fabric_type='PRICE TICKETS' AND tr.id_talla='"+talla+"'";
            }
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = query;             
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Trim_requests tr = new Trim_requests();
                    tr.id_request = Convert.ToInt32(leer["id_request"]);
                    tr.id_summary = Convert.ToInt32(leer["id_po_summary"]);
                    tr.id_talla = Convert.ToInt32(leer["id_size"]);
                    tr.id_item = Convert.ToInt32(leer["id_item"]);
                    tr.total = Convert.ToInt32(leer["total"]);
                    tr.id_inventario = Convert.ToInt32(leer["id_inventario"]);                    
                    lista.Add(tr);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public int buscar_family_trim_inventario(int inventario){
            int temp = 0;
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT id_family_trim FROM inventario WHERE id_inventario='" + inventario + "'  ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    temp = Convert.ToInt32(leer_ltd["id_family_trim"]);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return temp;
        }

        public List<Pedidos_trim> obtener_lista_pedidos_pt(){
            List<Pedidos_trim> listar = new List<Pedidos_trim>();
            Conexion conn = new Conexion();
            try{
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT P.ID_PEDIDO,P.PO,P.CUSTOMER,P.DATE_ORDER,CC.NAME from PEDIDO P, CAT_CUSTOMER CC where ID_STATUS!=7 AND ID_STATUS!=6 and CC.CUSTOMER=P.CUSTOMER ";               
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()){
                    Pedidos_trim p = new Pedidos_trim();
                    p.id_pedido = Convert.ToInt32(leerFilas["ID_PEDIDO"]);
                    p.pedido = leerFilas["PO"].ToString();
                    p.id_customer = Convert.ToInt32(leerFilas["CUSTOMER"]);
                    p.customer = Convert.ToString(leerFilas["NAME"]);
                    //if (p.id_pedido == 69){
                    p.lista_estilos = obtener_estilos_pt(p.id_pedido);
                    listar.Add(p);
                    //}
                }leerFilas.Close();
            }finally { conn.CerrarConexion(); conn.Dispose(); }
            return listar;
        }
        public List<Estilos_trims> obtener_estilos_pt(int pedido){
            List<Estilos_trims> lista = new List<Estilos_trims>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT ID_PO_SUMMARY,ITEM_ID FROM PO_SUMMARY WHERE ID_PEDIDOS='"+pedido+ "'  AND ID_ESTADO!=6 AND ID_ESTADO!=7 ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Estilos_trims e = new Estilos_trims();
                    e.id_estilo = Convert.ToInt32(leer_ltd["ITEM_ID"]);
                    e.id_summary = Convert.ToInt32(leer_ltd["ID_PO_SUMMARY"]);
                    e.estilo = (consultas.obtener_estilo(e.id_estilo)).Trim();
                    e.descripcion = (consultas.buscar_descripcion_estilo(e.id_estilo)).Trim();
                    e.lista_trims = obtener_trims_summary_pt(e.id_summary);
                    lista.Add(e);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }
        public List<Trim_requests> obtener_trims_summary_pt(int summary){
            List<Trim_requests> lista = new List<Trim_requests>();          
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT tr.id_request,tr.id_po_summary,tr.id_size,tr.total,tr.id_inventario,tr.impreso FROM " +
                    " trim_requests tr,items_catalogue ic WHERE ic.item_id=tr.id_item AND tr.id_po_summary='" + summary + "' " +
                    " AND ic.fabric_type='PRICE TICKETS'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Trim_requests tr = new Trim_requests();
                    tr.id_request = Convert.ToInt32(leer["id_request"]);
                    tr.id_summary = Convert.ToInt32(leer["id_po_summary"]);
                    tr.id_talla = Convert.ToInt32(leer["id_size"]);
                    tr.talla = consultas.obtener_size_id(Convert.ToString(tr.id_talla));                               
                    tr.total = Convert.ToInt32(leer["total"]);
                    tr.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    tr.impreso = Convert.ToInt32(leer["impreso"]);
                    lista.Add(tr);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }

        public void marcar_pt_impreso(int request){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "UPDATE trim_requests SET impreso=1 WHERE id_request='" + request + "' ";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public int buscar_instruccion_empaque(int pedido){
            int temp = 0;
            Conexion con_u_i = new Conexion();
            try{
                SqlCommand com_u_i = new SqlCommand();
                SqlDataReader leer_u_i = null;
                com_u_i.Connection = con_u_i.AbrirConexion();
                com_u_i.CommandText = "SELECT id_instruccion FROM instrucciones_empaque WHERE id_pedido='"+pedido+"' ";
                leer_u_i = com_u_i.ExecuteReader();
                while (leer_u_i.Read()){
                    temp = Convert.ToInt32(leer_u_i["id_instruccion"]);
                }leer_u_i.Close();
            }finally { con_u_i.CerrarConexion(); con_u_i.Dispose(); }
            return temp;
        }

        public void agregar_instruccion_empaque(int usuario,int pedido,string instruccion){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO instrucciones_empaque(id_pedido,estado,fecha,id_usuario) VALUES " +
                    "('" + pedido + "','" + instruccion + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + usuario + "')";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public void update_instruccion_empaque(int id_instruccion,int usuario, string instruccion){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "UPDATE instrucciones_empaque SET id_usuario='" + usuario + "',estado='" + instruccion + "'" +
                    ",fecha='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE id_instruccion='" + id_instruccion + "' ";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public int buscar_estado_instruccion_empaque(int pedido){
            int temp = 0;
            Conexion con_u_i = new Conexion();
            try{
                SqlCommand com_u_i = new SqlCommand();
                SqlDataReader leer_u_i = null;
                com_u_i.Connection = con_u_i.AbrirConexion();
                com_u_i.CommandText = "SELECT estado FROM instrucciones_empaque WHERE id_pedido='" + pedido + "' ";
                leer_u_i = com_u_i.ExecuteReader();
                while (leer_u_i.Read()){
                    temp = Convert.ToInt32(leer_u_i["estado"]);
                }leer_u_i.Close();
            }finally { con_u_i.CerrarConexion(); con_u_i.Dispose(); }
            return temp;
        }

        public List<Trim_requests> obtener_listado_cajas(int pedido){
            List<Trim_requests> lista = new List<Trim_requests>();
            Conexion conn = new Conexion();
            try{
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT ID_PO_SUMMARY FROM PO_SUMMARY where ID_PEDIDOS='"+pedido+ "'  AND ID_ESTADO!=6 AND ID_ESTADO!=7  ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()){
                    List<Trim_requests> temp = obtener_cajas_summary(Convert.ToInt32(leerFilas["ID_PO_SUMMARY"]));
                    lista.AddRange(temp);                   
                }leerFilas.Close();
            }finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public List<Trim_requests> obtener_cajas_summary(int summary){
            List<Trim_requests> lista = new List<Trim_requests>();          
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT tr.id_request,tr.total,tr.id_item,tr.id_po_summary FROM " +
                    " trim_requests tr,items_catalogue ic WHERE ic.item_id=tr.id_item AND tr.id_po_summary='" + summary + "' " +
                    " AND ic.fabric_type='BOXES'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Trim_requests tr = new Trim_requests();
                    tr.id_request = Convert.ToInt32(leer["id_request"]);
                    tr.total = Convert.ToInt32(leer["total"]);
                    tr.id_summary = Convert.ToInt32(leer["id_po_summary"]);
                    tr.item = consultas.buscar_amt_item(Convert.ToString(leer["id_item"])) + " " + consultas.buscar_descripcion_item(Convert.ToString(leer["id_item"]));
                    tr.cantidad = buscar_cantidad_solicitada_anteriormente(tr.id_request);
                    lista.Add(tr);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public int buscar_cantidad_solicitada_anteriormente(int request){
            int temp = 0;
            Conexion con_u_i = new Conexion();
            try{
                SqlCommand com_u_i = new SqlCommand();
                SqlDataReader leer_u_i = null;
                com_u_i.Connection = con_u_i.AbrirConexion();
                com_u_i.CommandText = "SELECT total FROM solicitudes_cajas WHERE id_request='" + request + "'  ";
                leer_u_i = com_u_i.ExecuteReader();
                while (leer_u_i.Read()){
                    temp += Convert.ToInt32(leer_u_i["total"]);
                }leer_u_i.Close();
            }finally { con_u_i.CerrarConexion(); con_u_i.Dispose(); }
            return temp;
        }


        public void guardar_solicitud_caja(string summary, string request, string total,int usuario){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO solicitudes_cajas(id_summary,total,id_usuario,id_request,fecha) VALUES " +
                    "('" + summary + "','" + total + "','" + usuario + "','"+request+"','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }

        public List<clientes> obtener_lista_clientes(){
            List<clientes> lista = new List<clientes>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT CUSTOMER_FINAL,NAME_FINAL from CAT_CUSTOMER_PO order by NAME_FINAL  ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    clientes c = new clientes();
                    c.id_customer=Convert.ToInt32(leer["CUSTOMER_FINAL"]);
                    c.customer=Convert.ToString(leer["NAME_FINAL"]);
                    lista.Add(c);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<generos> obtener_lista_generos(){
            Conexion con = new Conexion();
            List<generos> lista = new List<generos>();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ID_GENDER,GENERO FROM CAT_GENDER ORDER BY GENERO";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    generos g = new generos();
                    g.id_genero = Convert.ToInt32(leer["ID_GENDER"]);
                    g.genero = Convert.ToString(leer["GENERO"]);
                    lista.Add(g);
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return lista;
        }
        public void guardar_nueva_imagen(string imagen,string familia,int usuario){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO trims_imagenes_archivo(imagen,id_familia,fecha,id_usuario) VALUES " +
                    "('" + imagen + "','" + familia + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','"+usuario+"')";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public int obtener_ultimo_id_imagen(){
            int temp = 0;
            Conexion con_u_i = new Conexion();
            try{
                SqlCommand com_u_i = new SqlCommand();
                SqlDataReader leer_u_i = null;
                com_u_i.Connection = con_u_i.AbrirConexion();
                com_u_i.CommandText = "SELECT TOP 1 id_imagen FROM trims_imagenes_archivo ORDER BY  id_imagen DESC  ";
                leer_u_i = com_u_i.ExecuteReader();
                while (leer_u_i.Read()){
                    temp = Convert.ToInt32(leer_u_i["id_imagen"]);
                }leer_u_i.Close();
            }finally { con_u_i.CerrarConexion(); con_u_i.Dispose(); }
            return temp;
        }
        public void guardar_imagen_genero_cliente(int imagen,string cliente,string genero){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO trims_imagenes(id_imagen,id_cliente,id_genero) VALUES ('" + imagen + "','" + cliente + "','" + genero + "')";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }

        public List<imagenes_trim> obtener_lista_imagenes_familia(int familia){
            Conexion con = new Conexion();
            List<imagenes_trim> lista = new List<imagenes_trim>();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_imagen,imagen FROM trims_imagenes_archivo WHERE id_familia='" + familia + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    imagenes_trim i = new imagenes_trim();
                    i.id_imagen = Convert.ToInt32(leer["id_imagen"]);
                    //i.imagen = Convert.ToString(leer["imagen"]);
                    i.imagen = Convert.ToString(leer["imagen"]); 
                    i.lista_datos = obtener_lista_generos_clientes_imagen(i.id_imagen);
                    lista.Add(i);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<imagen_datos> obtener_lista_generos_clientes_imagen(int imagen){
            List<imagen_datos> lista = new List<imagen_datos>();
            Conexion con_u_i = new Conexion();
            try{
                SqlCommand com_u_i = new SqlCommand();
                SqlDataReader leer_u_i = null;
                com_u_i.Connection = con_u_i.AbrirConexion();
                com_u_i.CommandText = "SELECT id_cliente,id_genero FROM trims_imagenes WHERE id_imagen='"+imagen+"'  ";
                leer_u_i = com_u_i.ExecuteReader();
                while (leer_u_i.Read()){
                    imagen_datos i = new imagen_datos();
                    i.id_genero= Convert.ToInt32(leer_u_i["id_genero"]);
                    i.id_customer= Convert.ToInt32(leer_u_i["id_cliente"]);
                    lista.Add(i);
                }leer_u_i.Close();
            }finally { con_u_i.CerrarConexion(); con_u_i.Dispose(); }
            return lista;
        }
        public string buscar_imagen(int id){
            string nombre = "";
            Conexion con_u_i = new Conexion();
            try{
                SqlCommand com_u_i = new SqlCommand();
                SqlDataReader leer_u_i = null;
                com_u_i.Connection = con_u_i.AbrirConexion();
                com_u_i.CommandText = "SELECT imagen FROM trims_imagenes_archivo WHERE id_imagen='"+id+"'  ";
                leer_u_i = com_u_i.ExecuteReader();
                while (leer_u_i.Read()){
                    nombre = Convert.ToString(leer_u_i["imagen"]);
                }leer_u_i.Close();
            }finally { con_u_i.CerrarConexion(); con_u_i.Dispose(); }
            return nombre;
        }
        public void edicion_imagen_trim(string trim, string imagen,int usuario){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "UPDATE trims_imagenes_archivo SET imagen='"+imagen+"'," +
                    " fecha='"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',id_usuario='"+ usuario + "' " +
                    " WHERE id_imagen='"+trim+"' ";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public void borrar_genero_clientes_imagen(string id){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "DELETE FROM trims_imagenes WHERE id_imagen='" + id + "' ";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }

        public List<Familias_trim_card> obtener_datos_familias_pedido(int pedido, List<int> lista_generos,int customer){
            List<Familias_trim_card> lista = new List<Familias_trim_card>();
            List<int> lista_items = obtener_lista_items_pedido(pedido);
            List<int> lista_familias_pedido = (obtener_familias_items(lista_items)).Distinct().ToList();
            List<Familias_trim_card> lista_familias_imagen = obtener_imagenes_familia_cliente_genero(lista_familias_pedido,customer,lista_generos,pedido);

           return lista_familias_imagen;          
        }
        public List<int> obtener_lista_items_pedido(int pedido){
            List<int> lista = new List<int>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT ID_PO_SUMMARY FROM PO_SUMMARY WHERE ID_PEDIDOS='"+pedido+ "' ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){                    
                    lista.AddRange(obtener_lista_items_summary(Convert.ToInt32(leer_ltd["ID_PO_SUMMARY"])));
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }
        public List<int> obtener_lista_items_summary(int summary){
            List<int> lista = new List<int>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT distinct id_item FROM trim_requests WHERE id_po_summary='" + summary + "'   ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    lista.Add(Convert.ToInt32(leer_ltd["id_item"]));
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }





        public List<int> obtener_familias_items(List<int> lista_items){
            List<int> lista = new List<int>();
            foreach (int i in lista_items){
                Conexion con_ltd = new Conexion();
                try{
                    SqlCommand com_ltd = new SqlCommand();
                    SqlDataReader leer_ltd = null;
                    com_ltd.Connection = con_ltd.AbrirConexion();
                    com_ltd.CommandText = "SELECT ft.id_family_trim FROM family_trims ft,items_catalogue ic WHERE " +
                        "  ic.item_id='" + i + "' AND ic.fabric_type=ft.family_trim ";
                    leer_ltd = com_ltd.ExecuteReader();
                    while (leer_ltd.Read()){
                        lista.Add(Convert.ToInt32(leer_ltd["id_family_trim"]));
                    }leer_ltd.Close();
                }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            }
            return lista;
        }
        public string obtener_nombre_familia(int familia){
            string temporal = "";
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT family_trim FROM family_trims WHERE id_family_trim='" + familia + "'  ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    temporal = Convert.ToString(leer_ltd["family_trim"]);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return temporal;
        }
        public List<Familias_trim_card> obtener_imagenes_familia_cliente_genero(List<int> lista_familias_pedido, int customer, List<int> lista_generos,int pedido){
            List<Familias_trim_card> lista_final = new List<Familias_trim_card>();
            foreach (int f in lista_familias_pedido){
                Familias_trim_card familia = new Familias_trim_card();
                familia.id_family_trim = f;
                familia.family_trim = obtener_nombre_familia(f);
                List<imagenes_trim> lista_temporal_imagen = new List<imagenes_trim>();
                int contador=0;
                foreach (int g in lista_generos){
                    contador = 0;
                    Conexion con_ltd = new Conexion();
                    try{
                        SqlCommand com_ltd = new SqlCommand();
                        SqlDataReader leer_ltd = null;
                        com_ltd.Connection = con_ltd.AbrirConexion();
                        com_ltd.CommandText = "SELECT ti.id_imagen,tia.imagen FROM trims_imagenes ti,trims_imagenes_archivo tia WHERE " +
                            " tia.id_familia='"+f+"' AND ti.id_imagen=tia.id_imagen AND " +
                            " ti.id_cliente='" + customer + "' AND ti.id_genero='"+g+"' ";
                        leer_ltd = com_ltd.ExecuteReader();
                        while (leer_ltd.Read()){
                            imagenes_trim img = new imagenes_trim();
                            img.id_imagen = Convert.ToInt32(leer_ltd["id_imagen"]);
                            img.imagen = Convert.ToString(leer_ltd["imagen"]);
                            lista_temporal_imagen.Add(img);
                            contador++;
                        }
                        leer_ltd.Close();
                    }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }                    
                }
                if (contador == 0){
                    familia.lista_imagenes = obtener_imagenes_familia_cliente(f,customer);
                }else {
                    familia.lista_imagenes = lista_temporal_imagen;
                }
                familia.item = obtener_item_pedido_familia(familia.id_family_trim,pedido);
                lista_final.Add(familia);
            }
            return lista_final;
        }
        public string obtener_item_pedido_familia(int familia,int pedido){
            string temporal = "";
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = " select ic.descripcion from trim_requests tr, items_catalogue ic,PO_SUMMARY ps,family_trims ft where ps.ID_PEDIDOS='" + pedido + "' " +
                    " and ps.ID_PO_SUMMARY=tr.id_po_summary and ic.item_id=tr.id_item and ft.family_trim=ic.fabric_type and ft.id_family_trim='"+familia+"'" +
                    "  AND ps.ID_ESTADO!=6 AND ps.ID_ESTADO!=7  ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    temporal = Convert.ToString(leer_ltd["descripcion"]);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return temporal;
        }
        public List<imagenes_trim> obtener_imagenes_familia_cliente(int familia, int customer){            
            List<imagenes_trim> lista_temporal_imagen = new List<imagenes_trim>();
            int contador = 0;
            Conexion con_ltd = new Conexion();
                try{
                contador = 0;
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT ti.id_imagen,tia.imagen FROM trims_imagenes ti,trims_imagenes_archivo tia WHERE " +
                            " tia.id_familia='" + familia + "' AND ti.id_imagen=tia.id_imagen AND ti.id_cliente='" + customer + "' ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    imagenes_trim img = new imagenes_trim();
                    img.id_imagen = Convert.ToInt32(leer_ltd["id_imagen"]);
                    img.imagen = Convert.ToString(leer_ltd["imagen"]);
                    lista_temporal_imagen.Add(img);
                    contador++;
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            if (contador == 0){
                return obtener_imagenes_familia(familia);
            }else{
                return lista_temporal_imagen;
            }            
        }
        public List<imagenes_trim> obtener_imagenes_familia(int familia){
            List<imagenes_trim> lista_temporal_imagen = new List<imagenes_trim>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT ti.id_imagen,tia.imagen FROM trims_imagenes ti,trims_imagenes_archivo tia WHERE " +
                        " tia.id_familia='" + familia + "' AND ti.id_imagen=tia.id_imagen ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    imagenes_trim img = new imagenes_trim();
                    img.id_imagen = Convert.ToInt32(leer_ltd["id_imagen"]);
                    img.imagen = Convert.ToString(leer_ltd["imagen"]);
                    lista_temporal_imagen.Add(img);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista_temporal_imagen;
        }
        public void guardar_nuevo_trim_card(string pedido, string customer,  string tipo_empaque, string ratio,int usuario,string generos){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO trim_card(id_pedido,ratio,tipo_empaque,id_usuario,id_customer,generos,fecha) VALUES " +
                "('" + pedido + "','" + ratio + "','" + tipo_empaque + "','" + usuario + "','"+customer+"','" + generos + "','" +
                "" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public int obtener_ultimo_trim_card(){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT TOP 1 id_trim_card FROM trim_card  ORDER BY id_trim_card DESC ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["id_trim_card"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }
        public void guardar_trim_card_familia(int id_trim_card, string imagen, string especial, string notas, string familia,string item){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO trim_card_spec(id_trim_card,id_imagen,especial,notas,id_familia,item) VALUES " +
                "('" + id_trim_card + "','" + imagen + "','" + especial + "','" + notas + "','" + familia + "','"+item+"')";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }

        public List<Pedidos_trim_card> obtener_lista_trim_cards(string busqueda){
            List<Pedidos_trim_card> lista = new List<Pedidos_trim_card>();
            string query = "";
            if (busqueda == "0") {
                query = "SELECT TOP 50 id_trim_card,id_pedido,id_customer,id_usuario,fecha FROM trim_card ORDER BY id_trim_card DESC";
            } else {
                query = "SELECT TOP 50 tc.id_trim_card,tc.id_pedido,tc.id_customer,tc.id_usuario,tc.fecha FROM trim_card tc, PEDIDO p " +
                    " WHERE tc.id_pedido=p.ID_PEDIDO AND p.PO like'%"+busqueda+ "%' ORDER BY tc.id_trim_card DESC";
            }
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = query;
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Pedidos_trim_card p = new Pedidos_trim_card();
                    p.id_trim_card = Convert.ToInt32(leer_ltd["id_trim_card"]);
                    p.pedido = consultas.obtener_po_id(Convert.ToString(leer_ltd["id_pedido"]));
                    p.customer = consultas.obtener_customer_final_id(Convert.ToString(leer_ltd["id_customer"]));
                    p.usuario = consultas.buscar_nombre_usuario(Convert.ToString(leer_ltd["id_usuario"]));
                    p.fecha= (Convert.ToDateTime(leer_ltd["fecha"])).ToString("MMM dd yyyy");
                    lista.Add(p);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }
        
        public List<Pedidos_trim_card> obtener_trim_card(int id){
            List<Pedidos_trim_card> lista = new List<Pedidos_trim_card>();          
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT id_trim_card,id_pedido,id_customer,id_usuario,fecha,generos,ratio,tipo_empaque FROM trim_card WHERE id_trim_card='" + id+"' ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Pedidos_trim_card p = new Pedidos_trim_card();
                    p.id_pedido = Convert.ToInt32(leer_ltd["id_pedido"]);
                    p.id_trim_card = Convert.ToInt32(leer_ltd["id_trim_card"]);
                    p.pedido = consultas.obtener_po_id(Convert.ToString(leer_ltd["id_pedido"]));
                    p.customer = consultas.obtener_customer_final_id(Convert.ToString(leer_ltd["id_customer"]));
                    p.usuario = consultas.buscar_nombre_usuario(Convert.ToString(leer_ltd["id_usuario"]));
                    p.fecha = (Convert.ToDateTime(leer_ltd["fecha"])).ToString("MMM dd yyyy");
                    p.gender = Convert.ToString(leer_ltd["generos"]);
                    p.ratio =Convert.ToString(leer_ltd["ratio"]);
                    p.fold_size = buscar_fold_size_trim_card(id);
                    p.tipo_empaque =Convert.ToInt32(leer_ltd["tipo_empaque"]);
                    p.lista_estilos = lista_estilos_dropdown(Convert.ToString(Convert.ToInt32(leer_ltd["id_pedido"])));
                    p.lista_familias = obtener_datos_trim_card(p.id_trim_card);
                    p.lista_generos = obtener_generos(p.gender);
                    lista.Add(p);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }
        public List<Familias_trim_card> obtener_datos_trim_card(int id){
            List<Familias_trim_card> lista= new List<Familias_trim_card>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT id_familia,id_imagen,especial,notas,item FROM trim_card_spec WHERE id_trim_Card='"+id+"' ";                    
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Familias_trim_card f = new Familias_trim_card();
                    f.id_family_trim = Convert.ToInt32(leer_ltd["id_familia"]);
                    f.family_trim = obtener_nombre_familia(f.id_family_trim);
                    f.id_imagen = Convert.ToInt32(leer_ltd["id_imagen"]);
                    f.imagen = obtener_imagen(f.id_imagen);
                    f.id_especial = Convert.ToInt32(leer_ltd["especial"]);
                    f.notas = Convert.ToString(leer_ltd["notas"]);
                    f.item = Convert.ToString(leer_ltd["item"]);
                    f.especial = obtener_especial(f.id_especial);
                    lista.Add(f);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }
        public string obtener_especial(int id){
            string temporal = "";
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT especial FROM trims_especiales WHERE id_especial='" + id + "'  ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    temporal = Convert.ToString(leer_ltd["especial"]);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return temporal;
        }
        public string obtener_imagen(int id){
            string temporal = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT imagen FROM trims_imagenes_archivo WHERE id_imagen='" + id + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temporal = Convert.ToString(leer["imagen"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return temporal;
        }
        public List<generos> obtener_generos(string genero){
            string[] generos = genero.Split('*');
            List<generos> lista = new List<generos>();
            for (int i =1; i<generos.Length; i++){
                Conexion con_ltd = new Conexion();
                try{
                    SqlCommand com_ltd = new SqlCommand();
                    SqlDataReader leer_ltd = null;
                    com_ltd.Connection = con_ltd.AbrirConexion();
                    com_ltd.CommandText = "SELECT ID_GENDER,GENERO FROM CAT_GENDER WHERE ID_GENDER='" + generos[i] + "' ";
                    leer_ltd = com_ltd.ExecuteReader();
                    while (leer_ltd.Read()){
                        generos g = new generos();
                        g.id_genero = Convert.ToInt32(leer_ltd["ID_GENDER"]);
                        g.genero = Convert.ToString(leer_ltd["GENERO"]);
                        lista.Add(g);
                    }leer_ltd.Close();
                }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            }
            return lista;
        }
        public int buscar_pedido_trim_card(int trim_card){
            int temp = 0;
            Conexion con_u_i = new Conexion();
            try{
                SqlCommand com_u_i = new SqlCommand();
                SqlDataReader leer_u_i = null;
                com_u_i.Connection = con_u_i.AbrirConexion();
                com_u_i.CommandText = "SELECT id_pedido FROM trim_card WHERE id_trim_card='" + trim_card + "' ";
                leer_u_i = com_u_i.ExecuteReader();
                while (leer_u_i.Read()){
                    temp = Convert.ToInt32(leer_u_i["id_pedido"]);
                }leer_u_i.Close();
            }finally { con_u_i.CerrarConexion(); con_u_i.Dispose(); }
            return temp;
        }
        public string buscar_fold_size_trim_card(int trim_card){
            string temp = "";
            Conexion con_u_i = new Conexion();
            try{
                SqlCommand com_u_i = new SqlCommand();
                SqlDataReader leer_u_i = null;
                com_u_i.Connection = con_u_i.AbrirConexion();
                com_u_i.CommandText = "SELECT fold_size FROM trims_fold_sizes WHERE id_trim_card='" + trim_card + "' ";
                leer_u_i = com_u_i.ExecuteReader();
                while (leer_u_i.Read()){
                    temp = Convert.ToString(leer_u_i["fold_size"]);
                }leer_u_i.Close();
            }finally { con_u_i.CerrarConexion(); con_u_i.Dispose(); }
            return temp;
        }
        public void editar_trim_card(int trim_card, string tipo_empaque, string ratio){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "UPDATE trim_card SET tipo_empaque='" + tipo_empaque + "',ratio='" + ratio + "', " +
                    "fecha='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                    " WHERE id_trim_card='" + trim_card + "' ";
                com_c.ExecuteNonQuery();}
            finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public void borrar_trim_card_spec(int trim_card){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "Delete from trim_card_spec WHERE id_trim_card='" + trim_card + "' ";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public void ingresar_fold_size(int pedido, string fold_size) {
            int existe = buscar_fold_size(pedido);
            if (existe != 0){
                actualizar_fold_size(existe, fold_size);
            }else {
                insertar_fold_size(pedido,fold_size);
            }
        }
        public int buscar_fold_size(int pedido){
            int temp = 0;
            Conexion con_u_i = new Conexion();
            try{
                SqlCommand com_u_i = new SqlCommand();
                SqlDataReader leer_u_i = null;
                com_u_i.Connection = con_u_i.AbrirConexion();
                com_u_i.CommandText = "SELECT id_fold_size FROM trims_fold_sizes WHERE id_pedido='" + pedido + "' ";
                leer_u_i = com_u_i.ExecuteReader();
                while (leer_u_i.Read()){
                    temp = Convert.ToInt32(leer_u_i["id_fold_size"]);
                }leer_u_i.Close();
            }finally { con_u_i.CerrarConexion(); con_u_i.Dispose(); }
            return temp;
        }
        public void actualizar_fold_size(int id_fold_size,string fold_size){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "UPDATE trims_fold_sizes SET fold_size='" + fold_size + "' WHERE id_fold_size='"+ id_fold_size + "' ";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public void insertar_fold_size(int pedido,string fold_size){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO trims_fold_sizes(id_pedido,id_trim_card,fold_size)VALUES" +
                    "('"+ pedido + "','0','"+ fold_size + "')  ";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public void agregar_trim_card_fold_size(string pedido, int trim_card){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "UPDATE trims_fold_sizes SET id_trim_card='" + trim_card + "' WHERE id_pedido='" + pedido + "' ";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public string buscar_fold_size_pedido(int pedido){
            string temp = "";
            Conexion con_u_i = new Conexion();
            try{
                SqlCommand com_u_i = new SqlCommand();
                SqlDataReader leer_u_i = null;
                com_u_i.Connection = con_u_i.AbrirConexion();
                com_u_i.CommandText = "SELECT fold_size FROM trims_fold_sizes WHERE id_pedido='" + pedido + "' ";
                leer_u_i = com_u_i.ExecuteReader();
                while (leer_u_i.Read()){
                    temp = Convert.ToString(leer_u_i["fold_size"]);
                }leer_u_i.Close();
            }finally { con_u_i.CerrarConexion(); con_u_i.Dispose(); }
            return temp;
        }
        public List<Item_t> busqueda_items_trims(string busqueda){
            string query = "";
            if (busqueda != "0"){
                query = "SELECT top 30  item_id,item,descripcion,fabric_type FROM items_catalogue WHERE tipo=2 and " +
                    " descripcion like '%"+busqueda+ "%' or item  like '%" + busqueda + "%'  ";
            }else {
                query = "SELECT top 30  item_id,item,descripcion,fabric_type FROM items_catalogue WHERE tipo=2  ";
            }
            List<Item_t> lista = new List<Item_t>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = query;
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Item_t i = new Item_t();
                    i.id_item = Convert.ToInt32(leer_ltd["item_id"]);
                    i.componente = Convert.ToString(leer_ltd["item"]);
                    i.descripcion = Convert.ToString(leer_ltd["descripcion"]);
                    i.familia = Convert.ToString(leer_ltd["fabric_type"]);
                    lista.Add(i);
                }
                leer_ltd.Close();
            }
            finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }
        public List<clientes> lista_clientes(){
            List<clientes> lista = new List<clientes>();
            Conexion conn = new Conexion();
            try{
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT CUSTOMER,NAME from CAT_CUSTOMER  ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()){
                    clientes c = new clientes();
                    c.id_customer = Convert.ToInt32(leerFilas["CUSTOMER"]);
                    c.customer = leerFilas["NAME"].ToString();
                    lista.Add(c);
                }leerFilas.Close();
            }finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public List<Sucursal_trims> lista_sucursales(){
            List<Sucursal_trims> lista = new List<Sucursal_trims>();
            Conexion conn = new Conexion();
            try{
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT id_sucursal,sucursal FROM sucursales  ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()){
                    Sucursal_trims s = new Sucursal_trims();
                    s.id_sucursal= Convert.ToInt32(leerFilas["id_sucursal"]);
                    s.sucursal = leerFilas["sucursal"].ToString();
                    lista.Add(s);
                }leerFilas.Close();
            }finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public List<recibo> obtener_lista_recibos_filtro_trim(string busqueda){
            List<recibo> lista = new List<recibo>();
            if (busqueda != "")            {
                lista.AddRange(obtener_lista_recibos_filtro_millpo(busqueda));
                lista.AddRange(obtener_lista_recibos_filtro_po_reference(busqueda));
                lista.AddRange(obtener_lista_recibos_filtro_po(busqueda));
                lista.AddRange(obtener_lista_recibos_filtro_sucursal(busqueda));
                lista.AddRange(obtener_lista_recibos_filtro_cliente(busqueda));
                lista.AddRange(obtener_lista_recibos_filtro_item(busqueda));
            }else{
                Conexion con_ltf = new Conexion();
                try{
                    SqlCommand com_ltf = new SqlCommand();
                    SqlDataReader leer_ltf = null;
                    com_ltf.Connection = con_ltf.AbrirConexion();
                    com_ltf.CommandText = "SELECT top 30 r.id_recibo,r.fecha,r.total,r.mp_number,r.mill_po,r.po_reference, u.Nombres,u.Apellidos, " +
                            "s.sucursal FROM recibos r,Usuarios u,sucursales s WHERE r.id_usuario=u.Id and r.id_sucursal=s.id_sucursal order by r.id_recibo desc ";
                    leer_ltf = com_ltf.ExecuteReader();
                    while (leer_ltf.Read()){
                        int trim = buscar_trims_recibo(Convert.ToInt32(leer_ltf["id_recibo"]));
                        if (trim != 0)
                        {
                            recibo l = new recibo();
                            l.id_recibo = Convert.ToInt32(leer_ltf["id_recibo"]);
                            l.fecha = (Convert.ToDateTime(leer_ltf["fecha"])).ToString("MMM dd yyyy");
                            l.usuario = leer_ltf["Nombres"].ToString() + " " + leer_ltf["apellidos"].ToString();
                            l.total = Convert.ToInt32(leer_ltf["total"]);
                            l.sucursal = leer_ltf["sucursal"].ToString();
                            l.lista_recibos_item = obtener_lista_items_trims(l.id_recibo);
                            l.mp_number = leer_ltf["mp_number"].ToString();
                            l.mill_po = leer_ltf["mill_po"].ToString();
                            l.po_referencia = leer_ltf["po_reference"].ToString();
                            lista.Add(l);
                        }
                    }leer_ltf.Close();
                }finally { con_ltf.CerrarConexion(); con_ltf.Dispose(); }
            }
            List<recibo> lista_final = new List<recibo>();
            foreach (recibo r in lista) {
                int existencia = 0;
                foreach (recibo rr in lista_final){
                    if (rr.id_recibo == r.id_recibo) {
                        existencia++;
                    }
                }
                if (existencia == 0) {
                    recibo rec = new recibo();
                    rec = r;
                    lista_final.Add(rec);
                }
            }
            return lista_final;
        }
        public List<recibo> obtener_lista_recibos_filtro_millpo(string busqueda){
            List<recibo> lista = new List<recibo>();           
            Conexion con_ltf = new Conexion();
            try{
                SqlCommand com_ltf = new SqlCommand();
                SqlDataReader leer_ltf = null;
                com_ltf.Connection = con_ltf.AbrirConexion();
                com_ltf.CommandText = "SELECT top 50 r.id_recibo,r.fecha,r.total,r.mp_number,r.mill_po,r.po_reference, u.Nombres,u.Apellidos, " +
                "s.sucursal from recibos r,Usuarios u,sucursales s WHERE r.id_usuario=u.Id and r.id_sucursal=s.id_sucursal  " +
                " AND r.mill_po like'%" + busqueda + "%' ";
                leer_ltf = com_ltf.ExecuteReader();
                while (leer_ltf.Read()){
                    int trim = buscar_trims_recibo(Convert.ToInt32(leer_ltf["id_recibo"]));
                    if (trim != 0){
                         recibo l = new recibo();
                        l.id_recibo = Convert.ToInt32(leer_ltf["id_recibo"]);
                        l.fecha = (Convert.ToDateTime(leer_ltf["fecha"])).ToString("MMM dd yyyy");
                        l.usuario = leer_ltf["Nombres"].ToString() + " " + leer_ltf["apellidos"].ToString();
                        l.total = Convert.ToInt32(leer_ltf["total"]);
                        l.sucursal = leer_ltf["sucursal"].ToString();
                        l.lista_recibos_item = obtener_lista_items_trims(l.id_recibo);
                        l.mp_number = leer_ltf["mp_number"].ToString();
                        l.mill_po = leer_ltf["mill_po"].ToString();
                        l.po_referencia = leer_ltf["po_reference"].ToString();
                        lista.Add(l);
                    }
                }leer_ltf.Close();
            }finally { con_ltf.CerrarConexion(); con_ltf.Dispose(); }
            return lista;
        }
        public List<recibo> obtener_lista_recibos_filtro_po_reference(string busqueda){
            List<recibo> lista = new List<recibo>();           
            Conexion con_ltf = new Conexion();
            try{
                SqlCommand com_ltf = new SqlCommand();
                SqlDataReader leer_ltf = null;
                com_ltf.Connection = con_ltf.AbrirConexion();
                com_ltf.CommandText = "SELECT top 50 r.id_recibo,r.fecha,r.total,r.mp_number,r.mill_po,r.po_reference, u.Nombres,u.Apellidos, " +
                "s.sucursal from recibos r,Usuarios u,sucursales s WHERE r.id_usuario=u.Id and r.id_sucursal=s.id_sucursal  " +
                " AND r.po_reference like'%" + busqueda + "%' ";
                leer_ltf = com_ltf.ExecuteReader();
                while (leer_ltf.Read()){
                    int trim = buscar_trims_recibo(Convert.ToInt32(leer_ltf["id_recibo"]));
                    if (trim != 0){
                        recibo l = new recibo();
                        l.id_recibo = Convert.ToInt32(leer_ltf["id_recibo"]);
                        l.fecha = (Convert.ToDateTime(leer_ltf["fecha"])).ToString("MMM dd yyyy");
                        l.usuario = leer_ltf["Nombres"].ToString() + " " + leer_ltf["apellidos"].ToString();
                        l.total = Convert.ToInt32(leer_ltf["total"]);
                        l.sucursal = leer_ltf["sucursal"].ToString();
                        l.lista_recibos_item = obtener_lista_items_trims(l.id_recibo);
                        l.mp_number = leer_ltf["mp_number"].ToString();
                        l.mill_po = leer_ltf["mill_po"].ToString();
                        l.po_referencia = leer_ltf["po_reference"].ToString();
                        lista.Add(l);
                    }
                }leer_ltf.Close();
            }finally { con_ltf.CerrarConexion(); con_ltf.Dispose(); }
            return lista;
        }
        public List<recibo> obtener_lista_recibos_filtro_po(string busqueda){
            List<recibo> lista = new List<recibo>();
            Conexion con_ltf = new Conexion();
            try{
                SqlCommand com_ltf = new SqlCommand();
                SqlDataReader leer_ltf = null;
                com_ltf.Connection = con_ltf.AbrirConexion();
                com_ltf.CommandText = "SELECT top 50 r.id_recibo,r.fecha,r.total,r.mp_number,r.mill_po,r.po_reference, u.Nombres,u.Apellidos, " +
                    "s.sucursal from recibos r,Usuarios u,sucursales s,PEDIDO p,recibos_items ri,inventario i WHERE r.id_usuario=u.Id and r.id_sucursal=s.id_sucursal  " +
                    " AND p.PO like'%" + busqueda + "%' AND p.ID_PEDIDO=i.id_pedido AND i.id_inventario=ri.id_inventario AND ri.id_recibo=r.id_recibo";
                leer_ltf = com_ltf.ExecuteReader();
                while (leer_ltf.Read()){
                    int trim = buscar_trims_recibo(Convert.ToInt32(leer_ltf["id_recibo"]));
                    if (trim != 0){
                         recibo l = new recibo();
                        l.id_recibo = Convert.ToInt32(leer_ltf["id_recibo"]);
                        l.fecha = (Convert.ToDateTime(leer_ltf["fecha"])).ToString("MMM dd yyyy");
                        l.usuario = leer_ltf["Nombres"].ToString() + " " + leer_ltf["apellidos"].ToString();
                        l.total = Convert.ToInt32(leer_ltf["total"]);
                        l.sucursal = leer_ltf["sucursal"].ToString();
                        l.lista_recibos_item = obtener_lista_items_trims(l.id_recibo);
                        l.mp_number = leer_ltf["mp_number"].ToString();
                        l.mill_po = leer_ltf["mill_po"].ToString();
                        l.po_referencia = leer_ltf["po_reference"].ToString();
                        lista.Add(l);
                    }
                }leer_ltf.Close();
            }finally { con_ltf.CerrarConexion(); con_ltf.Dispose(); }
            return lista;
        }
        public List<recibo> obtener_lista_recibos_filtro_sucursal(string busqueda){
            List<recibo> lista = new List<recibo>();
            Conexion con_ltf = new Conexion();
            try{
                SqlCommand com_ltf = new SqlCommand();
                SqlDataReader leer_ltf = null;
                com_ltf.Connection = con_ltf.AbrirConexion();
                com_ltf.CommandText = "SELECT top 50 r.id_recibo,r.fecha,r.total,r.mp_number,r.mill_po,r.po_reference, u.Nombres,u.Apellidos, " +
                    "s.sucursal from recibos r,Usuarios u,sucursales s WHERE r.id_usuario=u.Id and r.id_sucursal=s.id_sucursal  " +
                    " AND s.sucursal like'%" + busqueda + "%' ";
                leer_ltf = com_ltf.ExecuteReader();
                while (leer_ltf.Read()){
                    int trim = buscar_trims_recibo(Convert.ToInt32(leer_ltf["id_recibo"]));
                    if (trim != 0){
                        recibo l = new recibo();
                        l.id_recibo = Convert.ToInt32(leer_ltf["id_recibo"]);
                        l.fecha = (Convert.ToDateTime(leer_ltf["fecha"])).ToString("MMM dd yyyy");
                        l.usuario = leer_ltf["Nombres"].ToString() + " " + leer_ltf["apellidos"].ToString();
                        l.total = Convert.ToInt32(leer_ltf["total"]);
                        l.sucursal = leer_ltf["sucursal"].ToString();
                        l.lista_recibos_item = obtener_lista_items_trims(l.id_recibo);
                        l.mp_number = leer_ltf["mp_number"].ToString();
                        l.mill_po = leer_ltf["mill_po"].ToString();
                        l.po_referencia = leer_ltf["po_reference"].ToString();
                        lista.Add(l);
                    }
                }leer_ltf.Close();
            }finally { con_ltf.CerrarConexion(); con_ltf.Dispose(); }
            return lista;
        }
        public List<recibo> obtener_lista_recibos_filtro_cliente(string busqueda){
            List<recibo> lista = new List<recibo>();
            Conexion con_ltf = new Conexion();
            try{
                SqlCommand com_ltf = new SqlCommand();
                SqlDataReader leer_ltf = null;
                com_ltf.Connection = con_ltf.AbrirConexion();
                com_ltf.CommandText = "SELECT distinct r.id_recibo,r.fecha,r.total,r.mp_number,r.mill_po,r.po_reference, u.Nombres,u.Apellidos, " +
                    "s.sucursal from recibos r,Usuarios u,sucursales s,recibos_items ri,inventario i,CAT_CUSTOMER cc WHERE r.id_usuario=u.Id and r.id_sucursal=s.id_sucursal  " +
                    " AND ri.id_recibo=r.id_recibo AND ri.id_inventario=i.id_inventario AND i.id_customer=cc.CUSTOMER  " +
                    " AND cc.NAME like'%" + busqueda + "%' ";
                leer_ltf = com_ltf.ExecuteReader();
                while (leer_ltf.Read()){
                    int trim = buscar_trims_recibo(Convert.ToInt32(leer_ltf["id_recibo"]));
                    if (trim != 0){
                        recibo l = new recibo();
                        l.id_recibo = Convert.ToInt32(leer_ltf["id_recibo"]);
                        l.fecha = (Convert.ToDateTime(leer_ltf["fecha"])).ToString("MMM dd yyyy");
                        l.usuario = leer_ltf["Nombres"].ToString() + " " + leer_ltf["apellidos"].ToString();
                        l.total = Convert.ToInt32(leer_ltf["total"]);
                        l.sucursal = leer_ltf["sucursal"].ToString();
                        l.lista_recibos_item = obtener_lista_items_trims(l.id_recibo);
                        l.mp_number = leer_ltf["mp_number"].ToString();
                        l.mill_po = leer_ltf["mill_po"].ToString();
                        l.po_referencia = leer_ltf["po_reference"].ToString();
                        lista.Add(l);
                    }
                }leer_ltf.Close();
            }finally { con_ltf.CerrarConexion(); con_ltf.Dispose(); }
            return lista;
        }
        public List<recibo> obtener_lista_recibos_filtro_item(string busqueda){
            List<recibo> lista = new List<recibo>();
            Conexion con_ltf = new Conexion();
            try{
                SqlCommand com_ltf = new SqlCommand();
                SqlDataReader leer_ltf = null;
                com_ltf.Connection = con_ltf.AbrirConexion();
                com_ltf.CommandText = "SELECT distinct r.id_recibo,r.fecha,r.total,r.mp_number,r.mill_po,r.po_reference, u.Nombres,u.Apellidos, " +
                    "s.sucursal from recibos r,Usuarios u,sucursales s,recibos_items ri,inventario i,CAT_CUSTOMER cc WHERE r.id_usuario=u.Id and r.id_sucursal=s.id_sucursal  " +
                    " AND ri.id_recibo=r.id_recibo AND ri.id_inventario=i.id_inventario AND i.descripcion like'%" + busqueda + "%' ";
                leer_ltf = com_ltf.ExecuteReader();
                while (leer_ltf.Read()){
                    int trim = buscar_trims_recibo(Convert.ToInt32(leer_ltf["id_recibo"]));
                    if (trim != 0){
                        recibo l = new recibo();
                        l.id_recibo = Convert.ToInt32(leer_ltf["id_recibo"]);
                        l.fecha = (Convert.ToDateTime(leer_ltf["fecha"])).ToString("MMM dd yyyy");
                        l.usuario = leer_ltf["Nombres"].ToString() + " " + leer_ltf["apellidos"].ToString();
                        l.total = Convert.ToInt32(leer_ltf["total"]);
                        l.sucursal = leer_ltf["sucursal"].ToString();
                        l.lista_recibos_item = obtener_lista_items_trims(l.id_recibo);
                        l.mp_number = leer_ltf["mp_number"].ToString();
                        l.mill_po = leer_ltf["mill_po"].ToString();
                        l.po_referencia = leer_ltf["po_reference"].ToString();
                        lista.Add(l);
                    }
                }leer_ltf.Close();
            }finally { con_ltf.CerrarConexion(); con_ltf.Dispose(); }
            return lista;
        }
        public List<Trim_requests> obtener_trims_customer(string summary){
            List<Trim_requests> lista = new List<Trim_requests>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT distinct id_item,impreso FROM trim_requests where id_po_summary='" + summary + "'";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Trim_requests tr = new Trim_requests();
                    tr.id_item = Convert.ToInt32(leer_ltd["id_item"]);
                    tr.item = consultas.buscar_amt_item(Convert.ToString(tr.id_item)) + " " + consultas.buscar_descripcion_item(Convert.ToString(tr.id_item));
                    tr.total = buscar_total_item_trim(summary,tr.id_item);
                    tr.restante = buscar_restante_item_trim(summary,tr.id_item);
                    tr.impreso= Convert.ToInt32(leer_ltd["impreso"]);
                    tr.familia = buscar_familia_item(tr.id_item);
                    lista.Add(tr);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }
        public int buscar_total_item_trim(string summary,int item){
            int temp = 0;
            Conexion con_u_i = new Conexion();
            try{
                SqlCommand com_u_i = new SqlCommand();
                SqlDataReader leer_u_i = null;
                com_u_i.Connection = con_u_i.AbrirConexion();
                com_u_i.CommandText = "SELECT total FROM trim_requests WHERE id_po_summary='" + summary + "' AND id_item='"+item+"' ";
                leer_u_i = com_u_i.ExecuteReader();
                while (leer_u_i.Read()){
                    temp += Convert.ToInt32(leer_u_i["total"]);
                }leer_u_i.Close();
            }finally { con_u_i.CerrarConexion(); con_u_i.Dispose(); }
            return temp;
        }
        public int buscar_restante_item_trim(string summary, int item){
            int temp = 0;
            Conexion con_u_i = new Conexion();
            try{
                SqlCommand com_u_i = new SqlCommand();
                SqlDataReader leer_u_i = null;
                com_u_i.Connection = con_u_i.AbrirConexion();
                com_u_i.CommandText = "SELECT restante FROM trim_requests WHERE id_po_summary='" + summary + "' AND id_item='" + item + "' ";
                leer_u_i = com_u_i.ExecuteReader();
                while (leer_u_i.Read()){
                    temp += Convert.ToInt32(leer_u_i["restante"]);
                }leer_u_i.Close();
            }finally { con_u_i.CerrarConexion(); con_u_i.Dispose(); }
            return temp;
        }
        public int buscar_familia_item(int item){
            int temp = 0;
            Conexion con_u_i = new Conexion();
            try{
                SqlCommand com_u_i = new SqlCommand();
                SqlDataReader leer_u_i = null;
                com_u_i.Connection = con_u_i.AbrirConexion();
                com_u_i.CommandText = "SELECT f.id_family_trim FROM family_trims f,items_catalogue i WHERE " +
                    " i.fabric_type=f.family_trim AND i.item_id='" + item + "' ";
                leer_u_i = com_u_i.ExecuteReader();
                while (leer_u_i.Read()){
                    temp += Convert.ToInt32(leer_u_i["id_family_trim"]);
                }leer_u_i.Close();
            }finally { con_u_i.CerrarConexion(); con_u_i.Dispose(); }
            return temp;
        }


        public List<Inventario> obtener_lista_trims_inventario(string busqueda){
            List<Inventario> lista = new List<Inventario>();
            if (busqueda != "0"){
                lista.AddRange(obtener_lista_trims_inventario_descripcion(busqueda));
                lista.AddRange(obtener_lista_trims_inventario_cliente(busqueda));
                lista.AddRange(obtener_lista_trims_inventario_familia(busqueda));
                lista.AddRange(obtener_lista_trims_inventario_po(busqueda));
                lista.AddRange(obtener_lista_trims_inventario_estilo(busqueda));
            }else{
                Conexion con = new Conexion();
                try{
                    SqlCommand com = new SqlCommand();
                    SqlDataReader leer = null;
                    com.Connection = con.AbrirConexion();
                    com.CommandText = "SELECT TOP 50 id_inventario,id_pedido,total,id_customer,id_family_trim,descripcion,id_estilo FROM inventario " +
                        " WHERE id_categoria_inventario=2 ";
                    leer = com.ExecuteReader();
                    while (leer.Read()){
                        Inventario i = new Inventario();
                        i.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                        i.id_pedido = Convert.ToInt32(leer["id_pedido"]);
                        i.po = consultas.obtener_po_id(Convert.ToString(leer["id_pedido"]));
                        i.id_estilo = Convert.ToInt32(leer["id_estilo"]);
                        i.estilo = consultas.obtener_estilo(Convert.ToInt32(leer["id_estilo"])) + " " + consultas.buscar_descripcion_estilo(Convert.ToInt32(leer["id_estilo"]));
                        i.total = Convert.ToInt32(leer["total"]);
                        i.descripcion = Convert.ToString(leer["descripcion"]);
                        i.id_customer = Convert.ToInt32(leer["id_customer"]);
                        i.customer = consultas.obtener_customer_id(Convert.ToString(i.id_customer));
                        i.family_trim = consultas.obtener_family_id(Convert.ToString(leer["id_family_trim"]));
                        lista.Add(i);
                    }leer.Close();
                }finally { con.CerrarConexion(); con.Dispose(); }
            }
            List<Inventario> lista_final = new List<Inventario>();
            foreach (Inventario r in lista){
                int existencia = 0;
                foreach (Inventario rr in lista_final){
                    if (rr.id_inventario == r.id_inventario){
                        existencia++;
                    }
                }
                if (existencia == 0){
                    Inventario rec = new Inventario();
                    rec = r;
                    lista_final.Add(rec);
                }
            }
            return lista_final;
        }
        public List<Inventario> obtener_lista_trims_inventario_descripcion(string busqueda){
            List<Inventario> lista = new List<Inventario>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT TOP 50 id_inventario,id_pedido,total,id_customer,id_family_trim,descripcion,id_estilo FROM inventario " +
                    " WHERE id_categoria_inventario=2 AND descripcion like'%"+busqueda+"%' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Inventario i = new Inventario();
                    i.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    i.id_pedido = Convert.ToInt32(leer["id_pedido"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer["id_pedido"]));
                    i.id_estilo = Convert.ToInt32(leer["id_estilo"]);
                    i.estilo = consultas.obtener_estilo(Convert.ToInt32(leer["id_estilo"])) + " " + consultas.buscar_descripcion_estilo(Convert.ToInt32(leer["id_estilo"]));
                    i.total = Convert.ToInt32(leer["total"]);
                    i.descripcion = Convert.ToString(leer["descripcion"]);
                    i.id_customer = Convert.ToInt32(leer["id_customer"]);
                    i.customer = consultas.obtener_customer_id(Convert.ToString(i.id_customer));
                    i.family_trim = consultas.obtener_family_id(Convert.ToString(leer["id_family_trim"]));
                    lista.Add(i);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<Inventario> obtener_lista_trims_inventario_cliente(string busqueda){
            List<Inventario> lista = new List<Inventario>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT TOP 50 i.id_inventario,i.id_pedido,i.total,i.id_customer,i.id_family_trim,i.descripcion,i.id_estilo " +
                    " FROM inventario i, CAT_CUSTOMER cc WHERE cc.CUSTOMER=i.id_customer AND " +
                    "  id_categoria_inventario=2 AND cc.NAME like'%" + busqueda + "%' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Inventario i = new Inventario();
                    i.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    i.id_pedido = Convert.ToInt32(leer["id_pedido"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer["id_pedido"]));
                    i.id_estilo = Convert.ToInt32(leer["id_estilo"]);
                    i.estilo = consultas.obtener_estilo(Convert.ToInt32(leer["id_estilo"])) + " " + consultas.buscar_descripcion_estilo(Convert.ToInt32(leer["id_estilo"]));
                    i.total = Convert.ToInt32(leer["total"]);
                    i.descripcion = Convert.ToString(leer["descripcion"]);
                    i.id_customer = Convert.ToInt32(leer["id_customer"]);
                    i.customer = consultas.obtener_customer_id(Convert.ToString(i.id_customer));
                    i.family_trim = consultas.obtener_family_id(Convert.ToString(leer["id_family_trim"]));
                    lista.Add(i);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<Inventario> obtener_lista_trims_inventario_familia(string busqueda){
            List<Inventario> lista = new List<Inventario>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT TOP 50 i.id_inventario,i.id_pedido,i.total,i.id_customer,i.id_family_trim,i.descripcion,i.id_estilo " +
                    " FROM inventario i, family_trims ft WHERE ft.id_family_trim=i.id_family_trim AND " +
                    "  id_categoria_inventario=2 AND ft.family_trim like'%" + busqueda + "%' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Inventario i = new Inventario();
                    i.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    i.id_pedido = Convert.ToInt32(leer["id_pedido"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer["id_pedido"]));
                    i.id_estilo = Convert.ToInt32(leer["id_estilo"]);
                    i.estilo = consultas.obtener_estilo(Convert.ToInt32(leer["id_estilo"])) + " " + consultas.buscar_descripcion_estilo(Convert.ToInt32(leer["id_estilo"]));
                    i.total = Convert.ToInt32(leer["total"]);
                    i.descripcion = Convert.ToString(leer["descripcion"]);
                    i.id_customer = Convert.ToInt32(leer["id_customer"]);
                    i.customer = consultas.obtener_customer_id(Convert.ToString(i.id_customer));
                    i.family_trim = consultas.obtener_family_id(Convert.ToString(leer["id_family_trim"]));
                    lista.Add(i);
                }
                leer.Close();
            }
            finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<Inventario> obtener_lista_trims_inventario_po(string busqueda){
            List<Inventario> lista = new List<Inventario>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT TOP 50 i.id_inventario,i.id_pedido,i.total,i.id_customer,i.id_family_trim,i.descripcion,i.id_estilo " +
                    " FROM inventario i, PEDIDO p WHERE p.ID_PEDIDO=i.id_pedido AND " +
                    "  id_categoria_inventario=2 AND p.PO like'%" + busqueda + "%' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Inventario i = new Inventario();
                    i.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    i.id_pedido = Convert.ToInt32(leer["id_pedido"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer["id_pedido"]));
                    i.id_estilo = Convert.ToInt32(leer["id_estilo"]);
                    i.estilo = consultas.obtener_estilo(Convert.ToInt32(leer["id_estilo"])) + " " + consultas.buscar_descripcion_estilo(Convert.ToInt32(leer["id_estilo"]));
                    i.total = Convert.ToInt32(leer["total"]);
                    i.descripcion = Convert.ToString(leer["descripcion"]);
                    i.id_customer = Convert.ToInt32(leer["id_customer"]);
                    i.customer = consultas.obtener_customer_id(Convert.ToString(i.id_customer));
                    i.family_trim = consultas.obtener_family_id(Convert.ToString(leer["id_family_trim"]));
                    lista.Add(i);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<Inventario> obtener_lista_trims_inventario_estilo(string busqueda){
            List<Inventario> lista = new List<Inventario>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT TOP 50 i.id_inventario,i.id_pedido,i.total,i.id_customer,i.id_family_trim,i.descripcion,i.id_estilo " +
                    " FROM inventario i, ITEM_DESCRIPTION e WHERE e.ITEM_ID=i.id_estilo AND " +
                    "  id_categoria_inventario=2 AND (e.ITEM_STYLE like'%" + busqueda + "%' or  e.DESCRIPTION like'%" + busqueda + "%' )";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Inventario i = new Inventario();
                    i.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    i.id_pedido = Convert.ToInt32(leer["id_pedido"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer["id_pedido"]));
                    i.id_estilo = Convert.ToInt32(leer["id_estilo"]);
                    i.estilo = consultas.obtener_estilo(Convert.ToInt32(leer["id_estilo"])) + " " + consultas.buscar_descripcion_estilo(Convert.ToInt32(leer["id_estilo"]));
                    i.total = Convert.ToInt32(leer["total"]);
                    i.descripcion = Convert.ToString(leer["descripcion"]);
                    i.id_customer = Convert.ToInt32(leer["id_customer"]);
                    i.customer = consultas.obtener_customer_id(Convert.ToString(i.id_customer));
                    i.family_trim = consultas.obtener_family_id(Convert.ToString(leer["id_family_trim"]));
                    lista.Add(i);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public int buscar_id_item(int item){
            int temp = 0;
            Conexion con_u_i = new Conexion();
            try{
                SqlCommand com_u_i = new SqlCommand();
                SqlDataReader leer_u_i = null;
                com_u_i.Connection = con_u_i.AbrirConexion();
                com_u_i.CommandText = "SELECT id_item FROM inventario WHERE id_inventario='" + item + "' ";
                leer_u_i = com_u_i.ExecuteReader();
                while (leer_u_i.Read()){
                    temp += Convert.ToInt32(leer_u_i["id_item"]);
                }leer_u_i.Close();
            }finally { con_u_i.CerrarConexion(); con_u_i.Dispose(); }
            return temp;
        }
        public int buscar_elemento_inventario_orden_item(int item,int summary,int pedido,int estilo,int cliente){
            int temp = 0;
            Conexion con_u_i = new Conexion();
            try{
                SqlCommand com_u_i = new SqlCommand();
                SqlDataReader leer_u_i = null;
                com_u_i.Connection = con_u_i.AbrirConexion();
                com_u_i.CommandText = "SELECT id_inventario FROM inventario WHERE id_item='" + item + "' AND id_summary='" + summary + "'" +
                    " AND id_pedido='" + pedido + "' AND id_estilo='" + estilo + "'  AND id_customer='" + cliente + "' ";
                leer_u_i = com_u_i.ExecuteReader();
                while (leer_u_i.Read()){
                    temp = Convert.ToInt32(leer_u_i["id_inventario"]);
                }leer_u_i.Close();
            }finally { con_u_i.CerrarConexion(); con_u_i.Dispose(); }
            return temp;
        }
        public int buscar_request_summary_item(int item,int summary){
            int temp = 0;
            Conexion con_u_i = new Conexion();
            try{
                SqlCommand com_u_i = new SqlCommand();
                SqlDataReader leer_u_i = null;
                com_u_i.Connection = con_u_i.AbrirConexion();
                com_u_i.CommandText = "SELECT id_request,id_po_summary,id_size,id_item,total,restante,revision,usuario," +
                   " fecha,id_recibo,cantidad,blanks FROM trim_requests " +
                   " WHERE id_po_summary='" + summary + "' and id_item='" + item + "'";
                leer_u_i = com_u_i.ExecuteReader();
                while (leer_u_i.Read()){
                    temp = Convert.ToInt32(leer_u_i["id_request"]);
                }leer_u_i.Close();
            }finally { con_u_i.CerrarConexion(); con_u_i.Dispose(); }
            return temp;
        }
        public Inventario obtener_inventario_trim(int id){
            Inventario i = new Inventario();
            Conexion con_in = new Conexion();
            try{
                SqlCommand com_in = new SqlCommand();
                SqlDataReader leer_in = null;
                com_in.Connection = con_in.AbrirConexion();
                com_in.CommandText = "SELECT i.id_inventario,i.id_pedido,i.id_categoria_inventario,i.descripcion,c.categoria " +
                    " from inventario i,categorias_inventarios c where i.id_categoria_inventario=c.id_categoria and i.id_inventario='" + id + "' ";
                leer_in = com_in.ExecuteReader();
                while (leer_in.Read()){
                    i.id_inventario = Convert.ToInt32(leer_in["id_inventario"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer_in["id_pedido"]));
                    i.categoria_inventario = Convert.ToString(leer_in["categoria"]);
                    i.descripcion = Convert.ToString(leer_in["descripcion"]);
                }leer_in.Close();
            }finally { con_in.CerrarConexion(); con_in.Dispose(); }
            return i;
        }
        public Inventario obtener_item_editar(int id)        {
            Inventario i = new Inventario();
            Conexion con_oie = new Conexion();
            try            {
                SqlCommand com_oie = new SqlCommand();
                SqlDataReader leer_oie = null;
                com_oie.Connection = con_oie.AbrirConexion();
                com_oie.CommandText = "SELECT id_estilo,id_inventario,id_pedido,id_pais,id_fabricante,id_categoria_inventario,id_color,id_body_type,id_genero,id_fabric_type," +
                    "id_location,total,id_size,id_customer,id_customer_final,minimo,notas,id_fabric_percent,date_comment,comment,id_family_trim,id_unit,id_trim,descripcion,auditoria,id_summary " +
                    " from inventario  where id_inventario='" + id + "' ";
                leer_oie = com_oie.ExecuteReader();
                while (leer_oie.Read()){
                    i.id_inventario = Convert.ToInt32(leer_oie["id_inventario"]);
                    i.id_pedido = Convert.ToInt32(leer_oie["id_pedido"]);
                    i.id_categoria_inventario = Convert.ToInt32(leer_oie["id_categoria_inventario"]);
                    i.minimo = Convert.ToInt32(leer_oie["minimo"]);
                    i.notas = Convert.ToString(leer_oie["notas"]);
                    i.id_pais = Convert.ToInt32(leer_oie["id_pais"]);
                    i.id_fabricante = Convert.ToInt32(leer_oie["id_fabricante"]);
                    i.id_color = Convert.ToInt32(leer_oie["id_color"]);
                    i.id_body_type = Convert.ToInt32(leer_oie["id_body_type"]);
                    i.id_genero = Convert.ToInt32(leer_oie["id_genero"]);
                    i.id_fabric_type =Convert.ToInt32(leer_oie["id_fabric_type"]);
                    i.id_location = Convert.ToInt32(leer_oie["id_location"]);
                    i.total = Convert.ToInt32(leer_oie["total"]);
                    i.id_size = Convert.ToInt32(leer_oie["id_size"]);
                    i.id_customer = Convert.ToInt32(leer_oie["id_customer"]);
                    i.id_final_customer = Convert.ToInt32(leer_oie["id_customer_final"]);
                    i.id_fabric_percent = Convert.ToInt32(leer_oie["id_fabric_percent"]);
                    i.comment = Convert.ToString(leer_oie["comment"]);
                    i.id_family_trim = Convert.ToString(leer_oie["id_family_trim"]);
                    i.id_unit =Convert.ToString(leer_oie["id_unit"]);
                    i.descripcion =Convert.ToString(leer_oie["descripcion"]);
                    i.id_trim = Convert.ToInt32(leer_oie["id_trim"]);
                    i.id_estilo = Convert.ToInt32(leer_oie["id_estilo"]);
                    i.auditoria= Convert.ToInt32(leer_oie["auditoria"]);
                    i.id_summary= Convert.ToInt32(leer_oie["id_summary"]);
                }
                leer_oie.Close();
            }finally { con_oie.CerrarConexion(); con_oie.Dispose(); }
            return i;
        }
        public void restar_cantidad_request(int request, int cantidad){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE trim_requests SET restante=restante+'" + cantidad + "' WHERE id_request='" + request + "'  ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public void eliminar_inventario(int inventario){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "DELETE FROM inventario WHERE id_inventario='" + inventario + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public int buscar_total_item_inventario(int id,int cliente){
            int temp =0;
            Conexion con_oie = new Conexion();
            try{
                SqlCommand com_oie = new SqlCommand();
                SqlDataReader leer_oie = null;
                com_oie.Connection = con_oie.AbrirConexion();
                com_oie.CommandText = "SELECT descripcion,total,id_inventario FROM inventario " +
                    " WHERE id_item='" + id + "' AND id_customer='" + cliente + "' " +
                    " AND id_estilo=0 AND id_pedido=0 AND id_summary=0 ";
                leer_oie = com_oie.ExecuteReader();
                while (leer_oie.Read()){
                    temp = Convert.ToInt32(leer_oie["total"]);                                      
                }leer_oie.Close();
            }finally { con_oie.CerrarConexion(); con_oie.Dispose(); }
            return temp;
        }
        public List<Inventario> buscar_inventario_customer(int cliente) {
            List<Inventario> lista = new List<Inventario>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ic.item,ic.descripcion,ic.fabric_type,i.total FROM inventario i,items_catalogue ic" +
                    " WHERE i.id_customer=" + cliente + " AND i.id_categoria_inventario=2 AND i.id_item=ic.item_id ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Inventario i = new Inventario();
                    i.amt_item = Convert.ToString(leer["item"]);
                    i.descripcion = Convert.ToString(leer["descripcion"]);                  
                    i.family_trim = Convert.ToString(leer["fabric_type"]);                  
                    i.total = Convert.ToInt32(leer["total"]);                  
                    lista.Add(i);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }




































































    }
}