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
    public class OperationalReportsController : Controller
    {
        OperationalReports_DAL operationalReportDAL_obj = new OperationalReports_DAL();
        UserLocationMapper_DAL userLocatioMapperDAL_obj = new UserLocationMapper_DAL();
        RevenueReports_DAL revenueReportsDAL_obj = new RevenueReports_DAL();

        #region By Operator
        public ActionResult ReportByOperator()
        {
            ViewBag.Menu = "OperationalReports";
            return View();
        }
        public JsonNetResult GetActiveOperatorsList(string supervisorID)
        {
            //IList<User> usersList = userLocatioMapperDAL_obj.GetOperatorsBySupervisorID(Convert.ToInt32(supervisorID));
            IList<User> usersList = operationalReportDAL_obj.GetOperatorsBySupervisorID(Convert.ToInt32(supervisorID));

            try
            {
                return new JsonNetResult()
                {
                    Data = usersList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        public JsonNetResult GetOperatorLotsList(string operatorID)
        {
            IList<Lots> lotsList = operationalReportDAL_obj.GetOperatorLotsList(Convert.ToInt32(operatorID));

            try
            {
                return new JsonNetResult()
                {
                    Data = lotsList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        public JsonNetResult GetActiveSupervisorsList()
        {
            IList<User> supervisorsList = operationalReportDAL_obj.GetActiveSupervisorsList();

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
        [HttpPost]
        public JsonResult GetReportByOperator(SearchFilters operatorFilterData)
        {
            try
            {
                List<OperatorHoursReport> hours_lst = operationalReportDAL_obj.GetReportByOperator(operatorFilterData);
                Session["OperatorHoursReportData"] = hours_lst;
                return new JsonResult()
                {
                    Data = hours_lst,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult OperatorHoursReportPDFDownload(string SelectedItems)
        {
            try
            {
                var myDetails = JObject.Parse(SelectedItems);
                var selectedLot = myDetails["SelectedLot"];
                string selectLot = ((Newtonsoft.Json.Linq.JValue)selectedLot).Value.ToString();
                var selectedSupervisor = myDetails["SupervisorName"];
                string selectSupervisor = ((Newtonsoft.Json.Linq.JValue)selectedSupervisor).Value.ToString();
                var selectedOperator = myDetails["OperatorName"];
                string selectOperator = ((Newtonsoft.Json.Linq.JValue)selectedOperator).Value.ToString();

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

                var fromShortDate = Convert.ToDateTime(fromDate).ToShortDateString();
                var toShortDate = Convert.ToDateTime(toDate).ToShortDateString();
                double days;
                if (fromShortDate == toShortDate)
                {
                    days = 1;
                }
                else
                {
                    days = (Convert.ToDateTime(toDate) - Convert.ToDateTime(fromDate)).TotalDays + 1;
                }

                List<OperatorHoursReport> result = (List<OperatorHoursReport>)Session["OperatorHoursReportData"];
                DataTable dt = ToDataTable(result);
                string TotalDays = Convert.ToString(dt.Rows[0]["Total"]);

                string filename = GenerateOperatorHourPDFReport(dt, selectSupervisor, selectOperator, selectLot, FDate, TDate, TotalDays, selectduration, days);
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
                return Redirect("CheckInReport");
            }
            catch (Exception ex)
            {
                return Redirect("CheckInReport");
            }
        }
        public string GenerateOperatorHourPDFReport(DataTable dt, string supervisor, string selectoperator, string selectLot, string FDate, string TDate, string TotalDays, string selectduration, double days)
        {
            Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//15012021

            string filename, AttachmentName1 = "";
            string path = System.Web.HttpContext.Current.Server.MapPath("~/PDFReports");
            AttachmentName1 = "Operator Report" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".pdf";
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
            phrase.Add(new Chunk("OPERATIONAL REPORT BY OPERATOR", _bf_headingTitle));
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
            phrase.Add(new Chunk(supervisor + ", " + selectoperator + ", " + selectLot, _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tablefilter.AddCell(cell);

            // phrase = new Phrase();
            // phrase.Add(new Chunk(supervisor, _bf_heading1));
            // cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            // cell.BorderColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#dbdbdb"));
            //tablefilter.AddCell(cell);

            // phrase = new Phrase();
            // phrase.Add(new Chunk(selectoperator, _bf_heading1));
            // cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            // cell.BorderColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#dbdbdb"));
            // tablefilter.AddCell(cell);

            // phrase = new Phrase();
            // phrase.Add(new Chunk(selectLot, _bf_heading1));
            // cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            // cell.BorderColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#dbdbdb"));
            // tablefilter.AddCell(cell);

            // phrase = new Phrase();
            // phrase.Add(new Chunk("", _bf_heading1));
            // cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            // cell.Border = 0;
            // //cell.Colspan = 10;
            // tablefilter.AddCell(cell);

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
            if (selectduration != "Current Month" && selectduration != "Previous Month" && days == 1)
            {
                table2 = new PdfPTable(dt.Columns.Count - 1);
                table2.SetWidths(new float[] { 0.2f, 0.18f, 0.2f, 0.23f, 0.22f, 0.1f, 0.1f });
            }
            else
            {
                table2 = new PdfPTable(dt.Columns.Count - 4);
                table2.SetWidths(new float[] { 0.25f, 0.25f, 0.25f, 0.25f });
            }

            table2.TotalWidth = doc.PageSize.Width - 40f;
            table2.LockedWidth = true;
            table2.SpacingBefore = 10f;
            // table2.SpacingAfter = 3f;

            if (selectduration != "Current Month" && selectduration != "Previous Month" && days == 1)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string cellText = Server.HtmlDecode(dt.Columns[i].ColumnName);
                    if (cellText != "Total")
                    {
                        PdfPCell cell2 = new PdfPCell();
                        if (cellText == "TotalDays")
                        {
                            cell2.Phrase = new Phrase("Login Days", _bf_headingboldTable);
                            cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                            cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                        }
                        else
                        {
                            if (cellText == "LocationParkingLotName")
                            {
                                cell2.Phrase = new Phrase("Parking Lot", _bf_headingboldTable);
                                // cell2.Phrase = new Phrase("Lot Code", _bf_headingboldTable);
                            }
                            else if (cellText == "TotalHours")
                            {
                                cell2.Phrase = new Phrase("Total Hours", _bf_headingboldTable);
                            }
                            else if (cellText == "CheckInTime")
                            {
                                cell2.Phrase = new Phrase("Check In Time", _bf_headingboldTable);
                            }
                            else if (cellText == "CheckOutTime")
                            {
                                cell2.Phrase = new Phrase("Check Out Time", _bf_headingboldTable);
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
            }
            else
            {
                for (int i = 0; i < dt.Columns.Count - 1; i++)
                {
                    string cellText = Server.HtmlDecode(dt.Columns[i].ColumnName);
                    if (cellText != "Total")
                    {
                        PdfPCell cell2 = new PdfPCell();
                        if (cellText != "LocationParkingLotName" && cellText != "CheckInTime" && cellText != "CheckOutTime")
                        {
                            if (cellText == "TotalDays")
                            {
                                cell2.Phrase = new Phrase("Login Days", _bf_headingboldTable);
                                cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                                cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                            }
                            else
                            {
                                if (cellText == "TotalHours")
                                {
                                    cell2.Phrase = new Phrase("Total Hours", _bf_headingboldTable);
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
                }
            }


            //writing table Data  
            if (selectduration != "Current Month" && selectduration != "Previous Month" && days == 1)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string cellText = Server.HtmlDecode(dt.Columns[j].ColumnName);
                        if (cellText != "Total")
                        {
                            PdfPCell cell2 = new PdfPCell();
                            if (cellText == "TotalDays")
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
            else
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count - 1; j++)
                    {
                        string cellText = Server.HtmlDecode(dt.Columns[j].ColumnName);
                        if (cellText != "Total")
                        {
                            PdfPCell cell2 = new PdfPCell();
                            if (cellText != "LocationParkingLotName" && cellText != "CheckInTime" && cellText != "CheckOutTime")
                            {
                                if (cellText == "TotalDays")
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
            if (selectduration != "Current Month" && selectduration != "Previous Month" && days == 1)
            {
                grandTable = new PdfPTable(dt.Columns.Count - 1);
            }
            else
            {
                grandTable = new PdfPTable(dt.Columns.Count - 4);
            }
            grandTable.TotalWidth = doc.PageSize.Width - 40f;
            grandTable.LockedWidth = true;
            PdfPCell cell3 = new PdfPCell();
            cell3.Phrase = new Phrase("Total Days :                             " + TotalDays, _bf_headingboldTable);
            cell3.Colspan = dt.Columns.Count;
            cell3.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cell3.Border = 0;
            cell3.BorderWidthTop = 0;
            grandTable.AddCell(cell3);
            //GRNAD TOTAL TABLE

            doc.Add(table2);

            doc.Add(grandTable);
            #endregion

            doc.Close();
            byte[] result = ms.ToArray();
            return filename;
        }
        public ActionResult OperatorHoursReportExcelDownload(string SelectedItems)
        {
            try
            {
                Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//18012021

                string path = System.Web.HttpContext.Current.Server.MapPath("~/ExcelReports");
                string filename = "Operator Report_" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".xlsx";
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

                var fromShortDate = Convert.ToDateTime(fromDate).ToShortDateString();
                var toShortDate = Convert.ToDateTime(toDate).ToShortDateString();
                double days;
                if (fromShortDate == toShortDate)
                {
                    days = 1;
                }
                else
                {
                    days = (Convert.ToDateTime(toDate) - Convert.ToDateTime(fromDate)).TotalDays + 1;
                }

                List<OperatorHoursReport> result = (List<OperatorHoursReport>)Session["OperatorHoursReportData"];
                DataTable dt = ToDataTable(result);
                string TotalDays = Convert.ToString(dt.Rows[0]["Total"]);

                if (dt.Rows.Count > 0)
                {
                    #region New Code
                    using (XLWorkbook wb = new XLWorkbook())
                    {

                        wb.Worksheets.Add("Operational Report By Operator", 0);
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Value = companydata.AccountName;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.FontSize = 12;
                        wb.Worksheets.Worksheet(0).Cell(1, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(2, 1).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(2, 20).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(3, 1).Value = "Operational Report By Operator";
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

                        if (selectduration != "Current Month" && selectduration != "Previous Month" && days == 1)
                        {
                            wb.Worksheets.Worksheet(0).Cell(6, 1).Value = "Supervisor";
                            wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "Operator";
                            wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Parking Lot";
                            // wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Lot Code";
                            wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(6, 4).Value = "Check In Time";
                            wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(6, 5).Value = "Check Out Time";
                            wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(6, 6).Value = "Total Hours";
                            wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(6, 7).Value = "Login Days";
                            wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        }
                        else
                        {
                            wb.Worksheets.Worksheet(0).Cell(6, 1).Value = "Supervisor";
                            wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "Operator";
                            wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Total Hours";
                            wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(6, 4).Value = "Login Days";
                            wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        }
                        int row = 7;
                        int column_num = 0;


                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (selectduration != "Current Month" && selectduration != "Previous Month" && days == 1)
                            {
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Value = Convert.ToString(dt.Rows[i]["Supervisor"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Style.Font.FontSize = 11;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Value = Convert.ToString(dt.Rows[i]["Operator"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Style.Font.FontSize = 11;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Value = Convert.ToString(dt.Rows[i]["LocationParkingLotName"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Font.FontSize = 11;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Value = Convert.ToString(dt.Rows[i]["CheckInTime"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Font.FontSize = 11;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Value = Convert.ToString(dt.Rows[i]["CheckOutTime"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Font.FontSize = 11;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Value = Convert.ToString(dt.Rows[i]["TotalHours"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Style.Font.FontSize = 11;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Value = Convert.ToString(dt.Rows[i]["TotalDays"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Style.Font.FontSize = 11;
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                                row = row + 1;
                            }
                            else
                            {
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Value = Convert.ToString(dt.Rows[i]["Supervisor"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Style.Font.FontSize = 11;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Value = Convert.ToString(dt.Rows[i]["Operator"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Style.Font.FontSize = 11;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Value = Convert.ToString(dt.Rows[i]["TotalHours"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Font.FontSize = 11;

                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Value = Convert.ToString(dt.Rows[i]["TotalDays"]);
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Font.FontSize = 11;
                                wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                                row = row + 1;
                            }
                        }

                        if (selectduration != "Current Month" && selectduration != "Previous Month" && days == 1)
                        {
                            wb.Worksheets.Worksheet(0).Cell(row, 6).Value = "Total Days : ";
                            wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, 7).Value = TotalDays;
                            wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.FontSize = 11;
                        }
                        else
                        {
                            wb.Worksheets.Worksheet(0).Cell(row, 3).Value = "Total Days : ";
                            wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, 4).Value = TotalDays;
                            wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.Bold = true;
                            wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.FontSize = 11;
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
                return Redirect("ReportByOperator");
            }
            catch (Exception ex)
            {
                // objExceptionlog.InsertException("Portal", ex.Message, "ReportController", "", "StationReportExcelDownload");
                return Redirect("ReportByOperator");
            }
        }
        #endregion

        #region Occupancy Report
        public ActionResult OccupancyReport()
        {
            ViewBag.Menu = "OperationalReports";
            return View();
        }
        [HttpPost]
        public JsonResult GetOccupancyReport(SearchFilters occupancyFilterData)
        {
            try
            {
                List<OccupancyReport> occupancy_lst = operationalReportDAL_obj.GetOccupancyReport(occupancyFilterData);
                Session["OccupancyReportData"] = occupancy_lst;
                return new JsonResult()
                {
                    Data = occupancy_lst,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult OccupancyReportPDFDownload(string SelectedItems)
        {
            try
            {
                var myDetails = JObject.Parse(SelectedItems);
                var selectedStation = myDetails["SelectedStation"];
                var selectedLot = myDetails["SelectedLot"];
                var selectedVehicle = myDetails["SelectedVehicle"];
                string selectstation = ((Newtonsoft.Json.Linq.JValue)selectedStation).Value.ToString();
                string selectLot = ((Newtonsoft.Json.Linq.JValue)selectedLot).Value.ToString();
                string selectVehicle = ((Newtonsoft.Json.Linq.JValue)selectedVehicle).Value.ToString();


                List<OccupancyReport> result = (List<OccupancyReport>)Session["OccupancyReportData"];
                DataTable dt = ToDataTable(result);
                string filename = GenerateOccupancyPDFReport(dt, selectstation, selectLot, selectVehicle);
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
                return Redirect("CheckInReport");
            }
            catch (Exception ex)
            {
                return Redirect("CheckInReport");
            }
        }
        public string GenerateOccupancyPDFReport(DataTable dt, string selectstation, string selectLot, string selectVehicle)
        {
            Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//15012021

            string filename, AttachmentName1 = "";
            string path = System.Web.HttpContext.Current.Server.MapPath("~/PDFReports");
            AttachmentName1 = "Occupancy Report" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".pdf";
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
            phrase.Add(new Chunk("OCCUPANCY REPORT", _bf_headingTitle));
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
            tabletime.SpacingAfter = 5f;


            phrase = new Phrase();
            phrase.Add(new Chunk("Generated On :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(DateTime.Now.ToString("dd/MM/yyyy HH:MM tt"), _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tabletime.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk(DateTime.Now.ToString("dd/MM/yyyy HH:MM tt"), _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.Border = 0;
            //tabletime.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk("", _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.Border = 0;
            //tabletime.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk("", _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.Border = 0;
            //tabletime.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk("", _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.Border = 0;
            //tabletime.AddCell(cell);

            doc.Add(tabletime);
            #endregion

            #region Data
            PdfPTable table2 = new PdfPTable(dt.Columns.Count);
            table2.TotalWidth = doc.PageSize.Width - 40f;
            table2.LockedWidth = true;
            table2.SpacingBefore = 10f;
            table2.SpacingAfter = 3f;

            for (int i = 0; i < dt.Columns.Count; i++)
            {

                string cellText = Server.HtmlDecode(dt.Columns[i].ColumnName);
                PdfPCell cell2 = new PdfPCell();
                if (cellText == "ParkingLot")
                {
                    cell2.Phrase = new Phrase("Parking Lot", _bf_headingboldTable);
                    // cell2.Phrase = new Phrase("Lot Code", _bf_headingboldTable);
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                }
                else if (cellText == "CurrentlyParked")
                {
                    cell2.Phrase = new Phrase("Currently Parked", _bf_headingboldTable);
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                }
                else if (cellText == "Occupancy")
                {
                    cell2.Phrase = new Phrase("% Occupancy", _bf_headingboldTable);
                    cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                }
                else
                {
                    cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                }
                cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                //cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                table2.AddCell(cell2);
            }

            //writing table Data  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string cellText = Server.HtmlDecode(dt.Columns[j].ColumnName);

                    PdfPCell cell2 = new PdfPCell();
                    if (cellText == "Occupancy")
                    {
                        cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                        cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    }
                    else
                    {
                        cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                        cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                        //  cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                    }
                    table2.AddCell(cell2);
                }
            }
            //GRAND TOTAL TABLE
            //PdfPTable grandTable = new PdfPTable(dt.Columns.Count);
            //grandTable.TotalWidth = doc.PageSize.Width - 40f;
            //grandTable.LockedWidth = true;
            //PdfPCell cell3 = new PdfPCell();
            //cell3.Phrase = new Phrase("Grand Total : " + grandTotal, _bf_headingboldTable);
            //cell3.Colspan = dt.Columns.Count;
            //cell3.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cell3.Border = 0;
            //grandTable.AddCell(cell3);
            //GRNAD TOTAL TABLE

            doc.Add(table2);

            //doc.Add(grandTable);
            #endregion

            doc.Close();
            byte[] result = ms.ToArray();
            return filename;

        }
        public ActionResult OccupancyReportExcelDownload(string SelectedItems)
        {
            try
            {
                Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//18012021

                string path = System.Web.HttpContext.Current.Server.MapPath("~/ExcelReports");
                string filename = "Occupancy Report_" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".xlsx";
                string file_path = path + "/" + filename;

                var myDetails = JObject.Parse(SelectedItems);
                //var fromDate = myDetails["FromDate"];
                //var toDate = myDetails["ToDate"];
                //string selectfromdate = ((Newtonsoft.Json.Linq.JValue)fromDate).Value.ToString();
                //string selecttodate = ((Newtonsoft.Json.Linq.JValue)toDate).Value.ToString();                

                //string FDate = DateTime.Parse(selectfromdate).ToString("dd/MM/yyyy");
                //string TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");

                List<OccupancyReport> result = (List<OccupancyReport>)Session["OccupancyReportData"];
                DataTable dt = ToDataTable(result);


                if (dt.Rows.Count > 0)
                {
                    #region New Code
                    using (XLWorkbook wb = new XLWorkbook())
                    {

                        wb.Worksheets.Add("Occupancy Report", 0);
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Value = companydata.AccountName;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.FontSize = 12;
                        wb.Worksheets.Worksheet(0).Cell(1, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(2, 1).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(2, 20).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(3, 1).Value = "Occupancy Report";
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(3, 4).IsMerged();

                        // wb.Worksheets.Worksheet(0).Cell(4, 1).Value = "From : " + FDate + " To : " + TDate;
                        wb.Worksheets.Worksheet(0).Cell(4, 1).Value = "Cur.Date : " + DateTime.Now.ToString("dd/MM/yyy").Split(' ')[0];
                        wb.Worksheets.Worksheet(0).Cell(4, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 5).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 6).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 7).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(4, 7).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(6, 1).Value = "Station";
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "Parking Lot";
                        // wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "Lot Code";
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Capacity";
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 4).Value = "Currently Parked";
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 5).Value = "% Occupancy";
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.FontSize = 11;

                        int row = 7;
                        int column_num = 0;


                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Value = Convert.ToString(dt.Rows[i]["Station"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Value = Convert.ToString(dt.Rows[i]["ParkingLot"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Value = Convert.ToString(dt.Rows[i]["Capacity"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Value = Convert.ToString(dt.Rows[i]["CurrentlyParked"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Value = Convert.ToString(dt.Rows[i]["Occupancy"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Font.FontSize = 11;

                            row = row + 1;
                        }

                        // row = row + 1;
                        //wb.Worksheets.Worksheet(0).Cell(row, 6).Value = "Grand Total : ";
                        //wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.FontSize = 11;

                        //wb.Worksheets.Worksheet(0).Cell(row, 7).Value = GrandTotal;
                        //wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.FontSize = 11;


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
                return Redirect("CheckInReport");
            }
            catch (Exception ex)
            {
                // objExceptionlog.InsertException("Portal", ex.Message, "ReportController", "", "StationReportExcelDownload");
                return Redirect("CheckInReport");
            }
        }
        #endregion

        #region CheckIn Report
        public ActionResult CheckInReport()
        {
            ViewBag.Menu = "OperationalReports";
            return View();
        }
        [HttpPost]
        public JsonResult GetCheckInReport(SearchFilters CheckInFilterData)
        {
            try
            {
                if (CheckInFilterData.FromTime != null || CheckInFilterData.Duration == "Today")
                {
                    string fDate1 = Convert.ToString(CheckInFilterData.FromDate).Split(' ')[0];
                    DateTime fTotalDate = Convert.ToDateTime(fDate1 + " 00:00:00");

                    string tDate1 = Convert.ToString(CheckInFilterData.ToDate).Split(' ')[0];
                    DateTime tTotalDate = Convert.ToDateTime(tDate1 + " 23:00:00");

                    CheckInFilterData.FromDate = fTotalDate;
                    CheckInFilterData.ToDate = tTotalDate;
                }

                List<CheckInReport> checkin_lst = operationalReportDAL_obj.GetCheckInReport(CheckInFilterData);
                Session["CheckInReportData"] = checkin_lst;
                return new JsonResult()
                {
                    Data = checkin_lst,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult CheckInReportPDFDownload(string GrandTotal, string SelectedItems)
        {
            try
            {
                var myDetails = JObject.Parse(SelectedItems);
                var selectedStation = myDetails["SelectedStation"];
                var selectedLot = myDetails["SelectedLot"];
                string selectstation = ((Newtonsoft.Json.Linq.JValue)selectedStation).Value.ToString();
                string selectLot = ((Newtonsoft.Json.Linq.JValue)selectedLot).Value.ToString();

                var fromDate = myDetails["FromDate"];
                var toDate = myDetails["ToDate"];
                var duration = myDetails["Duration"];
                string selectfromdate = ((Newtonsoft.Json.Linq.JValue)fromDate).Value.ToString();
                string selecttodate = ((Newtonsoft.Json.Linq.JValue)toDate).Value.ToString();
                string selectduration = ((Newtonsoft.Json.Linq.JValue)duration).Value.ToString();

                string FDate = "";
                string TDate = "";

                //if (selectduration == "Previous Month")
                //{
                //     FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                //     TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                //}
                //else if(selectduration == "Current Month")
                //{
                //    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                //    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                //}
                //else if (selectduration == "Yesterday")
                //{
                //    FDate = DateTime.Parse(selectfromdate).ToString("dd/MM/yyyy");
                //    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                //}
                //else
                //{
                //    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                //    TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                //}
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

                List<CheckInReport> result = (List<CheckInReport>)Session["CheckInReportData"];
                DataTable dt = ToDataTable(result);

                string TotalAppIn = Convert.ToString(dt.Rows[0]["AppTotal"]);
                string TotalPassesIn = Convert.ToString(dt.Rows[0]["PassTotal"]);
                string TotalOperatorIn = Convert.ToString(dt.Rows[0]["OperatorTotal"]);
                string TotalCallIn = Convert.ToString(dt.Rows[0]["CallPayTotal"]);
                string OutTotal = Convert.ToString(dt.Rows[0]["OutTotal"]);
                string FOCTotal = Convert.ToString(dt.Rows[0]["FOCTotal"]);

                string filename = GenerateReportNew(dt, GrandTotal, selectstation, selectLot, FDate, TDate, TotalAppIn, TotalPassesIn, TotalOperatorIn, TotalCallIn, OutTotal, FOCTotal);
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
                return Redirect("CheckInReport");
            }
            catch (Exception ex)
            {
                return Redirect("CheckInReport");
            }
        }
        public ActionResult CheckInReportExcelDownload(string GrandTotal, string SelectedItems)
        {
            try
            {
                Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//18012021

                string path = System.Web.HttpContext.Current.Server.MapPath("~/ExcelReports");
                string filename = "Check In Report_" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".xlsx";
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


                //if (selectduration == "Previous Month")
                //{
                //    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                //    TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                //}
                //else if (selectduration == "Current Month")
                //{
                //    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                //    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                //}
                //else if (selectduration == "Yesterday")
                //{
                //    FDate = DateTime.Parse(selectfromdate).ToString("dd/MM/yyyy");
                //    TDate = DateTime.Parse(selecttodate).ToString("dd/MM/yyyy");
                //}
                //else
                //{
                //    FDate = DateTime.Parse(selectfromdate).AddDays(1).ToString("dd/MM/yyyy");
                //    TDate = DateTime.Parse(selecttodate).AddDays(1).ToString("dd/MM/yyyy");
                //}
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

                List<CheckInReport> result = (List<CheckInReport>)Session["CheckInReportData"];
                DataTable dt = ToDataTable(result);

                string TotalAppIn = Convert.ToString(dt.Rows[0]["AppTotal"]);
                string TotalPassesIn = Convert.ToString(dt.Rows[0]["PassTotal"]);
                string TotalOperatorIn = Convert.ToString(dt.Rows[0]["OperatorTotal"]);
                string TotalCallIn = Convert.ToString(dt.Rows[0]["CallPayTotal"]);
                string OutTotal = Convert.ToString(dt.Rows[0]["OutTotal"]);
                string FOCTotal = Convert.ToString(dt.Rows[0]["FOCTotal"]);

                if (dt.Rows.Count > 0)
                {
                    #region New Code
                    using (XLWorkbook wb = new XLWorkbook())
                    {

                        wb.Worksheets.Add("Check In Report", 0);
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Value = companydata.AccountName;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.FontSize = 12;
                        wb.Worksheets.Worksheet(0).Cell(1, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(2, 1).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(2, 20).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(3, 1).Value = "Check In Report";
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(3, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 1).Value = "From : " + FDate + " To : " + TDate;
                        wb.Worksheets.Worksheet(0).Cell(4, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 5).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 6).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 6).Value = "Cur.Date : " + DateTime.Now.ToString("dd/MM/yyy").Split(' ')[0];
                        wb.Worksheets.Worksheet(0).Cell(4, 6).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(6, 1).Value = "Station";
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "Parking Lot";
                        // wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "Lot Code";
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "App";
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 4).Value = "Pass";
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 5).Value = "Operator";
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 6).Value = "Call Pay";
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 7).Value = "Out";
                        wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 8).Value = "FOC";
                        wb.Worksheets.Worksheet(0).Cell(6, 8).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 8).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 9).Value = "Total";
                        wb.Worksheets.Worksheet(0).Cell(6, 9).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 9).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        int row = 7;
                        int column_num = 0;


                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Value = Convert.ToString(dt.Rows[i]["Station"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Value = Convert.ToString(dt.Rows[i]["ParkingLot"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Value = Convert.ToString(dt.Rows[i]["App"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Value = Convert.ToString(dt.Rows[i]["Pass"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Value = Convert.ToString(dt.Rows[i]["Operator"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Value = Convert.ToString(dt.Rows[i]["CallPay"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Value = Convert.ToString(dt.Rows[i]["Out"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 8).Value = Convert.ToString(dt.Rows[i]["FOC"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 8).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 9).Value = Convert.ToString(dt.Rows[i]["Total"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 9).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            row = row + 1;
                        }

                        // row = row + 1;
                        wb.Worksheets.Worksheet(0).Cell(row, 2).Value = "Total : ";
                        wb.Worksheets.Worksheet(0).Cell(row, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 2).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 3).Value = TotalAppIn;
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 3).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 4).Value = TotalPassesIn;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 5).Value = TotalOperatorIn;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 6).Value = TotalCallIn;
                        wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 6).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 7).Value = OutTotal;
                        wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 7).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 8).Value = FOCTotal;
                        wb.Worksheets.Worksheet(0).Cell(row, 8).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 8).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 9).Value = GrandTotal;
                        wb.Worksheets.Worksheet(0).Cell(row, 9).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 9).Style.Font.FontSize = 11;


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
                return Redirect("CheckInReport");
            }
            catch (Exception ex)
            {
                // objExceptionlog.InsertException("Portal", ex.Message, "ReportController", "", "StationReportExcelDownload");
                return Redirect("CheckInReport");
            }
        }
        public string GenerateReportNew(DataTable dt, string grandTotal, string selectstation, string selectLot, string FDate, string TDate, string TotalAppIn, string TotalPassesIn, string TotalOperatorIn, string TotalCallIn, string OutTotal, string FOCTotal)
        {
            Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//15012021

            string filename, AttachmentName1 = "";
            string path = System.Web.HttpContext.Current.Server.MapPath("~/PDFReports");
            AttachmentName1 = "Check In Report" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".pdf";
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
            // cell = ImageCell("~/Images/" + companydata.CompanyLogo, 20f, PdfPCell.ALIGN_CENTER);
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
            phrase.Add(new Chunk("CHECK IN REPORT", _bf_headingTitle));
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
            phrase.Add(new Chunk(selectstation + ", " + selectLot, _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tablefilter.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk(selectstation, _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            ////cell.PaddingRight = 1f;
            ////cell.PaddingLeft = 1f;
            ////cell.PaddingTop = 1f;
            ////cell.PaddingBottom = 1f;
            //cell.BorderColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#dbdbdb"));
            ////cell.Border = 0;
            ////cell.Colspan = 10;
            //tablefilter.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk(selectLot, _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.BorderColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#dbdbdb"));
            //tablefilter.AddCell(cell);

            ////phrase = new Phrase();
            ////phrase.Add(new Chunk(selectLot, _bf_heading1));
            ////cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //////cell.PaddingRight = 1f;
            //////cell.PaddingLeft = 1f;
            //////cell.PaddingTop = 1f;
            //////cell.PaddingBottom = 1f;
            ////cell.BorderColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#dbdbdb"));
            //////cell.Border = 0;
            //////cell.Colspan = 10;
            ////tablefilter.AddCell(cell);

            ////phrase = new Phrase();
            ////phrase.Add(new Chunk(selectVehicle, _bf_heading1));
            ////cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            ////cell.BorderColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#dbdbdb"));
            ////tablefilter.AddCell(cell);


            //phrase = new Phrase();
            //phrase.Add(new Chunk("", _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.Border = 0;
            ////cell.Colspan = 10;
            //tablefilter.AddCell(cell);

            //phrase = new Phrase();
            //phrase.Add(new Chunk("", _bf_heading1));
            //cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            //cell.Border = 0;
            ////cell.Colspan = 10;
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
            PdfPTable table2 = new PdfPTable(dt.Columns.Count - 6);
            table2.SetWidths(new float[] { 0.2f, 0.2f, 0.15f, 0.15f, 0.15f, 0.15f, 0.13f, 0.13f, 0.18f });
            table2.TotalWidth = doc.PageSize.Width - 40f;
            table2.LockedWidth = true;
            table2.SpacingBefore = 10f;
            //table2.SpacingAfter = 3f;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string cellText = Server.HtmlDecode(dt.Columns[i].ColumnName);
                if (cellText != "OperatorTotal" && cellText != "PassTotal" && cellText != "AppTotal" && cellText != "CallPayTotal"
                     && cellText != "OutTotal" && cellText != "FOCTotal")
                {
                    PdfPCell cell2 = new PdfPCell();
                    if (cellText == "Total")
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
                            // cell2.Phrase = new Phrase("Lot Code", _bf_headingboldTable);
                        }
                        else if (cellText == "CallPay")
                        {
                            cell2.Phrase = new Phrase("Call Pay", _bf_headingboldTable);
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
                    if (cellText != "OperatorTotal" && cellText != "PassTotal" && cellText != "AppTotal" && cellText != "CallPayTotal"
                        && cellText != "OutTotal" && cellText != "FOCTotal")
                    {
                        PdfPCell cell2 = new PdfPCell();
                        if (cellText == "Total")
                        {
                            cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                            cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                            // cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
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
            ////GRAND TOTAL TABLE
            //PdfPTable grandTable = new PdfPTable(dt.Columns.Count-4);
            //grandTable.TotalWidth = doc.PageSize.Width - 40f;
            //grandTable.LockedWidth = true;
            //PdfPCell cell3 = new PdfPCell();
            //cell3.Phrase = new Phrase("Total :         " + grandTotal, _bf_headingboldTable);
            //cell3.Colspan = dt.Columns.Count;
            //cell3.HorizontalAlignment = Element.ALIGN_RIGHT;
            ////cell3.Border = 0;
            //cell3.BorderWidthTop = 0;
            //grandTable.AddCell(cell3);
            ////GRNAD TOTAL TABLE
            ///

            PdfPTable grandTable = new PdfPTable(dt.Columns.Count - 6);
            grandTable.SetWidths(new float[] { 0.2f, 0.2f, 0.15f, 0.15f, 0.15f, 0.15f, 0.13f, 0.13f, 0.18f });
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
            cellt.Phrase = new Phrase(TotalAppIn, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(TotalPassesIn, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
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
            cellt.Phrase = new Phrase(TotalCallIn, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(OutTotal, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(FOCTotal, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_LEFT;
            cellt.BorderWidthRight = 0;
            cellt.BorderWidthTop = 0;
            grandTable.AddCell(cellt);

            cell = new PdfPCell();
            cellt.Phrase = new Phrase(grandTotal, _bf_headingboldTable);
            cellt.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellt.BorderWidthTop = 0;
            cellt.BorderWidthRight = 1;
            grandTable.AddCell(cellt);

            doc.Add(table2);

            doc.Add(grandTable);
            #endregion

            doc.Close();
            byte[] result = ms.ToArray();
            return filename;

        }
        #endregion

        #region Disposition Report
        public ActionResult DispositionReport()
        {
            ViewBag.Menu = "OperationalReports";
            return View();
        }
        #endregion

        #region FOC Report
        public ActionResult FOCReport()
        {
            ViewBag.Menu = "OperationalReports";
            return View();
        }
        public JsonNetResult GetActiveFOCReasonList()
        {
            IList<ViolationReason> focReasonList = operationalReportDAL_obj.GetActiveFOCReasonList();

            try
            {
                return new JsonNetResult()
                {
                    Data = focReasonList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        [HttpPost]
        public JsonResult GetFOCReport(SearchFilters FOCFilterData)
        {
            try
            {
                List<FOCReport> FOC_lst = operationalReportDAL_obj.GetFOCReport(FOCFilterData);
                Session["FOCReportData"] = FOC_lst;
                return new JsonResult()
                {
                    Data = FOC_lst,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult FOCReportPDFDownload(string SelectedItems)
        {
            try
            {
                var myDetails = JObject.Parse(SelectedItems);
                var selectedSupervisor = myDetails["SupervisorName"];
                var selectedStation = myDetails["SelectedStation"];
                var selectedLot = myDetails["SelectedLot"];
                string selectsupervisor = ((Newtonsoft.Json.Linq.JValue)selectedSupervisor).Value.ToString();
                string selectstation = ((Newtonsoft.Json.Linq.JValue)selectedStation).Value.ToString();
                string selectLot = ((Newtonsoft.Json.Linq.JValue)selectedLot).Value.ToString();

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

                List<FOCReport> result = (List<FOCReport>)Session["FOCReportData"];
                DataTable dt = ToDataTable(result);
                string Total = Convert.ToString(dt.Rows[0]["Total"]);
                string filename = GenerateFOCPDFReport(dt, selectsupervisor, selectstation, selectLot, FDate, TDate, Total);
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
                return Redirect("FOCReport");
            }
            catch (Exception ex)
            {
                return Redirect("FOCReport");
            }
        }
        public string GenerateFOCPDFReport(DataTable dt, string supervisorName, string selectstation, string selectLot, string FDate, string TDate, string Total)
        {
            Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//15012021

            string filename, AttachmentName1 = "";
            string path = System.Web.HttpContext.Current.Server.MapPath("~/PDFReports");
            AttachmentName1 = "FOC Report" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".pdf";
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
            phrase.Add(new Chunk("FOC REPORT", _bf_headingTitle));
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
            phrase.Add(new Chunk(supervisorName + ", " + selectstation + ", " + selectLot, _bf_heading1));
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
            PdfPTable table2 = new PdfPTable(dt.Columns.Count - 1);
            table2.SetWidths(new float[] { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f });
            table2.TotalWidth = doc.PageSize.Width - 40f;
            table2.LockedWidth = true;
            table2.SpacingBefore = 10f;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string cellText = Server.HtmlDecode(dt.Columns[i].ColumnName);
                if (cellText != "Total")
                {
                    PdfPCell cell2 = new PdfPCell();
                    if (cellText == "DueAmount")
                    {
                        cell2.Phrase = new Phrase("Due Amount", _bf_headingboldTable);
                        cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                        cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    }
                    else
                    {
                        if (cellText == "ParkingLot")
                        {
                            cell2.Phrase = new Phrase("Parking Lot", _bf_headingboldTable);
                        }
                        else if (cellText == "FOCReason")
                        {
                            cell2.Phrase = new Phrase("FOC Reason", _bf_headingboldTable);
                        }
                        else if (cellText == "FOCCount")
                        {
                            cell2.Phrase = new Phrase("No of FOCs", _bf_headingboldTable);
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
                    if (cellText != "Total")
                    {
                        PdfPCell cell2 = new PdfPCell();
                        if (cellText == "DueAmount")
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
            PdfPTable grandTable = new PdfPTable(dt.Columns.Count - 1);
            grandTable.TotalWidth = doc.PageSize.Width - 40f;
            grandTable.LockedWidth = true;
            PdfPCell cell3 = new PdfPCell();
            cell3.Phrase = new Phrase("Grand Total :                   " + Total, _bf_headingboldTable);
            cell3.Colspan = dt.Columns.Count;
            cell3.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell3.BorderWidthTop = 0;
            grandTable.AddCell(cell3);
            //GRNAD TOTAL TABLE

            doc.Add(table2);

            doc.Add(grandTable);
            #endregion

            doc.Close();
            byte[] result = ms.ToArray();
            return filename;

        }
        public ActionResult FOCReportExcelDownload(string SelectedItems)
        {
            try
            {
                Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//18012021

                string path = System.Web.HttpContext.Current.Server.MapPath("~/ExcelReports");
                string filename = "FOC Report_" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".xlsx";
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

                List<FOCReport> result = (List<FOCReport>)Session["FOCReportData"];
                DataTable dt = ToDataTable(result);
                string Total = Convert.ToString(dt.Rows[0]["Total"]);

                if (dt.Rows.Count > 0)
                {
                    #region New Code
                    using (XLWorkbook wb = new XLWorkbook())
                    {

                        wb.Worksheets.Add("FOC Report", 0);
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Value = companydata.AccountName;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.FontSize = 12;
                        wb.Worksheets.Worksheet(0).Cell(1, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(2, 1).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(2, 20).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(3, 1).Value = "FOC Report";
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(3, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 1).Value = "From : " + FDate + " To : " + TDate;
                        wb.Worksheets.Worksheet(0).Cell(4, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 5).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 6).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 6).Value = "Cur.Date : " + DateTime.Now.ToString("dd/MM/yyy").Split(' ')[0];
                        wb.Worksheets.Worksheet(0).Cell(4, 6).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(6, 1).Value = "Station";
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "Parking Lot";
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "FOC Reason";
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 4).Value = "No of FOCs";
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 5).Value = "Due Amount";
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

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Value = Convert.ToString(dt.Rows[i]["FOCReason"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Value = Convert.ToString(dt.Rows[i]["FOCCount"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Value = Convert.ToString(dt.Rows[i]["DueAmount"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            row = row + 1;
                        }

                        wb.Worksheets.Worksheet(0).Cell(row, 4).Value = "Total : ";
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 4).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 5).Value = Total;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 5).Style.Font.FontSize = 11;


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
                return Redirect("FOCReport");
            }
            catch (Exception ex)
            {
                // objExceptionlog.InsertException("Portal", ex.Message, "ReportController", "", "StationReportExcelDownload");
                return Redirect("FOCReport");
            }
        }
        #endregion

        #region Allocations
        public ActionResult Allocations()
        {
            ViewBag.Menu = "OperationalReports";
            return View();
        }
        [HttpPost]
        public JsonResult GetAllAllocations(SearchFilters allocationsFilterData)
        {
            try
            {
                List<Allocations> allocations_lst = operationalReportDAL_obj.GetAllAllocations(allocationsFilterData);
                return new JsonResult()
                {
                    Data = allocations_lst,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult LogoutEmployeeLot(Allocations allocationsData)
        {
            try
            {
                string result = operationalReportDAL_obj.LogoutEmployee(allocationsData.EmpId, allocationsData.LoginTime);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Duplicates
        public ActionResult DuplicateEntries()
        {
            ViewBag.Menu = "OperationalReports";
            return View();
        }

        [HttpPost]
        public JsonResult GetDuplicateEntries(SearchFilters duplicateFilterData)
        {
            try
            {
                string fDate1 = Convert.ToString(duplicateFilterData.FromDate).Split(' ')[0];
                DateTime fTotalDate = Convert.ToDateTime(fDate1 + " 01:00");

                string tDate1 = Convert.ToString(duplicateFilterData.ToDate).Split(' ')[0];
                DateTime tTotalDate = Convert.ToDateTime(tDate1 + " 23:00");

                duplicateFilterData.FromDate = fTotalDate;
                duplicateFilterData.ToDate = tTotalDate;


                List<DuplicateEntries> duplicate_lst = operationalReportDAL_obj.GetDuplicateEntries(duplicateFilterData);
                Session["DuplicateRecordsData"] = duplicate_lst;
                return new JsonResult()
                {
                    Data = duplicate_lst,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult DuplicateRecordsPDFDownload(string SelectedItems)
        {
            try
            {
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

                List<DuplicateEntries> result = (List<DuplicateEntries>)Session["DuplicateRecordsData"];
                DataTable dt = ToDataTable(result);
                string Total = Convert.ToString(dt.Rows[0]["Total"]);
                string filename = PDFforDuplicateRecords(dt, FDate, TDate, Total);
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
                return Redirect("DuplicateEntries");
            }
            catch (Exception ex)
            {
                return Redirect("DuplicateEntries");
            }
        }
        public string PDFforDuplicateRecords(DataTable dt, string FDate, string TDate, string Total)
        {
            Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//15012021

            string filename, AttachmentName1 = "";
            string path = System.Web.HttpContext.Current.Server.MapPath("~/PDFReports");
            AttachmentName1 = "Duplicate Records" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".pdf";
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
            phrase.Add(new Chunk("Duplicate Records", _bf_headingTitle));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_CENTER);
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
            cell.Border = 0;
            tableTitle.AddCell(cell);
            doc.Add(tableTitle);
            //Title table end
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
            PdfPTable table2 = new PdfPTable(dt.Columns.Count - 1);
            table2.SetWidths(new float[] { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f });
            table2.TotalWidth = doc.PageSize.Width - 40f;
            table2.LockedWidth = true;
            table2.SpacingBefore = 10f;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string cellText = Server.HtmlDecode(dt.Columns[i].ColumnName);
                if (cellText != "Total")
                {
                    PdfPCell cell2 = new PdfPCell();
                    if (cellText == "Amount")
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
                        else if (cellText == "RegistrationNumber")
                        {
                            cell2.Phrase = new Phrase("Vehicle Number", _bf_headingboldTable);
                        }
                        else if (cellText == "ApplicationType")
                        {
                            cell2.Phrase = new Phrase("Application Type", _bf_headingboldTable);
                        }
                        else if (cellText == "TransactionDate")
                        {
                            cell2.Phrase = new Phrase("Transaction Date", _bf_headingboldTable);
                        }
                        else if (cellText == "NoofTimes")
                        {
                            cell2.Phrase = new Phrase("No of Times", _bf_headingboldTable);
                        }
                        else if (cellText == "GovtVehicle")
                        {
                            cell2.Phrase = new Phrase("Govt Vehicle", _bf_headingboldTable);
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
                    if (cellText != "Total")
                    {
                        PdfPCell cell2 = new PdfPCell();
                        if (cellText == "Amount")
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
            PdfPTable grandTable = new PdfPTable(dt.Columns.Count - 1);
            grandTable.TotalWidth = doc.PageSize.Width - 40f;
            grandTable.LockedWidth = true;
            PdfPCell cell3 = new PdfPCell();
            cell3.Phrase = new Phrase("Total :                   " + Total, _bf_headingboldTable);
            cell3.Colspan = dt.Columns.Count;
            cell3.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell3.BorderWidthTop = 0;
            grandTable.AddCell(cell3);
            //GRNAD TOTAL TABLE

            doc.Add(table2);
            doc.Add(grandTable);
            #endregion

            doc.Close();
            byte[] result = ms.ToArray();
            return filename;

        }
        public ActionResult DuplicateRecordsExcelDownload(string SelectedItems)
        {
            try
            {
                Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//18012021

                string path = System.Web.HttpContext.Current.Server.MapPath("~/ExcelReports");
                string filename = "Duplicate Records_" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".xlsx";
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

                List<DuplicateEntries> result = (List<DuplicateEntries>)Session["DuplicateRecordsData"];
                DataTable dt = ToDataTable(result);
                string Total = Convert.ToString(dt.Rows[0]["Total"]);

                if (dt.Rows.Count > 0)
                {
                    #region New Code
                    using (XLWorkbook wb = new XLWorkbook())
                    {

                        wb.Worksheets.Add("Duplicate Records", 0);
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Value = companydata.AccountName;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.FontSize = 12;
                        wb.Worksheets.Worksheet(0).Cell(1, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(2, 1).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(2, 20).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(3, 1).Value = "Duplicate Records";
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(3, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 1).Value = "From : " + FDate + " To : " + TDate;
                        wb.Worksheets.Worksheet(0).Cell(4, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 5).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 6).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 6).Value = "Cur.Date : " + DateTime.Now.ToString("dd/MM/yyy").Split(' ')[0];
                        wb.Worksheets.Worksheet(0).Cell(4, 6).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(6, 1).Value = "Operator";
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "Supervisor";
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Station";
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 4).Value = "Parking Lot";
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 5).Value = "Vehicle Number";
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 6).Value = "Status";
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.FontSize = 11;

                        //wb.Worksheets.Worksheet(0).Cell(6, 7).Value = "Application Type";
                        //wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.Bold = true;
                        //wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 7).Value = "Transaction Date";
                        wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 7).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 8).Value = "No of Times";
                        wb.Worksheets.Worksheet(0).Cell(6, 8).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 8).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 9).Value = "Govt Vehicle";
                        wb.Worksheets.Worksheet(0).Cell(6, 9).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 9).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 10).Value = "Violation";
                        wb.Worksheets.Worksheet(0).Cell(6, 10).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 10).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 11).Value = "Amount";
                        wb.Worksheets.Worksheet(0).Cell(6, 11).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 11).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(6, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        int row = 7;
                        int column_num = 0;

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Value = Convert.ToString(dt.Rows[i]["Operator"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Value = Convert.ToString(dt.Rows[i]["Supervisor"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Value = Convert.ToString(dt.Rows[i]["Station"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Value = Convert.ToString(dt.Rows[i]["ParkingLot"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Value = Convert.ToString(dt.Rows[i]["RegistrationNumber"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Value = Convert.ToString(dt.Rows[i]["Status"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Style.Font.FontSize = 11;

                            //wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Value = Convert.ToString(dt.Rows[i]["ApplicationType"]);
                            //wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Value = Convert.ToString(dt.Rows[i]["TransactionDate"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 7).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 8).Value = Convert.ToString(dt.Rows[i]["NoofTimes"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 8).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 9).Value = Convert.ToString(dt.Rows[i]["GovtVehicle"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 9).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 10).Value = Convert.ToString(dt.Rows[i]["Violation"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 10).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 11).Value = Convert.ToString(dt.Rows[i]["Amount"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 11).Style.Font.FontSize = 11;
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            row = row + 1;
                        }

                        wb.Worksheets.Worksheet(0).Cell(row, 10).Value = "Total : ";
                        wb.Worksheets.Worksheet(0).Cell(row, 10).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 10).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(row, 11).Value = Total;
                        wb.Worksheets.Worksheet(0).Cell(row, 11).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(row, 11).Style.Font.FontSize = 11;


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
                return Redirect("FOCReport");
            }
            catch (Exception ex)
            {
                // objExceptionlog.InsertException("Portal", ex.Message, "ReportController", "", "StationReportExcelDownload");
                return Redirect("FOCReport");
            }
        }
        public JsonResult DeleteDuplicateEntries(List<DuplicateEntries> duplicateList)
        {
            string resultmsg = string.Empty;

            try
            {
                resultmsg = operationalReportDAL_obj.DeleteDuplicateEntries(duplicateList, Convert.ToString(Session["UserID"]));
                return Json(resultmsg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(resultmsg, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region PassExpired Customers
        public ActionResult PassExpiryReport()
        {
            ViewBag.Menu = "OperationalReports";
            return View();
        }
        [HttpPost]
        public JsonNetResult GetPassExpiredCustomersList(SearchFilters passExpiryFilterData)
        {
            IList<PassExpiredCustomer> customersList = operationalReportDAL_obj.GetPassExpiredCustomersList(passExpiryFilterData);
            Session["PassExpiryReportData"] = customersList;
            try
            {
                return new JsonNetResult()
                {
                    Data = customersList,
                    MaxJsonLength = 2147483647
                };
            }
            catch (Exception ex)
            {
                return new JsonNetResult { Data = "Failed" };
            }
        }
        public ActionResult PassExpiryReportPDFDownload(string SelectedItems)
        {
            try
            {
                var myDetails = JObject.Parse(SelectedItems);
                var selectedDuration = myDetails["Duration"];
                var selectedVehicle = myDetails["SelectedVehicle"];
                string selectDuration = ((Newtonsoft.Json.Linq.JValue)selectedDuration).Value.ToString();
                string selectVehicle = ((Newtonsoft.Json.Linq.JValue)selectedVehicle).Value.ToString();


                List<PassExpiredCustomer> result = (List<PassExpiredCustomer>)Session["PassExpiryReportData"];
                DataTable dt = ToDataTable(result);
                string filename = GeneratePassExpiryPDFReport(dt, selectDuration, selectVehicle);
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
                return Redirect("PassExpiryReport");
            }
            catch (Exception ex)
            {
                return Redirect("PassExpiryReport");
            }
        }
        public string GeneratePassExpiryPDFReport(DataTable dt, string selectDuration, string selectVehicle)
        {
            Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//15012021

            string filename, AttachmentName1 = "";
            string path = System.Web.HttpContext.Current.Server.MapPath("~/PDFReports");
            AttachmentName1 = "Pass Expired Customers Report" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".pdf";
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
            phrase.Add(new Chunk("PASS EXPIRED CUSTOMERS REPORT", _bf_headingTitle));
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
            phrase.Add(new Chunk(selectDuration + ", " + selectVehicle, _bf_heading1));
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
            tabletime.SpacingAfter = 5f;


            phrase = new Phrase();
            phrase.Add(new Chunk("Generated On :  ", _bf_headingboldTable));
            phrase.Add(new Chunk(DateTime.Now.ToString("dd/MM/yyyy HH:MM tt"), _bf_heading1));
            cell = PhraseCell(phrase, PdfPCell.ALIGN_LEFT);
            cell.Border = 0;
            tabletime.AddCell(cell);

            doc.Add(tabletime);
            #endregion

            #region Data
            PdfPTable table2 = new PdfPTable(dt.Columns.Count);
            table2.TotalWidth = doc.PageSize.Width - 40f;
            table2.LockedWidth = true;
            table2.SpacingBefore = 10f;
            table2.SpacingAfter = 3f;

            for (int i = 0; i < dt.Columns.Count; i++)
            {

                string cellText = Server.HtmlDecode(dt.Columns[i].ColumnName);
                PdfPCell cell2 = new PdfPCell();
                if (cellText == "PhoneNumber")
                {
                    cell2.Phrase = new Phrase("Phone Number", _bf_headingboldTable);
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                }
                else if (cellText == "VehicleType")
                {
                    cell2.Phrase = new Phrase("Vehicle Type", _bf_headingboldTable);
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                }
                else if (cellText == "VehicleNumber")
                {
                    cell2.Phrase = new Phrase("Vehicle Number", _bf_headingboldTable);
                    cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                }
                else if (cellText == "TypeofPass")
                {
                    cell2.Phrase = new Phrase("Type of Pass", _bf_headingboldTable);
                    cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                }
                else if (cellText == "PassExpiryDate")
                {
                    cell2.Phrase = new Phrase("Pass Expiry Date", _bf_headingboldTable);
                    cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                }
                else
                {
                    cell2.Phrase = new Phrase(cellText, _bf_headingboldTable);
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                }
                cell2.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#f1f1f1"));
                table2.AddCell(cell2);
            }

            //writing table Data  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string cellText = Server.HtmlDecode(dt.Columns[j].ColumnName);
                    PdfPCell cell2 = new PdfPCell();
                    cell2.Phrase = new Phrase(dt.Rows[i][j].ToString(), _bf_headingboldTableNrml);
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    table2.AddCell(cell2);
                }
            }
            
            doc.Add(table2);

            #endregion

            doc.Close();
            byte[] result = ms.ToArray();
            return filename;

        }
        public ActionResult PassExpiryReportExcelDownload(string SelectedItems)
        {
            try
            {
                Account companydata = revenueReportsDAL_obj.GetCompanyInfoDetails();//18012021

                string path = System.Web.HttpContext.Current.Server.MapPath("~/ExcelReports");
                string filename = "Pass Expired Customers Report_" + " " + DateTime.Now.ToString("dd'_'MM'_'yyyy HH'_'mm'_'ss") + ".xlsx";
                string file_path = path + "/" + filename;

                var myDetails = JObject.Parse(SelectedItems);

                List<PassExpiredCustomer> result = (List<PassExpiredCustomer>)Session["PassExpiryReportData"];
                DataTable dt = ToDataTable(result);


                if (dt.Rows.Count > 0)
                {
                    #region New Code
                    using (XLWorkbook wb = new XLWorkbook())
                    {

                        wb.Worksheets.Add("Pass Expired Customers Report", 0);
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Value = companydata.AccountName;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(1, 1).Style.Font.FontSize = 12;
                        wb.Worksheets.Worksheet(0).Cell(1, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(2, 1).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(2, 20).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(3, 1).Value = "Pass Expired Customers Report";
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(3, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(3, 4).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 1).Value = "Cur.Date : " + DateTime.Now.ToString("dd/MM/yyy").Split(' ')[0];
                        wb.Worksheets.Worksheet(0).Cell(4, 1).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 5).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 6).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(4, 7).Value = "";
                        wb.Worksheets.Worksheet(0).Cell(4, 7).Style.Font.FontSize = 11;
                        wb.Worksheets.Worksheet(0).Cell(4, 2).IsMerged();

                        wb.Worksheets.Worksheet(0).Cell(6, 1).Value = "Name";
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 1).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 2).Value = "Phone Number";
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 2).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 3).Value = "Vehicle Type";
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 3).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 4).Value = "Vehicle Number";
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 4).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 5).Value = "Type of Pass";
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 5).Style.Font.FontSize = 11;

                        wb.Worksheets.Worksheet(0).Cell(6, 6).Value = "Pass Expiry Date";
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.Bold = true;
                        wb.Worksheets.Worksheet(0).Cell(6, 6).Style.Font.FontSize = 11;

                        int row = 7;
                        int column_num = 0;


                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Value = Convert.ToString(dt.Rows[i]["Name"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 1).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Value = Convert.ToString(dt.Rows[i]["PhoneNumber"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 2).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Value = Convert.ToString(dt.Rows[i]["VehicleType"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 3).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Value = Convert.ToString(dt.Rows[i]["VehicleNumber"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 4).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Value = Convert.ToString(dt.Rows[i]["TypeofPass"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 5).Style.Font.FontSize = 11;

                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Value = Convert.ToString(dt.Rows[i]["PassExpiryDate"]);
                            wb.Worksheets.Worksheet(0).Cell(row, column_num + 6).Style.Font.FontSize = 11;

                            row = row + 1;
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
                return Redirect("CheckInReport");
            }
            catch (Exception ex)
            {
                // objExceptionlog.InsertException("Portal", ex.Message, "ReportController", "", "StationReportExcelDownload");
                return Redirect("CheckInReport");
            }
        }
        #endregion

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
        private PdfPCell ImageCell(string path, float scale, int align)
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
