﻿using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace pdfMerge
{
    class Program
    {
        static void Main(string[] args)
        {
            //string sourcePdFpath = @"C:\TEMP\pdf\original\scan.pdf";
            //string outputPdFpath = @"C:\TEMP\pdf\split\1.pdf";
            //ExtractPages(sourcePdFpath, outputPdFpath, 1, 6);

            MegreMyImages();
        }

        public static void MegreMyPdfs()
        {
            string pdfFileLocation = @"C:\TEMP\pdf\split\";
            int numberOfPdfs = 2;
            string outFile = @"C:\TEMP\pdf\split\merge.pdf";
            Merge(pdfFileLocation, numberOfPdfs, outFile, false);
        }

        public static void MegreMyImages()
        {
            var pathToImages = "/Users/vitalidonghvani/Code/pdf-merge/pdfMergeCore/pdfMerge/bin/Debug/netcoreapp2.0/";
            var imagesAB = new List<string> { pathToImages+ "a.JPG", pathToImages + "b.JPG" };
            CombineMultipleImagesIntoPdf(imagesAB, "ab.pdf");

            var imageC = new List<string> { pathToImages + "c.JPG"};
            CombineMultipleImagesIntoPdf(imageC, "c.pdf");
        }

        public static void Merge(string pdfFileLocation, int numberOfPdfs, string outFile, bool zeroIndex = true)
        {
            Console.WriteLine("Started pdf merger");
            
            List<string> pdfs = new List<string>();
            for (int docIndex = 0 + (zeroIndex ? 0: 1); docIndex < numberOfPdfs + 1; docIndex++)
            {
                string fileName = $"{docIndex}.pdf";
                pdfs.Add(pdfFileLocation + fileName);
            }
            CombineMultiplePdFs(pdfs, outFile);
        }

        public static void CombineMultipleImagesIntoPdf(List<string> imagefiles, string outFile)
        {
            if (File.Exists(outFile))
            {
                File.Delete(outFile);
            }
            // step 1: creation of a document-object
            Document document = new Document();
            document.SetMargins(0, 0, 0, 0);

            // step 2: we create a writer that listens to the document
            FileStream fs = new FileStream(outFile, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);

            // step 3: we open the document
            document.Open();

            //step 4: we add images
            foreach (string fileName in imagefiles)
            {
                //document.Add(new Paragraph("Hello World!"));
                //document.NewPage();
                Image jpg = Image.GetInstance(fileName);
                jpg.RotationDegrees = -90;
                // x 
                //jpg.ScaleToFit(842f, 595f);
                jpg.ScaleToFit(800f, 840f);
                //jpg.ScaleToFit(140f, 120f);
                document.Add(jpg);
                Console.WriteLine("added file: " + fileName);
            }

            // step 5: we close the document and writer
            document.Close();
            writer.Close();            
            fs.Close();
        }

        public static void CombineMultiplePdFs(List<string> fileNames, string outFile)
        {
            // step 1: creation of a document-object
            Document document = new Document();

            // step 2: we create a writer that listens to the document
            PdfCopy writer = new PdfCopy(document, new FileStream(outFile, FileMode.Create));

            // step 3: we open the document
            document.Open();

            foreach (string fileName in fileNames)
            {
                // we create a reader for a certain document
                PdfReader reader = new PdfReader(fileName);
                reader.ConsolidateNamedDestinations();

                // step 4: we add content
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    PdfImportedPage page = writer.GetImportedPage(reader, i);
                    writer.AddPage(page);
                }

                PrAcroForm form = reader.AcroForm;
                if (form != null)
                {
                    //writer.CopyDocumentFields(reader);
                    writer.CopyAcroForm(reader);
                }

                reader.Close();
                Console.WriteLine("added file: " + fileName);
            }

            // step 5: we close the document and writer
            writer.Close();
            document.Close();
        }

        private static void ExtractPages(string sourcePdFpath, string outputPdFpath, int startpage, int endpage)
        {
            var reader = new PdfReader(sourcePdFpath);
            var sourceDocument = new Document(reader.GetPageSizeWithRotation(startpage));
            var pdfCopyProvider = new PdfCopy(sourceDocument, new FileStream(outputPdFpath, FileMode.Create));

            sourceDocument.Open();

            for (int index = startpage; index <= endpage; index++)
            {
                var importedPage = pdfCopyProvider.GetImportedPage(reader, index);
                pdfCopyProvider.AddPage(importedPage);
            }
            sourceDocument.Close();
            reader.Close();
        }
    }
}
