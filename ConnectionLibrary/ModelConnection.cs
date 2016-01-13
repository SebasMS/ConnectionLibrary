using System;
using System.Collections.Generic;
using System.Data; // Importamos para usar el DataTable
using System.Data.SqlClient; //Importamos para manipular base de datos
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionLibrary
{
    public class ModelConnection
    {
        #region Attributes
        private string fileParameters;
        private string connectionString;
        private string error;
        private bool existsConnection;
        private SqlConnection cnn;
        private SqlCommand cmd;
        private SqlDataReader dr;   
        #endregion

        #region Properties
        public string Error
        {
            get { return error; }
        }

        public SqlConnection Cnn
        {
            get { return cnn; }
        }

        public SqlDataReader Dr
        {
            get { return dr; }
        }
        #endregion

        #region Constructor
        public ModelConnection(string fileParameters){
            this.fileParameters = fileParameters;
            existsConnection = false;
            cnn = new SqlConnection(); // Instanciar los objetos de SqlConnection y SqlCommand
            cmd = new SqlCommand();
        }
        #endregion

        #region Private Methods
        private bool GenerateConnectionString()
        {
            try
            {
                Parameters parameter = new Parameters();
                if (!parameter.GenerateConnectionString(fileParameters))
                {
                    error = parameter.Error;
                    parameter = null;
                    return false;
                }
                else
                {
                    connectionString = parameter.ConnectionString;
                    parameter = null;
                    return true;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
        #endregion

        #region Public Methods
        public bool OpenConnection()
        {
            if (!GenerateConnectionString()) return false;
            cnn.ConnectionString = connectionString;
            try
            {
                cnn.Open();
                existsConnection = true;
                return true;
            }
            catch (Exception ex)
            {
                error = "No abrió la conexión, " + ex.Message;
                existsConnection = false;
                return false;
            }
        }

        public void CloseConnection()
        {
            try
            {
                cnn.Close();
                existsConnection = false;
            }
            catch (Exception ex)
            {
                error = "No cerró la conexión, " + ex.Message;
            }
        }

        public DataTable GetData(string procedureName, string[] parameterName, params Object[] parameterValues)
        {
            DataTable dt = new DataTable(); // Crear un DataTable
            cmd.Connection = cnn;
            cmd.CommandText = procedureName; // Especificar nombre del procedimiento a ejecutar
            cmd.CommandType = CommandType.StoredProcedure; // Determinar que es un procedimiento almacenado
            if (procedureName.Length != 0 && parameterName.Length == parameterValues.Length)
            {
                int i = 0;
                foreach (String parameter in parameterName)
                    cmd.Parameters.AddWithValue(parameter, parameterValues[i++]); // Ejecutar valores ingresados
                try
                {
                    dr = cmd.ExecuteReader(); // Ejecutar la instrucción con el SQLDataReader
                    dt.Load(dr); // Registrar el DataTable
                    CloseConnection();
                    return dt;
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }
            return dt;
        }

        public int ExecuteProcedures(string procedureName, string[] parameterName, params Object[] parameterValues)
        {
            cmd.Connection = cnn;
            cmd.CommandText = procedureName;
            cmd.CommandType = CommandType.StoredProcedure;
            if (procedureName.Length != 0 && parameterName.Length == parameterValues.Length)
            {
                int i = 0;
                foreach (String parameter in parameterName)
                    cmd.Parameters.AddWithValue(parameter, parameterValues[i++]);
                try
                {
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }
            return 0;
        }
        #endregion
    }
}
