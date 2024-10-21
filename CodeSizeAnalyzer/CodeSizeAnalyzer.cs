using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeSizeAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class CodeSizeAnalyzer : DiagnosticAnalyzer
{
    private static int _maxClassLines = 300;
    private static int _maxMethodLines = 30;

    private static DiagnosticDescriptor ClassTooLongRule => new(
        "ClassTooLongRule",
        "Class is too long",
        $"Class {0} exceeds the maximum accepted length ({_maxClassLines} lines)",
        "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static DiagnosticDescriptor MethodTooLongRule => new(
        "MethodTooLongRule",
        "Method is too long",
        $"Method {0} in class {1} exceeds the maximum accepted length ({_maxMethodLines} lines)",
        "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ClassTooLongRule, MethodTooLongRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ClassDeclaration, SyntaxKind.MethodDeclaration);
    }

    private void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var config = context.Options.AnalyzerConfigOptionsProvider.GetOptions(context.Node.SyntaxTree);
        
        config.TryGetValue("dotnet_diagnostic.code_length.max_class_lines", out var cfgMaxLines);
        if(int.TryParse(cfgMaxLines, out var maxLines)) _maxClassLines = maxLines;
        
        config.TryGetValue("dotnet_diagnostic.code_length.max_method_lines", out cfgMaxLines);
        if(int.TryParse(cfgMaxLines, out maxLines)) _maxMethodLines = maxLines;
        
        if (context.Node is ClassDeclarationSyntax classNode)
        {
            var lineCount = classNode.GetText().Lines.Count;
            if (lineCount > _maxClassLines)
            {
                var diagnostic = Diagnostic.Create(ClassTooLongRule, classNode.Identifier.GetLocation(), classNode.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }
        }
        else if (context.Node is MethodDeclarationSyntax methodNode)
        {
            var lineCount = methodNode.GetText().Lines.Count;
            if (lineCount > _maxMethodLines)
            {
                var classDeclaration = methodNode.FirstAncestorOrSelf<ClassDeclarationSyntax>();
                var diagnostic = Diagnostic.Create(MethodTooLongRule, methodNode.Identifier.GetLocation(), [methodNode.Identifier.Text, classDeclaration?.Identifier.Text]);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
