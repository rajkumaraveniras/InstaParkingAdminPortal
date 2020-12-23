using System;
using System.Linq;
using System.Web;
using InstaParking.Models;
using InstaParking.DAL;
using Newtonsoft.Json;
using System.Web.SessionState;
using System.Text;
using System.IO;
namespace InstaParking.Handlers
{
    /// <summary>
    /// Summary description for VehicleTypeIconUploadHandler
    /// </summary>
    public class VehicleTypeIconUploadHandler : IHttpHandler, IRequiresSessionState
    {
        string folderpath = System.Configuration.ConfigurationManager.AppSettings["VehicletypeiconsFolderforConsumerAPI"];
        string localpath = HttpContext.Current.Server.MapPath("~/VehicleTypeIcons/");
        VehicleType_DAL vehicleTypeDAL_obj = new VehicleType_DAL();
        string operatorPath = System.Configuration.ConfigurationManager.AppSettings["VehicletypeiconsFolderforOperatorAPI"];
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                byte[] IconData = null;
                string IconName = "";
                string IconType = "";

                string vehicleTypeImageName = "";

                if (context.Request.Files.Count > 0)
                {
                    HttpFileCollection files = context.Request.Files;
                    VehicleType vehicleTypeObj = new VehicleType();
                    vehicleTypeObj.VehicleTypeID = Convert.ToInt32(context.Session["VehicleTypeID"].ToString());


                    for (int i = 0; i < files.Count; i++)
                    {
                        string keyvalue = files.AllKeys[i].ToString();

                        Byte[] imgByte = null;
                        HttpPostedFile file = files[i];

                        string[] file_split = file.FileName.Split('\\').Last().Split('.');

                        StringBuilder s = new StringBuilder();
                        for (int j = 0; j < file_split.Length - 1; j++)
                        {
                            s.Append(file_split[j] + ".");
                        }

                        imgByte = new Byte[file.ContentLength];

                        if (keyvalue == "VehicleTypeLogo")
                        {
                            using (var binaryReader = new BinaryReader(file.InputStream))
                            {
                                IconName = file_split[0];
                                IconType = file_split[file_split.Length - 1];
                                vehicleTypeImageName = IconName.ToString().TrimEnd('.') + " " + DateTime.Now.ToString("dd MMM yyyy HH'-'mm'-'ss", System.Globalization.CultureInfo.InvariantCulture) + "." + IconType;
                                file.SaveAs(localpath + vehicleTypeImageName);//local folder 
                                file.SaveAs(folderpath + vehicleTypeImageName);
                                file.SaveAs(operatorPath + vehicleTypeImageName);
                            }
                        }
                    }
                    InsertVehicleTypeIconData(vehicleTypeImageName, vehicleTypeObj);
                }
            }
            catch (Exception ex)
            { }
            context.Response.ContentType = "text/plain";
        }
        public string InsertVehicleTypeIconData(string IconName, VehicleType vehicletypobj)
        {
            try
            {
                string message = vehicleTypeDAL_obj.InsertVehicleTypeIconData(IconName, vehicletypobj);
                return message;
            }
            catch (Exception ex)
            {
                return ex.Message;
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