using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using FortuneSystem.Models.Sizes;
using System.Globalization;

namespace FortuneSystem.Models.TallasEstilos
{
    public class DatosTallasEstilos
    {
        private Conexion conn = new Conexion();
        private SqlCommand comando = new SqlCommand();
        private SqlDataReader leerFilas = null;
        public int id_po_summary;
        public string codigo_estilo, estilo;
        public DatosSizes dt = new DatosSizes();

        public List<TallaEstilo> ListaTallasEstilos()
        {
            List<TallaEstilo> listTE = new List<TallaEstilo>();
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "SELECT ri.id_recibo,ri.total,ri.packing,ri.fecha,ri.origen,ri.material,ri.notas,ri.staging, u.Nombres,u.Apellidos from recibos_item ri,Usuarios u where ri.id_po_summary='" + id_po_summary+"' and u.Id=ri.id_usuario";
            leerFilas = comando.ExecuteReader();
            while (leerFilas.Read()){
                TallaEstilo te = new TallaEstilo();
                List<Size> talla = new List<Size>();
                te.id_recibo = Convert.ToInt32(leerFilas["id_recibo"]); 
                te.total = Convert.ToInt32(leerFilas["total"]);
                te.packing = leerFilas["packing"].ToString();
                te.fecha = (Convert.ToDateTime(leerFilas["fecha"])).ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX")); ;
                te.origen= leerFilas["origen"].ToString();
                te.material= leerFilas["material"].ToString();
                te.notas= leerFilas["notas"].ToString();
                te.staging = leerFilas["staging"].ToString();
                te.usuario_recibio= leerFilas["Nombres"].ToString()+" "+leerFilas["Apellidos"].ToString();
                talla = dt.ListaTallas(te.id_recibo).ToList();
                te.ListaTallas = talla;
                listTE.Add(te);
            }
            leerFilas.Close();
            conn.CerrarConexion();
            return listTE;
        }

        public string buscar_estilo_po_summary(int pos) {
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "SELECT ID.ITEM_STYLE,ID.DESCRIPTION FROM PO_SUMMARY PO, ITEM_DESCRIPTION ID WHERE PO.ITEM_ID=ID.ITEM_ID AND PO.ID_PO_SUMMARY='" + pos + "' ";
            leerFilas = comando.ExecuteReader();
            while (leerFilas.Read()){
                estilo= Convert.ToString(leerFilas["ITEM_STYLE"]) +" "+ Convert.ToString(leerFilas["DESCRIPTION"]);
            }
            leerFilas.Close();
            conn.CerrarConexion();
            return estilo;
        }




    }
}


