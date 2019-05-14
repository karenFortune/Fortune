using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FortuneSystem.Models.Almacen;
using FortuneSystem.Models.Trims;
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
using System.Data.SqlClient;
using Rotativa;
using System.Text.RegularExpressions;
using System.Drawing.Printing;
using System.Net.Sockets;

namespace FortuneSystem.Controllers
{
    public class TrimsController : Controller
    {
        DatosInventario di = new DatosInventario();
        DatosTrim dtrim = new DatosTrim();
        DatosReportes dr = new DatosReportes();
        FuncionesInventarioGeneral consultas = new FuncionesInventarioGeneral();
        DatosTransferencias dt = new DatosTransferencias();
        QRCodeController qr = new QRCodeController();
        PDFController pdf = new PDFController();
        DatosShipping ds = new DatosShipping();
        string filename, footer_alineacion, footer_size, vista;
        private object DialogResult;

        public ActionResult Index(){
            //Session["id_usuario"] = consultas.buscar_id_usuario(Convert.ToString(Session["usuario"]));
            int id_usuario = Convert.ToInt32(Session["id_Empleado"]);
            Session["id_usuario"] = id_usuario;
            if (Session["usuario"] == null){
                bool isExcelInstalled = Type.GetTypeFromProgID("Excel.Application") != null ? true : false;
                //return View(di.ListaInventario());
            }else{
                bool isExcelInstalled = Type.GetTypeFromProgID("Excel.Application") != null ? true : false;
                //return View(di.ListaInventario());
            }
            //ViewBag.WCPPDetectionScript = Neodynamic.SDK.Web.WebClientPrint.CreateWcppDetectionScript(Url.Action("ProcessRequest", "WebClientPrintAPI", null, HttpContext.Request.Url.Scheme), HttpContext.Session.SessionID);
            return View();
        }
        public ActionResult packing_instruction() {
            return View();
        }
        public ActionResult customer_trims() {
            return View();
        }
        //AUTOCOMPLETADOS
        public ActionResult Autocomplete_po(string term)
        {
            var items = consultas.Lista_po_abiertos();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_estilos(string term)
        {
            var items = consultas.Lista_styles();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_paises(string term)
        {
            var items = consultas.Lista_paises();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_fabricantes(string term)
        {
            var items = consultas.Lista_fabricantes();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_colores(string term)
        {
            var items = consultas.Lista_colores();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_colores_codigos(string term)
        {
            var items = consultas.Lista_colores_codigos();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_body_type(string term)
        {
            var items = consultas.Lista_body_types();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_tallas(string term)
        {
            var items = consultas.Lista_tallas();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_generos(string term)
        {
            var items = consultas.Lista_generos();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_telas(string term)
        {
            var items = consultas.Lista_telas();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_percents(string term)
        {
            var items = consultas.Lista_porcentajes();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_customer_name(string term)
        {
            var items = consultas.Lista_customers();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_cliente_final(string term)
        {
            var items = consultas.Lista_clientes_finales();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_ubicacion(string term)
        {
            var items = consultas.Lista_ubicaciones();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Autocomplete_unidades(string term)
        {
            var items = consultas.Lista_units();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_locacion(string term)
        {
            var items = consultas.Lista_customers();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_family(string term)
        {
            var items = consultas.Lista_family();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_trims(string term)
        {
            var items = consultas.Lista_trims();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_yarns(string term)
        {
            var items = consultas.Lista_yarn();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_units(string term)
        {
            var items = consultas.Lista_units();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        
        //AUTOCOMPLETADOS
        public JsonResult buscar_informacion_recibos_trims(string busqueda){
            return Json(dtrim.obtener_lista_recibos_trim(busqueda), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_recibos_filtro_trims(string busqueda){
            return Json(dtrim.obtener_lista_recibos_filtro_trim(busqueda), JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult buscar_pedidos(){
            return Json(dtrim.lista_ordenes(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_clientes()
        {
            return Json(dtrim.lista_clientes(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_estilos(string pedido){
            Session["pedido_trim_request"]=pedido;
            var result = Json(new {
                trims_anteriores = dtrim.obtener_trims_anteriores_pedido(pedido),//buscar las tallas del esa
                estilos = dtrim.lista_estilos_dropdown(pedido),
                empaque = dtrim.buscar_estado_instruccion_empaque(Convert.ToInt32(Session["pedido_trim_request"])),
                fold_size = dtrim.buscar_fold_size_pedido(Convert.ToInt32(Session["pedido_trim_request"])),
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_solo_estilos(string pedido){
            var result = Json(new{
                estilos = dtrim.lista_estilos_dropdown(pedido)
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_tallas_estilo(string estilo){
            return Json(dtrim.lista_tallas_dropdown(estilo), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_inventario_items_trim(){
            return Json(dtrim.lista_trim_items(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_total_estilo(string summary,string talla,string item){            
            var result = Json(new {
                total = dtrim.obtener_total_estilo(summary, talla),
                categoria = dtrim.obtener_family_trim_item(item),
                cajas=dtrim.obtener_cajas_estilo(summary,talla), 
                //trims_anteriores=dtrim.obtener_trims_anteriores(summary)//buscar las tallas del esa
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_trims_anteriores_pedido(string pedido){
            var result = Json(new{               
                trims_anteriores = dtrim.obtener_trims_anteriores_pedido(pedido)//buscar las tallas del esa
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult enviar_datos_request(string total,string estilo,string item,string talla,string cantidad,string blank,string request,string request_original,string instruccion,string fold_size){
            string[] totales = total.Split('*'), estilos = estilo.Split('*'), items = item.Split('*'), tallas = talla.Split('*');
            string[] cantidades = cantidad.Split('*'), blanks = blank.Split('*'), requests = request.Split('*'), requests_originales = request_original.Split('*');
            //buscar request a eliminar
            Session["cantidades_revisiones"] = cantidad;
            Session["items_revisiones"] = item;
            int cliente = consultas.obtener_customer_po(Convert.ToInt32(Session["pedido_trim_request"]));
            //Session["pedido_trim_request"])
            List<int> lista_original = new List<int>();
            for (int i = 1; i < requests_originales.Length; i++){ lista_original.Add(Convert.ToInt32(requests_originales[i])); }
            List<int> lista_nuevos = new List<int>();
            for (int i = 1; i < requests.Length; i++){ lista_nuevos.Add(Convert.ToInt32(requests[i])); }
            List<int> lista_final= lista_original.Except(lista_nuevos).ToList();
            foreach (int t in lista_final){ dtrim.eliminar_trim_request(Convert.ToString(t)); }
            List<Trim_requests> lista_existencias = new List<Trim_requests>();   
            for (int i = 1; i < totales.Length; i++) {
                if (requests[i] != "0"){                           
                    dtrim.modificar_request(requests[i], totales[i], Convert.ToInt32(Session["id_usuario"]), cantidades[i], blanks[i]);
                    dtrim.revisar_status_trim_request(requests[i], totales[i]);
                }else{
                    dtrim.guardar_request(totales[i], estilos[i], items[i], tallas[i], Convert.ToInt32(Session["id_usuario"]), cantidades[i], blanks[i]);
                }
                bool isEmpty = !lista_existencias.Any();
                if (isEmpty){
                    Trim_requests t = new Trim_requests();
                    t.id_item = Convert.ToInt32(items[i]);
                    t.cantidad = Convert.ToInt32(totales[i]);
                    lista_existencias.Add(t);                    
                }else{
                    int existe = 0;
                    foreach (Trim_requests r in lista_existencias) {
                        if (r.id_item == Convert.ToInt32(items[i])) { 
                            r.cantidad += Convert.ToInt32(totales[i]);
                            existe++;
                        }
                    }
                    if (existe == 0) {
                        Trim_requests t = new Trim_requests();
                        t.id_item = Convert.ToInt32(items[i]);
                        t.cantidad = Convert.ToInt32(totales[i]);
                        lista_existencias.Add(t);
                    }
                }
            }            
            int instruccion_empaque = dtrim.buscar_instruccion_empaque(Convert.ToInt32(Session["pedido_trim_request"]));
            if (instruccion_empaque == 0){
                dtrim.agregar_instruccion_empaque(Convert.ToInt32(Session["id_usuario"]), Convert.ToInt32(Session["pedido_trim_request"]), instruccion);
            }else {
                dtrim.update_instruccion_empaque(instruccion_empaque,Convert.ToInt32(Session["id_usuario"]),instruccion);
            }
            dtrim.ingresar_fold_size(Convert.ToInt32(Session["pedido_trim_request"]),fold_size);
            /******************************************************************************************************************************/
            foreach (Trim_requests r in lista_existencias){
                r.total = dtrim.buscar_total_item_inventario(r.id_item,cliente);
                r.item = dtrim.obtener_descripcion_item(r.id_item);
                r.pedido = consultas.obtener_po_id(Convert.ToString(Session["pedido_trim_request"]));
            }
            return Json(lista_existencias, JsonRequestBehavior.AllowGet);
        }
        





        public ActionResult Downloadexcel()
        {
            FileContentResult robj;
            List<Trim> lista = dtrim.lista_trims_por_pedir();
            int row = 1;
            using (XLWorkbook libro_trabajo = new XLWorkbook())
            { //Regex.Replace(pedido, @"\s+", " "); 
                var ws = libro_trabajo.Worksheets.Add("Material list");
                //CABECERAS TABLA
                var headers = new List<String[]>();
                List<String> titulos = new List<string>();
                titulos.Add("PO"); titulos.Add("STYLE"); titulos.Add("ITEM"); titulos.Add("TRIM"); titulos.Add("TOTAL"); titulos.Add("DATE"); titulos.Add("USER");
                headers.Add(titulos.ToArray());
                ws.Cell(row, 1).Value = headers;
                ws.Range(row, 1, row, 7).Style.Fill.BackgroundColor = XLColor.FromArgb(196, 215, 155);
                row++; //AGREGAR CABECERA TABLA
                foreach (Trim t in lista)
                {
                    var celdas = new List<String[]>();
                    List<String> datos = new List<string>();
                    datos.Add(t.po.pedido);
                    datos.Add(t.estilo.estilo);
                    datos.Add(t.item.componente);
                    datos.Add(t.item.descripcion);
                    datos.Add((t.item.total).ToString());
                    datos.Add(t.item.fecha);
                    datos.Add(t.usuario);
                    celdas.Add(datos.ToArray());
                    ws.Cell(row, 1).Value = celdas;
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
                
                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream()){
                    libro_trabajo.SaveAs(memoryStream);
                    var bytesdata = File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "myFileName.xlsx");
                    robj = bytesdata;
                }
               
            }
            return Json(robj, JsonRequestBehavior.AllowGet);
        }

        public void excel_material_pedir(){
            //string year = Convert.ToString(Session["year_reporte"]);
            List<Trim> lista = dtrim.lista_trims_por_pedir();
            int row = 1;
            using (XLWorkbook libro_trabajo = new XLWorkbook()){ //Regex.Replace(pedido, @"\s+", " "); 
                var ws = libro_trabajo.Worksheets.Add("Material list");
                //CABECERAS TABLA
                var headers = new List<String[]>();
                List<String> titulos = new List<string>();
                titulos.Add("PO"); titulos.Add("STYLE"); titulos.Add("ITEM"); titulos.Add("TRIM"); titulos.Add("TOTAL"); titulos.Add("DATE");titulos.Add("USER");
                headers.Add(titulos.ToArray());
                ws.Cell(row, 1).Value = headers;
                ws.Range(row, 1, row, 7).Style.Fill.BackgroundColor = XLColor.FromArgb(196, 215, 155);
                row++; //AGREGAR CABECERA TABLA
                foreach (Trim t in lista){
                    var celdas = new List<String[]>();
                    List<String> datos = new List<string>();
                    datos.Add(t.po.pedido);
                    datos.Add(t.estilo.estilo);
                    datos.Add(t.item.componente);
                    datos.Add(t.item.descripcion);
                    datos.Add((t.item.total).ToString());
                    datos.Add(t.item.fecha);
                    datos.Add(t.usuario);
                    celdas.Add(datos.ToArray());
                    ws.Cell(row, 1).Value = celdas;
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
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Material list to order.xlsx\"");
                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream())                {
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }
                httpResponse.End();
            }
        }
        
        public JsonResult buscar_lista_trims_recibidos(){
            return Json(dtrim.lista_trim_recibidos(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult enviar_datos_auditoria(string salidas,string originales,string nuevas){
            string[] inventario = salidas.Split('*'), original = originales.Split('*'), nueva = nuevas.Split('*');            
            for (int i = 1; i < inventario.Length; i++){                
                dtrim.actualizar_cantidad_inventario(Convert.ToInt32(inventario[i]), Convert.ToInt32(nueva[i]));
                consultas.insertar_registro(Convert.ToInt32(inventario[i]),Convert.ToInt32(Session["id_usuario"]),"Audit","UPDATE");
                dtrim.cambiar_estado_auditoria_inventario(inventario[i]);
                //actualizar cantidades en trims y el estado
                int recibo = dtrim.obtener_id_recibo_inventario(Convert.ToInt32(inventario[i]));
                dtrim.revisar_estados_cantidades_trim(Convert.ToInt32(inventario[i]), Convert.ToInt32(nueva[i]),recibo);
            }
            return Json(dtrim.comparacion_inventario_trim(salidas), JsonRequestBehavior.AllowGet);
        }
        public JsonResult po_session(string po){
            Session["po_trim"] = po;
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult trim_card(){
            return new ViewAsPdf("trim_card", dtrim.obtener_datos_trim_card(Convert.ToString(Session["po_trim"]), Convert.ToString(Session["id_usuario"]))){
                FileName = filename,
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageSize = Rotativa.Options.Size.Letter,
                PageMargins = new Rotativa.Options.Margins(5, 5, 5, 5),
                CustomSwitches = "--page-offset 0  ",
            };
        }

        public JsonResult buscar_items_trims(){
            return Json(dtrim.lista_descripciones_trims(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_informacion_trim(string id){
            var result = Json(new{
                trim = dtrim.informacion_editar_item_trim(id),                
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult editar_trim_catalogo(string id,string item,string minimo,string descripcion,string family,string unidad){
            dtrim.editar_informacion_trim(id,item,minimo,descripcion,family,unidad);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_trims_tabla_inicio(){
            return Json(dtrim.lista_trims_tabla_inicio(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_ordenes_tabla_inicio(){
            return Json(dtrim.obtener_lista_ordenes_estados("0"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_trims_recibidos_tabla_inicio(string busqueda,string fecha){
            var result = Json(new{
                tabla = dtrim.obtener_lista_trims_inicio(busqueda, fecha),
                mps = dtrim.buscar_mp_recibos_hoy()
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_especifico_ordenes_tabla_inicio(string busqueda){
            return Json(dtrim.obtener_lista_ordenes_estados(busqueda), JsonRequestBehavior.AllowGet);
        }
        public JsonResult obtener_informacion_recibo(string inventario,string total){
            int recibo = dtrim.obtener_id_recibo_inventario_detalles(Convert.ToInt32(inventario), Convert.ToInt32(total));
            return Json(di.lista_recibo_detalles(Convert.ToString(recibo)), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_trims_pedido(string pedido){
            var result = Json(new{
                trims_anteriores = dtrim.obtener_trims_anteriores_pedido(pedido),
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }   
        public JsonResult guardar_entrega(string pedido,string request,string cantidad,string entrega,string recibe,string original){
            string[] requests = request.Split('*'), cantidades = cantidad.Split('*'),originales=original.Split('*');
            dtrim.guardar_entrega(pedido,entrega,recibe);
            int id_entrega = dtrim.obtener_ultima_entrega();
            for (int i = 1; i < requests.Length; i++) {
                dtrim.guardar_entrega_item(id_entrega,requests[i],cantidades[i]);
                dtrim.actualizar_estado_entrega_request(requests[i],cantidades[i]);
                int inventario = dtrim.obtener_id_inventario_request(requests[i]);
                int familia = dtrim.buscar_family_trim_inventario(inventario);
                if (familia != 32) {
                    dtrim.actualizar_inventario(inventario, cantidades[i]);
                }                
                if (Convert.ToInt32(originales[i]) == Convert.ToInt32(cantidades[i])) {
                    dtrim.cambiar_estado_trim_request(Convert.ToInt32(requests[i]), 8);
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_entregas(string inicio,string final){
            return Json(dtrim.obtener_lista_entregas_fechas(inicio,final), JsonRequestBehavior.AllowGet);
        }        
        public JsonResult obtener_estado_total_pedido(string pedido){
            return Json(dtrim.buscar_estado_total_pedido(Convert.ToInt32(pedido)), JsonRequestBehavior.AllowGet);
        }
        public JsonResult sesiones_fechas(string inicio,string final){
            Session["inicio_excel"] = inicio;
            Session["final_excel"] = final;
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public void excel_trims_estados_fechas(){
            string inicio = Convert.ToString(Session["inicio_excel"]);            
            string final = Convert.ToString(Session["final_excel"]);
            List<Pedidos_trim> lista_pedidos = dtrim.obtener_pedidos_fechas(inicio,final);
            List<Family_trim> lista_familias = dtrim.obtener_familias();
            int row = 1,col=0;
            using (XLWorkbook libro_trabajo = new XLWorkbook()){ //Regex.Replace(pedido, @"\s+", " "); 
                var ws = libro_trabajo.Worksheets.Add("Material list");
                //CABECERAS TABLA
                //ws.Worksheets.Add("AutoFilter");
                int total_familias = 0;
                var headers = new List<String[]>();
                List<String> titulos = new List<string>();
                titulos.Add("PO"); titulos.Add("CUSTOMER"); titulos.Add("SHIP DATE"); titulos.Add("GENDER");titulos.Add("PACKING");
                foreach (Family_trim f in lista_familias) { titulos.Add(f.family_trim); total_familias++; }
                headers.Add(titulos.ToArray());
                ws.Cell(row, 1).Value = headers;
                ws.Range(row, 1, row, (total_familias+5)).Style.Fill.BackgroundColor = XLColor.FromArgb(52, 121, 191);
                ws.Range(row, 1, row, (total_familias + 5)).SetAutoFilter();
                ws.Range(row, 1, row, (total_familias + 5)).Style.Font.FontSize = 12;
                ws.Range(row, 1, row, (total_familias + 5)).Style.Font.Bold = true;
                for (int i = 1; i <= (total_familias + 5); i++){
                    ws.Cell(row, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(row, i).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                }
                row++; //AGREGAR CABECERA TABLA
                foreach (Pedidos_trim p in lista_pedidos){
                    var celdas = new List<String[]>();
                    List<String> datos = new List<string>();
                    datos.Add(Regex.Replace(p.pedido, @"\s+", " "));
                    datos.Add(Regex.Replace(p.customer, @"\s+", " "));
                    datos.Add(p.ship_date);
                    datos.Add(p.gender);
                    //AGREGAR AQUI EL PACKING                   
                    var d = ws.Cell(row, 5);
                    foreach (Empaque e in p.lista_empaque) {
                        if (e.tipo_empaque == 1){                                
                            foreach (ratio_tallas rt in e.lista_ratio){
                                d.RichText.AddText("BP ");
                                d.RichText.AddText(rt.talla + " " + rt.ratio);
                                d.RichText.AddText(Environment.NewLine);
                            }
                        }else {                            
                            d.RichText.AddText("PPK ");
                            foreach (ratio_tallas rt in e.lista_ratio){
                                d.RichText.AddText(" " + rt.ratio);
                            }
                            d.RichText.AddText(Environment.NewLine);
                        }
                    }                    
                   // d.RichText.AddText(Environment.NewLine);
                    foreach (Assortment a in p.lista_assort) {
                        d.RichText.AddText(" ASSORT " + a.cartones);
                        d.RichText.AddText(Environment.NewLine);
                    }
                    ws.Cell(row, 5).Style.Alignment.WrapText = true;
                    //AGREGAR AQUI EL PACKING
                    if (p.id_customer == 1) { ws.Cell(row,2).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 203, 229); }
                    col = 6;                     
                    foreach (Family_trim f in p.lista_families){
                        string texto = "",comentario= " Receives:\n ";
                        var c = ws.Cell(row,col); //C de CELDA
                        int estado_1 = 0, estado_2 = 0, estado_3 = 0,recibo=0,total_total=0,total_cantidad=0;
                        List<int> lista_estados = new List<int>();
                        foreach (Trim_requests t in f.lista_requests) {
                            texto = t.estilo + " "+t.item+" " + t.talla+" "+t.cantidad+"/"+t.total+"";
                            total_total += t.total;
                            total_cantidad += t.cantidad;
                            lista_estados.Add(t.id_estado);
                            c.RichText.AddText(Regex.Replace(texto, @"\s+", " "));
                            c.RichText.AddText(Environment.NewLine);
                            if (t.recibo != 0) {
                                recibo++;
                                comentario += t.recibo_item.fecha + " MP " + t.recibo_item.mp_number + "\n";
                            }
                        }
                        c.RichText.AddText(Regex.Replace(total_cantidad+"/"+total_total, @"\s+", " "));
                        foreach (int i in lista_estados) { if (i == 1) { estado_1++; } if (i == 2) { estado_2++; } if (i == 3) { estado_3++; } }                        
                        if (estado_1 != 0) { ws.Cell(row, col).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 122); }
                        if (estado_3 != 0) { ws.Cell(row, col).Style.Fill.BackgroundColor = XLColor.FromArgb(174, 252, 174); }
                        if (estado_2 != 0) { ws.Cell(row, col).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 133, 133); }
                        if (estado_1 != 0 && estado_2 != 0 && estado_3 != 0) { ws.Cell(row, col).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 255); }
                        //datos.Add(texto);
                        if (recibo!=0){ ws.Cell(row, col).Comment.AddText(comentario); }
                        ws.Cell(row, col).Style.Alignment.WrapText = true;                        
                        col++;
                    }//TRIMS
                    celdas.Add(datos.ToArray());
                    ws.Cell(row, 1).Value = celdas;
                    for (int i = 1; i <= total_familias; i++) {
                        ws.Cell(row,i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(row, i).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    }                    
                    row++;
                }//PEDIDOS
                ws.SheetView.FreezeColumns(5);
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();
                //ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"TRIM "+inicio+" to "+final+".xlsx\"");
                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream()){
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }
                httpResponse.End();
            }
        }
        //***********************************************************
        public JsonResult buscar_trims_pedido_localtion(string pedido){
            var result = Json(new{
                trims_anteriores = dtrim.obtener_trims_anteriores_pedido_location(pedido),
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult guardar_locaciones(string locacion, string request,string cantidad){
            string[] requests = request.Split('*'),cantidades=cantidad.Split('*');
            dtrim.guardar_locacion(locacion, Convert.ToInt32(Session["id_usuario"]));
            int location = dtrim.obtener_id_locacion();
            for (int i=1; i<requests.Length; i++) {
                dtrim.guardar_locacion_item(location,requests[i],cantidades[i]);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }        
        public JsonResult obtener_estado_total_pedido_location(string pedido){
            return Json(dtrim.buscar_estado_total_pedido_locacion(Convert.ToInt32(pedido)), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_entregas_estilo(string summary){
            return Json(dtrim.obtener_lista_entregas_estilos(summary), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_cajas_orden(string pedido){
            var result = Json(new{
                cajas=dtrim.obtener_listado_cajas(Convert.ToInt32(pedido)),
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }       
        [HttpPost]
        public ActionResult UploadFiles(){
            // Checking no of files injected in Request object 
            string archivo = "";
            if (Request.Files.Count > 0){
                try{
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++){
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  
                        HttpPostedFileBase file = files[i];
                        string fname;
                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER"){
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }else{
                            fname = file.FileName;
                        }
                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/Content/Uploads/"), fname);
                        file.SaveAs(fname);
                        archivo = fname;
                    }
                    Session["archivo_comparacion"] = archivo;
                    // Returns message that successfully uploaded  
                    /*if (System.IO.File.Exists(fullPath)){//PARA BORRAR
                        System.IO.File.Delete(fullPath);
                    }*/
                    //OBTENER INFORMACIÓN DEL PEDIDO
                    List<registro_price_tickets> lista_sistema = dtrim.buscar_registro_price_ticket_pedido(Convert.ToInt32(Session["pedido_comparacion"]));
                    //GUARDAR INFORMACIÓN EXCEL
                    List<registro_price_tickets> lista_excel = obtener_registros_excel(archivo);
                    //COMPARAR ¿¿??
                    List<registro_price_tickets> lista_final = new List<registro_price_tickets>();
                    foreach (registro_price_tickets e in lista_excel) {
                        int existe = 0;
                        registro_price_tickets rpt = new registro_price_tickets();
                        foreach (registro_price_tickets s in lista_sistema) {
                            if (s.talla == e.talla && s.estilo == e.estilo){
                                rpt.estado = 0;
                                existe++;
                                if (e.total == s.total) { rpt.total = s.total; } else { rpt.total = e.total + "*" + s.total; }
                                if (e.estilo == s.estilo) { rpt.estilo = s.estilo; } else { rpt.estilo = e.estilo + "*" + s.estilo; }
                                if (e.upc == s.upc) { rpt.upc = s.upc; } else { rpt.upc = e.upc + "*" + s.upc; }
                                if (e.descripcion_estilo == s.descripcion_estilo) { rpt.descripcion_estilo = s.descripcion_estilo; } else { rpt.descripcion_estilo = e.descripcion_estilo + "*" + s.descripcion_estilo; }
                                if (e.color == s.color) { rpt.color = s.color; } else { rpt.color = e.color + "*" + s.color; }
                                if (e.talla == s.talla) { rpt.talla = s.talla; } else { rpt.talla = e.talla + "*" + s.talla; }                                
                            }
                        }
                        if (existe == 0) {
                             rpt.estado = 1;
                             rpt.total = e.total;
                             rpt.estilo = e.estilo;
                             rpt.upc = e.upc;
                             rpt.descripcion_estilo = e.descripcion_estilo;
                             rpt.color = e.color;
                             rpt.talla = e.talla;                           
                            foreach (registro_price_tickets s in lista_sistema){                               
                                rpt.total += "*" + s.total; 
                                rpt.estilo += "*" + s.estilo; 
                                rpt.upc += "*" + s.upc; 
                                rpt.descripcion_estilo += "*" + s.descripcion_estilo; 
                                rpt.color += "*" + s.color; 
                                rpt.talla +="*"+ s.talla;                                
                            }
                        }
                        if (e.tickets != "") { rpt.tickets = e.tickets; } else { rpt.tickets = ""; }
                        if (e.dept != "") { rpt.dept = e.dept; } else { rpt.dept = ""; }
                        if (e.clas != "") { rpt.clas = e.clas; } else { rpt.clas = ""; }
                        if (e.sub != "") { rpt.sub = e.sub; } else { rpt.sub = ""; }
                        if (e.retail != "") { rpt.retail = e.retail; } else { rpt.retail = ""; }
                        if (e.cl != "") { rpt.cl = e.cl; } else { rpt.cl = ""; }                        
                        lista_final.Add(rpt);
                    }
                    if (System.IO.File.Exists(archivo)){ System.IO.File.Delete(archivo); }
                    // return Json("File Uploaded Successfully! "+archivo);

                    var result = Json(new{
                        id_pedido = Convert.ToString(Session["pedido_comparacion"]),
                        pedido=consultas.obtener_po_id(Convert.ToString(Session["pedido_comparacion"])),
                        lista = lista_final,
                    });
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex){ return Json("Error occurred. Error details: " + ex.Message); }
            }else { return Json("No files selected."); }
        }
        public JsonResult guardar_pedido_sesion(string pedido){
            Session["pedido_comparacion"] = pedido;
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public List<registro_price_tickets> obtener_registros_excel(string archivo){
            List<registro_price_tickets> lista = new List<registro_price_tickets>();
            
            using (XLWorkbook libro_trabajo = new XLWorkbook(archivo)){ 
                //Regex.Replace(pedido, @"\s+", " "); 
                var ws = libro_trabajo.Worksheet(1);
                var nonEmptyDataRows = libro_trabajo.Worksheet(1).RowsUsed();

                foreach (var dataRow in nonEmptyDataRows){
                    //for row number check
                    if (dataRow.RowNumber() >1){//&& dataRow.RowNumber() <= 20
                        //to get column # 3's data
                        //var cell = dataRow.Cell(3).Value;
                        registro_price_tickets r = new registro_price_tickets();
                        r.estado = 0;
                        r.total = Convert.ToString(dataRow.Cell(1).Value).Trim();
                        r.estilo = (Convert.ToString(dataRow.Cell(2).Value)).Trim();
                        r.upc = (Convert.ToString(dataRow.Cell(3).Value)).Trim();
                        r.descripcion_estilo = (Convert.ToString(dataRow.Cell(4).Value)).Trim();
                        r.color = (Convert.ToString(dataRow.Cell(5).Value)).Trim();
                        r.talla = (Convert.ToString(dataRow.Cell(6).Value)).Trim();
                        /*************************************************************************************************/
                        r.tickets = (Convert.ToString(dataRow.Cell(7).Value)).Trim();
                        r.dept = (Convert.ToString(dataRow.Cell(8).Value)).Trim();
                        r.clas = (Convert.ToString(dataRow.Cell(9).Value)).Trim();
                        r.sub = (Convert.ToString(dataRow.Cell(10).Value)).Trim();
                        r.retail = (Convert.ToString(dataRow.Cell(11).Value)).Trim();
                        r.cl = (Convert.ToString(dataRow.Cell(12).Value)).Trim();

                        lista.Add(r);
                    }
                }

            }
            return lista;
        }
        public JsonResult guardar_price_tickets(string pedido,string total,string estilo,string upc,string descripcion,string color,string talla,string ticket,string dept,string clas,string sub,string retail,string cl,string impreso){
            // pedido''total''estilo'upc','decripcion'color''talla''ticket'dept':','clas':,'sub''retail''cl':
            string[] totales = total.Split('*'), estilos = estilo.Split('*'), upcs = upc.Split('*'), descripciones = descripcion.Split('*');
            string[] colores = color.Split('*'), tallas = talla.Split('*'), tickets = ticket.Split('*'), depts = dept.Split('*'), clases = clas.Split('*');
            string[] subs = sub.Split('*'), retails = retail.Split('*'), cls = cl.Split('*');
            for (int i = 1; i<totales.Length; i++) {
                dtrim.guardar_price_tickets(pedido, totales[i], estilos[i], upcs[i], descripciones[i], colores[i], tallas[i], tickets[i], depts[i],clases[i], subs[i], retails[i], cls[i], Convert.ToString(Session["id_usuario"]));
            }
            if (impreso == "1") {
                dtrim.marcar_price_tickets_pedido_impreso(Convert.ToInt32(pedido));
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult buscar_price_tickets(string pedido){
            return Json(dtrim.buscar_price_tickets_pedido(Convert.ToInt32(pedido)), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_ordenes_pt(){
            return Json(dtrim.obtener_lista_pedidos_pt(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult marcar_pt_impreso(string informacion){
            string[] datos = informacion.Split('*');
            for (int i = 1; i < datos.Length; i++) {
                string[] individuales = datos[i].Split('_');
                //+ t.id_request + '_' + t.id_inventario + '_' + t.total + 
                dtrim.marcar_pt_impreso(Convert.ToInt32(individuales[0]));
                dtrim.actualizar_inventario(Convert.ToInt32(individuales[1]), individuales[2]);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_cajas_estilo(string summary){
            var result = Json(new{
                cajas = dtrim.obtener_cajas_summary(Convert.ToInt32(summary)),
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public string texto_imprimir;
        public JsonResult guardar_solicitud_cajas(string request, string summary, string total, string item){
            string[] requests = request.Split('*'), summarys = summary.Split('*'), totales = total.Split('*'), items = item.Split('*');
            texto_imprimir = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\nSOLICITUD\nPOR " + consultas.buscar_nombre_usuario(Convert.ToString(Session["id_usuario"])) + "\n\n";
            for (int i = 1; i < requests.Length; i++){
                dtrim.guardar_solicitud_caja(summarys[i], requests[i], totales[i], Convert.ToInt32(Session["id_usuario"]));
                texto_imprimir += items[i] + ":  " + totales[i] + "\n";
            }
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
            // Set the printer name.
            //pd.PrinterSettings.PrinterName = "file://ns5/hpoffice"
            pd.PrinterSettings.PrinterName = "Zebra ZP 500";
            pd.Print();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Button1_Click(){
            /*PrintEcentia p = new PrintEcentia();
            int id = Convert.ToInt16(idUsuarioSession);
            string archivo = "";
            archivo = p.imprimeEtiqueta("20000", 10);*/

            //p.printOne(portImpresora);

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
            // Set the printer name.
            //pd.PrinterSettings.PrinterName = "file://ns5/hpoffice"
            pd.PrinterSettings.PrinterName = "Zebra ZP 500";

            pd.Print();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            Font printFont = new Font("3 of 9 Barcode", 17);
            Font printFont1 = new Font("Times New Roman", 9, FontStyle.Bold);

            SolidBrush br = new SolidBrush(Color.Black);

            ev.Graphics.DrawString(texto_imprimir, printFont, br, 10, 65);
            ev.Graphics.DrawString("*texttodisplay*", printFont1, br, 10, 85);
        }
        /************************************************************************************************************************************************************************/
        //buscar_clientes_generos_familias
        public JsonResult buscar_clientes_generos_familias(){
            var result = Json(new{
                clientes = dtrim.obtener_lista_clientes(),
                generos = dtrim.obtener_lista_generos(),
                familias = dtrim.obtener_familias(),
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult upload_imagen_trim(){
            // Checking no of files injected in Request object 
            string archivo = "";
            if (Request.Files.Count > 0){
                try{
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++){
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  
                        HttpPostedFileBase file = files[i];
                        string fname;
                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER"){
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }else{
                            fname = file.FileName;
                        }
                        Session["imagen_nueva"] = fname;
                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/Content/img/Trims/"), fname);
                        file.SaveAs(fname);
                        archivo = fname;
                    }                                  
                    return Json("", JsonRequestBehavior.AllowGet);
                }catch (Exception ex) { return Json("Error occurred. Error details: " + ex.Message); }
            }else { return Json("No files selected."); }
        }
        //guardar_nueva_imagen

        public JsonResult guardar_nueva_imagen(string familia,string cliente,string genero){
            string[] clientes = cliente.Split('*'), generos = genero.Split('*');
            string imagen = Convert.ToString(Session["imagen_nueva"]);
            dtrim.guardar_nueva_imagen(imagen,familia,Convert.ToInt32(Session["id_usuario"]));
            int id_imagen = dtrim.obtener_ultimo_id_imagen();
            for (int i = 1; i < clientes.Length; i++) {
                dtrim.guardar_imagen_genero_cliente(id_imagen,clientes[i],generos[i]);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_imagenes_familia(string familia){
            return Json(dtrim.obtener_lista_imagenes_familia(Convert.ToInt32(familia)), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ConvertirImagen(string id){
            string nombre= dtrim.buscar_imagen(Convert.ToInt32(id));
            return new FilePathResult("~/Content/img/Trims/" + nombre, System.Net.Mime.MediaTypeNames.Application.Octet);
        }
        public JsonResult buscar_genero_cliente_trim(string trim){
            Session["trim_edicion"] = trim;
            return Json(dtrim.obtener_lista_generos_clientes_imagen(Convert.ToInt32(trim)), JsonRequestBehavior.AllowGet);
        }
        public JsonResult guardar_edicion_imagen(string trim, string cliente, string genero,string cambio,string imagen){
            string[] clientes = cliente.Split('*'), generos = genero.Split('*');
            if (cambio == "1") {
                dtrim.edicion_imagen_trim(trim, imagen, Convert.ToInt32(Session["id_usuario"]));
            }
            dtrim.borrar_genero_clientes_imagen(trim);
            for (int i = 1; i < clientes.Length; i++){
                dtrim.guardar_imagen_genero_cliente(Convert.ToInt32(trim), clientes[i], generos[i]);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult obtener_datos_pedido_tc(string pedido){
           
            List<estilo_shipping> e = ds.lista_estilos(pedido);
            List<int> lista_generos = (consultas.Lista_generos_po(Convert.ToInt32(pedido))).Distinct().ToList();
            clientes customer = new clientes();
            customer.id_customer = consultas.obtener_customer_final_po(Convert.ToInt32(pedido));
            customer.customer = consultas.obtener_customer_final_id(Convert.ToString(customer.id_customer));
            List<Familias_trim_card> lista_familias = dtrim.obtener_datos_familias_pedido(Convert.ToInt32(pedido), lista_generos, customer.id_customer);
            var result = Json(new{
                estilos=e,
                assorts = ds.lista_assortments_pedido(Convert.ToInt32(pedido)),
                familias_imagenes = lista_familias,
                cliente=customer,
                generos=lista_generos,
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult guardar_tc(string pedido,string customer,string tipo_empaque,string ratio,string familia,string imagen,string nota,string especial,string generos,string item){
            string[] familias = familia.Split('*'),imagenes=imagen.Split('*'),notas=nota.Split('*'),especiales=especial.Split('*'),items=item.Split('*');
            dtrim.guardar_nuevo_trim_card(pedido,customer,tipo_empaque,ratio, Convert.ToInt32(Session["id_usuario"]),generos);
            int id_trim_card = dtrim.obtener_ultimo_trim_card();
            for (int i = 1; i < familias.Length; i++) {
                dtrim.guardar_trim_card_familia(id_trim_card,imagenes[i],especiales[i],notas[i],familias[i],items[i]);
            }
            dtrim.agregar_trim_card_fold_size(pedido,id_trim_card);
            return Json(id_trim_card, JsonRequestBehavior.AllowGet);
            //return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult buscar_trim_card(string busqueda){            
            return Json(dtrim.obtener_lista_trim_cards(busqueda), JsonRequestBehavior.AllowGet);
        }

        public JsonResult session_tc(string id){
            Session["trim_card_id_print"] = id;
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult imprimir_tc(){
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            foreach (var key in Request.Cookies.AllKeys){
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            return new ViewAsPdf("trim_card_nuevo", dtrim.obtener_trim_card(Convert.ToInt32(Session["trim_card_id_print"]))){
                FileName = "Trim Card #" + Convert.ToString(Session["trim_card_id_print"]) + ".pdf",
                Cookies = cookieCollection,
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageSize = Rotativa.Options.Size.Letter,
                PageMargins = new Rotativa.Options.Margins(5, 5, 5, 5),
                CustomSwitches = "--page-offset 0 --footer-right [page]/[toPage] --footer-font-size 9 ",
                FormsAuthenticationCookieName = System.Web.Security.FormsAuthentication.FormsCookieName
            };
        }
        /*************************************************************************************************************************/

        public JsonResult obtener_datos_edicion_trim_card(string trim_card){
            int pedido = dtrim.buscar_pedido_trim_card(Convert.ToInt32(trim_card));
            List<estilo_shipping> e = ds.lista_estilos(pedido.ToString());
            List<int> lista_generos = (consultas.Lista_generos_po(pedido)).Distinct().ToList();
            clientes customer = new clientes();
            customer.id_customer = consultas.obtener_customer_final_po(pedido);
            customer.customer = consultas.obtener_customer_final_id(Convert.ToString(customer.id_customer));
            List<Familias_trim_card> lista_familias = dtrim.obtener_datos_familias_pedido(pedido, lista_generos, customer.id_customer);
            var result = Json(new{
                estilos = e,
                assorts = ds.lista_assortments_pedido(pedido),
                familias_imagenes = lista_familias,
                cliente = customer,
                generos = lista_generos,
                trim_card= dtrim.obtener_trim_card(Convert.ToInt32(trim_card)),
                id_pedido=pedido,
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult guardar_edicion_tc(string id_trim_card, string customer, string tipo_empaque, string ratio, string familia, string imagen, string nota, string especial, string generos, string item){
            string[] familias = familia.Split('*'), imagenes = imagen.Split('*'), notas = nota.Split('*'), especiales = especial.Split('*'), items = item.Split('*');            
            dtrim.editar_trim_card(Convert.ToInt32(id_trim_card), tipo_empaque,ratio);
            dtrim.borrar_trim_card_spec(Convert.ToInt32(id_trim_card));
            for (int i = 1; i < familias.Length; i++){
                dtrim.guardar_trim_card_familia(Convert.ToInt32(id_trim_card), imagenes[i], especiales[i], notas[i], familias[i], items[i]);
            }
            return Json(id_trim_card, JsonRequestBehavior.AllowGet);
        }

        public JsonResult buscar_items_recibos_trims(string busqueda){           
            return Json(dtrim.busqueda_items_trims(busqueda), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_sucursales(){           
            return Json(dtrim.lista_sucursales(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult guardar_recibo_trims(string pedido,string mp,string po_number,string item,string estilo,string cantidad,string packing_number,string cliente,string sucursal){
            string[] items = item.Split('*'), estilos = estilo.Split('*'), cantidades = cantidad.Split('*');
            int total = 0, existencia = 0, summary = 0;
            string trims_inventario = "", trims_cantidades = "", trims_item="", ids_inventario="", qty_item="", ids_summary="";
            //int customer = consultas.obtener_customer_final_po(Convert.ToInt32(pedido));
            for (int i=1; i<items.Length; i++) {
                total += Convert.ToInt32(cantidades[i]);
                qty_item += "*" + cantidades[i];
            }
            di.id_usuario = Convert.ToInt32(Session["id_usuario"]);
            di.id_sucursal = Convert.ToInt32(sucursal); 
            di.id_company = Convert.ToInt32(cliente);
            di.guardar_recibo(total, Convert.ToInt32(cliente), mp, po_number,packing_number);
            di.id_recibo = di.obtener_ultimo_recibo();
            for (int i=1; i<items.Length; i++) {
                consultas.buscar_informacion_trim_item(items[i]);
                di.cantidad = Convert.ToInt32(cantidades[i]);
                //di.obtener_datos_trim(Convert.ToInt32(items[i]), Convert.ToInt32(Session["id_usuario"]), estilos[i], "Trims", pos[i], consultas.unit, customers[i], total_item.ToString(), consultas.descripcion, consultas.family, "0");
                di.id_item= Convert.ToInt32(items[i]);
                di.id_estilo = Convert.ToInt32(estilos[i]);
                di.id_tipo = consultas.buscar_tipo_inventario("Trims");
                di.id_unit= consultas.buscar_unit(consultas.unit);
                di.id_customer = Convert.ToInt32(cliente);
                di.id_familia = consultas.buscar_familia_trim(dtrim.obtener_family_trim_item(items[i]));
                di.id_pedido= Convert.ToInt32(pedido);
                di.id_trim = 0;
                di.descripcion = dtrim.obtener_descripcion_item(Convert.ToInt32(items[i]))+" "+dtrim.obtener_family_trim_item(items[i]);
                di.descripcion= Regex.Replace(di.descripcion, @"\s+", " ");
                di.total = Convert.ToInt32(cantidades[i]);
                di.minimo_trim = 0;                
                existencia = di.buscar_existencia_trim_inventario();
                summary = consultas.obtener_po_summary(di.id_pedido, di.id_estilo);
                ids_summary += "*" + summary.ToString();
                if (existencia == 0){
                    di.guardar_trim_po();
                    di.id_inventario = di.obtener_ultimo_inventario();
                    ids_inventario += "*" + di.id_inventario.ToString();
                }else{
                    di.sumar_existencia_trim(existencia);
                    ids_inventario += "*" + existencia.ToString();
                }
                trims_inventario += "*" + consultas.obtener_po_summary(di.id_pedido, di.id_estilo);
                trims_cantidades += "*" + cantidades[i];
                trims_item += "*" + items[i];
            }
            string[] trimsInventario = trims_inventario.Split('*'), trimsCantidad = trims_cantidades.Split('*'), trimsItem = trims_item.Split('*');
            string[] inventarios = ids_inventario.Split('*'), totales_items = qty_item.Split('*'), summarys = ids_summary.Split('*');
            for (int j = 1; j < inventarios.Length; j++){
                di.guardar_recibo_item(di.id_recibo, inventarios[j], totales_items[j], summarys[j]);
               // di.id_recibo_item = di.obtener_ultimo_recibo_item();                
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult buscar_informacion_trim_estilo(string estilo){
            var result = Json(new{
                trims = dtrim.obtener_trims_customer(estilo),
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_inventario_trims(string busqueda){           
            return Json(dtrim.obtener_lista_trims_inventario(busqueda), JsonRequestBehavior.AllowGet);
        }
        public JsonResult cambiar_cantidades_inventario_trim(string inventario,string cantidad,string pedido,string tiene_po,string estilo){
            Inventario i = dtrim.obtener_item_editar(Convert.ToInt32(inventario));
            
            di.id_usuario = Convert.ToInt32(Session["id_usuario"]);
            di.id_sucursal = Convert.ToInt32(consultas.obtener_sucursal_id_usuario(Convert.ToInt32(Session["id_usuario"])));
            int summary;
            di.id_familia = dtrim.buscar_family_trim_inventario(Convert.ToInt32(inventario));
            di.id_item = dtrim.buscar_id_item(Convert.ToInt32(inventario));
            consultas.buscar_informacion_trim_item((di.id_item).ToString());
            di.cantidad = Convert.ToInt32(cantidad);
            di.id_tipo = 2;
            di.id_unit = Convert.ToInt32(i.id_unit);
            di.id_customer = i.id_customer;
            di.id_trim = i.id_trim;
            di.descripcion = i.descripcion;
            di.total = Convert.ToInt32(cantidad);
            di.minimo_trim = i.minimo;
            //restar cantidad de inventario
            dtrim.actualizar_inventario(Convert.ToInt32(inventario), cantidad);
           
            if (tiene_po == "0"){  //INVENTARIO A ORDEN   
                di.id_estilo = consultas.obtener_estilo_summary(Convert.ToInt32(estilo));
                di.id_pedido = Convert.ToInt32(pedido);
                summary = Convert.ToInt32(estilo);                
                int nuevo_inventario = dtrim.buscar_elemento_inventario_orden_item(di.id_item,summary,di.id_pedido,di.id_estilo,di.id_customer);
                if (nuevo_inventario != 0){
                    di.update_inventario(nuevo_inventario,Convert.ToInt32(cantidad));                    
                }else {
                    di.guardar_trim_po();
                }
                dtrim.revisar_estados_cantidades_trim(nuevo_inventario, Convert.ToInt32(cantidad), 0);
            }else { //DE ORDEN A INVENTARIO
                di.id_estilo = 0;
                di.id_pedido = 0;
                summary = 0;
                int nuevo_inventario = dtrim.buscar_elemento_inventario_orden_item(di.id_item, summary, di.id_pedido, di.id_estilo, di.id_customer);
                if (nuevo_inventario != 0){                  
                    di.update_inventario(nuevo_inventario, Convert.ToInt32(cantidad));
                }else{
                    di.guardar_trim_po();
                }
                if (i.auditoria == 1){
                    summary = Convert.ToInt32(estilo);
                    Trim_requests request = dtrim.buscar_trim(i.id_summary, di.id_item);
                    dtrim.restar_cantidad_request(request.id_request,Convert.ToInt32(cantidad));
                    if ((request.restante+ Convert.ToInt32(cantidad)) ==0){
                        dtrim.cambiar_estado_trim_request(request.id_request, 3);
                    }else{
                        dtrim.cambiar_estado_trim_request(request.id_request, 2);
                    }
                }
                if (i.total == Convert.ToInt32(cantidad)) {
                    dtrim.eliminar_inventario(i.id_inventario);
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult reporte_customer_stock(string cliente){
            Session["cliente_reporte_stock"] = cliente;
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public void excel_customer_stock(){
            int cliente = Convert.ToInt32(Session["cliente_reporte_stock"]);

            List<Inventario> lista = dtrim.buscar_inventario_customer(cliente);
            int row = 1;
            using (XLWorkbook libro_trabajo = new XLWorkbook()){ //Regex.Replace(pedido, @"\s+", " "); 
                var ws = libro_trabajo.Worksheets.Add("Fortune Inventory");
                
                //CABECERAS TABLA
                var headers = new List<String[]>();
                List<String> titulos = new List<string>();
                titulos.Add("AMT"); titulos.Add("ITEM"); titulos.Add("TIPO"); titulos.Add("STOCK"); 
                headers.Add(titulos.ToArray());
                ws.Cell(row, 1).Value = headers;
                ws.Range(row, 1, row, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 112, 192);
                ws.Range(row, 1, row, 4).Style.Font.Bold = true;
                ws.Range(row, 1, row, 4).Style.Font.FontColor = XLColor.White;
                ws.Range(1, 1, 1, 4).SetAutoFilter();
                row++; //AGREGAR CABECERA TABLA
                foreach (Inventario i in lista){
                    var celdas = new List<String[]>();
                    List<String> datos = new List<string>();
                    datos.Add(i.amt_item);
                    datos.Add(i.descripcion);
                    datos.Add(i.family_trim);
                    datos.Add(Convert.ToString(i.total));
                    
                    celdas.Add(datos.ToArray());
                    ws.Cell(row, 1).Value = celdas;
                    ws.Range(row, 1, row, 7).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range(row, 1, row, 7).Style.Border.LeftBorderColor = XLColor.FromArgb(178, 178, 178);
                    ws.Range(row, 1, row, 7).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range(row, 1, row, 7).Style.Border.RightBorderColor = XLColor.FromArgb(178, 178, 178);
                    ws.Range(row, 1, row, 7).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range(row, 1, row, 7).Style.Border.TopBorderColor = XLColor.FromArgb(178, 178, 178);
                    ws.Range(row, 1, row, 7).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(row, 1, row, 7).Style.Border.BottomBorderColor = XLColor.FromArgb(178, 178, 178);
                    ws.Cell(row, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    ws.Cell(row, 4).Style.Font.FontColor = XLColor.White;
                    ws.Cell(row, 4).Style.Font.Bold = true;
                    row++;
                }
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();
                //ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string nombre_cliente = consultas.obtener_customer_id(Convert.ToString(cliente));
                httpResponse.AddHeader("content-disposition", "attachment;filename=\""+ nombre_cliente + " "+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Inventory.xlsx\"");
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





































    }

    internal class PrintDialog
    {
        public PrinterSettings PrinterSettings { get; internal set; }
    }
}