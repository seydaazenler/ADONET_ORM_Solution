using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ADONET_ORM_Common
{
    public static class Tools
    {
        private static SqlConnection _mySqlDBConnection;
        public static SqlConnection MySqlDBConnection
        {
            get
            {
                if (_mySqlDBConnection == null)
                {
                    _mySqlDBConnection = new SqlConnection("Server=SUNUM2\\MSSQLSERVER01;Database=NORTHWND; Trusted_Connection=True;");
                }
                return _mySqlDBConnection;
            }
            set
            {
                _mySqlDBConnection = value;
            }
        }

        
    }
}
