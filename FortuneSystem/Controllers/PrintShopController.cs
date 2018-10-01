using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Pedidos;
using FortuneSystem.Models.PrintShop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers
{
    public class PrintShopController : Controller
    {
        PedidosData objPedido = new PedidosData();
        CatClienteData objCliente = new CatClienteData();
        CatClienteFinalData objClienteFinal = new CatClienteFinalData();
        // GET: PrintShop
        public ActionResult Index()
        {
            List<OrdenesCompra> listaPedidos = new List<OrdenesCompra>();
            listaPedidos = objPedido.ListaOrdenCompra().ToList();
            return View(listaPedidos);
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
            pedido.IdPedido = Convert.ToInt32(id);

            if (pedido == null)
            {
                return View();
            }
            return View(pedido);
        }
    }
}