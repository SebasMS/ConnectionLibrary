using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml; //Importamos para manipular los elementos XML

namespace ConnectionLibrary
{
    public class Parameters
    {
        #region Attributes
        private string server;
        private string database;
        private string user;
        private string password;
        private bool integrateSecurity;
        private string fileParameters;
        private string connectionString;
        private string error;        
        private XmlDocument xml = new XmlDocument();
        private XmlNode node; 
        #endregion

        #region Properties
        public string ConnectionString
        {
            get { return connectionString; }
        }

        public string Error
        {
            get { return error; }
        }
        #endregion

        #region Constructor
        public Parameters()
        {
            server = "";
            database = "";
            user = "";
            password = "";
            integrateSecurity = true;
            fileParameters = "";
            connectionString = "";
            error = "";
        }
        #endregion

        #region Public Methods
        public bool GenerateConnectionString(string fileNameParameters)
        {
            fileParameters = Application.StartupPath + "\\" + fileNameParameters;
            try
            {
                // Leer archivo XML
                xml.Load(fileParameters);

                // Leer el primer atributo de la cadena de conexión
                node = xml.SelectSingleNode("//Server");

                // Devolver el valor del nodo
                server = node.InnerText;

                node = xml.SelectSingleNode("//Database");
                database = node.InnerText;

                node = xml.SelectSingleNode("//User");
                user = node.InnerText;

                node = xml.SelectSingleNode("//Password");
                password = node.InnerText;

                node = xml.SelectSingleNode("//IntegrateSecurity");
                integrateSecurity = Convert.ToBoolean(node.InnerText);

                if (integrateSecurity) // Autenticación de Windows
                {
                    connectionString = "Data Source=" + server + 
                                       ";Initial Catalog=" + database + 
                                       ";Integrate Security=true";
                }
                else // Autenticación de SQL Server
                {
                    connectionString = "Data Source=" + server + 
                                       ";Initial Catalog=" + database + 
                                       ";User Id=" + user + 
                                       ";Password=" + password;                      
                }
                xml = null;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
        #endregion
    }
}
