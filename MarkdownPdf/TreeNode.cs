using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownPdf
{
    internal class TreeNode
    {
        public int Start { get; set; }
        public int Count { get; set; }

        public string? Value { get; set; }

        public TreeNode? Left { get; set; }
        public TreeNode? Right { get; set; }
    }
}
