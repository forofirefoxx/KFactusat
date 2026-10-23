using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.qrcode;
using System;
using System.Collections.Generic;
using System.IO;

namespace KFactuSat.Sat.utils
{
    public class QR
    {
        public QR() { }

        public static byte[] createBarCode(String Cadena)
        {

            byte[] array = null;
            byte[] array1 = null;


            using (MemoryStream ms = new MemoryStream())
            {

                Document doc = new Document(PageSize.A9, 30f, 30f, 30f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var hints = new Dictionary<EncodeHintType, object>();
                hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
                //Creamos el QR
                BarcodeQRCode qrcode = new BarcodeQRCode(Cadena, 1000, 1000,null);
                Image qrcodeImage = qrcode.GetImage();
                qrcodeImage.ScaleAbsolute(512, 512);
                doc.Add(qrcodeImage);
                doc.Close();
                byte[] result = ms.ToArray();
                array = result;
            }


            try
            {
                // Get a List of Image
                List<System.Drawing.Image> ListImage = ExtractImages(array);

                for (int i = 0; i < ListImage.Count; i++)
                {
                    try
                    {

                        array1 = imageToByteArray(ListImage[i]);
                    }
                    catch (Exception)
                    { }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return array1;
        }

        public static byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        private static List<System.Drawing.Image> ExtractImages(byte[] PDFSourcePath)
        {
            List<System.Drawing.Image> ImgList = new List<System.Drawing.Image>();

            PdfReader PDFReaderObj = null;
            PdfObject PDFObj = null;
            PdfStream PDFStremObj = null;

            try
            {

                PDFReaderObj = new iTextSharp.text.pdf.PdfReader(PDFSourcePath, null);

                for (int i = 0; i <= PDFReaderObj.XrefSize - 1; i++)
                {
                    PDFObj = PDFReaderObj.GetPdfObject(i);

                    if ((PDFObj != null) && PDFObj.IsStream())
                    {
                        PDFStremObj = (PdfStream)PDFObj;
                        iTextSharp.text.pdf.PdfObject subtype = PDFStremObj.Get(iTextSharp.text.pdf.PdfName.SUBTYPE);

                        if ((subtype != null) && subtype.ToString() == iTextSharp.text.pdf.PdfName.IMAGE.ToString())
                        {
                            try
                            {

                                iTextSharp.text.pdf.parser.PdfImageObject PdfImageObj =
                         new iTextSharp.text.pdf.parser.PdfImageObject((iTextSharp.text.pdf.PRStream)PDFStremObj);

                                System.Drawing.Image ImgPDF = PdfImageObj.GetDrawingImage();


                                ImgList.Add(ImgPDF);

                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
                PDFReaderObj.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ImgList;
        }
        /***************************/


    }
}
