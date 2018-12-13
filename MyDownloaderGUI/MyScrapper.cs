using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDownloaderGUI
{
    public class MyScrapper
    {
        static HtmlNodeCollection GetNodes(HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.SelectNodes(@"/html/body/pre/a");
            if (nodes == null)
            {
                nodes = doc.DocumentNode.SelectNodes(@"/html/body/table/tbody/tr/td/a");
                if(nodes == null)
                {
                    nodes = doc.DocumentNode.SelectNodes(@"/html/body/main/div/div[2]/div[4]/table/tbody/tr/td/a");
                    return nodes;
                }
                return nodes;
            }
            else return nodes;
        }
        
        public static List<string> Webscraper(string URL)
        {
            List<string> downloadLinks = new List<string>();
            List<string> folderLinks = new List<string>();
            
            var web = new HtmlWeb();
            folderLinks.Add(URL);

            var curIndex = 0;

            while (folderLinks.Count > curIndex)
            {
                var url = folderLinks[curIndex];

                var doc = web.Load(url);

                var Nodes= GetNodes(doc);
                if (Nodes == null)
                {

                }
                else
                {
                    foreach (var node in Nodes)
                    {
                        var href = node.Attributes["href"].Value;

                        if (href.StartsWith(".."))
                        {

                        }
                        else if (href.ToLower().EndsWith(".mkv"))
                        {
                            downloadLinks.Add(Path.Combine(url, href));
                        }
                        else if (href.ToLower().EndsWith(".mp4"))
                        {
                            downloadLinks.Add(Path.Combine(url, href));
                        }
                        else if (href.ToLower().EndsWith(".zip"))
                        {
                            downloadLinks.Add(Path.Combine(url, href));
                        }
                        else
                        {
                            folderLinks.Add(Path.Combine(url, href));
                        }

                    }
                }
                curIndex++;
            }

            return downloadLinks;

        }
    }
}
