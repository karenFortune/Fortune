using FortuneSystem.Models.Usuarios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Packing
{
    public class PackingData
    {
        CatUsuarioData objCatUser = new CatUsuarioData();
        //Muestra la lista de tallas de Packing por estilo
        public IEnumerable<PackingM> ListaTallasPacking(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingM> listTallas = new List<PackingM>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Lista_Packing";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();
                int i = 0;
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        Talla = leer["TALLA"].ToString()        

                    };
                    listTallas.Add(tallas);
                    i++;
                }
                if (i == 0)
                {
                    listTallas = ObtenerTallas(id);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }            

            return listTallas;
        }

        //Muestra la lista de tallas TOTAL de Packing por estilo
        public List<PackingM> ObtenerTallas(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingM> listTallas = new List<PackingM>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;                
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select S.TALLA from ITEM_SIZE I " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=I.TALLA_ITEM " +
                    "WHERE I.ID_SUMMARY= '" + id + "' ORDER BY S.TALLA";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        Talla = leer["TALLA"].ToString()

                    };
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }             
            return listTallas;
        }

        public List<PackingM> ObtenerCajasPacking(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingM> listTallas = new List<PackingM>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;

                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT P.ID_PACKING, P.CANT_BOX, P.TOTAL_PIECES FROM PACKING P, CAT_ITEM_SIZE S " +
                    "WHERE P.ID_SUMMARY= '" + id + "' AND S.ID=P.ID_TALLA  ORDER BY S.TALLA ASC";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {

                        IdPacking = Convert.ToInt32(leer["ID_PACKING"]),
                        CantBox = Convert.ToInt32(leer["CANT_BOX"]),
                        TotalPiezas = Convert.ToInt32(leer["TOTAL_PIECES"])

                    };

                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();

            }           

            return listTallas;
        }


        //Muestra la lista de tallas TOTAL de Packing por estilo
        public List<PackingSize> ListaTallasCalidadPack(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingSize> listTallas = new List<PackingSize>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select S.TALLA " +
                    "from PACKING_SIZE PZ, CAT_ITEM_SIZE S " +
                    "WHERE PZ.ID_SUMMARY= '" + id + "' AND S.ID=PZ.ID_TALLA ORDER BY S.TALLA";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    PackingSize tallas = new PackingSize()
                    {
                        Talla = leer["TALLA"].ToString()
                    };
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }           

            return listTallas;
        }

        //Muestra la lista de tallas de Packing Size por estilo
        public List<PackingSize> ObtenerListaPackingSizePorEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingSize> listTallas = new List<PackingSize>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;             
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select PZ.ID_PACKING_SIZE, PZ.ID_TALLA, S.TALLA, PZ.QUALITY " +
                    "from PACKING_SIZE PZ, CAT_ITEM_SIZE S " +
                    "WHERE PZ.ID_SUMMARY= '" + id + "' AND S.ID=PZ.ID_TALLA ORDER BY S.TALLA";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {

                    PackingSize tallas = new PackingSize()
                    {
                        IdPackingSize = Convert.ToInt32(leer["ID_PACKING_SIZE"]),
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString(),
                        Calidad = Convert.ToInt32(leer["QUALITY"])
                    };
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
            }           

            return listTallas;
        }

        //Muestra la lista de tallas de Packing Type Size por estilo
        public List<PackingTypeSize> ObtenerListaPackingTypeSizePorEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingTypeSize> listTallas = new List<PackingTypeSize>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;              
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select PZ.ID_PACKING_TYPE_SIZE, PZ.ID_TALLA, S.TALLA, PZ.PIECES, PZ.RATIO, PZ.TYPE_PACKING " +
                    "from PACKING_TYPE_SIZE PZ, CAT_ITEM_SIZE S " +
                    "WHERE PZ.ID_SUMMARY= '" + id + "' AND S.ID=PZ.ID_TALLA ORDER BY S.TALLA";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    PackingTypeSize tallas = new PackingTypeSize()
                    {
                        IdPackingTypeSize = Convert.ToInt32(leer["ID_PACKING_TYPE_SIZE"]),
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString(),
                        Pieces = Convert.ToInt32(leer["PIECES"]),
                        Ratio = Convert.ToInt32(leer["RATIO"]),
                        IdTipoEmpaque = Convert.ToInt32(leer["TYPE_PACKING"])
                    };
                    ObtenerNombreTipoEmpaque(tallas);
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }     

            return listTallas;
        }

        //Muestra de Packing Type Size por estilo
        public List<PackingTypeSize> ObtenerCajasPackingPorEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingTypeSize> listTallas = new List<PackingTypeSize>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;              
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = " select P.ID_PACKING,P.CANT_BOX, P.TOTAL_PIECES, PZ.ID_PACKING_TYPE_SIZE, PZ.ID_TALLA, S.TALLA, PZ.PIECES, PZ.RATIO, PZ.TYPE_PACKING " +
                    "from PACKING_TYPE_SIZE PZ, CAT_ITEM_SIZE S, PACKING P " +
                    "WHERE PZ.ID_SUMMARY= '" + id + "' AND S.ID=PZ.ID_TALLA AND PZ.ID_PACKING_TYPE_SIZE=P.ID_PACKING_TYPE_SIZE ORDER BY S.TALLA";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {

                    PackingM pack = new PackingM();
                    PackingTypeSize tallas = new PackingTypeSize();
                    tallas.IdPackingTypeSize = Convert.ToInt32(leer["ID_PACKING_TYPE_SIZE"]);
                    tallas.IdTalla = Convert.ToInt32(leer["ID_TALLA"]);
                    tallas.Talla = leer["TALLA"].ToString();
                    tallas.Pieces = Convert.ToInt32(leer["PIECES"]);
                    tallas.Ratio = Convert.ToInt32(leer["RATIO"]);
                    tallas.IdTipoEmpaque = Convert.ToInt32(leer["TYPE_PACKING"]);
                    pack.IdPacking = Convert.ToInt32(leer["ID_PACKING"]);
                    pack.CantBox = Convert.ToInt32(leer["CANT_BOX"]);
                    pack.TotalPiezas = Convert.ToInt32(leer["TOTAL_PIECES"]);

                    tallas.PackingM = pack;
                    ObtenerNombreTipoEmpaque(tallas);
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {

                conn.CerrarConexion();
                conn.Dispose();
            }          

            return listTallas;
        }

        //Muestra de Packing Type Size por estilo
        public List<PackingTypeSize> ObtenerCajasPackingPPKPorEstilo(int? id, int idBatch)
        {
            Conexion conn = new Conexion();
            List<PackingTypeSize> listTallas = new List<PackingTypeSize>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = " select P.ID_PACKING,P.CANT_BOX, P.TOTAL_PIECES, PZ.ID_PACKING_TYPE_SIZE, PZ.ID_TALLA, S.TALLA, PZ.PIECES, PZ.RATIO, PZ.TYPE_PACKING " +
                    "from PACKING_TYPE_SIZE PZ, CAT_ITEM_SIZE S, PACKING P " +
                    "WHERE PZ.ID_SUMMARY= '" + id + "'  AND ID_BATCH='" + idBatch + " ' AND S.ID=PZ.ID_TALLA AND PZ.ID_PACKING_TYPE_SIZE=P.ID_PACKING_TYPE_SIZE ORDER BY S.TALLA";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {

                    PackingM pack = new PackingM();
                    PackingTypeSize tallas = new PackingTypeSize();
                    tallas.IdPackingTypeSize = Convert.ToInt32(leer["ID_PACKING_TYPE_SIZE"]);
                    tallas.IdTalla = Convert.ToInt32(leer["ID_TALLA"]);
                    tallas.Talla = leer["TALLA"].ToString();
                    tallas.Pieces = Convert.ToInt32(leer["PIECES"]);
                    tallas.Ratio = Convert.ToInt32(leer["RATIO"]);
                    tallas.IdTipoEmpaque = Convert.ToInt32(leer["TYPE_PACKING"]);
                    pack.IdPacking = Convert.ToInt32(leer["ID_PACKING"]);
                    pack.CantBox = Convert.ToInt32(leer["CANT_BOX"]);
                    pack.TotalPiezas = Convert.ToInt32(leer["TOTAL_PIECES"]);

                    tallas.PackingM = pack;
                    ObtenerNombreTipoEmpaque(tallas);
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {

                conn.CerrarConexion();
                conn.Dispose();
            }         

            return listTallas;
        }

        //Muestra la lista de tallas de Packing Type Size por estilo
        public List<PackingTypeSize> ObtenerListaPackingTypeSizePiezasyRatioPorEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingTypeSize> listTallas = new List<PackingTypeSize>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select PZ.ID_PACKING_TYPE_SIZE,PZ.PIECES, PZ.RATIO " +
                    "from PACKING_TYPE_SIZE PZ " +
                    "WHERE PZ.ID_SUMMARY= '" + id + "'";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {

                    PackingTypeSize tallas = new PackingTypeSize()
                    {
                        IdPackingTypeSize = Convert.ToInt32(leer["ID_PACKING_TYPE_SIZE"]),
                        Pieces = Convert.ToInt32(leer["PIECES"]),
                        Ratio = Convert.ToInt32(leer["RATIO"])
                    };
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {

                conn.CerrarConexion();
                conn.Dispose();
            }      

            return listTallas;
        }
        public void ObtenerNombreTipoEmpaque(PackingTypeSize packingTypeSize)
        {
            switch (packingTypeSize.IdTipoEmpaque)
            {
                case 1:
                    packingTypeSize.NombreTipoPak = "BULK";
                    break;
                case 2:
                    packingTypeSize.NombreTipoPak = "PPK";
                    break;
                case 3:
                    packingTypeSize.NombreTipoPak = "ASSORTMENT";
                    break;
                default:
                    packingTypeSize.NombreTipoPak = "-";
                    break;
            }
        }

        //Muestra la lista de tallas de Batch por id
        public IEnumerable<PackingM> ListaTallasBatchId(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingM> listTallas = new List<PackingM>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Lista_Batch_Packing";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();

                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        Talla = leer["TALLA"].ToString(),
                        IdBatch = Convert.ToInt32(leer["ID_BATCH"])


                    };

                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }          

            return listTallas;
        }

        //Muestra la lista de tallas TOTAL de Packing por estilo
        public IEnumerable<PackingM> ListaTallasTotalPacking(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingM> listTallas = new List<PackingM>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Lista_Total_Packing";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();

                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {

                        // Printed = Convert.ToInt32(leer["TOTAL"]),
                        
                    };

                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }     

            return listTallas;
        }

        //Muestra la lista de tallas TOTAL de Packing por estilo
        public IEnumerable<int> ListaTotalTallasPackingBatchEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT distinct (T.ID_TALLA), S.TALLA FROM PACKING T " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "'";
                leer = comando.ExecuteReader();
                int total = 0;
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString()

                    };
                    total = SumaTotalBacheTalla(id, tallas.IdTalla);
                    listTallas.Add(total);

                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
            }      

            return listTallas;
        }

        //Muestra la lista de tallas TOTAL Extra de PACKING por estilo
        public IEnumerable<int> ListaTotalETallasBatchEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT distinct (T.ID_TALLA), S.TALLA FROM PACKING T " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "'";
                leer = comando.ExecuteReader();
                int totalExtra = 0;
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString()

                    };
                    totalExtra = SumaTotalExtraBacheTalla(id, tallas.IdTalla);
                    listTallas.Add(totalExtra);
                }
                leer.Close();
            }
            finally
            {

                conn.CerrarConexion();
                conn.Dispose();
            } 
            return listTallas;
        }

        //Muestra la lista de tallas TOTAL Defect de PACKING por estilo
        public IEnumerable<int> ListaTotalDefTallasBatchEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT distinct (T.ID_TALLA), S.TALLA FROM PACKING T " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "'";
                leer = comando.ExecuteReader();
                int totalDefect = 0;
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString()

                    };
                    totalDefect = SumaTotalDefectBacheTalla(id, tallas.IdTalla);
                    listTallas.Add(totalDefect);
                }
                leer.Close();
            }
            finally
            {

                conn.CerrarConexion();
                conn.Dispose();
            }      
                return listTallas;
        }

        //Muestra la lista de tallas TOTAL Cajas de PACKING por estilo
        public IEnumerable<int> ListaTotalCajasTallasBatchEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT distinct (T.ID_TALLA), S.TALLA FROM PACKING T " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "'";
                leer = comando.ExecuteReader();
                int totalCajas = 0;
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString()

                    };
                    totalCajas = SumaTotalCajasdBacheTalla(id, tallas.IdTalla);
                    listTallas.Add(totalCajas);

                }
                leer.Close();
            }
            finally
            {

                conn.CerrarConexion();
                conn.Dispose();
            }    
            
            return listTallas;
        }

        //Muestra la lista de suma de  tallas por Batch
        public int SumaTotalBacheTalla(int? idEstilo, int idTalla)
        {
            Conexion conex = new Conexion();
            int suma = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;
                com.Connection = conex.AbrirConexion();
                com.CommandText = "SELECT TOTAL_PIECES  FROM PACKING WHERE ID_SUMMARY='" + idEstilo + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = com.ExecuteReader();                
                while (leerF.Read())
                {
                    suma += Convert.ToInt32(leerF["TOTAL_PIECES"]);

                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return suma;
        }

        //Muestra la lista de suma de Cajas tallas por Batch
        public int SumaTotalCajasdBacheTalla(int? idEstilo, int idTalla)
        {
            Conexion conex = new Conexion();
            int suma = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;
                com.Connection = conex.AbrirConexion();
                com.CommandText = "SELECT CANT_BOX  FROM PACKING WHERE ID_SUMMARY='" + idEstilo + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = com.ExecuteReader();
                while (leerF.Read())
                {
                    suma += Convert.ToInt32(leerF["CANT_BOX"]);

                }
                leerF.Close();
            }
            finally
            {

                conex.CerrarConexion();
                conex.Dispose();
            }   
            
            return suma;
        }

        //Muestra la lista de suma de Extra tallas por Batch
        public int SumaTotalExtraBacheTalla(int? idEstilo, int idTalla)
        {
            Conexion conex = new Conexion();
            int suma = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;
                com.Connection = conex.AbrirConexion();
                com.CommandText = "SELECT EXTRA  FROM PACKING WHERE ID_SUMMARY='" + idEstilo + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = com.ExecuteReader();
                while (leerF.Read())
                {
                    suma += Convert.ToInt32(leerF["EXTRA"]);

                }
                leerF.Close();
            }
           finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }         

            return suma;
        }

        //Muestra la lista de suma de DEFECT tallas por Batch
        public int SumaTotalDefectBacheTalla(int? idEstilo, int idTalla)
        {
            Conexion conex = new Conexion();
            int suma = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;
                com.Connection = conex.AbrirConexion();
                com.CommandText = "SELECT DEFECT FROM PACKING WHERE ID_SUMMARY='" + idEstilo + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = com.ExecuteReader();
                while (leerF.Read())
                {
                    suma += Convert.ToInt32(leerF["DEFECT"]);

                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
            }         

            return suma;
        }

        //Muestra la lista de tallas de Batch por estilo
        public IEnumerable<PackingM> ListaBatch(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingM> listTallas = new List<PackingM>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT distinct ID_BATCH FROM PACKING WHERE ID_SUMMARY='" + id + "'";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {

                        IdBatch = Convert.ToInt32(leer["ID_BATCH"])
                    };

                    tallas.Batch = ListaTallasBatch(tallas.IdBatch, id);
                    foreach (var item in tallas.Batch)
                    {
                        tallas.TipoTurno = item.TipoTurno;
                        tallas.NombreUsr = item.NombreUsr;
                        tallas.IdPacking = item.IdPacking;
                        tallas.NombreUsrModif = item.NombreUsrModif;
                        tallas.TipoEmpaque = item.PackingTypeSize.IdTipoEmpaque;

                    }
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
            }        

            return listTallas;
        }

        //Muestra la lista de tallas de UN Batch por estilo y id Batch seleccionado
        public IEnumerable<PackingM> ListaCantidadesTallaPorIdBatchEstilo(int? idEstilo, int idBatch)
        {
            Conexion conn = new Conexion();
            List<PackingM> listTallas = new List<PackingM>();
            List<PackingTypeSize> listaEmpaque = new List<PackingTypeSize>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT ID_PACKING, ID_TALLA, S.TALLA, CANT_BOX, TOTAL_PIECES FROM PACKING " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=PACKING.ID_TALLA " +
                    "WHERE ID_SUMMARY='" + idEstilo + "' AND ID_BATCH='" + idBatch + " 'ORDER BY ID_TALLA asc ";
                leer = comando.ExecuteReader();

                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        Talla = leer["TALLA"].ToString(),
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        CantBox = Convert.ToInt32(leer["CANT_BOX"]),
                        TotalPiezas = Convert.ToInt32(leer["TOTAL_PIECES"])
                    };

                    listaEmpaque = ObtenerListaPackingTypeSizePiezasyRatioPorEstilo(idEstilo);
                    tallas.ListEmpaque = listaEmpaque;
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
           finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }   
         
            return listTallas;
        }

        //Obtener el tipo de empaque de una talla
        public int ObtenerTipoEmpaque(int? idEstilo)
        {
            Conexion conn = new Conexion();
            int tipoEmpaque = 0;
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT distinct pt.type_packing FROM packing_type_size pt WHERE pt.ID_SUMMARY='" + idEstilo + "'";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {


                    if (!Convert.IsDBNull(leer["TYPE_PACKING"]))
                    {
                        tipoEmpaque = Convert.ToInt32(leer["TYPE_PACKING"]);
                    }

                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }           
            return tipoEmpaque;
        }

        //Muestra la lista de tallas por Batch
        public List<PackingM> ListaTallasBatch(int? batch, int? id)
        {
            Conexion conex = new Conexion();
            List<PackingM> listTallas = new List<PackingM>();
            try
            {
                SqlCommand c = new SqlCommand();
                SqlDataReader leerF = null;
                c.Connection = conex.AbrirConexion();
                c.CommandText = "SELECT p.ID_PACKING, P.ID_SUMMARY, P.ID_BATCH, CONCAT(U.Nombres,' ',U.Apellidos)AS NOMBRE,P.ID_USUARIO_MODIF, P.TURNO,  " +
                    " P.ID_TALLA, S.TALLA, P.CANT_BOX, P.TOTAL_PIECES FROM PACKING P, CAT_ITEM_SIZE S, USUARIOS U " +
                    "WHERE P.ID_BATCH='" + batch + "' AND P.ID_SUMMARY='" + id + "'AND S.ID=P.ID_TALLA AND U.Id=P.ID_USUARIO " +
                    "ORDER BY S.TALLA ASC ";
                leerF = c.ExecuteReader();

                while (leerF.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        Talla = leerF["TALLA"].ToString(),
                        IdBatch = Convert.ToInt32(leerF["ID_BATCH"]),
                        IdPacking = Convert.ToInt32(leerF["ID_PACKING"]),
                        IdSummary = Convert.ToInt32(leerF["ID_SUMMARY"]),
                        TipoTurno = Convert.ToInt32(leerF["TURNO"]),
                        NombreUsr = leerF["NOMBRE"].ToString(),
                        CantBox = Convert.ToInt32(leerF["CANT_BOX"]),
                        TotalPiezas = Convert.ToInt32(leerF["TOTAL_PIECES"])


                    };

                    if (!Convert.IsDBNull(leerF["ID_USUARIO_MODIF"]))
                    {
                        tallas.UsuarioModif = Convert.ToInt32(leerF["ID_USUARIO_MODIF"]);
                    }

                    if (tallas.UsuarioModif != 0)
                    {
                        tallas.NombreUsrModif = objCatUser.Obtener_Nombre_Usuario_PorID(tallas.UsuarioModif);
                    }
                    else
                    {
                        tallas.NombreUsrModif = "-";
                    }

                    PackingTypeSize tipoEmp = new PackingTypeSize();
                    tipoEmp.IdTipoEmpaque = ObtenerTipoEmpaque(id);
                    tallas.PackingTypeSize = tipoEmp;
                    listTallas.Add(tallas);
                }
                leerF.Close();
            }
            catch (Exception)
            {
                conex.CerrarConexion();
                conex.Dispose();
            }        

            return listTallas;
        }

        //Agregar las tallas de un batch
        public void AgregarTallasPacking(PackingM packing)
        {
            Conexion conex = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = conex.AbrirConexion();
                com.CommandText = "AgregarPacking";
                com.CommandType = CommandType.StoredProcedure;

                com.Parameters.AddWithValue("@idSummary", packing.IdSummary);
                com.Parameters.AddWithValue("@idBatch", packing.IdBatch);
                com.Parameters.AddWithValue("@idTalla", packing.IdTalla);
                com.Parameters.AddWithValue("@cantB", packing.CantBox);
                com.Parameters.AddWithValue("@total", packing.TotalPiezas);
                com.Parameters.AddWithValue("@turno", packing.IdTurno);
                com.Parameters.AddWithValue("@idUsr", packing.Usuario);
                com.Parameters.AddWithValue("@idPack", packing.IdPackingTypeSize);

                com.ExecuteNonQuery();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }        

        }

        //Agregar las tallas de Packing
        public void AgregarTallasP(PackingM packing)
        {
            Conexion conex = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = conex.AbrirConexion();
                com.CommandText = "AgregarPackingSize";
                com.CommandType = CommandType.StoredProcedure;


                com.Parameters.AddWithValue("@idTalla", packing.PackingSize.IdTalla);
                com.Parameters.AddWithValue("@calidad", packing.PackingSize.Calidad);
                com.Parameters.AddWithValue("@idSummary", packing.PackingSize.IdSummary);
                com.ExecuteNonQuery();
            }
            finally
            {

                conex.CerrarConexion();
                conex.Dispose();
            }

            

        }

        //Agregar las tallas de Packing Type Size
        public void AgregarTallasTypePack(PackingM packing)
        {
            Conexion conex = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = conex.AbrirConexion();
                com.CommandText = "AgregarTypePackingSize";
                com.CommandType = CommandType.StoredProcedure;

                com.Parameters.AddWithValue("@idTalla", packing.PackingTypeSize.IdTalla);
                com.Parameters.AddWithValue("@piece", packing.PackingTypeSize.Pieces);
                com.Parameters.AddWithValue("@ratio", packing.PackingTypeSize.Ratio);
                com.Parameters.AddWithValue("@typeP", packing.PackingTypeSize.IdTipoEmpaque);
                com.Parameters.AddWithValue("@numPO", packing.PackingTypeSize.NumberPO);
                com.Parameters.AddWithValue("@packF", packing.PackingTypeSize.IdFormaEmpaque);
                com.Parameters.AddWithValue("@qty", packing.PackingTypeSize.Cantidad);
                com.Parameters.AddWithValue("@idSummary", packing.PackingTypeSize.IdSummary);
                com.ExecuteNonQuery();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }        
            

        }

        //Permite actualizar la información de un batch
        public void ActualizarTallasPacking(PackingM packing)
        {
            Conexion conex = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = conex.AbrirConexion();
                com.CommandText = "ActualizarBatchPacking";
                com.CommandType = CommandType.StoredProcedure;

                com.Parameters.AddWithValue("@id", packing.IdPacking);
                com.Parameters.AddWithValue("@idSummary", packing.IdSummary);
                com.Parameters.AddWithValue("@idBatch", packing.IdBatch);
                com.Parameters.AddWithValue("@idTalla", packing.IdTalla);
                com.Parameters.AddWithValue("@cantB", packing.CantBox);
                com.Parameters.AddWithValue("@total", packing.TotalPiezas);
                com.Parameters.AddWithValue("@turno", packing.IdTurno);
                com.Parameters.AddWithValue("@idUsr", packing.Usuario);
                com.Parameters.AddWithValue("@idPack", packing.IdPackingTypeSize);
                com.Parameters.AddWithValue("@idUsrMod", packing.UsuarioModif);

                com.ExecuteNonQuery();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

            

        }

        //Permite obtener el id del batch de los registro 
        public int ObtenerIdBatch(int id)
        {
            int idBatch = 0;
            int idTotal = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT distinct ID_BATCH FROM PACKING WHERE ID_SUMMARY='" + id + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    idBatch += Convert.ToInt32(leerF["ID_BATCH"]);
                    idTotal++;
                }
                leerF.Close();
            }
            finally 
            {
                conex.CerrarConexion();
                conex.Dispose();
            }           
           
            return idTotal;
        }

        //Permite obtener el id del packing size de los registro 
        public int ObtenerIdPackingSize(int id)
        {
            int idPack = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "select id_packing_type_size from packing where id_Packing='" + id + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    idPack += Convert.ToInt32(leerF["id_packing_type_size"]);

                }
                leerF.Close();
            }
            catch (Exception)
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
                      
            return idPack;
        }

        //Permite obtener el idPacking del batch de los registro por idestilo
        public int ObtenerIdPackingPorBatchEstilo(int idBatch, int idSummary, int idTalla)
        {

            int idPacking = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT ID_PACKING FROM PACKING WHERE ID_BATCH='" + idBatch + "' AND ID_SUMMARY='" + idSummary + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    idPacking += Convert.ToInt32(leerF["ID_PACKING"]);

                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }  
           
            return idPacking;
        }

        public int Obtener_Utlimo_Packing_Type()
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            Conexion conex = new Conexion();
            try
            {
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "SELECT ID_PACKING_TYPE_SIZE FROM PACKING_TYPE_SIZE WHERE ID_PACKING_TYPE_SIZE = (SELECT MAX(ID_PACKING_TYPE_SIZE) FROM PACKING_TYPE_SIZE)";
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    return Convert.ToInt32(reader["ID_PEDIDO"]);
                }
                reader.Close();
            }
            finally {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return 0;
        }

        //Permite obtener el idUsuario del batch registrado
        public int ObtenerIdUsuarioPorBatchEstilo(int idBatch, int idSummary, int idTalla)
        {

            int idUsuario = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT ID_USUARIO FROM PACKING WHERE ID_BATCH='" + idBatch + "' AND ID_SUMMARY='" + idSummary + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    idUsuario += Convert.ToInt32(leerF["ID_USUARIO"]);


                }
                leerF.Close();
            }
            catch (Exception)
            {

                conex.CerrarConexion();
                conex.Dispose();
            }

            return idUsuario;
        }

    }
}

