using FortuneSystem.Models.Roles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Models.Usuarios
{
    public partial class CatUsuario
    {


        public int Id { get; set; }
        [Required(ErrorMessage = "Ingrese el No. Empleado.")]
        [Display(Name = "No. Empleado")]
        public int NoEmpleado { get; set; }
        [Required(ErrorMessage = "Ingrese el Nombre(s).")]
        [Display(Name = "Nombre")]
        public string Nombres { get; set; }
        [Required(ErrorMessage = "Ingrese el Apellido(s).")]
        public string Apellidos { get; set; }

        [ForeignKey("Cargo")]
        [Column("Cargo")]
        public int Cargo { get; set; }  
        
        public virtual CatRoles CatRoles { get; set; }
        public List<CatRoles> ListaRoles { get; set; }

        [Required(ErrorMessage = "Ingrese el Correo Electrónico.")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Por favor, introduce un correo electrónico valido.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Ingrese la Contraseña.")]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }

      




    }


      
}