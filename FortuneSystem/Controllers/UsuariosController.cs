using FortuneSystem.Models.Roles;
using FortuneSystem.Models.Usuarios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers
{
    public class UsuariosController : Controller
    {
        CatUsuarioData objCatUser= new CatUsuarioData();
        CatRolesData objCaRoles = new CatRolesData();
        CatUsuario usuario = new CatUsuario();
        // GET: Usuarios
        public ActionResult Index()
        {            
            //  Lista de Roles
            List<CatUsuario> listaUsuarios = new List<CatUsuario>();
            listaUsuarios = objCatUser.ListaUsuarios().ToList();               
            
            return View(listaUsuarios);
        }

       [HttpGet]
        public ActionResult CrearUsuario()
        {
            CatUsuario usuario = new CatUsuario();

            List<CatRoles> listaRoles = usuario.ListaRoles;
            listaRoles = objCaRoles.ListaRoles().ToList();
            ViewBag.listRoles = new SelectList(listaRoles, "Id", "rol", usuario.Cargo);
         
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearUsuario([Bind] CatUsuario usuario)
        {
           
            if(ModelState.IsValid)
            {
                string rol = Request.Form["listRoles"].ToString();
                usuario.Cargo=Int32.Parse(rol);
               usuario.CatRoles= objCaRoles.ConsultarListaRoles(usuario.Cargo);
                objCatUser.AgregarUsuarios(usuario);
                TempData["usuarioOK"] = "Se registro correctamente el usuario.";
                return RedirectToAction("Index");
            }else
            {
                TempData["usuarioError"] = "No se pudo registrar el usuario, intentelo más tarde.";
            }
            return View(usuario);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if(id == null)
            {
                return View();
            }

            CatUsuario usuario = objCatUser.ConsultarListaUsuarios(id);
            usuario.CatRoles = objCaRoles.ConsultarListaRoles(usuario.Cargo);
            if (usuario == null)
            {
                return View();
            }
            return View(usuario);


        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if(id == null)
            {
                return View();
            }      
           

            CatUsuario usuario = objCatUser.ConsultarListaUsuarios(id);
            List<CatRoles> listaRoles = usuario.ListaRoles;
            listaRoles = objCaRoles.ListaRoles().ToList();
            usuario.CatRoles = objCaRoles.ConsultarListaRoles(usuario.Cargo);
            usuario.CatRoles.Id = usuario.Cargo;
            ViewBag.listRoles = new SelectList(listaRoles, "Id", "rol", usuario.Cargo);


            if (usuario == null)
            {
               
                return View();
            }

            return View(usuario);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind] CatUsuario usuarios)
        {
            if (id != usuarios.Id)
            {
                return View();
            }
            if (ModelState.IsValid)
            {
                string rol = Request.Form["Rol"].ToString();
                usuarios.Cargo = Int32.Parse(rol);
                //usuario.CatRoles = objCaRoles.ConsultarListaRoles(usuario.Cargo);
                objCatUser.ActualizarUsuarios(usuarios);
                TempData["usuarioEditar"] = "Se modifico correctamente el usuario.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["usuarioEditarError"] = "No se pudo modificar el usuario, intentelo más tarde.";
            }
            return View(usuarios);
        }

        [HttpGet]
        public ActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatUsuario usuarios = objCatUser.ConsultarListaUsuarios(id);

            if(usuarios == null)
            {
                return View();
            }
            return View(usuarios);

        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfimacionEliminar(int? id)
        {
            objCatUser.EliminarUsuario(id);
            TempData["usuarioEliminar"] = "Se elimino correctamente el usuario.";
            return RedirectToAction("Index");
        }

    }
}