using System;
using System.Net;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ExCSS;
using System.IO;
namespace webGrader2
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
            Console.WriteLine("Enter file path to evaluate (for example \"/home/olep/Dropbox/webdesign/\"");
            //string path = Console.ReadLine();
            string path="/home/olep/Dropbox/webdesign/ExampleKurs/";
            string[] folders = Directory.GetDirectories(path);
            string[] gradings = new string[folders.Length];
            int i = 0;
            foreach (string folder in folders)
            {
                WebpageEvaluation e;
                if (File.Exists(folder + "home.html"))
                {
                    e = new WebpageEvaluation("/home.html", folder);
                }
                else
                {
                    string[] htmlFiles = Directory.GetFiles(folder, "*.html");
                    string[] split = htmlFiles[0].Split("/");
                    e = new WebpageEvaluation(split[split.Length - 1], folder + "/");
                }
                System.IO.File.WriteAllText(folder + "/Grading.txt", e.evaluationText);
                gradings[i] = folder + ":" + e.points;
                i++;
            }
            System.IO.File.WriteAllLines(path + "Gradings.txt", gradings);
            //WebpageEvaluation evaluation = new WebpageEvaluation("Home.html", "Website_final/");
        }
    }
    class WebpageEvaluation
    {
        int paragraphs, headings, lists, tables, links, images, head, title, tr, td, th, validCSS, cssSelectors, invalidCSS, cssErrors, htmlErrors;
        public string evaluationText;
        double maxPoints;
        public double points = 0;
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
            evaluateResult("paragraphs", 3, paragraphs, 10);
            evaluateResult("headings", 2, headings, 2);
            evaluateResult("heads", 2, head, 2);
            evaluateResult("images", 5, images, 2);
            evaluateResult("lists", 10, lists, 3);
            evaluateResult("tables", 3, tables, 1);
            evaluateResult("Table rows", 3, tr, 5);
            evaluateResult("Highlited table elements", 2, th, 2);
            evaluateResult("Table cells", 2, td, 10);
            evaluateResult("titles", 2, title, 2);
            evaluateResult("valid CSS selectors", 5, cssSelectors, 10);
            evaluateResult("valid CSS rules", 5, validCSS, 10);
            evaluateResult("links", 5, links, 2);
            evaluateResult("html validation", 5, Math.Max(5 - htmlErrors, 0), 5);
            Console.WriteLine("Invalid css selectors: " + invalidCSS);
            Console.WriteLine("Total Points (max" + maxPoints + "): " + points);
            evaluationText += "Invalid css rules: " + invalidCSS + Environment.NewLine + "Total Points (max" + maxPoints + "): " + points;
        }
        public void evaluateResult(string cathegory, double maxPoints, int occurences, int wishedOccurrences)
        {
            double points = Math.Min(maxPoints * (double)occurences / (double)wishedOccurrences, maxPoints);
            Console.WriteLine(cathegory + ":" + occurences + "/" + wishedOccurrences + "=>" + points + "points of " + maxPoints + "points");
            evaluationText += cathegory + ":" + occurences + "/" + wishedOccurrences + "=>" + points + "points of " + maxPoints + "points" + Environment.NewLine;
            this.points += points;
            this.maxPoints += maxPoints;
        }
        public List<string> getXPathFromStylesheet(string path)
        {
            bool isInRule = false;
            List<string> selectors = new List<string>();
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(path))
                {
                    // Read the stream to a string, and write the string to the console.
                    while (!sr.EndOfStream)
                    {
                        String line = sr.ReadLine();
                        string[] split = line.Split('{');
                        if (!isInRule && split[0].Length > 0)
                        {
                            selectors.Add(MostThingsWeb.css2xpath.Transform(split[0]));
                        }
                        if (line.Contains("{"))
                        {
                            isInRule = true;
                        }
                        if (line.Contains("}"))
                        {
                            isInRule = false;
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Could not read Stylesheet");
                Console.WriteLine(e.Message);
            }
            return selectors;
        }
        public static bool equals(string css1, string css2){
            List<string> words1=getWords(css1);
            List<string> words2=getWords(css2);
            if(words1.Count!=words2.Count){
                return false;
            }
            for(int i=0;i<words1.Count;i++){
                if(words1[i]!=words2[i]){
                    return false;
                }
            }
            return true;
        }
        private static bool isWord(string w){
            int letters=0;
            if(w=="rgb"){
                return false;
            }
            foreach(char c in w.ToCharArray()){
                if((c>='A' && c<='Z') || (c>='a' && c<='z')){
                    letters++;
                }else if(c<='0'||c>='9'){
                    return false;
                }
            }
            return letters>0;
        }
        private static List<string> getWords(string s){
            List<string> toReturn= new List<string>(s.Split(new char[]{' ','{','}',':','(',')','-','+',',','\t',Environment.NewLine.ToCharArray()[0]}));
            toReturn.RemoveAll(word=>!isWord(word));
            return toReturn;
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
            IEnumerable<HtmlParseError> errors = document.ParseErrors;
            foreach (var i in errors)
            {
                htmlErrors++;
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
                    case "link":
                        //MostThingsWeb.css2xpath.Transform()
                        List<string> XPaths = getXPathFromStylesheet(homeDirPath + current.Attributes[0].Value);
                        foreach (string rule in XPaths)
                        {
                            try
                            {
                                HtmlAgilityPack.HtmlNodeCollection selectedElements = document.DocumentNode.SelectNodes(rule);
                                if (selectedElements != null && selectedElements.Count > 0)
                                {
                                    cssSelectors++;
                                }
                            }
                            catch
                            {
                                invalidCSS++;
                                Console.WriteLine("Fehlerhaftes CSS: " + rule);
                            }
                        }
                        StylesheetParser parser = new StylesheetParser();
                        try
                        {

                            Stylesheet stylesheet;
                            foreach (var at in current.Attributes)
                            {
                                if (at.Name == "href")
                                {
                                    string rawCSS=File.ReadAllText(homeDirPath + at.Value);
                                    stylesheet = parser.Parse(rawCSS);
                                    string validCSS=stylesheet.ToCss();
                                    
                                    foreach(IStylesheetNode el in  stylesheet.Children){
                                        
                                        if(!equals(el.StylesheetText.Text,el.ToCss())){
                                            cssErrors++;
                                        }
                                    }

                                    foreach (var rule in stylesheet.Children)
                                    {
                                        this.validCSS++;
                                    }
                                    break;
                                }
                            }

                        }
                        catch { }
                        break;
                    case "p":
                        paragraphs++;
                        break;
                    case "h1":
                        headings++;
                        break;
                    case "h2":
                        headings++;
                        break;
                    case "h3":
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
                        string currentFolder="";
                        string[] f=path.Split("/");
                        for(int i=0;i<f.Length-1;i++){
                            currentFolder+=f[i]+"/";
                        }
                        foreach (var at in current.Attributes)
                        {
                            if (at.Name == "href")
                            {
                                evaluateDocument(currentFolder + at.Value);
                                break;
                            }
                        }
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







namespace MostThingsWeb
{

    /// <summary>
    /// A static utility class for transforming CSS selectors to XPath selectors.
    /// </summary>
    public static class css2xpath
    {

        private static List<Regex> patterns;
        private static List<Object> replacements;

        static css2xpath()
        {
            // Initalize list of patterns and replacements
            patterns = new List<Regex>();
            replacements = new List<Object>();

            // Generate all the rules

            // Attributes
            AddRule(new Regex(@"\[([^\]~\$\*\^\|\!]+)(=[^\]]+)?\]"), "[@$1$2]");

            // Multiple queries
            AddRule(new Regex(@"\s*,\s*"), "|");

            // Remove space around +, ~, and >
            AddRule(new Regex(@"\s*(\+|~|>)\s*"), "$1");

            //Handle *, ~, +, and >
            AddRule(new Regex(@"([a-zA-Z0-9_\-\*])~([a-zA-Z0-9_\-\*])"), "$1/following-sibling::$2");
            AddRule(new Regex(@"([a-zA-Z0-9_\-\*])\+([a-zA-Z0-9_\-\*])"), "$1/following-sibling::*[1]/self::$2");
            AddRule(new Regex(@"([a-zA-Z0-9_\-\*])>([a-zA-Z0-9_\-\*])"), "$1/$2");

            // Escaping
            AddRule(new Regex(@"\[([^=]+)=([^'|" + "\"" + @"][^\]]*)\]"), "[$1='$2']");

            // All descendant or self to //
            AddRule(new Regex(@"(^|[^a-zA-Z0-9_\-\*])(#|\.)([a-zA-Z0-9_\-]+)"), "$1*$2$3");
            AddRule(new Regex(@"([\>\+\|\~\,\s])([a-zA-Z\*]+)"), "$1//$2");
            AddRule(new Regex(@"\s+\/\/"), "//");

            // Handle :first-child
            AddRule(new Regex(@"([a-zA-Z0-9_\-\*]+):first-child"), "*[1]/self::$1");

            // Handle :last-child
            AddRule(new Regex(@"([a-zA-Z0-9_\-\*]+):last-child"), "$1[not(following-sibling::*)]");

            // Handle :only-child
            AddRule(new Regex(@"([a-zA-Z0-9_\-\*]+):only-child"), "*[last()=1]/self::$1");

            // Handle :empty
            AddRule(new Regex(@"([a-zA-Z0-9_\-\*]+):empty"), "$1[not(*) and not(normalize-space())]");

            // Handle :not
            AddRule(new Regex(@"([a-zA-Z0-9_\-\*]+):not\(([^\)]*)\)"), new MatchEvaluator((Match m) =>
            {
                return m.Groups[1].Value + "[not(" + (new Regex("^[^\\[]+\\[([^\\]]*)\\].*$")).Replace(Transform(m.Groups[2].Value), "$1") + ")]";
            }));

            // Handle :nth-child
            AddRule(new Regex(@"([a-zA-Z0-9_\-\*]+):nth-child\(([^\)]*)\)"), new MatchEvaluator((Match m) =>
            {
                String b = m.Groups[2].Value;
                String a = m.Groups[1].Value;

                switch (b)
                {
                    case "n":
                        return a;
                    case "even":
                        return "*[position() mod 2=0 and position()>=0]/self::" + a;
                    case "odd":
                        return a + "[(count(preceding-sibling::*) + 1) mod 2=1]";
                    default:
                        // Parse out the 'n'
                        b = ((new Regex("^([0-9])*n.*?([0-9])*$")).Replace(b, "$1+$2"));

                        // Explode on + (i.e 'nth-child(2n+0)' )
                        String[] b2 = new String[2];
                        String[] splitResult = b.Split('+');

                        // The first component will always be a number
                        b2[0] = splitResult[0];

                        int buffer = 0;

                        // The second component might be missing
                        if (splitResult.Length == 2)
                            if (!int.TryParse(splitResult[1], out buffer))
                                buffer = 0;

                        b2[1] = buffer.ToString();

                        return "*[(position()-" + b2[1] + ") mod " + b2[0] + "=0 and position()>=" + b2[1] + "]/self::" + a;
                }
            }));

            // Handle :contains
            AddRule(new Regex(@":contains\(([^\)]*)\)"), new MatchEvaluator((Match m) =>
            {
                return "[contains(string(.),'" + m.Groups[1].Value + "')]";
            }));

            // != attribute
            AddRule(new Regex(@"\[([a-zA-Z0-9_\-]+)\|=([^\]]+)\]"), "[@$1=$2 or starts-with(@$1,concat($2,'-'))]");

            // *= attribute
            AddRule(new Regex(@"\[([a-zA-Z0-9_\-]+)\*=([^\]]+)\]"), "[contains(@$1,$2)]");

            // ~= attribute
            AddRule(new Regex(@"\[([a-zA-Z0-9_\-]+)~=([^\]]+)\]"), "[contains(concat(' ',normalize-space(@$1),' '),concat(' ',$2,' '))]");

            // ^= attribute
            AddRule(new Regex(@"\[([a-zA-Z0-9_\-]+)\^=([^\]]+)\]"), "[starts-with(@$1,$2)]");

            // $= attribute
            AddRule(new Regex(@"\[([a-zA-Z0-9_\-]+)\$=([^\]]+)\]"), new MatchEvaluator((Match m) =>
            {
                String a = m.Groups[1].Value;
                String b = m.Groups[2].Value;
                return "[substring(@" + a + ",string-length(@" + a + ")-" + (b.Length - 3) + ")=" + b + "]";
            }));

            // != attribute
            AddRule(new Regex(@"\[([a-zA-Z0-9_\-]+)\!=([^\]]+)\]"), "[not(@$1) or @$1!=$2]");

            // ID and class
            AddRule(new Regex(@"#([a-zA-Z0-9_\-]+)"), "[@id='$1']");
            AddRule(new Regex(@"\.([a-zA-Z0-9_\-]+)"), "[contains(concat(' ',normalize-space(@class),' '),' $1 ')]");

            // Normalize filters
            AddRule(new Regex(@"\]\[([^\]]+)"), " and ($1)");
        }

        /// <summary>
        /// Adds a rule for transforming CSS to XPath.
        /// </summary>
        /// <param name="regex">A Regex for the parts of the CSS you want to transform.</param>
        /// <param name="replacement">A MatchEvaluator for converting the matched CSS parts to XPath.</param>
        /// <exception cref="ArgumentException">Thrown if regex or replacement is null.</exception>
        /// <example>
        /// <code>
        /// // Handle :contains selectors
        /// AddRule(new Regex(@":contains\(([^\)]*)\)"), new MatchEvaluator((Match m) => {
        ///     return "[contains(string(.),'" + m.Groups[1].Value + "')]";
        /// }));
        /// 
        /// // Note: Remember that m.Groups[1] refers to the first captured group; m.Groups[0] refers
        /// // to the entire match.
        /// </code>
        /// </example>
        public static void AddRule(Regex regex, MatchEvaluator replacement)
        {
            _AddRule(regex, replacement);
        }

        /// <summary>
        /// Adds a rule for transforming CSS to XPath.
        /// </summary>
        /// <param name="regex">A Regex for the parts of the CSS you want to transform.</param>
        /// <param name="replacement">A String for converting the matched CSS parts to XPath.</param>
        /// <exception cref="ArgumentException">Thrown if regex or replacement is null.</exception>
        /// <example>
        /// <code>
        /// // Replace commas (denotes multiple queries) with pipes (|)
        /// AddRule(new Regex(@"\s*,\s*"), "|");
        /// </code>
        /// </example>
        public static void AddRule(Regex regex, String replacement)
        {
            _AddRule(regex, replacement);
        }

        /// <summary>
        /// Adds a rule for transforming CSS to XPath. For internal use only.
        /// </summary>
        /// <param name="regex">A Regex for the parts of the CSS you want to transform.</param>
        /// <param name="replacement">A String or MatchEvaluator for converting the matched CSS parts to XPath.</param>
        /// <exception cref="ArgumentException">Thrown if regex or replacement is null, or if the replacement is neither a String nor a MatchEvaluator.</exception>
        private static void _AddRule(Regex regex, Object replacement)
        {
            if (regex == null)
                throw new ArgumentException("Must supply non-null Regex.", "regex");

            if (replacement == null || (!(replacement is String) && !(replacement is MatchEvaluator)))
                throw new ArgumentException("Must supply non-null replacement (either String or MatchEvaluator).", "replacement");

            patterns.Add(regex);
            replacements.Add(replacement);
        }

        /// <summary>
        /// Transforms the given CSS selector to an XPath selector.
        /// </summary>
        /// <param name="css">The CSS selector to transform into an XPath selector.</param>
        /// <returns>The resultant XPath selector.</returns>
        public static String Transform(String css)
        {
            int len = patterns.Count;

            for (int i = 0; i < len; i++)
            {
                Regex pattern = patterns[i];
                Object replacement = replacements[i];

                // Depending on what the replacement is, we need to cast it to either a String or a MatchEvaluator
                if (replacement is String)
                    css = pattern.Replace(css, (String)replacement);
                else
                    css = pattern.Replace(css, (MatchEvaluator)replacement);
            }

            return "//" + css;
        }

        /// <summary>
        /// Forces the CSS to XPath rules to be created. Not neccesary; the rules are created the first time Transform is called.
        /// </summary>
        /// <remarks>
        /// Perhaps you would want to use this in the initalization procedure of your application.
        /// </remarks>
        public static void PreloadRules()
        {
            /* Empty by design:
             * 
             * The static class initializer will be called the first time any static method, such
             * as this one, is called
             */
        }
    }
}