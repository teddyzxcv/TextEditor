using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Options;


namespace TextEditor
{
    public class CSharpCompiler
    {
        /// Writen use these link:
        /// https://laurentkempe.com/2019/02/18/dynamically-compile-and-run-code-using-dotNET-Core-3.0/
        /// https://stackoverflow.com/questions/826398/is-it-possible-to-dynamically-compile-and-execute-c-sharp-code-fragments
        /// http://www.blackwasp.co.uk/RuntimeCompilation.aspx
        /// https://www.youtube.com/watch?v=Kyd-5UzzU2A

        public static List<string> ComplieCode(string code)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            var assemblyName = "TestLibrary";
            var cop = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                new[] { syntaxTree },
                new MetadataReference[]
                {
          MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
                },
                cop);
            CSharpCompilation compilation1 = GenerateCode(code);
            var assemblyPath = Path.ChangeExtension(Path.GetTempFileName(), "exe");
            var result = compilation1.Emit(assemblyName);
            List<string> errors = result.Diagnostics.Select(e => e.ToString()).ToList();
            return errors;
        }
        private static CSharpCompilation GenerateCode(string sourceCode)
        {
            var codeString = SourceText.From(sourceCode);
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3);

            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);

            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location),
            };

            return CSharpCompilation.Create("Hello.dll",
                new[] { parsedSyntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.ConsoleApplication,
                    optimizationLevel: OptimizationLevel.Release,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
        }


    }
}
