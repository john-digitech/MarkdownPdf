using MigraDocCore.DocumentObjectModel;
using MigraDocCore.Rendering;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MarkdownPdf
{
    public class MarkdownPdfGenerator
    {        
        static readonly Regex ItalicTextRegEx = new(@"(\*{1}[^*]*\*{1})", RegexOptions.Compiled);
        static readonly Regex BoldTextRegEx = new(@"(\*{2}[^*:]*\*{2})", RegexOptions.Compiled);
        static readonly Regex ItalicOrBoldTextRegEx = new(@"(\*{2}[^*:]*\*{2})|(\*{1}[^*]*\*{1})", RegexOptions.Compiled);
        static readonly Regex CodeTextRegEx = new(@"(`{1}[^`]*`{1})", RegexOptions.Compiled);
        static readonly Regex LinkTextRegEx = new(@"(\[{1}[^]]*\]{1}\({1}[^)]*\){1})", RegexOptions.Compiled);
        static readonly Regex ImageTextRegEx = new(@"(\!\[{1}[^]]*\]{1}\({1}[^)]*\){1})", RegexOptions.Compiled);
        static readonly Regex ListTextRegEx = new(@"^([\*\-\+]{1}\s[^*]*)", RegexOptions.Compiled);
        static readonly Regex NumberedListTextRegEx = new(@"^(\d{1}\.\s[^*]*)", RegexOptions.Compiled);
        static readonly Regex HorizontalRuleTextRegEx = new(@"(\-{3,})", RegexOptions.Compiled);
        static readonly Regex BlockQuoteTextRegEx = new(@"(\>{1}\s[^*]*)", RegexOptions.Compiled);
        static readonly Regex Heading1TextRegEx = new(@"^(#{1})\s.*$", RegexOptions.Compiled);
        static readonly Regex Heading2TextRegEx = new(@"^(#{2})\s.*$", RegexOptions.Compiled);
        static readonly Regex Heading3TextRegEx = new(@"^(#{3})\s.*$", RegexOptions.Compiled);
        static readonly Regex Heading4TextRegEx = new(@"^(#{4})\s.*$", RegexOptions.Compiled);
        static readonly Regex Heading5TextRegEx = new(@"^(#{5})\s.*$", RegexOptions.Compiled);
        static readonly Regex Heading6TextRegEx = new(@"^(#{6})\s.*$", RegexOptions.Compiled);
        static readonly Regex HeadingTextRegEx = new(@"^(#{1,6})\s.*$", RegexOptions.Compiled);
        
        public string FontFamily { get; set; } = "Segoe UI";
        public string CodeFontFamily { get; set; } = "Consolas";

        public char BulletCharacter { get; set; } = '\u2022';
        public char CheckmarkCharacter { get; set; } = '\uE081';

        public MarkdownPdf.Unit Header1Size { get; set; } = Unit.FromPoint(24);
        public MarkdownPdf.Unit Header2Size { get; set; } = Unit.FromPoint(20);
        public MarkdownPdf.Unit Header3Size { get; set; } = Unit.FromPoint(16);
        public MarkdownPdf.Unit Header4Size { get; set; } = Unit.FromPoint(14);
        public MarkdownPdf.Unit Header5Size { get; set; } = Unit.FromPoint(12);
        public MarkdownPdf.Unit Header6Size { get; set; } = Unit.FromPoint(10);
        
        public MarkdownPdf.Unit RegularTextSize { get; set; } = Unit.FromPoint(10);

        public bool UsePageNumbers { get; set; } = false;

        public DateTime? DocumentDate { get; set; } = null;

        public MarkdownPdf.Unit LeftMargin { get; set; } = Unit.FromPoint(60.0);
        public MarkdownPdf.Unit RightMargin { get; set; } = Unit.FromPoint(60.0);
        public MarkdownPdf.Unit TopMargin { get; set; } = Unit.FromPoint(60.0);
        public MarkdownPdf.Unit BottomMargin { get; set; } = Unit.FromPoint(60.0);

        public string? Title { get; set; }

        private Document? _document;
        Font? _header1Font;
        Font? _header2Font;
        Font? _header3Font;
        Font? _header4Font;
        Font? _header5Font;
        Font? _header6Font;

        Font? _regulatTextFont;
        Font? _boldTextFont;
        Font? _italicTextFont;
        Font? _codeTextFont;
        Font? _glyphFont;

        public MarkdownPdfGenerator()
        {

        }

        public MarkdownPdfGenerator(MarkdownPdf.Unit leftMargin,
                                    MarkdownPdf.Unit topMargin,
                                    MarkdownPdf.Unit rightMargin,
                                    MarkdownPdf.Unit bottomMargin,
                                    string? title = null,
                                    DateTime? docDate = null)
        {
            LeftMargin = leftMargin;
            RightMargin = rightMargin;
            BottomMargin = bottomMargin;
            TopMargin = topMargin;            
            Title = title;            
            DocumentDate = docDate;
        }

        private void BuildFonts()
        {
            _header1Font = new(FontFamily, Header1Size) { Color = Colors.Black, Bold = true };
            _header2Font = new(FontFamily, Header2Size) { Color = Colors.Black, Bold = true };
            _header3Font = new(FontFamily, Header3Size) { Color = Colors.Black, Bold = true };
            _header4Font = new(FontFamily, Header4Size) { Color = Colors.Black, Bold = true };
            _header5Font = new(FontFamily, Header5Size) { Color = Colors.Black, Bold = true };
            _header6Font = new(FontFamily, Header6Size) { Color = Colors.Black, Bold = true };

            _regulatTextFont = new(FontFamily, RegularTextSize) { Color = Colors.Black };

            _boldTextFont = new(FontFamily, RegularTextSize) { Color = Colors.Black, Bold = true };

            _italicTextFont = new(FontFamily, RegularTextSize) { Color = Colors.Black, Italic = true };

            _glyphFont = new Font("Segoe MDL2 Assets", RegularTextSize) { Color = Colors.Black };

            _codeTextFont = new(CodeFontFamily, RegularTextSize) { Color = Colors.Black };
        }

        public void GeneratePdf(string text)
        {
            BuildFonts();
            
            _document = new();            
            
            Section section = _document.AddSection();
            section.PageSetup.LeftMargin = LeftMargin;
            section.PageSetup.RightMargin = RightMargin;
            section.PageSetup.TopMargin = TopMargin;
            section.PageSetup.BottomMargin = BottomMargin;
            
            Unit currentIndent = new Unit(0, UnitType.Point);

            if (DocumentDate.HasValue)
            {
                var header = section.Headers.Primary.AddParagraph();
                header.AddText($"Date: {DocumentDate.Value.ToShortDateString()}");
                header.Format.Alignment = ParagraphAlignment.Right;
                header.Format.Font = _regulatTextFont?.Clone();
            }

            if (UsePageNumbers)
            {
                var footer = section.Footers.Primary.AddParagraph();
                footer.Format.Alignment = ParagraphAlignment.Center;
                footer.Format.Font = _regulatTextFont?.Clone();
                footer.AddPageField();
            }

            if (!String.IsNullOrEmpty(Title))
            {
                var docTitle = section.AddParagraph();
                docTitle.AddFormattedText(Title);
                docTitle.Format.Font = _header1Font?.Clone();                   
                docTitle.Format.Alignment = ParagraphAlignment.Center;
                docTitle.Format.SpaceBefore = 0;
                docTitle.Format.SpaceAfter = new Unit(16, UnitType.Point);
            }            

            Stack<int> numberedListCounts = new Stack<int>();
            int numberedListCount = 1;
            numberedListCounts.Push(numberedListCount);           

            string[] lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (string currentLine in lines)
            {
                if (String.IsNullOrEmpty(currentLine) == false)
                {
                    string line = currentLine;

                    TreeNode root = new();
                    BuildSyntaxTreeFromString(root, line);

                    if (line.StartsWith("<"))
                    {
                        currentIndent.Value += 20;

                        numberedListCounts.Push(1);

                        continue;

                    }
                    else if (line.StartsWith(">"))
                    {
                        currentIndent.Value -= 20;

                        numberedListCounts.Pop();
                        continue;
                    }



                    Paragraph paragraph = section.AddParagraph();


                    if (IsHeader(line))
                    {
                        SetHeaderFormat(ref paragraph);
                    }                   
                    else
                    {
                        SetTextFormat(ref paragraph);
                    }

                    paragraph.Format.LeftIndent = currentIndent;

                    numberedListCount = numberedListCounts.Pop();

                    Output(root, paragraph, ref numberedListCount);

                    numberedListCounts.Push(numberedListCount);
                }
            }

            #region IsHeader

            // Local Function
            bool IsHeader(string text)
            {
                return HeadingTextRegEx.IsMatch(text);        
            }

            #endregion

            #region SetHeaderFormat

            void SetHeaderFormat(ref Paragraph paragraph)
            {
                paragraph.Format.SpaceBefore = Unit.FromPoint(8);
                paragraph.Format.SpaceAfter = Unit.FromPoint(8);
            }

            #endregion

            #region SetTextFormat

            void SetTextFormat(ref Paragraph paragraph)
            {
                paragraph.Format.SpaceBefore = Unit.FromPoint(4);
                paragraph.Format.SpaceAfter = Unit.FromPoint(6);
            }

            #endregion
        }


        static void BuildSyntaxTreeFromString(TreeNode node, string line)
        {
            line = line.Trim('\n');

            if (ItalicOrBoldTextRegEx.IsMatch(line))
            {
                Match? matchedBoldOrItalics = ItalicOrBoldTextRegEx.Match(line);

                if (matchedBoldOrItalics != null)
                {
                    Group? boldTextMatch = matchedBoldOrItalics.Groups[1];
                    Group? italicTextMatch = matchedBoldOrItalics.Groups[2];

                    Group? matchedText = null;

                    if (boldTextMatch.Success)
                        matchedText = boldTextMatch;

                    else if (italicTextMatch.Success)
                        matchedText = italicTextMatch;

                    // If there is Bold/Italic text
                    // Store it in the node
                    if (matchedText != null)
                    {
                        node.Start = matchedText.Index;
                        node.Count = matchedText.Length;
                        node.Value = line.Substring(node.Start, node.Count);
                    }

                    // Grab the text preceeding (to the left) the Bold/Italic text
                    // If there is no Bold/Italic text, grab the whole line
                    // Then add a TreeNode for the left text and build the branch
                    // for that node
                    string left = line.Substring(0, matchedText?.Index ?? line.Length);

                    TreeNode leftNode = new();
                    node.Left = leftNode;

                    BuildSyntaxTreeFromString(leftNode, left);

                    // Grab the text following (to the right) the Bold/Italic text
                    // Then add a TreeNode for the right text and build the branch
                    // for that node
                    string right = line.Substring(node.Start + node.Count);

                    TreeNode rightNode = new();
                    node.Right = rightNode;

                    BuildSyntaxTreeFromString(rightNode, right);
                }
            }
            else
            {
                node.Value = line;
            }
        }

        public void Save(string filename)
        {
            PdfDocumentRenderer pdfDocumentRenderer = new PdfDocumentRenderer(false);
            pdfDocumentRenderer.Document = _document;
            pdfDocumentRenderer.RenderDocument();
            pdfDocumentRenderer.Save(filename);
        }

        public void Save(Stream saveStream, bool closeStream = true)
        {
            try
            {
                PdfDocumentRenderer pdfDocumentRenderer = new PdfDocumentRenderer(true);
                pdfDocumentRenderer.Document = _document;                
                pdfDocumentRenderer.RenderDocument();
                pdfDocumentRenderer.Save(saveStream, closeStream);
            }
            catch { }
        }

        private void Output(TreeNode node, Paragraph paragraph, ref int listCount)
        {



            if (node.Left != null)
                Output(node.Left, paragraph, ref listCount);

            string? line = node.Value;

            if (!String.IsNullOrEmpty(line))
            {
                if (Heading1TextRegEx.IsMatch(line))
                {
                    paragraph.AddFormattedText(line?.Trim('#'), _header1Font);

                    listCount = 1;
                }
                else if (Heading2TextRegEx.IsMatch(line))
                {
                    paragraph.AddFormattedText(line?.Trim('#'), _header2Font);

                    listCount = 1;
                }
                else if (Heading3TextRegEx.IsMatch(line))
                {
                    paragraph.AddFormattedText(line?.Trim('#'), _header3Font);

                    listCount = 1;
                }
                else if (Heading4TextRegEx.IsMatch(line))
                {
                    paragraph.AddFormattedText(line?.Trim('#'), _header4Font);

                    listCount = 1;
                }
                else if (Heading5TextRegEx.IsMatch(line))
                {
                    paragraph.AddFormattedText(line?.Trim('#'), _header5Font);

                    listCount = 1;
                }
                else if (Heading6TextRegEx.IsMatch(line))
                {
                    paragraph.AddFormattedText(line?.Trim('#'), _header6Font);

                    listCount = 1;
                }
                else if (BoldTextRegEx.IsMatch(line))
                {
                    paragraph.AddFormattedText(line?.Trim('*'), _boldTextFont);
                }
                else if (ItalicTextRegEx.IsMatch(line))
                {
                    paragraph.AddFormattedText(line?.Trim('*'), _italicTextFont);
                }
                else if (NumberedListTextRegEx.IsMatch(line))
                {
                    string? text = line?.Substring(3);

                    paragraph.AddFormattedText($"{listCount++}. {text}", _regulatTextFont);
                }
                else if (ListTextRegEx.IsMatch(line))
                {
                    paragraph.AddFormattedText($"{BulletCharacter} ");
                    line = line?.Substring(2);
                    paragraph.AddFormattedText(line, _regulatTextFont);
                }                
                else if (line?.StartsWith("-- ") == true)
                {
                    paragraph.AddFormattedText($"{CheckmarkCharacter} ", _glyphFont);
                    line = line?.Substring(3);
                    paragraph.AddFormattedText(line, _regulatTextFont);
                }
                else
                {
                    paragraph.AddFormattedText(line, _regulatTextFont);
                }

            }

            if (node.Right != null)
                Output(node.Right, paragraph, ref listCount);



        }

    }



}
