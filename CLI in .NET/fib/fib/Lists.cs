using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fib
{
    public class Lists
    {
        public static Dictionary<string, List<string>> languageExtensions { get; } = new Dictionary<string, List<string>>
        {
            { "C#", new List<string> { ".cs" } },
            { "Java", new List<string> { ".java" } },
            { "Python", new List<string> { ".py" } },
            { "JavaScript", new List<string> { ".js" } },
            { "C++", new List<string> { ".cpp", ".h" } },
            { "C", new List<string> { ".c", ".h" } },
            { "PHP", new List<string> { ".php" } },
            { "Ruby", new List<string> { ".rb" } },
            { "Go", new List<string> { ".go" } },
            { "Swift", new List<string> { ".swift" } },
            { "Kotlin", new List<string> { ".kt", ".kts" } },
            { "Rust", new List<string> { ".rs" } },
            { "TypeScript", new List<string> { ".ts" } },
            { "HTML", new List<string> { ".html", ".htm" } },
            { "CSS", new List<string> { ".css" } },
            { "Assembly", new List<string> { ".asm", ".s" } },
            { "Shell", new List<string> { ".sh" } },
            { "SQL", new List<string> { ".sql" } },
            { "R", new List<string> { ".R", ".r" } },
            { "Dart", new List<string> { ".dart" } },
            {"txt",new List<string> {".txt"} }
        };

        // Class-level list for ignored directories
        public static List<string> ignoredDirectories { get; } = new List<string>
        {
            "node_modules",
            "bin",
            "obj",
            "debug",
            "dist",
            "build",
            ".vs",
            "out",
            "tmp",
            "log",
            ".idea",
            ".vscode",
            "coverage",
            "*.sublime-workspace",
            ".DS_Store"
        };
    }
}
