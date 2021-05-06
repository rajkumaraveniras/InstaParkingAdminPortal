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
using System.IO.Compression;

namespace InstaParking.Handlers
{
   
    public class EmployeeFileHandler : IHttpHandler, IRequiresSessionState
    {
        string localpath = HttpContext.Current.Server.MapPath("~/EmployeeImages/");
        Users_DAL usersDAL_obj = new Users_DAL();

        //For compression
        int inputByteSize = 0;
        int BUFFER_SIZE = 100 * 1024; //64kB


        public void ProcessRequest(HttpContext context)
        {
            
            try
            {
                string employee_logo = "";

                byte[] EmpPhotoData = null;

                string EmpPhotoName = "";
                string EmpPhotoType = "";

                string AadharFileName = "";
                string AadharType = "";
                byte[] AadharData = null;

                string PANFileName = "";
                string PANType = "";
                byte[] PANData = null;

                string EmpPhoto = "";
                string EmpAadhar = "";
                string EmpPAN = "";

             


                if (context.Request.Files.Count > 0)
                {
                    HttpFileCollection files = context.Request.Files;
                    User usersObj = new User();
                    usersObj.UserID = Convert.ToInt32(context.Session["EmployeeID"].ToString());
                   

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

                        if (keyvalue == "EmpPhotoLogo")
                        {
                            using (var binaryReader = new BinaryReader(file.InputStream))
                            {
                                EmpPhotoData = binaryReader.ReadBytes(file.ContentLength);
                                //EmpPhotoData = compressImageBytes(EmpPhotoData);
                                EmpPhotoData = Compress(EmpPhotoData);
                               // EmpPhotoData = CompressByImageAlg(1, EmpPhotoData);
                                EmpPhotoName = file_split[0];
                                EmpPhotoType = file_split[file_split.Length - 1];
                                EmpPhoto = EmpPhotoName.ToString().TrimEnd('.') + " " + DateTime.Now.ToString("dd MMM yyyy HH'-'mm'-'ss", System.Globalization.CultureInfo.InvariantCulture) + "." + EmpPhotoType;
                                file.SaveAs(localpath + EmpPhoto);//local folder 
                            }
                        }
                        if (keyvalue == "AadharProof")
                        {
                            using (var binaryReader = new BinaryReader(file.InputStream))
                            {
                               // AadharData = binaryReader.ReadBytes(file.ContentLength);
                               // AadharData = compressImageBytes(AadharData);
                                AadharFileName = file_split[0];
                                AadharType = file_split[file_split.Length - 1];
                                EmpAadhar = AadharFileName.ToString().TrimEnd('.') + " " + DateTime.Now.ToString("dd MMM yyyy HH'-'mm'-'ss", System.Globalization.CultureInfo.InvariantCulture) + "." + AadharType;
                                file.SaveAs(localpath + EmpAadhar);//local folder 
                            }
                        }
                        if (keyvalue == "PANProof")
                        {
                            using (var binaryReader = new BinaryReader(file.InputStream))
                            {
                                //PANData = binaryReader.ReadBytes(file.ContentLength);
                               // PANData = compressImageBytes(PANData);
                                PANFileName = file_split[0];
                                PANType = file_split[file_split.Length - 1];
                                EmpPAN = PANFileName.ToString().TrimEnd('.') + " " + DateTime.Now.ToString("dd MMM yyyy HH'-'mm'-'ss", System.Globalization.CultureInfo.InvariantCulture) + "." + PANType;
                                file.SaveAs(localpath + EmpPAN);//local folder 
                            }
                        }
                    }
                    InsertEmployeeFiles(EmpPhoto, EmpAadhar, EmpPAN, usersObj,EmpPhotoData);
                    
                }
                else
                {
                    User usersObj = new User();
                    usersObj.UserID = Convert.ToInt32(context.Session["EmployeeID"].ToString());
                    string Photosting = Convert.ToString(context.Session["EmployeePhotoName"].ToString());
                    string path = HttpContext.Current.Server.MapPath("~/EmployeeImages/" + Photosting);
                    EmpPhotoData = File.ReadAllBytes(path);
                    EmpPhotoData = Compress(EmpPhotoData);
                    //EmpPhotoData = CompressByImageAlg(80L, EmpPhotoData);
                    UpdateEmployeePhoto(usersObj, EmpPhotoData);
                }
            }
           
            catch (Exception ex)
            {
               
            }
            context.Response.ContentType = "text/plain";            
        }

        public string InsertEmployeeFiles(string EmpPhoto,string EmpAadhar, string EmpPAN, User usersObj,byte[] EmpphotoBytes)
        {
            try
            {
                string message = usersDAL_obj.InsertEmployeeFiles(EmpPhoto, EmpAadhar, EmpPAN, usersObj, EmpphotoBytes);
                return message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public string UpdateEmployeePhoto(User usersObj, byte[] EmpphotoBytes)
        {
            try
            {
                string message = usersDAL_obj.UpdateEmployeePhoto(usersObj,EmpphotoBytes);
                return message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public byte[] compressImageBytes(byte[] inputBytes)
        {
            var jpegQuality = 10;
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

        
        public byte[] Compress(byte[] inputData)
        {

            if (inputData == null)
                throw new ArgumentNullException("inputData must be non-null");

            using (var compressIntoMs = new MemoryStream())
            {
                using (var gzs = new BufferedStream(new GZipStream(compressIntoMs,
                 CompressionMode.Compress), BUFFER_SIZE))
                {
                    gzs.Write(inputData, 0, inputData.Length);
                }
                return compressIntoMs.ToArray();
            }
        }

        //public byte[] CompressByImageAlg(long jpegQuality, byte[] data)
        //{
        //    using (MemoryStream inputStream = new MemoryStream(data))
        //    {
        //        using (Image image = Image.FromStream(inputStream))
        //        {
        //            ImageCodecInfo jpegEncoder = ImageCodecInfo.GetImageDecoders()
        //                .First(c => c.FormatID == ImageFormat.Jpeg.Guid);
        //            var encoderParameters = new EncoderParameters(1);
        //            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, jpegQuality);
        //            byte[] outputBytes = null;
        //            using (MemoryStream outputStream = new MemoryStream())
        //            {
        //                image.Save(outputStream, jpegEncoder, encoderParameters);
        //                return outputStream.ToArray();
        //            }
        //        }
        //    }
        //}

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}