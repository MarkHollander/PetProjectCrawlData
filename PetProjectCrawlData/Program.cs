using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Fizzler;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using DocumentFormat.OpenXml;

namespace PetProjectCrawlData
{
    class Program
    {
        private static List<object> items { get; set; }         
        static void Main(string[] args)
        {
            HtmlWeb htmlWeb = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8
            };
            HtmlDocument htmlDocument = htmlWeb.Load("http://kenh14.vn/doi-song.chn");
            
            items = new List<object>();
            string crawlMethod = ConfigurationManager.AppSettings["CrawlMethod"];
            switch (crawlMethod)
            {
                case "LambdaExpression":
                    CrawlWithLambdaExpression(htmlDocument);
                    break;

                case "Fizzler":
                    CrawlWithFizzler(htmlDocument);
                    break;
            }

            string filepath = Directory.GetCurrentDirectory() + "/OutputExcel";
            DirectoryInfo di = new DirectoryInfo(filepath);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "mySheet" };
            sheets.Append(sheet);

            workbookpart.Workbook.Save();

            // Close the document.
            spreadsheetDocument.Close();
        }

        public static void CrawlWithLambdaExpression(HtmlDocument htmlDocument)
        {
            List<HtmlNode> threadItems = htmlDocument.DocumentNode.Descendants("ul")
                            .First(nodes => nodes.Attributes.Contains("class") && nodes.Attributes["class"].Value == "ktnc-list")
                            .ChildNodes.Where(nodes => nodes.Attributes.Contains("class") && nodes.Attributes["class"].Value == "ktncli")
                            .ToList();
            foreach (var item in threadItems)
            {
                HtmlNode linkNode = item.Descendants("a").First(nodes => nodes.Attributes.Contains("class") && nodes.Attributes["Class"].Value == "ktncli-ava");
                string text = linkNode.InnerText;
                string link = linkNode.Attributes["href"].Value;
                DateTime currentDate = DateTime.Now.Date;
                items.Add(new { currentDate, text, link });
            }
        }

        public static void CrawlWithFizzler(HtmlDocument htmlDocument)
        {
            List<HtmlNode> threadItems = htmlDocument.DocumentNode.QuerySelector("ul.ktnc-list").QuerySelectorAll(".ktncli").ToList();
            foreach (var item in threadItems)
            {
                var linkNode = item.QuerySelector(".ktncli-ava");
                string text = linkNode.InnerText;
                string link = linkNode.Attributes["href"].Value;
                DateTime currentDate = DateTime.Now.Date;
                items.Add(new { currentDate, text, link });
            }
        }
    }
}
