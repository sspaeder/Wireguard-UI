using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WireGuard.GUI.AttachedProperties
{
    /// <summary>
    /// 
    /// </summary>
    internal class Log2FlowDocument
    {
        #region Variables

        FlowDocument document;
        string cache;
        int ptr = 0;

        #endregion

        #region Regex

        /// <summary>
        /// Regex for detecting the tpye of an entry
        /// </summary>
        const string TYPE = @"\[(?<Type>\w{3})\]";

        /// <summary>
        /// Regex for detecting the DateTime of the entry
        /// </summary>
        const string DATETIME = @"\[(?<Date>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3})\]";

        /// <summary>
        /// Regex for detecting the Text of an entry
        /// </summary>
        const string TEXT = @"(?<Text>(?s).*?)(?=\[\w{3}\]|\z)";

        /// <summary>
        /// Regex for the detection of entrys
        /// </summary>
        static readonly string REGEX = $"{TYPE}{DATETIME}{TEXT}";

        #endregion

        #region Methods

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="doc">Document to represent</param>
        public Log2FlowDocument(FlowDocument doc)
        {
            document = doc;
            document.Blocks.Clear();
        }

        /// <summary>
        /// Method to read the data form a log file
        /// </summary>
        public IEnumerable<Paragraph> Read(string data)
        {
            MatchCollection mc = Regex.Matches(data, REGEX);

            foreach (Match m in mc)
                yield return CreateParagraph(m);
        }

        /// <summary>
        /// Creates a formated paragraph to dispaly
        /// </summary>
        /// <param name="m">Match that contains the data</param>
        /// <returns></returns>
        public Paragraph CreateParagraph(Match m)
        {
            Paragraph p = new Paragraph();

            Brush brush = new SolidColorBrush(Colors.Black);

            switch (m.Groups["Type"].Value)
            {
                case "DBG": brush = new SolidColorBrush(Colors.Cyan); break;
                case "INF": brush = new SolidColorBrush(Colors.Blue); break;
                case "WAR": brush = new SolidColorBrush(Colors.Yellow); break;
                case "ERR": brush = new SolidColorBrush(Colors.Red); break;
            }

            p.Inlines.Add(new Run(m.Groups["Type"].Value.PadRight(4))
            {
                Foreground = brush,
                FontWeight = FontWeights.Bold
            });

            p.Inlines.Add(new Run(m.Groups["Date"].Value + "\n")
            {
                Foreground = brush,
                FontWeight = FontWeights.Bold
            });

            p.Inlines.Add(new Run(m.Groups["Text"].Value.Trim())
            {
            });

            return p;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            cache = text;
            ReadOnlySpan<char> chars = new ReadOnlySpan<char>(cache.ToArray());

            foreach(Paragraph p in Read(chars.Slice(ptr, cache.Length - ptr).ToString()))
                document.Blocks.Add(p);

            ptr = cache.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetText() => cache;

        #endregion

        #region Attachable Property

        #region Initilaize
        
        private static Log2FlowDocument GetObject(DependencyObject obj)
        {
            return (Log2FlowDocument)obj.GetValue(InitLog2FlowProperty);
        }

        private static void SetObject(DependencyObject obj, Log2FlowDocument value)
        {
            obj.SetValue(InitLog2FlowProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty InitLog2FlowProperty =
            DependencyProperty.RegisterAttached("InitLog2Flow", typeof(Log2FlowDocument), typeof(Log2FlowDocument), new PropertyMetadata(null));

        #endregion

        #region Content

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetContent(DependencyObject obj)
        {
            Log2FlowDocument document = (Log2FlowDocument)obj.GetValue(InitLog2FlowProperty);

            if(document == null)
                return null;

            return document.GetText();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetContent(DependencyObject obj, string value)
        {
            Log2FlowDocument document = (Log2FlowDocument)obj.GetValue(InitLog2FlowProperty);

            if(document != null)
                document.SetText(value);
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.RegisterAttached("Content", typeof(string), typeof(Log2FlowDocument), new PropertyMetadata(propertyChangedCallback));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void propertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is RichTextBox)
            {
                RichTextBox textBox = (RichTextBox)d;
                Log2FlowDocument document = (Log2FlowDocument)textBox.GetValue(InitLog2FlowProperty);

                //Initialzie the content
                if (document == null)
                {
                    document = new Log2FlowDocument(textBox.Document);
                    textBox.SetValue(InitLog2FlowProperty, document);
                }

                //Update the document
                document.SetText((string)e.NewValue);
                textBox.ScrollToEnd();
            }
        }

        #endregion

        #endregion
    }
}
