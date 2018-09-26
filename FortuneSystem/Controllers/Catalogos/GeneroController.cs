using FortuneSystem.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers.Catalogos
{
    public class GeneroController : Controller
    {
        // GET: Genero
        CatGeneroData objGenero = new CatGeneroData();
        public ActionResult Index()
        {
            List<CatGenero> listaGenero = new List<CatGenero>();
            listaGenero = objGenero.ListaGeneros().ToList();
            return View(listaGenero);
        }

        [HttpGet]
        public ActionResult CrearGenero()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearGenero([Bind] CatGenero generos)
        {
            if (ModelState.IsValid)
            {
                objGenero.AgregarGenero(generos);
                TempData["generoOK"] = "Se registro correctamente el género.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["generoError"] = "No se pudo registrar el género, intentelo más tarde.";
            }
            return View(generos);
        }

        [HttpGet]
        public ActionResult Detalles(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatGenero generos = objGenero.ConsultarListaGenero(id);
            if (generos == null)
            {
                return View();
            }
            return View(generos);
        }

        [HttpGet]
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatGenero generos = objGenero.ConsultarListaGenero(id);
            if (generos == null)
            {
                return View();
            }

            return View(generos);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, [Bind] CatGenero generos)
        {
            if (id != generos.IdGender)
            {
                return View();
            }
            if (ModelState.IsValid)
            {
                objGenero.ActualizarGenero(generos);
                TempData["generoEditar"] = "Se modifico correctamente el género.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["generoEditarError"] = "No se pudo modificar el género, intentelo más tarde.";
            }
            return View(generos);
        }

        [HttpGet]
        public ActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatGenero generos = objGenero.ConsultarListaGenero(id);


            if (generos == null)
            {
                return View();
            }
            return View(generos);

        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfimacionEliminar(int? id)
        {
            objGenero.EliminarGenero(id);
            TempData["generoEliminar"] = "Se elimino correctamente el género.";
            return RedirectToAction("Index");
        }



    }
}