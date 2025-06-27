using CodeGen.Common;
using CodeGen.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Services
{
    internal class FeaturesServices
    {
        internal static void GenerateFeatures(string inputName)
        {
            Console.Write($"Do you want to create the Features file? (y/n): ");
            var answer = Console.ReadLine()?.Trim().ToLower();

            if (answer == "y" || answer == "yes")
            {
                string selectedFolderName = string.Empty;
                do
                {
                    Console.Write($"Select folder list: ");
                    var folderList = Utilities.GetFeatureFolders();

                    var optionsFolder = folderList.Select((folderName, index) => new FeatureOptions
                    {
                        Id = index,
                        Label = folderName,
                        Name = folderName
                    }).ToList();

                    optionsFolder.Add(new FeatureOptions
                    {
                        Id = optionsFolder.Count,
                        Label = "➕ Add new folder",
                        Name = "newfolder"
                    });


                    int menuFolderStartRow = Console.CursorTop;
                    var selectedFolder = Utilities.DisplayMenu(optionsFolder, menuFolderStartRow + 1);

                    var selectedoptionsFolder = optionsFolder.FirstOrDefault(x => x.Id == selectedFolder.Id);

                    if (selectedoptionsFolder?.Name == "newfolder")
                    {
                        Console.Write("Enter new feature folder name: ");
                        var newFolderName = Console.ReadLine()?.Trim();
                        if (string.IsNullOrWhiteSpace(newFolderName))
                        {
                            Console.WriteLine("❌ Folder name cannot be empty.");
                            continue;
                        }

                        var basePath = Utilities.GetProjectRootPath();
                        var fullPath = Path.Combine(basePath, "src", "Application", "Features", Utilities.ToPascalCase(newFolderName));
                        if (Directory.Exists(fullPath))
                        {
                            Console.WriteLine("⚠️ Folder already exists. Please enter a different name.");
                            continue;
                        }
                        Directory.CreateDirectory(fullPath);
                        Console.WriteLine($"📁 Created folder: {newFolderName}");
                        continue;
                    }
                    else
                    {
                        selectedFolderName = selectedoptionsFolder?.Name ?? string.Empty;
                    }
                }
                while (string.IsNullOrWhiteSpace(selectedFolderName));

                var options = Utilities.GetCQRSOptions();
                int menuStartRow = Console.CursorTop;
                var selected = Utilities.DisplayMenu(options, menuStartRow + 1);
                string selectedTemplate = string.Empty;

                string relativePath = string.Empty;
                switch (selected.Id)
                {
                    case 0:
                        selectedTemplate = "Command";
                        relativePath = $"src/Application/Features/{selectedFolderName}/Commands/{inputName}/{Utilities.ToPascalCase(inputName)}Command.cs";
                        break;
                    case 1:
                        selectedTemplate = "Query";
                        relativePath = $"src/Application/Features/{selectedFolderName}/Queries/{inputName}/{Utilities.ToPascalCase(inputName)}Query.cs";
                        break;
                    case 4:
                        Console.WriteLine("❌ Operation cancelled.");
                        return;

                    default:
                        Console.WriteLine("❌ Invalid option. Exiting.");
                        return;
                }
                Utilities.GenerateFromTemplate(selectedTemplate, relativePath, Utilities.ToPascalCase(inputName), selectedFolderName);
            }
            else
            {
                Console.WriteLine($"Skipped creating Entity file.");
            }
        }




    }
}
