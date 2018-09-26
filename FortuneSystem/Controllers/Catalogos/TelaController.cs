using FortuneSystem.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers.Catalogos
{
    public class TelaController : Controller
    {
        // GET: Tela
        CatTelaData objTela = new CatTelaData();
        public ActionResult Index()
        {
            List<CatTela> listaTela = new List<CatTela>();
            listaTela = objTela.ListaTela().ToList();
            return View(listaTela);
        }

        [HttpGet]
        public ActionResult CrearTela()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearTela([Bind] CatTela telas)
        {
            if (ModelState.IsValid)
            {
                objTela.AgregarTelas(telas);
                TempData["telaOK"] = "Se registro correctamente la tela.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["telaError"] = "No se pudo registrar la tela, intentelo más tarde.";
            }
            return View(telas);
        }

        [HttpGet]
        public ActionResult Detalles(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatTela telas = objTela.ConsultarListaTelas(id);
            if (telas == null)
            {
                return View();
            }
            return View(telas);
        }

        [HttpGet]
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatTela telas = objTela.ConsultarListaTelas(id);
            if (telas == null)
            {
                return View();
            }

            return View(telas);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, [Bind] CatTela telas)
        {
            if (id != telas.Id_Tela)
            {
                return View();
            }
            if (ModelState.IsValid)
            {
                objTela.ActualizarTelas(telas);
                TempData["telaEditar"] = "Se modifico correctamente la tela.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["telaEditarError"] = "No se pudo modificar la tela, intentelo más tarde.";
            }
            return View(telas);
        }

        [HttpGet]
        public ActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatTela telas = objTela.ConsultarListaTelas(id);


            if (telas == null)
            {
                return View();
            }
            return View(telas);

        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfimacionEliminar(int? id)
        {
            objTela.EliminarTelas(id);
            TempData["telaEliminar"] = "Se elimino correctamente la tela.";
            return RedirectToAction("Index");
        }
    }
}