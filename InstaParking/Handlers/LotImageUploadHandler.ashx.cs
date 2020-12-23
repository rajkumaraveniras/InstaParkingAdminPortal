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
    public class LotImageUploadHandler : IHttpHandler, IRequiresSessionState
    {
        Lots_DAL lotsDAL_obj = new Lots_DAL();
        string folderpath = System.Configuration.ConfigurationManager.AppSettings["lotimagesFolderforAPI"];
        string localpath = HttpContext.Current.Server.MapPath("~/LotImages/");
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string lot_logo = "";

                byte[] lotData = null;
                string lotName = "";
                string lotType = "";

                byte[] lotData2 = null;
                string lotName2 = "";
                string lotType2 = "";

                byte[] lotData3 = null;
                string lotName3 = "";
                string lotType3 = "";

                string imagelotImageName = "";
                string imagelotImageName2 = "";
                string imagelotImageName3 = "";

                if (context.Request.Files.Count > 0)
                {
                    HttpFileCollection files = context.Request.Files;
                    Lots lotObj = new Lots();
                    lotObj.LocationParkingLotID = Convert.ToInt32(context.Session["LotID"].ToString());


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

                        lot_logo = s.ToString().TrimEnd('.') + " " + DateTime.Now.ToString("dd MMM yyyy HH'-'mm'-'ss", System.Globalization.CultureInfo.InvariantCulture) + "." + file_split[file_split.Length - 1];

                        imgByte = new Byte[file.ContentLength];

                        if (keyvalue == "LotLogo")
                        {
                            using (var binaryReader = new BinaryReader(file.InputStream))
                            {
                                //lotData = binaryReader.ReadBytes(file.ContentLength);
                               // lotData = compressImageBytes(lotData);                                
                                lotName = file_split[0];
                                lotType = file_split[file_split.Length - 1];                                
                                imagelotImageName = lotName.ToString().TrimEnd('.') + " " + DateTime.Now.ToString("dd MMM yyyy HH'-'mm'-'ss", System.Globalization.CultureInfo.InvariantCulture) + "." + lotType;
                                //file.SaveAs(HttpContext.Current.Server.MapPath("~/LotImages/") + imagelotImageName);
                                //file.SaveAs("C:/Users/SPRV/Desktop/my images/" + imagelotImageName);                                

                                file.SaveAs(localpath + imagelotImageName);//local folder                               
                                file.SaveAs(folderpath + imagelotImageName);//consumer Api folder

                                //string imgPath = Path.Combine(folderpath, imagelotImageName);//consumer Api folder
                                //File.WriteAllBytes(imgPath, lotData);//consumer Api folder
                            }
                        }
                        if (keyvalue == "LotLogo2")
                        {
                            using (var binaryReader = new BinaryReader(file.InputStream))
                            {
                               // lotData2 = binaryReader.ReadBytes(file.ContentLength);
                               // lotData2 = compressImageBytes(lotData2);
                                lotName2 = file_split[0];
                                lotType2 = file_split[file_split.Length - 1];
                                imagelotImageName2 = lotName2.ToString().TrimEnd('.') + " " + DateTime.Now.ToString("dd MMM yyyy HH'-'mm'-'ss", System.Globalization.CultureInfo.InvariantCulture) + "." + lotType2;
                                // file.SaveAs(HttpContext.Current.Server.MapPath("~/LotImages/") + imagelotImageName2);
                                file.SaveAs(localpath + imagelotImageName2);//local folder
                                file.SaveAs(folderpath + imagelotImageName2);//consumer Api folder

                                //string imgPath2 = Path.Combine(folderpath, imagelotImageName2);//consumer Api folder
                                //File.WriteAllBytes(imgPath2, lotData);//consumer Api folder
                            }
                        }
                        if (keyvalue == "LotLogo3")
                        {
                            using (var binaryReader = new BinaryReader(file.InputStream))
                            {
                              //  lotData3 = binaryReader.ReadBytes(file.ContentLength);
                               // lotData3 = compressImageBytes(lotData3);
                                lotName3 = file_split[0];
                                lotType3 = file_split[file_split.Length - 1];
                                imagelotImageName3 = lotName3.ToString().TrimEnd('.') + " " + DateTime.Now.ToString("dd MMM yyyy HH'-'mm'-'ss", System.Globalization.CultureInfo.InvariantCulture) + "." + lotType3;
                                // file.SaveAs(HttpContext.Current.Server.MapPath("~/LotImages/") + imagelotImageName3);
                                file.SaveAs(localpath + imagelotImageName3);//local folder
                                file.SaveAs(folderpath + imagelotImageName3);//consumer Api folder
                            }
                        }

                    }
                    InsertLotImageData(imagelotImageName, imagelotImageName2, imagelotImageName3, lotObj);
                }
            }
            catch (Exception ex)
            { }

            context.Response.ContentType = "text/plain";
        }

        public string InsertLotImageData(string lotName, string lotName2, string lotName3, Lots lotobj)
        {
            try
            {
                string message = lotsDAL_obj.InsertLotImageData(lotName, lotName2, lotName3, lotobj);
                return message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public byte[] compressImageBytes(byte[] inputBytes)
        {
            var jpegQuality = 50;
            Image image;
            Byte[] outputBytes;
            using (var inputStream = new MemoryStream(inputBytes))
            {
                image = Image.FromStream(inputStream);
                var jpegEncoder = ImageCodecInfo.GetImageDecoders()
                  .First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                var encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, jpegQuality);
               
                using (var outputStream = new MemoryStream())
                {
                    image.Save(outputStream, jpegEncoder, encoderParameters);
                    outputBytes = outputStream.ToArray();
                }
            }
            return outputBytes;

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