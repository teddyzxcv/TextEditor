using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Options;
using System.Xml;

namespace TextEditor
{
    class FormattingCode
    {
        static int TabBeforeLine = 0;
        //Do formatting
        public static string GetFormatCode(string code)
        {
            // Create syntax tree, convert to classification.
            var host = MefHostServices.Create(MefHostServices.DefaultAssemblies);
            var workspace = new AdhocWorkspace(host);
            OptionSet options = workspace.Options;
            options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInMethods, true);
            options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInTypes, true);
            var sourceText = SourceText.From(code);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);
            SyntaxNode root = tree.GetRoot();
            root = Formatter.Format(root, workspace, options);
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                root.WriteTo(writer);
            }
            code = sb.ToString();
            code = code.Replace("\n\n", "\n");
            return code;
        }
    }
}
