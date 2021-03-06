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
    public partial class frmChooseStatement : Form
    {
        private static string _cardNo;
        public string CardNo { get => _cardNo; set => _cardNo = value; }
        Log_BUL log_BUL = new Log_BUL();
        Account_BUL account_BUL = new Account_BUL();
        SubStringDate sub = new SubStringDate();
        public frmChooseStatement()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Hiển thị thông tin lịch sử giao dịch lên màn hình
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDisplayScreen_Click(object sender, EventArgs e)
        {
            frmViewHistory viewHistory = new frmViewHistory();
            viewHistory.CardNo = CardNo;
            viewHistory.Show();
            this.Close();
        }
        public List<Log> DisplayHistory()
        {
            var listHistory =  log_BUL.GetListLog(CardNo);
            return listHistory;
        }
        //In lịch sử giao dịch
        private void btnPrintPdf_Click(object sender, EventArgs e)
        {
            var history = DisplayHistory();
            var balance = account_BUL.GetBalance(CardNo).ToString() + " VND";
            var balanceRight = account_BUL.GetBalanceRight(CardNo).ToString() + " VND";
            FileStream fs = new
                FileStream(@"F:\SystemATM\FITHAUI.ATMSystem\cash_transfer.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            iTextSharp.text.Rectangle rec =
                new iTextSharp.text.Rectangle(240, 340);
            rec.BackgroundColor = new BaseColor(System.Drawing.Color.WhiteSmoke);
            Document doc = new Document(rec, 14, 14, 22.6f, 22.6f);
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();
            iTextSharp.text.Font headerFont = FontFactory.GetFont("Verdana", 8);
            iTextSharp.text.Font emptyFont = FontFactory.GetFont("Verdana", 5);
            //Ảnh header
            string imageURL = @"F:\SystemATM\FITHAUI.ATMSystem\FITHAUI.ATMSystem.UI\Content\Images\Logo.png";
            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
            jpg.Alignment = Element.ALIGN_CENTER;
            jpg.ScaleToFit(80f, 120f);
            Paragraph receiptNamePara =
                new Paragraph("LIET KE GIAO DICH", headerFont);
            Paragraph emptyPara = new Paragraph("    ", emptyFont);
            receiptNamePara.Alignment = Element.ALIGN_CENTER;
            doc.Add(jpg);

            doc.Add(receiptNamePara);
            doc.Add(emptyPara);
            PdfPTable table = new PdfPTable(3);
            table.HorizontalAlignment = Element.ALIGN_CENTER;
            table.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            table.WidthPercentage = 95f;
            var dateNow = sub.SubDate(DateTime.Now.ToString().Trim());
            var time = sub.SubTime(DateTime.Now.ToString().Trim());
            PdfPCell pDays = new PdfPCell(new Phrase(String.Format("NGAY                       :  {0}        GIO     {1}", dateNow, time), headerFont));
            pDays.Colspan = 3;
            pDays.Border = iTextSharp.text.Rectangle.NO_BORDER;
            PdfPCell listHistory = new PdfPCell(new Phrase(String.Format("              LIET KE GIA0 DICH\n",""), headerFont));
            listHistory.Colspan = 3;
            listHistory.Border = iTextSharp.text.Rectangle.NO_BORDER;
            PdfPCell cCardNo = new PdfPCell(new Phrase(String.Format("SO TAI KHOAN                       :  {0}", CardNo), headerFont));
            cCardNo.Colspan = 3;
            cCardNo.Border = iTextSharp.text.Rectangle.NO_BORDER;
            PdfPCell cTransNo = new PdfPCell(new Phrase(String.Format("SO DU CHO PHEP                         :  {0}", ""+balanceRight.ToString()+""), headerFont));
            cTransNo.Colspan = 3;
            cTransNo.Border = iTextSharp.text.Rectangle.NO_BORDER;
            PdfPCell cAvailBal = new PdfPCell(new Phrase(String.Format("SO DU KHA DUNG     :  {0}", balance.ToString()), headerFont));
            cAvailBal.Colspan = 3;
            cAvailBal.Border = iTextSharp.text.Rectangle.NO_BORDER;
            PdfPCell cFee = new PdfPCell(new Phrase(String.Format("PHI DICH VU:  {0} VND", "1100"), headerFont));
            cFee.Colspan = 3;
            cFee.Border = iTextSharp.text.Rectangle.NO_BORDER;
            PdfPCell cVAT = new PdfPCell(new Phrase("(DA BAO GOM VAT)", headerFont));
            cVAT.Colspan = 3;
            cVAT.Border = iTextSharp.text.Rectangle.NO_BORDER;
            cVAT.Rowspan = 5;
            List logs = new List();
            table.AddCell(pDays);
            table.AddCell(listHistory);
            table.AddCell(cCardNo);
            table.AddCell(cTransNo);
            table.AddCell(cAvailBal);
            foreach (var item in history)
            {
                var dateFomat = sub.SubDate(item.LogDate.ToString());
                PdfPCell pHistory = new PdfPCell(new Phrase(String.Format("{0}             {1}             {2}\n", dateFomat, item.Description.ToString(), item.Amount.ToString()), headerFont));
                pHistory.Colspan = 3;
                pHistory.Border = iTextSharp.text.Rectangle.NO_BORDER;
                table.AddCell(pHistory);
            }
            table.AddCell(cFee);
            table.AddCell(cVAT);
            doc.Add(table);

            FileStream fs1 = new FileStream(@"F:\SystemATM\FITHAUI.ATMSystem\FITHAUI.ATMSystem.UI\Content\Images\techcombank_bg.png", FileMode.Open);
            iTextSharp.text.Image watermark = iTextSharp.text.Image.GetInstance(System.Drawing.Image.FromStream(fs1), ImageFormat.Png);
            watermark.ScalePercent(40f, 43f);
            watermark.SetAbsolutePosition(-10f, 0f);
            fs1.Close();
            doc.Add(watermark);
            doc.Close();
            MessageBox.Show("GIAO DỊCH THÀNH CÔNG");
            Application.Exit();
            try
            {
                Process myProcess = new Process();
                Process.Start(@"F:\PHAN MEM LAP\SumatraPDF-3.1.2-64\SumatraPDF.exe", @"F:\SystemATM\FITHAUI.ATMSystem\cash_transfer.pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
