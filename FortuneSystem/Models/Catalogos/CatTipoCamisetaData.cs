using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatTipoCamisetaData
    {
        private Conexion conn = new Conexion();
        private SqlCommand comando = new SqlCommand();
        private SqlDataReader leer = null;


        public IEnumerable<CatTipoCamiseta> ListaTipoCamiseta()
        {
            List<CatTipoCamiseta> listTipoCamiseta = new List<CatTipoCamiseta>();
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "Listar_Tipo_Camiseta";
            comando.CommandType = CommandType.StoredProcedure;
            leer = comando.ExecuteReader();

            while (leer.Read())
            {
                CatTipoCamiseta tipoC = new CatTipoCamiseta()
                {
                    IdTipo = Convert.ToInt32(leer["ID_TYPE_CODE"]),
                    TipoProducto = leer["PRODUCT_TYPE_CODE"].ToString(),
                    DescripcionTipo = leer["DESCRIPTION"].ToString(),
                    TipoGrupo = leer["GROUP_TYPE"].ToString()
                };

                listTipoCamiseta.Add(tipoC);
            }
            leer.Close();
            conn.CerrarConexion();

            return listTipoCamiseta;
        }

        //Permite crear un nuevo tipo de camiseta
        public void AgregarCamiseta(CatTipoCamiseta camiseta)
        {
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "AgregarTipoCamiseta";
            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue("@Codigo", camiseta.TipoProducto);
            comando.Parameters.AddWithValue("@Descripcion", camiseta.DescripcionTipo);
            comando.Parameters.AddWithValue("@Grupo", camiseta.TipoGrupo);
            comando.ExecuteNonQuery();
            conn.CerrarConexion();

        }

        //Permite consultar los detalles de un tipo de camiseta
        public CatTipoCamiseta ConsultarListaCamisetas(int? id)
        {
            CatTipoCamiseta camiseta = new CatTipoCamiseta();

            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "Listar_Camiseta_Por_Id";
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("@Id", id);

            leer = comando.ExecuteReader();
            while (leer.Read())
            {

                camiseta.IdTipo = Convert.ToInt32(leer["ID_TYPE_CODE"]);
                camiseta.TipoProducto = leer["PRODUCT_TYPE_CODE"].ToString();
                camiseta.DescripcionTipo = leer["DESCRIPTION"].ToString();
                camiseta.TipoGrupo = leer["GROUP_TYPE"].ToString();
            }
            return camiseta;

        }

        //Permite actualiza la informacion de un tipo de camiseta
        public void ActualizarCamisetas(CatTipoCamiseta camiseta)
        {
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "Actualizar_Camiseta";
            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue("@Id", camiseta.IdTipo);
            comando.Parameters.AddWithValue("@Codigo", camiseta.TipoProducto);
            comando.Parameters.AddWithValue("@Descripcion", camiseta.DescripcionTipo);
            comando.Parameters.AddWithValue("@Grupo", camiseta.TipoGrupo);

            comando.ExecuteNonQuery();
            conn.CerrarConexion();
        }

        //Permite eliminar la informacion de un tipo de camiseta
        public void EliminarCamisetas(int? id)
        {
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "EliminarCamiseta";
            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue("@Id", id);

            comando.ExecuteNonQuery();
            conn.CerrarConexion();
        }

        public int ObtenerIdTipoCamiseta(string tipoCamiseta)
        {
            int idTipoCamiseta = 0;
            Conexion conex = new Conexion();
            SqlCommand coman = new SqlCommand();
            SqlDataReader leerF = null;
            coman.Connection = conex.AbrirConexion();
            coman.CommandText = "SELECT ID_TYPE_CODE FROM CAT_PRODUCT_TYPE_CODES " +
                                 "WHERE PRODUCT_TYPE_CODE='" + tipoCamiseta + "' ";
            leerF = coman.ExecuteReader();
            while (leerF.Read())
            {
                idTipoCamiseta += Convert.ToInt32(leerF["ID_TYPE_CODE"]);
            }
            leerF.Close();
            conex.CerrarConexion();
            return idTipoCamiseta;
        }
    }
}