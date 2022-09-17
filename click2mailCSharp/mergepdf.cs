using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
namespace c2mAPI
{
    internal class MergePDF
    {
        

        public static void MargeMultiplePDF(string[] PDFfileNames, FileInfo OutputFile)
        {
            // Create document object  
            PdfDocument pdf = new PdfDocument(new PdfWriter(OutputFile));
            PdfMerger merger = new PdfMerger(pdf);

            //Add pages from the first document
            foreach (String s in PDFfileNames)
            {
                PdfDocument firstSourcePdf = new PdfDocument(new PdfReader(s));
                merger.Merge(firstSourcePdf, 1, firstSourcePdf.GetNumberOfPages());
                firstSourcePdf.Close();
            }
            pdf.Close();
        }// Disposes the Object of FileStream  


        public static void MergeMultiplePDFJobs(List<Jobs> jobs, FileInfo OutputFile)
        {
            // Create document object  
            PdfDocument pdf = new PdfDocument(new PdfWriter(OutputFile));
            PdfMerger merger = new PdfMerger(pdf);

            //Add pages from the first document
            int start = 0;
            
            foreach (c2mAPI.Jobs s in jobs)
            {
                start++;
                PdfDocument firstSourcePdf = new PdfDocument(new PdfReader(s.MergeFile));
                merger.Merge(firstSourcePdf, 1, firstSourcePdf.GetNumberOfPages());
                
                s.StartingPage = start;
                s.EndingPage = start + firstSourcePdf.GetNumberOfPages() -1;
                start = s.EndingPage;
                firstSourcePdf.Close();
            }
            pdf.Close();
        }// Disposes the Object of FileStream  
    }
}
