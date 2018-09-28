using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using FortuneSystem.Models.Almacen;

namespace FortuneSystem.Models.Almacen
{
    public class DatosInventario
    {
        private Conexion conn = new Conexion();
        private SqlCommand comando = new SqlCommand();
        private SqlDataReader leerFilas = null;

        //**************************************************************************
        public List<Inventario> ListaInventario(){
            List<Inventario> listInventario = new List<Inventario>();
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = " SELECT i.id_inventario,s.sucursal,p.PO,pa.pais,f.fabricante,i.mill_po, i.amt_item, ci.categoria,cc.CODIGO_COLOR,cc.DESCRIPCION,"
                +" bt.body_type,g.GENERO,ft.FABRIC,fp.fabric_percent,i.total,cis.TALLA,c.NAME,cf.NAME_FINAL, i.minimo,i.notas from"
                +" inventario i,sucursales s,PEDIDO p,paises pa,fabricantes f,categorias_inventarios ci,CAT_COLORES cc,body_types bt,CAT_GENDER g,CAT_FABRIC_CODES ft,fabric_percents fp,CAT_ITEM_SIZE cis,"
                +" CAT_CUSTOMER c,CAT_CUSTOMER_PO cf where i.id_sucursal=s.id_sucursal and i.id_pedido=p.ID_PEDIDO and i.id_pais=pa.id_pais and i.id_fabricante=f.id_fabricante and "
                +" i.id_categoria_inventario=ci.id_categoria and i.id_color=cc.ID_COLOR and i.id_body_type=bt.id_body_type and i.id_genero=g.ID_GENDER and "
                +" i.id_fabric_type=ft.ID and i.id_fabric_percent=fp.id_fabric_percent and i.id_size=cis.ID and i.id_customer=c.CUSTOMER and "
                +" i.id_customer_final=cf.CUSTOMER_FINAL ";
            leerFilas = comando.ExecuteReader();
            while (leerFilas.Read()){
                Inventario i = new Inventario();
                i.id_inventario = Convert.ToInt32(leerFilas["id_inventario"]);
                i.sucursal= Convert.ToString(leerFilas["sucursal"]);
                i.po= Convert.ToString(leerFilas["PO"]);
                i.pais= Convert.ToString(leerFilas["pais"]);
                i.fabricante= Convert.ToString(leerFilas["fabricante"]);
                i.mill_po= Convert.ToString(leerFilas["mill_po"]);
                i.amt_item= Convert.ToString(leerFilas["amt_item"]);
                i.categoria_inventario= Convert.ToString(leerFilas["categoria_inventario"]);
                i.color= Convert.ToString(leerFilas["CODIGO_COLOR"])+" "+ Convert.ToString(leerFilas["DESCRIPCION"]); 
                i.body_type= Convert.ToString(leerFilas["body_type"]);
                i.genero= Convert.ToString(leerFilas["GENERO"]);
                i.fabric_type= Convert.ToString(leerFilas["fabric_type"]);
                i.fabric_percent= Convert.ToString(leerFilas["fabric_percent"]);
                i.total= Convert.ToInt32(leerFilas["total"]);
                i.size= Convert.ToString(leerFilas["TALLA"]);
                i.customer= Convert.ToString(leerFilas["NAME"]);
                i.final_customer = Convert.ToString(leerFilas["NAME_FINAL"]);
                i.notas = Convert.ToString(leerFilas["notas"]);
                listInventario.Add(i);
            }
            leerFilas.Close();
            conn.CerrarConexion();
            return listInventario;
        }









    }
}