using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using InstaParking.DAL;
using InstaParking.Models;
using System;
namespace InstaParking.Handlers
{    
    public class ShowServiceTypeIcons : IHttpHandler
    {
        ServiceType_DAL serviceTypeDAL = new ServiceType_DAL();
        public void ProcessRequest(HttpContext context)
        {
            Int32 serviceTypeID;
            string name = "";

            if (context.Request.QueryString["ServiceTypeID"] != null)
            {
                serviceTypeID = Convert.ToInt32(context.Request.QueryString["ServiceTypeID"]);
            }
            else
                throw new ArgumentException("No parameter specified");
            if (context.Request.QueryString["Icon"] != null)
            {
                name = Convert.ToString(context.Request.QueryString["Icon"]);
            }

            context.Response.ContentType = "image/jpeg";
            Stream strm = ShowServiceTypeIcon(serviceTypeID, name);
            byte[] buffer = new byte[4096];
            int byteSeq = strm.Read(buffer, 0, 4096);

            while (byteSeq > 0)
            {
                context.Response.OutputStream.Write(buffer, 0, byteSeq);
                byteSeq = strm.Read(buffer, 0, 4096);
            }
        }
        public Stream ShowServiceTypeIcon(int serviceTypeID, string name)
        {
            try
            {
                return serviceTypeDAL.ShowServiceTypeIcons(serviceTypeID, name);
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