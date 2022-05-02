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
    internal class WgLog2FlowDocument
    {
        #region Variables

        FlowDocument document;
        string cache;
        int ptr = 0;

        #endregion

        #region Methods

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="doc">Document to represent</param>
        public WgLog2FlowDocument(FlowDocument doc)
        {
            document = doc;
            document.Blocks.Clear();
        }

        /// <summary>
        /// Method to read the data form a log file
        /// </summary>
        public IEnumerable<Paragraph> Read(string data)
        {
            foreach (string str in data.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                yield return CreateParagraph(str);
        }

        /// <summary>
        /// Creates a formated paragraph to dispaly
        /// </summary>
        /// <param name="str">String to dispaly</param>
        /// <returns></returns>
        public Paragraph CreateParagraph(string str)
        {
            Paragraph p = new Paragraph();

            p.Inlines.Add(new Run(str));

            return p;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            cache = text;

            document.Blocks.Clear();
            document.Blocks.Add(new Paragraph(new Run(text)));

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
        
        private static WgLog2FlowDocument GetObject(DependencyObject obj)
        {
            return (WgLog2FlowDocument)obj.GetValue(InitWgLog2FlowProperty);
        }

        private static void SetObject(DependencyObject obj, Log2FlowDocument value)
        {
            obj.SetValue(InitWgLog2FlowProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty InitWgLog2FlowProperty =
            DependencyProperty.RegisterAttached("InitWgLog2Flow", typeof(WgLog2FlowDocument), typeof(WgLog2FlowDocument), new PropertyMetadata(null));

        #endregion

        #region Content

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetContent(DependencyObject obj)
        {
            WgLog2FlowDocument document = (WgLog2FlowDocument)obj.GetValue(InitWgLog2FlowProperty);

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
            WgLog2FlowDocument document = (WgLog2FlowDocument)obj.GetValue(InitWgLog2FlowProperty);

            if(document != null)
                document.SetText(value);
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.RegisterAttached("Content", typeof(string), typeof(WgLog2FlowDocument), new PropertyMetadata(propertyChangedCallback));

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
                WgLog2FlowDocument document = (WgLog2FlowDocument)textBox.GetValue(InitWgLog2FlowProperty);

                //Initialzie the content
                if (document == null)
                {
                    document = new WgLog2FlowDocument(textBox.Document);
                    textBox.SetValue(InitWgLog2FlowProperty, document);
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
