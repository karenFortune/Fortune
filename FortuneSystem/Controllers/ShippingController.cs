using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FortuneSystem.Models.Almacen;
using FortuneSystem.Models.Shipping;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using OfficeOpenXml;
using System.Data;
using ClosedXML.Excel;
using ZXing.Common;
using ZXing.QrCode;
using System.Text.RegularExpressions;

namespace FortuneSystem.Controllers
{
    public class ShippingController : Controller
    {
        DatosInventario di = new DatosInventario();
        DatosReportes dr = new DatosReportes();
        DatosShipping ds = new DatosShipping();
        FuncionesInventarioGeneral consultas = new FuncionesInventarioGeneral();
        DatosTransferencias dt = new DatosTransferencias();
        QRCodeController qr = new QRCodeController();
        PDFController pdf = new PDFController();

        public ActionResult Index()
        {
            //Session["id_usuario"] = 2;
            //Session["id_usuario"] = consultas.buscar_id_usuario(Convert.ToString(Session["usuario"]));
            if (Session["usuario"] == null){
                return View();
            }else {
                return View();
            }
        }

        public ActionResult new_pk(){
            return View();
        }

        public JsonResult buscar_pedidos_inicio(string busqueda)
        {
            return Json(ds.lista_ordenes(busqueda), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_estilos_pedido(string id_pedido)
        {
            Session["id_pedido"] = id_pedido;
            return Json(ds.lista_estilos(id_pedido), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Autocomplete_tallas(string term){
            var items = consultas.Lista_tallas();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public JsonResult guardar_recibo_inventario_shipping(string estilos, string colores, string tallas, string piezas, string cajas){
            string[] Estilos = estilos.Split('*'), Colores = colores.Split('*'), Tallas = tallas.Split('*'), Piezas = piezas.Split('*'), Cajas = cajas.Split('*');
            int total_piezas = 0, id_talla, id_color, existencia, total_cajas = 0, id_recibo, id_inventario;
            string descripcion, estilo;
            for (int i = 1; i < Estilos.Length; i++){
                total_piezas += Convert.ToInt32(Piezas[i]);
                total_cajas += Convert.ToInt32(Cajas[i]);
            }
            ds.guardar_recibo_fantasy(Convert.ToInt32(Session["id_pedido"]), Convert.ToInt32(Session["id_usuario"]), total_piezas, total_cajas);
            id_recibo = ds.obtener_ultimo_recibo();
            for (int i = 1; i < Estilos.Length; i++){
                id_talla = consultas.buscar_talla(Tallas[i]);
                id_color = consultas.buscar_color_codigo(Colores[i]);
                estilo = consultas.obtener_estilo(Convert.ToInt32(Estilos[i])) + " " + consultas.buscar_descripcion_estilo(Convert.ToInt32(Estilos[i]));
                descripcion = estilo + " " + Colores[i] + " " + Tallas[i];
                descripcion = Regex.Replace(descripcion, @"\s+", " ");
                existencia = ds.buscar_existencia_inventario(id_color, id_talla, Estilos[i]);
                if (existencia == 0){
                    ds.guardar_item_inventario(id_color, id_talla, Estilos[i], descripcion, Convert.ToInt32(Cajas[i]) * Convert.ToInt32(Piezas[i]));
                    id_inventario = ds.obtener_ultimo_item();
                }else{
                    id_inventario = existencia;
                    ds.aumentar_inventario(existencia, Convert.ToInt32(Cajas[i]), Convert.ToInt32(Piezas[i]));
                }
                ds.guardar_recibo_fantasy_item(id_recibo, id_inventario, Cajas[i], Piezas[i]);
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_po(string term){
            var items = consultas.Lista_po_abiertos();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_estilos(string term){
            var items = consultas.Lista_styles();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_po_abiertos(string term)
        {
            var items = consultas.Lista_po_abiertos();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public JsonResult obtener_estilos_dc(string po)
        {
            Session["po"] = po;
            return Json(ds.buscar_estilos_po(po), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_conductores()
        {
            return Json(ds.obtener_drivers(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_carriers()
        {
            return Json(ds.obtener_carriers(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult enviar_informacion_driver(string carrier, string nombre, string plates, string scac, string caat, string tractor)
        {
            ds.guardar_nuevo_conductor(carrier, nombre, plates, scac, caat, tractor);
            return Json(ds.obtener_carriers(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_informacion_driver(string id)
        {
            Session["id_driver_edit"] = id;
            return Json(ds.obtener_conductor_edicion(id), JsonRequestBehavior.AllowGet);
        }
        public JsonResult enviar_informacion_driver_edicion(string id, string carrier, string nombre, string plates, string scac, string caat, string tractor)
        {
            ds.guardar_conductor_edicion(id, carrier, nombre, plates, scac, caat, tractor);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_direcciones_envio()
        {
            return Json(ds.obtener_direcciones(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult enviar_informacion_direccion(string nombre, string direccion, string ciudad, string zip)
        {
            ds.guardar_nueva_direccion(nombre, direccion, ciudad, zip);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_modificar_edicion(string id)
        {
            return Json(ds.obtener_direccion_edicion(id), JsonRequestBehavior.AllowGet);
        }
        public JsonResult enviar_informacion_direccion_edicion(string id, string nombre, string direccion, string ciudad, string zip)
        {
            ds.guardar_direccion_edicion(id, nombre, direccion, ciudad, zip);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult eliminar_conductor(string id)
        {
            ds.borrar_conductor(id);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult eliminar_direccion(string id)
        {
            ds.borrar_direccion(id);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_contenedores_select()
        {
            return Json(ds.obtener_contenedores_select(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_conductores_select()
        {
            return Json(ds.obtener_conductores_select(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_direcciones_select()
        {
            return Json(ds.obtener_direcciones_select(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_estilos_pk(string pedido)
        {
            int id_pedido = consultas.buscar_pedido(pedido);
            return Json(ds.obtener_lista_tarimas_estilos(id_pedido), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_pedidos()
        {
            return Json(ds.obtener_lista_po_summarys(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult guardar_pk(string po, string address, string driver, string container, string seal, string replacement, string manager, string tipo, string labels, string type_labels, int dcs,string num_envio)
        {
            int usuario = Convert.ToInt32(Session["id_usuario"]);
            string[] pk_ant, year_pk;
            string pk_anterior = ds.obtener_ultimo_pk();
            int indice_pk = 0, ultimo_pk, id_customer, id_customer_po, id_pedido;
            if (pk_anterior != ""){
                pk_ant = pk_anterior.Split('-');
                year_pk = pk_ant[1].Split(' ');
                if (year_pk[0] == (DateTime.Now.Year.ToString())){
                    indice_pk = (Convert.ToInt32(pk_ant[0]) + 1);
                }else { indice_pk = 1; }
            }else{
                indice_pk = 1;
            }
            id_pedido = Convert.ToInt32(po);
            id_customer = consultas.obtener_customer_po(id_pedido);
            id_customer_po = consultas.obtener_customer_final_po(id_pedido);
            ds.guardar_pk_nuevo(id_pedido, id_customer, id_customer_po, ((Convert.ToString(indice_pk).PadLeft(4, '0')) + "-" + DateTime.Now.Year.ToString() + " FFB"), address, driver, container, seal, replacement, manager, tipo, usuario, num_envio);
            ultimo_pk = ds.obtener_ultimo_pk_registrado();
            if (labels != "N/A"){
                string[] label = labels.Split('*');
                for (int i = 1; i < label.Length; i++){
                    ds.guardar_pk_labels(label[i], ultimo_pk, type_labels);
                }
            }
            Session["pedido"] = id_pedido;
            Session["pk"] = ultimo_pk;
            var result = Json(new { dc = dcs, estilos = ds.lista_estilos(Convert.ToString(id_pedido)), cantidades_estilos = ds.obtener_cantidades_estilos(id_pedido), number_po = ds.obtener_number_po_pedido(id_pedido) });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_assortment_pedido()
        {
            return Json(ds.lista_assortments_pedido(Convert.ToInt32(Session["pedido"])), JsonRequestBehavior.AllowGet);
        }
        public JsonResult agregar_otros_estilos(string estilos, string rows, string number_po)
        {
            string[] Row = rows.Split(','), Talla = estilos.Split('*'), Estilo = estilos.Split('*');
            for (int i = 0; i < Row.Length; i++)
            {
                string[] info = Row[i].Split('*');
                int total = 0;
                for (int j = 2; j < info.Length; j++) { total += Convert.ToInt32(info[j]); }
                int summary = consultas.obtener_po_summary(Convert.ToInt32(Session["pedido"]), Convert.ToInt32(info[0]));
                ds.guardar_pk_otros(Convert.ToInt32(Session["pk"]), Convert.ToInt32(Session["pedido"]), info[0], info[1], number_po, summary, total);
                int id_shipping_id = ds.obtener_ultimo_shipping_id();

                for (int j = 2; j < info.Length; j++)
                {
                    if (info[j] != "0")
                    {                     //cantidad//talla
                        ds.guardar_ratio_otros(id_shipping_id, info[j], Estilo[j - 1]);
                        ds.agregar_cantidades_enviadas(summary, Convert.ToInt32(Estilo[j - 1]), Convert.ToInt32(info[j]), id_shipping_id, info[1], 0);
                    }
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_estilos_pk_otros(string id)
        {
            Session["pedido"] = ds.buscar_pedido_pk(id);
            Session["pk"] = id;
            var result = Json(new { po_number = ds.buscar_po_number_pk(id), estilos = ds.lista_estilos(Convert.ToString(Session["pedido"])) });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult agregar_dcs_estilos(string estilos, string dcs, string number_po)
        {//ESTE AGREGA LOS DC POR ESTILO
            string[] dc = dcs.Split(',');
            for (int i = 0; i < dc.Length; i++)
            {
                ds.guardar_estilos_dcs(Convert.ToString(Session["pedido"]), estilos, dc[i], number_po, Convert.ToInt32(Session["pk"]));
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        //GUARDAR ESTILOS DE PK
        //GUARDAR ESTILOS DE PK
        public JsonResult guardar_pk_estilos(string tipo_packing, string estilos, string cantidades, string tiendas, string tipos, string tallas, string number_po)
        {  //ESTE GUARDA LOS ESTILOS  s
            string[] Estilo = estilos.Split('*'), Cantidad = cantidades.Split('*'), Tienda = tiendas.Split('*'), Tipo = tipos.Split('*'), Talla = tallas.Split('*');
            int ultimo_shipping_id = 0;
            for (int i = 1; i < Estilo.Length; i++){
                int id_talla = consultas.buscar_talla(Talla[i]);
                int summary = consultas.obtener_po_summary(Convert.ToInt32(Session["pedido"]), Convert.ToInt32(Estilo[i]));
                ds.guardar_estilos_paking(Convert.ToInt32(Session["pk"]), number_po, Convert.ToString(Session["pedido"]), Estilo[i], Cantidad[i], Tienda[i], Tipo[i], id_talla.ToString(), summary);
                ultimo_shipping_id = ds.obtener_ultimo_shipping_registrado();
                if (tipo_packing == "1"){
                    ds.agregar_cantidades_enviadas(summary, id_talla, Convert.ToInt32(Cantidad[i]), ultimo_shipping_id, Tipo[i], 0);
                }else{//tipo packing 2
                    List<ratio_tallas> ratio = ds.obtener_lista_ratio(summary, Convert.ToInt32(Estilo[i]));
                    foreach (ratio_tallas r in ratio){
                        ds.agregar_cantidades_enviadas(summary, r.id_talla, (Convert.ToInt32(Cantidad[i]) * r.ratio), ultimo_shipping_id, Tipo[i], 0);
                    }
                }
            }
            ds.actualizar_tipo_empaque_pk(Convert.ToInt32(Session["pk"]), Convert.ToInt32(tipo_packing));
            verificar_estado_pedido();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        //VERIFICAR SI YA SE MANDO TODO O NO PARA CERRAR EL PEDIDO
        public void verificar_estado_pedido()
        {
            int cerrar = 0;
            List<Cantidades_Estilos> cantidades_estilos = ds.obtener_cantidades_estilos(Convert.ToInt32(Session["pedido"]));
            foreach (Cantidades_Estilos item in cantidades_estilos){
                foreach (Talla i in item.lista_tallas){
                    if (i.total > 0){ //SI QUEDAN RESTANTES
                        cerrar++;
                    }
                }
            }
            if (cerrar == 0) {
                ds.cerrar_pedido(Convert.ToInt32(Session["pedido"]));
                ds.eliminar_inventario_pedido(Convert.ToInt32(Session["pedido"]));
            }
        }
        public JsonResult cerrar_po(string po) {
            ds.cerrar_pedido(consultas.buscar_pedido(po));
            return Json("", JsonRequestBehavior.AllowGet);
        }
        //GUARDAR ESTILOS DE PK
        //GUARDAR ESTILOS DE PK
        public JsonResult guardar_pk_estilos_dcs(string tipo_packing, string estilos, string cantidades, string tiendas, string tipos, string tallas, string number_po, string dcs)
        {
            string[] Estilo = estilos.Split('*'), Cantidad = cantidades.Split('*'), Tienda = tiendas.Split('*'), Tipo = tipos.Split('*'), Talla = tallas.Split('*'), DC = dcs.Split('*');
            int ultimo_shipping_id = 0;
            for (int i = 1; i < Estilo.Length; i++)
            {
                int id_talla = consultas.buscar_talla(Talla[i]);
                int summary = consultas.obtener_po_summary(Convert.ToInt32(Session["pedido"]), Convert.ToInt32(Estilo[i]));
                ds.guardar_estilos_paking_dcs(Convert.ToInt32(Session["pk"]), number_po, Convert.ToString(Session["pedido"]), Estilo[i], Cantidad[i], Tienda[i], Tipo[i], id_talla.ToString(), summary, DC[i]);
                ultimo_shipping_id = ds.obtener_ultimo_shipping_registrado();
                List<ratio_tallas> ratio = ds.obtener_lista_ratio(summary, Convert.ToInt32(Estilo[i]));
                foreach (ratio_tallas r in ratio)
                {
                    ds.agregar_cantidades_enviadas(summary, r.id_talla, (Convert.ToInt32(Cantidad[i]) * r.ratio), ultimo_shipping_id, Tipo[i], 0);
                }
            }
            ds.actualizar_tipo_empaque_pk(Convert.ToInt32(Session["pk"]), Convert.ToInt32(tipo_packing));
            verificar_estado_pedido();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        //GUARDAR ESTILOS DE PK
        //GUARDAR ESTILOS DE PK
        public JsonResult guardar_pk_estilos_assort(string estilos, string cantidades, string tiendas, string tipos, string number_po)
        {
            //LOS ESTILOS SON LOS IDS DE ASSORTMENT
            string[] Estilo = estilos.Split('*'), Cantidad = cantidades.Split('*'), Tienda = tiendas.Split('*'), Tipo = tipos.Split('*');
            int ultimo_shipping_id = 0;
            ds.actualizar_tipo_empaque_pk(Convert.ToInt32(Session["pk"]), 3);
            for (int i = 1; i < Estilo.Length; i++)
            {
                ds.guardar_estilos_paking_assort(Convert.ToInt32(Session["pk"]), number_po, Convert.ToString(Session["pedido"]), Estilo[i], Cantidad[i], Tienda[i], Tipo[i]);
                ultimo_shipping_id = ds.obtener_ultimo_shipping_registrado();
                List<Assortment> assort = ds.obtener_assortment_by_id(Convert.ToInt32(Estilo[i]));
                foreach (Assortment a in assort)
                {
                    ds.agregar_cantidades_enviadas(a.id_summary, a.id_talla, (Convert.ToInt32(Cantidad[i]) * a.ratio), ultimo_shipping_id, Tipo[i], Convert.ToInt32(Estilo[i]));
                }
            }
            verificar_estado_pedido();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public void actualizar_packing_type(int pk, int summary)
        {
            int tipo_packing = ds.buscar_tipo_pk(summary);
            ds.actualizar_tipo_empaque_pk(pk, tipo_packing);
        }
        public JsonResult buscar_estilos_packing(int id)
        {
            //buscar_tipo_empaque en el pk
            int tipo_empaque = ds.obtener_tipo_empaque_pk(id);
            if (tipo_empaque != 3)
            {
                return Json(Json(new { tipo_packing = tipo_empaque, estilos = ds.lista_estilos_packing(id) }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Json(new { tipo_packing = tipo_empaque, estilos = ds.lista_assortings_packing(id) }), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult guardar_ids_pk(int tarima, string estilos, int pk)
        {
            string[] Estilo = estilos.Split('*');
            for (int i = 1; i < Estilo.Length; i++)
            {
                ds.guardar_ids_tarimas(tarima, Estilo[i]);
            }
            int tipo_empaque = ds.obtener_tipo_empaque_pk(pk);
            if (tipo_empaque != 3)
            {
                return Json(Json(new { tipo_packing = tipo_empaque, estilos = ds.lista_estilos_packing(pk) }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Json(new { tipo_packing = tipo_empaque, estilos = ds.lista_assortings_packing(pk) }), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult buscar_pk_tabla(string busqueda)
        {
            return Json(ds.lista_buscar_pk_inicio(busqueda), JsonRequestBehavior.AllowGet);
        }
        public JsonResult abrir_pk(string id)
        {
            Session["pk"] = id;
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public void excel_pk(){
            int rerferw = 0;
            rerferw++;
            List<Pk> lista = ds.obtener_packing_list(Convert.ToInt32(Session["pk"]));
            foreach (Pk item in lista){
                switch (item.tipo_empaque){
                    case 1:
                        excel_pk_bp(lista);
                        break;
                    case 2:
                        excel_pk_ppk(lista);
                        break;
                    case 3:
                        excel_pk_assort(lista);
                        break;
                }
            }
        }
        public void excel_pk_ppk(List<Pk> lista)
        {
            // List<Pk> lista = ds.obtener_packing_list(Convert.ToInt32(Session["pk"]));
            string clave_packing = "";
            using (XLWorkbook libro_trabajo = new XLWorkbook())
            {
                int estilos_total = 0, rows = 0, tarima_contador = 0, total_ratio, contador, r, c, total_cajas = 0, filas = 0, columnas = 0, tallas_id;

                var ws = libro_trabajo.Worksheets.Add("PK");
                foreach (Pk item in lista)
                {
                    //item.tipo_empaque = 2;                    
                    clave_packing = item.packing;
                    /*****INICIO CON DIRECCIONES, LOGO, ETC******/
                    ws.Cell("A2").Value = "FORTUNE FASHIONS BAJA, S.R.L. DE C.V.";
                    ws.Cell("A3").Value = "CALLE TORTUGA No 313-A";
                    ws.Cell("A4").Value = "MANEADERO CP 22790";
                    ws.Cell("A5").Value = "BAJA CALIFORNIA";
                    ws.Style.Font.FontSize = 11;

                    ws.Range("A2:A10").Style.Font.Bold = true;
                    ws.Range("A7:A10").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    //DIRECCIÓN DE ORIGEN
                    ws.Cell("A7").Value = "CUSTOMER: ";
                    ws.Cell("B7").Value = item.customer;

                    int ex_label = ds.contar_labels(item.id_packing_list);
                    if (ex_label != 0)
                    {
                        List<Labels> lista_etiquetas = new List<Labels>();
                        lista_etiquetas = ds.obtener_etiquetas(item.id_packing_list);
                        ws.Cell("A8").Value = "P.O.: ";
                        string label = Regex.Replace(item.pedido, @"\s+", " ") + "(PO# ";
                        foreach (Labels l in lista_etiquetas) { label += " " + l.label; }
                        if (ex_label == 1)
                        {
                            label += " )" + " With UCC Labels " + item.parte;
                        }
                        else
                        {
                            label += " )" + " With TPM Labels " + item.parte;
                        }
                        ws.Cell("B8").Value = label;
                    }
                    else
                    {
                        ws.Cell("A8").Value = "P.O.: ";
                        ws.Cell("B8").Value = Regex.Replace(item.pedido, @"\s+", " ") + " Without UCC Labels " + item.parte;
                    }

                    ws.Cell("A9").Value = "RETAILER: ";
                    ws.Cell("B9").Value = item.customer_po;
                    if (item.tipo != "1")
                    {
                        ws.Cell("A10").Value = "EXAMPLES ";
                        ws.Cell("A10").Style.Font.FontSize = 14;
                    }

                    //IMAGEN AL CENTRO
                    ws.Range(1, 7, 1, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    var imagePath = Server.MapPath("/") + "/Content/img/LOGO FORTUNE.png";
                    //var imagePath = @"C:\Users\melissa\source\repos\FortuneSys----\FortuneSystem\Content\img\LOGO FORTUNE.png";
                    var image = ws.AddPicture(imagePath).MoveTo(ws.Cell("E1")).Scale(0.30);
                    //PK ABAJO DE LA IMAGEN
                    ws.Cell("D7").Value = "PK: ";
                    ws.Cell("D7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("D7").Style.Font.Bold = true;
                    ws.Cell("E7").Value = item.packing;
                    ws.Cell("E7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range("E7:F7").Merge();
                    ws.Range("E7", "F7").Style.Font.Bold = true;
                    ws.Range("D7", "F7").Style.Font.FontSize = 15;
                    ws.Range("E7:F7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range("E7:F7").Style.Border.BottomBorderColor = XLColor.Black;
                    //DIRECCION DE DESTINO
                    ws.Cell("L2").Value = "SHIP TO: ";
                    ws.Cell("L3").Value = item.destino.nombre;
                    ws.Cell("L4").Value = item.destino.direccion;
                    ws.Cell("L5").Value = item.destino.ciudad + " " + item.destino.zip;
                    ws.Cell("L8").Value = "DATE:" + item.fecha;
                    ws.Range("L2", "L10").Style.Font.Bold = true;
                    var columna_a = ws.Range("A2", "A10");
                    ws.Rows("6").Height = 30;
                    /****************T*A*B*L*A************************************************/
                    int contador_cabeceras = 0, contador_tallas = 0, pallets = 0, tiendas = 0;
                    //PPK
                    var headers = new List<String[]>();
                    List<String> titulos = new List<string>();//=  "ID", "P.O. NUM", "TYPE", "COLOR", "DESCRIPTION";
                    titulos.Add("ID"); titulos.Add("P.O. NUM"); titulos.Add("TYPE"); titulos.Add("STYLE"); titulos.Add("COLOR"); titulos.Add("DESCRIPTION");
                    foreach (Tarima t in item.lista_tarimas)
                    {
                        foreach (estilos e in t.lista_estilos)
                        {
                            if (e.store != "N/A" && e.store != "NA")
                            {
                                tiendas++;
                            }
                        }
                    }
                    if (tiendas != 0) { titulos.Add("STORE"); }
                    if (item.dc != 0) { titulos.Add("DC"); }
                    titulos.Add("PPK");
                    foreach (Tarima t in item.lista_tarimas)
                    {
                        foreach (estilos e in t.lista_estilos)
                        {
                            if (contador_cabeceras == 0 && e.tipo != "EXT" && e.tipo != "DMG" && e.tipo != "RPLN" && e.tipo != "ECOM")
                            {
                                foreach (ratio_tallas ra in e.lista_ratio)
                                {
                                    titulos.Add(ra.talla);
                                }
                                contador_cabeceras++;
                            }
                        }
                    }
                    titulos.Add("PCS"); titulos.Add("BXS"); titulos.Add("PALLETS");
                    headers.Add(titulos.ToArray());
                    int total_titulos = (titulos.ToArray()).Length;
                    ws.Cell(11, 1).Value = headers;
                    ws.Column(2).AdjustToContents();
                    ws.Column(5).AdjustToContents();
                    for (int i = 1; i <= total_titulos; i++)
                    {
                        ws.Cell(11, i).Style.Font.Bold = true;
                        ws.Cell(11, i).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                        ws.Cell(11, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(11, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.LeftBorderColor = XLColor.Black;
                        ws.Cell(11, i).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.RightBorderColor = XLColor.Black;
                        ws.Cell(11, i).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.TopBorderColor = XLColor.Black;
                        ws.Cell(11, i).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.BottomBorderColor = XLColor.Black;
                    }
                    ws.Rows("6").Height = 30;
                    /***********************************************************************************/
                    List<int> tallas_id_temporal = new List<int>();
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        if (contador_tallas == 0)
                        {
                            foreach (estilos e in tarimas.lista_estilos)
                            {
                                if (contador_tallas == 0 && e.tipo != "EXT" && e.tipo != "DMG" && e.tipo != "RPLN" && e.tipo != "ECOM")
                                {
                                    foreach (var ratio in e.lista_ratio)
                                    {
                                        contador_tallas++;
                                    }
                                }
                            }
                        }
                    }
                    int temporal = 0;
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        foreach (estilos e in tarimas.lista_estilos)
                        {
                            if (temporal == 0 && e.tipo != "EXT" && e.tipo != "DMG" && e.tipo != "RPLN" && e.tipo != "ECOM")
                            {
                                foreach (var ratio in e.lista_ratio)
                                {
                                    tallas_id_temporal.Add(ratio.id_talla);
                                }
                                temporal++;
                            }
                        }
                    }

                    //tallas_id_temporal.Add(ratio.id_talla);
                    int[] tallas_comparacion = tallas_id_temporal.ToArray();

                    foreach (Tarima tarimas in item.lista_tarimas) { pallets++; }

                    int[] sumas_tallas = new int[contador_tallas + 2];
                    for (int i = 0; i < contador_tallas + 2; i++) { sumas_tallas[i] = 0; }

                    r = 12;
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        estilos_total = 0; rows = 0; tarima_contador = 0; rows = 0;
                        foreach (estilos estilo in tarimas.lista_estilos) { estilos_total++; }
                        ws.Cell(r, 1).Value = tarimas.id_tarima;
                        ws.Range(r, 1, (r + estilos_total - 1), 1).Merge();
                        var celdas_estilos = new List<String[]>();
                        foreach (estilos estilo in tarimas.lista_estilos)
                        {
                            List<String> datos = new List<string>();
                            datos.Add(Convert.ToString(estilo.number_po));
                            if (estilo.tipo != "NONE") { datos.Add(estilo.tipo); }
                            else { datos.Add(" "); }
                            datos.Add(estilo.estilo);
                            datos.Add(estilo.color);
                            datos.Add(estilo.descripcion);
                            if (tiendas != 0) { datos.Add(estilo.store); }
                            if (item.dc != 0) { datos.Add(Convert.ToString(estilo.dc)); }
                            total_ratio = 0;
                            foreach (ratio_tallas ratio in estilo.lista_ratio) { total_ratio++; }
                            contador = 0;
                            string ppk = "";
                            if (estilo.tipo == "EXT" || estilo.tipo == "DMG" || estilo.tipo == "RPLN" || estilo.tipo == "ECOM")
                            {
                                datos.Add("N/A");
                            }
                            else
                            {
                                foreach (ratio_tallas ratio in estilo.lista_ratio)
                                {
                                    contador++;
                                    ppk += ratio.ratio;
                                    if (contador < total_ratio) { ppk += "-"; }
                                }
                                datos.Add(ppk);
                            }

                            int ii = 0, total_talla, total_estilo = 0;
                            if (estilo.tipo == "DMG" || estilo.tipo == "EXT" || estilo.tipo == "RPLN" || estilo.tipo == "ECOM")
                            {

                                foreach (int tall in tallas_comparacion)
                                {
                                    foreach (ratio_tallas ratio in estilo.lista_ratio)
                                    {
                                        if (tall == ratio.id_talla)
                                        {
                                            total_talla = ratio.ratio * estilo.boxes;
                                            sumas_tallas[ii] += total_talla;
                                            total_estilo += total_talla;
                                            datos.Add(Convert.ToString(total_talla));
                                        }
                                        else
                                        {
                                            datos.Add("");
                                        }
                                        ii++;
                                        //tallas_id++;
                                    }
                                }
                            }
                            else
                            {
                                foreach (ratio_tallas ratio in estilo.lista_ratio)
                                {
                                    total_talla = ratio.ratio * estilo.boxes;
                                    sumas_tallas[ii] += total_talla;
                                    total_estilo += total_talla;
                                    ii++;
                                    datos.Add(Convert.ToString(total_talla));
                                }
                            }

                            datos.Add(Convert.ToString(total_estilo));
                            sumas_tallas[ii] += total_estilo; ii++;
                            if (estilo.tipo == "EXT" || estilo.tipo == "DMG" || estilo.tipo == "RPLN" || estilo.tipo == "ECOM")
                            {
                                datos.Add("1");
                                sumas_tallas[ii] += 1;
                            }
                            else
                            {
                                datos.Add(Convert.ToString(estilo.boxes));
                                sumas_tallas[ii] += estilo.boxes;
                            }
                            celdas_estilos.Add(datos.ToArray());
                        }//ESTILOS                            
                        ws.Cell(r, 2).Value = celdas_estilos;
                        c = 8;
                        if (item.dc != 0) { c++; }
                        if (tiendas != 0) { c++; }
                        c = c + (contador_tallas + 2);
                        ws.Cell(r, c).Value = "1";
                        ws.Range(r, c, (r + estilos_total - 1), c).Merge();
                        r += estilos_total;
                        columnas = c;
                    }//TARIMAS
                    contador = 0;
                    string descripcion_final = "";
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        if (contador == 0)
                        {
                            foreach (estilos estilo in tarimas.lista_estilos)
                            {
                                descripcion_final += Regex.Replace(estilo.descripcion_final, @"\s+", " ") + " ";
                            }
                            contador++;
                        }
                    }

                    ws.Cell(r, 1).Value = descripcion_final;
                    ws.Cell(r, 1).Style.Font.Bold = true;
                    ws.Cell(r, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(r, 1, r, 4).Merge();
                    ws.Range(r, 1, r, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);


                    ws.Cell(r, 5).Value = "TOTAL";
                    ws.Cell(r, 5).Style.Font.Bold = true;
                    ws.Cell(r, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    c = 7;
                    if (item.dc != 0) { c++; }
                    if (tiendas != 0) { c++; }
                    ws.Range(r, 5, r, c).Merge();
                    ws.Range(r, 5, r, c).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                    c++;

                    for (int i = 0; i < contador_tallas + 2; i++)
                    {
                        ws.Cell(r, c).Value = sumas_tallas[i];
                        ws.Cell(r, c).Style.Font.Bold = true;
                        ws.Cell(r, c).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                        ws.Cell(r, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        c++;
                    }
                    ws.Cell(r, c).Value = pallets;
                    ws.Cell(r, c).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                    ws.Cell(r, c).Style.Font.Bold = true;
                    ws.Cell(r, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    filas = r;
                    for (int i = 11; i <= (filas); i++)
                    {
                        for (int j = 1; j <= columnas; j++)
                        {
                            ws.Cell(i, j).Style.Font.FontSize = 9;
                            ws.Cell(i, j).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            ws.Cell(i, j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(i, j).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.LeftBorderColor = XLColor.Black;
                            ws.Cell(i, j).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.RightBorderColor = XLColor.Black;
                            ws.Cell(i, j).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.TopBorderColor = XLColor.Black;
                            ws.Cell(i, j).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.BottomBorderColor = XLColor.Black;
                        }
                    }

                    //tabla------PPK

                    filas += 2;
                    ws.Range(filas, 1, (filas + 2), 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Range(filas, 1, (filas + 2), 1).Style.Font.Bold = true;
                    ws.Range(filas, 7, (filas + 2), 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Range(filas, 7, (filas + 2), 7).Style.Font.Bold = true;
                    ws.Range(filas, 2, (filas + 2), 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(filas, 8, (filas + 2), 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(filas, 1).Value = "DRIVER NAME:";
                    ws.Cell(filas, 2).Value = item.conductor.driver_name;
                    ws.Cell(filas, 7).Value = "SHIPPING MANAGER:";
                    ws.Cell(filas, 8).Value = item.shipping_manager;
                    ws.Range(filas, 2, filas, 3).Merge();
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorderColor = XLColor.Black;
                    ws.Range(filas, 8, filas, 11).Merge();
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorderColor = XLColor.Black;
                    filas++;
                    ws.Cell(filas, 1).Value = "TRAILER/PLATES:";
                    ws.Cell(filas, 2).Value = item.conductor.tractor + "/" + item.conductor.plates;
                    ws.Cell(filas, 7).Value = "SEAL:";
                    ws.Cell(filas, 8).Value = item.seal;
                    ws.Range(filas, 2, filas, 3).Merge();
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorderColor = XLColor.Black;
                    ws.Range(filas, 8, filas, 11).Merge();
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorderColor = XLColor.Black;
                    filas++;
                    ws.Cell(filas, 1).Value = "CONTAINER/PLATES:";
                    ws.Cell(filas, 2).Value = item.contenedor.eco + "/" + item.contenedor.plates;
                    ws.Cell(filas, 7).Value = "REPLACEMENT:";
                    ws.Cell(filas, 8).Value = item.replacement;
                    ws.Range(filas, 2, filas, 3).Merge();
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorderColor = XLColor.Black;
                    ws.Range(filas, 8, filas, 11).Merge();
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorderColor = XLColor.Black;
                    filas++;
                    ws.Cell(filas, 3).Value = "DOCUMENTO CONTROLADO. ÚNICAMENTE SE PUEDE MODIFICAR POR EL SUPERVISOR DE SHIPPING";
                    ws.Cell(filas, 3).Style.Font.FontColor = XLColor.FromArgb(100, 100, 100);
                    filas += 2;
                    columnas = 2;

                    var porcentajes = new List<String[]>();
                    List<String> p = new List<string>();
                    p.Add("ORIGIN"); p.Add("QTY");p.Add("%");
                    porcentajes.Add(p.ToArray());
                    List<Fabricantes> totales_paises_estilo = new List<Fabricantes>();
                    List<Fabricantes> totales_paises = new List<Fabricantes>();
                    int add = 0, total_paises = 0, iguales;
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        totales_paises_estilo = ds.buscar_paises_estilos(tarimas.lista_estilos);
                        foreach (Fabricantes fa in totales_paises_estilo)
                        {
                            iguales = 0;
                            if (add == 0)
                            {
                                Fabricantes nf = new Fabricantes();
                                nf.cantidad = fa.cantidad;
                                nf.pais = fa.pais;
                                nf.percent = fa.percent;
                                totales_paises.Add(nf);
                                add++;
                            }
                            else
                            {
                                foreach (Fabricantes f in totales_paises.ToList())
                                {
                                    if (f.pais == fa.pais)
                                    {
                                        f.cantidad = fa.cantidad;
                                        iguales++;
                                    }
                                }
                                if (iguales == 0)
                                {
                                    Fabricantes nf = new Fabricantes();
                                    nf.cantidad = fa.cantidad;
                                    nf.pais = fa.pais;
                                    nf.percent = fa.percent;
                                    totales_paises.Add(nf);
                                }
                                add++;
                            }

                        }
                    }
                    foreach (Fabricantes f in totales_paises) { total_paises += f.cantidad; }
                    foreach (Fabricantes f in totales_paises) { f.porcentaje = (f.cantidad * 100) / total_paises; }
                    List<Fabricantes> totales_paises_envio = new List<Fabricantes>();
                    foreach (Fabricantes f in totales_paises)
                    {
                        Fabricantes nf = new Fabricantes();
                        double x = (((sumas_tallas[sumas_tallas.Length - 2]) * f.cantidad) / total_paises);
                        nf.cantidad = Convert.ToInt32(Math.Round(x));
                        nf.pais = f.pais;
                        nf.percent = f.percent;
                        totales_paises_envio.Add(nf);
                    }
                    foreach (Fabricantes f in totales_paises_envio)
                    {
                        porcentajes.Add(new string[] { f.pais, (f.cantidad).ToString(),f.percent });
                    }
                    ws.Cell(filas, 2).Value = "%";
                    ws.Cell(filas, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    filas++;
                    ws.Cell(filas, 2).Value = porcentajes;
                }
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();

                /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Packing List " + clave_packing + ".xlsx\"");

                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }

                httpResponse.End();

            }
        }

        public void excel_pk_bp(List<Pk> lista)
        {
            // List<Pk> lista = ds.obtener_packing_list(Convert.ToInt32(Session["pk"]));
            string clave_packing = "";
            using (XLWorkbook libro_trabajo = new XLWorkbook())
            {
                int estilos_total = 0, rows = 0, tarima_contador = 0, total_ratio, contador, r, c, total_cajas = 0, filas = 0, columnas = 0, tallas_id, piezas_estilo = 0;

                var ws = libro_trabajo.Worksheets.Add("PK");
                foreach (Pk item in lista){
                    //item.tipo_empaque = 2;                    
                    clave_packing = item.packing;
                    /*****INICIO CON DIRECCIONES, LOGO, ETC******/
                    ws.Cell("A2").Value = "FORTUNE FASHIONS BAJA, S.R.L. DE C.V.";
                    ws.Cell("A3").Value = "CALLE TORTUGA No 313-A";
                    ws.Cell("A4").Value = "MANEADERO CP 22790";
                    ws.Cell("A5").Value = "BAJA CALIFORNIA";
                    ws.Style.Font.FontSize = 11;

                    ws.Range("A2:A10").Style.Font.Bold = true;
                    ws.Range("A7:A10").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    //DIRECCIÓN DE ORIGEN
                    ws.Cell("A7").Value = "CUSTOMER: ";
                    ws.Cell("B7").Value = item.customer;

                    int ex_label = ds.contar_labels(item.id_packing_list);
                    if (ex_label != 0){
                        List<Labels> lista_etiquetas = new List<Labels>();
                        lista_etiquetas = ds.obtener_etiquetas(item.id_packing_list);
                        ws.Cell("A8").Value = "P.O.: ";
                        string label = Regex.Replace(item.pedido, @"\s+", " ") + "(PO# ";
                        foreach (Labels l in lista_etiquetas) { label += " " + l.label; }
                        if (ex_label == 1){
                            label += " )" + " With UCC Labels " + item.parte;
                        }else{
                            label += " )" + " With TPM Labels " + item.parte;
                        }
                        ws.Cell("B8").Value = label;
                    }else{
                        ws.Cell("A8").Value = "P.O.: ";
                        ws.Cell("B8").Value = Regex.Replace(item.pedido, @"\s+", " ") + " Without UCC Labels " + item.parte;
                    }
                    ws.Cell("A9").Value = "RETAILER: ";
                    ws.Cell("B9").Value = item.customer_po;
                    if (item.tipo != "1"){
                        ws.Cell("A10").Value = "EXAMPLES ";
                        ws.Cell("A10").Style.Font.FontSize = 14;
                    }
                    //IMAGEN AL CENTRO
                    ws.Range(1, 7, 1, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    var imagePath = Server.MapPath("/") + "/Content/img/LOGO FORTUNE.png";
                    //var imagePath = @"C:\Users\melissa\source\repos\FortuneSys----\FortuneSystem\Content\img\LOGO FORTUNE.png";
                    var image = ws.AddPicture(imagePath).MoveTo(ws.Cell("E1")).Scale(0.30);
                    //PK ABAJO DE LA IMAGEN
                    ws.Cell("D7").Value = "PK: ";
                    ws.Cell("D7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("D7").Style.Font.Bold = true;
                    ws.Cell("E7").Value = item.packing;
                    ws.Cell("E7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range("E7:F7").Merge();
                    ws.Range("E7", "F7").Style.Font.Bold = true;
                    ws.Range("D7", "F7").Style.Font.FontSize = 15;
                    ws.Range("E7:F7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range("E7:F7").Style.Border.BottomBorderColor = XLColor.Black;
                    //DIRECCION DE DESTINO
                    ws.Cell("L2").Value = "SHIP TO: ";
                    ws.Cell("L3").Value = item.destino.nombre;
                    ws.Cell("L4").Value = item.destino.direccion;
                    ws.Cell("L5").Value = item.destino.ciudad + " " + item.destino.zip;
                    ws.Cell("L8").Value = "DATE:" + item.fecha;
                    ws.Range("L2", "L10").Style.Font.Bold = true;
                    var columna_a = ws.Range("A2", "A10");
                    ws.Rows("6").Height = 30;
                    /****************T*A*B*L*A************************************************/
                    int contador_cabeceras = 0, contador_tallas = 0, pallets = 0, tiendas = 0;
                    //PPK
                    var headers = new List<String[]>();
                    List<String> titulos = new List<string>();//=  "ID", "P.O. NUM", "TYPE", "COLOR", "DESCRIPTION";
                    titulos.Add("ID"); titulos.Add("P.O. NUM"); titulos.Add("TYPE"); titulos.Add("STYLE"); titulos.Add("COLOR"); titulos.Add("DESCRIPTION");
                    foreach (Tarima t in item.lista_tarimas){
                        foreach (estilos e in t.lista_estilos){
                            if (e.store != "N/A" && e.store != "NA") { tiendas++; }
                        }
                    }
                    if (tiendas != 0) { titulos.Add("STORE"); }
                    foreach (Tarima t in item.lista_tarimas){
                        foreach (estilos e in t.lista_estilos){
                            if (contador_cabeceras == 0 && e.tipo != "EXT" && e.tipo != "DMG" && e.tipo != "RPLN" && e.tipo != "ECOM"){
                                foreach (ratio_tallas ra in e.lista_ratio){
                                    titulos.Add(ra.talla);
                                }
                                contador_cabeceras++;
                            }
                        }
                    }
                    titulos.Add("PCS"); titulos.Add("BXS"); titulos.Add("PALLETS");
                    headers.Add(titulos.ToArray());
                    int total_titulos = (titulos.ToArray()).Length;
                    ws.Cell(11, 1).Value = headers;
                    ws.Column(2).AdjustToContents();
                    ws.Column(5).AdjustToContents();
                    for (int i = 1; i <= total_titulos; i++){
                        ws.Cell(11, i).Style.Font.Bold = true;
                        ws.Cell(11, i).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                        ws.Cell(11, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(11, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.LeftBorderColor = XLColor.Black;
                        ws.Cell(11, i).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.RightBorderColor = XLColor.Black;
                        ws.Cell(11, i).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.TopBorderColor = XLColor.Black;
                        ws.Cell(11, i).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.BottomBorderColor = XLColor.Black;
                    }
                    ws.Rows("6").Height = 30;
                    /***********************************************************************************/
                    List<int> tallas_id_temporal = new List<int>();
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        if (contador_tallas == 0)
                        {
                            foreach (estilos e in tarimas.lista_estilos)
                            {
                                if (contador_tallas == 0 && e.tipo != "EXT" && e.tipo != "DMG" && e.tipo != "RPLN" && e.tipo != "ECOM")
                                {
                                    foreach (var ratio in e.lista_ratio)
                                    {
                                        contador_tallas++;
                                    }
                                }
                            }
                        }
                    }
                    int temporal = 0;
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        foreach (estilos e in tarimas.lista_estilos)
                        {
                            if (temporal == 0 && e.tipo != "EXT" && e.tipo != "DMG" && e.tipo != "RPLN" && e.tipo != "ECOM")
                            {
                                foreach (var ratio in e.lista_ratio)
                                {
                                    tallas_id_temporal.Add(ratio.id_talla);
                                }
                                temporal++;
                            }
                        }
                    }
                    //tallas_id_temporal.Add(ratio.id_talla);
                    int[] tallas_comparacion = tallas_id_temporal.ToArray();//ARREGLO DE TALLAS PARA COMPARAR
                    foreach (Tarima tarimas in item.lista_tarimas) { pallets++; } //TOTAL DE TARIMAS
                    int[] sumas_tallas = new int[contador_tallas + 2];
                    for (int i = 0; i < contador_tallas + 2; i++) { sumas_tallas[i] = 0; }//totales de tallas +total piezas +total cajas
                    r = 12;
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        estilos_total = 0; rows = 0; tarima_contador = 0; rows = 0;
                        foreach (estilos estilo in tarimas.lista_estilos) { estilos_total++; }
                        ws.Cell(r, 1).Value = tarimas.id_tarima;
                        ws.Range(r, 1, (r + estilos_total - 1), 1).Merge();
                        var celdas_estilos = new List<String[]>();
                        foreach (estilos estilo in tarimas.lista_estilos)
                        {
                            List<String> datos = new List<string>();
                            datos.Add(Convert.ToString(estilo.number_po));
                            if (estilo.tipo != "NONE") { datos.Add(estilo.tipo); }
                            else { datos.Add(" "); }
                            datos.Add(estilo.estilo);
                            datos.Add(estilo.color);
                            datos.Add(estilo.descripcion);
                            if (tiendas != 0) { datos.Add(estilo.store); }
                            total_ratio = 0;
                            foreach (ratio_tallas ratio in estilo.lista_ratio) { total_ratio++; }//esperar con esto a ver que 
                            contador = 0;
                            int ii = 0, total_talla, total_estilo = 0;
                            if (estilo.tipo == "DMG" || estilo.tipo == "EXT" || estilo.tipo == "RPLN" || estilo.tipo == "ECOM")
                            {
                                foreach (int tall in tallas_comparacion)
                                {
                                    foreach (ratio_tallas ratio in estilo.lista_ratio)
                                    {
                                        if (tall == ratio.id_talla)
                                        {
                                            total_talla = ratio.ratio * estilo.boxes;
                                            sumas_tallas[ii] += total_talla;
                                            total_estilo += total_talla;
                                            datos.Add(Convert.ToString(total_talla));
                                        }
                                        else { datos.Add(" "); }
                                        ii++;
                                    }
                                }
                            }
                            else
                            {
                                foreach (int tall in tallas_comparacion)
                                {
                                    if (tall == estilo.id_talla)
                                    {
                                        total_talla = estilo.boxes;
                                        piezas_estilo = estilo.boxes;
                                        sumas_tallas[ii] += total_talla;
                                        total_estilo += total_talla;
                                        datos.Add(Convert.ToString(total_talla));
                                    }
                                    else { datos.Add(""); }
                                    ii++;
                                }
                            }
                            datos.Add(Convert.ToString(total_estilo));
                            sumas_tallas[ii] += total_estilo; ii++;
                            if (estilo.tipo == "EXT" || estilo.tipo == "DMG" || estilo.tipo == "RPLN" || estilo.tipo == "ECOM")
                            {
                                datos.Add("1");
                                sumas_tallas[ii] += 1;
                            }
                            else
                            {
                                int piezas = ds.buscar_cajas_talla_estilo(estilo.id_po_summary, estilo.id_talla);
                                datos.Add(Convert.ToString((piezas_estilo / piezas)));
                                sumas_tallas[ii] += Convert.ToInt32((piezas_estilo / piezas));
                            }
                            celdas_estilos.Add(datos.ToArray());
                        }//ESTILOS                            
                        ws.Cell(r, 2).Value = celdas_estilos;
                        c = 7;           //COLUMNA DONDE TERMINA LOS DATOS BÁSICOS, LUEGO VIENEN LOS DINÁMICOS Y POR ÚLTIMO LA DE PALLETS             
                        if (tiendas != 0) { c++; }
                        c = c + (contador_tallas + 2);
                        ws.Cell(r, c).Value = "1";
                        ws.Range(r, c, (r + estilos_total - 1), c).Merge();
                        r += estilos_total;
                        columnas = c;
                    }//TARIMAS
                    contador = 0;
                    string descripcion_final = "";
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        if (contador == 0)
                        {
                            foreach (estilos estilo in tarimas.lista_estilos)
                            {
                                descripcion_final += Regex.Replace(estilo.descripcion_final, @"\s+", " ") + " ";
                            }
                            contador++;
                        }
                    }

                    ws.Cell(r, 1).Value = descripcion_final;
                    ws.Cell(r, 1).Style.Font.Bold = true;
                    ws.Cell(r, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(r, 1, r, 4).Merge();
                    ws.Range(r, 1, r, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);


                    ws.Cell(r, 5).Value = "TOTAL";
                    ws.Cell(r, 5).Style.Font.Bold = true;
                    ws.Cell(r, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    c = 6;
                    if (tiendas != 0) { c++; }
                    ws.Range(r, 5, r, c).Merge();
                    ws.Range(r, 5, r, c).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                    c++;

                    for (int i = 0; i < contador_tallas + 2; i++)
                    {
                        ws.Cell(r, c).Value = sumas_tallas[i];
                        ws.Cell(r, c).Style.Font.Bold = true;
                        ws.Cell(r, c).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                        ws.Cell(r, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        c++;
                    }
                    ws.Cell(r, c).Value = pallets;
                    ws.Cell(r, c).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                    ws.Cell(r, c).Style.Font.Bold = true;
                    ws.Cell(r, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    filas = r;

                    for (int i = 11; i <= (filas); i++)
                    {
                        for (int j = 1; j <= columnas; j++)
                        {

                            ws.Cell(i, j).Style.Font.FontSize = 9;
                            ws.Cell(i, j).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            ws.Cell(i, j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(i, j).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.LeftBorderColor = XLColor.Black;
                            ws.Cell(i, j).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.RightBorderColor = XLColor.Black;
                            ws.Cell(i, j).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.TopBorderColor = XLColor.Black;
                            ws.Cell(i, j).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.BottomBorderColor = XLColor.Black;
                        }
                    }
                    //tabla------------------------------------------------------------------------------------------------BPK

                    filas += 2;
                    ws.Range(filas, 1, (filas + 2), 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Range(filas, 1, (filas + 2), 1).Style.Font.Bold = true;
                    ws.Range(filas, 7, (filas + 2), 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Range(filas, 7, (filas + 2), 7).Style.Font.Bold = true;
                    ws.Range(filas, 2, (filas + 2), 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(filas, 8, (filas + 2), 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(filas, 1).Value = "DRIVER NAME:";
                    ws.Cell(filas, 2).Value = item.conductor.driver_name;
                    ws.Cell(filas, 7).Value = "SHIPPING MANAGER:";
                    ws.Cell(filas, 8).Value = item.shipping_manager;
                    ws.Range(filas, 2, filas, 3).Merge();
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorderColor = XLColor.Black;
                    ws.Range(filas, 8, filas, 11).Merge();
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorderColor = XLColor.Black;
                    filas++;
                    ws.Cell(filas, 1).Value = "TRAILER/PLATES:";
                    ws.Cell(filas, 2).Value = item.conductor.tractor + "/" + item.conductor.plates;
                    ws.Cell(filas, 7).Value = "SEAL:";
                    ws.Cell(filas, 8).Value = item.seal;
                    ws.Range(filas, 2, filas, 3).Merge();
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorderColor = XLColor.Black;
                    ws.Range(filas, 8, filas, 11).Merge();
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorderColor = XLColor.Black;
                    filas++;
                    ws.Cell(filas, 1).Value = "CONTAINER/PLATES:";
                    ws.Cell(filas, 2).Value = item.contenedor.eco + "/" + item.contenedor.plates;
                    ws.Cell(filas, 7).Value = "REPLACEMENT:";
                    ws.Cell(filas, 8).Value = item.replacement;
                    ws.Range(filas, 2, filas, 3).Merge();
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorderColor = XLColor.Black;
                    ws.Range(filas, 8, filas, 11).Merge();
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorderColor = XLColor.Black;
                    filas++;
                    ws.Cell(filas, 3).Value = "DOCUMENTO CONTROLADO. ÚNICAMENTE SE PUEDE MODIFICAR POR EL SUPERVISOR DE SHIPPING";
                    ws.Cell(filas, 3).Style.Font.FontColor = XLColor.FromArgb(100, 100, 100);
                    filas += 2;
                    columnas = 2;

                    var porcentajes = new List<String[]>();
                    List<String> p = new List<string>();
                    p.Add("ORIGIN"); p.Add("QTY");
                    porcentajes.Add(p.ToArray());
                    List<Fabricantes> totales_paises_estilo = new List<Fabricantes>();
                    List<Fabricantes> totales_paises = new List<Fabricantes>();
                    int add = 0, total_paises = 0, iguales;
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        totales_paises_estilo = ds.buscar_paises_estilos(tarimas.lista_estilos);
                        foreach (Fabricantes fa in totales_paises_estilo)
                        {
                            iguales = 0;
                            if (add == 0)
                            {
                                Fabricantes nf = new Fabricantes();
                                nf.cantidad = fa.cantidad;
                                nf.pais = fa.pais;
                                totales_paises.Add(nf);
                                add++;
                            }
                            else
                            {
                                foreach (Fabricantes f in totales_paises.ToList())
                                {
                                    if (f.pais == fa.pais)
                                    {
                                        f.cantidad = fa.cantidad;
                                        iguales++;
                                    }
                                }
                                if (iguales == 0)
                                {
                                    Fabricantes nf = new Fabricantes();
                                    nf.cantidad = fa.cantidad;
                                    nf.pais = fa.pais;
                                    totales_paises.Add(nf);
                                }
                                add++;
                            }

                        }
                    }
                    foreach (Fabricantes f in totales_paises) { total_paises += f.cantidad; }
                    foreach (Fabricantes f in totales_paises) { f.porcentaje = (f.cantidad * 100) / total_paises; }
                    List<Fabricantes> totales_paises_envio = new List<Fabricantes>();
                    foreach (Fabricantes f in totales_paises)
                    {
                        Fabricantes nf = new Fabricantes();
                        double x = (((sumas_tallas[sumas_tallas.Length - 2]) * f.cantidad) / total_paises);
                        nf.cantidad = Convert.ToInt32(Math.Round(x));
                        nf.pais = f.pais;
                        totales_paises_envio.Add(nf);
                    }
                    foreach (Fabricantes f in totales_paises_envio)
                    {
                        porcentajes.Add(new string[] { f.pais, (f.cantidad).ToString() });
                    }
                    ws.Cell(filas, 2).Value = "%";
                    ws.Cell(filas, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    filas++;
                    ws.Cell(filas, 2).Value = porcentajes;
                }
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();
                /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Packing List " + clave_packing + ".xlsx\"");

                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }

                httpResponse.End();

            }
        }

        public void excel_pk_assort(List<Pk> lista)
        {
            // List<Pk> lista = ds.obtener_packing_list(Convert.ToInt32(Session["pk"]));
            string clave_packing = "";
            using (XLWorkbook libro_trabajo = new XLWorkbook())
            {
                int estilos_total = 0, rows = 0, tarima_contador = 0, total_ratio, contador, r, c, total_cajas = 0, filas = 0, columnas = 0, tallas_id, piezas_estilo = 0;
                string tipo = "";
                var ws = libro_trabajo.Worksheets.Add("PK");
                foreach (Pk item in lista)
                {
                    clave_packing = item.packing;
                    /*****INICIO CON DIRECCIONES, LOGO, ETC******/
                    ws.Cell("A2").Value = "FORTUNE FASHIONS BAJA, S.R.L. DE C.V.";
                    ws.Cell("A3").Value = "CALLE TORTUGA No 313-A";
                    ws.Cell("A4").Value = "MANEADERO CP 22790";
                    ws.Cell("A5").Value = "BAJA CALIFORNIA";
                    ws.Style.Font.FontSize = 11;

                    ws.Range("A2:A10").Style.Font.Bold = true;
                    ws.Range("A7:A10").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    //DIRECCIÓN DE ORIGEN
                    ws.Cell("A7").Value = "CUSTOMER: ";
                    ws.Cell("B7").Value = item.customer;

                    int ex_label = ds.contar_labels(item.id_packing_list);
                    if (ex_label != 0)
                    {
                        List<Labels> lista_etiquetas = new List<Labels>();
                        lista_etiquetas = ds.obtener_etiquetas(item.id_packing_list);
                        ws.Cell("A8").Value = "P.O.: ";
                        string label = Regex.Replace(item.pedido, @"\s+", " ") + "(PO# ";
                        foreach (Labels l in lista_etiquetas) { label += " " + l.label; }
                        if (ex_label == 1)
                        {
                            label += " )" + " With UCC Labels " + item.parte;
                        }
                        else
                        {
                            label += " )" + " With TPM Labels " + item.parte;
                        }
                        ws.Cell("B8").Value = label;
                    }
                    else
                    {
                        ws.Cell("A8").Value = "P.O.: ";
                        ws.Cell("B8").Value = Regex.Replace(item.pedido, @"\s+", " ") + " Without UCC Labels " + item.parte;
                    }
                    ws.Cell("A9").Value = "RETAILER: ";
                    ws.Cell("B9").Value = item.customer_po;
                    if (item.tipo != "1")
                    {
                        ws.Cell("A10").Value = "EXAMPLES ";
                        ws.Cell("A10").Style.Font.FontSize = 14;
                    }

                    //IMAGEN AL CENTRO
                    ws.Range(1, 7, 1, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    var imagePath = Server.MapPath("/") + "/Content/img/LOGO FORTUNE.png";
                    //var imagePath = @"C:\Users\melissa\source\repos\karen\FortuneSystem\Content\img\LOGO FORTUNE.png";
                    var image = ws.AddPicture(imagePath).MoveTo(ws.Cell("E1")).Scale(0.30);
                    //PK ABAJO DE LA IMAGEN
                    ws.Cell("D7").Value = "PK: ";
                    ws.Cell("D7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("D7").Style.Font.Bold = true;
                    ws.Cell("E7").Value = item.packing;
                    ws.Cell("E7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range("E7:F7").Merge();
                    ws.Range("E7", "F7").Style.Font.Bold = true;
                    ws.Range("D7", "F7").Style.Font.FontSize = 15;
                    ws.Range("E7:F7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range("E7:F7").Style.Border.BottomBorderColor = XLColor.Black;
                    //DIRECCION DE DESTINO
                    ws.Cell("L2").Value = "SHIP TO: ";
                    ws.Cell("L3").Value = item.destino.nombre;
                    ws.Cell("L4").Value = item.destino.direccion;
                    ws.Cell("L5").Value = item.destino.ciudad + " " + item.destino.zip;
                    ws.Cell("L8").Value = "DATE:" + item.fecha;
                    ws.Range("L2", "L10").Style.Font.Bold = true;
                    var columna_a = ws.Range("A2", "A10");
                    ws.Rows("6").Height = 30;
                    /****************T*A*B*L*A************************************************/
                    int contador_cabeceras = 0, contador_tallas = 0, pallets = 0, tiendas = 0;
                    //PPK
                    var headers = new List<String[]>();
                    List<String> titulos = new List<string>();//=  "ID", "P.O. NUM", "TYPE", "COLOR", "DESCRIPTION";
                    titulos.Add("ID"); titulos.Add("P.O. NUM"); titulos.Add("TYPE"); titulos.Add("ASSORTMENT"); titulos.Add("STYLE"); titulos.Add("COLOR"); titulos.Add("DESCRIPTION");
                    foreach (Tarima t in item.lista_tarimas)
                    {
                        foreach (Assortment a in t.lista_assortments)
                        {
                            foreach (estilos e in a.lista_estilos)
                            {
                                if (e.store != "N/A" && e.store != "NA") { tiendas++; }
                            }
                        }

                    }
                    if (tiendas != 0) { titulos.Add("STORE"); }
                    foreach (Tarima t in item.lista_tarimas)
                    {
                        foreach (Assortment a in t.lista_assortments)
                        {
                            foreach (estilos e in a.lista_estilos)
                            {
                                if (contador_cabeceras == 0 && e.tipo != "EXT" && e.tipo != "DMG" && e.tipo != "RPLN" && e.tipo != "ECOM")
                                {
                                    foreach (ratio_tallas ra in e.lista_ratio) { titulos.Add(ra.talla); }
                                    contador_cabeceras++;
                                }
                            }
                        }
                    }
                    titulos.Add("PCS"); titulos.Add("BXS"); titulos.Add("PALLETS");
                    headers.Add(titulos.ToArray());
                    int total_titulos = (titulos.ToArray()).Length;
                    ws.Cell(11, 1).Value = headers;
                    for (int i = 1; i <= total_titulos; i++)
                    {
                        ws.Cell(11, i).Style.Font.Bold = true;
                        ws.Cell(11, i).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                        ws.Cell(11, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(11, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.LeftBorderColor = XLColor.Black;
                        ws.Cell(11, i).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.RightBorderColor = XLColor.Black;
                        ws.Cell(11, i).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.TopBorderColor = XLColor.Black;
                        ws.Cell(11, i).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.BottomBorderColor = XLColor.Black;
                    }
                    ws.Rows("6").Height = 30;
                    /***********************************************************************************/
                    List<int> tallas_id_temporal = new List<int>();
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        foreach (Assortment a in tarimas.lista_assortments)
                        {
                            if (contador_tallas == 0)
                            {
                                foreach (estilos e in a.lista_estilos)
                                {
                                    if (contador_tallas == 0 && e.tipo != "EXT" && e.tipo != "DMG" && e.tipo != "RPLN" && e.tipo != "ECOM")
                                    {
                                        foreach (var ratio in e.lista_ratio) { contador_tallas++; }
                                    }
                                }
                            }
                        }
                    }
                    int temporal = 0;
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        foreach (Assortment a in tarimas.lista_assortments)
                        {
                            foreach (estilos e in a.lista_estilos)
                            {
                                if (temporal == 0 && e.tipo != "EXT" && e.tipo != "DMG" && e.tipo != "RPLN" && e.tipo != "ECOM")
                                {
                                    foreach (var ratio in e.lista_ratio) { tallas_id_temporal.Add(ratio.id_talla); }
                                    temporal++;
                                }
                            }
                        }
                    }
                    //tallas_id_temporal.Add(ratio.id_talla);
                    int[] tallas_comparacion = tallas_id_temporal.ToArray();//ARREGLO DE TALLAS PARA COMPARAR
                    foreach (Tarima tarimas in item.lista_tarimas) { pallets++; } //TOTAL DE TARIMAS
                    int[] sumas_tallas = new int[contador_tallas + 2];
                    for (int i = 0; i < contador_tallas + 2; i++) { sumas_tallas[i] = 0; }//totales de tallas +total piezas +total cajas
                    r = 12;
                    int assort = 0;
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        int extras = 0;
                        foreach (Assortment a in tarimas.lista_assortments)
                        {
                            estilos_total = 0; rows = 0; tarima_contador = 0; rows = 0;
                            foreach (estilos estilo in a.lista_estilos) { estilos_total++; }
                            ws.Cell(r, 1).Value = tarimas.id_tarima;
                            ws.Range(r, 1, (r + estilos_total - 1), 1).Merge();
                            ws.Range(r, 4, (r + estilos_total - 1), 4).Merge();

                            var celdas_estilos = new List<String[]>();
                            int ii = 0;
                            foreach (estilos estilo in a.lista_estilos)
                            {
                                List<String> datos = new List<string>();
                                datos.Add(Convert.ToString(estilo.number_po));
                                if (estilo.tipo != "NONE") { datos.Add(estilo.tipo); }
                                else { datos.Add(" "); }
                                datos.Add(a.nombre);//NOMBRE DEL ASSORT
                                datos.Add(estilo.estilo);
                                datos.Add(estilo.color);
                                datos.Add(estilo.descripcion);
                                if (tiendas != 0) { datos.Add(estilo.store); }
                                total_ratio = 0;
                                foreach (ratio_tallas ratio in estilo.lista_ratio) { total_ratio++; }//esperar con esto a ver que 
                                contador = 0;
                                int total_talla, total_estilo = 0; ii = 0;
                                if (estilo.tipo == "DMG" || estilo.tipo == "EXT" || estilo.tipo == "RPLN" || estilo.tipo == "ECOM")
                                {
                                    foreach (int tall in tallas_comparacion)
                                    {
                                        assort = 0;
                                        foreach (ratio_tallas ratio in estilo.lista_ratio)
                                        {
                                            if (tall == ratio.id_talla)
                                            {
                                                total_talla = ratio.ratio;// * estilo.boxes;
                                                sumas_tallas[ii] += total_talla;
                                                total_estilo += total_talla;
                                                assort = ratio.ratio;
                                                //datos.Add(Convert.ToString(total_talla));
                                            }
                                            //else { datos.Add(" "); }
                                            //ii++;
                                        }
                                        if (assort != 0) { datos.Add(Convert.ToString(assort)); }
                                        else { datos.Add(" "); }
                                        ii++;
                                    }
                                }
                                else
                                {
                                    foreach (int tall in tallas_comparacion)
                                    {
                                        assort = 0;
                                        foreach (ratio_tallas ratio in estilo.lista_ratio)
                                        {
                                            if (tall == ratio.id_talla)
                                            {
                                                total_talla = ratio.ratio * estilo.boxes;
                                                sumas_tallas[ii] += total_talla;
                                                total_estilo += total_talla;
                                                assort = total_talla;
                                            }
                                        }
                                        if (assort != 0) { datos.Add(Convert.ToString(assort)); }
                                        else { datos.Add(" "); }
                                        ii++;
                                    }
                                }
                                datos.Add(Convert.ToString(total_estilo));
                                sumas_tallas[ii] += total_estilo; ii++;
                                tipo = estilo.tipo;
                                if (estilo.tipo == "EXT" || estilo.tipo == "DMG" || estilo.tipo == "RPLN" || estilo.tipo == "ECOM")
                                {
                                    extras++;
                                }
                                else
                                {
                                    extras = 0;
                                }
                                celdas_estilos.Add(datos.ToArray());
                            }//ESTILOS  
                            if (extras != 0)
                            {
                                sumas_tallas[ii] += 1;
                            }
                            else
                            {
                                sumas_tallas[ii] += a.cartones;
                            }
                            ws.Cell(r, 2).Value = celdas_estilos;
                            c = 8;           //COLUMNA DONDE TERMINA LOS DATOS BÁSICOS, LUEGO VIENEN LOS DINÁMICOS Y POR ÚLTIMO LA DE PALLETS  
                            if (tiendas != 0) { c++; }
                            c = c + (contador_tallas + 1);

                            ws.Cell(r, c).Value = a.cartones;
                            ws.Range(r, c, (r + estilos_total - 1), c).Merge();
                            c++;

                            ws.Cell(r, c).Value = "1";
                            ws.Range(r, c, (r + estilos_total - 1), c).Merge();
                            r += estilos_total;
                            columnas = c;
                        }



                    }//TARIMAS
                    contador = 0;
                    string descripcion_final = "";
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        foreach (Assortment a in tarimas.lista_assortments)
                        {
                            if (contador == 0)
                            {
                                foreach (estilos estilo in a.lista_estilos)
                                {
                                    descripcion_final += Regex.Replace(estilo.descripcion_final, @"\s+", " ") + " ";
                                }
                                contador++;
                            }
                        }

                    }

                    ws.Cell(r, 1).Value = descripcion_final;
                    ws.Cell(r, 1).Style.Font.Bold = true;
                    ws.Cell(r, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(r, 1, r, 4).Merge();
                    ws.Range(r, 1, r, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);


                    ws.Cell(r, 5).Value = "TOTAL";
                    ws.Cell(r, 5).Style.Font.Bold = true;
                    ws.Cell(r, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    c = 7;
                    if (tiendas != 0) { c++; }
                    ws.Range(r, 5, r, c).Merge();
                    ws.Range(r, 5, r, c).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                    c++;

                    for (int i = 0; i < contador_tallas + 2; i++)
                    {
                        ws.Cell(r, c).Value = sumas_tallas[i];
                        ws.Cell(r, c).Style.Font.Bold = true;
                        ws.Cell(r, c).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                        ws.Cell(r, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        c++;
                    }
                    ws.Cell(r, c).Value = pallets;
                    ws.Cell(r, c).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                    ws.Cell(r, c).Style.Font.Bold = true;
                    ws.Cell(r, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    filas = r;
                    //ws.Column(1).Width = 30;
                    for (int i = 11; i <= (filas); i++)
                    {
                        for (int j = 1; j <= columnas; j++)
                        {
                            //if (j == 4 || j == 6) { ws.Column(j).Width = 30; }
                            ws.Cell(i, j).Style.Font.FontSize = 9;
                            ws.Cell(i, j).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            ws.Cell(i, j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(i, j).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.LeftBorderColor = XLColor.Black;
                            ws.Cell(i, j).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.RightBorderColor = XLColor.Black;
                            ws.Cell(i, j).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.TopBorderColor = XLColor.Black;
                            ws.Cell(i, j).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.BottomBorderColor = XLColor.Black;
                        }
                    }
                    //tabla------------------------------------------------------------------------------------------------BPK

                    filas += 2;
                    ws.Range(filas, 1, (filas + 2), 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Range(filas, 1, (filas + 2), 1).Style.Font.Bold = true;
                    ws.Range(filas, 7, (filas + 2), 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Range(filas, 7, (filas + 2), 7).Style.Font.Bold = true;
                    ws.Range(filas, 2, (filas + 2), 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(filas, 8, (filas + 2), 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(filas, 1).Value = "DRIVER NAME:";
                    ws.Cell(filas, 2).Value = item.conductor.driver_name;
                    ws.Cell(filas, 7).Value = "SHIPPING MANAGER:";
                    ws.Cell(filas, 8).Value = item.shipping_manager;
                    ws.Range(filas, 2, filas, 3).Merge();
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorderColor = XLColor.Black;
                    ws.Range(filas, 8, filas, 11).Merge();
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorderColor = XLColor.Black;
                    filas++;
                    ws.Cell(filas, 1).Value = "TRAILER/PLATES:";
                    ws.Cell(filas, 2).Value = item.conductor.tractor + "/" + item.conductor.plates;
                    ws.Cell(filas, 7).Value = "SEAL:";
                    ws.Cell(filas, 8).Value = item.seal;
                    ws.Range(filas, 2, filas, 3).Merge();
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorderColor = XLColor.Black;
                    ws.Range(filas, 8, filas, 11).Merge();
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorderColor = XLColor.Black;
                    filas++;
                    ws.Cell(filas, 1).Value = "CONTAINER/PLATES:";
                    ws.Cell(filas, 2).Value = item.contenedor.eco + "/" + item.contenedor.plates;
                    ws.Cell(filas, 7).Value = "REPLACEMENT:";
                    ws.Cell(filas, 8).Value = item.replacement;
                    ws.Range(filas, 2, filas, 3).Merge();
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorderColor = XLColor.Black;
                    ws.Range(filas, 8, filas, 11).Merge();
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorderColor = XLColor.Black;
                    filas++;
                    ws.Cell(filas, 3).Value = "DOCUMENTO CONTROLADO. ÚNICAMENTE SE PUEDE MODIFICAR POR EL SUPERVISOR DE SHIPPING";
                    ws.Cell(filas, 3).Style.Font.FontColor = XLColor.FromArgb(100, 100, 100);
                    filas += 2;
                    columnas = 2;

                    var porcentajes = new List<String[]>();
                    List<String> p = new List<string>();
                    p.Add("ORIGIN"); p.Add("QTY");
                    porcentajes.Add(p.ToArray());
                    List<Fabricantes> totales_paises_estilo = new List<Fabricantes>();
                    List<Fabricantes> totales_paises = new List<Fabricantes>();
                    int add = 0, total_paises = 0, iguales;
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        foreach (Assortment a in tarimas.lista_assortments)
                        {
                            totales_paises_estilo = ds.buscar_paises_estilos(a.lista_estilos);
                            foreach (Fabricantes fa in totales_paises_estilo)
                            {
                                iguales = 0;
                                if (add == 0)
                                {
                                    Fabricantes nf = new Fabricantes();
                                    nf.cantidad = fa.cantidad;
                                    nf.pais = fa.pais;
                                    totales_paises.Add(nf);
                                    add++;
                                }
                                else
                                {
                                    foreach (Fabricantes f in totales_paises.ToList())
                                    {
                                        if (f.pais == fa.pais)
                                        {
                                            f.cantidad = fa.cantidad;
                                            iguales++;
                                        }
                                    }
                                    if (iguales == 0)
                                    {
                                        Fabricantes nf = new Fabricantes();
                                        nf.cantidad = fa.cantidad;
                                        nf.pais = fa.pais;
                                        totales_paises.Add(nf);
                                    }
                                    add++;
                                }

                            }
                        }

                    }
                    foreach (Fabricantes f in totales_paises) { total_paises += f.cantidad; }
                    foreach (Fabricantes f in totales_paises) { f.porcentaje = (f.cantidad * 100) / total_paises; }
                    List<Fabricantes> totales_paises_envio = new List<Fabricantes>();
                    foreach (Fabricantes f in totales_paises)
                    {
                        Fabricantes nf = new Fabricantes();
                        double x = (((sumas_tallas[sumas_tallas.Length - 2]) * f.cantidad) / total_paises);
                        nf.cantidad = Convert.ToInt32(Math.Round(x));
                        nf.pais = f.pais;
                        totales_paises_envio.Add(nf);
                    }
                    foreach (Fabricantes f in totales_paises_envio)
                    {
                        porcentajes.Add(new string[] { f.pais, (f.cantidad).ToString() });
                    }
                    ws.Cell(filas, 2).Value = "%";
                    ws.Cell(filas, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    filas++;
                    ws.Cell(filas, 2).Value = porcentajes;
                }
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();
                /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Packing List " + clave_packing + ".xlsx\"");

                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }

                httpResponse.End();

            }
        }

        public JsonResult buscar_informacion_edicion_pk(string pk)
        {
            return Json(ds.obtener_informacion_editar_pk(Convert.ToInt32(pk)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult guardar_informacion_edicion_pk(string id, string sello, string replacement, string conductor, string contenedor)
        {
            ds.actualizar_datos_pk(id, sello, replacement, conductor, contenedor);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        //****************************************************************************************************************************************************************
        //*********REPORTES***********************************************************************************************************************************************
        //****************************************************************************************************************************************************************
        //string fecha_inicio, fecha_final;
        public JsonResult fechas_reporte(string inicio, string final){
            Session["fechas"] = inicio + "*" + final;
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public void excel_reporte_po_abierto(){
            string[] fechas = (Convert.ToString(Session["fechas"])).Split('*');
            
            List<Pk> lista = ds.obtener_pedido_cantidades(fechas[0], fechas[1]);
            int row = 1, column = 2, suma_totales_talla = 0, total_talla, suma_estilo, existe_talla, total_cabeceras = 4;
            using (XLWorkbook libro_trabajo = new XLWorkbook()){
                var ws = libro_trabajo.Worksheets.Add("Open orders");
                ws.Cell(row, column).Value = "OPEN WIP";
                ws.Cell(row, column).Style.Font.FontSize = 22;
                ws.Cell(row, column).Style.Font.Bold= true;
                ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);
                row++;

                foreach (Pk p in lista){
                    total_cabeceras = 4;
                    List<string> tallas_letras = new List<string>();
                    List<Int32> talla_tempo = new List<Int32>();
                    int[] tallas_ids;
                    var headers = new List<String[]>();
                    List<String> titulos = new List<string>();
                    titulos.Add("CANCEL DATE"); titulos.Add("STYLE"); titulos.Add("ITEM DESCRIPTION"); titulos.Add("COLOR");
                    foreach (estilos e in p.lista_estilos){
                        foreach (ratio_tallas r in e.lista_ratio){
                            bool isEmpty = !tallas_letras.Any();
                            if (isEmpty){
                                talla_tempo.Add(r.id_talla);
                                tallas_letras.Add(r.talla);
                            }else{
                                existe_talla = 0;
                                foreach (string s in tallas_letras){
                                    if (s == r.talla){
                                        existe_talla++;
                                    }
                                }
                                if (existe_talla == 0){
                                    talla_tempo.Add(r.id_talla);
                                    tallas_letras.Add(r.talla);
                                }
                            }
                        }
                    }
                    foreach (string s in tallas_letras){
                        titulos.Add(s);
                        total_cabeceras++;
                    }
                    tallas_ids = talla_tempo.ToArray();
                    titulos.Add("(+/-)"); total_cabeceras++;
                    headers.Add(titulos.ToArray());
                   
                    int tempo = 0; suma_totales_talla = 0;
                    foreach (estilos e in p.lista_estilos){
                        tempo++;
                        foreach (ratio_tallas r in e.lista_ratio){
                            suma_totales_talla += r.total_talla;
                        }
                    }
                    if (tempo != 0) {
                        if (suma_totales_talla >= 5) {
                            row++;
                            ws.Cell(row, 2).Value = p.pedido;
                            ws.Cell(row, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);
                            ws.Cell(row, 2).Style.Font.Bold = true;
                            ws.Cell(row, 2).Style.Font.FontSize = 14;
                            row++;
                            ws.Cell(row, 1).Value = headers;
                            ws.Range(row, 1, row, total_cabeceras).Style.Font.Bold = true;
                            ws.Range(row, 1, row, total_cabeceras).Style.Font.FontSize = 14;
                            ws.Range(row, 1, row, total_cabeceras).Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);
                            row++;
                        }                        
                    }
                    suma_totales_talla = 0;
                    foreach (estilos e in p.lista_estilos){
                        foreach (ratio_tallas r in e.lista_ratio){
                            suma_totales_talla += r.total_talla;
                        }
                        if (suma_totales_talla >= 5) {                            
                            var celdas_estilos = new List<String[]>();
                            List<String> datos = new List<string>();
                            datos.Add(p.cancel_date);
                            datos.Add(e.estilo);
                            datos.Add(e.descripcion);
                            datos.Add(e.color);
                            suma_estilo = 0;
                            foreach (int i in tallas_ids){
                                total_talla = 0;
                                foreach (ratio_tallas r in e.lista_ratio){
                                    if (r.id_talla == i){
                                        total_talla = r.total_talla;
                                    }
                                }
                                suma_estilo += total_talla;
                                if (total_talla == 0){
                                    datos.Add("0");
                                }else{
                                    datos.Add("-" + (total_talla).ToString());
                                }
                            }
                            datos.Add("-" + (suma_estilo).ToString());
                            celdas_estilos.Add(datos.ToArray());
                            ws.Cell(row, 1).Value = celdas_estilos;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.LeftBorderColor = XLColor.White;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.RightBorderColor = XLColor.White;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.TopBorderColor = XLColor.White;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.BottomBorderColor = XLColor.White;
                            row++;
                        }
                       
                    }
                    
                }
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();
                ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Open orders " + fechas[0] + "-" + fechas[1] + ".xlsx\"");
                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream()){
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }
                httpResponse.End();
            }
        }
        public JsonResult po_reporte(string po){
            Session["po_reporte"] = po ;
            return Json("", JsonRequestBehavior.AllowGet);
        }      
        public void excel_reporte_status(){
            string pedido = Convert.ToString(Session["po_reporte"]);
            int id_pedido = consultas.buscar_pedido(pedido);
            List<Estilo_Pedido> lista_estilos = ds.obtener_estilos_pedido_status(id_pedido);
            int row = 1, aux,cabeceras;
            using (XLWorkbook libro_trabajo = new XLWorkbook()){ //Regex.Replace(pedido, @"\s+", " "); 
                var ws = libro_trabajo.Worksheets.Add("Report");

                ws.Cell(row, 3).Value = Regex.Replace("ORDER "+pedido, @"\s+", " "); 
                ws.Cell(row, 3).Style.Font.FontSize = 22;
                ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);
                row+=2;
                foreach (Estilo_Pedido ep in lista_estilos){
                    cabeceras = 7;
                    List<Int32> id_tallas_tempo = new List<Int32>();
                    List<Int32> cantidades_tallas_tempo = new List<Int32>();
                    List<string> tallas_tempo = new List<string>();
                    foreach (Talla t in ep.totales_pedido){
                        bool isEmpty = !id_tallas_tempo.Any();
                        if (isEmpty){
                            id_tallas_tempo.Add(t.id_talla);
                            tallas_tempo.Add(t.talla);
                            cantidades_tallas_tempo.Add(t.total);
                            cabeceras++;
                        }else{
                            aux = 0;
                            foreach (int i in id_tallas_tempo){
                                if (i == t.id_talla) { aux++; }
                            }
                            if (aux == 0){
                                id_tallas_tempo.Add(t.id_talla);
                                tallas_tempo.Add(t.talla);
                                cantidades_tallas_tempo.Add(t.total);
                                cabeceras++;
                            }
                        }
                    }//OBTENER TALLAS DE EL ESTILO
                    var headers = new List<String[]>();
                    List<String> titulos = new List<string>();
                    titulos.Add("ID"); titulos.Add("STYLE"); titulos.Add("ITEM DESCRIPTION"); titulos.Add("COLOR"); titulos.Add("TYPE");
                    int titulo_c = 0;
                    foreach (string s in tallas_tempo) { titulo_c++; titulos.Add(s); }
                    titulos.Add("PACKING"); titulos.Add("DATE");
                    if (titulo_c != 0) { 
                        headers.Add(titulos.ToArray());
                        ws.Cell(row, 1).Value = headers;
                        ws.Range(row,1,row, cabeceras).Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);
                        ws.Range(row, 1, row, cabeceras).Style.Font.FontSize = 13;
                        ws.Range(row, 1, row, cabeceras).Style.Font.Bold = true;
                        row++; //AGHREGAR CABECERA TABLA
                        //AGREGAR ROWS DE INFORMACIÓN POR PK DE CADA ESTILO
                        foreach (Packing_Estilo pe in ep.lista_pk){
                            var celdas = new List<String[]>();
                            List<String> datos = new List<string>();
                            datos.Add((ep.id_estilo).ToString());
                            datos.Add(ep.estilo);
                            datos.Add(ep.descripcion);
                            datos.Add(ep.color);
                            datos.Add(pe.tipo);
                            int j = 0;
                            foreach (int i in id_tallas_tempo){
                                aux = 0;
                                foreach (Talla te in pe.lista_enviados){
                                    if (te.id_talla == i){
                                        aux = te.total;
                                    }
                                }
                                if (aux == 0){
                                    datos.Add(" ");
                                }else{
                                    datos.Add((aux).ToString());
                                    cantidades_tallas_tempo[j] = cantidades_tallas_tempo[j] - aux;
                                }
                                j++;
                            }
                            datos.Add(pe.package);
                            datos.Add(pe.fecha);
                            celdas.Add(datos.ToArray());
                            ws.Cell(row, 1).Value = celdas;
                            ws.Range(row, 1, row, cabeceras).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, cabeceras).Style.Border.LeftBorderColor = XLColor.White;
                            ws.Range(row, 1, row, cabeceras).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, cabeceras).Style.Border.RightBorderColor = XLColor.White;
                            ws.Range(row, 1, row, cabeceras).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, cabeceras).Style.Border.TopBorderColor = XLColor.White;
                            ws.Range(row, 1, row, cabeceras).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, cabeceras).Style.Border.BottomBorderColor = XLColor.White;
                            row++;                            
                        }

                        var celdas2 = new List<String[]>();
                        List<String> datos2 = new List<string>();
                        datos2.Add("+/-");
                        foreach (int i in cantidades_tallas_tempo){
                            if (i < 0){
                                datos2.Add("+" + (i * -1).ToString());
                            }else{
                                datos2.Add("-" + i.ToString());
                            }
                        }
                        celdas2.Add(datos2.ToArray());
                        ws.Cell(row, 5).Value = celdas2;
                        ws.Range(row, 1, row, cabeceras).Style.Font.Bold = true;
                        ws.Range(row, 1, row, cabeceras).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                        ws.Range(row, 1, row, cabeceras).Style.Border.LeftBorderColor = XLColor.White;
                        ws.Range(row, 1, row, cabeceras).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                        ws.Range(row, 1, row, cabeceras).Style.Border.RightBorderColor = XLColor.White;
                        ws.Range(row, 1, row, cabeceras).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                        ws.Range(row, 1, row, cabeceras).Style.Border.TopBorderColor = XLColor.White;
                        ws.Range(row, 1, row, cabeceras).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                        ws.Range(row, 1, row, cabeceras).Style.Border.BottomBorderColor = XLColor.White;
                        row++; row++;
                    }
                }
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();                
                ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
    /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Order status " +pedido+ ".xlsx\"");
                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream()){
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }
                httpResponse.End();
            }
        }
        public JsonResult estilo_reporte(string estilo){
            Session["estilo_reporte"] = estilo;
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public void excel_reporte_por_estilos(){
            string estilo = Convert.ToString(Session["estilo_reporte"]);
            int id_estilo = consultas.obtener_estilo_id(estilo);
            List<Estilo_PO> lista_estilos = ds.obtener_pedidos_po_estilo(id_estilo);
            int row = 1;
            using (XLWorkbook libro_trabajo = new XLWorkbook()){ //Regex.Replace(pedido, @"\s+", " "); 
                var ws = libro_trabajo.Worksheets.Add("Report");
                ws.Cell(row, 2).Value = Regex.Replace("STYLE: "+estilo, @"\s+", " ");
                ws.Cell(row, 2).Style.Font.FontSize = 22;
                ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromArgb(196, 215, 155);
                row++;
                //CABECERAS TABLA
                var headers = new List<String[]>();
                List<String> titulos = new List<string>();
                titulos.Add("PO"); titulos.Add("STYLE"); titulos.Add("ITEM DESCRIPTION"); titulos.Add("COLOR"); titulos.Add("PCS"); titulos.Add("STATUS");
                headers.Add(titulos.ToArray());
                ws.Cell(row, 1).Value = headers;
                ws.Range(row, 1,row,6).Style.Fill.BackgroundColor = XLColor.FromArgb(196, 215, 155);
                ws.Range(row, 1, row, 6).Style.Font.Bold = true;
                row++; //AGHREGAR CABECERA TABLA
                foreach (Estilo_PO e in lista_estilos) {
                    var celdas = new List<String[]>();
                    List<String> datos = new List<string>();
                    datos.Add(e.pedido);
                    datos.Add(e.estilo);
                    datos.Add(e.descripcion);
                    datos.Add(e.color);
                    if (e.total < 0){
                        datos.Add((e.total * -1).ToString());
                    }else{
                        datos.Add(e.total.ToString());
                    }
                    //datos.Add((e.total).ToString());
                    datos.Add(e.estado);
                    celdas.Add(datos.ToArray());
                    ws.Cell(row, 1).Value = celdas;
                    ws.Range(row, 1, row, 6).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range(row, 1, row, 6).Style.Border.LeftBorderColor = XLColor.White;
                    ws.Range(row, 1, row, 6).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range(row, 1, row, 6).Style.Border.RightBorderColor = XLColor.White;
                    ws.Range(row, 1, row, 6).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range(row, 1, row, 6).Style.Border.TopBorderColor = XLColor.White;
                    ws.Range(row, 1, row, 6).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                    ws.Range(row, 1, row, 6).Style.Border.BottomBorderColor = XLColor.White;
                    row++;
                }              
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();
                ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Orders with " + estilo + ".xlsx\"");
                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream()){
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }
                httpResponse.End();

            }

        }
        public JsonResult year_report(string year){
            Session["year_reporte"] = year;
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public void listado_year() {
            string year = Convert.ToString(Session["year_reporte"]);
            List<Shipping_pk> lista_pk = ds.obtener_listado_packing_year(year);
            excel_listado_packing(lista_pk, "SHIPMENT REPORTS "+year);
        }
        public void listado_diario(){
            List<Shipping_pk> lista_pk = ds.obtener_listado_packing_diario();
            excel_listado_packing(lista_pk,"SHIPMENT REPORTS "+DateTime.Now.ToString("yyyy-MM-dd"));
        }
        public void excel_listado_packing(List<Shipping_pk> lista_pk,string titulo){
            //string year = Convert.ToString(Session["year_reporte"]);
            //List<Shipping_pk> lista_pk = ds.obtener_listado_packing_year(year);
            int row = 1, column = 1;
            using (XLWorkbook libro_trabajo = new XLWorkbook()){ //Regex.Replace(pedido, @"\s+", " "); 
                var ws = libro_trabajo.Worksheets.Add("Report");
                ws.Cell(row, column).Value = titulo;
                ws.Row(1).Style.Font.FontSize = 20;
                ws.Range(1, 1, 1, 7).Merge();
                ws.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Row(1).Style.Fill.BackgroundColor = XLColor.FromArgb(196, 215, 155);
                ws.Row(1).Style.Font.Bold = true;
                row++;
                //CABECERAS TABLA
                var headers = new List<String[]>();
                List<String> titulos = new List<string>();
                titulos.Add("PK"); titulos.Add("PO"); titulos.Add("SHIP TO"); titulos.Add("PCS"); titulos.Add("BXS"); titulos.Add("PALLETS");titulos.Add("# SHIPPING");
                headers.Add(titulos.ToArray());
                ws.Cell(row, 1).Value = headers;
                ws.Row(row).Style.Font.FontSize = 13;
                ws.Row(row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromArgb(196, 215, 155);
                ws.Row(row).Style.Font.Bold = true;
                row++; //AGHREGAR CABECERA TABLA
                foreach (Shipping_pk e in lista_pk){
                    var celdas = new List<String[]>();
                    List<String> datos = new List<string>();
                    datos.Add(e.packing);
                    datos.Add(e.pedido);
                    datos.Add(e.destino);
                    datos.Add((e.piezas).ToString());
                    datos.Add((e.cajas).ToString());
                    datos.Add((e.pallets).ToString());
                    datos.Add((e.num_envio).ToString());
                    celdas.Add(datos.ToArray());
                    ws.Cell(row, 1).Value = celdas;
                    ws.Range(row, 1, row, 7).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range(row, 1, row, 7).Style.Border.LeftBorderColor = XLColor.White;
                    ws.Range(row, 1, row, 7).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range(row, 1, row, 7).Style.Border.RightBorderColor = XLColor.White;
                    ws.Range(row, 1, row, 7).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range(row, 1, row, 7).Style.Border.TopBorderColor = XLColor.White;
                    ws.Range(row, 1, row, 7).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                    ws.Range(row, 1, row, 7).Style.Border.BottomBorderColor = XLColor.White;
                    row++;
                }
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();
                ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Shipping Report.xlsx\"");
                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }
                httpResponse.End();

            }

        }
        public void excel_reporte_status_orden(){
            string pedido = Convert.ToString(Session["po_reporte"]);
            int id_pedido = consultas.buscar_pedido(pedido);
            List<Pk> lista = ds.obtener_pedido_cantidades_orden(id_pedido);
            int row = 1, column = 1, suma_totales_talla = 0, total_talla, suma_estilo, existe_talla, total_cabeceras ;
            using (XLWorkbook libro_trabajo = new XLWorkbook()){
                var ws = libro_trabajo.Worksheets.Add("Report");
                List<Int32> talla_tempo = new List<Int32>();
                int[] tallas_ids;
                
                foreach (Pk p in lista){
                    total_cabeceras = 5;
                    List<string> tallas_letras = new List<string>();
                    var headers = new List<String[]>();
                    List<String> titulos = new List<string>();
                    titulos.Add("CANCEL DATE"); titulos.Add("STYLE"); titulos.Add("ITEM DESCRIPTION"); titulos.Add("COLOR");
                    foreach (estilos e in p.lista_estilos){
                        foreach (ratio_tallas r in e.lista_ratio){
                            bool isEmpty = !tallas_letras.Any();
                            if (isEmpty){
                                talla_tempo.Add(r.id_talla);
                                tallas_letras.Add(r.talla);
                            }else{
                                existe_talla = 0;
                                foreach (string s in tallas_letras){
                                    if (s == r.talla){
                                        existe_talla++;
                                    }
                                }
                                if (existe_talla == 0){
                                    talla_tempo.Add(r.id_talla);
                                    tallas_letras.Add(r.talla);
                                }
                            }
                        }
                    }
                    foreach (string s in tallas_letras){
                        titulos.Add(s);
                        total_cabeceras++;
                    }
                    tallas_ids = talla_tempo.ToArray();
                    titulos.Add("(+/-)");
                    headers.Add(titulos.ToArray());
                    row++;
                    ws.Cell(row, 2).Value ="MISSING PIECES "+ p.pedido;
                    ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromArgb(196, 215, 155);
                    ws.Row(row).Style.Font.Bold = true;
                    ws.Row(row).Style.Font.FontSize = 14;
                    row++;
                    ws.Cell(row, 1).Value = headers;
                    ws.Range(row, 1, row, total_cabeceras).Style.Font.Bold = true;
                    ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromArgb(196, 215, 155);
                    ws.Range(row, 1, row, total_cabeceras).Style.Font.FontSize = 14;
                    row++;
                    foreach (estilos e in p.lista_estilos){
                        foreach (ratio_tallas r in e.lista_ratio){
                            suma_totales_talla += r.total_talla;
                        }
                        //if (suma_totales_talla >= 1){
                            var celdas_estilos = new List<String[]>();
                            List<String> datos = new List<string>();
                            datos.Add(p.cancel_date);
                            datos.Add(e.estilo);
                            datos.Add(e.descripcion);
                            datos.Add(e.color);
                            suma_estilo = 0;
                            foreach (int i in tallas_ids){
                                total_talla = 0;
                                foreach (ratio_tallas r in e.lista_ratio){
                                    if (r.id_talla == i){
                                        total_talla = r.total_talla;
                                    }
                                }
                                suma_estilo += total_talla;
                                if (total_talla == 0){
                                    datos.Add("0");
                                }else{
                                    datos.Add("-" + (total_talla).ToString());
                                }
                            }
                            datos.Add("-" + (suma_estilo).ToString());
                            celdas_estilos.Add(datos.ToArray());
                            ws.Cell(row, 1).Value = celdas_estilos;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.LeftBorderColor = XLColor.White;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.RightBorderColor = XLColor.White;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.TopBorderColor = XLColor.White;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.BottomBorderColor = XLColor.White;
                        //}
                        row++;
                    }                    
                    ws.Cell(row, 2).Value = "TOTAL PO";
                    ws.Cell(row, 3).Value = p.total_po;
                    ws.Cell(row, 4).Value = "SHIPPED";
                    ws.Cell(row, 5).Value = p.total_enviado;
                    ws.Cell(row, 6).Value = "TOTAL";
                    if (p.total_po - p.total_enviado > 0){
                        ws.Cell(row, 7).Value = "-" + (p.total_po - p.total_enviado).ToString();
                    }else {
                        ws.Cell(row, 7).Value = ((p.total_po - p.total_enviado)*-1).ToString();
                    }
                    
                    ws.Range(row,1,row,10).Style.Font.FontSize = 13;
                    ws.Row(row).Style.Font.Bold = true;
                    ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromArgb(196, 215, 155);
                    row++;
                }
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();
                ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Open orders " + pedido + ".xlsx\"");
                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream()){
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }
                httpResponse.End();
            }
        }
        
        public void estados_pedidos(List<Shipping_pk> lista_pk, string titulo){
            //string year = Convert.ToString(Session["year_reporte"]);
            List<Po> lista_pedidos = ds.obtener_lista_pedidos();
            int row = 1, column = 1;
            using (XLWorkbook libro_trabajo = new XLWorkbook())
            { //Regex.Replace(pedido, @"\s+", " "); 
                var ws = libro_trabajo.Worksheets.Add("Report");
                
                //CABECERAS TABLA
                var headers = new List<String[]>();
                List<String> titulos = new List<string>();
                titulos.Add("PO"); titulos.Add("CUSTOMER PO"); titulos.Add("STYLE"); titulos.Add("CANCEL DATE"); titulos.Add("TOTAL UNITS"); titulos.Add("STATUS"); titulos.Add("CUSTOMER");
                headers.Add(titulos.ToArray());
                ws.Cell(row, 1).Value = headers;
                ws.Range(row,1,row,7).Style.Fill.BackgroundColor = XLColor.FromArgb(196, 215, 155);
                row++; //AGREGAR CABECERA TABLA
                foreach (Po e in lista_pedidos){
                    var celdas = new List<String[]>();
                    List<String> datos = new List<string>();
                    datos.Add(e.pedido);
                    datos.Add(e.customer_po);
                    datos.Add((e.estilos).ToString());
                    datos.Add(e.fecha_cancelacion);
                    datos.Add((e.total).ToString());
                    datos.Add(e.estado);
                    datos.Add(e.customer);
                    celdas.Add(datos.ToArray());
                    ws.Cell(row, 1).Value = celdas;
                    if (e.estado == "CANCELLED") {
                        ws.Row(row).Style.Font.FontColor= XLColor.FromArgb(150, 54, 52);
                    }
                    if (e.estado == "COMPLETED") {
                        ws.Row(row).Style.Font.FontColor= XLColor.FromArgb(57, 71, 29);
                    }
                    if (e.estado == "INCOMPLETE") {
                        ws.Row(row).Style.Font.FontColor= XLColor.FromArgb(54, 96, 146);
                    }
                    ws.Range(row, 1, row, 7).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range(row, 1, row, 7).Style.Border.LeftBorderColor = XLColor.FromArgb(178, 178, 178); 
                    ws.Range(row, 1, row, 7).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range(row, 1, row, 7).Style.Border.RightBorderColor = XLColor.FromArgb(178, 178, 178);
                    ws.Range(row, 1, row, 7).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range(row, 1, row, 7).Style.Border.TopBorderColor = XLColor.FromArgb(178, 178, 178);
                    ws.Range(row, 1, row, 7).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(row, 1, row, 7).Style.Border.BottomBorderColor = XLColor.FromArgb(178, 178, 178);
                    row++;
                }
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();
                //ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Shipping Report Orders.xlsx\"");
                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream()){
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }
                httpResponse.End();
            }
        }







































    }
}