using System;
using System.Linq;
using System.Web;
using InstaParking.Models;
using InstaParking.DAL;
using Newtonsoft.Json;
using System.Web.SessionState;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace InstaParking.Handlers
{
    public class CompanyLogoHandler : IHttpHandler, IRequiresSessionState
    {
        string localpath = HttpContext.Current.Server.MapPath("~/Images/");
        CompanyInfo_DAL companyInfoDAL_obj = new CompanyInfo_DAL();
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string company_logo = "";
                string IconName = "";
                string IconType = "";
                string companyImageName = "";

                if (context.Request.Files.Count > 0)
                {
                    HttpFileCollection files = context.Request.Files;
                    Account accountObj = new Account();
                    accountObj.AccountID = Convert.ToInt32(context.Session["AccountID"].ToString());
                    accountObj.CreatedBy = Convert.ToInt32(context.Session["UserID"].ToString());

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

                        company_logo = s.ToString().TrimEnd('.') + " " + DateTime.Now.ToString("dd MMM yyyy HH'-'mm'-'ss", System.Globalization.CultureInfo.InvariantCulture) + "." + file_split[file_split.Length - 1];

                        imgByte = new Byte[file.ContentLength];

                        if (keyvalue == "CompanyImg")
                        {
                            using (var binaryReader = new BinaryReader(file.InputStream))
                            {
                                IconName = file_split[0];
                                IconType = file_split[file_split.Length - 1];
                                companyImageName = IconName.ToString().TrimEnd('.') + " " + DateTime.Now.ToString("dd MMM yyyy HH'-'mm'-'ss", System.Globalization.CultureInfo.InvariantCulture) + "." + IconType;
                                file.SaveAs(localpath + companyImageName);//local folder 
                            }
                        }
                    }
                    InsertCompanyLogo(companyImageName, accountObj);
                }
            }
            catch (Exception ex)
            { }
            context.Response.ContentType = "text/plain";
        }

        public string InsertCompanyLogo(string companyImageName, Account accountObj)
        {
            try
            {
                string message = companyInfoDAL_obj.InsertCompanyLogo(companyImageName, accountObj);
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