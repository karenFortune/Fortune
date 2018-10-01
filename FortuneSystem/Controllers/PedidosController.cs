using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Item;
using FortuneSystem.Models.Items;
using FortuneSystem.Models.Pedidos;
using FortuneSystem.Models.POSummary;
using FortuneSystem.Models.Revisiones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers
{
    public class PedidosController : Controller
    {
        // GET: Pedido
        PedidosData objPedido = new PedidosData();
        CatClienteData objCliente = new CatClienteData();
        CatClienteFinalData objClienteFinal = new CatClienteFinalData();
        CatStatusData objEstados = new CatStatusData();
        CatGeneroData objGenero = new CatGeneroData();
        CatColoresData objColores = new CatColoresData();
        DescripcionItemData objItems = new DescripcionItemData();
        CatTelaData objTela = new CatTelaData();
        CatTipoCamisetaData objTipoC = new CatTipoCamisetaData();
        ItemTallaData objTallas = new ItemTallaData();
        RevisionesData objRevision = new RevisionesData();
        ItemDescripcionData objEst = new ItemDescripcionData(); 
        public int estado;
        public int IdPO;
        public int pedidos;
    
        public ActionResult Index()
         {
            List<OrdenesCompra> listaPedidos= new List<OrdenesCompra>();
             listaPedidos = objPedido.ListaOrdenCompra().ToList();
             return View(listaPedidos);
         }      
     

        /*  [ChildActionOnly]
          public ActionResult StudentList()
          {
              lista = objPedido.ListaOrdenCompra().ToList();
              return PartialView(lista);
          }*/

        /* public ActionResult Lista_Pedido_Por_Fecha(DateTime? fechaCancel, DateTime? fechaOrden)
         {
             List<OrdenesCompra> lista = objPedido.ListaOrdenCompra(fechaCancel, fechaOrden).ToList();
             return PartialView(lista);
         }*/

        [HttpPost]
        public JsonResult Lista_Estilos_PO(int? id)
        {

            List<POSummary> listaItems = objItems.ListaItemsPorPO(id).ToList();
      
            var result = Json(new { listaItem = listaItems});
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        public ActionResult Listado_Estilos_PO()
        {
            int? id = Convert.ToInt32(Convert.ToInt32(Session["id_pedido"]));
         
            List<POSummary> listaItems = objItems.ListaItemsPorPO(id).ToList();

            return PartialView(listaItems);
        }

        [ChildActionOnly]
        public ActionResult Listado_Tallas_Estilo(int? id)
        {
            List<ItemTalla> listaTallas = objTallas.ListaTallasPorEstilo(id).ToList();

            return PartialView(listaTallas);
        }

        public ActionResult HistorialPedidos(int id)
        {
            List<OrdenesCompra> listaPedidosRev = new List<OrdenesCompra>();
            listaPedidosRev = objPedido.ListaRevisionesPO(id).ToList();

            return View(listaPedidosRev);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Estilo(int? id)
        {
            List<ItemTalla> listaTallas = objTallas.ListaTallasPorEstilo(id).ToList();
            string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }
            var result = Json(new { listaTalla = listaTallas, estilos = estilo });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Staging_Estilo(int? id)
        {
            List<ItemTalla> listaTallas = objTallas.ListaTallasStagingPorEstilo(id).ToList();
            string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }
            var result = Json(new { listaTalla = listaTallas, estilos = estilo });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult CrearPO()
        {
            OrdenesCompra pedido = new OrdenesCompra();
            POSummary summary = new POSummary();
            ListasClientes(pedido);
            ListaEstados(pedido);
            ListaGenero(summary);
            ListaTela(summary);
            ListaTipoCamiseta(summary);
            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearPO([Bind] OrdenesCompra ordenCompra)
        {
            if (ModelState.IsValid)
            {
                ObtenerIdClientes(ordenCompra);
                ListaEstados(ordenCompra);
                //objPedido.AgregarPO(pedido);
            }

            return View();
        }

        [HttpGet]
        public ActionResult Detalles(int? id)
        {
            if (id == null)
            {
                return View();
            }

            OrdenesCompra pedido = objPedido.ConsultarListaPO(id);
            pedido.CatCliente = objCliente.ConsultarListaClientes(pedido.Cliente);
            pedido.CatClienteFinal = objClienteFinal.ConsultarListaClientesFinal(pedido.ClienteFinal);
            pedido.IdPedido =Convert.ToInt32(id);
            
            if (pedido == null)
            {
                return View();
            }
            return View(pedido);
        }

        [HttpGet]
        public ActionResult DetallesRev(int? id)
        {
            if (id == null)
            {
                return View();
            }

            OrdenesCompra pedido = objPedido.ConsultarListaPO(id);
            pedido.CatCliente = objCliente.ConsultarListaClientes(pedido.Cliente);
            pedido.CatClienteFinal = objClienteFinal.ConsultarListaClientesFinal(pedido.ClienteFinal);
            pedido.IdPedido = Convert.ToInt32(id);

            if (pedido == null)
            {
                return View();
            }
            return View(pedido);
        }

        [HttpGet]
        public int ObtenerPORevision(int? id)
        {
            OrdenesCompra pedido = objPedido.ConsultarListaPO(id);
            SeleccionarClientes(pedido);
            SeleccionarClienteFinal(pedido);
            /*int revisiones = objRevision.ObtenerNumeroRevisiones(id);
            int identificador = 0;
            string rev;
            if (revisiones == 0 && pedido.IdStatus != 3)
            {
                identificador = revisiones + 1;
                rev = pedido.PO + "-REV" + identificador;
            }
            else
            {
                identificador = revisiones + 1;
                rev = pedido.PO + "-REV" + identificador;
            }pedido.PO = rev.Replace(" ", "");*/


            pedido.IdPedido = Convert.ToInt32(id);
            pedido.FechaOrden = DateTime.Today;
            Session["id_pedido"] = id;
           // ObtenerEstadoRevisado(pedido);
            objPedido.AgregarPO(pedido);

            //Cambia estado pedido original a 5
           objPedido.ActualizarEstadoPO(Convert.ToInt32(Session["id_pedido"]));

            //Registrar en Revisado el Pedido Nuevo 
            int PedidosId = objPedido.Obtener_Utlimo_po();
            Session["idPedidoNuevo"] = PedidosId;
            int PedidoNuevo = Convert.ToInt32(Session["idPedidoNuevo"]);
            if (PedidoNuevo != 0)
            {
                Revision revisionPO = new Revision()
                {
                    IdPedido = Convert.ToInt32(Session["id_pedido"]),
                    IdRevisionPO = Convert.ToInt32(Session["idPedidoNuevo"]),
                    FechaRevision = DateTime.Today,
                    IdStatus = pedido.IdStatus

                };
                objRevision.AgregarRevisionesPO(revisionPO);
            }
            //Obtener los estilos por ID Pedido Anterior
            List<POSummary> listaItems = objItems.ListaEstilosPorPO(Convert.ToInt32(Session["id_pedido"])).ToList();
            POSummary estilos = new POSummary();
            foreach (var item in listaItems)
            {
                estilos.EstiloItem = item.EstiloItem;
                estilos.IdColor = item.CatColores.CodigoColor;
                estilos.Cantidad = item.Cantidad;          
                estilos.Precio =item.Precio;
                estilos.PedidosId = item.PedidosId;
                estilos.IdGenero = item.CatGenero.GeneroCode;
                estilos.IdTela = item.IdTela;
                estilos.TipoCamiseta = item.CatTipoCamiseta.TipoProducto;
                estilos.IdItems = item.IdItems;
                Session["id_estilo"] = estilos.IdItems;
                int? idEstilo = Convert.ToInt32(Convert.ToInt32(Session["id_estilo"]));
                estilos.PedidosId = Convert.ToInt32(Session["idPedidoNuevo"]);
                objItems.AgregarItems(estilos);

                //Obtener la lista de tallas del item
                List<ItemTalla> listaTallas = objTallas.ListaTallasPorSummary(idEstilo).ToList();
                ItemTalla tallas = new ItemTalla();
               
                foreach (var itemT in listaTallas)
                {

                    tallas.Talla = itemT.Talla;
                    tallas.Cantidad = itemT.Cantidad;
                    tallas.Extras = itemT.Extras;
                    tallas.Ejemplos = itemT.Ejemplos;
                    tallas.IdSummary = objItems.Obtener_Utlimo_Item();

                    objTallas.RegistroTallas(tallas);
                }
                
            }
            return Convert.ToInt32(Session["idPedidoNuevo"]);
        }

        [HttpGet]
        public ActionResult Revision(int? id)
        {
            int idPedido = ObtenerPORevision(id);
            Session["idPedidoRevision"]= idPedido;
            POSummary summary = new POSummary();
            ListaGenero(summary);
            ListaTela(summary);
            ListaTipoCamiseta(summary);
            if (id == null)
            {
                return View();
            }
            OrdenesCompra pedidos = objPedido.ConsultarListaPO(idPedido);
            ListasClientes(pedidos);
            

           /* if(id != null)
            {
                RegistrarRevisionPO(pedidos);
            }   */


            if (pedidos == null)
            {
                return View();
            }

            return View(pedidos);

        }
        [HttpPost]
        public ActionResult RegistrarRevisionPO([Bind] OrdenesCompra pedido)
        {
            List<POSummary> listaItems = objItems.ListaItemsPorPO(pedido.IdPedido).ToList();


           // List<ItemTalla> listaTallas = objTallas.ListaTallasPorEstilo(id).ToList();
            return View(pedido);
        }

        public ActionResult CancelarPO(int id)
        {
            objPedido.ActualizarEstadoPOCancelado(id);
            TempData["cancelarPO"] = "Se cancelo correctamente la orden de compra.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Revision(int id, [Bind] OrdenesCompra pedido)
        {
            string cliente = Request.Form["Nombre"].ToString();
            pedido.Cliente = Int32.Parse(cliente);

            string clienteFinal = Request.Form["NombreCliente"].ToString();
            pedido.ClienteFinal = Int32.Parse(clienteFinal);
            /*if (id != pedido.IdPedido)
            {
                return View();
            }*/
            /* if (ModelState.IsValid)
             {
                 objPedido.ActualizarPedidos(pedido);
                 TempData["pedidoRevision"] = "Se registro correctamente la revisión de la orden de compra .";
                 return RedirectToAction("Index");
             }
             else
             {
                 TempData["pedidoRevisionError"] = "No se pudo registrar la revisión de la orden de compra, intentelo más tarde.";
             }*/
            return View(pedido);
        }



        [HttpGet]
        public ActionResult EditarEstilo(int? id)
        {
            if (id == null)
            {
                return View();
            }


            POSummary items = objItems.ConsultarListaEstilos(id);
            ListaGenero(items);
            ListaTela(items);
            ListaTipoCamiseta(items);
            items.CatColores = objColores.ConsultarListaColores(items.ColorId);
            items.ItemDescripcion = objEst.ConsultarListaItemDesc(items.IdItems);
            items.PedidosId = items.PedidosId;
            SeleccionarGenero(items);
            SeleccionarTela(items);
            SeleccionarTipoCamiseta(items);


            if (items == null)
            {

                return View();
            }

            return PartialView(items);

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEstilo(int id, [Bind] POSummary items)
        {
            items.IdItems = id;
           /* if (id != items.IdItems)
            {
                return View();
            }*/
            string genero = Request.Form["Genero"].ToString();
            items.Id_Genero = objGenero.ObtenerIdGenero(genero);
            string tipoCamiseta = Request.Form["DescripcionTipo"].ToString();
            items.IdCamiseta = objTipoC.ObtenerIdTipoCamiseta(tipoCamiseta);
            string tela = Request.Form["Tela"].ToString();
            items.IdTela = Int32.Parse(tela);
            string estilo = items.ItemDescripcion.ItemEstilo;
            items.IdEstilo = objEst.ObtenerIdEstilo(estilo);
            string color = items.CatColores.CodigoColor;
            items.ColorId = objColores.ObtenerIdColor(color);
            if (items.IdItems != 0)
            {
                objItems.ActualizarEstilos(items);
                TempData["itemEditar"] = "Se modifico correctamente el estilo.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["itemEditarError"] = "No se pudo modificar el estilo, intentelo más tarde.";
            }
          return View(items);
        }


        [HttpPost]     
        public ActionResult RegistrarPO([Bind] OrdenesCompra ordenCompra,string po, int VPO, DateTime FechaCancel, DateTime FechaOrden, int Cliente, int Clientefinal, int TotalUnidades)
        {
            ListaEstados(ordenCompra);                          
            objPedido.AgregarPO(ordenCompra);           
            
            return View(ordenCompra);
        }

        public void SeleccionarClientes(OrdenesCompra pedido)
        {
            List<CatCliente> listaClientes = pedido.LCliente;
            listaClientes = objCliente.ListaClientes().ToList();
            pedido.CatCliente = objCliente.ConsultarListaClientes(pedido.Cliente);
            pedido.CatCliente.Customer = pedido.Cliente;
            ViewBag.listCliente = new SelectList(listaClientes, "Customer", "Nombre", pedido.Cliente);


        }

        public void SeleccionarClienteFinal(OrdenesCompra pedido)
        {
        
            List<CatClienteFinal> listaClientesFinal = pedido.LClienteFinal;
            listaClientesFinal = objClienteFinal.ListaClientesFinal().ToList();
            pedido.CatClienteFinal = objClienteFinal.ConsultarListaClientesFinal(pedido.ClienteFinal);
            pedido.CatClienteFinal.CustomerFinal = pedido.ClienteFinal;
            ViewBag.listClienteFinal = new SelectList(listaClientesFinal, "CustomerFinal", "NombreCliente", pedido.ClienteFinal);

        }

        public void SeleccionarGenero(POSummary items)
        {

            List<CatGenero> listaGenero = items.ListaGeneros;
            listaGenero = objGenero.ListaGeneros().ToList();
            items.CatGenero = objGenero.ConsultarListaGenero(items.Id_Genero);
            items.CatGenero.IdGender = items.Id_Genero;            
            ViewBag.listGenero = new SelectList(listaGenero, "GeneroCode", "Genero", items.CatGenero.GeneroCode);

        }

        public void SeleccionarTela(POSummary items)
        {

            List<CatTela> listaTela = items.ListaTelas;
            listaTela = objTela.ListaTela().ToList();
            items.CatTela = objTela.ConsultarListaTelas(items.IdTela);
            items.CatTela.Id_Tela = items.IdTela;      
            ViewBag.listTela = new SelectList(listaTela, "Id_Tela", "Tela", items.IdTela);
        }

        public void SeleccionarTipoCamiseta(POSummary items)
        {

            List<CatTipoCamiseta> listaTipoCamiseta = items.ListaTipoCamiseta;
            listaTipoCamiseta = objTipoC.ListaTipoCamiseta().ToList();
            items.CatTipoCamiseta = objTipoC.ConsultarListaCamisetas(items.IdCamiseta);
            items.CatTipoCamiseta.IdTipo= items.IdCamiseta;           
            ViewBag.listTipoCamiseta = new SelectList(listaTipoCamiseta, "TipoProducto", "DescripcionTipo", items.CatTipoCamiseta.TipoProducto);
        }

        public void ListasClientes(OrdenesCompra pedido)
        {
            List<CatCliente> listaClientes = pedido.ListaClientes;
            listaClientes = objCliente.ListaClientes().ToList();

            ViewBag.listCliente = new SelectList(listaClientes, "Customer", "Nombre", pedido.Cliente);

            List<CatClienteFinal> listaClientesFinal = pedido.ListaClientesFinal;
            listaClientesFinal = objClienteFinal.ListaClientesFinal().ToList();
            ViewBag.listClienteFinal = new SelectList(listaClientesFinal, "CustomerFinal", "NombreCliente", pedido.ClienteFinal);
        }

        public void ObtenerIdClientes(OrdenesCompra pedido)
        {
            string cliente = Request.Form["listCliente"].ToString();
            pedido.Cliente = Int32.Parse(cliente);
            pedido.CatCliente = objCliente.ConsultarListaClientes(pedido.Cliente);


            string clienteFinal = Request.Form["listClienteFinal"].ToString();
            pedido.ClienteFinal = Int32.Parse(clienteFinal);
            pedido.CatClienteFinal = objClienteFinal.ConsultarListaClientesFinal(pedido.ClienteFinal);



        }

        public void ListaGenero(POSummary summary)
        {
            List<CatGenero> listaGenero = summary.ListaGeneros;
            listaGenero = objGenero.ListaGeneros().ToList();

            ViewBag.listGenero = new SelectList(listaGenero, "GeneroCode", "Genero", summary.IdGenero);

        }

        public void ListaTela(POSummary summary)
        {
            List<CatTela> listaTela = summary.ListaTelas;
            listaTela = objTela.ListaTela().ToList();

            ViewBag.listTela = new SelectList(listaTela, "Id_Tela", "Tela", summary.IdTela);

        }

        public void ListaTipoCamiseta(POSummary summary)
        {
            List<CatTipoCamiseta> listaTipoCamiseta = summary.ListaTipoCamiseta;
            listaTipoCamiseta = objTipoC.ListaTipoCamiseta().ToList();

            ViewBag.listTipoCamiseta = new SelectList(listaTipoCamiseta, "TipoProducto", "DescripcionTipo", summary.TipoCamiseta);

        }

        public void ListaEstados(OrdenesCompra pedido)
        {
            List<CatStatus> listaEstados = pedido.ListaCatStatus;
            listaEstados = objEstados.ListarEstados().ToList();

            ViewBag.listEstados = new SelectList(listaEstados, "IdStatus", "Estado", pedido.IdStatus);
            foreach (var item in listaEstados)
            {
                if(item.IdStatus == 1)
                {
                    pedido.IdStatus = item.IdStatus;
                }
               
            }

        }
        public void ObtenerEstadoRevisado(OrdenesCompra pedido)
        {
            List<CatStatus> listaEstados = pedido.ListaCatStatus;
            listaEstados = objEstados.ListarEstados().ToList();

            ViewBag.listEstados = new SelectList(listaEstados, "IdStatus", "Estado", pedido.IdStatus);
            foreach (var item in listaEstados)
            {
                if (item.IdStatus == 1)
                {
                    pedido.IdStatus = item.IdStatus;
                }

            }

        }


    }
}