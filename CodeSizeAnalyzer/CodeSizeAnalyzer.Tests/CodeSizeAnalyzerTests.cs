using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<CodeSizeAnalyzer.CodeSizeAnalyzer>;

namespace CodeSizeAnalyzer.Tests;

public class CodeSizeAnalyzerTests
{
    [Fact]
    public async Task MethodShouldBeNoLongerThan30Lines()
    {
        const string text = """
                            public class MyLongClass
                            {
                                public void MyLongMethod()
                                {
                                    var x = 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                    x = x + 1;
                                }
                            }
                            """;
        
        var expected = Verifier.Diagnostic("MethodTooLongRule")
            .WithLocation(3, 17)
            .WithArguments(["MyLongMethod", "MyLongClass"]);
        
        await Verifier.VerifyAnalyzerAsync(text, expected);
    }

    [Fact]
    public async Task ClassShouldBeNoLongerThan300Lines()
    {
        const string methodText = """
                              public void MyMethod_000()
                              {
                                  var x = 1;
                                  x = x + 1;
                                  x = x + 1;
                                  x = x + 1;
                                  x = x + 1;
                                  x = x + 1;
                                  x = x + 1;
                                  x = x + 1;
                                  x = x + 1;
                                  x = x + 1;
                                  x = x + 1;
                                  x = x + 1;
                                  x = x + 1;
                                  x = x + 1;
                                  x = x + 1;
                                  x = x + 1;
                                  x = x + 1;
                                  x = x + 1;
                                  x = x + 1;
                              }
                              """;
        
        var classText = """
                        public class MyLongClass
                        {
                            MMM
                        }
                        """;

        var methods = Enumerable.Range(1, 20)
            .Select(m => methodText.Replace("000", m.ToString()));
            
        classText = classText.Replace("MMM", string.Join(Environment.NewLine, methods));
        
        var expected = Verifier.Diagnostic("ClassTooLongRule")
            .WithLocation(1, 14)
            .WithArguments(["MyLongClass"]);
        
        await Verifier.VerifyAnalyzerAsync(classText, expected);
    }

}