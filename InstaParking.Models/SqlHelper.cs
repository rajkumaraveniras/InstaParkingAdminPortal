using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace InstaParking.Models
{
    public class SqlHelper
    {
        public string SQLDBCon { get; set; }

        public SqlHelper()
        {
            SQLDBCon = ConfigurationManager.ConnectionStrings["Parking_connstring"].ConnectionString;
        }

        public string GetConnectionSrting() {
            return ConfigurationManager.ConnectionStrings["Parking_connstring"].ConnectionString;
        }
    }
}