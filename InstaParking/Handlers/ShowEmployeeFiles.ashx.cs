using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using InstaParking.DAL;
using InstaParking.Models;
using System;

namespace InstaParking.Handlers
{   
    public class ShowEmployeeFiles : IHttpHandler
    {
        Users_DAL usersDAL_obj = new Users_DAL();

        public void ProcessRequest(HttpContext context)
        {
            Int32 UserID;
            string name = "";

            if (context.Request.QueryString["UserID"] != null)
            {
                UserID = Convert.ToInt32(context.Request.QueryString["UserID"]);
            }
            else
                throw new ArgumentException("No parameter specified");
            if (context.Request.QueryString["Photo"] != null)
            {
                name = Convert.ToString(context.Request.QueryString["Photo"]);
            }
            else if (context.Request.QueryString["Aadhar"] != null)
            {
                name = Convert.ToString(context.Request.QueryString["Aadhar"]);
            }
            else if (context.Request.QueryString["PAN"] != null)
            {
                name = Convert.ToString(context.Request.QueryString["PAN"]);
            }

            context.Response.ContentType = "image/jpeg";
            Stream strm = ShowCustomrLogo(UserID, name);
            byte[] buffer = new byte[4096];
            int byteSeq = strm.Read(buffer, 0, 4096);

            while (byteSeq > 0)
            {
                context.Response.OutputStream.Write(buffer, 0, byteSeq);
                byteSeq = strm.Read(buffer, 0, 4096);
            }
        }
        public Stream ShowCustomrLogo(int UserID, string name)
        {
            try
            {
                return usersDAL_obj.ShowImage(UserID, name);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}