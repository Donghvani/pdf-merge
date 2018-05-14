using System;
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


            //string pdfFileLocation = @"C:\Temp\bookpages\";
            //int numberOfPdfs = 671;
            //string outFile = @"C:\Temp\bookpages\merged\merge.pdf";
            //Merge(pdfFileLocation, numberOfPdfs, outFile);
        }

        public static void Merge(string pdfFileLocation, int numberOfPdfs, string outFile)
        {
            Console.WriteLine("Started pdf merger");
            
            List<string> pdfs = new List<string>();
            for (int docIndex = 0; docIndex < numberOfPdfs + 1; docIndex++)
            {
                string fileName = $"{docIndex}.pdf";
                pdfs.Add(pdfFileLocation + fileName);
            }
            CombineMultiplePdFs(pdfs, outFile);
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

                PRAcroForm form = reader.AcroForm;
                if (form != null)
                {
                    writer.CopyDocumentFields(reader);
                }

                reader.Close();
                Console.WriteLine("added file: " + fileName);
            }

            // step 5: we close the document and writer
            writer.Close();
            document.Close();
        }
    }
}
