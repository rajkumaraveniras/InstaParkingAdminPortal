using ClosedXML.Excel;
using InstaParking.DAL;
using InstaParking.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
//using System.Drawing;
using System.IO;
using System.Reflection;
using System.Web.Mvc;

namespace InstaParking.Controllers
{
    public class RevenueReportsController : Controller
    {
        RevenueReports_DAL revenueReportsDAL_obj = new RevenueReports_DAL();
        DALExceptionManagement objExceptionlog = new DALExceptionManagement();
        CompanyInfo_DAL companyInfoDAL_obj = new CompanyInfo_DAL();

        public JsonNetResult GetActiveChannelsList()
        {
            IList<ApplicationType> applicationTypesList = revenueReportsDAL_obj.GetActiveChannelsList();
            try
            {
                return new JsonNetResult()
                {
                    Data = applicationTypesList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        public JsonNetResult GetChannelListforPasses()
        {
            IList<ApplicationType> applicationTypesList = revenueReportsDAL_obj.GetChannelListforPasses();
            try
            {
                return new JsonNetResult()
                {
                    Data = applicationTypesList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        public JsonNetResult GetChannelListforPaymentType()
        {
            IList<ApplicationType> applicationTypesList = revenueReportsDAL_obj.GetChannelListforPaymentType();
            try
            {
                return new JsonNetResult()
                {
                    Data = applicationTypesList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        public JsonNetResult GetActiveReasonsList()
        {
            IList<ViolationReason> reasonList = revenueReportsDAL_obj.GetActiveReasonsList();

            try
            {
                return new JsonNetResult()
                {
                    Data = reasonList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }

        #region Station Report
        public ActionResult ReportByStation()
        {
            ViewBag.Menu = "RevenueReports";
            return View();
        }
        [HttpPost]
        public JsonResult GetReportByStation(SearchFilters stationFilterData)
        {
            try
            {
                if (stationFilterData.FromTime != null || stationFilterData.Duration == "Today")
                {
                    string fDate1 = Convert.ToString(stationFilterData.FromDate).Split(' ')[0];
                    DateTime fTotalDate = Convert.ToDateTime(fDate1 + " " + stationFilterData.FromTime + ":00 " + stationFilterData.FromMeridiem);

                    string tDate1 = Convert.ToString(stationFilterData.ToDate).Split(' ')[0];
                    DateTime tTotalDate = Convert.ToDateTime(tDate1 + " " + stationFilterData.ToTime + ":00 " + stationFilterData.ToMeridiem);

                    stationFilterData.FromDate = fTotalDate;
                    stationFilterData.ToDate = tTotalDate;
                }
                List<RevenueByStation> revenueByStation_lst = revenueReportsDAL_obj.GetReportByStation(stationFilterData);
                Session["StationReportData"] = revenueByStation_lst;
                return new JsonResult()
                {
                    Data = revenueByStation_lst,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult StationReportPDFDownload(string GrandTotal, string SelectedItems, string Collections, string CGST, string SGST)
        {
            try
            {
                var myDetails = JObject.Parse(SelectedItems);
                var selectedStation = myDetails["SelectedStation"];
                var selectedLot = myDetails["SelectedLot"];
                var SelectedVehicle = myDetails["SelectedVehicle"];
                string selectstation = ((Newtonsoft.Json.Linq.JValue)selectedStation).Value.ToString();
                string selectLot = ((Newtonsoft.Json.Linq.JValue)selectedLot).Value.ToString();
                string selectVehicle = ((Newtonsoft.Json.Linq.JValue)SelectedVehicle).Value.ToString();


                var fromDate = myDetails["FromDate"];
                var toDate = myDetails["ToDate"];
                var duration = myDetails["Duration"];
                string selectfromdate = ((Newtonsoft.Json.Linq.JValue)fromDate).Value.ToString();
                string selecttodate = ((Newtonsoft.Json.Linq.JValue)toDate).Value.ToString();
                string selectduration = ((Newtonsoft.Json.Linq.JValue)duration).Value.ToString();

                string FDate = "";
                string TDate = "";

                if (selectduration == "" || selectduration == null || selectduration == "0")
                {
                    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                }
                else
                {
                    FDate = DateTime.Parse(selectfromdate).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                }


                //string FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                //string TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");


                List<RevenueByStation> result = (List<RevenueByStation>)Session["StationReportData"];
                DataTable dt = ToDataTable(result);

                string TotalOperatorIn = Convert.ToString(dt.Rows[0]["TotalOperatorIn"]);
                string TotalPassesIn = Convert.ToString(dt.Rows[0]["TotalPassesIn"]);
                string TotalAppIn = Convert.ToString(dt.Rows[0]["TotalAppIn"]);
                string TotalCallIn = Convert.ToString(dt.Rows[0]["TotalCallIn"]);
                string TotalOut = Convert.ToString(dt.Rows[0]["TotalOut"]);
                string TotalFOC = Convert.ToString(dt.Rows[0]["TotalFOC"]);
                string TotalClamps = Convert.ToString(dt.Rows[0]["TotalClamps"]);

                string TotalCash = Convert.ToString(dt.Rows[0]["TotalCash"]);
                string TotalEPay = Convert.ToString(dt.Rows[0]["TotalEPay"]);

                string filename = GenerateReportNew(dt, GrandTotal, selectstation, selectLot, selectVehicle, FDate, TDate, TotalCash, TotalEPay, TotalOperatorIn, TotalPassesIn, TotalAppIn, TotalCallIn, TotalOut, TotalFOC, TotalClamps, Collections, CGST, SGST);
                FileStream sourceFile = null;

                if (filename != "")
                {
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("content-disposition", "attachment; filename=\"" + Path.GetFileName(filename) + "\"");
                    sourceFile = new FileStream(filename, FileMode.Open);
                    long FileSize;
                    FileSize = sourceFile.Length;
                    byte[] getContent = new byte[(int)FileSize];
                    sourceFile.Read(getContent, 0, (int)sourceFile.Length);
                    sourceFile.Close();
                    Response.Clear();
                    Response.BinaryWrite(getContent);
                    Response.End();
                }
                return Redirect("ReportByStation");
            }
            catch (Exception ex)
            {
                return Redirect("ReportByStation");
            }
        }
        public ActionResult StationReportExcelDownload(string GrandTotal, string SelectedItems, string Collections, string CGST, string SGST)
        {
            try
            {
                Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//18012021

                string path = System.Web.HttpContext.Current.Server.MapPath("~/ExcelReports");
                string filename = "RevenueByStationReport_" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".xlsx";
                string file_path = path + "/" + filename;

                var myDetails = JObject.Parse(SelectedItems);
                var fromDate = myDetails["FromDate"];
                var toDate = myDetails["ToDate"];
                var duration = myDetails["Duration"];
                string selectfromdate = ((Newtonsoft.Json.Linq.JValue)fromDate).Value.ToString();
                string selecttodate = ((Newtonsoft.Json.Linq.JValue)toDate).Value.ToString();
                string selectduration = ((Newtonsoft.Json.Linq.JValue)duration).Value.ToString();

                string FDate = "";
                string TDate = "";

                if (selectduration == "" || selectduration == null || selectduration == "0")
                {
                    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                }
                else
                {
                    FDate = DateTime.Parse(selectfromdate).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                }

                //string FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                //string TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");

                List<RevenueByStation> result = (List<RevenueByStation>)Session["StationReportData"];
                DataTable dt = ToDataTable(result);


                string TotalOperatorIn = Convert.ToString(dt.Rows[0]["TotalOperatorIn"]);
                string TotalPassesIn = Convert.ToString(dt.Rows[0]["TotalPassesIn"]);
                string TotalAppIn = Convert.ToString(dt.Rows[0]["TotalAppIn"]);
                string TotalCallIn = Convert.ToString(dt.Rows[0]["TotalCallIn"]);
                string TotalOut = Convert.ToString(dt.Rows[0]["TotalOut"]);
                string TotalFOC = Convert.ToString(dt.Rows[0]["TotalFOC"]);
                string TotalClamps = Convert.ToString(dt.Rows[0]["TotalClamps"]);

                string TotalCash = Convert.ToString(dt.Rows[0]["TotalCash"]);
                string TotalEPay = Convert.ToString(dt.Rows[0]["TotalEPay"]);



                if (dt.Rows.Count > 0)
                {
                    #region New Code
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add("RevenueReportByStation", 0);
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Value = companydata.AccountName;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.FontSize = 12;
                        wb.Worksheets.Worksheet(0).Cell(1, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(2, 1).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(2, 20).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(3, 1).Value = "Revenue By Station Report";
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(3, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 1).Value = "From : " + FDate + " To : " + TDate;
                        wb.Worksheets.Worksheet(0).Cell(4, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 5).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 6).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 7).Value = "Cur.Date : " + DateTime.Now.ToString("dd/MM/yyy").Split(' ')[0];
                        wb.Worksheets.Worksheet(0).Cell(4, 7).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(6, 1).Value = "Station";
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.FontSize = 11;
                       
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "Parking Lot";
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Operator In";
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.FontSize = 11;

                        //wb.Worksheets.Worksheet(0).Cell(6, 4).Value = "Passes In";
                        //wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 4).Value = "App In";
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.FontSize = 11;

                        //wb.Worksheets.Worksheet(0).Cell(6, 6).Value = "Call In";
                        //wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.FontSize = 11;

                        //wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Out";
                        //wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.FontSize = 11;

                        //wb.Worksheets.Worksheet(0).Cell(6, 8).Value = "FOC";
                        //wb.Worksheets.Worksheet(0).Cell(6, 8).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(6, 8).Style.Font.FontSize = 11;

                        //wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Clamps";
                        //wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.FontSize = 11;
                        //wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 5).Value = "Cash";
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 6).Value = "EPay";
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 7).Value = "Amount";
                        wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        int row = 7;
                        int column_num = 0;

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Value = Convert.ToString(dt.Rows[i]["Station"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Value = Convert.ToString(dt.Rows[i]["ParkingLot"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Value = Convert.ToString(dt.Rows[i]["OperatorIn"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            //wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Value = Convert.ToString(dt.Rows[i]["PassesIn"]);
                            //wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Value = Convert.ToString(dt.Rows[i]["AppIn"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            //wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Value = Convert.ToString(dt.Rows[i]["CallIn"]);
                            //wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Style.Font.FontSize = 11;

                            //wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Value = Convert.ToString(dt.Rows[i]["Out"]);
                            //wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Font.FontSize = 11;

                            //wb.Worksheets.Worksheet(0).Cell(row, column_num + 8).Value = Convert.ToString(dt.Rows[i]["FOC"]);
                            //wb.Worksheets.Worksheet(0).Cell(row, column_num + 8).Style.Font.FontSize = 11;

                            //wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Value = Convert.ToString(dt.Rows[i]["Clamps"]);
                            //wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Font.FontSize = 11;
                            //wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Value = Convert.ToString(dt.Rows[i]["Cash"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Value = Convert.ToString(dt.Rows[i]["EPay"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Value = Convert.ToString(dt.Rows[i]["Amount"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            row = row + 1;
                        }

                        // row = row + 1;
                        wb.Worksheets.Worksheet(0).Cell(row, 2).Value = "Total : ";
                        wb.Worksheets.Worksheet(0).Cell(row, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 2).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 3).Value = TotalOperatorIn;
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        //wb.Worksheets.Worksheet(0).Cell(row, 4).Value = TotalPassesIn;
                        //wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 4).Value = TotalAppIn;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        //wb.Worksheets.Worksheet(0).Cell(row, 6).Value = TotalCallIn;
                        //wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.FontSize = 11;

                        //wb.Worksheets.Worksheet(0).Cell(row, 3).Value = TotalOut;
                        //wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.FontSize = 11;

                        //wb.Worksheets.Worksheet(0).Cell(row, 8).Value = TotalFOC;
                        //wb.Worksheets.Worksheet(0).Cell(row, 8).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row, 8).Style.Font.FontSize = 11;

                        //wb.Worksheets.Worksheet(0).Cell(row, 3).Value = TotalClamps;
                        //wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.FontSize = 11;
                        //wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;


                        wb.Worksheets.Worksheet(0).Cell(row, 5).Value = TotalCash;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(row, 6).Value = TotalEPay;
                        wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(row, 7).Value = GrandTotal;
                        wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(row + 1, 2).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(row + 1, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row + 1, 2).Style.Font.FontSize = 11;

                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 3).Value = "Collections : "+Collections;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 3).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 3).Style.Font.FontSize = 11;

                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 4).Value = "CGST : "+CGST;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 4).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 4).Style.Font.FontSize = 11;

                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 5).Value = "SGST : "+ SGST;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 5).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 5).Style.Font.FontSize = 11;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(row + 1, 2).Value = "Parking Fee : ";
                        wb.Worksheets.Worksheet(0).Cell(row + 1, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row + 1, 2).Style.Font.FontSize = 12;
                        wb.Worksheets.Worksheet(0).Cell(row + 1, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        wb.Worksheets.Worksheet(0).Cell(row + 1, 7).Value = Collections;
                        wb.Worksheets.Worksheet(0).Cell(row + 1, 7).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row + 1, 7).Style.Font.FontSize = 12;
                        wb.Worksheets.Worksheet(0).Cell(row + 1, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(row + 2, 2).Value = "CGST 9%: ";
                        wb.Worksheets.Worksheet(0).Cell(row + 2, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row + 2, 2).Style.Font.FontSize = 12;
                        wb.Worksheets.Worksheet(0).Cell(row + 2, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        wb.Worksheets.Worksheet(0).Cell(row + 2, 7).Value = CGST;
                        wb.Worksheets.Worksheet(0).Cell(row + 2, 7).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row + 2, 7).Style.Font.FontSize = 12;
                        wb.Worksheets.Worksheet(0).Cell(row + 2, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(row + 3, 2).Value = "SGST 9%: ";
                        wb.Worksheets.Worksheet(0).Cell(row + 3, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row + 3, 2).Style.Font.FontSize = 12;
                        wb.Worksheets.Worksheet(0).Cell(row + 3, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        wb.Worksheets.Worksheet(0).Cell(row + 3, 7).Value = SGST;
                        wb.Worksheets.Worksheet(0).Cell(row + 3, 7).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row + 3, 7).Style.Font.FontSize = 12;
                        wb.Worksheets.Worksheet(0).Cell(row + 3, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(row + 4, 2).Value = "Grand Total : ";
                        wb.Worksheets.Worksheet(0).Cell(row + 4, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row + 4, 2).Style.Font.FontSize = 13;
                        wb.Worksheets.Worksheet(0).Cell(row + 4, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        wb.Worksheets.Worksheet(0).Cell(row + 4, 7).Value = GrandTotal;
                        wb.Worksheets.Worksheet(0).Cell(row + 4, 7).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row + 4, 7).Style.Font.FontSize = 13;
                        wb.Worksheets.Worksheet(0).Cell(row + 4, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;


                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                        using (MemoryStream MyMemoryStream = new MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                    #endregion

                }
                return Redirect("ReportByStation");
            }
            catch (Exception ex)
            {
                objExceptionlog.InsertException("Portal", ex.Message, "ReportController", "", "StationReportExcelDownload");
                return Redirect("ReportByStation");
            }
        }
        public string GenerateReportNew(DataTable dt, string grandTotal, string selectstation, string selectLot, string selectVehicle, string FDate, string TDate, string TotalCash, string TotalEPay, string TotalOperatorIn, string TotalPassesIn, string TotalAppIn, string TotalCallIn, string TotalOut, string TotalFOC, string TotalClamps, string Collections, string CGST, string SGST)
        {
            Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//07012021

            string filename, AttachmentName1 = "";
            string path = System.Web.HttpContext.Current.Server.MapPath("~/PDFReports");
            AttachmentName1 = "Report By Station" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".pdf";
            filename = path + "/" + AttachmentName1;

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(PageSize.A4);
            rec.BackgroundColor = new BaseColor(System.Drawing.Color.Olive);
            Document doc = new Document(rec, 88f, 88f, 10f, 50f);
            var output = new FileStream(filename, FileMode.Create);
            doc.SetPageSize(iTextSharp.text.PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, output);

            writer.PageEvent = new Footer();

            doc.Open();

            BaseColor FontColour = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#353535"));
            BaseColor FontColourBlack = BaseColor.BLACK;
            iTextSharp.text.Font calibri8 = FontFactory.GetFont("Calibri", 10f, FontColour);
            iTextSharp.text.Font _bf_headingaddr = FontFactory.GetFont("Calibri", 8f, FontColour);
            iTextSharp.text.Font _bf_headingaddrbold = FontFactory.GetFont("Calibri", 8f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_heading1 = FontFactory.GetFont("Calibri", 10f, FontColour);
            iTextSharp.text.Font _bf_headingbold = FontFactory.GetFont("Calibri", 10f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingTitle = FontFactory.GetFont("Calibri", 12f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingboldTable = FontFactory.GetFont("Calibri", 10f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingboldTableNrml = FontFactory.GetFont("Calibri", 9f, FontColour);
            iTextSharp.text.Font _bf_headingboldGSTTable = FontFactory.GetFont("Calibri", 11f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingboldGTotalTable = FontFactory.GetFont("Calibri", 12f, iTextSharp.text.Font.BOLD, FontColour);

            Phrase phrase = null;
            PdfPCell cell = null;
            PdfPTable table = null;

            #region Logo 
            //Header Table start
            table = new PdfPTable(2);
            table.TotalWidth = 550f;
            table.LockedWidth = true;
            table.SetWidths(new float[] { 0.3f, 0.7f });
            table.DefaultCell.Border = Rectangle.NO_BORDER;

            //Company Logo           
            // cell = ImageCell("~/assets/images/Logo4.png", 70f, PdfPCell.ALIGN_CENTER);
            if (companydata.CompanyLogo != null && companydata.CompanyLogo != "")
            {
                cell = ImageCell("~/Images/" + companydata.CompanyLogo, 20f, PdfPCell.ALIGN_CENTER);
            }
            else
            {
                cell = ImageCell("~/Images/default-logo.png", 20f, PdfPCell.ALIGN_CENTER);
            }
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;

            cell.Border = 0;
            table.AddCell(cell);

            PdfPTable innertbl = new PdfPTable(1);
            innertbl.SetWidths(new float[] { 1.0f });
            PdfPCell innercell1 = null;
            phrase = new Phrase();
            //phrase.Add(new Chunk("                                                        Unit #3A, Plot No:847, Pacific Towers,", _bf_headingaddr));
            phrase.Add(new Chunk("                                                              " + companydata.Address1, _bf_headingaddr));
            innercell1 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell1.Border = 0;
            innercell1.Padding = 0;

            innertbl.SpacingBefore = 0f;
            innertbl.SpacingAfter = 2f;
            innertbl.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl.AddCell(innercell1);

            cell = new PdfPCell();
            cell.Border = 0;
            cell.Padding = 0;
            cell.AddElement(innertbl);

            PdfPTable innertbl2 = new PdfPTable(1);
            innertbl2.SetWidths(new float[] { 1.0f });
            PdfPCell innercell2 = null;
            phrase = new Phrase();
            //phrase.Add(new Chunk("                                                        Madhapur, Hyderabad-500081, TS", _bf_headingaddr));
            phrase.Add(new Chunk("                                                              " + companydata.Address2, _bf_headingaddr));
            innercell2 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell2.Border = 0;
            innercell2.Padding = 0;
            innertbl2.SpacingBefore = 0f;
            innertbl2.SpacingAfter = 2f;
            innertbl2.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl2.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl2.AddCell(innercell2);
            cell.AddElement(innertbl2);

            PdfPTable innertbl3 = new PdfPTable(1);
            innertbl3.SetWidths(new float[] { 1.0f });
            PdfPCell innercellTwo = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              GSTIN: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.GSTNumber, _bf_headingaddr));
            //phrase.Add(new Chunk("36AABCT3518Q1ZX", _bf_headingaddr));
            innercellTwo = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercellTwo.Border = 0;
            innercellTwo.Padding = 0;
            innertbl3.SpacingBefore = 0f;
            innertbl3.SpacingAfter = 2f;
            innertbl3.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl3.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl3.AddCell(innercellTwo);
            cell.AddElement(innertbl3);


            PdfPTable innertbl4 = new PdfPTable(1);
            innertbl4.SetWidths(new float[] { 1.0f });
            PdfPCell innercell5 = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              Ph: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.ContactNumber, _bf_headingaddr));
            //phrase.Add(new Chunk("+91 8143143143", _bf_headingaddr));
            innercell5 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell5.Border = 0;
            innercell5.Padding = 0;
            innertbl4.SpacingBefore = 0f;
            innertbl4.SpacingAfter = 2f;
            innertbl4.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl4.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl4.AddCell(innercell5);
            cell.AddElement(innertbl4);
            //table.AddCell(cell);

            PdfPTable innertbl5 = new PdfPTable(1);
            innertbl5.SetWidths(new float[] { 1.0f });
            PdfPCell innercell6 = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              Email: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.SupportEmailID, _bf_headingaddr));
            //phrase.Add(new Chunk("support@hmrl.com", _bf_headingaddr));
            innercell6 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell6.Border = 0;
            innercell6.Padding = 0;
            innertbl5.SpacingBefore = 0f;
            innertbl5.SpacingAfter = 0f;
            innertbl5.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl5.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl5.AddCell(innercell6);
            cell.AddElement(innertbl5);
            table.AddCell(cell);

            doc.Add(table);

            //Header Table end
            #endregion

            #region Title
            //Title table start
            PdfPTable tableTitle = new PdfPTable(1);
            tableTitle.DefaultCell.FixedHeight = 150f;
            tableTitle.TotalWidth = 550f;
            tableTitle.LockedWidth = true;
            tableTitle.SetWidths(new float[] { 1f });
            tableTitle.SpacingBefore = 10f;
            tableTitle.SpacingAfter = 3f;

            phrase = new Phrase();
            phrase.Add(new Chunk("REPORT BY STATION", _bf_headingTitle));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
            cell.Border = 0;
            tableTitle.AddCell(cell);
            doc.Add(tableTitle);
            //Title table end
            #endregion

            #region Filter 
            PdfPTable tablefilter = new PdfPTable(1);
            tablefilter.TotalWidth = 550f;
            tablefilter.LockedWidth = true;
            tablefilter.SetWidths(new float[] { 1.0f });
            tablefilter.SpacingBefore = 3f;
            tablefilter.SpacingAfter = 3f;


            phrase = new Phrase();
            phrase.Add(new Chunk("Filtered By      :  ", _bf_headingboldTable));
            // phrase.Add(new Chunk("Filtered By : ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, 1, new BaseColor(System.Drawing.ColorTranslator.FromHtml("#353535")))));
            phrase.Add(new Chunk(selectstation + ", " + selectLot + ", " + selectVehicle, _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tablefilter.AddCell(cell);
            doc.Add(tablefilter);
            #endregion

            #region time
            PdfPTable tabletime = new PdfPTable(1);
            tabletime.TotalWidth = 550f;
            tabletime.LockedWidth = true;
            tabletime.SetWidths(new float[] { 1.0f });
            tabletime.SpacingBefore = 3f;
            tabletime.SpacingAfter = 3f;

            phrase = new Phrase();
            phrase.Add(new Chunk("Report From   :  ", _bf_headingboldTable));
            // phrase.Add(new Chunk("Report From : ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, 1, new BaseColor(System.Drawing.ColorTranslator.FromHtml("#353535")))));
            phrase.Add(new Chunk(FDate + " to " + TDate, _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tabletime.AddCell(cell);
            doc.Add(tabletime);

            PdfPTable tabletime1 = new PdfPTable(1);
            tabletime1.TotalWidth = 550f;
            tabletime1.LockedWidth = true;
            tabletime1.SetWidths(new float[] { 1.0f });
            tabletime1.SpacingBefore = 3f;
            tabletime1.SpacingAfter = 5f;
            phrase = new Phrase();
            phrase.Add(new Chunk("Generated On :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(DateTime.Now.ToString("dd/MM/yyyy HH:MM tt"), _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tabletime1.AddCell(cell);
            doc.Add(tabletime1);
            #endregion

            #region Data
            //PdfPTable table2 = new PdfPTable(dt.Columns.Count - 9);          
            //table2.SetWidths(new float[] { 0.2f, 0.15f, 0.12f, 0.1f, 0.07f, 0.07f, 0.07f, 0.07f, 0.1f, 0.1f, 0.1f, 0.1f });
            PdfPTable table2 = new PdfPTable(dt.Columns.Count - 14);
            table2.SetWidths(new float[] { 0.2f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f });
            table2.TotalWidth = doc.PageSize.Width - 40f;
            table2.LockedWidth = true;
            table2.SpacingBefore = 10f;
            // table2.SpacingAfter = 3f;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string cellText = Server.HtmlDecode(dt.Columns[i].ColumnName);
                if (cellText != "TotalCash" && cellText != "TotalEPay" && cellText != "TotalOperatorIn" && cellText != "TotalPassesIn" && cellText != "TotalAppIn"
                    && cellText != "TotalCallIn" && cellText != "TotalOut" && cellText != "TotalFOC" && cellText != "TotalClamps"
                    && cellText != "PassesIn" && cellText != "Clamps" && cellText != "CallIn" && cellText != "FOC" && cellText != "Out")
                {
                    PdfPCell cell2 = new PdfPCell();

                    if (cellText == "Cash" || cellText == "EPay" || cellText == "Amount")
                    {
                        cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                        cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                        cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    }
                    else
                    {
                        if (cellText == "ParkingLot")
                        {
                            cell2.Phrase = new Phrase("Parking Lot", _bf_headingboldTable);
                        }
                        else if (cellText == "OperatorIn")
                        {
                            cell2.Phrase = new Phrase("Operator In", _bf_headingboldTable);
                        }
                        //else if (cellText == "PassesIn")
                        //{
                        //    cell2.Phrase = new Phrase("Passes In", _bf_headingboldTable);
                        //}
                        else if (cellText == "AppIn")
                        {
                            cell2.Phrase = new Phrase("App In", _bf_headingboldTable);
                        }
                        //else if (cellText == "CallIn")
                        //{
                        //    cell2.Phrase = new Phrase("Call In", _bf_headingboldTable);
                        //}
                        else
                        {
                            cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                        }
                        cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                        cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    }
                    table2.AddCell(cell2);
                }
            }

            //writing table Data  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string cellText = Server.HtmlDecode(dt.Columns[j].ColumnName);
                    if (cellText != "TotalCash" && cellText != "TotalEPay" && cellText != "TotalOperatorIn" && cellText != "TotalPassesIn" && cellText != "TotalAppIn"
                    && cellText != "TotalCallIn" && cellText != "TotalOut" && cellText != "TotalFOC" && cellText != "TotalClamps"
                    && cellText != "PassesIn" && cellText != "Clamps" && cellText != "CallIn" && cellText != "FOC" && cellText != "Out")
                    {
                        PdfPCell cell2 = new PdfPCell();
                        if (cellText == "Cash" || cellText == "EPay" || cellText == "Amount")
                        {
                            cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                            cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell2.VerticalAlignment = Element.ALIGN_RIGHT;
                        }
                        else
                        {
                            cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                            cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                            // cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                        }

                        table2.AddCell(cell2);
                    }
                }
            }
            //GRAND TOTAL TABLE
            //PdfPTable grandTable = new PdfPTable(dt.Columns.Count - 2);
            //grandTable.TotalWidth = doc.PageSize.Width - 40f;
            //grandTable.LockedWidth = true;
            //PdfPCell cell3 = new PdfPCell();
            //string totalstring = string.Concat("Total      " + TotalCash + "     " + TotalEPay + "     " + grandTotal);
            //cell3.Phrase = new Phrase(totalstring, _bf_headingboldTable);
            //cell3.Colspan = dt.Columns.Count;
            //cell3.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cell3.BorderWidthTop = 0;
            //grandTable.AddCell(cell3);


            //PdfPTable grandTable = new PdfPTable(dt.Columns.Count - 9);
            //grandTable.SetWidths(new float[] { 0.2f, 0.15f, 0.12f, 0.1f, 0.07f, 0.07f, 0.07f, 0.07f, 0.1f, 0.1f, 0.1f, 0.1f });
            PdfPTable grandTable = new PdfPTable(dt.Columns.Count - 14);
            grandTable.SetWidths(new float[] { 0.2f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f });
            grandTable.TotalWidth = doc.PageSize.Width - 40f;
            grandTable.LockedWidth = true;

            PdfPCell cellt = new PdfPCell(new Phrase(" ", _bf_headingboldTable));
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cellt = new PdfPCell();
            cellt.Phrase = new Phrase("Total ", _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
            cellt.BorderWidthLeft = 0;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalOperatorIn, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalAppIn, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalCash, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalEPay, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(grandTotal, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthTop = 0;
            cellt.BorderWidthRight = 1;
            grandTable.AddCell(cellt);

            //GRNAD TOTAL TABLE

            //ParkingFee Table
            PdfPTable gstTable = new PdfPTable(dt.Columns.Count - 14);
            gstTable.SetWidths(new float[] { 0.2f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f });
            gstTable.TotalWidth = doc.PageSize.Width - 40f;
            gstTable.LockedWidth = true;


            PdfPCell cellGST = new PdfPCell();
            cellGST.Phrase = new Phrase(" ", _bf_headingboldGSTTable);
            cellGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellGST.BorderWidthRight = 0;
            cellGST.BorderWidthTop = 0;          
            gstTable.AddCell(cellGST);

            cellGST = new PdfPCell();
            cellGST.Phrase = new Phrase("Parking Fee  ", _bf_headingboldGSTTable);
            cellGST.HorizontalAlignment = Element.ALIGN_LEFT;
            cellGST.BorderWidthLeft = 0;
            cellGST.BorderWidthRight = 0;
            cellGST.BorderWidthTop = 0;
            gstTable.AddCell(cellGST);

            cellGST = new PdfPCell();
            cellGST.Phrase = new Phrase(" ", _bf_headingboldGSTTable);
            cellGST.HorizontalAlignment = Element.ALIGN_LEFT;
            cellGST.BorderWidthLeft = 0;
            cellGST.BorderWidthRight = 0;
            cellGST.BorderWidthTop = 0;
            gstTable.AddCell(cellGST);

            cellGST = new PdfPCell();
            cellGST.Phrase = new Phrase(" ", _bf_headingboldGSTTable);
            cellGST.HorizontalAlignment = Element.ALIGN_LEFT;
            cellGST.BorderWidthLeft = 0;
            cellGST.BorderWidthRight = 0;
            cellGST.BorderWidthTop = 0;
            gstTable.AddCell(cellGST);

            cellGST = new PdfPCell();
            cellGST.Phrase = new Phrase(" ", _bf_headingboldGSTTable);
            cellGST.HorizontalAlignment = Element.ALIGN_LEFT;
            cellGST.BorderWidthLeft = 0;
            cellGST.BorderWidthRight = 0;
            cellGST.BorderWidthTop = 0;
            gstTable.AddCell(cellGST);

            cellGST = new PdfPCell();
            cellGST.Phrase = new Phrase(" ", _bf_headingboldGSTTable);
            cellGST.HorizontalAlignment = Element.ALIGN_LEFT;
            cellGST.BorderWidthLeft = 0;
            cellGST.BorderWidthRight = 0;
            cellGST.BorderWidthTop = 0;
            gstTable.AddCell(cellGST);

            cellGST = new PdfPCell();
            cellGST.Phrase = new Phrase(Collections, _bf_headingboldGSTTable);
            //cellGST.Phrase = new Phrase("Parking Fee : " + Collections + "\r\n" + "CGST : " + CGST + "\r\n" + "SGST : " + SGST + "\r\n" + "Grand Total : " + grandTotal, _bf_headingboldTable);
            cellGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellGST.BorderWidthTop = 0;
            cellGST.BorderWidthRight = 1;
            cellGST.BorderWidthLeft = 0;
            cellGST.Colspan = 2;
            gstTable.AddCell(cellGST);


            //CGST Table
            PdfPTable cgstTable = new PdfPTable(dt.Columns.Count - 14);
            cgstTable.SetWidths(new float[] { 0.2f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f });
            cgstTable.TotalWidth = doc.PageSize.Width - 40f;
            cgstTable.LockedWidth = true;


            PdfPCell cellCGST = new PdfPCell();
            cellCGST.Phrase = new Phrase(" ", _bf_headingboldGSTTable);
            cellCGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellCGST.BorderWidthRight = 0;
            cellCGST.BorderWidthTop = 0;
            cgstTable.AddCell(cellCGST);

            cellCGST = new PdfPCell();
            cellCGST.Phrase = new Phrase("CGST 9% ", _bf_headingboldGSTTable);
            cellCGST.HorizontalAlignment = Element.ALIGN_LEFT;
            cellCGST.BorderWidthLeft = 0;
            cellCGST.BorderWidthRight = 0;
            cellCGST.BorderWidthTop = 0;
            cgstTable.AddCell(cellCGST);

            cellCGST = new PdfPCell();
            cellCGST.Phrase = new Phrase(" ", _bf_headingboldGSTTable);
            cellCGST.HorizontalAlignment = Element.ALIGN_LEFT;
            cellCGST.BorderWidthLeft = 0;
            cellCGST.BorderWidthRight = 0;
            cellCGST.BorderWidthTop = 0;
            cgstTable.AddCell(cellCGST);

            cellCGST = new PdfPCell();
            cellCGST.Phrase = new Phrase(" ", _bf_headingboldGSTTable);
            cellCGST.HorizontalAlignment = Element.ALIGN_LEFT;
            cellCGST.BorderWidthLeft = 0;
            cellCGST.BorderWidthRight = 0;
            cellCGST.BorderWidthTop = 0;
            cgstTable.AddCell(cellCGST);

            cellCGST = new PdfPCell();
            cellCGST.Phrase = new Phrase(" ", _bf_headingboldGSTTable);
            cellCGST.HorizontalAlignment = Element.ALIGN_LEFT;
            cellCGST.BorderWidthLeft = 0;
            cellCGST.BorderWidthRight = 0;
            cellCGST.BorderWidthTop = 0;
            cgstTable.AddCell(cellCGST);

            cellCGST = new PdfPCell();
            cellCGST.Phrase = new Phrase(" ", _bf_headingboldGSTTable);
            cellCGST.HorizontalAlignment = Element.ALIGN_LEFT;
            cellCGST.BorderWidthLeft = 0;
            cellCGST.BorderWidthRight = 0;
            cellCGST.BorderWidthTop = 0;
            cgstTable.AddCell(cellCGST);

            cellCGST = new PdfPCell();
            cellCGST.Phrase = new Phrase(CGST, _bf_headingboldGSTTable);
            //cellGST.Phrase = new Phrase("Parking Fee : " + Collections + "\r\n" + "CGST : " + CGST + "\r\n" + "SGST : " + SGST + "\r\n" + "Grand Total : " + grandTotal, _bf_headingboldTable);
            cellCGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellCGST.BorderWidthTop = 0;
            cellCGST.BorderWidthRight = 1;
            cellCGST.BorderWidthLeft = 0;
            cgstTable.AddCell(cellCGST);

            //SGST Table
            PdfPTable sgstTable = new PdfPTable(dt.Columns.Count - 14);
            sgstTable.SetWidths(new float[] { 0.2f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f });
            sgstTable.TotalWidth = doc.PageSize.Width - 40f;
            sgstTable.LockedWidth = true;


            PdfPCell cellSGST = new PdfPCell();
            cellSGST.Phrase = new Phrase(" ", _bf_headingboldGSTTable);
            cellSGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellSGST.BorderWidthRight = 0;
            cellSGST.BorderWidthTop = 0;
            sgstTable.AddCell(cellSGST);

            cellSGST = new PdfPCell();
            cellSGST.Phrase = new Phrase("SGST 9% ", _bf_headingboldGSTTable);
            cellSGST.HorizontalAlignment = Element.ALIGN_LEFT;
            cellSGST.BorderWidthLeft = 0;
            cellSGST.BorderWidthRight = 0;
            cellSGST.BorderWidthTop = 0;
            sgstTable.AddCell(cellSGST);

            cellSGST = new PdfPCell();
            cellSGST.Phrase = new Phrase(" ", _bf_headingboldGSTTable);
            cellSGST.HorizontalAlignment = Element.ALIGN_LEFT;
            cellSGST.BorderWidthLeft = 0;
            cellSGST.BorderWidthRight = 0;
            cellSGST.BorderWidthTop = 0;
            sgstTable.AddCell(cellSGST);

            cellSGST = new PdfPCell();
            cellSGST.Phrase = new Phrase(" ", _bf_headingboldGSTTable);
            cellSGST.HorizontalAlignment = Element.ALIGN_LEFT;
            cellSGST.BorderWidthLeft = 0;
            cellSGST.BorderWidthRight = 0;
            cellSGST.BorderWidthTop = 0;
            sgstTable.AddCell(cellSGST);

            cellSGST = new PdfPCell();
            cellSGST.Phrase = new Phrase(" ", _bf_headingboldGSTTable);
            cellSGST.HorizontalAlignment = Element.ALIGN_LEFT;
            cellSGST.BorderWidthLeft = 0;
            cellSGST.BorderWidthRight = 0;
            cellSGST.BorderWidthTop = 0;
            sgstTable.AddCell(cellSGST);

            cellSGST = new PdfPCell();
            cellSGST.Phrase = new Phrase(" ", _bf_headingboldGSTTable);
            cellSGST.HorizontalAlignment = Element.ALIGN_LEFT;
            cellSGST.BorderWidthLeft = 0;
            cellSGST.BorderWidthRight = 0;
            cellSGST.BorderWidthTop = 0;
            sgstTable.AddCell(cellSGST);

            cellSGST = new PdfPCell();
            cellSGST.Phrase = new Phrase(SGST, _bf_headingboldGSTTable);
            //cellGST.Phrase = new Phrase("Parking Fee : " + Collections + "\r\n" + "CGST : " + CGST + "\r\n" + "SGST : " + SGST + "\r\n" + "Grand Total : " + grandTotal, _bf_headingboldTable);
            cellSGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellSGST.BorderWidthTop = 0;
            cellSGST.BorderWidthRight = 1;
            cellSGST.BorderWidthLeft = 0;
            sgstTable.AddCell(cellSGST);

            //GTotal Table
            PdfPTable gTotalTable = new PdfPTable(dt.Columns.Count - 14);
            gTotalTable.SetWidths(new float[] { 0.2f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f });
            gTotalTable.TotalWidth = doc.PageSize.Width - 40f;
            gTotalTable.LockedWidth = true;


            PdfPCell cellGTotal = new PdfPCell();
            cellGTotal.Phrase = new Phrase(" ", _bf_headingboldGTotalTable);
            cellGTotal.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellGTotal.BorderWidthRight = 0;
            cellGTotal.BorderWidthTop = 0;
            gTotalTable.AddCell(cellGTotal);

            cellGTotal = new PdfPCell();
            cellGTotal.Phrase = new Phrase("Grand Total  ", _bf_headingboldGTotalTable);
            cellGTotal.HorizontalAlignment = Element.ALIGN_LEFT;
            cellGTotal.BorderWidthLeft = 0;
            cellGTotal.BorderWidthRight = 0;
            cellGTotal.BorderWidthTop = 0;
            gTotalTable.AddCell(cellGTotal);

            cellGTotal = new PdfPCell();
            cellGTotal.Phrase = new Phrase(" ", _bf_headingboldGTotalTable);
            cellGTotal.HorizontalAlignment = Element.ALIGN_LEFT;
            cellGTotal.BorderWidthLeft = 0;
            cellGTotal.BorderWidthRight = 0;
            cellGTotal.BorderWidthTop = 0;
            gTotalTable.AddCell(cellGTotal);

            cellGTotal = new PdfPCell();
            cellGTotal.Phrase = new Phrase(" ", _bf_headingboldGTotalTable);
            cellGTotal.HorizontalAlignment = Element.ALIGN_LEFT;
            cellGTotal.BorderWidthLeft = 0;
            cellGTotal.BorderWidthRight = 0;
            cellGTotal.BorderWidthTop = 0;
            gTotalTable.AddCell(cellGTotal);

            cellGTotal = new PdfPCell();
            cellGTotal.Phrase = new Phrase(" ", _bf_headingboldGTotalTable);
            cellGTotal.HorizontalAlignment = Element.ALIGN_LEFT;
            cellGTotal.BorderWidthLeft = 0;
            cellGTotal.BorderWidthRight = 0;
            cellGTotal.BorderWidthTop = 0;
            gTotalTable.AddCell(cellGTotal);

            cellGTotal = new PdfPCell();
            cellGTotal.Phrase = new Phrase(" ", _bf_headingboldGTotalTable);
            cellGTotal.HorizontalAlignment = Element.ALIGN_LEFT;
            cellGTotal.BorderWidthLeft = 0;
            cellGTotal.BorderWidthRight = 0;
            cellGTotal.BorderWidthTop = 0;
            gTotalTable.AddCell(cellGTotal);

            cellGTotal = new PdfPCell();
            cellGTotal.Phrase = new Phrase(grandTotal, _bf_headingboldGTotalTable);
            //cellGST.Phrase = new Phrase("Parking Fee : " + Collections + "\r\n" + "CGST : " + CGST + "\r\n" + "SGST : " + SGST + "\r\n" + "Grand Total : " + grandTotal, _bf_headingboldTable);
            cellGTotal.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellGTotal.BorderWidthTop = 0;
            cellGTotal.BorderWidthRight = 1;
            cellGTotal.BorderWidthLeft = 0;
            gTotalTable.AddCell(cellGTotal);

            doc.Add(table2);
            doc.Add(grandTable);
            doc.Add(gstTable);
            doc.Add(cgstTable);
            doc.Add(sgstTable);
            doc.Add(gTotalTable);
            #endregion

            doc.Close();
            byte[] result = ms.ToArray();
            return filename;

        }

        #endregion

        #region Vehicle Report
        public ActionResult ReportByVehicle()
        {
            ViewBag.Menu = "RevenueReports";
            return View();
        }
        [HttpPost]
        public JsonResult GetReportByVehicle(SearchFilters vehicleFilterData)
        {
            try
            {
                if (vehicleFilterData.FromTime != null || vehicleFilterData.Duration == "Today")
                {
                    string fDate1 = Convert.ToString(vehicleFilterData.FromDate).Split(' ')[0];
                    DateTime fTotalDate = Convert.ToDateTime(fDate1 + " " + vehicleFilterData.FromTime + ":00 " + vehicleFilterData.FromMeridiem);

                    string tDate1 = Convert.ToString(vehicleFilterData.ToDate).Split(' ')[0];
                    DateTime tTotalDate = Convert.ToDateTime(tDate1 + " " + vehicleFilterData.ToTime + ":00 " + vehicleFilterData.ToMeridiem);

                    vehicleFilterData.FromDate = fTotalDate;
                    vehicleFilterData.ToDate = tTotalDate;
                }

                List<RevenueByVehicle> revenueByVehicle_lst = revenueReportsDAL_obj.GetReportByVehicle(vehicleFilterData);
                Session["VehicleReportData"] = revenueByVehicle_lst;
                return new JsonResult()
                {
                    Data = revenueByVehicle_lst,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult VehicleReportPDFDownload(string GrandTotal, string SelectedItems)
        {
            try
            {
                var myDetails = JObject.Parse(SelectedItems);
                var selectedStation = myDetails["SelectedStation"];
                var selectedLot = myDetails["SelectedLot"];
                var SelectedVehicle = myDetails["SelectedVehicle"];
                string selectstation = ((Newtonsoft.Json.Linq.JValue)selectedStation).Value.ToString();
                string selectLot = ((Newtonsoft.Json.Linq.JValue)selectedLot).Value.ToString();
                string selectVehicle = ((Newtonsoft.Json.Linq.JValue)SelectedVehicle).Value.ToString();

                var fromDate = myDetails["FromDate"];
                var toDate = myDetails["ToDate"];
                var duration = myDetails["Duration"];
                string selectfromdate = ((Newtonsoft.Json.Linq.JValue)fromDate).Value.ToString();
                string selecttodate = ((Newtonsoft.Json.Linq.JValue)toDate).Value.ToString();
                string selectduration = ((Newtonsoft.Json.Linq.JValue)duration).Value.ToString();

                string FDate = "";
                string TDate = "";

                if (selectduration == "" || selectduration == null || selectduration == "0")
                {
                    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                }
                else
                {
                    FDate = DateTime.Parse(selectfromdate).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                }
                //string FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                //string TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");

                List<RevenueByVehicle> result = (List<RevenueByVehicle>)Session["VehicleReportData"];
                DataTable dt = ToDataTable(result);

                string TotalIn = Convert.ToString(dt.Rows[0]["TotalIn"]);
                string TotalOut = Convert.ToString(dt.Rows[0]["TotalOut"]);
                string TotalFOC = Convert.ToString(dt.Rows[0]["TotalFOC"]);
                string TotalClamps = Convert.ToString(dt.Rows[0]["TotalClamps"]);

                string TotalCash = Convert.ToString(dt.Rows[0]["TotalCash"]);
                string TotalEPay = Convert.ToString(dt.Rows[0]["TotalEPay"]);

                string filename = GenerateReportByVehicleNew(dt, GrandTotal, selectstation, selectLot, selectVehicle, FDate, TDate, TotalCash, TotalEPay, TotalIn, TotalOut, TotalFOC, TotalClamps);
                FileStream sourceFile = null;

                if (filename != "")
                {
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("content-disposition", "attachment; filename=\"" + Path.GetFileName(filename) + "\"");
                    sourceFile = new FileStream(filename, FileMode.Open);
                    long FileSize;
                    FileSize = sourceFile.Length;
                    byte[] getContent = new byte[(int)FileSize];
                    sourceFile.Read(getContent, 0, (int)sourceFile.Length);
                    sourceFile.Close();
                    Response.Clear();
                    Response.BinaryWrite(getContent);
                    Response.End();
                }
                return Redirect("ReportByVehicle");
            }
            catch (Exception ex)
            {
                return Redirect("ReportByVehicle");
            }
        }
        public ActionResult VehicleReportExcelDownload(string GrandTotal, string SelectedItems)
        {
            try
            {
                Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//18012021

                var myDetails = JObject.Parse(SelectedItems);
                var fromDate = myDetails["FromDate"];
                var toDate = myDetails["ToDate"];
                var duration = myDetails["Duration"];
                string selectfromdate = ((Newtonsoft.Json.Linq.JValue)fromDate).Value.ToString();
                string selecttodate = ((Newtonsoft.Json.Linq.JValue)toDate).Value.ToString();
                string selectduration = ((Newtonsoft.Json.Linq.JValue)duration).Value.ToString();

                string FDate = "";
                string TDate = "";

                if (selectduration == "" || selectduration == null || selectduration == "0")
                {
                    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                }
                else
                {
                    FDate = DateTime.Parse(selectfromdate).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                }
                //string FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                //string TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");


                List<RevenueByVehicle> result = (List<RevenueByVehicle>)Session["VehicleReportData"];
                DataTable dt = ToDataTable(result);

                string TotalIn = Convert.ToString(dt.Rows[0]["TotalIn"]);
                string TotalOut = Convert.ToString(dt.Rows[0]["TotalOut"]);
                string TotalFOC = Convert.ToString(dt.Rows[0]["TotalFOC"]);
                string TotalClamps = Convert.ToString(dt.Rows[0]["TotalClamps"]);

                string TotalCash = Convert.ToString(dt.Rows[0]["TotalCash"]);
                string TotalEPay = Convert.ToString(dt.Rows[0]["TotalEPay"]);

                string path = System.Web.HttpContext.Current.Server.MapPath("~/ExcelReports");
                string filename = "RevenueByVehicleReport_" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".xlsx";
                string file_path = path + "/" + filename;

                if (dt.Rows.Count > 0)
                {
                    #region New Code
                    using (XLWorkbook wb = new XLWorkbook())
                    {

                        wb.Worksheets.Add("RevenueReportByVehicle", 0);
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Value = companydata.AccountName;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.FontSize = 12;
                        wb.Worksheets.Worksheet(0).Cell(1, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(2, 1).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(2, 20).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(3, 1).Value = "Revenue By Vehicle Report";
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(3, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 1).Value = "From : " + FDate + " To : " + TDate;
                        wb.Worksheets.Worksheet(0).Cell(4, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 5).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 6).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 7).Value = "Cur.Date : " + DateTime.Now.ToString("dd/MM/yyy").Split(' ')[0];
                        wb.Worksheets.Worksheet(0).Cell(4, 7).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(6, 1).Value = "Vehicle Type";
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "Station";
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Parking Lot";
                        //wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Lot Code";
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 4).Value = "In";
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 5).Value = "Out";
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 6).Value = "FOC";
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 7).Value = "Clamps";
                        wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 8).Value = "Cash";
                        wb.Worksheets.Worksheet(0).Cell(6, 8).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 8).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 9).Value = "EPay";
                        wb.Worksheets.Worksheet(0).Cell(6, 9).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 9).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 10).Value = "Amount";
                        wb.Worksheets.Worksheet(0).Cell(6, 10).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 10).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        int row = 7;
                        int column_num = 0;


                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Value = Convert.ToString(dt.Rows[i]["VehicleType"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Value = Convert.ToString(dt.Rows[i]["Station"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Value = Convert.ToString(dt.Rows[i]["ParkingLot"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Value = Convert.ToString(dt.Rows[i]["In"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Value = Convert.ToString(dt.Rows[i]["Out"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Value = Convert.ToString(dt.Rows[i]["FOC"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Value = Convert.ToString(dt.Rows[i]["Clamps"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 8).Value = Convert.ToString(dt.Rows[i]["Cash"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 8).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 9).Value = Convert.ToString(dt.Rows[i]["EPay"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 9).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 10).Value = Convert.ToString(dt.Rows[i]["Amount"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 10).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            row = row + 1;
                        }

                        //row = row + 1;
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Value = "Total";
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        wb.Worksheets.Worksheet(0).Cell(row, 4).Value = TotalIn;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        wb.Worksheets.Worksheet(0).Cell(row, 5).Value = TotalOut;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        wb.Worksheets.Worksheet(0).Cell(row, 6).Value = TotalFOC;
                        wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        wb.Worksheets.Worksheet(0).Cell(row, 7).Value = TotalClamps;
                        wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        wb.Worksheets.Worksheet(0).Cell(row, 8).Value = TotalCash;
                        wb.Worksheets.Worksheet(0).Cell(row, 8).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 8).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(row, 9).Value = TotalEPay;
                        wb.Worksheets.Worksheet(0).Cell(row, 9).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 9).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(row, 10).Value = GrandTotal;
                        wb.Worksheets.Worksheet(0).Cell(row, 10).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 10).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 9).Value = "GST";
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 9).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 9).Style.Font.FontSize = 11;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 10).Value = GST;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 10).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 10).Style.Font.FontSize = 11;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;


                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                        using (MemoryStream MyMemoryStream = new MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                    #endregion

                }
                return Redirect("ReportByVehicle");
            }
            catch (Exception ex)
            {
                return Redirect("ReportByVehicle");
            }
        }
        public string GenerateReportByVehicleNew(DataTable dt, string grandTotal, string selectstation, string selectLot, string selectVehicle, string FDate, string TDate, string TotalCash, string TotalEPay, string TotalIn, string TotalOut, string TotalFOC, string TotalClamps)
        {
            Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//15012021

            string filename, AttachmentName1 = "";
            string path = System.Web.HttpContext.Current.Server.MapPath("~/PDFReports");
            AttachmentName1 = "Report By Vehicle" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".pdf";
            filename = path + "/" + AttachmentName1;

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(PageSize.A4);
            rec.BackgroundColor = new BaseColor(System.Drawing.Color.Olive);
            Document doc = new Document(rec, 88f, 88f, 10f, 50f);
            var output = new FileStream(filename, FileMode.Create);
            doc.SetPageSize(iTextSharp.text.PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, output);

            writer.PageEvent = new Footer();
            doc.Open();

            BaseColor FontColour = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#353535"));
            BaseColor FontColourBlack = BaseColor.BLACK;
            iTextSharp.text.Font calibri8 = FontFactory.GetFont("Calibri", 10f, FontColour);
            iTextSharp.text.Font _bf_headingaddr = FontFactory.GetFont("Calibri", 8f, FontColour);
            iTextSharp.text.Font _bf_headingaddrbold = FontFactory.GetFont("Calibri", 8f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_heading1 = FontFactory.GetFont("Calibri", 10f, FontColour);
            iTextSharp.text.Font _bf_headingbold = FontFactory.GetFont("Calibri", 10f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingTitle = FontFactory.GetFont("Calibri", 12f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingboldTable = FontFactory.GetFont("Calibri", 10f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingboldTableNrml = FontFactory.GetFont("Calibri", 9f, FontColour);

            Phrase phrase = null;
            PdfPCell cell = null;
            PdfPTable table = null;

            #region Logo 
            //Header Table start
            table = new PdfPTable(2);
            table.TotalWidth = 550f;
            table.LockedWidth = true;
            table.SetWidths(new float[] { 0.3f, 0.7f });
            table.DefaultCell.Border = Rectangle.NO_BORDER;

            //Company Logo           
            // cell = ImageCell("~/assets/images/Logo4.png", 70f, PdfPCell.ALIGN_CENTER);
            //cell = ImageCell("~/Images/" + companydata.CompanyLogo, 20f, PdfPCell.ALIGN_CENTER);
            if (companydata.CompanyLogo != null && companydata.CompanyLogo != "")
            {
                cell = ImageCell("~/Images/" + companydata.CompanyLogo, 20f, PdfPCell.ALIGN_CENTER);
            }
            else
            {
                cell = ImageCell("~/Images/default-logo.png", 20f, PdfPCell.ALIGN_CENTER);
            }
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            table.AddCell(cell);

            PdfPTable innertbl = new PdfPTable(1);
            innertbl.SetWidths(new float[] { 1.0f });
            PdfPCell innercell1 = null;
            phrase = new Phrase();
            //phrase.Add(new Chunk("                                                        Unit #3A, Plot No:847, Pacific Towers,", _bf_headingaddr));
            phrase.Add(new Chunk("                                                              " + companydata.Address1, _bf_headingaddr));
            innercell1 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell1.Border = 0;
            innercell1.Padding = 0;

            innertbl.SpacingBefore = 0f;
            innertbl.SpacingAfter = 2f;
            innertbl.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl.AddCell(innercell1);

            cell = new PdfPCell();
            cell.Border = 0;
            cell.Padding = 0;
            cell.AddElement(innertbl);

            PdfPTable innertbl2 = new PdfPTable(1);
            innertbl2.SetWidths(new float[] { 1.0f });
            PdfPCell innercell2 = null;
            phrase = new Phrase();
            //phrase.Add(new Chunk("                                                        Madhapur, Hyderabad-500081, TS", _bf_headingaddr));
            phrase.Add(new Chunk("                                                              " + companydata.Address2, _bf_headingaddr));
            innercell2 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell2.Border = 0;
            innercell2.Padding = 0;
            innertbl2.SpacingBefore = 0f;
            innertbl2.SpacingAfter = 2f;
            innertbl2.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl2.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl2.AddCell(innercell2);
            cell.AddElement(innertbl2);

            PdfPTable innertbl3 = new PdfPTable(1);
            innertbl3.SetWidths(new float[] { 1.0f });
            PdfPCell innercellTwo = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              GSTIN: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.GSTNumber, _bf_headingaddr));
            //phrase.Add(new Chunk("36AABCT3518Q1ZX", _bf_headingaddr));
            innercellTwo = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercellTwo.Border = 0;
            innercellTwo.Padding = 0;
            innertbl3.SpacingBefore = 0f;
            innertbl3.SpacingAfter = 2f;
            innertbl3.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl3.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl3.AddCell(innercellTwo);
            cell.AddElement(innertbl3);


            PdfPTable innertbl4 = new PdfPTable(1);
            innertbl4.SetWidths(new float[] { 1.0f });
            PdfPCell innercell5 = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              Ph: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.ContactNumber, _bf_headingaddr));
            //phrase.Add(new Chunk("+91 8143143143", _bf_headingaddr));
            innercell5 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell5.Border = 0;
            innercell5.Padding = 0;
            innertbl4.SpacingBefore = 0f;
            innertbl4.SpacingAfter = 2f;
            innertbl4.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl4.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl4.AddCell(innercell5);
            cell.AddElement(innertbl4);
            //table.AddCell(cell);

            PdfPTable innertbl5 = new PdfPTable(1);
            innertbl5.SetWidths(new float[] { 1.0f });
            PdfPCell innercell6 = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              Email: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.SupportEmailID, _bf_headingaddr));
            //phrase.Add(new Chunk("support@hmrl.com", _bf_headingaddr));
            innercell6 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell6.Border = 0;
            innercell6.Padding = 0;
            innertbl5.SpacingBefore = 0f;
            innertbl5.SpacingAfter = 0f;
            innertbl5.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl5.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl5.AddCell(innercell6);
            cell.AddElement(innertbl5);
            table.AddCell(cell);

            doc.Add(table);

            //Header Table end
            #endregion

            #region Title
            //Title table start
            PdfPTable tableTitle = new PdfPTable(1);
            tableTitle.DefaultCell.FixedHeight = 150f;
            tableTitle.TotalWidth = 550f;
            tableTitle.LockedWidth = true;
            tableTitle.SetWidths(new float[] { 1f });
            tableTitle.SpacingBefore = 10f;
            tableTitle.SpacingAfter = 3f;

            phrase = new Phrase();
            phrase.Add(new Chunk("REPORT BY VEHICLE", _bf_headingTitle));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
            cell.Border = 0;
            tableTitle.AddCell(cell);
            doc.Add(tableTitle);
            //Title table end
            #endregion

            #region Filter            
            PdfPTable tablefilter = new PdfPTable(1);
            tablefilter.TotalWidth = 550f;
            tablefilter.LockedWidth = true;
            tablefilter.SetWidths(new float[] { 1.0f });
            tablefilter.SpacingBefore = 3f;
            tablefilter.SpacingAfter = 3f;


            phrase = new Phrase();
            phrase.Add(new Chunk("Filtered By      :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(selectstation + ", " + selectLot + ", " + selectVehicle, _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tablefilter.AddCell(cell);

            doc.Add(tablefilter);
            #endregion

            #region time
            PdfPTable tabletime = new PdfPTable(1);
            tabletime.TotalWidth = 550f;
            tabletime.LockedWidth = true;
            tabletime.SetWidths(new float[] { 1.0f });
            tabletime.SpacingBefore = 3f;
            tabletime.SpacingAfter = 3f;

            phrase = new Phrase();
            phrase.Add(new Chunk(" Report From   :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(FDate + " to " + TDate, _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            cell.Padding = 0;
            tabletime.AddCell(cell);
            doc.Add(tabletime);

            PdfPTable tabletime1 = new PdfPTable(1);
            tabletime1.TotalWidth = 550f;
            tabletime1.LockedWidth = true;
            tabletime1.SetWidths(new float[] { 1.0f });
            tabletime1.SpacingBefore = 3f;
            tabletime1.SpacingAfter = 5f;
            phrase = new Phrase();
            phrase.Add(new Chunk("Generated On :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(DateTime.Now.ToString("dd/MM/yyyy HH:MM tt"), _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tabletime1.AddCell(cell);

            doc.Add(tabletime1);
            #endregion

            #region Data
            PdfPTable table2 = new PdfPTable(dt.Columns.Count - 6);
            table2.SetWidths(new float[] { 0.12f, 0.2f, 0.11f, 0.09f, 0.09f, 0.09f, 0.12f, 0.1f, 0.1f, 0.1f });
            table2.TotalWidth = doc.PageSize.Width - 40f;
            table2.LockedWidth = true;
            table2.SpacingBefore = 10f;
            //table2.SpacingAfter = 3f;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string cellText = Server.HtmlDecode(dt.Columns[i].ColumnName);
                if (cellText != "TotalCash" && cellText != "TotalEPay" && cellText != "TotalIn" && cellText != "TotalOut" && cellText != "TotalFOC" && cellText != "TotalClamps")
                {
                    PdfPCell cell2 = new PdfPCell();
                    if (cellText == "Cash" || cellText == "EPay" || cellText == "Amount")
                    {
                        cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                        cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                        cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    }
                    else
                    {
                        if (cellText == "VehicleType")
                        {
                            cell2.Phrase = new Phrase("Vehicle Type", _bf_headingboldTable);
                        }
                        else if (cellText == "ParkingLot")
                        {
                            cell2.Phrase = new Phrase("Parking Lot", _bf_headingboldTable);
                            // cell2.Phrase = new Phrase("Lot Code", _bf_headingboldTable);
                        }
                        else
                        {
                            cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                        }
                        cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                        cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    }
                    table2.AddCell(cell2);
                }
            }

            //writing table Data  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string cellText = Server.HtmlDecode(dt.Columns[j].ColumnName);
                    if (cellText != "TotalCash" && cellText != "TotalEPay" && cellText != "TotalIn" && cellText != "TotalOut" && cellText != "TotalFOC" && cellText != "TotalClamps")
                    {
                        PdfPCell cell2 = new PdfPCell();
                        if (cellText == "Cash" || cellText == "EPay" || cellText == "Amount")
                        {
                            cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                            cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell2.VerticalAlignment = Element.ALIGN_RIGHT;
                        }
                        else
                        {
                            cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                            cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                            //cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                        }
                        table2.AddCell(cell2);
                    }
                }
            }
            //GRAND TOTAL TABLE

            //PdfPTable grandTable = new PdfPTable(dt.Columns.Count - 2);
            //grandTable.TotalWidth = doc.PageSize.Width - 40f;
            //grandTable.LockedWidth = true;
            //PdfPCell cell3 = new PdfPCell();
            //string totalstring = string.Concat("Total         " + TotalCash + "       " + TotalEPay + "      " + grandTotal);
            //cell3.Phrase = new Phrase(totalstring, _bf_headingboldTable);
            //cell3.Colspan = dt.Columns.Count;
            //cell3.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cell3.BorderWidthTop = 0;
            //grandTable.AddCell(cell3);

            PdfPTable grandTable = new PdfPTable(dt.Columns.Count - 6);
            grandTable.SetWidths(new float[] { 0.12f, 0.2f, 0.11f, 0.09f, 0.09f, 0.09f, 0.12f, 0.1f, 0.1f, 0.1f });
            grandTable.TotalWidth = doc.PageSize.Width - 40f;
            grandTable.LockedWidth = true;

            PdfPCell cellt = new PdfPCell(new Phrase(" ", _bf_headingboldTable));
            cellt.Colspan = 2;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cellt = new PdfPCell();
            cellt.Phrase = new Phrase("Total ", _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
            cellt.BorderWidthLeft = 0;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalIn, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalOut, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalFOC, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalClamps, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalCash, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalEPay, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(grandTotal, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthTop = 0;
            cellt.BorderWidthRight = 1;
            grandTable.AddCell(cellt);

            //GRNAD TOTAL TABLE

            ////GST Table
            //PdfPTable gstTable = new PdfPTable(dt.Columns.Count - 6);
            //gstTable.SetWidths(new float[] { 0.12f, 0.2f, 0.11f, 0.09f, 0.09f, 0.09f, 0.12f, 0.1f, 0.1f, 0.1f });
            //gstTable.TotalWidth = doc.PageSize.Width - 40f;
            //gstTable.LockedWidth = true;

            //PdfPCell cellGST = new PdfPCell(new Phrase(" ", _bf_headingboldTable));
            //cellGST.BorderWidthRight = 0;
            //cellGST.BorderWidthTop = 0;
            //cellGST.Colspan = 8;
            //gstTable.AddCell(cellGST);

            //cellGST = new PdfPCell();
            //cellGST.Phrase = new Phrase("GST", _bf_headingboldTable);
            //cellGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cellGST.BorderWidthLeft = 0;
            //cellGST.BorderWidthRight = 0;
            //cellGST.BorderWidthTop = 0;
            //gstTable.AddCell(cellGST);

            //cell = new PdfPCell();
            //cellGST.Phrase = new Phrase(GST, _bf_headingboldTable);
            //cellGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cellGST.BorderWidthTop = 0;
            //cellGST.BorderWidthRight = 1;
            //gstTable.AddCell(cellGST);
            ////GST Table


            doc.Add(table2);
            doc.Add(grandTable);
           // doc.Add(gstTable);
            #endregion

            doc.Close();
            byte[] result = ms.ToArray();
            return filename;

        }

        #endregion

        #region PaymentType Report
        public ActionResult ReportByPaymentType()
        {
            ViewBag.Menu = "RevenueReports";
            return View();
        }
        [HttpPost]
        public JsonResult GetReportByPaymentType(SearchFilters paymentFilterData)
        {
            try
            {
                if (paymentFilterData.FromTime != null || paymentFilterData.Duration == "Today")
                {
                    string fDate1 = Convert.ToString(paymentFilterData.FromDate).Split(' ')[0];
                    DateTime fTotalDate = Convert.ToDateTime(fDate1 + " " + paymentFilterData.FromTime + ":00 " + paymentFilterData.FromMeridiem);

                    string tDate1 = Convert.ToString(paymentFilterData.ToDate).Split(' ')[0];
                    DateTime tTotalDate = Convert.ToDateTime(tDate1 + " " + paymentFilterData.ToTime + ":00 " + paymentFilterData.ToMeridiem);

                    paymentFilterData.FromDate = fTotalDate;
                    paymentFilterData.ToDate = tTotalDate;
                }

                List<RevenueByPaymentType> revenueByPayment_lst = revenueReportsDAL_obj.GetReportByPaymentType(paymentFilterData);
                Session["PaymentReportData"] = revenueByPayment_lst;
                return new JsonResult()
                {
                    Data = revenueByPayment_lst,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult PaymentTypeReportPDFDownload(string GrandTotal, string SelectedItems)
        {
            try
            {
                var myDetails = JObject.Parse(SelectedItems);
                var selectedStation = myDetails["SelectedStation"];
                var selectedLot = myDetails["SelectedLot"];
                var SelectedVehicle = myDetails["SelectedVehicle"];
                string selectstation = ((Newtonsoft.Json.Linq.JValue)selectedStation).Value.ToString();
                string selectLot = ((Newtonsoft.Json.Linq.JValue)selectedLot).Value.ToString();
                string selectVehicle = ((Newtonsoft.Json.Linq.JValue)SelectedVehicle).Value.ToString();

                var fromDate = myDetails["FromDate"];
                var toDate = myDetails["ToDate"];
                var duration = myDetails["Duration"];
                string selectfromdate = ((Newtonsoft.Json.Linq.JValue)fromDate).Value.ToString();
                string selecttodate = ((Newtonsoft.Json.Linq.JValue)toDate).Value.ToString();
                string selectduration = ((Newtonsoft.Json.Linq.JValue)duration).Value.ToString();

                string FDate = "";
                string TDate = "";

                if (selectduration == "" || selectduration == null || selectduration == "0")
                {
                    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                }
                else
                {
                    FDate = DateTime.Parse(selectfromdate).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                }

                //string FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                //string TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");

                List<RevenueByPaymentType> result = (List<RevenueByPaymentType>)Session["PaymentReportData"];
                DataTable dt = ToDataTable(result);

                string OperatorInTotal = Convert.ToString(dt.Rows[0]["OperatorInTotal"]);
                string AppInTotal = Convert.ToString(dt.Rows[0]["AppInTotal"]);
                string CallInTotal = Convert.ToString(dt.Rows[0]["CallInTotal"]);

                string filename = GenerateReportByPaymentType(dt, GrandTotal, selectstation, selectLot, selectVehicle, FDate, TDate, OperatorInTotal, AppInTotal, CallInTotal);
                FileStream sourceFile = null;

                if (filename != "")
                {
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("content-disposition", "attachment; filename=\"" + Path.GetFileName(filename) + "\"");
                    sourceFile = new FileStream(filename, FileMode.Open);
                    long FileSize;
                    FileSize = sourceFile.Length;
                    byte[] getContent = new byte[(int)FileSize];
                    sourceFile.Read(getContent, 0, (int)sourceFile.Length);
                    sourceFile.Close();
                    Response.Clear();
                    Response.BinaryWrite(getContent);
                    Response.End();
                }
                return Redirect("ReportByPaymentType");
            }
            catch (Exception ex)
            {
                return Redirect("ReportByPaymentType");
            }
        }
        public ActionResult PaymentTypeReportExcelDownload(string GrandTotal, string SelectedItems)
        {
            try
            {
                Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//18012021

                var myDetails = JObject.Parse(SelectedItems);
                var fromDate = myDetails["FromDate"];
                var toDate = myDetails["ToDate"];
                var duration = myDetails["Duration"];
                string selectfromdate = ((Newtonsoft.Json.Linq.JValue)fromDate).Value.ToString();
                string selecttodate = ((Newtonsoft.Json.Linq.JValue)toDate).Value.ToString();
                string selectduration = ((Newtonsoft.Json.Linq.JValue)duration).Value.ToString();

                string FDate = "";
                string TDate = "";

                if (selectduration == "" || selectduration == null || selectduration == "0")
                {
                    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                }
                else
                {
                    FDate = DateTime.Parse(selectfromdate).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                }

                //string FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                //string TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");


                List<RevenueByPaymentType> result = (List<RevenueByPaymentType>)Session["PaymentReportData"];
                DataTable dt = ToDataTable(result);

                string OperatorInTotal = Convert.ToString(dt.Rows[0]["OperatorInTotal"]);
                string AppInTotal = Convert.ToString(dt.Rows[0]["AppInTotal"]);
                string CallInTotal = Convert.ToString(dt.Rows[0]["CallInTotal"]);

                string path = System.Web.HttpContext.Current.Server.MapPath("~/ExcelReports");
                string filename = "RevenueByPaymentTypeReport_" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".xlsx";
                string file_path = path + "/" + filename;

                if (dt.Rows.Count > 0)
                {
                    #region New Code
                    using (XLWorkbook wb = new XLWorkbook())
                    {

                        wb.Worksheets.Add("RevenueReportByPaymentType", 0);
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Value = companydata.AccountName;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.FontSize = 12;
                        wb.Worksheets.Worksheet(0).Cell(1, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(2, 1).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(2, 20).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(3, 1).Value = "Revenue By Payment Type Report";
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(3, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 1).Value = "From : " + FDate + " To : " + TDate;
                        wb.Worksheets.Worksheet(0).Cell(4, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 5).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 6).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 7).Value = "Cur.Date : " + DateTime.Now.ToString("dd/MM/yyy").Split(' ')[0];
                        wb.Worksheets.Worksheet(0).Cell(4, 7).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(6, 1).Value = "Payment Type";
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.FontSize = 11;

                        //wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "In";
                        //wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.FontSize = 11;

                        //wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Out";
                        //wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.FontSize = 11;

                        //wb.Worksheets.Worksheet(0).Cell(6, 4).Value = "FOC";
                        //wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "OperatorIn Amount";
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "AppIn Amount";
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 4).Value = "CallIn Amount";
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 5).Value = "Amount";
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        int row = 7;
                        int column_num = 0;


                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Value = Convert.ToString(dt.Rows[i]["PaymentType"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Style.Font.FontSize = 11;

                            //wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Value = Convert.ToString(dt.Rows[i]["In"]);
                            //wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Style.Font.FontSize = 11;

                            //wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Value = Convert.ToString(dt.Rows[i]["Out"]);
                            //wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Font.FontSize = 11;

                            //wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Value = Convert.ToString(dt.Rows[i]["FOC"]);
                            //wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Value = Convert.ToString(dt.Rows[i]["OperatorIn"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Value = Convert.ToString(dt.Rows[i]["AppIn"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Value = Convert.ToString(dt.Rows[i]["CallIn"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Value = Convert.ToString(dt.Rows[i]["Amount"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            row = row + 1;
                        }

                        //row = row + 1;
                        wb.Worksheets.Worksheet(0).Cell(row, 1).Value = "Total : ";
                        wb.Worksheets.Worksheet(0).Cell(row, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 1).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 2).Value = OperatorInTotal;
                        wb.Worksheets.Worksheet(0).Cell(row, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 2).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 3).Value = AppInTotal;
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 4).Value = CallInTotal;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 5).Value = GrandTotal;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.FontSize = 11;

                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 4).Value = "GST";
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 4).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 4).Style.Font.FontSize = 11;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 5).Value = GST;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 5).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 5).Style.Font.FontSize = 11;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;


                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                        using (MemoryStream MyMemoryStream = new MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                    #endregion

                }
                return Redirect("ReportByPaymentType");
            }
            catch (Exception ex)
            {
                return Redirect("ReportByPaymentType");
            }
        }
        public string GenerateReportByPaymentType(DataTable dt, string grandTotal, string selectstation, string selectLot, string selectVehicle, string FDate, string TDate, string OperatorInTotal, string AppInTotal, string CallInTotal)
        {
            Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//15012021

            string filename, AttachmentName1 = "";
            string path = System.Web.HttpContext.Current.Server.MapPath("~/PDFReports");
            AttachmentName1 = "Report By PaymentType" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".pdf";
            filename = path + "/" + AttachmentName1;

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(PageSize.A4);
            rec.BackgroundColor = new BaseColor(System.Drawing.Color.Olive);
            Document doc = new Document(rec, 88f, 88f, 10f, 50f);
            var output = new FileStream(filename, FileMode.Create);
            doc.SetPageSize(iTextSharp.text.PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, output);
            writer.PageEvent = new Footer();
            doc.Open();

            BaseColor FontColour = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#353535"));
            BaseColor FontColourBlack = BaseColor.BLACK;
            iTextSharp.text.Font calibri8 = FontFactory.GetFont("Calibri", 10f, FontColour);
            iTextSharp.text.Font _bf_headingaddr = FontFactory.GetFont("Calibri", 8f, FontColour);
            iTextSharp.text.Font _bf_headingaddrbold = FontFactory.GetFont("Calibri", 8f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_heading1 = FontFactory.GetFont("Calibri", 10f, FontColour);
            iTextSharp.text.Font _bf_headingbold = FontFactory.GetFont("Calibri", 10f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingTitle = FontFactory.GetFont("Calibri", 12f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingboldTable = FontFactory.GetFont("Calibri", 10f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingboldTableNrml = FontFactory.GetFont("Calibri", 9f, FontColour);

            Phrase phrase = null;
            PdfPCell cell = null;
            PdfPTable table = null;

            #region Logo 
            //Header Table start
            table = new PdfPTable(2);
            table.TotalWidth = 550f;
            table.LockedWidth = true;
            table.SetWidths(new float[] { 0.3f, 0.7f });
            table.DefaultCell.Border = Rectangle.NO_BORDER;

            //Company Logo           
            // cell = ImageCell("~/assets/images/Logo4.png", 70f, PdfPCell.ALIGN_CENTER);
            //cell = ImageCell("~/Images/" + companydata.CompanyLogo, 20f, PdfPCell.ALIGN_CENTER);
            if (companydata.CompanyLogo != null && companydata.CompanyLogo != "")
            {
                cell = ImageCell("~/Images/" + companydata.CompanyLogo, 20f, PdfPCell.ALIGN_CENTER);
            }
            else
            {
                cell = ImageCell("~/Images/default-logo.png", 20f, PdfPCell.ALIGN_CENTER);
            }
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            table.AddCell(cell);

            PdfPTable innertbl = new PdfPTable(1);
            innertbl.SetWidths(new float[] { 1.0f });
            PdfPCell innercell1 = null;
            phrase = new Phrase();
            //phrase.Add(new Chunk("                                                        Unit #3A, Plot No:847, Pacific Towers,", _bf_headingaddr));
            phrase.Add(new Chunk("                                                              " + companydata.Address1, _bf_headingaddr));
            innercell1 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell1.Border = 0;
            innercell1.Padding = 0;

            innertbl.SpacingBefore = 0f;
            innertbl.SpacingAfter = 2f;
            innertbl.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl.AddCell(innercell1);

            cell = new PdfPCell();
            cell.Border = 0;
            cell.Padding = 0;
            cell.AddElement(innertbl);

            PdfPTable innertbl2 = new PdfPTable(1);
            innertbl2.SetWidths(new float[] { 1.0f });
            PdfPCell innercell2 = null;
            phrase = new Phrase();
            //phrase.Add(new Chunk("                                                        Madhapur, Hyderabad-500081, TS", _bf_headingaddr));
            phrase.Add(new Chunk("                                                              " + companydata.Address2, _bf_headingaddr));
            innercell2 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell2.Border = 0;
            innercell2.Padding = 0;
            innertbl2.SpacingBefore = 0f;
            innertbl2.SpacingAfter = 2f;
            innertbl2.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl2.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl2.AddCell(innercell2);
            cell.AddElement(innertbl2);

            PdfPTable innertbl3 = new PdfPTable(1);
            innertbl3.SetWidths(new float[] { 1.0f });
            PdfPCell innercellTwo = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              GSTIN: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.GSTNumber, _bf_headingaddr));
            //phrase.Add(new Chunk("36AABCT3518Q1ZX", _bf_headingaddr));
            innercellTwo = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercellTwo.Border = 0;
            innercellTwo.Padding = 0;
            innertbl3.SpacingBefore = 0f;
            innertbl3.SpacingAfter = 2f;
            innertbl3.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl3.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl3.AddCell(innercellTwo);
            cell.AddElement(innertbl3);


            PdfPTable innertbl4 = new PdfPTable(1);
            innertbl4.SetWidths(new float[] { 1.0f });
            PdfPCell innercell5 = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              Ph: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.ContactNumber, _bf_headingaddr));
            //phrase.Add(new Chunk("+91 8143143143", _bf_headingaddr));
            innercell5 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell5.Border = 0;
            innercell5.Padding = 0;
            innertbl4.SpacingBefore = 0f;
            innertbl4.SpacingAfter = 2f;
            innertbl4.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl4.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl4.AddCell(innercell5);
            cell.AddElement(innertbl4);
            //table.AddCell(cell);

            PdfPTable innertbl5 = new PdfPTable(1);
            innertbl5.SetWidths(new float[] { 1.0f });
            PdfPCell innercell6 = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              Email: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.SupportEmailID, _bf_headingaddr));
            //phrase.Add(new Chunk("support@hmrl.com", _bf_headingaddr));
            innercell6 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell6.Border = 0;
            innercell6.Padding = 0;
            innertbl5.SpacingBefore = 0f;
            innertbl5.SpacingAfter = 0f;
            innertbl5.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl5.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl5.AddCell(innercell6);
            cell.AddElement(innertbl5);
            table.AddCell(cell);

            doc.Add(table);

            //Header Table end
            #endregion

            #region Title
            //Title table start
            PdfPTable tableTitle = new PdfPTable(1);
            tableTitle.DefaultCell.FixedHeight = 150f;
            tableTitle.TotalWidth = 550f;
            tableTitle.LockedWidth = true;
            tableTitle.SetWidths(new float[] { 1f });
            tableTitle.SpacingBefore = 10f;
            tableTitle.SpacingAfter = 3f;

            phrase = new Phrase();
            phrase.Add(new Chunk("REPORT BY PAYMENT TYPE", _bf_headingTitle));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
            cell.Border = 0;
            tableTitle.AddCell(cell);
            doc.Add(tableTitle);
            //Title table end
            #endregion

            #region Filter            
            PdfPTable tablefilter = new PdfPTable(1);
            tablefilter.TotalWidth = 550f;
            tablefilter.LockedWidth = true;
            tablefilter.SetWidths(new float[] { 1.0f });
            tablefilter.SpacingBefore = 3f;
            tablefilter.SpacingAfter = 3f;


            phrase = new Phrase();
            phrase.Add(new Chunk("Filtered By      :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(selectstation + ", " + selectLot + ", " + selectVehicle, _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tablefilter.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk(selectstation, _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.BorderColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#dbdbdb"));
            //tablefilter.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk(selectLot, _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.BorderColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#dbdbdb"));
            //tablefilter.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk(selectVehicle, _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.BorderColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#dbdbdb"));
            //tablefilter.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk("", _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.Border = 0;
            //tablefilter.AddCell(cell);

            doc.Add(tablefilter);
            #endregion

            #region time
            PdfPTable tabletime = new PdfPTable(1);
            tabletime.TotalWidth = 550f;
            tabletime.LockedWidth = true;
            tabletime.SetWidths(new float[] { 1.0f });
            tabletime.SpacingBefore = 3f;
            tabletime.SpacingAfter = 3f;

            phrase = new Phrase();
            phrase.Add(new Chunk("Report From   :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(FDate + " to " + TDate, _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tabletime.AddCell(cell);
            doc.Add(tabletime);
            //phrase = new Phrase();
            //phrase.Add(new Chunk(FDate + " to " + TDate, _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            //cell.Border = 0;
            //tabletime.AddCell(cell);

            PdfPTable tabletime1 = new PdfPTable(1);
            tabletime1.TotalWidth = 550f;
            tabletime1.LockedWidth = true;
            tabletime1.SetWidths(new float[] { 1.0f });
            tabletime1.SpacingBefore = 3f;
            tabletime1.SpacingAfter = 5f;
            phrase = new Phrase();
            phrase.Add(new Chunk("Generated On :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(DateTime.Now.ToString("dd/MM/yyyy HH:MM tt"), _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tabletime1.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk(DateTime.Now.ToString("dd/MM/yyyy HH:MM tt"), _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            //cell.Border = 0;
            //tabletime.AddCell(cell);

            doc.Add(tabletime1);
            #endregion

            #region Data
            PdfPTable table2 = new PdfPTable(dt.Columns.Count - 3);
            table2.TotalWidth = doc.PageSize.Width - 40f;
            table2.LockedWidth = true;
            table2.SpacingBefore = 10f;
            // table2.SpacingAfter = 3f;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string cellText = Server.HtmlDecode(dt.Columns[i].ColumnName);
                if (cellText != "OperatorInTotal" && cellText != "AppInTotal" && cellText != "CallInTotal")
                {
                    PdfPCell cell2 = new PdfPCell();
                    if (cellText == "OperatorIn" || cellText == "AppIn" || cellText == "CallIn" || cellText == "Amount")
                    {
                        if (cellText == "OperatorIn")
                        {
                            cell2.Phrase = new Phrase("OperatorIn Amount", _bf_headingboldTable);
                        }
                        else if (cellText == "AppIn")
                        {
                            cell2.Phrase = new Phrase("AppIn Amount", _bf_headingboldTable);
                        }
                        else if (cellText == "CallIn")
                        {
                            cell2.Phrase = new Phrase("CallIn Amount", _bf_headingboldTable);
                        }
                        else
                        {
                            cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                        }
                        cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                        cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    }
                    else
                    {
                        if (cellText == "PaymentType")
                        {
                            cell2.Phrase = new Phrase("Payment Type", _bf_headingboldTable);
                        }
                        else
                        {
                            cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                        }
                        cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                        cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    }
                    table2.AddCell(cell2);
                }
            }

            //writing table Data  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string cellText = Server.HtmlDecode(dt.Columns[j].ColumnName);
                    if (cellText != "OperatorInTotal" && cellText != "AppInTotal" && cellText != "CallInTotal")
                    {
                        PdfPCell cell2 = new PdfPCell();
                        if (cellText == "OperatorIn" || cellText == "AppIn" || cellText == "CallIn" || cellText == "Amount")
                        {
                            cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                            cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell2.VerticalAlignment = Element.ALIGN_RIGHT;
                        }
                        else
                        {
                            cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                            cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                        }
                        table2.AddCell(cell2);
                    }
                }
            }
            //GRAND TOTAL TABLE
            //PdfPTable grandTable = new PdfPTable(dt.Columns.Count - 3);
            //grandTable.TotalWidth = doc.PageSize.Width - 40f;
            //grandTable.LockedWidth = true;
            //PdfPCell cell3 = new PdfPCell();
            //string totalstring = string.Concat("Total                                             " + OperatorInTotal + "                                 " + AppInTotal + "                                 " + CallInTotal + "                          " + grandTotal);
            //cell3.Phrase = new Phrase(totalstring, _bf_headingboldTable);
            //cell3.Colspan = dt.Columns.Count;
            //cell3.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cell3.BorderWidthTop = 0;
            //grandTable.AddCell(cell3);

            PdfPTable grandTable = new PdfPTable(dt.Columns.Count - 3);
            //grandTable.SetWidths(new float[] { 0.12f, 0.2f, 0.11f, 0.09f, 0.09f, 0.09f, 0.12f, 0.1f, 0.1f, 0.1f });
            grandTable.TotalWidth = doc.PageSize.Width - 40f;
            grandTable.LockedWidth = true;

            //PdfPCell cellt = new PdfPCell(new Phrase(" ", _bf_headingboldTable));
            //cellt.Colspan = 3;
            //cellt.BorderWidthRight = 0;
            //cellt.BorderWidthTop = 0;
            //grandTable.AddCell(cellt);

            PdfPCell cellt = new PdfPCell();
            cellt.Phrase = new Phrase("Total ", _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cellt.BorderWidthLeft = 0;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(OperatorInTotal, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            cellt.BorderWidthLeft = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(AppInTotal, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            cellt.BorderWidthLeft = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(CallInTotal, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            cellt.BorderWidthLeft = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(grandTotal, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthTop = 0;
            cellt.BorderWidthRight = 1;
            cellt.BorderWidthLeft = 0;
            grandTable.AddCell(cellt);

            //GRNAD TOTAL TABLE

            ////GST Table
            //PdfPTable gstTable = new PdfPTable(dt.Columns.Count - 3);
            //gstTable.TotalWidth = doc.PageSize.Width - 40f;
            //gstTable.LockedWidth = true;

            //PdfPCell cellGST = new PdfPCell(new Phrase(" ", _bf_headingboldTable));
            //cellGST.BorderWidthRight = 0;
            //cellGST.BorderWidthTop = 0;
            //cellGST.Colspan = 3;
            //gstTable.AddCell(cellGST);

            //cellGST = new PdfPCell();
            //cellGST.Phrase = new Phrase("GST", _bf_headingboldTable);
            //cellGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cellGST.BorderWidthLeft = 0;
            //cellGST.BorderWidthRight = 0;
            //cellGST.BorderWidthTop = 0;
            //gstTable.AddCell(cellGST);

            //cell = new PdfPCell();
            //cellGST.Phrase = new Phrase(GST, _bf_headingboldTable);
            //cellGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cellGST.BorderWidthTop = 0;
            //cellGST.BorderWidthRight = 1;
            //gstTable.AddCell(cellGST);
            ////GST Table

            doc.Add(table2);
            doc.Add(grandTable);
            //doc.Add(gstTable);
            #endregion

            doc.Close();
            byte[] result = ms.ToArray();
            return filename;

        }

        #endregion

        #region Time Report
        public ActionResult ReportByTime()
        {
            ViewBag.Menu = "RevenueReports";
            return View();
        }
        [HttpPost]
        public JsonResult GetReportByTime(SearchFilters TimeFilterData)
        {
            try
            {
                if (TimeFilterData.FromTime != null || TimeFilterData.Duration == "Today")
                {
                    string fDate1 = Convert.ToString(TimeFilterData.FromDate).Split(' ')[0];
                    DateTime fTotalDate = Convert.ToDateTime(fDate1 + " " + TimeFilterData.FromTime + ":00 " + TimeFilterData.FromMeridiem);

                    string tDate1 = Convert.ToString(TimeFilterData.ToDate).Split(' ')[0];
                    DateTime tTotalDate = Convert.ToDateTime(tDate1 + " " + TimeFilterData.ToTime + ":00 " + TimeFilterData.ToMeridiem);

                    TimeFilterData.FromDate = fTotalDate;
                    TimeFilterData.ToDate = tTotalDate;
                }

                List<RevenueByTime> revenueByTime_lst = revenueReportsDAL_obj.GetReportByTime(TimeFilterData);
                Session["TimeReportData"] = revenueByTime_lst;
                return new JsonResult()
                {
                    Data = revenueByTime_lst,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult TimeReportPDFDownload(string GrandTotal, string SelectedItems)
        {
            try
            {
                var myDetails = JObject.Parse(SelectedItems);
                var selectedStation = myDetails["SelectedStation"];
                var selectedLot = myDetails["SelectedLot"];
                var SelectedVehicle = myDetails["SelectedVehicle"];
                string selectstation = ((Newtonsoft.Json.Linq.JValue)selectedStation).Value.ToString();
                string selectLot = ((Newtonsoft.Json.Linq.JValue)selectedLot).Value.ToString();
                string selectVehicle = ((Newtonsoft.Json.Linq.JValue)SelectedVehicle).Value.ToString();

                var fromDate = myDetails["FromDate"];
                var toDate = myDetails["ToDate"];
                var duration = myDetails["Duration"];
                string selectfromdate = ((Newtonsoft.Json.Linq.JValue)fromDate).Value.ToString();
                string selecttodate = ((Newtonsoft.Json.Linq.JValue)toDate).Value.ToString();
                string selectduration = ((Newtonsoft.Json.Linq.JValue)duration).Value.ToString();

                string FDate = "";
                string TDate = "";

                if (selectduration == "" || selectduration == null || selectduration == "0")
                {
                    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                }
                else
                {
                    FDate = DateTime.Parse(selectfromdate).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                }

                List<RevenueByTime> result = (List<RevenueByTime>)Session["TimeReportData"];
                DataTable dt = ToDataTable(result);

                string TotalIn = Convert.ToString(dt.Rows[0]["TotalIn"]);
                string TotalOut = Convert.ToString(dt.Rows[0]["TotalOut"]);
                string TotalFOC = Convert.ToString(dt.Rows[0]["TotalFOC"]);

                string TotalCash = Convert.ToString(dt.Rows[0]["TotalCash"]);
                string TotalEPay = Convert.ToString(dt.Rows[0]["TotalEPay"]);

                string filename = GenerateReportByTime(dt, GrandTotal, selectstation, selectLot, selectVehicle, FDate, TDate, TotalCash, TotalEPay, TotalIn, TotalOut, TotalFOC);
                FileStream sourceFile = null;

                if (filename != "")
                {
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("content-disposition", "attachment; filename=\"" + Path.GetFileName(filename) + "\"");
                    sourceFile = new FileStream(filename, FileMode.Open);
                    long FileSize;
                    FileSize = sourceFile.Length;
                    byte[] getContent = new byte[(int)FileSize];
                    sourceFile.Read(getContent, 0, (int)sourceFile.Length);
                    sourceFile.Close();
                    Response.Clear();
                    Response.BinaryWrite(getContent);
                    Response.End();
                }
                return Redirect("ReportByTime");
            }
            catch (Exception ex)
            {
                return Redirect("ReportByTime");
            }
        }
        public string GenerateReportByTime(DataTable dt, string grandTotal, string selectstation, string selectLot, string selectVehicle, string FDate, string TDate, string TotalCash, string TotalEPay, string TotalIn, string TotalOut, string TotalFOC)
        {
            Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//15012021

            string filename, AttachmentName1 = "";
            string path = System.Web.HttpContext.Current.Server.MapPath("~/PDFReports");
            AttachmentName1 = "Report By Time" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".pdf";
            filename = path + "/" + AttachmentName1;

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(PageSize.A4);
            rec.BackgroundColor = new BaseColor(System.Drawing.Color.Olive);
            Document doc = new Document(rec, 88f, 88f, 10f, 50f);
            var output = new FileStream(filename, FileMode.Create);
            doc.SetPageSize(iTextSharp.text.PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, output);
            writer.PageEvent = new Footer();
            doc.Open();

            writer.PageEvent = new Footer();

            BaseColor FontColour = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#353535"));
            BaseColor FontColourBlack = BaseColor.BLACK;
            iTextSharp.text.Font calibri8 = FontFactory.GetFont("Calibri", 10f, FontColour);
            iTextSharp.text.Font _bf_headingaddr = FontFactory.GetFont("Calibri", 8f, FontColour);
            iTextSharp.text.Font _bf_headingaddrbold = FontFactory.GetFont("Calibri", 8f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_heading1 = FontFactory.GetFont("Calibri", 10f, FontColour);
            iTextSharp.text.Font _bf_headingbold = FontFactory.GetFont("Calibri", 10f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingTitle = FontFactory.GetFont("Calibri", 12f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingboldTable = FontFactory.GetFont("Calibri", 10f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingboldTableNrml = FontFactory.GetFont("Calibri", 9f, FontColour);


            Phrase phrase = null;
            PdfPCell cell = null;
            PdfPTable table = null;

            #region Logo 
            //Header Table start
            table = new PdfPTable(2);
            table.TotalWidth = 550f;
            table.LockedWidth = true;
            table.SetWidths(new float[] { 0.3f, 0.7f });
            table.DefaultCell.Border = Rectangle.NO_BORDER;

            //Company Logo           
            // cell = ImageCell("~/assets/images/Logo4.png", 70f, PdfPCell.ALIGN_CENTER);
            //cell = ImageCell("~/Images/" + companydata.CompanyLogo, 20f, PdfPCell.ALIGN_CENTER);
            if (companydata.CompanyLogo != null && companydata.CompanyLogo != "")
            {
                cell = ImageCell("~/Images/" + companydata.CompanyLogo, 20f, PdfPCell.ALIGN_CENTER);
            }
            else
            {
                cell = ImageCell("~/Images/default-logo.png", 20f, PdfPCell.ALIGN_CENTER);
            }
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            table.AddCell(cell);

            PdfPTable innertbl = new PdfPTable(1);
            innertbl.SetWidths(new float[] { 1.0f });
            PdfPCell innercell1 = null;
            phrase = new Phrase();
            //phrase.Add(new Chunk("                                                        Unit #3A, Plot No:847, Pacific Towers,", _bf_headingaddr));
            phrase.Add(new Chunk("                                                              " + companydata.Address1, _bf_headingaddr));
            innercell1 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell1.Border = 0;
            innercell1.Padding = 0;

            innertbl.SpacingBefore = 0f;
            innertbl.SpacingAfter = 2f;
            innertbl.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl.AddCell(innercell1);

            cell = new PdfPCell();
            cell.Border = 0;
            cell.Padding = 0;
            cell.AddElement(innertbl);

            PdfPTable innertbl2 = new PdfPTable(1);
            innertbl2.SetWidths(new float[] { 1.0f });
            PdfPCell innercell2 = null;
            phrase = new Phrase();
            //phrase.Add(new Chunk("                                                        Madhapur, Hyderabad-500081, TS", _bf_headingaddr));
            phrase.Add(new Chunk("                                                              " + companydata.Address2, _bf_headingaddr));
            innercell2 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell2.Border = 0;
            innercell2.Padding = 0;
            innertbl2.SpacingBefore = 0f;
            innertbl2.SpacingAfter = 2f;
            innertbl2.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl2.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl2.AddCell(innercell2);
            cell.AddElement(innertbl2);

            PdfPTable innertbl3 = new PdfPTable(1);
            innertbl3.SetWidths(new float[] { 1.0f });
            PdfPCell innercellTwo = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              GSTIN: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.GSTNumber, _bf_headingaddr));
            //phrase.Add(new Chunk("36AABCT3518Q1ZX", _bf_headingaddr));
            innercellTwo = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercellTwo.Border = 0;
            innercellTwo.Padding = 0;
            innertbl3.SpacingBefore = 0f;
            innertbl3.SpacingAfter = 2f;
            innertbl3.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl3.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl3.AddCell(innercellTwo);
            cell.AddElement(innertbl3);


            PdfPTable innertbl4 = new PdfPTable(1);
            innertbl4.SetWidths(new float[] { 1.0f });
            PdfPCell innercell5 = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              Ph: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.ContactNumber, _bf_headingaddr));
            //phrase.Add(new Chunk("+91 8143143143", _bf_headingaddr));
            innercell5 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell5.Border = 0;
            innercell5.Padding = 0;
            innertbl4.SpacingBefore = 0f;
            innertbl4.SpacingAfter = 2f;
            innertbl4.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl4.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl4.AddCell(innercell5);
            cell.AddElement(innertbl4);
            //table.AddCell(cell);

            PdfPTable innertbl5 = new PdfPTable(1);
            innertbl5.SetWidths(new float[] { 1.0f });
            PdfPCell innercell6 = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              Email: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.SupportEmailID, _bf_headingaddr));
            //phrase.Add(new Chunk("support@hmrl.com", _bf_headingaddr));
            innercell6 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell6.Border = 0;
            innercell6.Padding = 0;
            innertbl5.SpacingBefore = 0f;
            innertbl5.SpacingAfter = 0f;
            innertbl5.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl5.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl5.AddCell(innercell6);
            cell.AddElement(innertbl5);
            table.AddCell(cell);

            doc.Add(table);

            //Header Table end
            #endregion

            #region Title
            //Title table start
            PdfPTable tableTitle = new PdfPTable(1);
            tableTitle.DefaultCell.FixedHeight = 150f;
            tableTitle.TotalWidth = 550f;
            tableTitle.LockedWidth = true;
            tableTitle.SetWidths(new float[] { 1f });
            tableTitle.SpacingBefore = 10f;
            tableTitle.SpacingAfter = 3f;

            phrase = new Phrase();
            phrase.Add(new Chunk("REPORT BY TIME", _bf_headingTitle));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
            cell.Border = 0;
            tableTitle.AddCell(cell);
            doc.Add(tableTitle);
            //Title table end
            #endregion

            #region Filter            
            PdfPTable tablefilter = new PdfPTable(1);
            tablefilter.TotalWidth = 550f;
            tablefilter.LockedWidth = true;
            tablefilter.SetWidths(new float[] { 1.0f });
            tablefilter.SpacingBefore = 3f;
            tablefilter.SpacingAfter = 3f;


            phrase = new Phrase();
            phrase.Add(new Chunk("Filtered By      :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(selectstation + ", " + selectLot + ", " + selectVehicle, _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tablefilter.AddCell(cell);
            doc.Add(tablefilter);
            #endregion

            #region time
            PdfPTable tabletime = new PdfPTable(1);
            tabletime.TotalWidth = 550f;
            tabletime.LockedWidth = true;
            tabletime.SetWidths(new float[] { 1.0f });
            tabletime.SpacingBefore = 3f;
            tabletime.SpacingAfter = 3f;

            phrase = new Phrase();
            phrase.Add(new Chunk("Report From   :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(FDate + " to " + TDate, _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tabletime.AddCell(cell);
            doc.Add(tabletime);

            PdfPTable tabletime1 = new PdfPTable(1);
            tabletime1.TotalWidth = 550f;
            tabletime1.LockedWidth = true;
            tabletime1.SetWidths(new float[] { 1.0f });
            tabletime1.SpacingBefore = 3f;
            tabletime1.SpacingAfter = 5f;
            phrase = new Phrase();
            phrase.Add(new Chunk("Generated On :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(DateTime.Now.ToString("dd/MM/yyyy HH:MM tt"), _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tabletime1.AddCell(cell);
            doc.Add(tabletime1);
            #endregion

            #region Data
            PdfPTable table2 = new PdfPTable(dt.Columns.Count - 5);
            table2.SetWidths(new float[] { 0.25f, 0.2f, 0.11f, 0.07f, 0.07f, 0.07f, 0.12f, 0.12f, 0.12f });
            table2.TotalWidth = doc.PageSize.Width - 40f;
            table2.LockedWidth = true;
            table2.SpacingBefore = 10f;
            //table2.SpacingAfter = 3f;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string cellText = Server.HtmlDecode(dt.Columns[i].ColumnName);
                if (cellText != "TotalCash" && cellText != "TotalEPay" && cellText != "TotalIn" && cellText != "TotalOut" && cellText != "TotalFOC")
                {
                    PdfPCell cell2 = new PdfPCell();
                    if (cellText == "Cash" || cellText == "EPay" || cellText == "Amount")
                    {
                        cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                        cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                        cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    }
                    else
                    {
                        if (cellText == "TimePeriod")
                        {
                            cell2.Phrase = new Phrase("Time Period", _bf_headingboldTable);
                        }
                        else if (cellText == "ParkingLot")
                        {
                            cell2.Phrase = new Phrase("Parking Lot", _bf_headingboldTable);
                            //cell2.Phrase = new Phrase("Lot Code", _bf_headingboldTable);
                        }
                        else
                        {
                            cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                        }
                        cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                        cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    }
                    table2.AddCell(cell2);
                }
            }

            //writing table Data  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string cellText = Server.HtmlDecode(dt.Columns[j].ColumnName);
                    if (cellText != "TotalCash" && cellText != "TotalEPay" && cellText != "TotalIn" && cellText != "TotalOut" && cellText != "TotalFOC")
                    {
                        PdfPCell cell2 = new PdfPCell();
                        if (cellText == "Cash" || cellText == "EPay" || cellText == "Amount")
                        {
                            cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                            cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                        }

                        else
                        {
                            cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                            cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                        }
                        table2.AddCell(cell2);
                    }
                }
            }
            //GRAND TOTAL TABLE
            //PdfPTable grandTable = new PdfPTable(dt.Columns.Count - 2);
            //grandTable.TotalWidth = doc.PageSize.Width - 40f;
            //grandTable.LockedWidth = true;
            //PdfPCell cell3 = new PdfPCell();
            //string totalstring = string.Concat("Total          " + TotalCash + "        " + TotalEPay + "     " + grandTotal);
            //cell3.Phrase = new Phrase(totalstring, _bf_headingboldTable);
            //cell3.Colspan = dt.Columns.Count;
            //cell3.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cell3.BorderWidthTop = 0;
            //grandTable.AddCell(cell3);

            PdfPTable grandTable = new PdfPTable(dt.Columns.Count - 5);
            grandTable.SetWidths(new float[] { 0.25f, 0.2f, 0.11f, 0.07f, 0.07f, 0.07f, 0.12f, 0.12f, 0.12f });
            grandTable.TotalWidth = doc.PageSize.Width - 40f;
            grandTable.LockedWidth = true;

            PdfPCell cellt = new PdfPCell(new Phrase(" ", _bf_headingboldTable));
            cellt.Colspan = 2;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cellt = new PdfPCell();
            cellt.Phrase = new Phrase("Total ", _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
            cellt.BorderWidthLeft = 0;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalIn, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalOut, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalFOC, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalCash, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalEPay, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(grandTotal, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthTop = 0;
            cellt.BorderWidthRight = 1;
            grandTable.AddCell(cellt);

            //GRNAD TOTAL TABLE

            ////GST Table
            //PdfPTable gstTable = new PdfPTable(dt.Columns.Count - 5);
            //gstTable.SetWidths(new float[] { 0.25f, 0.2f, 0.11f, 0.07f, 0.07f, 0.07f, 0.12f, 0.12f, 0.12f });
            //gstTable.TotalWidth = doc.PageSize.Width - 40f;
            //gstTable.LockedWidth = true;

            //PdfPCell cellGST = new PdfPCell(new Phrase(" ", _bf_headingboldTable));
            //cellGST.BorderWidthRight = 0;
            //cellGST.BorderWidthTop = 0;
            //cellGST.Colspan = 7;
            //gstTable.AddCell(cellGST);

            //cellGST = new PdfPCell();
            //cellGST.Phrase = new Phrase("GST", _bf_headingboldTable);
            //cellGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cellGST.BorderWidthLeft = 0;
            //cellGST.BorderWidthRight = 0;
            //cellGST.BorderWidthTop = 0;
            //gstTable.AddCell(cellGST);

            //cell = new PdfPCell();
            //cellGST.Phrase = new Phrase(GST, _bf_headingboldTable);
            //cellGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cellGST.BorderWidthTop = 0;
            //cellGST.BorderWidthRight = 1;
            //gstTable.AddCell(cellGST);
            ////GST Table

            doc.Add(table2);
            doc.Add(grandTable);
           // doc.Add(gstTable);
            #endregion

            doc.Close();
            byte[] result = ms.ToArray();
            return filename;

        }
        public ActionResult TimeReportExcelDownload(string GrandTotal, string SelectedItems)
        {
            try
            {
                Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//18012021

                var myDetails = JObject.Parse(SelectedItems);
                var fromDate = myDetails["FromDate"];
                var toDate = myDetails["ToDate"];
                var duration = myDetails["Duration"];
                string selectfromdate = ((Newtonsoft.Json.Linq.JValue)fromDate).Value.ToString();
                string selecttodate = ((Newtonsoft.Json.Linq.JValue)toDate).Value.ToString();
                string selectduration = ((Newtonsoft.Json.Linq.JValue)duration).Value.ToString();

                string FDate = "";
                string TDate = "";

                if (selectduration == "" || selectduration == null || selectduration == "0")
                {
                    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                }
                else
                {
                    FDate = DateTime.Parse(selectfromdate).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                }

                List<RevenueByTime> result = (List<RevenueByTime>)Session["TimeReportData"];
                DataTable dt = ToDataTable(result);

                string TotalIn = Convert.ToString(dt.Rows[0]["TotalIn"]);
                string TotalOut = Convert.ToString(dt.Rows[0]["TotalOut"]);
                string TotalFOC = Convert.ToString(dt.Rows[0]["TotalFOC"]);

                string TotalCash = Convert.ToString(dt.Rows[0]["TotalCash"]);
                string TotalEPay = Convert.ToString(dt.Rows[0]["TotalEPay"]);

                string path = System.Web.HttpContext.Current.Server.MapPath("~/ExcelReports");
                string filename = "RevenueByTimeReport_" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".xlsx";
                string file_path = path + "/" + filename;

                if (dt.Rows.Count > 0)
                {
                    #region New Code
                    using (XLWorkbook wb = new XLWorkbook())
                    {

                        wb.Worksheets.Add("RevenueReportByTime", 0);
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Value = companydata.AccountName;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.FontSize = 12;
                        wb.Worksheets.Worksheet(0).Cell(1, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(2, 1).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(2, 20).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(3, 1).Value = "Revenue By Time Report";
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(3, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 1).Value = "From : " + FDate + " To : " + TDate;
                        wb.Worksheets.Worksheet(0).Cell(4, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 5).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 6).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 7).Value = "Cur.Date : " + DateTime.Now.ToString("dd/MM/yyy").Split(' ')[0];
                        wb.Worksheets.Worksheet(0).Cell(4, 7).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(6, 1).Value = "Time Period";
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "Station";
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Parking Lot";
                        //wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Lot Code";
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 4).Value = "In";
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 5).Value = "Out";
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 6).Value = "FOC";
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 7).Value = "Cash";
                        wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 8).Value = "EPay";
                        wb.Worksheets.Worksheet(0).Cell(6, 8).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 8).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 9).Value = "Amount";
                        wb.Worksheets.Worksheet(0).Cell(6, 9).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 9).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        int row = 7;
                        int column_num = 0;

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Value = Convert.ToString(dt.Rows[i]["TimePeriod"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Value = Convert.ToString(dt.Rows[i]["Station"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Value = Convert.ToString(dt.Rows[i]["ParkingLot"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Value = Convert.ToString(dt.Rows[i]["In"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Value = Convert.ToString(dt.Rows[i]["Out"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Value = Convert.ToString(dt.Rows[i]["FOC"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Value = Convert.ToString(dt.Rows[i]["Cash"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 8).Value = Convert.ToString(dt.Rows[i]["EPay"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 8).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 9).Value = Convert.ToString(dt.Rows[i]["Amount"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 9).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            row = row + 1;
                        }

                        //row = row + 1;
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Value = "Total ";
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 4).Value = TotalIn;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        wb.Worksheets.Worksheet(0).Cell(row, 5).Value = TotalOut;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        wb.Worksheets.Worksheet(0).Cell(row, 6).Value = TotalFOC;
                        wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        wb.Worksheets.Worksheet(0).Cell(row, 7).Value = TotalCash;
                        wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(row, 8).Value = TotalEPay;
                        wb.Worksheets.Worksheet(0).Cell(row, 8).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 8).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(row, 9).Value = GrandTotal;
                        wb.Worksheets.Worksheet(0).Cell(row, 9).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 9).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 8).Value = "GST";
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 8).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 8).Style.Font.FontSize = 11;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 9).Value = GST;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 9).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 9).Style.Font.FontSize = 11;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;


                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                        using (MemoryStream MyMemoryStream = new MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                    #endregion

                }
                return Redirect("ReportByTime");
            }
            catch (Exception ex)
            {
                return Redirect("ReportByTime");
            }
        }
        #endregion

        #region Passes Report
        public ActionResult ReportByPasses()
        {
            ViewBag.Menu = "RevenueReports";
            return View();
        }
        [HttpPost]
        public JsonResult GetReportByPasses(SearchFilters passFilterData)
        {
            try
            {
                if (passFilterData.FromTime != null || passFilterData.Duration == "Today")
                {
                    string fDate1 = Convert.ToString(passFilterData.FromDate).Split(' ')[0];
                    DateTime fTotalDate = Convert.ToDateTime(fDate1 + " " + passFilterData.FromTime + ":00 " + passFilterData.FromMeridiem);

                    string tDate1 = Convert.ToString(passFilterData.ToDate).Split(' ')[0];
                    DateTime tTotalDate = Convert.ToDateTime(tDate1 + " " + passFilterData.ToTime + ":00 " + passFilterData.ToMeridiem);

                    passFilterData.FromDate = fTotalDate;
                    passFilterData.ToDate = tTotalDate;
                }

                List<RevenueByPasses> revenueByPasses_lst = revenueReportsDAL_obj.GetReportByPasses(passFilterData);
                Session["PassesReportData"] = revenueByPasses_lst;
                return new JsonResult()
                {
                    Data = revenueByPasses_lst,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult PassesReportPDFDownload(string GrandTotal, string SelectedItems)
        {
            try
            {
                var myDetails = JObject.Parse(SelectedItems);
                var selectedStation = myDetails["SelectedStation"];
                var selectedLot = myDetails["SelectedLot"];
                var SelectedVehicle = myDetails["SelectedVehicle"];
                var SelectedChannel = myDetails["SelectedChannel"];
                string selectstation = ((Newtonsoft.Json.Linq.JValue)selectedStation).Value.ToString();
                string selectLot = ((Newtonsoft.Json.Linq.JValue)selectedLot).Value.ToString();
                string selectChannel = ((Newtonsoft.Json.Linq.JValue)SelectedChannel).Value.ToString();
                string selectVehicle = ((Newtonsoft.Json.Linq.JValue)SelectedVehicle).Value.ToString();

                if (selectChannel == null || selectChannel == "")
                {
                    selectChannel = "All Channels";
                }

                var fromDate = myDetails["FromDate"];
                var toDate = myDetails["ToDate"];
                var duration = myDetails["Duration"];
                string selectfromdate = ((Newtonsoft.Json.Linq.JValue)fromDate).Value.ToString();
                string selecttodate = ((Newtonsoft.Json.Linq.JValue)toDate).Value.ToString();
                string selectduration = ((Newtonsoft.Json.Linq.JValue)duration).Value.ToString();

                string FDate = "";
                string TDate = "";

                if (selectduration == "" || selectduration == null || selectduration == "0")
                {
                    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                }
                else
                {
                    FDate = DateTime.Parse(selectfromdate).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                }

                //string FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                //string TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");

                List<RevenueByPasses> result = (List<RevenueByPasses>)Session["PassesReportData"];
                DataTable dt = ToDataTable(result);

                string TotalCount = Convert.ToString(dt.Rows[0]["TotalCount"]);
                string TotalPassWithNFC = Convert.ToString(dt.Rows[0]["TotalPassWithNFC"]);
                string TotalOnlyNFC = Convert.ToString(dt.Rows[0]["TotalOnlyNFC"]);

                string TotalCash = Convert.ToString(dt.Rows[0]["TotalCash"]);
                string TotalEPay = Convert.ToString(dt.Rows[0]["TotalEPay"]);

                string filename = GenerateReportByPasses(dt, GrandTotal, selectstation, selectChannel, selectVehicle, FDate, TDate, TotalCash, TotalEPay, TotalCount, TotalPassWithNFC, TotalOnlyNFC);
                FileStream sourceFile = null;

                if (filename != "")
                {
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("content-disposition", "attachment; filename=\"" + Path.GetFileName(filename) + "\"");
                    sourceFile = new FileStream(filename, FileMode.Open);
                    long FileSize;
                    FileSize = sourceFile.Length;
                    byte[] getContent = new byte[(int)FileSize];
                    sourceFile.Read(getContent, 0, (int)sourceFile.Length);
                    sourceFile.Close();
                    Response.Clear();
                    Response.BinaryWrite(getContent);
                    Response.End();
                }
                return Redirect("ReportByPasses");
            }
            catch (Exception ex)
            {
                return Redirect("ReportByPasses");
            }
        }
        public string GenerateReportByPasses(DataTable dt, string grandTotal, string selectstation, string selectChannel, string selectVehicle, string FDate, string TDate, string TotalCash, string TotalEPay, string TotalCount, string TotalPassWithNFC, string TotalOnlyNFC)
        {
            Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//15012021

            string filename, AttachmentName1 = "";
            string path = System.Web.HttpContext.Current.Server.MapPath("~/PDFReports");
            AttachmentName1 = "Report By Passes" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".pdf";
            filename = path + "/" + AttachmentName1;

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(PageSize.A4);
            rec.BackgroundColor = new BaseColor(System.Drawing.Color.Olive);
            Document doc = new Document(rec, 88f, 88f, 10f, 50f);
            var output = new FileStream(filename, FileMode.Create);
            doc.SetPageSize(iTextSharp.text.PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, output);
            writer.PageEvent = new Footer();
            doc.Open();

            BaseColor FontColour = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#353535"));
            BaseColor FontColourBlack = BaseColor.BLACK;
            iTextSharp.text.Font calibri8 = FontFactory.GetFont("Calibri", 10f, FontColour);
            iTextSharp.text.Font _bf_headingaddr = FontFactory.GetFont("Calibri", 8f, FontColour);
            iTextSharp.text.Font _bf_headingaddrbold = FontFactory.GetFont("Calibri", 8f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_heading1 = FontFactory.GetFont("Calibri", 10f, FontColour);
            iTextSharp.text.Font _bf_headingbold = FontFactory.GetFont("Calibri", 10f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingTitle = FontFactory.GetFont("Calibri", 12f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingboldTable = FontFactory.GetFont("Calibri", 10f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingboldTableNrml = FontFactory.GetFont("Calibri", 9f, FontColour);

            Phrase phrase = null;
            PdfPCell cell = null;
            PdfPTable table = null;

            #region Logo 
            //Header Table start
            table = new PdfPTable(2);
            table.TotalWidth = 550f;
            table.LockedWidth = true;
            table.SetWidths(new float[] { 0.3f, 0.7f });
            table.DefaultCell.Border = Rectangle.NO_BORDER;

            //Company Logo           
            // cell = ImageCell("~/assets/images/Logo4.png", 70f, PdfPCell.ALIGN_CENTER);
            //cell = ImageCell("~/Images/" + companydata.CompanyLogo, 20f, PdfPCell.ALIGN_CENTER);
            if (companydata.CompanyLogo != null && companydata.CompanyLogo != "")
            {
                cell = ImageCell("~/Images/" + companydata.CompanyLogo, 20f, PdfPCell.ALIGN_CENTER);
            }
            else
            {
                cell = ImageCell("~/Images/default-logo.png", 20f, PdfPCell.ALIGN_CENTER);
            }
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            table.AddCell(cell);

            PdfPTable innertbl = new PdfPTable(1);
            innertbl.SetWidths(new float[] { 1.0f });
            PdfPCell innercell1 = null;
            phrase = new Phrase();
            //phrase.Add(new Chunk("                                                        Unit #3A, Plot No:847, Pacific Towers,", _bf_headingaddr));
            phrase.Add(new Chunk("                                                              " + companydata.Address1, _bf_headingaddr));
            innercell1 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell1.Border = 0;
            innercell1.Padding = 0;

            innertbl.SpacingBefore = 0f;
            innertbl.SpacingAfter = 2f;
            innertbl.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl.AddCell(innercell1);

            cell = new PdfPCell();
            cell.Border = 0;
            cell.Padding = 0;
            cell.AddElement(innertbl);

            PdfPTable innertbl2 = new PdfPTable(1);
            innertbl2.SetWidths(new float[] { 1.0f });
            PdfPCell innercell2 = null;
            phrase = new Phrase();
            //phrase.Add(new Chunk("                                                        Madhapur, Hyderabad-500081, TS", _bf_headingaddr));
            phrase.Add(new Chunk("                                                              " + companydata.Address2, _bf_headingaddr));
            innercell2 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell2.Border = 0;
            innercell2.Padding = 0;
            innertbl2.SpacingBefore = 0f;
            innertbl2.SpacingAfter = 2f;
            innertbl2.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl2.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl2.AddCell(innercell2);
            cell.AddElement(innertbl2);

            PdfPTable innertbl3 = new PdfPTable(1);
            innertbl3.SetWidths(new float[] { 1.0f });
            PdfPCell innercellTwo = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              GSTIN: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.GSTNumber, _bf_headingaddr));
            //phrase.Add(new Chunk("36AABCT3518Q1ZX", _bf_headingaddr));
            innercellTwo = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercellTwo.Border = 0;
            innercellTwo.Padding = 0;
            innertbl3.SpacingBefore = 0f;
            innertbl3.SpacingAfter = 2f;
            innertbl3.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl3.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl3.AddCell(innercellTwo);
            cell.AddElement(innertbl3);


            PdfPTable innertbl4 = new PdfPTable(1);
            innertbl4.SetWidths(new float[] { 1.0f });
            PdfPCell innercell5 = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              Ph: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.ContactNumber, _bf_headingaddr));
            //phrase.Add(new Chunk("+91 8143143143", _bf_headingaddr));
            innercell5 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell5.Border = 0;
            innercell5.Padding = 0;
            innertbl4.SpacingBefore = 0f;
            innertbl4.SpacingAfter = 2f;
            innertbl4.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl4.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl4.AddCell(innercell5);
            cell.AddElement(innertbl4);
            //table.AddCell(cell);

            PdfPTable innertbl5 = new PdfPTable(1);
            innertbl5.SetWidths(new float[] { 1.0f });
            PdfPCell innercell6 = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              Email: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.SupportEmailID, _bf_headingaddr));
            //phrase.Add(new Chunk("support@hmrl.com", _bf_headingaddr));
            innercell6 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell6.Border = 0;
            innercell6.Padding = 0;
            innertbl5.SpacingBefore = 0f;
            innertbl5.SpacingAfter = 0f;
            innertbl5.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl5.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl5.AddCell(innercell6);
            cell.AddElement(innertbl5);
            table.AddCell(cell);

            doc.Add(table);

            //Header Table end
            #endregion

            #region Title
            //Title table start
            PdfPTable tableTitle = new PdfPTable(1);
            tableTitle.DefaultCell.FixedHeight = 150f;
            tableTitle.TotalWidth = 550f;
            tableTitle.LockedWidth = true;
            tableTitle.SetWidths(new float[] { 1f });
            tableTitle.SpacingBefore = 10f;
            tableTitle.SpacingAfter = 3f;

            phrase = new Phrase();
            phrase.Add(new Chunk("REPORT BY PASSES", _bf_headingTitle));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
            cell.Border = 0;
            tableTitle.AddCell(cell);
            doc.Add(tableTitle);
            //Title table end
            #endregion

            #region Filter            
            PdfPTable tablefilter = new PdfPTable(1);
            tablefilter.TotalWidth = 550f;
            tablefilter.LockedWidth = true;
            tablefilter.SetWidths(new float[] { 1.0f });
            tablefilter.SpacingBefore = 3f;
            tablefilter.SpacingAfter = 3f;


            phrase = new Phrase();
            phrase.Add(new Chunk("Filtered By      :  ", _bf_headingboldTable));
            if (selectChannel == "OPERATOR PAY")
            {
                phrase.Add(new Chunk(selectChannel + ", " + selectstation + ", " + selectVehicle, _bf_heading1));
            }
            else
            {
                phrase.Add(new Chunk(selectChannel + ", " + selectVehicle, _bf_heading1));
            }
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tablefilter.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk(selectstation, _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.BorderColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#dbdbdb"));
            //tablefilter.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk(selectVehicle, _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.BorderColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#dbdbdb"));
            //tablefilter.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk("", _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            ////cell.BorderColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#dbdbdb"));
            //cell.Border = 0;
            //tablefilter.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk("", _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.Border = 0;
            //tablefilter.AddCell(cell);

            doc.Add(tablefilter);
            #endregion

            #region time
            PdfPTable tabletime = new PdfPTable(1);
            tabletime.TotalWidth = 550f;
            tabletime.LockedWidth = true;
            tabletime.SetWidths(new float[] { 1.0f });
            tabletime.SpacingBefore = 3f;
            tabletime.SpacingAfter = 3f;

            phrase = new Phrase();
            phrase.Add(new Chunk("Report From   :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(FDate + " to " + TDate, _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tabletime.AddCell(cell);
            doc.Add(tabletime);

            //phrase = new Phrase();
            //phrase.Add(new Chunk(FDate + " to " + TDate, _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            //cell.Border = 0;
            //tabletime.AddCell(cell);
            PdfPTable tabletime1 = new PdfPTable(1);
            tabletime1.TotalWidth = 550f;
            tabletime1.LockedWidth = true;
            tabletime1.SetWidths(new float[] { 1.0f });
            tabletime1.SpacingBefore = 3f;
            tabletime1.SpacingAfter = 5f;
            phrase = new Phrase();
            phrase.Add(new Chunk("Generated On :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(DateTime.Now.ToString("dd/MM/yyyy HH:MM tt"), _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tabletime1.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk(DateTime.Now.ToString("dd/MM/yyyy HH:MM tt"), _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            //cell.Border = 0;
            //tabletime.AddCell(cell);

            doc.Add(tabletime1);
            #endregion

            #region Data
            PdfPTable table2;
            if (selectChannel == "OPERATOR PAY")
            {
                table2 = new PdfPTable(dt.Columns.Count - 5);
                table2.SetWidths(new float[] { 0.2f, 0.25f, 0.2f, 0.1f, 0.2f, 0.2f, 0.11f, 0.11f, 0.15f });
            }
            else
            {
                table2 = new PdfPTable(dt.Columns.Count - 7);
                table2.SetWidths(new float[] { 0.25f, 0.09f, 0.2f, 0.2f, 0.1f, 0.1f, 0.15f });
            }

            table2.TotalWidth = doc.PageSize.Width - 40f;
            table2.LockedWidth = true;
            table2.SpacingBefore = 10f;
            //table2.SpacingAfter = 3f;

            if (selectChannel == "OPERATOR PAY")
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string cellText = Server.HtmlDecode(dt.Columns[i].ColumnName);
                    if (cellText != "TotalCash" && cellText != "TotalEPay" && cellText != "TotalCount" && cellText != "TotalPassWithNFC" && cellText != "TotalOnlyNFC")
                    {
                        PdfPCell cell2 = new PdfPCell();
                        if (cellText == "Cash" || cellText == "EPay" || cellText == "Amount")
                        {
                            cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                            cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                            cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                        }
                        else
                        {
                            //if (cellText == "TypeofPass")
                            //{
                            //    cell2.Phrase = new Phrase("Type of Pass", _bf_headingboldTable);
                            //    cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                            //    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                            //}
                            if (cellText == "TypeofPassName")
                            {
                                cell2.Phrase = new Phrase("Type of Pass Name", _bf_headingboldTable);
                                cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                                cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                            }
                            else if (cellText == "ParkingLot")
                            {
                                cell2.Phrase = new Phrase("Parking Lot", _bf_headingboldTable);
                                cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                                cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                            }
                            else if (cellText == "PassWithNFC")
                            {
                                cell2.Phrase = new Phrase("Pass With NFC", _bf_headingboldTable);
                                cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                                cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                            }
                            else if (cellText == "OnlyNFC")
                            {
                                cell2.Phrase = new Phrase("Only NFC", _bf_headingboldTable);
                                cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                                cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                            }
                            //else if (cellText == "PassIn")
                            //{
                            //    cell2.Phrase = new Phrase("Pass In", _bf_headingboldTable);
                            //    cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                            //    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                            //}
                            else
                            {
                                cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                                cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                                cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                            }
                        }
                        table2.AddCell(cell2);
                    }
                }
            }
            else
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string cellText = Server.HtmlDecode(dt.Columns[i].ColumnName);

                    if (cellText != "TotalCash" && cellText != "TotalEPay" && cellText != "TotalCount" && cellText != "TotalPassWithNFC" && cellText != "TotalOnlyNFC")
                    {
                        if (cellText != "Station" && cellText != "ParkingLot")
                        {
                            PdfPCell cell2 = new PdfPCell();
                            if (cellText == "Cash" || cellText == "EPay" || cellText == "Amount")
                            {
                                cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                                cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                                cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                            }
                            else
                            {
                                //if (cellText == "TypeofPass")
                                //{
                                //    cell2.Phrase = new Phrase("Type of Pass", _bf_headingboldTable);
                                //    cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                                //    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                                //}
                                if (cellText == "TypeofPassName")
                                {
                                    cell2.Phrase = new Phrase("Type of Pass Name", _bf_headingboldTable);
                                    cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                                }
                                else if (cellText == "PassWithNFC")
                                {
                                    cell2.Phrase = new Phrase("Pass With NFC", _bf_headingboldTable);
                                    cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                                }
                                else if (cellText == "OnlyNFC")
                                {
                                    cell2.Phrase = new Phrase("Only NFC", _bf_headingboldTable);
                                    cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                                }
                                //else if (cellText == "PassIn")
                                //{
                                //    cell2.Phrase = new Phrase("Pass In", _bf_headingboldTable);
                                //    cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                                //    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                                //}
                                else
                                {
                                    cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                                    cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                                }
                            }
                            table2.AddCell(cell2);
                        }
                    }

                }
            }

            //writing table Data  
            if (selectChannel == "OPERATOR PAY")
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string cellText = Server.HtmlDecode(dt.Columns[j].ColumnName);
                        if (cellText != "TotalCash" && cellText != "TotalEPay" && cellText != "TotalCount" && cellText != "TotalPassWithNFC" && cellText != "TotalOnlyNFC")
                        {
                            PdfPCell cell2 = new PdfPCell();
                            if (cellText == "Cash" || cellText == "EPay" || cellText == "Amount")
                            {
                                cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                                cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                                //cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                            }

                            else
                            {
                                cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                                cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                                //cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                            }
                            table2.AddCell(cell2);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string cellText = Server.HtmlDecode(dt.Columns[j].ColumnName);
                        if (cellText != "TotalCash" && cellText != "TotalEPay" && cellText != "TotalCount" && cellText != "TotalPassWithNFC" && cellText != "TotalOnlyNFC")
                        {
                            if (cellText != "Station" && cellText != "ParkingLot")
                            {
                                PdfPCell cell2 = new PdfPCell();
                                if (cellText == "Cash" || cellText == "EPay" || cellText == "Amount")
                                {
                                    cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                                    cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                                }
                                else
                                {
                                    cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                                }
                                table2.AddCell(cell2);
                            }
                        }
                    }
                }
            }

            //GRAND TOTAL TABLE
            PdfPTable grandTable;
            if (selectChannel == "OPERATOR PAY")
            {
                //grandTable = new PdfPTable(dt.Columns.Count - 2);
                //grandTable.TotalWidth = doc.PageSize.Width - 40f;
                //grandTable.LockedWidth = true;
                //PdfPCell cell3 = new PdfPCell();
                //string totalstring = string.Concat("Total         " + TotalCash + "       " + TotalEPay + "        " + grandTotal);
                //cell3.Phrase = new Phrase(totalstring, _bf_headingboldTable);
                //cell3.Colspan = dt.Columns.Count;
                //cell3.HorizontalAlignment = Element.ALIGN_RIGHT;
                //cell3.BorderWidthTop = 0;
                //grandTable.AddCell(cell3);

                grandTable = new PdfPTable(dt.Columns.Count - 5);
                grandTable.SetWidths(new float[] { 0.2f, 0.25f, 0.2f, 0.08f, 0.2f, 0.2f, 0.11f, 0.11f, 0.15f });
                grandTable.TotalWidth = doc.PageSize.Width - 40f;
                grandTable.LockedWidth = true;

                PdfPCell cellt = new PdfPCell(new Phrase(" ", _bf_headingboldTable));
                cellt.Colspan = 2;
                cellt.BorderWidthRight = 0;
                cellt.BorderWidthTop = 0;
                grandTable.AddCell(cellt);

                cellt = new PdfPCell();
                cellt.Phrase = new Phrase("Total ", _bf_headingboldTable);
                cellt.HorizontalAlignment = Element.ALIGN_LEFT;
                cellt.BorderWidthLeft = 0;
                cellt.BorderWidthRight = 0;
                cellt.BorderWidthTop = 0;
                grandTable.AddCell(cellt);

                cell = new PdfPCell();
                cellt.Phrase = new Phrase(TotalCount, _bf_headingboldTable);
                cellt.HorizontalAlignment = Element.ALIGN_LEFT;
                cellt.BorderWidthRight = 0;
                cellt.BorderWidthTop = 0;
                grandTable.AddCell(cellt);

                cell = new PdfPCell();
                cellt.Phrase = new Phrase(TotalPassWithNFC, _bf_headingboldTable);
                cellt.HorizontalAlignment = Element.ALIGN_LEFT;
                cellt.BorderWidthRight = 0;
                cellt.BorderWidthTop = 0;
                grandTable.AddCell(cellt);

                cell = new PdfPCell();
                cellt.Phrase = new Phrase(TotalOnlyNFC, _bf_headingboldTable);
                cellt.HorizontalAlignment = Element.ALIGN_LEFT;
                cellt.BorderWidthRight = 0;
                cellt.BorderWidthTop = 0;
                grandTable.AddCell(cellt);

                cell = new PdfPCell();
                cellt.Phrase = new Phrase(TotalCash, _bf_headingboldTable);
                cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellt.BorderWidthRight = 0;
                cellt.BorderWidthTop = 0;
                grandTable.AddCell(cellt);

                cell = new PdfPCell();
                cellt.Phrase = new Phrase(TotalEPay, _bf_headingboldTable);
                cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
                grandTable.AddCell(cellt);

                cell = new PdfPCell();
                cellt.Phrase = new Phrase(grandTotal, _bf_headingboldTable);
                cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellt.BorderWidthTop = 0;
                cellt.BorderWidthRight = 1;
                grandTable.AddCell(cellt);
            }
            else
            {
                //grandTable = new PdfPTable(dt.Columns.Count - 3);
                //grandTable.TotalWidth = doc.PageSize.Width - 40f;
                //grandTable.LockedWidth = true;
                //PdfPCell cell3 = new PdfPCell();
                //string totalstring = string.Concat("Total            " + TotalCash + "            " + TotalEPay + "                      " + grandTotal);
                //cell3.Phrase = new Phrase(totalstring, _bf_headingboldTable);
                //cell3.Colspan = dt.Columns.Count;
                //cell3.HorizontalAlignment = Element.ALIGN_RIGHT;
                //cell3.BorderWidthTop = 0;
                //grandTable.AddCell(cell3);

                grandTable = new PdfPTable(dt.Columns.Count - 7);
                grandTable.SetWidths(new float[] { 0.25f, 0.09f, 0.2f, 0.2f, 0.1f, 0.1f, 0.15f });
                grandTable.TotalWidth = doc.PageSize.Width - 40f;
                grandTable.LockedWidth = true;



                PdfPCell cellt = new PdfPCell();
                cellt.Phrase = new Phrase("Total ", _bf_headingboldTable);
                cellt.HorizontalAlignment = Element.ALIGN_LEFT;
                cellt.BorderWidthRight = 0;
                cellt.BorderWidthTop = 0;
                grandTable.AddCell(cellt);

                cell = new PdfPCell();
                cellt.Phrase = new Phrase(TotalCount, _bf_headingboldTable);
                cellt.HorizontalAlignment = Element.ALIGN_LEFT;
                cellt.BorderWidthLeft = 0;
                cellt.BorderWidthRight = 0;
                cellt.BorderWidthTop = 0;
                grandTable.AddCell(cellt);

                cell = new PdfPCell();
                cellt.Phrase = new Phrase(TotalPassWithNFC, _bf_headingboldTable);
                cellt.HorizontalAlignment = Element.ALIGN_LEFT;
                cellt.BorderWidthRight = 0;
                cellt.BorderWidthTop = 0;
                grandTable.AddCell(cellt);

                cell = new PdfPCell();
                cellt.Phrase = new Phrase(TotalOnlyNFC, _bf_headingboldTable);
                cellt.HorizontalAlignment = Element.ALIGN_LEFT;
                cellt.BorderWidthRight = 0;
                cellt.BorderWidthTop = 0;
                grandTable.AddCell(cellt);

                cell = new PdfPCell();
                cellt.Phrase = new Phrase(TotalCash, _bf_headingboldTable);
                cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellt.BorderWidthRight = 0;
                cellt.BorderWidthTop = 0;
                grandTable.AddCell(cellt);

                cell = new PdfPCell();
                cellt.Phrase = new Phrase(TotalEPay, _bf_headingboldTable);
                cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
                grandTable.AddCell(cellt);

                cell = new PdfPCell();
                cellt.Phrase = new Phrase(grandTotal, _bf_headingboldTable);
                cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellt.BorderWidthTop = 0;
                cellt.BorderWidthRight = 1;
                grandTable.AddCell(cellt);
            }
            //GRNAD TOTAL TABLE

            ////GST Table
            //PdfPTable gstTable;
            //if (selectChannel == "OPERATOR PAY")
            //{
            //    gstTable = new PdfPTable(dt.Columns.Count - 5);
            //    gstTable.SetWidths(new float[] { 0.2f, 0.25f, 0.2f, 0.08f, 0.2f, 0.2f, 0.11f, 0.11f, 0.15f });
            //    gstTable.TotalWidth = doc.PageSize.Width - 40f;
            //    gstTable.LockedWidth = true;

            //    PdfPCell cellGST = new PdfPCell(new Phrase(" ", _bf_headingboldTable));
            //    cellGST.BorderWidthRight = 0;
            //    cellGST.BorderWidthTop = 0;
            //    cellGST.Colspan = 7;
            //    gstTable.AddCell(cellGST);

            //    cellGST = new PdfPCell();
            //    cellGST.Phrase = new Phrase("GST", _bf_headingboldTable);
            //    cellGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            //    cellGST.BorderWidthLeft = 0;
            //    cellGST.BorderWidthRight = 0;
            //    cellGST.BorderWidthTop = 0;
            //    gstTable.AddCell(cellGST);

            //    cell = new PdfPCell();
            //    cellGST.Phrase = new Phrase(GST, _bf_headingboldTable);
            //    cellGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            //    cellGST.BorderWidthTop = 0;
            //    cellGST.BorderWidthRight = 1;
            //    gstTable.AddCell(cellGST);
            //}
            //else
            //{
            //    gstTable = new PdfPTable(dt.Columns.Count - 7);
            //    gstTable.SetWidths(new float[] { 0.25f, 0.09f, 0.2f, 0.2f, 0.1f, 0.1f, 0.15f });
            //    gstTable.TotalWidth = doc.PageSize.Width - 40f;
            //    gstTable.LockedWidth = true;

            //    PdfPCell cellGST = new PdfPCell(new Phrase(" ", _bf_headingboldTable));
            //    cellGST.BorderWidthRight = 0;
            //    cellGST.BorderWidthTop = 0;
            //    cellGST.Colspan = 5;
            //    gstTable.AddCell(cellGST);

            //    cellGST = new PdfPCell();
            //    cellGST.Phrase = new Phrase("GST", _bf_headingboldTable);
            //    cellGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            //    cellGST.BorderWidthLeft = 0;
            //    cellGST.BorderWidthRight = 0;
            //    cellGST.BorderWidthTop = 0;
            //    gstTable.AddCell(cellGST);

            //    cell = new PdfPCell();
            //    cellGST.Phrase = new Phrase(GST, _bf_headingboldTable);
            //    cellGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            //    cellGST.BorderWidthTop = 0;
            //    cellGST.BorderWidthRight = 1;
            //    gstTable.AddCell(cellGST);
            //}
            ////GST Table

            doc.Add(table2);
            doc.Add(grandTable);
           // doc.Add(gstTable);
            #endregion

            doc.Close();
            byte[] result = ms.ToArray();
            return filename;

        }
        public ActionResult PassesReportExcelDownload(string GrandTotal, string SelectedItems, string Channel)
        {
            try
            {
                Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//18012021

                var myDetails = JObject.Parse(SelectedItems);
                var fromDate = myDetails["FromDate"];
                var toDate = myDetails["ToDate"];
                var duration = myDetails["Duration"];
                string selectfromdate = ((Newtonsoft.Json.Linq.JValue)fromDate).Value.ToString();
                string selecttodate = ((Newtonsoft.Json.Linq.JValue)toDate).Value.ToString();
                string selectduration = ((Newtonsoft.Json.Linq.JValue)duration).Value.ToString();

                string FDate = "";
                string TDate = "";

                if (selectduration == "" || selectduration == null || selectduration == "0")
                {
                    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                }
                else
                {
                    FDate = DateTime.Parse(selectfromdate).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                }

                List<RevenueByPasses> result = (List<RevenueByPasses>)Session["PassesReportData"];
                DataTable dt = ToDataTable(result);

                string TotalCount = Convert.ToString(dt.Rows[0]["TotalCount"]);
                string TotalPassWithNFC = Convert.ToString(dt.Rows[0]["TotalPassWithNFC"]);
                string TotalOnlyNFC = Convert.ToString(dt.Rows[0]["TotalOnlyNFC"]);

                string TotalCash = Convert.ToString(dt.Rows[0]["TotalCash"]);
                string TotalEPay = Convert.ToString(dt.Rows[0]["TotalEPay"]);
                string path = System.Web.HttpContext.Current.Server.MapPath("~/ExcelReports");
                string filename = "RevenueByPassesReport_" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".xlsx";
                string file_path = path + "/" + filename;

                if (dt.Rows.Count > 0)
                {
                    #region New Code
                    using (XLWorkbook wb = new XLWorkbook())
                    {

                        wb.Worksheets.Add("RevenueReportByPasses", 0);
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Value = companydata.AccountName;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.FontSize = 12;
                        wb.Worksheets.Worksheet(0).Cell(1, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(2, 1).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(2, 20).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(3, 1).Value = "Revenue By Passes Report";
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(3, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 1).Value = "From : " + FDate + " To : " + TDate;
                        wb.Worksheets.Worksheet(0).Cell(4, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 5).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 6).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 7).Value = "Cur.Date : " + DateTime.Now.ToString("dd/MM/yyy").Split(' ')[0];
                        wb.Worksheets.Worksheet(0).Cell(4, 7).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        if (Channel == "OPERATOR PAY")
                        {
                            wb.Worksheets.Worksheet(0).Cell(6, 1).Value = "Station";
                            wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "Parking Lot";
                            wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.FontSize = 11;

                            //wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "Type of Pass";
                            //wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.Bold = true;
                            //wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Type of Pass Name";
                            wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.FontSize = 11;

                            //wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Pass In";
                            //wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.Bold = true;
                            //wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(6, 4).Value = "Count";
                            wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(6, 5).Value = "Pass With NFC";
                            wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(6, 6).Value = "Only NFC";
                            wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(6, 7).Value = "Cash";
                            wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(6, 8).Value = "EPay";
                            wb.Worksheets.Worksheet(0).Cell(6, 8).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 8).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(6, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(6, 9).Value = "Amount";
                            wb.Worksheets.Worksheet(0).Cell(6, 9).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 9).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(6, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        }
                        else
                        {
                            //wb.Worksheets.Worksheet(0).Cell(6, 1).Value = "Type of Pass";
                            //wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.Bold = true;
                            //wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(6, 1).Value = "Type of Pass Name";
                            wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.FontSize = 11;

                            //wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "Pass In";
                            //wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.Bold = true;
                            //wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "Count";
                            wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Pass With NFC";
                            wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(6, 4).Value = "Only NFC";
                            wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(6, 5).Value = "Cash";
                            wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(6, 6).Value = "EPay";
                            wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(6, 7).Value = "Amount";
                            wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        }

                        int row = 7;
                        int column_num = 0;

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (Channel == "OPERATOR PAY")
                            {
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Value = Convert.ToString(dt.Rows[i]["Station"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Style.Font.FontSize = 11;

                                //wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Value = Convert.ToString(dt.Rows[i]["TypeofPass"]);
                                //wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Style.Font.FontSize = 11;
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Value = Convert.ToString(dt.Rows[i]["ParkingLot"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Style.Font.FontSize = 11;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Value = Convert.ToString(dt.Rows[i]["TypeofPassName"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Font.FontSize = 11;

                                //wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Value = Convert.ToString(dt.Rows[i]["PassIn"]);
                                //wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Font.FontSize = 11;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Value = Convert.ToString(dt.Rows[i]["Count"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Font.FontSize = 11;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Value = Convert.ToString(dt.Rows[i]["PassWithNFC"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Font.FontSize = 11;
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Value = Convert.ToString(dt.Rows[i]["OnlyNFC"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Style.Font.FontSize = 11;
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Value = Convert.ToString(dt.Rows[i]["Cash"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Style.Font.FontSize = 11;
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 8).Value = Convert.ToString(dt.Rows[i]["EPay"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 8).Style.Font.FontSize = 11;
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 9).Value = Convert.ToString(dt.Rows[i]["Amount"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 9).Style.Font.FontSize = 11;
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                                row = row + 1;
                            }
                            else
                            {
                                //wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Value = Convert.ToString(dt.Rows[i]["TypeofPass"]);
                                //wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Style.Font.FontSize = 11;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Value = Convert.ToString(dt.Rows[i]["TypeofPassName"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Style.Font.FontSize = 11;

                                //wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Value = Convert.ToString(dt.Rows[i]["PassIn"]);
                                //wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Style.Font.FontSize = 11;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Value = Convert.ToString(dt.Rows[i]["Count"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Style.Font.FontSize = 11;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Value = Convert.ToString(dt.Rows[i]["PassWithNFC"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Font.FontSize = 11;
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Value = Convert.ToString(dt.Rows[i]["OnlyNFC"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Font.FontSize = 11;
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Value = Convert.ToString(dt.Rows[i]["Cash"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Font.FontSize = 11;
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Value = Convert.ToString(dt.Rows[i]["EPay"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Style.Font.FontSize = 11;
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Value = Convert.ToString(dt.Rows[i]["Amount"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Style.Font.FontSize = 11;
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                                row = row + 1;
                            }

                        }

                        //row = row + 1;
                        if (Channel == "OPERATOR PAY")
                        {
                            wb.Worksheets.Worksheet(0).Cell(row, 3).Value = "Total : ";
                            wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, 4).Value = TotalCount;
                            wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, 5).Value = TotalPassWithNFC;
                            wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, 6).Value = TotalOnlyNFC;
                            wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, 7).Value = TotalCash;
                            wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, 8).Value = TotalEPay;
                            wb.Worksheets.Worksheet(0).Cell(row, 8).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(row, 8).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, 9).Value = GrandTotal;
                            wb.Worksheets.Worksheet(0).Cell(row, 9).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(row, 9).Style.Font.FontSize = 11;

                            //wb.Worksheets.Worksheet(0).Cell(row + 1, 8).Value = "GST";
                            //wb.Worksheets.Worksheet(0).Cell(row + 1, 8).Style.Font.Bold = true;
                            //wb.Worksheets.Worksheet(0).Cell(row + 1, 8).Style.Font.FontSize = 11;
                            //wb.Worksheets.Worksheet(0).Cell(row + 1, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            //wb.Worksheets.Worksheet(0).Cell(row + 1, 9).Value = GST;
                            //wb.Worksheets.Worksheet(0).Cell(row + 1, 9).Style.Font.Bold = true;
                            //wb.Worksheets.Worksheet(0).Cell(row + 1, 9).Style.Font.FontSize = 11;
                            //wb.Worksheets.Worksheet(0).Cell(row + 1, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        }
                        else
                        {
                            wb.Worksheets.Worksheet(0).Cell(row, 1).Value = "Total : ";
                            wb.Worksheets.Worksheet(0).Cell(row, 1).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(row, 1).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, 2).Value = TotalCount;
                            wb.Worksheets.Worksheet(0).Cell(row, 2).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(row, 2).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, 3).Value = TotalPassWithNFC;
                            wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, 4).Value = TotalOnlyNFC;
                            wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, 5).Value = TotalCash;
                            wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, 6).Value = TotalEPay;
                            wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, 7).Value = GrandTotal;
                            wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.FontSize = 11;

                            //wb.Worksheets.Worksheet(0).Cell(row + 1, 7).Value = "GST";
                            //wb.Worksheets.Worksheet(0).Cell(row + 1, 7).Style.Font.Bold = true;
                            //wb.Worksheets.Worksheet(0).Cell(row + 1, 7).Style.Font.FontSize = 11;
                            //wb.Worksheets.Worksheet(0).Cell(row + 1, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            //wb.Worksheets.Worksheet(0).Cell(row + 1, 8).Value = GST;
                            //wb.Worksheets.Worksheet(0).Cell(row + 1, 8).Style.Font.Bold = true;
                            //wb.Worksheets.Worksheet(0).Cell(row + 1, 8).Style.Font.FontSize = 11;
                            //wb.Worksheets.Worksheet(0).Cell(row + 1, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        }



                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                        using (MemoryStream MyMemoryStream = new MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                    #endregion

                }
                return Redirect("ReportByPasses");
            }
            catch (Exception ex)
            {
                return Redirect("ReportByPasses");
            }
        }
        #endregion

        #region Channel Report
        public ActionResult ReportByChannel()
        {
            ViewBag.Menu = "RevenueReports";
            return View();
        }
        [HttpPost]
        public JsonResult GetReportByChannel(SearchFilters channelFilterData)
        {
            try
            {
                if (channelFilterData.FromTime != null || channelFilterData.Duration == "Today")
                {
                    string fDate1 = Convert.ToString(channelFilterData.FromDate).Split(' ')[0];
                    DateTime fTotalDate = Convert.ToDateTime(fDate1 + " " + channelFilterData.FromTime + ":00 " + channelFilterData.FromMeridiem);

                    string tDate1 = Convert.ToString(channelFilterData.ToDate).Split(' ')[0];
                    DateTime tTotalDate = Convert.ToDateTime(tDate1 + " " + channelFilterData.ToTime + ":00 " + channelFilterData.ToMeridiem);

                    channelFilterData.FromDate = fTotalDate;
                    channelFilterData.ToDate = tTotalDate;
                }

                List<RevenueByChannel> revenueBychannel_lst = revenueReportsDAL_obj.GetReportByChannel(channelFilterData);
                Session["ChannelReportData"] = revenueBychannel_lst;
                return new JsonResult()
                {
                    Data = revenueBychannel_lst,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult ChannelReportPDFDownload(string GrandTotal, string SelectedItems)
        {
            try
            {
                var myDetails = JObject.Parse(SelectedItems);
                var selectedStation = myDetails["SelectedStation"];
                var selectedLot = myDetails["SelectedLot"];
                var SelectedVehicle = myDetails["SelectedVehicle"];
                string selectstation = ((Newtonsoft.Json.Linq.JValue)selectedStation).Value.ToString();
                string selectLot = ((Newtonsoft.Json.Linq.JValue)selectedLot).Value.ToString();
                string selectVehicle = ((Newtonsoft.Json.Linq.JValue)SelectedVehicle).Value.ToString();

                var fromDate = myDetails["FromDate"];
                var toDate = myDetails["ToDate"];
                var duration = myDetails["Duration"];
                string selectfromdate = ((Newtonsoft.Json.Linq.JValue)fromDate).Value.ToString();
                string selecttodate = ((Newtonsoft.Json.Linq.JValue)toDate).Value.ToString();
                string selectduration = ((Newtonsoft.Json.Linq.JValue)duration).Value.ToString();

                string FDate = "";
                string TDate = "";

                if (selectduration == "" || selectduration == null || selectduration == "0")
                {
                    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                }
                else
                {
                    FDate = DateTime.Parse(selectfromdate).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                }

                //string FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                //string TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");

                List<RevenueByChannel> result = (List<RevenueByChannel>)Session["ChannelReportData"];
                DataTable dt = ToDataTable(result);
                string TotalCheckIns = Convert.ToString(dt.Rows[0]["TotalCheckIns"]);
                string TotalCash = Convert.ToString(dt.Rows[0]["TotalCash"]);
                string TotalEPay = Convert.ToString(dt.Rows[0]["TotalEPay"]);
                string filename = GenerateReportByChannel(dt, GrandTotal, selectstation, selectLot, selectVehicle, FDate, TDate, TotalCash, TotalEPay, TotalCheckIns);
                FileStream sourceFile = null;

                if (filename != "")
                {
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("content-disposition", "attachment; filename=\"" + Path.GetFileName(filename) + "\"");
                    sourceFile = new FileStream(filename, FileMode.Open);
                    long FileSize;
                    FileSize = sourceFile.Length;
                    byte[] getContent = new byte[(int)FileSize];
                    sourceFile.Read(getContent, 0, (int)sourceFile.Length);
                    sourceFile.Close();
                    Response.Clear();
                    Response.BinaryWrite(getContent);
                    Response.End();
                }
                return Redirect("ReportByChannel");
            }
            catch (Exception ex)
            {
                return Redirect("ReportByChannel");
            }
        }
        public string GenerateReportByChannel(DataTable dt, string grandTotal, string selectstation, string selectLot, string selectVehicle, string FDate, string TDate, string TotalCash, string TotalEPay, string TotalCheckIns)
        {
            Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//15012021

            string filename, AttachmentName1 = "";
            string path = System.Web.HttpContext.Current.Server.MapPath("~/PDFReports");
            AttachmentName1 = "Report By Channel" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".pdf";
            filename = path + "/" + AttachmentName1;

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(PageSize.A4);
            rec.BackgroundColor = new BaseColor(System.Drawing.Color.Olive);
            Document doc = new Document(rec, 88f, 88f, 10f, 50f);
            var output = new FileStream(filename, FileMode.Create);
            doc.SetPageSize(iTextSharp.text.PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, output);
            writer.PageEvent = new Footer();
            doc.Open();

            BaseColor FontColour = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#353535"));
            BaseColor FontColourBlack = BaseColor.BLACK;
            iTextSharp.text.Font calibri8 = FontFactory.GetFont("Calibri", 10f, FontColour);
            iTextSharp.text.Font _bf_headingaddr = FontFactory.GetFont("Calibri", 8f, FontColour);
            iTextSharp.text.Font _bf_headingaddrbold = FontFactory.GetFont("Calibri", 8f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_heading1 = FontFactory.GetFont("Calibri", 10f, FontColour);
            iTextSharp.text.Font _bf_headingbold = FontFactory.GetFont("Calibri", 10f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingTitle = FontFactory.GetFont("Calibri", 12f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingboldTable = FontFactory.GetFont("Calibri", 10f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingboldTableNrml = FontFactory.GetFont("Calibri", 9f, FontColour);

            Phrase phrase = null;
            PdfPCell cell = null;
            PdfPTable table = null;

            #region Logo 
            //Header Table start
            table = new PdfPTable(2);
            table.TotalWidth = 550f;
            table.LockedWidth = true;
            table.SetWidths(new float[] { 0.3f, 0.7f });
            table.DefaultCell.Border = Rectangle.NO_BORDER;

            //Company Logo           
            // cell = ImageCell("~/assets/images/Logo4.png", 70f, PdfPCell.ALIGN_CENTER);
            //cell = ImageCell("~/Images/" + companydata.CompanyLogo, 20f, PdfPCell.ALIGN_CENTER);
            if (companydata.CompanyLogo != null && companydata.CompanyLogo != "")
            {
                cell = ImageCell("~/Images/" + companydata.CompanyLogo, 20f, PdfPCell.ALIGN_CENTER);
            }
            else
            {
                cell = ImageCell("~/Images/default-logo.png", 20f, PdfPCell.ALIGN_CENTER);
            }
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            table.AddCell(cell);

            PdfPTable innertbl = new PdfPTable(1);
            innertbl.SetWidths(new float[] { 1.0f });
            PdfPCell innercell1 = null;
            phrase = new Phrase();
            //phrase.Add(new Chunk("                                                        Unit #3A, Plot No:847, Pacific Towers,", _bf_headingaddr));
            phrase.Add(new Chunk("                                                              " + companydata.Address1, _bf_headingaddr));
            innercell1 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell1.Border = 0;
            innercell1.Padding = 0;

            innertbl.SpacingBefore = 0f;
            innertbl.SpacingAfter = 2f;
            innertbl.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl.AddCell(innercell1);

            cell = new PdfPCell();
            cell.Border = 0;
            cell.Padding = 0;
            cell.AddElement(innertbl);

            PdfPTable innertbl2 = new PdfPTable(1);
            innertbl2.SetWidths(new float[] { 1.0f });
            PdfPCell innercell2 = null;
            phrase = new Phrase();
            //phrase.Add(new Chunk("                                                        Madhapur, Hyderabad-500081, TS", _bf_headingaddr));
            phrase.Add(new Chunk("                                                              " + companydata.Address2, _bf_headingaddr));
            innercell2 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell2.Border = 0;
            innercell2.Padding = 0;
            innertbl2.SpacingBefore = 0f;
            innertbl2.SpacingAfter = 2f;
            innertbl2.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl2.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl2.AddCell(innercell2);
            cell.AddElement(innertbl2);

            PdfPTable innertbl3 = new PdfPTable(1);
            innertbl3.SetWidths(new float[] { 1.0f });
            PdfPCell innercellTwo = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              GSTIN: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.GSTNumber, _bf_headingaddr));
            //phrase.Add(new Chunk("36AABCT3518Q1ZX", _bf_headingaddr));
            innercellTwo = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercellTwo.Border = 0;
            innercellTwo.Padding = 0;
            innertbl3.SpacingBefore = 0f;
            innertbl3.SpacingAfter = 2f;
            innertbl3.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl3.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl3.AddCell(innercellTwo);
            cell.AddElement(innertbl3);


            PdfPTable innertbl4 = new PdfPTable(1);
            innertbl4.SetWidths(new float[] { 1.0f });
            PdfPCell innercell5 = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              Ph: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.ContactNumber, _bf_headingaddr));
            //phrase.Add(new Chunk("+91 8143143143", _bf_headingaddr));
            innercell5 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell5.Border = 0;
            innercell5.Padding = 0;
            innertbl4.SpacingBefore = 0f;
            innertbl4.SpacingAfter = 2f;
            innertbl4.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl4.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl4.AddCell(innercell5);
            cell.AddElement(innertbl4);
            //table.AddCell(cell);

            PdfPTable innertbl5 = new PdfPTable(1);
            innertbl5.SetWidths(new float[] { 1.0f });
            PdfPCell innercell6 = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              Email: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.SupportEmailID, _bf_headingaddr));
            //phrase.Add(new Chunk("support@hmrl.com", _bf_headingaddr));
            innercell6 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell6.Border = 0;
            innercell6.Padding = 0;
            innertbl5.SpacingBefore = 0f;
            innertbl5.SpacingAfter = 0f;
            innertbl5.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl5.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl5.AddCell(innercell6);
            cell.AddElement(innertbl5);
            table.AddCell(cell);

            doc.Add(table);

            //Header Table end
            #endregion

            #region Title
            //Title table start
            PdfPTable tableTitle = new PdfPTable(1);
            tableTitle.DefaultCell.FixedHeight = 150f;
            tableTitle.TotalWidth = 550f;
            tableTitle.LockedWidth = true;
            tableTitle.SetWidths(new float[] { 1f });
            tableTitle.SpacingBefore = 10f;
            tableTitle.SpacingAfter = 3f;

            phrase = new Phrase();
            phrase.Add(new Chunk("REPORT BY CHANNEL", _bf_headingTitle));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
            cell.Border = 0;
            tableTitle.AddCell(cell);
            doc.Add(tableTitle);
            //Title table end
            #endregion

            #region Filter            
            PdfPTable tablefilter = new PdfPTable(1);
            tablefilter.TotalWidth = 550f;
            tablefilter.LockedWidth = true;
            tablefilter.SetWidths(new float[] { 1.0f });
            tablefilter.SpacingBefore = 3f;
            tablefilter.SpacingAfter = 3f;


            phrase = new Phrase();
            phrase.Add(new Chunk("Filtered By      :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(selectstation + ", " + selectLot + ", " + selectVehicle, _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tablefilter.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk(selectstation, _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.BorderColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#dbdbdb"));
            //tablefilter.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk(selectLot, _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.BorderColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#dbdbdb"));
            //tablefilter.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk(selectVehicle, _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.BorderColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#dbdbdb"));
            //tablefilter.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk("", _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.Border = 0;
            //tablefilter.AddCell(cell);

            doc.Add(tablefilter);
            #endregion

            #region time
            PdfPTable tabletime = new PdfPTable(1);
            tabletime.TotalWidth = 550f;
            tabletime.LockedWidth = true;
            tabletime.SetWidths(new float[] { 1.0f });
            tabletime.SpacingBefore = 3f;
            tabletime.SpacingAfter = 3f;

            phrase = new Phrase();
            phrase.Add(new Chunk("Report From   :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(FDate + " to " + TDate, _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tabletime.AddCell(cell);
            doc.Add(tabletime);
            //phrase = new Phrase();
            //phrase.Add(new Chunk(FDate + " to " + TDate, _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            //cell.Border = 0;
            //tabletime.AddCell(cell);
            PdfPTable tabletime1 = new PdfPTable(1);
            tabletime1.TotalWidth = 550f;
            tabletime1.LockedWidth = true;
            tabletime1.SetWidths(new float[] { 1.0f });
            tabletime1.SpacingBefore = 3f;
            tabletime1.SpacingAfter = 5f;
            phrase = new Phrase();
            phrase.Add(new Chunk("Generated On :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(DateTime.Now.ToString("dd/MM/yyyy HH:MM tt"), _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tabletime1.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk(DateTime.Now.ToString("dd/MM/yyyy HH:MM tt"), _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            //cell.Border = 0;
            //tabletime.AddCell(cell);

            doc.Add(tabletime1);
            #endregion

            #region Data
            PdfPTable table2 = new PdfPTable(dt.Columns.Count - 3);
            table2.SetWidths(new float[] { 0.2f, 0.25f, 0.15f, 0.12f, 0.18f, 0.18f, 0.18f });
            table2.TotalWidth = doc.PageSize.Width - 40f;
            table2.LockedWidth = true;
            table2.SpacingBefore = 10f;
            //table2.SpacingAfter = 3f;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string cellText = Server.HtmlDecode(dt.Columns[i].ColumnName);
                if (cellText != "TotalCash" && cellText != "TotalEPay" && cellText != "TotalCheckIns")
                {
                    PdfPCell cell2 = new PdfPCell();
                    if (cellText == "Cash" || cellText == "EPay" || cellText == "Amount")
                    {
                        cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                        cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                        cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    }
                    else
                    {
                        if (cellText == "CheckIns")
                        {
                            cell2.Phrase = new Phrase("Check Ins", _bf_headingboldTable);
                        }
                        else if (cellText == "ParkingLot")
                        {
                            cell2.Phrase = new Phrase("Parking Lot", _bf_headingboldTable);
                            // cell2.Phrase = new Phrase("Lot Code", _bf_headingboldTable);
                        }
                        else
                        {
                            cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                        }
                        cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                        cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    }
                    table2.AddCell(cell2);
                }
            }

            //writing table Data  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string cellText = Server.HtmlDecode(dt.Columns[j].ColumnName);
                    if (cellText != "TotalCash" && cellText != "TotalEPay" && cellText != "TotalCheckIns")
                    {
                        PdfPCell cell2 = new PdfPCell();
                        if (cellText == "Cash" || cellText == "EPay" || cellText == "Amount")
                        {
                            cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                            cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                            //  cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                        }
                        else
                        {
                            cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                            cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                        }
                        table2.AddCell(cell2);
                    }
                }
            }
            //GRAND TOTAL TABLE
            //PdfPTable grandTable = new PdfPTable(dt.Columns.Count - 2);
            //grandTable.TotalWidth = doc.PageSize.Width - 40f;
            //grandTable.LockedWidth = true;
            //PdfPCell cell3 = new PdfPCell();
            //string totalstring = string.Concat("Total                " + TotalCash + "                " + TotalEPay + "             " + grandTotal);
            //cell3.Phrase = new Phrase(totalstring, _bf_headingboldTable);
            //cell3.Colspan = dt.Columns.Count;
            //cell3.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cell3.BorderWidthTop = 0;
            //grandTable.AddCell(cell3);

            PdfPTable grandTable = new PdfPTable(dt.Columns.Count - 3);
            grandTable.SetWidths(new float[] { 0.2f, 0.25f, 0.15f, 0.12f, 0.18f, 0.18f, 0.18f });
            grandTable.TotalWidth = doc.PageSize.Width - 40f;
            grandTable.LockedWidth = true;

            PdfPCell cellt = new PdfPCell(new Phrase(" ", _bf_headingboldTable));
            cellt.Colspan = 2;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cellt = new PdfPCell();
            cellt.Phrase = new Phrase("Total ", _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
            cellt.BorderWidthLeft = 0;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cellt = new PdfPCell();
            cellt.Phrase = new Phrase(TotalCheckIns, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
            cellt.BorderWidthLeft = 0;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalCash, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalEPay, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(grandTotal, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthTop = 0;
            cellt.BorderWidthRight = 1;
            grandTable.AddCell(cellt);

            //GRNAD TOTAL TABLE

            ////GST Table
            //PdfPTable gstTable = new PdfPTable(dt.Columns.Count - 3);
            //gstTable.SetWidths(new float[] { 0.2f, 0.25f, 0.15f, 0.12f, 0.18f, 0.18f, 0.18f });
            //gstTable.TotalWidth = doc.PageSize.Width - 40f;
            //gstTable.LockedWidth = true;

            //PdfPCell cellGST = new PdfPCell(new Phrase(" ", _bf_headingboldTable));
            //cellGST.BorderWidthRight = 0;
            //cellGST.BorderWidthTop = 0;
            //cellGST.Colspan = 5;
            //gstTable.AddCell(cellGST);

            //cellGST = new PdfPCell();
            //cellGST.Phrase = new Phrase("GST", _bf_headingboldTable);
            //cellGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cellGST.BorderWidthLeft = 0;
            //cellGST.BorderWidthRight = 0;
            //cellGST.BorderWidthTop = 0;
            //gstTable.AddCell(cellGST);

            //cell = new PdfPCell();
            //cellGST.Phrase = new Phrase(GST, _bf_headingboldTable);
            //cellGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cellGST.BorderWidthTop = 0;
            //cellGST.BorderWidthRight = 1;
            //gstTable.AddCell(cellGST);
            ////GST Table


            doc.Add(table2);
            doc.Add(grandTable);
          //  doc.Add(gstTable);
            #endregion

            doc.Close();
            byte[] result = ms.ToArray();
            return filename;

        }
        public ActionResult ChannelReportExcelDownload(string GrandTotal, string SelectedItems)
        {
            try
            {
                Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//18012021

                string path = System.Web.HttpContext.Current.Server.MapPath("~/ExcelReports");
                string filename = "RevenueByChannelReport_" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".xlsx";
                string file_path = path + "/" + filename;

                var myDetails = JObject.Parse(SelectedItems);
                var fromDate = myDetails["FromDate"];
                var toDate = myDetails["ToDate"];
                var duration = myDetails["Duration"];
                string selectfromdate = ((Newtonsoft.Json.Linq.JValue)fromDate).Value.ToString();
                string selecttodate = ((Newtonsoft.Json.Linq.JValue)toDate).Value.ToString();
                string selectduration = ((Newtonsoft.Json.Linq.JValue)duration).Value.ToString();

                string FDate = "";
                string TDate = "";

                if (selectduration == "" || selectduration == null || selectduration == "0")
                {
                    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                }
                else
                {
                    FDate = DateTime.Parse(selectfromdate).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                }

                //string FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                //string TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");

                List<RevenueByChannel> result = (List<RevenueByChannel>)Session["ChannelReportData"];
                DataTable dt = ToDataTable(result);
                string TotalCheckIns = Convert.ToString(dt.Rows[0]["TotalCheckIns"]);
                string TotalCash = Convert.ToString(dt.Rows[0]["TotalCash"]);
                string TotalEPay = Convert.ToString(dt.Rows[0]["TotalEPay"]);

                if (dt.Rows.Count > 0)
                {
                    #region New Code
                    using (XLWorkbook wb = new XLWorkbook())
                    {

                        wb.Worksheets.Add("RevenueReportByChannel", 0);
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Value = companydata.AccountName;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.FontSize = 12;
                        wb.Worksheets.Worksheet(0).Cell(1, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(2, 1).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(2, 20).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(3, 1).Value = "Revenue By Channel Report";
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(3, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 1).Value = "From : " + FDate + " To : " + TDate;
                        wb.Worksheets.Worksheet(0).Cell(4, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 5).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 6).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 7).Value = "Cur.Date : " + DateTime.Now.ToString("dd/MM/yyy").Split(' ')[0];
                        wb.Worksheets.Worksheet(0).Cell(4, 7).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(6, 1).Value = "Channel";
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "Station";
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Parking Lot";
                        // wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Lot Code";
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 4).Value = "Check Ins";
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 5).Value = "Cash";
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 6).Value = "EPay";
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 7).Value = "Amount";
                        wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        int row = 7;
                        int column_num = 0;


                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Value = Convert.ToString(dt.Rows[i]["Channel"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Value = Convert.ToString(dt.Rows[i]["Station"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Value = Convert.ToString(dt.Rows[i]["ParkingLot"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Value = Convert.ToString(dt.Rows[i]["CheckIns"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Value = Convert.ToString(dt.Rows[i]["Cash"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Value = Convert.ToString(dt.Rows[i]["EPay"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Value = Convert.ToString(dt.Rows[i]["Amount"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            row = row + 1;
                        }

                        // row = row + 1;
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Value = "Total : ";
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 4).Value = TotalCheckIns;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(row, 5).Value = TotalCash;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(row, 6).Value = TotalEPay;
                        wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(row, 7).Value = GrandTotal;
                        wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 6).Value = "GST";
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 6).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 6).Style.Font.FontSize = 11;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 7).Value = GST;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 7).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 7).Style.Font.FontSize = 11;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                        using (MemoryStream MyMemoryStream = new MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                    #endregion

                }
                return Redirect("ReportByChannel");
            }
            catch (Exception ex)
            {
                return Redirect("ReportByChannel");
            }
        }
        #endregion

        #region Supervisor Report
        public ActionResult ReportBySupervisor()
        {
            ViewBag.Menu = "RevenueReports";
            return View();
        }
        public JsonNetResult GetActiveSupervisorsList()
        {
            IList<User> supervisorsList = revenueReportsDAL_obj.GetActiveSupervisorsList();

            try
            {
                return new JsonNetResult()
                {
                    Data = supervisorsList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        public JsonNetResult GetSupervisorLocationList(string SupervisorID)
        {
            IList<Locations> locationsList = revenueReportsDAL_obj.GetSupervisorLocationList(Convert.ToInt32(SupervisorID));

            try
            {
                return new JsonNetResult()
                {
                    Data = locationsList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        [HttpPost]
        public JsonResult GetReportBySupervisor(SearchFilters supervisorFilterData)
        {
            try
            {
                if (supervisorFilterData.FromTime != null || supervisorFilterData.Duration == "Today")
                {
                    string fDate1 = Convert.ToString(supervisorFilterData.FromDate).Split(' ')[0];
                    DateTime fTotalDate = Convert.ToDateTime(fDate1 + " " + supervisorFilterData.FromTime + ":00 " + supervisorFilterData.FromMeridiem);

                    string tDate1 = Convert.ToString(supervisorFilterData.ToDate).Split(' ')[0];
                    DateTime tTotalDate = Convert.ToDateTime(tDate1 + " " + supervisorFilterData.ToTime + ":00 " + supervisorFilterData.ToMeridiem);

                    supervisorFilterData.FromDate = fTotalDate;
                    supervisorFilterData.ToDate = tTotalDate;
                }

                List<RevenueBySupervisor> revenueBysupervisor_lst = revenueReportsDAL_obj.GetReportBySupervisor(supervisorFilterData);
                Session["SupervisorReportData"] = revenueBysupervisor_lst;
                return new JsonResult()
                {
                    Data = revenueBysupervisor_lst,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult SupervisorReportPDFDownload(string GrandTotal, string SelectedItems)
        {
            try
            {
                var myDetails = JObject.Parse(SelectedItems);
                var selectedStation = myDetails["SelectedStation"];
                var selectedLot = myDetails["SelectedLot"];
                var SelectedSupervisor = myDetails["SupervisorID"];
                string selectstation = ((Newtonsoft.Json.Linq.JValue)selectedStation).Value.ToString();
                string selectLot = ((Newtonsoft.Json.Linq.JValue)selectedLot).Value.ToString();
                string selectSupervisor = ((Newtonsoft.Json.Linq.JValue)SelectedSupervisor).Value.ToString();

                var fromDate = myDetails["FromDate"];
                var toDate = myDetails["ToDate"];
                var duration = myDetails["Duration"];
                string selectfromdate = ((Newtonsoft.Json.Linq.JValue)fromDate).Value.ToString();
                string selecttodate = ((Newtonsoft.Json.Linq.JValue)toDate).Value.ToString();
                string selectduration = ((Newtonsoft.Json.Linq.JValue)duration).Value.ToString();

                string FDate = "";
                string TDate = "";

                if (selectduration == "" || selectduration == null || selectduration == "0")
                {
                    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                }
                else
                {
                    FDate = DateTime.Parse(selectfromdate).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                }

                List<RevenueBySupervisor> result = (List<RevenueBySupervisor>)Session["SupervisorReportData"];
                DataTable dt = ToDataTable(result);

                string TotalClampCash = Convert.ToString(dt.Rows[0]["TotalClampCash"]);
                string TotalClampEPay = Convert.ToString(dt.Rows[0]["TotalClampEPay"]);
                string TotalCheckInsCash = Convert.ToString(dt.Rows[0]["TotalCheckInsCash"]);
                string TotalCheckInsEPay = Convert.ToString(dt.Rows[0]["TotalCheckInsEPay"]);
                string TotalPassesCash = Convert.ToString(dt.Rows[0]["TotalPassesCash"]);
                string TotalPassesEPay = Convert.ToString(dt.Rows[0]["TotalPassesEPay"]);
                string TotalNFCCash = Convert.ToString(dt.Rows[0]["TotalNFCCash"]);
                string TotalNFCEPay = Convert.ToString(dt.Rows[0]["TotalNFCEPay"]);
                string TotalDueCash = Convert.ToString(dt.Rows[0]["TotalDueCash"]);
                string TotalDueEPay = Convert.ToString(dt.Rows[0]["TotalDueEPay"]);

                string filename = GenerateReportBySupervisor(dt, GrandTotal, selectSupervisor, selectstation, selectLot, FDate, TDate, TotalClampCash, TotalClampEPay, TotalCheckInsCash, TotalCheckInsEPay, TotalPassesCash, TotalPassesEPay, TotalNFCCash, TotalNFCEPay, TotalDueCash, TotalDueEPay);
                FileStream sourceFile = null;

                if (filename != "")
                {
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("content-disposition", "attachment; filename=\"" + Path.GetFileName(filename) + "\"");
                    sourceFile = new FileStream(filename, FileMode.Open);
                    long FileSize;
                    FileSize = sourceFile.Length;
                    byte[] getContent = new byte[(int)FileSize];
                    sourceFile.Read(getContent, 0, (int)sourceFile.Length);
                    sourceFile.Close();
                    Response.Clear();
                    Response.BinaryWrite(getContent);
                    Response.End();
                }
                return Redirect("ReportByPasses");
            }
            catch (Exception ex)
            {
                return Redirect("ReportByPasses");
            }
        }
        public string GenerateReportBySupervisor(DataTable dt, string grandTotal, string supervisor, string selectstation, string selectLot, string FDate, string TDate, string TotalClampCash, string TotalClampEPay, string TotalCheckInsCash, string TotalCheckInsEPay, string TotalPassesCash, string TotalPassesEPay, string TotalNFCCash, string TotalNFCEPay, string TotalDueCash, string TotalDueEPay)
        {
            Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//15012021

            string filename, AttachmentName1 = "";
            string path = System.Web.HttpContext.Current.Server.MapPath("~/PDFReports");
            AttachmentName1 = "Report By Supervisor" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".pdf";
            filename = path + "/" + AttachmentName1;

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(PageSize.A4);
            rec.BackgroundColor = new BaseColor(System.Drawing.Color.Olive);
            Document doc = new Document(rec, 88f, 88f, 10f, 50f);
            var output = new FileStream(filename, FileMode.Create);
            doc.SetPageSize(iTextSharp.text.PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, output);
            writer.PageEvent = new Footer();
            doc.Open();

            BaseColor FontColour = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#353535"));
            BaseColor FontColourBlack = BaseColor.BLACK;
            iTextSharp.text.Font calibri8 = FontFactory.GetFont("Calibri", 10f, FontColour);
            iTextSharp.text.Font _bf_headingaddr = FontFactory.GetFont("Calibri", 8f, FontColour);
            iTextSharp.text.Font _bf_headingaddrbold = FontFactory.GetFont("Calibri", 8f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_heading1 = FontFactory.GetFont("Calibri", 10f, FontColour);
            iTextSharp.text.Font _bf_headingbold = FontFactory.GetFont("Calibri", 10f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingTitle = FontFactory.GetFont("Calibri", 12f, iTextSharp.text.Font.BOLD, FontColour);
            //iTextSharp.text.Font _bf_headingboldTable = FontFactory.GetFont("Calibri", 10f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingboldTable = FontFactory.GetFont("Calibri", 9f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingboldTableNrml = FontFactory.GetFont("Calibri", 9f, FontColour);


            Phrase phrase = null;
            PdfPCell cell = null;
            PdfPTable table = null;

            #region Logo 
            //Header Table start
            table = new PdfPTable(2);
            table.TotalWidth = 550f;
            table.LockedWidth = true;
            table.SetWidths(new float[] { 0.3f, 0.7f });
            table.DefaultCell.Border = Rectangle.NO_BORDER;

            //Company Logo           
            // cell = ImageCell("~/assets/images/Logo4.png", 70f, PdfPCell.ALIGN_CENTER);
            //cell = ImageCell("~/Images/" + companydata.CompanyLogo, 20f, PdfPCell.ALIGN_CENTER);
            if (companydata.CompanyLogo != null && companydata.CompanyLogo != "")
            {
                cell = ImageCell("~/Images/" + companydata.CompanyLogo, 20f, PdfPCell.ALIGN_CENTER);
            }
            else
            {
                cell = ImageCell("~/Images/default-logo.png", 20f, PdfPCell.ALIGN_CENTER);
            }
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            table.AddCell(cell);

            PdfPTable innertbl = new PdfPTable(1);
            innertbl.SetWidths(new float[] { 1.0f });
            PdfPCell innercell1 = null;
            phrase = new Phrase();
            //phrase.Add(new Chunk("                                                        Unit #3A, Plot No:847, Pacific Towers,", _bf_headingaddr));
            phrase.Add(new Chunk("                                                              " + companydata.Address1, _bf_headingaddr));
            innercell1 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell1.Border = 0;
            innercell1.Padding = 0;

            innertbl.SpacingBefore = 0f;
            innertbl.SpacingAfter = 2f;
            innertbl.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl.AddCell(innercell1);

            cell = new PdfPCell();
            cell.Border = 0;
            cell.Padding = 0;
            cell.AddElement(innertbl);

            PdfPTable innertbl2 = new PdfPTable(1);
            innertbl2.SetWidths(new float[] { 1.0f });
            PdfPCell innercell2 = null;
            phrase = new Phrase();
            //phrase.Add(new Chunk("                                                        Madhapur, Hyderabad-500081, TS", _bf_headingaddr));
            phrase.Add(new Chunk("                                                              " + companydata.Address2, _bf_headingaddr));
            innercell2 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell2.Border = 0;
            innercell2.Padding = 0;
            innertbl2.SpacingBefore = 0f;
            innertbl2.SpacingAfter = 2f;
            innertbl2.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl2.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl2.AddCell(innercell2);
            cell.AddElement(innertbl2);

            PdfPTable innertbl3 = new PdfPTable(1);
            innertbl3.SetWidths(new float[] { 1.0f });
            PdfPCell innercellTwo = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              GSTIN: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.GSTNumber, _bf_headingaddr));
            //phrase.Add(new Chunk("36AABCT3518Q1ZX", _bf_headingaddr));
            innercellTwo = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercellTwo.Border = 0;
            innercellTwo.Padding = 0;
            innertbl3.SpacingBefore = 0f;
            innertbl3.SpacingAfter = 2f;
            innertbl3.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl3.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl3.AddCell(innercellTwo);
            cell.AddElement(innertbl3);


            PdfPTable innertbl4 = new PdfPTable(1);
            innertbl4.SetWidths(new float[] { 1.0f });
            PdfPCell innercell5 = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              Ph: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.ContactNumber, _bf_headingaddr));
            //phrase.Add(new Chunk("+91 8143143143", _bf_headingaddr));
            innercell5 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell5.Border = 0;
            innercell5.Padding = 0;
            innertbl4.SpacingBefore = 0f;
            innertbl4.SpacingAfter = 2f;
            innertbl4.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl4.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl4.AddCell(innercell5);
            cell.AddElement(innertbl4);
            //table.AddCell(cell);

            PdfPTable innertbl5 = new PdfPTable(1);
            innertbl5.SetWidths(new float[] { 1.0f });
            PdfPCell innercell6 = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              Email: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.SupportEmailID, _bf_headingaddr));
            //phrase.Add(new Chunk("support@hmrl.com", _bf_headingaddr));
            innercell6 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell6.Border = 0;
            innercell6.Padding = 0;
            innertbl5.SpacingBefore = 0f;
            innertbl5.SpacingAfter = 0f;
            innertbl5.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl5.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl5.AddCell(innercell6);
            cell.AddElement(innertbl5);
            table.AddCell(cell);

            doc.Add(table);

            //Header Table end
            #endregion

            #region Title
            //Title table start
            PdfPTable tableTitle = new PdfPTable(1);
            tableTitle.DefaultCell.FixedHeight = 150f;
            tableTitle.TotalWidth = 550f;
            tableTitle.LockedWidth = true;
            tableTitle.SetWidths(new float[] { 1f });
            tableTitle.SpacingBefore = 10f;
            tableTitle.SpacingAfter = 3f;

            phrase = new Phrase();
            phrase.Add(new Chunk("REPORT BY SUPERVISOR", _bf_headingTitle));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
            cell.Border = 0;
            tableTitle.AddCell(cell);
            doc.Add(tableTitle);
            //Title table end
            #endregion

            #region Filter            
            PdfPTable tablefilter = new PdfPTable(1);
            tablefilter.TotalWidth = 550f;
            tablefilter.LockedWidth = true;
            tablefilter.SetWidths(new float[] { 1.0f });
            tablefilter.SpacingBefore = 3f;
            tablefilter.SpacingAfter = 3f;


            phrase = new Phrase();
            phrase.Add(new Chunk("Filtered By      :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(supervisor + ", " + selectstation + ", " + selectLot, _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tablefilter.AddCell(cell);
            doc.Add(tablefilter);
            #endregion

            #region time
            PdfPTable tabletime = new PdfPTable(1);
            tabletime.TotalWidth = 550f;
            tabletime.LockedWidth = true;
            tabletime.SetWidths(new float[] { 1.0f });
            tabletime.SpacingBefore = 3f;
            tabletime.SpacingAfter = 3f;

            phrase = new Phrase();
            phrase.Add(new Chunk("Report From   :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(FDate + " to " + TDate, _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tabletime.AddCell(cell);
            doc.Add(tabletime);

            PdfPTable tabletime1 = new PdfPTable(1);
            tabletime1.TotalWidth = 550f;
            tabletime1.LockedWidth = true;
            tabletime1.SetWidths(new float[] { 1.0f });
            tabletime1.SpacingBefore = 3f;
            tabletime1.SpacingAfter = 5f;
            phrase = new Phrase();
            phrase.Add(new Chunk("Generated On :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(DateTime.Now.ToString("dd/MM/yyyy HH:MM tt"), _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tabletime1.AddCell(cell);
            doc.Add(tabletime1);
            #endregion

            #region Data
            PdfPTable table2 = new PdfPTable(dt.Columns.Count - 10);
            table2.SetWidths(new float[] { 0.23f, 0.2f, 0.2f, 0.2f, 0.15f, 0.15f, 0.16f, 0.16f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f, 0.17f });
            table2.TotalWidth = doc.PageSize.Width - 40f;
            table2.LockedWidth = true;
            table2.SpacingBefore = 10f;
            // table2.SpacingAfter = 3f;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string cellText = Server.HtmlDecode(dt.Columns[i].ColumnName);
                if (cellText != "TotalClampCash" && cellText != "TotalClampEPay" && cellText != "TotalCheckInsCash" && cellText != "TotalCheckInsEPay"
                        && cellText != "TotalPassesCash" && cellText != "TotalPassesEPay" && cellText != "TotalNFCCash" && cellText != "TotalNFCEPay"
                        && cellText != "TotalDueCash" && cellText != "TotalDueEPay")
                {
                    PdfPCell cell2 = new PdfPCell();
                    if (cellText == "ClampCash" || cellText == "ClampEPay" || cellText == "CheckInsCash" || cellText == "CheckInsEPay"
                                || cellText == "PassesCash" || cellText == "PassesEPay" || cellText == "NFCCash" || cellText == "NFCEPay" || cellText == "DueCash"
                                || cellText == "DueEPay" || cellText == "Amount")
                    {
                        if (cellText == "ClampCash")
                        {
                            cell2.Phrase = new Phrase("Clamp Cash", _bf_headingboldTable);
                        }
                        if (cellText == "ClampEPay")
                        {
                            cell2.Phrase = new Phrase("Clamp EPay", _bf_headingboldTable);
                        }
                        if (cellText == "CheckInsCash")
                        {
                            cell2.Phrase = new Phrase("Check Ins-Cash", _bf_headingboldTable);
                        }
                        else if (cellText == "CheckInsEPay")
                        {
                            cell2.Phrase = new Phrase("Check Ins-EPay", _bf_headingboldTable);
                        }
                        else if (cellText == "PassesCash")
                        {
                            cell2.Phrase = new Phrase("Passes-Cash", _bf_headingboldTable);
                        }
                        else if (cellText == "PassesEPay")
                        {
                            cell2.Phrase = new Phrase("Passes-EPay", _bf_headingboldTable);
                        }
                        else if (cellText == "NFCCash")
                        {
                            cell2.Phrase = new Phrase("NFC Cash", _bf_headingboldTable);
                        }
                        else if (cellText == "NFCEPay")
                        {
                            cell2.Phrase = new Phrase("NFC EPay", _bf_headingboldTable);
                        }
                        else if (cellText == "DueCash")
                        {
                            cell2.Phrase = new Phrase("Due Cash", _bf_headingboldTable);
                        }
                        else if (cellText == "DueEPay")
                        {
                            cell2.Phrase = new Phrase("Due EPay", _bf_headingboldTable);
                        }
                        else
                        {
                            cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                        }
                        cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                        cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    }
                    else
                    {
                        if (cellText == "ParkingLot")
                        {
                            cell2.Phrase = new Phrase("Parking Lot", _bf_headingboldTable);
                        }
                        else
                        {
                            cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                        }
                        //cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                        cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                        cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    }
                    table2.AddCell(cell2);
                }
            }

            //writing table Data  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string cellText = Server.HtmlDecode(dt.Columns[j].ColumnName);
                    if (cellText != "TotalClampCash" && cellText != "TotalClampEPay" && cellText != "TotalCheckInsCash" && cellText != "TotalCheckInsEPay"
                            && cellText != "TotalPassesCash" && cellText != "TotalPassesEPay" && cellText != "TotalNFCCash" && cellText != "TotalNFCEPay"
                            && cellText != "TotalDueCash" && cellText != "TotalDueEPay")
                    {
                        PdfPCell cell2 = new PdfPCell();
                        if (cellText == "ClampCash" || cellText == "ClampEPay" || cellText == "CheckInsCash" || cellText == "CheckInsEPay"
                                    || cellText == "PassesCash" || cellText == "PassesEPay" || cellText == "NFCCash" || cellText == "NFCEPay"
                                    || cellText == "DueCash" || cellText == "DueEPay" || cellText == "Amount")
                        {
                            cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                            cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                        }

                        else
                        {
                            cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                            cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                        }
                        table2.AddCell(cell2);
                    }
                }
            }
            //GRAND TOTAL TABLE
            //PdfPTable grandTable = new PdfPTable(dt.Columns.Count - 6);
            //grandTable.TotalWidth = doc.PageSize.Width - 40f;
            //grandTable.LockedWidth = true;
            //PdfPCell cell3 = new PdfPCell();
            //string totalstring = string.Concat("Total                  " + TotalClampCash + "            " + TotalClampEPay + "       " + TotalCheckInsCash + "      " + TotalCheckInsEPay + "       " + TotalPassesCash + "       " + TotalPassesEPay + "  " + grandTotal);
            //cell3.Phrase = new Phrase(totalstring, _bf_headingboldTable);
            //cell3.Colspan = dt.Columns.Count;
            //cell3.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cell3.BorderWidthTop = 0;
            //grandTable.AddCell(cell3);

            PdfPTable grandTable = new PdfPTable(dt.Columns.Count - 10);
            //grandTable.SetWidths(new float[] { 0.21f, 0.17f, 0.18f, 0.18f, 0.21f, 0.21f, 0.14f, 0.14f, 0.14f, 0.14f, 0.17f, 0.17f, 0.17f, 0.17f, 0.16f });
            grandTable.SetWidths(new float[] { 0.23f, 0.2f, 0.2f, 0.2f, 0.15f, 0.15f, 0.16f, 0.16f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f, 0.17f });
            grandTable.TotalWidth = doc.PageSize.Width - 40f;
            grandTable.LockedWidth = true;

            PdfPCell cellt = new PdfPCell(new Phrase(" ", _bf_headingboldTable));
            cellt.Colspan = 3;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cellt = new PdfPCell();
            cellt.Phrase = new Phrase("Total ", _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthLeft = 0;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            if (TotalClampCash == "")
            {
                TotalClampCash = "0";
            }
            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalClampCash, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            if (TotalClampEPay == "")
            {
                TotalClampEPay = "0";
            }
            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalClampEPay, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            if (TotalCheckInsCash == "")
            {
                TotalCheckInsCash = "0";
            }
            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalCheckInsCash, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            if (TotalCheckInsEPay == "")
            {
                TotalCheckInsEPay = "0";
            }
            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalCheckInsEPay, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            if (TotalPassesCash == "")
            {
                TotalPassesCash = "0";
            }
            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalPassesCash, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            if (TotalPassesEPay == "")
            {
                TotalPassesEPay = "0";
            }
            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalPassesEPay, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            if (TotalNFCCash == "")
            {
                TotalNFCCash = "0";
            }
            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalNFCCash, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            if (TotalNFCEPay == "")
            {
                TotalNFCEPay = "0";
            }
            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalNFCEPay, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            grandTable.AddCell(cellt);

            if (TotalDueCash == "")
            {
                TotalDueCash = "0";
            }
            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalDueCash, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            if (TotalDueEPay == "")
            {
                TotalDueEPay = "0";
            }
            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalDueEPay, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);


            cell = new PdfPCell();
            cellt.Phrase = new Phrase(grandTotal, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthTop = 0;
            cellt.BorderWidthRight = 1;
            grandTable.AddCell(cellt);

            //GRNAD TOTAL TABLE

            ////GST Table
            //PdfPTable gstTable = new PdfPTable(dt.Columns.Count - 10);
            //gstTable.SetWidths(new float[] { 0.23f, 0.2f, 0.2f, 0.2f, 0.15f, 0.15f, 0.16f, 0.16f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f, 0.15f, 0.17f });
            //gstTable.TotalWidth = doc.PageSize.Width - 40f;
            //gstTable.LockedWidth = true;

            //PdfPCell cellGST = new PdfPCell(new Phrase(" ", _bf_headingboldTable));
            //cellGST.BorderWidthRight = 0;
            //cellGST.BorderWidthTop = 0;
            //cellGST.Colspan = 13;
            //gstTable.AddCell(cellGST);

            //cellGST = new PdfPCell();
            //cellGST.Phrase = new Phrase("GST", _bf_headingboldTable);
            //cellGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cellGST.BorderWidthLeft = 0;
            //cellGST.BorderWidthRight = 0;
            //cellGST.BorderWidthTop = 0;
            //gstTable.AddCell(cellGST);

            //cell = new PdfPCell();
            //cellGST.Phrase = new Phrase(GST, _bf_headingboldTable);
            //cellGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cellGST.BorderWidthTop = 0;
            //cellGST.BorderWidthRight = 1;
            //gstTable.AddCell(cellGST);
            ////GST Table

            doc.Add(table2);
            doc.Add(grandTable);
           // doc.Add(gstTable);
            #endregion

            doc.Close();
            byte[] result = ms.ToArray();
            return filename;

        }
        public ActionResult SupervisorReportExcelDownload(string GrandTotal, string SelectedItems)
        {
            try
            {
                Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//18012021

                var myDetails = JObject.Parse(SelectedItems);
                var fromDate = myDetails["FromDate"];
                var toDate = myDetails["ToDate"];
                var duration = myDetails["Duration"];
                string selectfromdate = ((Newtonsoft.Json.Linq.JValue)fromDate).Value.ToString();
                string selecttodate = ((Newtonsoft.Json.Linq.JValue)toDate).Value.ToString();
                string selectduration = ((Newtonsoft.Json.Linq.JValue)duration).Value.ToString();

                string FDate = "";
                string TDate = "";

                if (selectduration == "" || selectduration == null || selectduration == "0")
                {
                    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                }
                else
                {
                    FDate = DateTime.Parse(selectfromdate).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                }

                List<RevenueBySupervisor> result = (List<RevenueBySupervisor>)Session["SupervisorReportData"];
                DataTable dt = ToDataTable(result);

                string TotalClampCash = Convert.ToString(dt.Rows[0]["TotalClampCash"]);
                string TotalClampEPay = Convert.ToString(dt.Rows[0]["TotalClampEPay"]);
                string TotalCheckInsCash = Convert.ToString(dt.Rows[0]["TotalCheckInsCash"]);
                string TotalCheckInsEPay = Convert.ToString(dt.Rows[0]["TotalCheckInsEPay"]);
                string TotalPassesCash = Convert.ToString(dt.Rows[0]["TotalPassesCash"]);
                string TotalPassesEPay = Convert.ToString(dt.Rows[0]["TotalPassesEPay"]);
                string TotalNFCCash = Convert.ToString(dt.Rows[0]["TotalNFCCash"]);
                string TotalNFCEPay = Convert.ToString(dt.Rows[0]["TotalNFCEPay"]);
                string TotalDueCash = Convert.ToString(dt.Rows[0]["TotalDueCash"]);
                string TotalDueEPay = Convert.ToString(dt.Rows[0]["TotalDueEPay"]);

                if (TotalClampCash == "")
                {
                    TotalClampCash = "0.00";
                }
                if (TotalClampEPay == "")
                {
                    TotalClampEPay = "0.00";
                }
                if (TotalCheckInsCash == "")
                {
                    TotalCheckInsCash = "0.00";
                }
                if (TotalCheckInsEPay == "")
                {
                    TotalCheckInsEPay = "0.00";
                }
                if (TotalPassesCash == "")
                {
                    TotalPassesCash = "0.00";
                }
                if (TotalPassesEPay == "")
                {
                    TotalPassesEPay = "0.00";
                }
                if (TotalNFCCash == "")
                {
                    TotalNFCCash = "0.00";
                }
                if (TotalNFCEPay == "")
                {
                    TotalNFCEPay = "0.00";
                }
                if (TotalDueCash == "")
                {
                    TotalDueCash = "0.00";
                }
                if (TotalDueEPay == "")
                {
                    TotalDueEPay = "0.00";
                }

                string path = System.Web.HttpContext.Current.Server.MapPath("~/ExcelReports");
                string filename = "RevenueBySupervisorReport_" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".xlsx";
                string file_path = path + "/" + filename;

                if (dt.Rows.Count > 0)
                {
                    #region New Code
                    using (XLWorkbook wb = new XLWorkbook())
                    {

                        wb.Worksheets.Add("RevenueReportBySupervisor", 0);
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Value = companydata.AccountName;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.FontSize = 12;
                        wb.Worksheets.Worksheet(0).Cell(1, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(2, 1).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(2, 20).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(3, 1).Value = "Revenue By Supervisor Report";
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(3, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 1).Value = "From : " + FDate + " To : " + TDate;
                        wb.Worksheets.Worksheet(0).Cell(4, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 5).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 6).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 7).Value = "Cur.Date : " + DateTime.Now.ToString("dd/MM/yyy").Split(' ')[0];
                        wb.Worksheets.Worksheet(0).Cell(4, 7).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(6, 1).Value = "Supervisor";
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "Operator";
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Station";
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 4).Value = "Parking Lot";
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 5).Value = "Clamp-Cash";
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 6).Value = "Clamp-EPay";
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 7).Value = "Check Ins-Cash";
                        wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 8).Value = "Check Ins-EPay";
                        wb.Worksheets.Worksheet(0).Cell(6, 8).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 8).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 9).Value = "Passes-Cash";
                        wb.Worksheets.Worksheet(0).Cell(6, 9).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 9).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 10).Value = "Passes-EPay";
                        wb.Worksheets.Worksheet(0).Cell(6, 10).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 10).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 11).Value = "NFC Cash";
                        wb.Worksheets.Worksheet(0).Cell(6, 11).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 11).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 12).Value = "NFC EPay";
                        wb.Worksheets.Worksheet(0).Cell(6, 12).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 12).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 13).Value = "Due Cash";
                        wb.Worksheets.Worksheet(0).Cell(6, 13).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 13).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 13).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 14).Value = "Due EPay";
                        wb.Worksheets.Worksheet(0).Cell(6, 14).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 14).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 14).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 15).Value = "Amount";
                        wb.Worksheets.Worksheet(0).Cell(6, 15).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 15).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 15).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        int row = 7;
                        int column_num = 0;

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Value = Convert.ToString(dt.Rows[i]["Supervisor"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Value = Convert.ToString(dt.Rows[i]["Operator"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Value = Convert.ToString(dt.Rows[i]["Station"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Value = Convert.ToString(dt.Rows[i]["ParkingLot"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Value = Convert.ToString(dt.Rows[i]["ClampCash"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Value = Convert.ToString(dt.Rows[i]["ClampEPay"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Value = Convert.ToString(dt.Rows[i]["CheckInsCash"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 8).Value = Convert.ToString(dt.Rows[i]["CheckInsEPay"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 8).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 9).Value = Convert.ToString(dt.Rows[i]["PassesCash"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 9).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 10).Value = Convert.ToString(dt.Rows[i]["PassesEPay"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 10).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 11).Value = Convert.ToString(dt.Rows[i]["NFCCash"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 11).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 12).Value = Convert.ToString(dt.Rows[i]["NFCEPay"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 12).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 13).Value = Convert.ToString(dt.Rows[i]["DueCash"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 13).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 13).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 14).Value = Convert.ToString(dt.Rows[i]["DueEPay"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 14).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 14).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 15).Value = Convert.ToString(dt.Rows[i]["Amount"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 15).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 15).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            row = row + 1;
                        }

                        //row = row + 1;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Value = "Total : ";
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 5).Value = TotalClampCash;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 6).Value = TotalClampEPay;
                        wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 7).Value = TotalCheckInsCash;
                        wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 8).Value = TotalCheckInsEPay;
                        wb.Worksheets.Worksheet(0).Cell(row, 8).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 8).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 9).Value = TotalPassesCash;
                        wb.Worksheets.Worksheet(0).Cell(row, 9).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 9).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 10).Value = TotalPassesEPay;
                        wb.Worksheets.Worksheet(0).Cell(row, 10).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 10).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 11).Value = TotalNFCCash;
                        wb.Worksheets.Worksheet(0).Cell(row, 11).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 11).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 12).Value = TotalNFCEPay;
                        wb.Worksheets.Worksheet(0).Cell(row, 12).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 12).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 13).Value = TotalDueCash;
                        wb.Worksheets.Worksheet(0).Cell(row, 13).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 13).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 14).Value = TotalDueEPay;
                        wb.Worksheets.Worksheet(0).Cell(row, 14).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 14).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 15).Value = GrandTotal;
                        wb.Worksheets.Worksheet(0).Cell(row, 15).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 15).Style.Font.FontSize = 11;

                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 14).Value = "GST";
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 14).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 14).Style.Font.FontSize = 11;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 14).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 15).Value = GST;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 15).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 15).Style.Font.FontSize = 11;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 15).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;


                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                        using (MemoryStream MyMemoryStream = new MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                    #endregion

                }
                return Redirect("ReportByPasses");
            }
            catch (Exception ex)
            {
                return Redirect("ReportByPasses");
            }
        }
        #endregion

        #region Violation Report
        public ActionResult ReportByViolation()
        {
            ViewBag.Menu = "RevenueReports";
            return View();
        }
        [HttpPost]
        public JsonResult GetReportByViolation(SearchFilters violationFilterData)
        {
            try
            {
                if (violationFilterData.FromTime != null || violationFilterData.Duration == "Today")
                {
                    string fDate1 = Convert.ToString(violationFilterData.FromDate).Split(' ')[0];
                    DateTime fTotalDate = Convert.ToDateTime(fDate1 + " " + violationFilterData.FromTime + ":00 " + violationFilterData.FromMeridiem);

                    string tDate1 = Convert.ToString(violationFilterData.ToDate).Split(' ')[0];
                    DateTime tTotalDate = Convert.ToDateTime(tDate1 + " " + violationFilterData.ToTime + ":00 " + violationFilterData.ToMeridiem);

                    violationFilterData.FromDate = fTotalDate;
                    violationFilterData.ToDate = tTotalDate;
                }

                List<RevenueByViolation> revenueByviolation_lst = revenueReportsDAL_obj.GetReportByViolation(violationFilterData);
                Session["ViolationReportData"] = revenueByviolation_lst;
                return new JsonResult()
                {
                    Data = revenueByviolation_lst,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult ViolationReportPDFDownload(string GrandTotal, string SelectedItems)
        {
            try
            {
                var myDetails = JObject.Parse(SelectedItems);
                var selectedStation = myDetails["SelectedStation"];
                var selectedLot = myDetails["SelectedLot"];
                var SelectedVehicle = myDetails["SelectedVehicle"];
                string selectstation = ((Newtonsoft.Json.Linq.JValue)selectedStation).Value.ToString();
                string selectLot = ((Newtonsoft.Json.Linq.JValue)selectedLot).Value.ToString();
                string selectVehicle = ((Newtonsoft.Json.Linq.JValue)SelectedVehicle).Value.ToString();

                var fromDate = myDetails["FromDate"];
                var toDate = myDetails["ToDate"];
                var duration = myDetails["Duration"];
                string selectfromdate = ((Newtonsoft.Json.Linq.JValue)fromDate).Value.ToString();
                string selecttodate = ((Newtonsoft.Json.Linq.JValue)toDate).Value.ToString();
                string selectduration = ((Newtonsoft.Json.Linq.JValue)duration).Value.ToString();

                string FDate = "";
                string TDate = "";

                if (selectduration == "" || selectduration == null || selectduration == "0")
                {
                    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                }
                else
                {
                    FDate = DateTime.Parse(selectfromdate).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                }

                //string FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                //string TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");

                List<RevenueByViolation> result = (List<RevenueByViolation>)Session["ViolationReportData"];
                DataTable dt = ToDataTable(result);
                string TotalClamps = Convert.ToString(dt.Rows[0]["TotalClamps"]);
                string TotalCash = Convert.ToString(dt.Rows[0]["TotalCash"]);
                string TotalEPay = Convert.ToString(dt.Rows[0]["TotalEPay"]);
                string filename = GenerateReportByViolation(dt, GrandTotal, selectstation, selectLot, selectVehicle, FDate, TDate, TotalCash, TotalEPay, TotalClamps);
                FileStream sourceFile = null;

                if (filename != "")
                {
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("content-disposition", "attachment; filename=\"" + Path.GetFileName(filename) + "\"");
                    sourceFile = new FileStream(filename, FileMode.Open);
                    long FileSize;
                    FileSize = sourceFile.Length;
                    byte[] getContent = new byte[(int)FileSize];
                    sourceFile.Read(getContent, 0, (int)sourceFile.Length);
                    sourceFile.Close();
                    Response.Clear();
                    Response.BinaryWrite(getContent);
                    Response.End();
                }
                return Redirect("ReportByViolation");
            }
            catch (Exception ex)
            {
                return Redirect("ReportByViolation");
            }
        }
        public string GenerateReportByViolation(DataTable dt, string grandTotal, string selectstation, string selectLot, string selectVehicle, string FDate, string TDate, string TotalCash, string TotalEPay, string TotalClamps)
        {
            Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//15012021

            string filename, AttachmentName1 = "";
            string path = System.Web.HttpContext.Current.Server.MapPath("~/PDFReports");
            AttachmentName1 = "Report By Violation" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".pdf";
            filename = path + "/" + AttachmentName1;

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(PageSize.A4);
            rec.BackgroundColor = new BaseColor(System.Drawing.Color.Olive);
            Document doc = new Document(rec, 88f, 88f, 10f, 50f);
            var output = new FileStream(filename, FileMode.Create);
            doc.SetPageSize(iTextSharp.text.PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, output);
            writer.PageEvent = new Footer();
            doc.Open();

            BaseColor FontColour = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#353535"));
            BaseColor FontColourBlack = BaseColor.BLACK;
            iTextSharp.text.Font calibri8 = FontFactory.GetFont("Calibri", 10f, FontColour);
            iTextSharp.text.Font _bf_headingaddr = FontFactory.GetFont("Calibri", 8f, FontColour);
            iTextSharp.text.Font _bf_headingaddrbold = FontFactory.GetFont("Calibri", 8f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_heading1 = FontFactory.GetFont("Calibri", 10f, FontColour);
            iTextSharp.text.Font _bf_headingbold = FontFactory.GetFont("Calibri", 10f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingTitle = FontFactory.GetFont("Calibri", 12f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingboldTable = FontFactory.GetFont("Calibri", 10f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingboldTableNrml = FontFactory.GetFont("Calibri", 9f, FontColour);


            Phrase phrase = null;
            PdfPCell cell = null;
            PdfPTable table = null;

            #region Logo 
            //Header Table start
            table = new PdfPTable(2);
            table.TotalWidth = 550f;
            table.LockedWidth = true;
            table.SetWidths(new float[] { 0.3f, 0.7f });
            table.DefaultCell.Border = Rectangle.NO_BORDER;

            //Company Logo           
            // cell = ImageCell("~/assets/images/Logo4.png", 70f, PdfPCell.ALIGN_CENTER);
            //cell = ImageCell("~/Images/" + companydata.CompanyLogo, 20f, PdfPCell.ALIGN_CENTER);
            if (companydata.CompanyLogo != null && companydata.CompanyLogo != "")
            {
                cell = ImageCell("~/Images/" + companydata.CompanyLogo, 20f, PdfPCell.ALIGN_CENTER);
            }
            else
            {
                cell = ImageCell("~/Images/default-logo.png", 20f, PdfPCell.ALIGN_CENTER);
            }
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            table.AddCell(cell);

            PdfPTable innertbl = new PdfPTable(1);
            innertbl.SetWidths(new float[] { 1.0f });
            PdfPCell innercell1 = null;
            phrase = new Phrase();
            //phrase.Add(new Chunk("                                                        Unit #3A, Plot No:847, Pacific Towers,", _bf_headingaddr));
            phrase.Add(new Chunk("                                                              " + companydata.Address1, _bf_headingaddr));
            innercell1 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell1.Border = 0;
            innercell1.Padding = 0;

            innertbl.SpacingBefore = 0f;
            innertbl.SpacingAfter = 2f;
            innertbl.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl.AddCell(innercell1);

            cell = new PdfPCell();
            cell.Border = 0;
            cell.Padding = 0;
            cell.AddElement(innertbl);

            PdfPTable innertbl2 = new PdfPTable(1);
            innertbl2.SetWidths(new float[] { 1.0f });
            PdfPCell innercell2 = null;
            phrase = new Phrase();
            //phrase.Add(new Chunk("                                                        Madhapur, Hyderabad-500081, TS", _bf_headingaddr));
            phrase.Add(new Chunk("                                                              " + companydata.Address2, _bf_headingaddr));
            innercell2 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell2.Border = 0;
            innercell2.Padding = 0;
            innertbl2.SpacingBefore = 0f;
            innertbl2.SpacingAfter = 2f;
            innertbl2.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl2.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl2.AddCell(innercell2);
            cell.AddElement(innertbl2);

            PdfPTable innertbl3 = new PdfPTable(1);
            innertbl3.SetWidths(new float[] { 1.0f });
            PdfPCell innercellTwo = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              GSTIN: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.GSTNumber, _bf_headingaddr));
            //phrase.Add(new Chunk("36AABCT3518Q1ZX", _bf_headingaddr));
            innercellTwo = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercellTwo.Border = 0;
            innercellTwo.Padding = 0;
            innertbl3.SpacingBefore = 0f;
            innertbl3.SpacingAfter = 2f;
            innertbl3.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl3.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl3.AddCell(innercellTwo);
            cell.AddElement(innertbl3);


            PdfPTable innertbl4 = new PdfPTable(1);
            innertbl4.SetWidths(new float[] { 1.0f });
            PdfPCell innercell5 = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              Ph: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.ContactNumber, _bf_headingaddr));
            //phrase.Add(new Chunk("+91 8143143143", _bf_headingaddr));
            innercell5 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell5.Border = 0;
            innercell5.Padding = 0;
            innertbl4.SpacingBefore = 0f;
            innertbl4.SpacingAfter = 2f;
            innertbl4.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl4.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl4.AddCell(innercell5);
            cell.AddElement(innertbl4);
            //table.AddCell(cell);

            PdfPTable innertbl5 = new PdfPTable(1);
            innertbl5.SetWidths(new float[] { 1.0f });
            PdfPCell innercell6 = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              Email: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.SupportEmailID, _bf_headingaddr));
            //phrase.Add(new Chunk("support@hmrl.com", _bf_headingaddr));
            innercell6 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell6.Border = 0;
            innercell6.Padding = 0;
            innertbl5.SpacingBefore = 0f;
            innertbl5.SpacingAfter = 0f;
            innertbl5.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl5.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl5.AddCell(innercell6);
            cell.AddElement(innertbl5);
            table.AddCell(cell);

            doc.Add(table);

            //Header Table end
            #endregion

            #region Title
            //Title table start
            PdfPTable tableTitle = new PdfPTable(1);
            tableTitle.DefaultCell.FixedHeight = 150f;
            tableTitle.TotalWidth = 550f;
            tableTitle.LockedWidth = true;
            tableTitle.SetWidths(new float[] { 1f });
            tableTitle.SpacingBefore = 10f;
            tableTitle.SpacingAfter = 3f;

            phrase = new Phrase();
            phrase.Add(new Chunk("REPORT BY VIOLATION", _bf_headingTitle));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
            cell.Border = 0;
            tableTitle.AddCell(cell);
            doc.Add(tableTitle);
            //Title table end
            #endregion

            #region Filter            
            PdfPTable tablefilter = new PdfPTable(1);
            tablefilter.TotalWidth = 550f;
            tablefilter.LockedWidth = true;
            tablefilter.SetWidths(new float[] { 1.0f });
            tablefilter.SpacingBefore = 3f;
            tablefilter.SpacingAfter = 3f;


            phrase = new Phrase();
            phrase.Add(new Chunk("Filtered By      :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(selectstation + ", " + selectLot + ", " + selectVehicle, _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tablefilter.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk(selectstation, _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.BorderColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#dbdbdb"));
            //tablefilter.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk(selectLot, _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.BorderColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#dbdbdb"));
            //tablefilter.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk(selectVehicle, _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.BorderColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#dbdbdb"));
            //tablefilter.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk("", _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.Border = 0;
            //tablefilter.AddCell(cell);

            doc.Add(tablefilter);
            #endregion

            #region time
            PdfPTable tabletime = new PdfPTable(1);
            tabletime.TotalWidth = 550f;
            tabletime.LockedWidth = true;
            tabletime.SetWidths(new float[] { 1.0f });
            tabletime.SpacingBefore = 3f;
            tabletime.SpacingAfter = 3f;

            phrase = new Phrase();
            phrase.Add(new Chunk("Report From   :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(FDate + " to " + TDate, _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tabletime.AddCell(cell);
            doc.Add(tabletime);

            PdfPTable tabletime1 = new PdfPTable(1);
            tabletime1.TotalWidth = 550f;
            tabletime1.LockedWidth = true;
            tabletime1.SetWidths(new float[] { 1.0f });
            tabletime1.SpacingBefore = 3f;
            tabletime1.SpacingAfter = 5f;
            phrase = new Phrase();
            phrase.Add(new Chunk("Generated On :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(DateTime.Now.ToString("dd/MM/yyyy HH:MM tt"), _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tabletime1.AddCell(cell);
            doc.Add(tabletime1);
            #endregion

            #region Data
            PdfPTable table2 = new PdfPTable(dt.Columns.Count - 3);
            table2.SetWidths(new float[] { 0.25f, 0.2f, 0.12f, 0.15f, 0.15f, 0.15f, 0.15f });
            table2.TotalWidth = doc.PageSize.Width - 40f;
            table2.LockedWidth = true;
            table2.SpacingBefore = 10f;
            // table2.SpacingAfter = 3f;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string cellText = Server.HtmlDecode(dt.Columns[i].ColumnName);
                if (cellText != "TotalCash" && cellText != "TotalEPay" && cellText != "TotalClamps")
                {
                    PdfPCell cell2 = new PdfPCell();
                    if (cellText == "Cash" || cellText == "EPay" || cellText == "Amount")
                    {
                        cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                        cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                        cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    }
                    else
                    {
                        if (cellText == "ParkingLot")
                        {
                            cell2.Phrase = new Phrase("Parking Lot", _bf_headingboldTable);
                            //cell2.Phrase = new Phrase("Lot Code", _bf_headingboldTable);
                        }
                        else
                        {
                            cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                        }
                        cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                        cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    }
                    table2.AddCell(cell2);
                }
            }

            //writing table Data  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string cellText = Server.HtmlDecode(dt.Columns[j].ColumnName);
                    if (cellText != "TotalCash" && cellText != "TotalEPay" && cellText != "TotalClamps")
                    {
                        PdfPCell cell2 = new PdfPCell();
                        if (cellText == "Cash" || cellText == "EPay" || cellText == "Amount")
                        {
                            cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                            cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                        }
                        else
                        {
                            cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                            cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                        }
                        table2.AddCell(cell2);
                    }
                }
            }
            //GRAND TOTAL TABLE
            //PdfPTable grandTable = new PdfPTable(dt.Columns.Count - 2);
            //grandTable.TotalWidth = doc.PageSize.Width - 40f;
            //grandTable.LockedWidth = true;
            //PdfPCell cell3 = new PdfPCell();
            //string totalstring = string.Concat("Total                   " + TotalCash + "                 " + TotalEPay + "              " + grandTotal);
            //cell3.Phrase = new Phrase(totalstring, _bf_headingboldTable);
            //cell3.Colspan = dt.Columns.Count;
            //cell3.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cell3.BorderWidthTop = 0;
            //grandTable.AddCell(cell3);

            PdfPTable grandTable = new PdfPTable(dt.Columns.Count - 3);
            grandTable.SetWidths(new float[] { 0.25f, 0.2f, 0.12f, 0.15f, 0.15f, 0.15f, 0.15f });
            grandTable.TotalWidth = doc.PageSize.Width - 40f;
            grandTable.LockedWidth = true;

            PdfPCell cellt = new PdfPCell(new Phrase(" ", _bf_headingboldTable));
            cellt.Colspan = 2;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cellt = new PdfPCell();
            cellt.Phrase = new Phrase("Total ", _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
            cellt.BorderWidthLeft = 0;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalClamps, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalCash, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalEPay, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(grandTotal, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthTop = 0;
            cellt.BorderWidthRight = 1;
            grandTable.AddCell(cellt);

            //GRNAD TOTAL TABLE

            ////GST Table
            //PdfPTable gstTable = new PdfPTable(dt.Columns.Count - 3);
            //gstTable.SetWidths(new float[] { 0.25f, 0.2f, 0.12f, 0.15f, 0.15f, 0.15f, 0.15f });
            //gstTable.TotalWidth = doc.PageSize.Width - 40f;
            //gstTable.LockedWidth = true;

            //PdfPCell cellGST = new PdfPCell(new Phrase(" ", _bf_headingboldTable));
            //cellGST.BorderWidthRight = 0;
            //cellGST.BorderWidthTop = 0;
            //cellGST.Colspan = 5;
            //gstTable.AddCell(cellGST);

            //cellGST = new PdfPCell();
            //cellGST.Phrase = new Phrase("GST", _bf_headingboldTable);
            //cellGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cellGST.BorderWidthLeft = 0;
            //cellGST.BorderWidthRight = 0;
            //cellGST.BorderWidthTop = 0;
            //gstTable.AddCell(cellGST);

            //cell = new PdfPCell();
            //cellGST.Phrase = new Phrase(GST, _bf_headingboldTable);
            //cellGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cellGST.BorderWidthTop = 0;
            //cellGST.BorderWidthRight = 1;
            //gstTable.AddCell(cellGST);
            ////GST Table

            doc.Add(table2);
            doc.Add(grandTable);
           // doc.Add(gstTable);
            #endregion

            doc.Close();
            byte[] result = ms.ToArray();
            return filename;

        }
        public ActionResult ViolationReportExcelDownload(string GrandTotal, string SelectedItems)
        {
            try
            {
                Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//18012021

                var myDetails = JObject.Parse(SelectedItems);
                var fromDate = myDetails["FromDate"];
                var toDate = myDetails["ToDate"];
                var duration = myDetails["Duration"];
                string selectfromdate = ((Newtonsoft.Json.Linq.JValue)fromDate).Value.ToString();
                string selecttodate = ((Newtonsoft.Json.Linq.JValue)toDate).Value.ToString();
                string selectduration = ((Newtonsoft.Json.Linq.JValue)duration).Value.ToString();

                string FDate = "";
                string TDate = "";

                if (selectduration == "" || selectduration == null || selectduration == "0")
                {
                    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                }
                else
                {
                    FDate = DateTime.Parse(selectfromdate).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                }


                //string FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                //string TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");

                List<RevenueByViolation> result = (List<RevenueByViolation>)Session["ViolationReportData"];
                DataTable dt = ToDataTable(result);

                string TotalClamps = Convert.ToString(dt.Rows[0]["TotalClamps"]);
                string TotalCash = Convert.ToString(dt.Rows[0]["TotalCash"]);
                string TotalEPay = Convert.ToString(dt.Rows[0]["TotalEPay"]);

                string path = System.Web.HttpContext.Current.Server.MapPath("~/ExcelReports");
                string filename = "RevenueByViolationReport_" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".xlsx";
                string file_path = path + "/" + filename;

                if (dt.Rows.Count > 0)
                {
                    #region New Code
                    using (XLWorkbook wb = new XLWorkbook())
                    {

                        wb.Worksheets.Add("RevenueReportByViolation", 0);
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Value = companydata.AccountName;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.FontSize = 12;
                        wb.Worksheets.Worksheet(0).Cell(1, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(2, 1).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(2, 20).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(3, 1).Value = "Revenue By Violation Report";
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(3, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 1).Value = "From : " + FDate + " To : " + TDate;
                        wb.Worksheets.Worksheet(0).Cell(4, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 5).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 6).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 5).Value = "Cur.Date : " + DateTime.Now.ToString("dd/MM/yyy").Split(' ')[0];
                        wb.Worksheets.Worksheet(0).Cell(4, 5).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(6, 1).Value = "Reason";
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "Station";
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Parking Lot";
                        //  wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Lot Code";
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.FontSize = 11;

                        //wb.Worksheets.Worksheet(0).Cell(6, 4).Value = "Clamp Fee";
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Value = "Clamps";
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        wb.Worksheets.Worksheet(0).Cell(6, 5).Value = "Cash";
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 6).Value = "EPay";
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 7).Value = "Amount";
                        wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        int row = 7;
                        int column_num = 0;


                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Value = Convert.ToString(dt.Rows[i]["Reason"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Value = Convert.ToString(dt.Rows[i]["Station"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Value = Convert.ToString(dt.Rows[i]["ParkingLot"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Value = Convert.ToString(dt.Rows[i]["Clamps"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Value = Convert.ToString(dt.Rows[i]["Cash"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Value = Convert.ToString(dt.Rows[i]["EPay"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Value = Convert.ToString(dt.Rows[i]["Amount"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            row = row + 1;
                        }

                        // row = row + 1;
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Value = "Total : ";
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 4).Value = TotalClamps;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        wb.Worksheets.Worksheet(0).Cell(row, 5).Value = TotalCash;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(row, 6).Value = TotalEPay;
                        wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(row, 7).Value = GrandTotal;
                        wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 6).Value = "GST";
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 6).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 6).Style.Font.FontSize = 11;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 7).Value = GST;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 7).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 7).Style.Font.FontSize = 11;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                        using (MemoryStream MyMemoryStream = new MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                    #endregion

                }
                return Redirect("GetReportByViolation");
            }
            catch (Exception ex)
            {
                return Redirect("GetReportByViolation");
            }
        }

        #endregion

        #region Due Collected Report
        public ActionResult DueCollectedReport()
        {
            ViewBag.Menu = "RevenueReports";
            return View();
        }
        [HttpPost]
        public JsonResult GetDueCollectedReport(SearchFilters filterData)
        {
            try
            {
                if (filterData.FromTime != null || filterData.Duration == "Today")
                {
                    string fDate1 = Convert.ToString(filterData.FromDate).Split(' ')[0];
                    DateTime fTotalDate = Convert.ToDateTime(fDate1 + " " + filterData.FromTime + ":00 " + filterData.FromMeridiem);

                    string tDate1 = Convert.ToString(filterData.ToDate).Split(' ')[0];
                    DateTime tTotalDate = Convert.ToDateTime(tDate1 + " " + filterData.ToTime + ":00 " + filterData.ToMeridiem);

                    filterData.FromDate = fTotalDate;
                    filterData.ToDate = tTotalDate;
                }
                List<RevenueByDueAmount> revenueByduecollected_lst = revenueReportsDAL_obj.GetDueAmountReport(filterData);
                Session["DueAmountReportData"] = revenueByduecollected_lst;
                return new JsonResult()
                {
                    Data = revenueByduecollected_lst,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult DueCollectedReportPDFDownload(string GrandTotal, string SelectedItems)
        {
            try
            {
                var myDetails = JObject.Parse(SelectedItems);
                var selectedStation = myDetails["SelectedStation"];
                var selectedLot = myDetails["SelectedLot"];
                var SelectedVehicle = myDetails["SelectedVehicle"];
                string selectstation = ((Newtonsoft.Json.Linq.JValue)selectedStation).Value.ToString();
                string selectLot = ((Newtonsoft.Json.Linq.JValue)selectedLot).Value.ToString();
                string selectVehicle = ((Newtonsoft.Json.Linq.JValue)SelectedVehicle).Value.ToString();


                var fromDate = myDetails["FromDate"];
                var toDate = myDetails["ToDate"];
                var duration = myDetails["Duration"];
                string selectfromdate = ((Newtonsoft.Json.Linq.JValue)fromDate).Value.ToString();
                string selecttodate = ((Newtonsoft.Json.Linq.JValue)toDate).Value.ToString();
                string selectduration = ((Newtonsoft.Json.Linq.JValue)duration).Value.ToString();

                string FDate = "";
                string TDate = "";

                if (selectduration == "" || selectduration == null || selectduration == "0")
                {
                    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                }
                else
                {
                    FDate = DateTime.Parse(selectfromdate).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                }

                List<RevenueByDueAmount> result = (List<RevenueByDueAmount>)Session["DueAmountReportData"];
                DataTable dt = ToDataTable(result);

                string TotalCash = Convert.ToString(dt.Rows[0]["TotalCash"]);
                string TotalEPay = Convert.ToString(dt.Rows[0]["TotalEPay"]);

                string filename = GeneratePDFReportforDueCollected(dt, GrandTotal, selectstation, selectLot, selectVehicle, FDate, TDate, TotalCash, TotalEPay);
                FileStream sourceFile = null;

                if (filename != "")
                {
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("content-disposition", "attachment; filename=\"" + Path.GetFileName(filename) + "\"");
                    sourceFile = new FileStream(filename, FileMode.Open);
                    long FileSize;
                    FileSize = sourceFile.Length;
                    byte[] getContent = new byte[(int)FileSize];
                    sourceFile.Read(getContent, 0, (int)sourceFile.Length);
                    sourceFile.Close();
                    Response.Clear();
                    Response.BinaryWrite(getContent);
                    Response.End();
                }
                return Redirect("DueCollectedReport");
            }
            catch (Exception ex)
            {
                return Redirect("DueCollectedReport");
            }
        }
        public ActionResult DueCollectedReportExcelDownload(string GrandTotal, string SelectedItems)
        {
            try
            {
                Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();

                string path = System.Web.HttpContext.Current.Server.MapPath("~/ExcelReports");
                string filename = "DueCollectedReport_" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".xlsx";
                string file_path = path + "/" + filename;

                var myDetails = JObject.Parse(SelectedItems);
                var fromDate = myDetails["FromDate"];
                var toDate = myDetails["ToDate"];
                var duration = myDetails["Duration"];
                string selectfromdate = ((Newtonsoft.Json.Linq.JValue)fromDate).Value.ToString();
                string selecttodate = ((Newtonsoft.Json.Linq.JValue)toDate).Value.ToString();
                string selectduration = ((Newtonsoft.Json.Linq.JValue)duration).Value.ToString();

                string FDate = "";
                string TDate = "";

                if (selectduration == "" || selectduration == null || selectduration == "0")
                {
                    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                }
                else
                {
                    FDate = DateTime.Parse(selectfromdate).ToString("dd/MM/yyyy");
                    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                }

                List<RevenueByDueAmount> result = (List<RevenueByDueAmount>)Session["DueAmountReportData"];
                DataTable dt = ToDataTable(result);

                string TotalCash = Convert.ToString(dt.Rows[0]["TotalCash"]);
                string TotalEPay = Convert.ToString(dt.Rows[0]["TotalEPay"]);

                if (dt.Rows.Count > 0)
                {
                    #region New Code
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add("DueCollectedReport", 0);
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Value = companydata.AccountName;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.FontSize = 12;
                        wb.Worksheets.Worksheet(0).Cell(1, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(2, 1).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(2, 20).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(3, 1).Value = "Due Collected Report";
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(3, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 1).Value = "From : " + FDate + " To : " + TDate;
                        wb.Worksheets.Worksheet(0).Cell(4, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 5).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 6).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 7).Value = "Cur.Date : " + DateTime.Now.ToString("dd/MM/yyy").Split(' ')[0];
                        wb.Worksheets.Worksheet(0).Cell(4, 7).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(6, 1).Value = "Station";
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "Parking Lot";
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Cash";
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 4).Value = "EPay";
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(6, 5).Value = "Amount";
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        int row = 7;
                        int column_num = 0;

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Value = Convert.ToString(dt.Rows[i]["Station"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Value = Convert.ToString(dt.Rows[i]["ParkingLot"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Value = Convert.ToString(dt.Rows[i]["Cash"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Value = Convert.ToString(dt.Rows[i]["EPay"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Value = Convert.ToString(dt.Rows[i]["Amount"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            row = row + 1;
                        }

                        wb.Worksheets.Worksheet(0).Cell(row, 2).Value = "Total : ";
                        wb.Worksheets.Worksheet(0).Cell(row, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 2).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 3).Value = TotalCash;
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(row, 4).Value = TotalEPay;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        wb.Worksheets.Worksheet(0).Cell(row, 5).Value = GrandTotal;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 4).Value = "GST";
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 4).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 4).Style.Font.FontSize = 11;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 5).Value = GST;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 5).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 5).Style.Font.FontSize = 11;
                        //wb.Worksheets.Worksheet(0).Cell(row + 1, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                        using (MemoryStream MyMemoryStream = new MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                    #endregion

                }
                return Redirect("DueCollectedReport");
            }
            catch (Exception ex)
            {
                objExceptionlog.InsertException("Portal", ex.Message, "ReportController", "", "DueCollectedReportExcelDownload");
                return Redirect("DueCollectedReport");
            }
        }
        public string GeneratePDFReportforDueCollected(DataTable dt, string grandTotal, string selectstation, string selectLot, string selectVehicle, string FDate, string TDate, string TotalCash, string TotalEPay)
        {
            Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//07012021

            string filename, AttachmentName1 = "";
            string path = System.Web.HttpContext.Current.Server.MapPath("~/PDFReports");
            AttachmentName1 = "Due Collected Report" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".pdf";
            filename = path + "/" + AttachmentName1;

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(PageSize.A4);
            rec.BackgroundColor = new BaseColor(System.Drawing.Color.Olive);
            Document doc = new Document(rec, 88f, 88f, 10f, 50f);
            var output = new FileStream(filename, FileMode.Create);
            doc.SetPageSize(iTextSharp.text.PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, output);

            writer.PageEvent = new Footer();

            doc.Open();

            BaseColor FontColour = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#353535"));
            BaseColor FontColourBlack = BaseColor.BLACK;
            iTextSharp.text.Font calibri8 = FontFactory.GetFont("Calibri", 10f, FontColour);
            iTextSharp.text.Font _bf_headingaddr = FontFactory.GetFont("Calibri", 8f, FontColour);
            iTextSharp.text.Font _bf_headingaddrbold = FontFactory.GetFont("Calibri", 8f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_heading1 = FontFactory.GetFont("Calibri", 10f, FontColour);
            iTextSharp.text.Font _bf_headingbold = FontFactory.GetFont("Calibri", 10f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingTitle = FontFactory.GetFont("Calibri", 12f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingboldTable = FontFactory.GetFont("Calibri", 10f, iTextSharp.text.Font.BOLD, FontColour);
            iTextSharp.text.Font _bf_headingboldTableNrml = FontFactory.GetFont("Calibri", 9f, FontColour);

            Phrase phrase = null;
            PdfPCell cell = null;
            PdfPTable table = null;

            #region Logo 
            //Header Table start
            table = new PdfPTable(2);
            table.TotalWidth = 550f;
            table.LockedWidth = true;
            table.SetWidths(new float[] { 0.3f, 0.7f });
            table.DefaultCell.Border = Rectangle.NO_BORDER;

            //Company Logo           
            // cell = ImageCell("~/assets/images/Logo4.png", 70f, PdfPCell.ALIGN_CENTER);
            if (companydata.CompanyLogo != null && companydata.CompanyLogo != "")
            {
                cell = ImageCell("~/Images/" + companydata.CompanyLogo, 20f, PdfPCell.ALIGN_CENTER);
            }
            else
            {
                cell = ImageCell("~/Images/default-logo.png", 20f, PdfPCell.ALIGN_CENTER);
            }
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;

            cell.Border = 0;
            table.AddCell(cell);

            PdfPTable innertbl = new PdfPTable(1);
            innertbl.SetWidths(new float[] { 1.0f });
            PdfPCell innercell1 = null;
            phrase = new Phrase();
            //phrase.Add(new Chunk("                                                        Unit #3A, Plot No:847, Pacific Towers,", _bf_headingaddr));
            phrase.Add(new Chunk("                                                              " + companydata.Address1, _bf_headingaddr));
            innercell1 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell1.Border = 0;
            innercell1.Padding = 0;

            innertbl.SpacingBefore = 0f;
            innertbl.SpacingAfter = 2f;
            innertbl.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl.AddCell(innercell1);

            cell = new PdfPCell();
            cell.Border = 0;
            cell.Padding = 0;
            cell.AddElement(innertbl);

            PdfPTable innertbl2 = new PdfPTable(1);
            innertbl2.SetWidths(new float[] { 1.0f });
            PdfPCell innercell2 = null;
            phrase = new Phrase();
            //phrase.Add(new Chunk("                                                        Madhapur, Hyderabad-500081, TS", _bf_headingaddr));
            phrase.Add(new Chunk("                                                              " + companydata.Address2, _bf_headingaddr));
            innercell2 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell2.Border = 0;
            innercell2.Padding = 0;
            innertbl2.SpacingBefore = 0f;
            innertbl2.SpacingAfter = 2f;
            innertbl2.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl2.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl2.AddCell(innercell2);
            cell.AddElement(innertbl2);

            PdfPTable innertbl3 = new PdfPTable(1);
            innertbl3.SetWidths(new float[] { 1.0f });
            PdfPCell innercellTwo = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              GSTIN: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.GSTNumber, _bf_headingaddr));
            //phrase.Add(new Chunk("36AABCT3518Q1ZX", _bf_headingaddr));
            innercellTwo = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercellTwo.Border = 0;
            innercellTwo.Padding = 0;
            innertbl3.SpacingBefore = 0f;
            innertbl3.SpacingAfter = 2f;
            innertbl3.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl3.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl3.AddCell(innercellTwo);
            cell.AddElement(innertbl3);


            PdfPTable innertbl4 = new PdfPTable(1);
            innertbl4.SetWidths(new float[] { 1.0f });
            PdfPCell innercell5 = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              Ph: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.ContactNumber, _bf_headingaddr));
            //phrase.Add(new Chunk("+91 8143143143", _bf_headingaddr));
            innercell5 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell5.Border = 0;
            innercell5.Padding = 0;
            innertbl4.SpacingBefore = 0f;
            innertbl4.SpacingAfter = 2f;
            innertbl4.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl4.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl4.AddCell(innercell5);
            cell.AddElement(innertbl4);
            //table.AddCell(cell);

            PdfPTable innertbl5 = new PdfPTable(1);
            innertbl5.SetWidths(new float[] { 1.0f });
            PdfPCell innercell6 = null;
            phrase = new Phrase();
            phrase.Add(new Chunk("                                                              Email: ", _bf_headingaddrbold));
            phrase.Add(new Chunk(companydata.SupportEmailID, _bf_headingaddr));
            //phrase.Add(new Chunk("support@hmrl.com", _bf_headingaddr));
            innercell6 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            innercell6.Border = 0;
            innercell6.Padding = 0;
            innertbl5.SpacingBefore = 0f;
            innertbl5.SpacingAfter = 0f;
            innertbl5.HorizontalAlignment = Element.ALIGN_RIGHT;
            innertbl5.DefaultCell.Border = Rectangle.NO_BORDER;
            innertbl5.AddCell(innercell6);
            cell.AddElement(innertbl5);
            table.AddCell(cell);

            doc.Add(table);

            //Header Table end
            #endregion

            #region Title
            //Title table start
            PdfPTable tableTitle = new PdfPTable(1);
            tableTitle.DefaultCell.FixedHeight = 150f;
            tableTitle.TotalWidth = 550f;
            tableTitle.LockedWidth = true;
            tableTitle.SetWidths(new float[] { 1f });
            tableTitle.SpacingBefore = 10f;
            tableTitle.SpacingAfter = 3f;

            phrase = new Phrase();
            phrase.Add(new Chunk("REPORT BY DUE AMOUNT", _bf_headingTitle));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
            cell.Border = 0;
            tableTitle.AddCell(cell);
            doc.Add(tableTitle);
            //Title table end
            #endregion

            #region Filter 
            PdfPTable tablefilter = new PdfPTable(1);
            tablefilter.TotalWidth = 550f;
            tablefilter.LockedWidth = true;
            tablefilter.SetWidths(new float[] { 1.0f });
            tablefilter.SpacingBefore = 3f;
            tablefilter.SpacingAfter = 3f;


            phrase = new Phrase();
            phrase.Add(new Chunk("Filtered By      :  ", _bf_headingboldTable));
            // phrase.Add(new Chunk("Filtered By : ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, 1, new BaseColor(System.Drawing.ColorTranslator.FromHtml("#353535")))));
            phrase.Add(new Chunk(selectstation + ", " + selectLot + ", " + selectVehicle, _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tablefilter.AddCell(cell);
            doc.Add(tablefilter);
            #endregion

            #region time
            PdfPTable tabletime = new PdfPTable(1);
            tabletime.TotalWidth = 550f;
            tabletime.LockedWidth = true;
            tabletime.SetWidths(new float[] { 1.0f });
            tabletime.SpacingBefore = 3f;
            tabletime.SpacingAfter = 3f;

            phrase = new Phrase();
            phrase.Add(new Chunk("Report From   :  ", _bf_headingboldTable));
            // phrase.Add(new Chunk("Report From : ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 10, 1, new BaseColor(System.Drawing.ColorTranslator.FromHtml("#353535")))));
            phrase.Add(new Chunk(FDate + " to " + TDate, _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tabletime.AddCell(cell);
            doc.Add(tabletime);

            PdfPTable tabletime1 = new PdfPTable(1);
            tabletime1.TotalWidth = 550f;
            tabletime1.LockedWidth = true;
            tabletime1.SetWidths(new float[] { 1.0f });
            tabletime1.SpacingBefore = 3f;
            tabletime1.SpacingAfter = 5f;
            phrase = new Phrase();
            phrase.Add(new Chunk("Generated On :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(DateTime.Now.ToString("dd/MM/yyyy HH:MM tt"), _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tabletime1.AddCell(cell);
            doc.Add(tabletime1);
            #endregion

            #region Data
            PdfPTable table2 = new PdfPTable(dt.Columns.Count - 2);
            table2.SetWidths(new float[] { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f });
            table2.TotalWidth = doc.PageSize.Width - 40f;
            table2.LockedWidth = true;
            table2.SpacingBefore = 10f;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string cellText = Server.HtmlDecode(dt.Columns[i].ColumnName);
                if (cellText != "TotalCash" && cellText != "TotalEPay")
                {
                    PdfPCell cell2 = new PdfPCell();

                    if (cellText == "Cash" || cellText == "EPay" || cellText == "Amount")
                    {
                        cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                        cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                        cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    }
                    else
                    {
                        if (cellText == "ParkingLot")
                        {
                            cell2.Phrase = new Phrase("Parking Lot", _bf_headingboldTable);
                        }
                        else
                        {
                            cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                        }
                        cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                        cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    }
                    table2.AddCell(cell2);
                }
            }

            //writing table Data  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string cellText = Server.HtmlDecode(dt.Columns[j].ColumnName);
                    if (cellText != "TotalCash" && cellText != "TotalEPay")
                    {
                        PdfPCell cell2 = new PdfPCell();
                        if (cellText == "Cash" || cellText == "EPay" || cellText == "Amount")
                        {
                            cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                            cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell2.VerticalAlignment = Element.ALIGN_RIGHT;
                        }
                        else
                        {
                            cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                            cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                        }

                        table2.AddCell(cell2);
                    }
                }
            }
            //GRAND TOTAL TABLE            
            PdfPTable grandTable = new PdfPTable(dt.Columns.Count - 2);
            grandTable.SetWidths(new float[] { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f });
            grandTable.TotalWidth = doc.PageSize.Width - 40f;
            grandTable.LockedWidth = true;

            PdfPCell cellt = new PdfPCell(new Phrase(" ", _bf_headingboldTable));
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cellt = new PdfPCell();
            cellt.Phrase = new Phrase("Total ", _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
            cellt.BorderWidthLeft = 0;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalCash, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalEPay, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(grandTotal, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthTop = 0;
            cellt.BorderWidthRight = 1;
            grandTable.AddCell(cellt);
            //GRNAD TOTAL TABLE

            //// GST Table
            //PdfPTable gstTable = new PdfPTable(dt.Columns.Count - 2);
            //gstTable.SetWidths(new float[] { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f });
            //gstTable.TotalWidth = doc.PageSize.Width - 40f;
            //gstTable.LockedWidth = true;

            //PdfPCell cellGST = new PdfPCell(new Phrase(" ", _bf_headingboldTable));
            //cellGST.BorderWidthRight = 0;
            //cellGST.BorderWidthTop = 0;
            //cellGST.Colspan = 3;
            //gstTable.AddCell(cellGST);

            //cellGST = new PdfPCell();
            //cellGST.Phrase = new Phrase("GST", _bf_headingboldTable);
            //cellGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cellGST.BorderWidthLeft = 0;
            //cellGST.BorderWidthRight = 0;
            //cellGST.BorderWidthTop = 0;
            //gstTable.AddCell(cellGST);

            //cell = new PdfPCell();
            //cellGST.Phrase = new Phrase(GST, _bf_headingboldTable);
            //cellGST.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cellGST.BorderWidthTop = 0;
            //cellGST.BorderWidthRight = 1;
            //gstTable.AddCell(cellGST);
            ////GST Table

            doc.Add(table2);
            doc.Add(grandTable);
           // doc.Add(gstTable);
            #endregion

            doc.Close();
            byte[] result = ms.ToArray();
            return filename;

        }

        #endregion

        public JsonNetResult GetAccountDetails()
        {
            List<Account> accountList = companyInfoDAL_obj.GetAccountDetails();

            try
            {
                return new JsonNetResult()
                {
                    Data = accountList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }

        #region List to Datatable Convertion
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        #endregion

        #region PDF Suport Files
        private static void DrawLine(PdfWriter writer, float x1, float y1, float x2, float y2, System.Drawing.Color color)
        {
            PdfContentByte contentByte = writer.DirectContent;
            //contentByte.SetColorStroke(color);
            contentByte.MoveTo(x1, y1);
            contentByte.LineTo(x2, y2);
            contentByte.Stroke();
        }
        private static PdfPCell PhraseCell(Phrase phrase, int align)
        {
            PdfPCell cell = new PdfPCell(phrase);
            //cell.BorderColor = Color.White;
            cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 2f;
            cell.PaddingTop = 0f;
            return cell;
        }
        public PdfPCell ImageCell(string path, float scale, int align)
        {
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(Server.MapPath(path));
            image.ScalePercent(scale);
            image.ScaleAbsolute(50f, 50f);
            PdfPCell cell = new PdfPCell(image);
            //cell.BorderColor = Color.WHITE;
            cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 0f;
            cell.PaddingTop = 0f;
            return cell;
        }
        public partial class Footer : PdfPageEventHelper
        {
            public override void OnEndPage(PdfWriter writer, Document doc)
            {
                BaseColor FontColour = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#A9A9A9"));
                iTextSharp.text.Font footerFont = FontFactory.GetFont("Calibri", 10f, FontColour);
                Paragraph footer = new Paragraph("Powered by InstaParking", footerFont);
                footer.Alignment = Element.ALIGN_RIGHT;
                PdfPTable footerTbl = new PdfPTable(1);
                footerTbl.TotalWidth = 550;
                footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell cell = new PdfPCell(footer);
                cell.Border = 0;
                cell.PaddingLeft = 10;

                footerTbl.AddCell(cell);
                footerTbl.WriteSelectedRows(0, -1, 450, 30, writer.DirectContent);
            }
            //public override void OnOpenDocument(PdfWriter writer, Document document)
            //{
            //    base.OnOpenDocument(writer, document);
            //    //PdfPTable tabFot = new PdfPTable(new float[] { 1F });
            //    //tabFot.SpacingAfter = 10F;
            //    //PdfPCell cell;
            //    //tabFot.TotalWidth = 550F;
            //    //cell = new PdfPCell(new Phrase("Header"));
            //    //tabFot.AddCell(cell);
            //    //tabFot.WriteSelectedRows(0, -1, 150, document.Top, writer.DirectContent);

            //    RevenueReports_DAL revenueReportsDAL_obj = new RevenueReports_DAL();
            //    Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//14012021

            //    BaseColor FontColour = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#353535"));
            //    BaseColor FontColourBlack = BaseColor.BLACK;
            //    iTextSharp.text.Font calibri8 = FontFactory.GetFont("Calibri", 10f, FontColour);
            //    iTextSharp.text.Font _bf_headingaddr = FontFactory.GetFont("Calibri", 8f, FontColour);
            //    iTextSharp.text.Font _bf_headingaddrbold = FontFactory.GetFont("Calibri", 8f, iTextSharp.text.Font.BOLD, FontColour);
            //    iTextSharp.text.Font _bf_heading1 = FontFactory.GetFont("Calibri", 10f, FontColour);
            //    iTextSharp.text.Font _bf_headingbold = FontFactory.GetFont("Calibri", 10f, iTextSharp.text.Font.BOLD, FontColour);
            //    iTextSharp.text.Font _bf_headingTitle = FontFactory.GetFont("Calibri", 12f, iTextSharp.text.Font.BOLD, FontColour);
            //    iTextSharp.text.Font _bf_headingboldTable = FontFactory.GetFont("Calibri", 10f, iTextSharp.text.Font.BOLD, FontColour);
            //    iTextSharp.text.Font _bf_headingboldTableNrml = FontFactory.GetFont("Calibri", 9f, FontColour);

            //    #region Logo 
            //    //Header Table start

            //    Phrase phrase = null;
            //    PdfPCell cell = null;
            //    PdfPTable table = null;

            //    table = new PdfPTable(2);
            //    table.TotalWidth = 550f;
            //    table.LockedWidth = true;
            //    table.SetWidths(new float[] { 0.3f, 0.7f });
            //    table.DefaultCell.Border = Rectangle.NO_BORDER;

            //    //Company Logo  

            //    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(HttpContext.Server.MapPath("~/Images/") + companydata.CompanyLogo);
            //    cell = new PdfPCell(jpg);
            //    //cell = ImageCells("~/assets/images/Logo4.png", 70f, PdfPCell.ALIGN_CENTER);
            //    //cell = ImageCell("~/Images/"+companydata.CompanyLogo, 20f, PdfPCell.ALIGN_CENTER);
            //    cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //    cell.Border = 0;
            //    table.AddCell(cell);

            //    PdfPTable innertbl = new PdfPTable(1);
            //    innertbl.SetWidths(new float[] { 1.0f });
            //    PdfPCell innercell1 = null;
            //    phrase = new Phrase();
            //    //phrase.Add(new Chunk("                                                        Unit #3A, Plot No:847, Pacific Towers,", _bf_headingaddr));
            //    phrase.Add(new Chunk("                                                              " + companydata.Address1, _bf_headingaddr));
            //    innercell1 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            //    innercell1.Border = 0;
            //    innercell1.Padding = 0;

            //    innertbl.SpacingBefore = 0f;
            //    innertbl.SpacingAfter = 2f;
            //    innertbl.HorizontalAlignment = Element.ALIGN_RIGHT;
            //    innertbl.AddCell(innercell1);

            //    cell = new PdfPCell();
            //    cell.Border = 0;
            //    cell.Padding = 0;
            //    cell.AddElement(innertbl);

            //    PdfPTable innertbl2 = new PdfPTable(1);
            //    innertbl2.SetWidths(new float[] { 1.0f });
            //    PdfPCell innercell2 = null;
            //    phrase = new Phrase();
            //    //phrase.Add(new Chunk("                                                        Madhapur, Hyderabad-500081, TS", _bf_headingaddr));
            //    phrase.Add(new Chunk("                                                              " + companydata.Address2, _bf_headingaddr));
            //    innercell2 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            //    innercell2.Border = 0;
            //    innercell2.Padding = 0;
            //    innertbl2.SpacingBefore = 0f;
            //    innertbl2.SpacingAfter = 2f;
            //    innertbl2.HorizontalAlignment = Element.ALIGN_RIGHT;
            //    innertbl2.DefaultCell.Border = Rectangle.NO_BORDER;
            //    innertbl2.AddCell(innercell2);
            //    cell.AddElement(innertbl2);

            //    PdfPTable innertbl3 = new PdfPTable(1);
            //    innertbl3.SetWidths(new float[] { 1.0f });
            //    PdfPCell innercellTwo = null;
            //    phrase = new Phrase();
            //    phrase.Add(new Chunk("                                                              GSTIN: ", _bf_headingaddrbold));
            //    phrase.Add(new Chunk(companydata.GSTNumber, _bf_headingaddr));
            //    //phrase.Add(new Chunk("36AABCT3518Q1ZX", _bf_headingaddr));
            //    innercellTwo = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            //    innercellTwo.Border = 0;
            //    innercellTwo.Padding = 0;
            //    innertbl3.SpacingBefore = 0f;
            //    innertbl3.SpacingAfter = 2f;
            //    innertbl3.HorizontalAlignment = Element.ALIGN_RIGHT;
            //    innertbl3.DefaultCell.Border = Rectangle.NO_BORDER;
            //    innertbl3.AddCell(innercellTwo);
            //    cell.AddElement(innertbl3);


            //    PdfPTable innertbl4 = new PdfPTable(1);
            //    innertbl4.SetWidths(new float[] { 1.0f });
            //    PdfPCell innercell5 = null;
            //    phrase = new Phrase();
            //    phrase.Add(new Chunk("                                                              Ph: ", _bf_headingaddrbold));
            //    phrase.Add(new Chunk(companydata.ContactNumber, _bf_headingaddr));
            //    //phrase.Add(new Chunk("+91 8143143143", _bf_headingaddr));
            //    innercell5 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            //    innercell5.Border = 0;
            //    innercell5.Padding = 0;
            //    innertbl4.SpacingBefore = 0f;
            //    innertbl4.SpacingAfter = 2f;
            //    innertbl4.HorizontalAlignment = Element.ALIGN_RIGHT;
            //    innertbl4.DefaultCell.Border = Rectangle.NO_BORDER;
            //    innertbl4.AddCell(innercell5);
            //    cell.AddElement(innertbl4);
            //    //table.AddCell(cell);

            //    PdfPTable innertbl5 = new PdfPTable(1);
            //    innertbl5.SetWidths(new float[] { 1.0f });
            //    PdfPCell innercell6 = null;
            //    phrase = new Phrase();
            //    phrase.Add(new Chunk("                                                              Email: ", _bf_headingaddrbold));
            //    phrase.Add(new Chunk(companydata.SupportEmailID, _bf_headingaddr));
            //    //phrase.Add(new Chunk("support@hmrl.com", _bf_headingaddr));
            //    innercell6 = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            //    innercell6.Border = 0;
            //    innercell6.Padding = 0;
            //    innertbl5.SpacingBefore = 0f;
            //    innertbl5.SpacingAfter = 0f;
            //    innertbl5.HorizontalAlignment = Element.ALIGN_RIGHT;
            //    innertbl5.DefaultCell.Border = Rectangle.NO_BORDER;
            //    innertbl5.AddCell(innercell6);
            //    cell.AddElement(innertbl5);
            //    table.AddCell(cell);

            //    table.WriteSelectedRows(0, -1, 150, document.Top, writer.DirectContent);

            //    //Header Table end
            //    #endregion




            //}

        }

        #endregion
        public class RoundRectangle : IPdfPCellEvent
        {
            public void CellLayout(
              PdfPCell cell, Rectangle rect, PdfContentByte[] canvas
            )
            {
                PdfContentByte cb = canvas[PdfPTable.LINECANVAS];
                cb.RoundRectangle(
                  rect.Left,
                  rect.Bottom,
                  rect.Width,
                  rect.Height,
                  4 // change to adjust how "round" corner is displayed
                );
                cb.SetLineWidth(1f);
                cb.SetCMYKColorStrokeF(0f, 0f, 0f, 1f);
                cb.Stroke();
            }
        }
    }
}
