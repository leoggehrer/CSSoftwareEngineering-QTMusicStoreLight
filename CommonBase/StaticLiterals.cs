﻿namespace CommonBase
{
    public static class StaticLiterals
    {
        public static string SolutionFileExtension => ".sln";
        public static string ProjectFileExtension => ".csproj";
        public static string[] SolutionProjects => new string[]
        {
            "CommonBase",
        };
        public static string[] ProjectExtensions => new string[]
        {
            ".ConApp",
            ".Logic",
            ".WebApi",
            ".AspMvc",
            ".Angular",
            ".WpfApp",
        };
        public static string[] SolutionToolProjects { get; } = new[]
        {
            "TemplateComparsion.ConApp",
            "TemplateCopier.ConApp",
        };

        public static string GeneratedCodeLabel => "@GeneratedCode";
        public static string IgnoreLabel => "@Ignore";
        public static string BaseCodeLabel => "@CodeCopy";
        public static string CodeCopyLabel => "@CodeCopy";
        public static string SourceFileExtensions => "*.css|*.cs|*.ts|*.cshtml|*.razor|*.razor.cs|*.template";
    }
}
