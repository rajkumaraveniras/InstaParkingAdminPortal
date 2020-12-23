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
    public class ServiceTypeIconUploadHandler : IHttpHandler, IRequiresSessionState
    {
        string localpath = HttpContext.Current.Server.MapPath("~/ServiceTypeImages/");
        ServiceType_DAL serviceTypeDAL_obj = new ServiceType_DAL();

        string folderpath = System.Configuration.ConfigurationManager.AppSettings["servicetypeiconsFolderforConsumerAPI"];
        string operatorPath = System.Configuration.ConfigurationManager.AppSettings["servicetypeiconsFolderforOperatorAPI"];
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string employee_logo = "";

                byte[] IconData = null;
                string IconName = "";
                string IconType = "";

                string serviceTypeImageName = "";

                if (context.Request.Files.Count > 0)
                {
                    HttpFileCollection files = context.Request.Files;
                    ServiceType serviceTypeObj = new ServiceType();
                    serviceTypeObj.ServiceTypeID = Convert.ToInt32(context.Session["ServiceTypeID"].ToString());


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

                        employee_logo = s.ToString().TrimEnd('.') + " " + DateTime.Now.ToString("dd MMM yyyy HH'-'mm'-'ss", System.Globalization.CultureInfo.InvariantCulture) + "." + file_split[file_split.Length - 1];

                        imgByte = new Byte[file.ContentLength];
                        
                        if (keyvalue == "ServiceTypeLogo")
                        {
                            using (var binaryReader = new BinaryReader(file.InputStream))
                            {
                                //IconData = binaryReader.ReadBytes(file.ContentLength);
                                IconName = file_split[0];
                                IconType = file_split[file_split.Length - 1];
                                serviceTypeImageName = IconName.ToString().TrimEnd('.') + " " + DateTime.Now.ToString("dd MMM yyyy HH'-'mm'-'ss", System.Globalization.CultureInfo.InvariantCulture) + "." + IconType;
                                file.SaveAs(localpath + serviceTypeImageName);//local folder 
                                file.SaveAs(folderpath + serviceTypeImageName);
                                file.SaveAs(operatorPath + serviceTypeImageName);
                            }
                        }                       
                    }
                    InsertServiceTypeIconData(serviceTypeImageName, serviceTypeObj);
                }
            }
            catch (Exception ex)
            { }


            context.Response.ContentType = "text/plain";
        }

        public string InsertServiceTypeIconData(string IconName, ServiceType servicetypobj)
        {
            try
            {
                string message = serviceTypeDAL_obj.InsertServiceTypeIconData(IconName, servicetypobj);
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