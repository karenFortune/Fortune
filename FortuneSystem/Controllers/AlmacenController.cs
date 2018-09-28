using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FortuneSystem.Models.Almacen;

namespace FortuneSystem.Controllers.Catalogos
{
    public class AlmacenController : Controller
    {
        // GET: Almacen
        DatosInventario di = new DatosInventario();
        public ActionResult Index()
        {   
            return View(di.ListaInventario());
        }
    }
}