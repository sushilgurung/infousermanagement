using CodeGen.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Common
{
    internal class Utilities
    {
        /// <summary>
        /// This method retrieves the root path of the project by navigating up from the current directory.
        /// </summary>
        /// <returns></returns>
        internal static string GetProjectRootPath()
        {
            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        }

        /// <summary>
        /// This method retrieves the full path of a file or directory relative to the project root path.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        internal static string GetProjectPath(string relativePath)
        {
            return Path.GetFullPath(Path.Combine(GetProjectRootPath(), relativePath));
        }

        /// <summary>
        /// This method retrieves the base path of the project, which is typically five levels up from the current directory.
        /// </summary>
        /// <returns></returns>
        internal static string basePath()
        {
            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        }

        /// <summary>
        /// This method retrieves the path to a template file based on its name.
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        internal static string GetTemplatePath(string templateName)
        {
            var projectFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
            return Path.Combine(projectFolder, "Templates", $"{templateName}.txt");
        }
        /// <summary>
        /// This method retrieves the content of a template file based on its name.
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        internal static string GetTemplateContent(string templateName)
        {
            var templatePath = GetTemplatePath(templateName);
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Template file '{templateName}' not found at '{templatePath}'.");
            }
            return File.ReadAllText(templatePath);
        }


        internal static bool GenerateFromTemplate(string templateName, string relativePath, string feature, string folderName = "")
        {
            try
            {
                var projectFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
                var templatePath = Path.Combine(projectFolder, "Templates", $"{templateName}.txt");
                var outputPath = Path.Combine(Utilities.basePath(), relativePath);

                Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

                var template = File.ReadAllText(templatePath);
                var content = template
                    .Replace("{{Feature}}", feature)
                    .Replace("{{Feature pascalName}}", ToPascalCase(feature))
                    .Replace("{{Feature camelCase}}", ToCamelCase(feature))
                    .Replace("{{FolderName}}", !string.IsNullOrWhiteSpace(folderName) ? folderName : "");

                File.WriteAllText(outputPath, content);
                Console.WriteLine($"📁 Created: {outputPath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error generating {templateName}: {ex.Message}");
                return false;
            }
        }

        internal static string ToPascalCase(string input) =>
        string.Join("", input.Split('-', '_', ' ', (char)StringSplitOptions.RemoveEmptyEntries)
                             .Select(w => char.ToUpperInvariant(w[0]) + w.Substring(1)));

        internal static string ToCamelCase(string input)
        {
            var pascal = ToPascalCase(input);
            return char.ToLowerInvariant(pascal[0]) + pascal.Substring(1);
        }

        internal static FeatureOptions DisplayMenu(List<FeatureOptions> options, int startRow)
        {
            int current = 0;
            int selectedIndex = -1;
            ConsoleKey key;

            do
            {
                for (int i = 0; i < options.Count; i++)
                {
                    int row = startRow + i;

                    if (row >= Console.BufferHeight - 1)
                    {
                        Console.WriteLine("Not enough space in the console window. Please enlarge the console and try again.");
                        Environment.Exit(1);
                    }


                    Console.SetCursorPosition(0, row);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, row);

                    string prefix = (i == selectedIndex) ? "[x]" : "[ ]";
                    if (i == current)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine($"{prefix} {options[i].Label}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"{prefix} {options[i].Label}");
                    }
                }

                var keyInfo = Console.ReadKey(true);
                key = keyInfo.Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (current > 0) current--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (current < options.Count - 1) current++;
                        break;
                    case ConsoleKey.Spacebar:
                        selectedIndex = current; // only one selection allowed
                        break;
                }

            } while (key != ConsoleKey.Enter);

            return selectedIndex >= 0 ? options[selectedIndex] : options[current];
        }

        /// <summary>
        /// This method retrieves a list of feature folders from the specified base path.
        /// </summary>
        /// <param name="basePath"></param>
        /// <returns></returns>
        internal static List<string> GetFeatureFolders()
        {
            var featuresPath = Path.Combine(basePath(), "src", "Application", "Features");
            if (!Directory.Exists(featuresPath))
            {
                Console.WriteLine("Features folder does not exist.");
                return new List<string>();
            }
            // Get all directories directly under Features
            var directories = Directory.GetDirectories(featuresPath, "*", SearchOption.TopDirectoryOnly)
                                       .Select(Path.GetFileName)
                                       .ToList();
            //Directory.GetDirectories(featuresPath).Select(Path.GetFileName).ToList()
            return directories;
        }



        /// <summary>
        /// This method retrieves a list of feature options available for code generation.
        /// </summary>
        /// <returns></returns>
        internal static List<FeatureOptions> GetFeatureOptions()
        {
            return new List<FeatureOptions>
            {
                new FeatureOptions
                {
                    Id= 0,
                    Label = "Features (Handler, Validator, Request)",
                    Name = "Features",
                    Features = new() { "Handler", "Validator", "Request" },
                    Prompts = new() { "Name", "Questions" }
                },
                 new FeatureOptions
                {
                    Id= 1,
                    Label = "Service",
                    Name = "Service",
                    Features = new List<string> { "Interface & Service" },
                    Prompts = new List<string>
                    {
                        "Enter the name of the controller (e.g., ProductController):"
                    }
                },
                 new FeatureOptions
                {
                    Id= 2,
                    Label = "Repository",
                    Name = "Repository",
                    Features = new List<string> { "Repository" },
                    Prompts = new List<string>
                    {
                        "Enter the name of the controller (e.g., ProductController):"
                    }
                },
                new FeatureOptions
                {
                    Id= 3,
                    Label = "Entity",
                    Name = "Entity",
                    Features = new List<string> { "Entity" },
                    Prompts = new List<string>
                    {
                        "Enter the name of the entity (e.g., Product, Order):",
                        "Enter the properties of the entity (comma-separated):"
                    }
                },
                new FeatureOptions
                {
                    Id= 4,
                    Label = "DTO",
                    Name = "DTO",
                    Features = new List<string> { "DTO" },
                    Prompts = new List<string>
                    {
                        "Enter the name of the controller (e.g., ProductController):"
                    }
                },
                   new FeatureOptions
                {
                    Id= 5,
                    Label = "All",
                    Name = "Features",
                    Features = new() { "Entity", "DTO", "Interface", "Service", "Handler", "Validator", "Request" },
                    Prompts = new() { "Name", "Questions" }
                },
                    new FeatureOptions
                {
                        Label = "Cancel",
                        Features = new() } 
                // Add more feature options as needed
            };
        }

        /// <summary>
        /// This method retrieves a list of CQRS options available for code generation.
        /// </summary>
        /// <returns></returns>
        internal static List<FeatureOptions> GetCQRSOptions()
        {
            return new List<FeatureOptions>
            {
                new FeatureOptions
                {
                    Id= 0,
                    Label = "Command Handler",
                    Name = "Command",
                    Features = new() { "Command Handler" },
                    Prompts = new() { "Name", "Questions" }
                },
                 new FeatureOptions
                {
                    Id= 1,
                    Label = "Query Handler",
                    Name = "Query",
                    Features = new List<string> { "Query" },
                    Prompts = new List<string>
                    {
                        "Query"
                    }
                },
            };
        }

    }
}
