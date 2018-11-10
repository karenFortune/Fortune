using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Arte
{
    public class ArteData
    {

        //Muestra la lista de Usuarios 
        public IEnumerable<IMAGEN_ARTE> ListaArtes(int id)
        {
             Conexion conn = new Conexion();
            List<IMAGEN_ARTE> listArte = new List<IMAGEN_ARTE>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;               
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select IA.IdImgArte, IA.IdEstilo,IA.StatusArte,A.IdSummary, C.NAME_FINAL, IA.StatusPNL, ID.ITEM_STYLE, IA.extensionArte, IA.extensionPNL,P.PO from IMAGEN_ARTE IA " +
                    "INNER JOIN ITEM_DESCRIPTION ID ON ID.ITEM_ID = IA.IdEstilo " +
                    "INNER JOIN ARTE A ON A.IdImgArte=IA.IdImgArte " +
                    "INNER JOIN PO_SUMMARY PS ON PS.ID_PO_SUMMARY=A.IdSummary " +
                    "INNER JOIN PEDIDO P ON P.ID_PEDIDO=PS.ID_PEDIDOS " +
                    "INNER JOIN CAT_CUSTOMER_PO C ON P.CUSTOMER_FINAL=C.CUSTOMER_FINAL WHERE IA.IdImgArte='" + id + "'";
                leerFilas = comando.ExecuteReader();

                while (leerFilas.Read())
                {

                    IMAGEN_ARTE arte = new IMAGEN_ARTE()
                    {
                        IdImgArte = Convert.ToInt32(leerFilas["IdImgArte"]),
                        IdEstilo = Convert.ToInt32(leerFilas["IdEstilo"]),
                        StatusArte = Convert.ToInt32(leerFilas["StatusArte"]),
                        StatusPNL = Convert.ToInt32(leerFilas["StatusPNL"]),
                        Estilo = leerFilas["ITEM_STYLE"].ToString(),
                        Tienda = leerFilas["NAME_FINAL"].ToString(),
                        extensionArte = leerFilas["extensionArte"].ToString(),
                        extensionPNL = leerFilas["extensionPNL"].ToString(),
                        PO = leerFilas["PO"].ToString()

                    };
                    ARTE catArte = new ARTE()
                    {
                        IdSummary = Convert.ToInt32(leerFilas["IdSummary"])
                    };

                    //Obtener el idEstado Arte 
                    if (arte.StatusArte == 1)
                    {
                        arte.EstadosArte = EstatusArte.APPROVED;
                    }
                    else if (arte.StatusArte == 2)
                    {
                        arte.EstadosArte = EstatusArte.REVIEWED;
                    }
                    else if (arte.StatusArte == 3)
                    {
                        arte.EstadosArte = EstatusArte.PENDING;
                    }
                    //Obtener el idEstado PNL
                    if (arte.StatusPNL == 1)
                    {
                        arte.EstadosPNL = EstatusPNL.APPROVED;
                    }
                    else if (arte.StatusPNL == 2)
                    {
                        arte.EstadosPNL = EstatusPNL.REVIEWED;
                    }
                    else if (arte.StatusPNL == 3)
                    {
                        arte.EstadosPNL = EstatusPNL.PENDING;
                    }

                    arte.CATARTE = catArte;
                    listArte.Add(arte);

                }
                leerFilas.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }          

            return listArte;
        }

        //Muestra la lista de Usuarios 
        public IEnumerable<IMAGEN_ARTE> ListaInvArtes()
        {
            Conexion conn = new Conexion();
            List<IMAGEN_ARTE> listArte = new List<IMAGEN_ARTE>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;                
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select IA.IdImgArte, IA.IdEstilo,IA.StatusArte, IA.StatusPNL, ID.ITEM_STYLE, ID.DESCRIPTION, IA.extensionArte, IA.extensionPNL from IMAGEN_ARTE IA " +
                    "INNER JOIN ITEM_DESCRIPTION ID ON ID.ITEM_ID = IA.IdEstilo";
                leerFilas = comando.ExecuteReader();

                while (leerFilas.Read())
                {

                    IMAGEN_ARTE arte = new IMAGEN_ARTE()
                    {
                        IdImgArte = Convert.ToInt32(leerFilas["IdImgArte"]),
                        IdEstilo = Convert.ToInt32(leerFilas["IdEstilo"]),
                        StatusArte = Convert.ToInt32(leerFilas["StatusArte"]),
                        StatusPNL = Convert.ToInt32(leerFilas["StatusPNL"]),
                        Estilo = leerFilas["ITEM_STYLE"].ToString(),
                        extensionArte = leerFilas["extensionArte"].ToString(),
                        extensionPNL = leerFilas["extensionPNL"].ToString(),
                        DescripcionEstilo = leerFilas["DESCRIPTION"].ToString()


                    };

                    /* if (!Convert.IsDBNull(leerFilas["imgArte"]))
                     {
                         arte.imgArte = (byte[])leerFilas["imgArte"];
                     }

                     if (!Convert.IsDBNull(leerFilas["imgPNL"]))
                     {
                         arte.imgPNL = (byte[])leerFilas["imgPNL"];
                     }*/

                    //Obtener el idEstado Arte 
                    if (arte.StatusArte == 1)
                    {
                        arte.EstadosArte = EstatusArte.APPROVED;
                    }
                    else if (arte.StatusArte == 2)
                    {
                        arte.EstadosArte = EstatusArte.REVIEWED;
                    }
                    else if (arte.StatusArte == 3)
                    {
                        arte.EstadosArte = EstatusArte.PENDING;
                    }
                    //Obtener el idEstado PNL
                    if (arte.StatusPNL == 1)
                    {
                        arte.EstadosPNL = EstatusPNL.APPROVED;
                    }
                    else if (arte.StatusPNL == 2)
                    {
                        arte.EstadosPNL = EstatusPNL.REVIEWED;
                    }
                    else if (arte.StatusPNL == 3)
                    {
                        arte.EstadosPNL = EstatusPNL.PENDING;
                    }


                    listArte.Add(arte);

                }
                leerFilas.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
            
            

            return listArte;
        }

        public void ActualizarArteEstilo (int idArte, byte[] imagenArte, byte[] imagenPNL, int idStatus)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            Conexion conex = new Conexion();
            try
            {
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "UPDATE IMAGEN_ARTE SET StatusArte='" + idStatus + "', imgArte='" + imagenArte + "',imgPNL='" + imagenPNL + "'  WHERE IdImgArte='" + idArte + "'";
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();               
            }
            finally {
                conex.CerrarConexion();
                conex.Dispose();
            }

        }

        public void AgregarArteImagen(IMAGEN_ARTE arte)
        {
            Conexion conn = new Conexion();
            SqlCommand comando = new SqlCommand();

            comando.Connection = conn.AbrirConexion();
            try
            {
                comando.CommandText = "INSERT INTO  IMAGEN_ARTE (IdEstilo,StatusArte,StatusPNL,extensionArte,extensionPNL) " +
                    " VALUES('"+ arte.IdEstilo + "','" + arte.StatusArte + "','" + arte.StatusPNL + "','" + arte.extensionArte + "','" + arte.extensionPNL + "')";
                comando.ExecuteNonQuery();
            }
            finally {
                conn.CerrarConexion();
                conn.Dispose();
            }

        }

        public void AgregarArte(int? idImgArte, int? idSummary)
        {
            Conexion conn = new Conexion();
            SqlCommand comando = new SqlCommand();

            comando.Connection = conn.AbrirConexion();
            try
            {
                comando.CommandText = "INSERT INTO ARTE (IdImgArte,IdSummary) " +
                    " VALUES('" + idImgArte + "','" + idSummary + "')";
                comando.ExecuteNonQuery();
            }
            finally {
                conn.CerrarConexion();
                conn.Dispose();
            }

        }

        public int Obtener_Utlimo_Arte_Imagen()
        {
            Conexion conex = new Conexion();
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            try
            {
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "SELECT IdImgArte FROM IMAGEN_ARTE WHERE IdImgArte = (SELECT MAX(IdImgArte) FROM IMAGEN_ARTE) ";
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    return Convert.ToInt32(reader["IdImgArte"]);
                }
                
            }
            finally {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return 0;
        }

        public int BuscarIdEstiloArteImagen(int? idEstilo)
        {
            Conexion conex = new Conexion();
            int idEst = 0;
            try
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataReader leerF = null;             
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "select IA.IdEstilo from IMAGEN_ARTE IA where IA.IdEstilo='" + idEstilo + "'";
                cmd.CommandType = CommandType.Text;
                leerF = cmd.ExecuteReader();
                while (leerF.Read())
                {
                    idEst = Convert.ToInt32(leerF["IdEstilo"]);

                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }              
                return idEst;
            }



        public IMAGEN_ARTE BuscarEstiloArteImagen(int? idEstilo)
        {
            Conexion conex = new Conexion();
            IMAGEN_ARTE arte = new IMAGEN_ARTE();
            try
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataReader leerF = null;
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "select * from IMAGEN_ARTE IA where IA.IdEstilo='" + idEstilo + "'";
                cmd.CommandType = CommandType.Text;
                leerF = cmd.ExecuteReader();               
                while (leerF.Read())
                {

                    arte.IdImgArte = Convert.ToInt32(leerF["IdImgArte"]);
                    arte.StatusArte = Convert.ToInt32(leerF["StatusArte"]);
                    arte.StatusPNL = Convert.ToInt32(leerF["StatusPNL"]);
                    arte.extensionArte = leerF["extensionArte"].ToString();
                    arte.extensionPNL = leerF["extensionPNL"].ToString();
                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }            
            return arte;
        }

        //Permite obtener el cliente final de un estilo
        public string ObtenerclienteEstilo(int? idSummary, int idArte)
        {
            Conexion conex = new Conexion();
            string nombre = "";
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "select  C.NAME_FINAL from IMAGEN_ARTE IA " +
                    "INNER JOIN ITEM_DESCRIPTION ID ON ID.ITEM_ID = IA.IdEstilo " +
                    "INNER JOIN ARTE A ON A.IdImgArte=IA.IdImgArte " +
                    "INNER JOIN PO_SUMMARY PS ON PS.ID_PO_SUMMARY=A.IdSummary " +
                    "INNER JOIN PEDIDO P ON P.ID_PEDIDO=PS.ID_PEDIDOS " +
                    "INNER JOIN CAT_CUSTOMER_PO C ON P.CUSTOMER_FINAL=C.CUSTOMER_FINAL " +
                    " WHERE  A.IdSummary='" + idSummary + "' AND IA.IdImgArte='" + idArte + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    nombre = leerF["NAME_FINAL"].ToString();

                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }           
            return nombre;
        }

      
        public byte[] ObtenerImagenArte(int idArte)
        {

            byte[] iArte = null;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT imgArte FROM IMAGEN_ARTE WHERE IdImgArte='" + idArte + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {

                    if (!Convert.IsDBNull(leerF["imgArte"]))
                    {
                        iArte = (byte[])leerF["imgArte"];
                    }


                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }            
            return iArte;
        }       
     
    }
}