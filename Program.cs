using System;
using System.Net;
using HtmlAgilityPack;
using System.Collections.Generic;
namespace webGrader
{
    class Program
    {
        static HashSet<HtmlNode> viewedNodes = new HashSet<HtmlNode>();
        static int getPositionFromParent(HtmlNode n)
        {
            return n.ParentNode.ChildNodes.IndexOf(n);
        }
        static void Main(string[] args)
        {
            WebpageEvaluation evaluation = new WebpageEvaluation("Home.html", "Website_final/");
        }
    }
    class WebpageEvaluation
    {
        int paragraphs, headings, lists, tables, links, images, head, title, tr, td, th;
        //Dictionary<string, double> evaluations = new Dictionary<string, double>();
        string homeDirPath;
        HashSet<string> evaluatedURLs = new HashSet<string>();
        public WebpageEvaluation(string homeFile, string homeDirPath)
        {
            this.homeDirPath = homeDirPath;
            evaluateDocument(homeDirPath + homeFile);
            Console.WriteLine("Evaluation for : " + homeDirPath);
            printResult();
        }
        public void printResult()
        {
            Console.WriteLine("Results:");
            Console.WriteLine("paragraphs: " + paragraphs);
            Console.WriteLine("headings: " + headings);
            Console.WriteLine("heads: " + head);
            Console.WriteLine("links: " + links);
            Console.WriteLine("lists: " + lists);
            Console.WriteLine("images: " + images);
            Console.WriteLine("titles: " + title);
            Console.WriteLine("tables: " + tables);
            Console.WriteLine("table rows: " + tr);
            Console.WriteLine("table elements: " + td);
            Console.WriteLine("table bold elements: " + th);
            double points = 0;
            points += Math.Min(2, head);
            points += Math.Min(1, title / 2);
            points += Math.Min(2, headings);
            points += Math.Min(3, paragraphs / 10);
            points += Math.Min(3, tables);
            points += Math.Min(3, tr / 2);
            points += Math.Min(2, th / 2);
            points += Math.Min(2, td / 2);
            points += Math.Min(10, lists);
            Console.WriteLine("Total Points (max 28): " + points);
        }
        public void evaluateDocument(string path)
        {
            if (evaluatedURLs.Contains(path))
            {
                return;
            }
            else
            {
                evaluatedURLs.Add(path);
            }
            HtmlDocument document = new HtmlDocument();
            try
            {
                document.Load(path);
                Console.WriteLine("Successfully read file" + path);
            }
            catch
            {
                Console.WriteLine("No valid file path: " + path);
                return;
            }
            HtmlNode current = document.DocumentNode;
            while (true)
            {
                if (current.HasChildNodes)
                {
                    current = current.ChildNodes[0];
                }
                else
                {
                    HtmlNode c = current;
                    while (c != null)
                    {
                        if (c.NextSibling != null)
                        {
                            current = c.NextSibling;
                            break;
                        }
                        c = c.ParentNode;
                    }
                    if (c == null)
                    {
                        break;
                    }
                }
                switch (current.Name)
                {
                    case "p":
                        paragraphs++;
                        break;
                    case "h1":
                        headings++;
                        break;
                    case "h2":
                        headings++;
                        break;
                    case "head":
                        head++;
                        break;
                    case "img":
                        images++;
                        break;
                    case "table":
                        tables++;
                        break;
                    case "tr":
                        if (current.ParentNode.Name == "table")
                        {
                            tr++;
                        }
                        break;
                    case "td":
                        if (current.ParentNode.Name == "tr")
                        {
                            td++;
                        }
                        break;
                    case "th":
                        if (current.ParentNode.Name == "tr")
                        {
                            th++;
                        }
                        break;
                    case "title":
                        title++;
                        break;
                    case "a":
                        links++;
                        evaluateDocument(homeDirPath + current.Attributes[0].Value);
                        break;
                    case "li":
                        lists++;
                        break;
                    default:
                        break;
                }
                if (current.Name.Equals("#text"))
                {
                    //Console.WriteLine(current.ParentNode.Name + " : " + current.InnerHtml);
                }
            }
        }
    }
}