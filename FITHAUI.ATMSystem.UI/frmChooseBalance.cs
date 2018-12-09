﻿using FITHAUI.ATMSystem.BULs;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FITHAUI.ATMSystem.UI
{
    public partial class frmChooseBalance : Form
    {
        private static string _cardNo;
        public string CardNo { get => _cardNo; set => _cardNo = value; }
        Account_BUL account_BUL = new Account_BUL();
        Log_BUL log = new Log_BUL();
        ATM_BUL atm_BUL = new ATM_BUL(); 
        SubStringDate sub = new SubStringDate();
        public frmChooseBalance()
        {
            InitializeComponent();
        }

        private void btnDisplayScreen_Click(object sender, EventArgs e)
        {
            frmBalanceScreen balanceScreen = new frmBalanceScreen();
            balanceScreen.CardNo = CardNo;
            this.Close();
            log.CreateLog(DateTime.Now, 550, "SUCCESS", "39137be2-0446-4688-be5a-862e94b8a6b9", "fc57dd25-0a60-427a-aaa5-f9d2059c8abb", CardNo, "");
            balanceScreen.Show();
        }
        private void btnPrintPdf_Click(object sender, EventArgs e)
        {
            string path = "F:/YEN/ATM/FITHAUI.ATMSystem.UI";
            var atm = atm_BUL.GetATMName();
            var balance = account_BUL.GetBalance(CardNo).ToString() + " VND";
            var balanceRight = account_BUL.GetBalanceRight(CardNo).ToString() + " VND";
            FileStream fs = new
                FileStream(path + "/pdf/Balance.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            iTextSharp.text.Rectangle rec =
                new iTextSharp.text.Rectangle(240, 340);
            rec.BackgroundColor = new BaseColor(System.Drawing.Color.WhiteSmoke);
            Document doc = new Document(rec, 14, 14, 22.6f, 22.6f);
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();
            iTextSharp.text.Font headerFont = FontFactory.GetFont("Verdana", 8);
            iTextSharp.text.Font emptyFont = FontFactory.GetFont("Verdana", 5);
            //Ảnh header
            string imageURL = path + "/Content/Images/Logo.png";
            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
            jpg.Alignment = Element.ALIGN_CENTER;
            jpg.ScaleToFit(80f, 120f);
            doc.Add(jpg);
            PdfPTable common = new PdfPTable(3);
            common.HorizontalAlignment = Element.ALIGN_CENTER;
            common.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            common.WidthPercentage = 95f;

            Paragraph emptyPara = new Paragraph("    ", emptyFont);
            var dateNow = sub.SubDate(DateTime.Now.ToString().Trim());
            var time = sub.SubTime(DateTime.Now.ToString().Trim());
            PdfPCell pDay = new PdfPCell(new Phrase(string.Format("NGAY                       :  {0}        GIO     {1}\n", dateNow, time), headerFont));
            pDay.Colspan = 3;
            pDay.Border = iTextSharp.text.Rectangle.NO_BORDER;
            PdfPCell pNameATM = new PdfPCell(new Phrase(string.Format("TEN MAY                       :  {0}", atm[0].ATMID), headerFont));
            pNameATM.Colspan = 3;
            pNameATM.Border = iTextSharp.text.Rectangle.NO_BORDER;
            PdfPCell pAddressATM = new PdfPCell(new Phrase(string.Format("DIA CHI                       :  {0}", atm[0].Address), headerFont));
            pAddressATM.Colspan = 3;
            pAddressATM.Border = iTextSharp.text.Rectangle.NO_BORDER;
            PdfPCell pCardNoATM = new PdfPCell(new Phrase(string.Format("SO THE                       :  {0}", CardNo), headerFont));
            pCardNoATM.Colspan = 3;
            pCardNoATM.Border = iTextSharp.text.Rectangle.NO_BORDER;
            PdfPCell pTrace = new PdfPCell(new Phrase(string.Format("SO TRACE                       :  {0}", "12345689"), headerFont));
            pTrace.Colspan = 3;
            pTrace.Border = iTextSharp.text.Rectangle.NO_BORDER;
            common.AddCell(pDay);
            common.AddCell(pNameATM);
            common.AddCell(pAddressATM);
            common.AddCell(pCardNoATM);
            common.AddCell(pTrace);
            doc.Add(common);

            Paragraph receiptNamePara = new Paragraph("XEM SO DU", headerFont);
            receiptNamePara.Alignment = Element.ALIGN_CENTER;
            doc.Add(receiptNamePara);
            doc.Add(emptyPara);

            PdfPTable table = new PdfPTable(3);
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            table.WidthPercentage = 95f;
            PdfPCell cCardNo = new PdfPCell(new Phrase(String.Format("SO TAI KHOAN                       :  {0}", CardNo), headerFont));
            cCardNo.Colspan = 3;
            cCardNo.Border = iTextSharp.text.Rectangle.NO_BORDER;
            PdfPCell cTransNo = new PdfPCell(new Phrase(String.Format("SO DU CHO PHEP                         :  {0}", "" + balanceRight.ToString() + ""), headerFont));
            cTransNo.Colspan = 3;
            cTransNo.Border = iTextSharp.text.Rectangle.NO_BORDER;
            PdfPCell cAvailBal = new PdfPCell(new Phrase(String.Format("SO DU KHA DUNG     :  {0}", balance.ToString()), headerFont));
            cAvailBal.Colspan = 3;
            cAvailBal.Border = iTextSharp.text.Rectangle.NO_BORDER;
            PdfPCell cFee = new PdfPCell(new Phrase(String.Format("PHI DICH VU:  {0} VND", "1100"), headerFont));
            cFee.Colspan = 3;
            cFee.Border = iTextSharp.text.Rectangle.NO_BORDER;
            PdfPCell cVAT = new PdfPCell(new Phrase("                               (DA BAO GOM VAT)", headerFont));
            cVAT.Colspan = 3;
            cVAT.Border = iTextSharp.text.Rectangle.NO_BORDER;
            cVAT.Rowspan = 5;
            List logs = new List();
            table.AddCell(cCardNo);
            table.AddCell(cTransNo);
            table.AddCell(cAvailBal);
            table.AddCell(cFee);
            table.AddCell(cVAT);
            doc.Add(table);

            doc.Close();
            log.CreateLog(DateTime.Now, 550, "SUCCESS", "39137be2-0446-4688-be5a-862e94b8a6b9", "fc57dd25-0a60-427a-aaa5-f9d2059c8abb", CardNo, "");
            MessageBox.Show("GIAO DỊCH THÀNH CÔNG");
            Application.Exit();
            try
            {
                Process myProcess = new Process();
                Process.Start(path + "/pdfexe/SumatraPDF.exe", path + "/pdf/Balance.pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
