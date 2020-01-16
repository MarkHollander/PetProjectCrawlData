using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fizzler;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

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
