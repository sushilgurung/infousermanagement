using CodeGen.Common;
using CodeGen.Dtos;
using CodeGen.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("\n🛠️ What do you want to generate?");
        List<FeatureOptions> featureOptions = Utilities.GetFeatureOptions();

        int menuStartRow = Console.CursorTop;
        var selected = DisplayMenu(featureOptions, menuStartRow + 1);

        Console.Write($"Enter {selected.Name} name:");
        var input = Console.ReadLine() ?? "";
        var feature = Utilities.ToPascalCase(input);

        switch (selected.Id)
        {
            case 0:
                FeaturesServices.GenerateFeatures(feature);
                break;

            case 1:
                InterfaceServices.GenerateInterface(feature);
                break;

            case 2:
                RepositoryService.GenerateRepository(feature);
                break;

            case 3:
                EntityService.GenerateEntity(feature);
                break;

            case 4:
                Console.WriteLine("❌ Operation cancelled.");
                return;

            default:
                Console.WriteLine("❌ Invalid option. Exiting.");
                return;
        }



        // GenerateEntity("Entity", $"src/Domain/Entities/{feature}.cs", feature, basePath);
        // bool dtoCreated = AskAndGenerate("Entity", $"src/Domain/Entities/{feature}.cs", feature, basePath);
        //GenerateFromTemplate("Dto", $"src/Application/Features/{feature}/Dtos/{feature}Dto.cs", feature, basePath);
        //GenerateFromTemplate("Entity", $"src/Domain/Entities/{feature}.cs", feature, basePath);
        //GenerateFromTemplate("Interface", $"src/Application/Interfaces/I{feature}Service.cs", feature, basePath);
        //GenerateFromTemplate("Service", $"src/Infrastructure/Services/{feature}Service.cs", feature, basePath);

        // UpdateServiceRegistration(feature, basePath);
        // Console.WriteLine($"✅ Generated code for: {feature}");
    }

    static FeatureOptions DisplayMenu(List<FeatureOptions> options, int startRow)
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



    //static bool AskAndGenerate(string templateName, string relativePath, string feature, string basePath)
    //{
    //    Console.Write($"Do you want to create the {templateName} file? (y/n): ");
    //    var answer = Console.ReadLine()?.Trim().ToLower();

    //    if (answer == "y" || answer == "yes")
    //    {
    //        return GenerateFromTemplate(templateName, relativePath, feature, basePath);
    //    }
    //    else
    //    {
    //        Console.WriteLine($"Skipped creating {templateName} file.");
    //        return false;
    //    }
    //}

    //static string ToPascalCase(string input) =>
    //    string.Join("", input.Split('-', '_', ' ')).Replace(" ", "");





    static void GenerateInterface(string templateName, string relativePath, string feature, string basePath)
    {
        var projectFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
        var templatePath = Path.Combine(projectFolder, "Templates", $"{templateName}.txt");
        var outputPath = Path.Combine(basePath, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        var template = File.ReadAllText(templatePath);
        var content = template.Replace("{{Feature}}", feature);
        File.WriteAllText(outputPath, content);
        Console.WriteLine($"📁 Created: {outputPath}");
    }



    static void UpdateServiceRegistration(string feature, string basePath)
    {
        var filePath = Path.Combine(basePath, "Infrastructure/Persistence/ServiceRegister/ServiceRegistration.cs");
        var marker = "// PLOP: AddServiceHere";
        var registrationLine = $"        services.AddScoped<I{feature}Service, {feature}Service>();";

        if (!File.Exists(filePath)) return;

        var lines = File.ReadAllLines(filePath).ToList();
        int markerIndex = lines.FindIndex(x => x.Contains(marker));
        if (markerIndex == -1) return;

        if (!lines.Any(x => x.Contains($"I{feature}Service")))
        {
            lines.Insert(markerIndex, registrationLine);
            File.WriteAllLines(filePath, lines);
            Console.WriteLine("🛠 Updated ServiceRegistration.cs");
        }
    }





}
