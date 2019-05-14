using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models
{
    public class Conexion
    {
        private static readonly object objectLock = new object();
        private bool _disposed;
        //private SqlConnection conn = new SqlConnection("Server=tcp:189.213.233.1,1433;Database=FFB;User Id=administrator; Password=A!3$$traFoR7un3; Connection Timeout=10000;");
       private SqlConnection conn = new SqlConnection("Server=tcp:fortunesp.database.windows.net,1433;Initial Catalog=FortuneTest;Persist Security Info=False;User ID=AdminFB;Password=Admin@2019;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;");
      // private SqlConnection conn = new SqlConnection("Server=W_KAREN;Database=FFB;Integrated Security =true"); //FortuneTest
     // private SqlConnection conn = new SqlConnection("Server=tcp:10.8.1.128,1433;Database=FFB;Integrated Security =true; Connection Timeout=10000;");

        public SqlConnection AbrirConexion()
        {
            if (conn.State == ConnectionState.Closed)
            {               
                    conn.Open();             
            }
            return conn;
        }
        public SqlConnection CerrarConexion()
        {
            if(conn.State == ConnectionState.Closed)
            {
                conn.Close();
                SqlConnection.ClearPool(conn);
            }
            return conn;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (objectLock)
            {
                if (_disposed == false)
                {
                    if (disposing == true)
                    {
                        if (conn != null)
                        {
                            conn.Dispose();
                            conn = null;
                        }              
                        _disposed = true;
                    }
                }
            }
        }


    }
}