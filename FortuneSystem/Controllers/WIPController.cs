using FortuneSystem.Models;
using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Pedidos;
using FortuneSystem.Models.POSummary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace FortuneSystem.Controllers
{
    public class WIPController : Controller
    {
        PedidosData objPedido = new PedidosData();
        CatComentariosData objComent = new CatComentariosData();
        DescripcionItemData objSummary = new DescripcionItemData();
        public ActionResult Index()
        {
			OrdenesCompra pedido = new OrdenesCompra();
			ListaPeriodos(pedido);
		   /*List<OrdenesCompra> listaPedidos = new List<OrdenesCompra>();
            listaPedidos = objPedido.ListaOrdenCompraWIP().ToList();*/
			return View(); 
        }

        /*public JsonResult GetListadoPedido(string sidx, string sort, int page, int rows)
        {
            sort = (sort == null) ? "" : sort;
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            List<OrdenesCompra> listaPedidos = new List<OrdenesCompra>();
            listaPedidos = objPedido.ListaOrdenCompraWIP().ToList();
            int totalRecords = listaPedidos.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = listaPedidos
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }*/


        public JsonResult ListadoPedido()
        {           
            List<OrdenesCompra> listaPedidos = new List<OrdenesCompra>();
            int estadoTab = 1;
            listaPedidos = CatalagoWIP(listaPedidos, estadoTab);
            return Json(listaPedidos, JsonRequestBehavior.AllowGet);
        }

		public void ListaPeriodos(OrdenesCompra pedido)
		{		
			List<Periodo>  listaYear = objPedido.ListadoPeriodos().ToList();	
			List<SelectListItem> items = new SelectList(listaYear, "NumPeriodo", "NumPeriodo", pedido.IdPeriodo).ToList();
			items.Insert(0, (new SelectListItem { Text = "ALL", Value = "0" }));

			ViewBag.listPeriodo = items;
		}


		public JsonResult ListadoPedidoShipping()
        {            
            List<OrdenesCompra> listaPedidos = new List<OrdenesCompra>();
            int estadoTab = 2;
           listaPedidos = CatalagoWIP(listaPedidos, estadoTab);

            return Json(listaPedidos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListadoPedidoCancelled()
        {
            List<OrdenesCompra> listaPedidos = new List<OrdenesCompra>();
            int estadoTab = 3;
            listaPedidos = CatalagoWIP(listaPedidos, estadoTab);

            return Json(listaPedidos, JsonRequestBehavior.AllowGet);
        }

        public List<OrdenesCompra> CatalagoWIP(List<OrdenesCompra> listaPedidos, int estadoTab)
        {
			listaPedidos = objPedido.ListaOrdenCompraWIP(estadoTab).ToList();
			return listaPedidos;
        }

         public JsonResult ListadoComentariosWIP()
        {
            string tipoArchivo = "WIP";   
			List<CatComentarios> listaComentarios = objComent.ListadoAllWIPComentarios(tipoArchivo).ToList();

            return Json(listaComentarios, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListadoComentariosShipped()
        {
            string tipoArchivo = "SHIPPED";
			List<CatComentarios> listaComentarios = objComent.ListadoAllWIPComentarios(tipoArchivo).ToList();

            return Json(listaComentarios, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListadoComentariosCancelled()
        {
            string tipoArchivo = "CANCELLED";
			List<CatComentarios> listaComentarios = objComent.ListadoAllWIPComentarios(tipoArchivo).ToList();

            return Json(listaComentarios, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public void RegistrarCometarioWIP(string Comentario, int IdSummary, string TipoArchivo)
        {
            DateTime fecha = DateTime.Now;
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            CatComentarios catComentario = new CatComentarios()
            {
                FechaComentario = fecha,
                IdSummary = IdSummary,
                Comentario = Comentario,
                IdUsuario = noEmpleado,
                TipoArchivo = TipoArchivo
            };
            objComent.AgregarComentario(catComentario);

        }
        [HttpPost]
        public void RegistrarFechaUCC(DateTime FechaUCC, int IdSummary)
        {
           // DateTime fecha = DateTime.Now;
          //  int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            POSummary poSummary = new POSummary()
            {
                IdItems = IdSummary,
                FechaUCC= FechaUCC
            };
            objSummary.AgregarFechaUCC(poSummary);

        }

		public void ActualizarSucursalIdSummary(int IdSucursal, int IdSummary)
		{
			SqlCommand cmd = new SqlCommand();
			SqlDataReader reader;
			Conexion conex = new Conexion();
			try
			{
				cmd.Connection = conex.AbrirConexion();
				cmd.CommandText = "UPDATE PO_SUMMARY SET ID_SUCURSAL='"+ IdSucursal + "' WHERE ID_PO_SUMMARY='" + IdSummary + "'";
				cmd.CommandType = CommandType.Text;
				reader = cmd.ExecuteReader();
				conex.CerrarConexion();
			}
			finally
			{
				conex.CerrarConexion();
				conex.Dispose();
			}

		}
	}
}