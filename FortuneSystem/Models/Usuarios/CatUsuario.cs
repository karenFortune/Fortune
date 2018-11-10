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
       // [Required(ErrorMessage = "Ingrese el No. Empleado.")]
        [Required]
        [Display(Name = "NO.EMPLOYEE")]
        public int NoEmpleado { get; set; }
        // [Required(ErrorMessage = "Ingrese el Nombre(s).")]
        [Required]
        [Display(Name = "NAME")]
        public string Nombres { get; set; }
        //[Required(ErrorMessage = "Ingrese el Apellido(s).")]
        [Required]
        [Display(Name = "SURNAMES")]
        public string Apellidos { get; set; }

        [ForeignKey("Cargo")]
        [Column("Cargo")]
        [Display(Name = "POSITION")]
        public int Cargo { get; set; }  
        
        public virtual CatRoles CatRoles { get; set; }
        public List<CatRoles> ListaRoles { get; set; }

        //[Required(ErrorMessage = "Ingrese el Correo Electrónico.")]
        [Required]
        [DataType(DataType.EmailAddress)]
        //[EmailAddress(ErrorMessage = "Por favor, introduce un correo electrónico valido.")]
        [EmailAddress]
        [Display(Name = "EMAIL")]
        public string Email { get; set; }
        //[Required(ErrorMessage = "Ingrese la Contraseña.")]
        [Required]
        [Display(Name = "PASSWORD")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }

        public string NombreCompleto { get; set; }
      




    }


      
}