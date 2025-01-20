
using fib;
using System.CommandLine;
void AddPathsToList(List<string> paths,string directory,string[] languages)
{
    foreach (var file in Directory.GetFiles(directory))
    {
        foreach (var language in languages)
        {
            if (languages[0] == "all" || Lists.languageExtensions.ContainsKey(language) && Lists.languageExtensions[language].Contains(Path.GetExtension(file)))
            {
                paths.Add(file);
            }
        }
    }
    foreach(var d in Directory.GetDirectories(directory))
    {
        if (!Lists.ignoredDirectories.Contains(Path.GetFileName(d)))
            AddPathsToList(paths, d,languages);
    }
}

var bundleOptionOutput = new Option<FileInfo>("--output", "File path and name");
bundleOptionOutput.AddAlias("-o");
var bundleOptionLanguages = new Option<string>("--languages", "Desired languages") { IsRequired = true};
bundleOptionLanguages.AddAlias("-l");
var bundleOptionNote = new Option<bool>("--note", "Add a note to the code source");
bundleOptionNote.AddAlias("-n");
var bundleOptionSort = new Option<string>("--sort", "Sort the file");
bundleOptionSort.AddAlias("-s");
var bundleOptionAuthor = new Option<string>("--author", "Write in the head of file the neme of the author");
bundleOptionAuthor.AddAlias("-a");
var bundleOptionRemoveLines = new Option<bool>("--remove", "Remove empty lines from file");
bundleOptionRemoveLines.AddAlias("-r");
var CreateRspCommand = new Command("--create-rsp", "Create response file");
CreateRspCommand.AddAlias("c");
var bundleCommand = new Command("bundle", "Bundle code files to a single file");
bundleCommand.AddAlias("b");
bundleCommand.AddOption(bundleOptionOutput);
bundleCommand.AddOption(bundleOptionLanguages);
bundleCommand.AddOption(bundleOptionNote);
bundleCommand.AddOption(bundleOptionSort);
bundleCommand.AddOption(bundleOptionAuthor);
bundleCommand.AddOption(bundleOptionRemoveLines);
bundleCommand.SetHandler((output,languages,note,sortBy, author, removeEmptyLines) =>
{
    try
    {
        List<string> paths = new List<string>();
        Console.WriteLine(output.FullName);
        string directory = Directory.GetCurrentDirectory();
        AddPathsToList(paths, directory, languages.Split(' '));
        paths = sortBy == "type" ? paths.OrderBy(Path.GetExtension).ToList() : paths.OrderBy(Path.GetFileName).ToList();
        
        using (FileStream file = File.Create(output.FullName))
        using (StreamWriter writer = new StreamWriter(file))
        {
            if (!string.IsNullOrEmpty(author))
                writer.WriteLine(author);

            foreach (var item in paths)
            {
                using (StreamReader reader = new StreamReader(item))
                {
                    string content = reader.ReadToEnd();
                    if (removeEmptyLines)
                    {
                        content = string.Join(Environment.NewLine, content.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
                    }
                    if (note)
                        writer.WriteLine("#This code is sourced from: " + item);
                    writer.WriteLine(content);
                }
            }
        }
    }
    catch (FileNotFoundException fnfEx)
    {
        Console.WriteLine($"ERROR: File not found - {fnfEx.Message}");
    }
    catch (UnauthorizedAccessException uaEx)
    {
        Console.WriteLine($"ERROR: Access denied - {uaEx.Message}");
    }
    catch (DirectoryNotFoundException dirEx)
    {
        Console.WriteLine($"ERROR: Directory not found - {dirEx.Message}");
    }
    catch (ArgumentException argEx)
    {
        Console.WriteLine($"ERROR: Invalid argument - {argEx.Message}");
    }
    catch (IOException ioEx)
    {
        Console.WriteLine($"ERROR: I/O error occurred - {ioEx.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: An unexpected error occurred - {ex.Message}");
    }
},bundleOptionOutput, bundleOptionLanguages,bundleOptionNote,bundleOptionSort,bundleOptionAuthor,bundleOptionRemoveLines);

CreateRspCommand.SetHandler(async () => {
    static async Task UserAsyncInputs()
    {
        Console.WriteLine("Enter an output file name");
        string output = Console.ReadLine();

        Console.WriteLine("Enter the desired languages codes to be bundled");
        string languages = Console.ReadLine();

        Console.WriteLine("Enter true/false if you want to write the source of the code :");
        string noteInput = Console.ReadLine();
        bool? note = null;
        if (!string.IsNullOrWhiteSpace(noteInput))
        {
            note = bool.Parse(noteInput);
        }

        Console.WriteLine("Enter the parameter to sort the files - default is alphabetical");
        string sortBy = Console.ReadLine();

        Console.WriteLine("Enter the name of the author to be written in the head of the file");
        string author = Console.ReadLine();

        var commandParts = new List<string> { $"bundle --output {output} --languages \"{languages}\"" };
        if (note.HasValue)
            commandParts.Add($"--note {note.Value}");

        if (!string.IsNullOrWhiteSpace(sortBy))
            commandParts.Add($"--sort {sortBy}");

        if (!string.IsNullOrWhiteSpace(author))
            commandParts.Add($"--author {author}");

        string command = string.Join(" ", commandParts).Trim();
        await File.WriteAllTextAsync("response.rsp", command);
        Console.WriteLine($"You can run the command using: fib @response.rsp");
    }
    await UserAsyncInputs();
});
var rootCommand = new RootCommand("Root command for file bundler CLI");
rootCommand.AddCommand(bundleCommand);
rootCommand.AddCommand(CreateRspCommand);
await rootCommand.InvokeAsync(args);

