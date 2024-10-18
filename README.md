# IctBaden CodeSizeAnalyzer
Roslyn Analyzer to warn about too large classes and methods.

Thanks **Max Jung** for initial Chat-GPT powered help.

## Configuration
Default accepted sizes are as follows:
* Method: 30 lines
* Class: 300 lines

To set accepted sizes use .editorconfig

```
root = true

[*.cs]
dotnet_diagnostic.code_length.max_method_lines = 22
dotnet_diagnostic.code_length.max_class_lines = 222
```

