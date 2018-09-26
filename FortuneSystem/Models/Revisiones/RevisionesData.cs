using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Revisiones
{
    public class RevisionesData
    {
        private Conexion conn = new Conexion();
        private SqlCommand comando = new SqlCommand();
        private SqlDataReader leer = null;
        //Permite crear revisiones de un PO
        public void AgregarRevisionesPO(Revisiones revision)
        {
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "AgregarRevisionPO";
            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue("@idPedido", revision.IdPedido);
            comando.Parameters.AddWithValue("@idPedidoRevision", revision.IdRevisionPO);
            comando.Parameters.AddWithValue("@dateRevision", revision.FechaRevision);
            comando.Parameters.AddWithValue("@idStatus", revision.IdStatus);

            comando.ExecuteNonQuery();
            conn.CerrarConexion();

        }

        public int ObtenerNumeroRevisiones(int? id)
        {
            int rev = 0;
            Conexion conex = new Conexion();
            SqlCommand coman = new SqlCommand();
            SqlDataReader leerF = null;
            coman.Connection = conex.AbrirConexion();
            coman.CommandText = "select COUNT(R.ID_PEDIDO) AS REVISIONES from REVISIONES_PO R " +
                    "INNER JOIN PEDIDO PE ON PE.ID_PEDIDO=R.ID_PEDIDO " +
                    "WHERE R.ID_PEDIDO='" + id + "' ";
            leerF = coman.ExecuteReader();
            while (leerF.Read())
            {
                rev += Convert.ToInt32(leerF["REVISIONES"]);
            }
            leerF.Close();
            conex.CerrarConexion();
            return rev;
        }


        public int ObtenerPedidoRevisiones(int? id)
        {
            int rev = 0;
            Conexion conex = new Conexion();
            SqlCommand coman = new SqlCommand();
            SqlDataReader leerF = null;
            coman.Connection = conex.AbrirConexion();
            coman.CommandText = "select COUNT(R.ID_REVISION_PO) AS REVISIONES from REVISIONES_PO R " +
                    "INNER JOIN PEDIDO PE ON PE.ID_PEDIDO=R.ID_REVISION_PO " +
                    "WHERE R.ID_REVISION_PO='" + id + "' ";
            leerF = coman.ExecuteReader();
            while (leerF.Read())
            {
                rev += Convert.ToInt32(leerF["REVISIONES"]);
            }
            leerF.Close();
            conex.CerrarConexion();
            return rev;
        }



    }
}