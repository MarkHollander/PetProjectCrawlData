using System;
using System.Collections.Generic;
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
        static void Main(string[] args)
        {
            HtmlWeb htmlWeb = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8
            };
            HtmlDocument htmlDocument = htmlWeb.Load("http://kenh14.vn/doi-song.chn");
            var ThreadItems = htmlDocument.DocumentNode.Descendants("ul")
                            .First(nodes => nodes.Attributes.Contains("class") && nodes.Attributes["class"].Value == "ktnc-list")
                            .ChildNodes.Where(nodes => nodes.Attributes.Contains("class") && nodes.Attributes["class"].Value == "ktncli")
                            .ToList();
            foreach (var item in ThreadItems)
            {

            }
        }
    }
}
