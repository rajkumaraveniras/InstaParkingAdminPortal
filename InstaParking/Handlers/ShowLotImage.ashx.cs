using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using InstaParking.DAL;
using InstaParking.Models;
using System;

namespace InstaParking.Handlers
{
    public class ShowLotImage : IHttpHandler
    {
        Lots_DAL lotDAL_obj = new Lots_DAL();   
        public void ProcessRequest(HttpContext context)
        {
            Int32 lotID;
            string name = "";

            if (context.Request.QueryString["LotID"] != null)
            {
                lotID = Convert.ToInt32(context.Request.QueryString["LotID"]);
            }
            else
                throw new ArgumentException("No parameter specified");
            if (context.Request.QueryString["Image"] != null)
            {
                name = Convert.ToString(context.Request.QueryString["Image"]);
            }
            else if (context.Request.QueryString["Image2"] != null)
            {
                name = Convert.ToString(context.Request.QueryString["Image2"]);
            }
            else if (context.Request.QueryString["Image3"] != null)
            {
                name = Convert.ToString(context.Request.QueryString["Image3"]);
            }

            context.Response.ContentType = "image/jpeg";
            Stream strm = ShowLotImages(lotID, name);
            byte[] buffer = new byte[4096];
            int byteSeq = strm.Read(buffer, 0, 4096);

            while (byteSeq > 0)
            {
                context.Response.OutputStream.Write(buffer, 0, byteSeq);
                byteSeq = strm.Read(buffer, 0, 4096);
            }
        }
        public Stream ShowLotImages(int lotID, string name)
        {
            try
            {
                return lotDAL_obj.ShowLotImg(lotID, name);
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