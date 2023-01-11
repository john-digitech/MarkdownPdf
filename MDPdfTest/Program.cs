using MarkdownPdf;

namespace MDPdfTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            string markdown = @"# Hello, World!

- This is a list
- This is another item in the list

## This is a subheading

This is a paragraph.

This is another paragraph.

1. This is a numbered list
1. This is another item in the list
1. This is a third item in the list";


            MarkdownPdfGenerator generator = new MarkdownPdfGenerator(Unit.FromInch(0.5), Unit.FromInch(0.5), Unit.FromInch(0.5), Unit.FromInch(0.5), "The Title", DateTime.Now);           
            generator.GeneratePdf(markdown);
            generator.Save("test.pdf");

        }
    }
}