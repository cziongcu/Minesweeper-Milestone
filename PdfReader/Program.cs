using System;
using System.IO;
using UglyToad.PdfPig;

namespace PdfReader
{
    class Program
    {
        static void Main(string[] args)
        {
            string pdfPath = @"C:\Users\chris\source\repos\CST-250\MineSweeperClassLib\CST-250-RS-ProjectMilestoneGuidev2.pdf";
            using (PdfDocument document = PdfDocument.Open(pdfPath))
            {
                foreach (var page in document.GetPages())
                {
                    Console.WriteLine(page.Text);
                }
            }
        }
    }
}
