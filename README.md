# MarkdownPdf

## Renders Markdown text to a .pdf file.

    using MarkdownPdf;

    // Create a markdown document in a string
    string markdown = 
    @"# This is markdown text
    ## This is a subheading
    * This is a list item
    * This is another list item
    
    This is a paragraph of text.
    
    This is another paragraph of text.
    ";

    
    // Create a new MarkdownPdf object
    MarkdownPdfGenerator generator = new();
    
    generator.LeftMargin = Unit.FromInch(0.5);
    generator.RightMargin = Unit.FromInch(0.5);
    generator.TopMargin = Unit.FromInch(0.5);
    generator.BottomMargin = Unit.FromInch(0.5);       
    
    generator.GeneratePdf(markdown);
    generator.Save("temp.pdf");

## Properties

### Fonts

**FontFamily** : *string* - Font family of the text.

**CodeFontFamily** : *string* - Font family for code text (*not yet implemented).

### Text Sizes

**RegularTextSize** : *Unit* - Size of regular text.

**Heading1Size** : *Unit* - Text size of *Heading 1*.

**Heading2Size** : *Unit* - Text size of *Heading 2*.

**Heading3Size** : *Unit* - Text size of *Heading 3*.

**Heading4Size** : *Unit* - Text size of *Heading 4*.

**Heading5Size** : *Unit* - Text size of *Heading 5*.

**Heading6Size** : *Unit* - Text size of *Heading 6*.

### Margins

**LeftMargin** : *Unit* - Size of left margin.

**RightMargin** : *Unit* - Size of right margin.

**TopMargin** : *Unit* - Size of top margin.

**BottomMargin** : *Unit* - Size of bottom margin.

### Glyphs

**BulletCharacter** : *Char* - Character used for the bullet in unordered lists.

**CheckmarkCharacter** : *Char* - Character used for checkmarks.

### Document Attributes

**Title** : *string* - Title of the document. Appears centered at top of first page. (*optional).

**DocumentDate** : *DateTime* - Date displayed in upper right of page (*optional).

**UsePageNumbers** : *Boolean* - Places page number on bottom of each page if set to *TRUE*.


## Units

The following units are supported:

* Centemeter
* Milimetter
* Inch
* Point
* Pica

