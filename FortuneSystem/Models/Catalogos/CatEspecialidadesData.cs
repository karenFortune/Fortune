﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatEspecialidadesData
    {
        public IEnumerable<CatEspecialidades> ListaEspecialidades()
        {
            List<CatEspecialidades> listEspecialidad = new List<CatEspecialidades>();
            Conexion conn = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT * FROM CAT_SPECIALTIES ";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    CatEspecialidades especialidad = new CatEspecialidades()
                    {
                        IdEspecialidad = Convert.ToInt32(leer["ID_SPECIALTIES"]),
                        Especialidad = leer["SPECIALTIES"].ToString()

                    };

                    listEspecialidad.Add(especialidad);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listEspecialidad;
        }
    }
}