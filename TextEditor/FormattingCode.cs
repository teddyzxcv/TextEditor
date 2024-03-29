﻿
using System.Text;

using System.IO;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Options;

using FastColoredTextBoxNS;

namespace TextEditor
{
    class FormattingCode
    {
        // All style parameter.
        static int TabBeforeLine = 0;
        public static SyntaxHighlighter CSharpSyntaxHighlighter { get; set; }
        public static Style CSharpKeywordStyle { get; set; }
        public static Style CSharpCommentStyle { get; set; }
        public static Style CSharpAttributeStyle { get; set; }
        public static Style CSharpClassNameStyle { get; set; }
        public static Style CSharpCommentTagStyle { get; set; }
        public static Style CSharpNumberStyle { get; set; }
        public static Style CSharpStringStyle { get; set; }
        public static Style CSharpVariableStyle { get; set; }

        static FormattingCode()
        {
            FastColoredTextBox fb = new FastColoredTextBox();
            fb.Language = Language.CSharp;
            CSharpSyntaxHighlighter = fb.SyntaxHighlighter;
            CSharpSyntaxHighlighter = GetSyntaxColor(CSharpSyntaxHighlighter);
        }

        /// <summary>
        /// Do formatting.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Set the syntax color.
        /// Writen by reading this link: 
        /// https://www.codeproject.com/Articles/161871/Fast-Colored-TextBox-for-syntax-highlighting-2
        /// </summary>
        public static void SetSyntaxColor()
        {
            CSharpSyntaxHighlighter = GetSyntaxColor(CSharpSyntaxHighlighter);
        }
        /// <summary>
        /// Get all syntax setting.
        /// </summary>
        /// <param name="shl"></param>
        /// <returns></returns>

        public static SyntaxHighlighter GetSyntaxColor(SyntaxHighlighter shl)
        {
            shl.CommentStyle = CSharpCommentStyle;
            shl.KeywordStyle = CSharpKeywordStyle;
            shl.AttributeStyle = CSharpAttributeStyle;
            shl.ClassNameStyle = CSharpClassNameStyle;
            shl.CommentTagStyle = CSharpCommentTagStyle;
            shl.NumberStyle = CSharpNumberStyle;
            shl.StringStyle = CSharpStringStyle;
            shl.VariableStyle = CSharpVariableStyle;
            return shl;
        }

    }
}
