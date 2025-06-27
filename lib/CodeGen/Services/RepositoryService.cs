using CodeGen.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Services
{
    internal class RepositoryService
    {
        /// <summary>
        /// This method generates a Repository file based on the specified input name.
        /// </summary>
        /// <param name="inputName"></param>
        internal static void GenerateRepository(string inputName)
        {
            Console.Write($"Do you want to create the Repository file? (y/n): ");
            var answer = Console.ReadLine()?.Trim().ToLower();

            if (answer == "y" || answer == "yes")
            {
                Utilities.GenerateFromTemplate("IRepository", $"src/Application/Interfaces/Repositories/I{inputName}Repository.cs", inputName);
                Utilities.GenerateFromTemplate("Repository", $"src/Infrastructure/Persistence/Repositories/{inputName}Repository.cs", inputName);
                UpdateRepositoryRegistration(inputName);
            }
            else
            {
                Console.WriteLine($"Skipped creating Entity file.");
            }
        }

        /// <summary>
        /// This method updates the ServiceRegistration file to include a repository registration for the specified feature.
        /// </summary>
        /// <param name="feature"></param>
        internal static void UpdateRepositoryRegistration(string feature)
        {
            var basePath = Utilities.basePath();
            var filePath = Path.Combine(basePath, "src", "Infrastructure", "Persistence", "ServiceRegister", "RepositoriesRegistration.cs");
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} does not exist. Please create it first.");
                return;
            }
            var registrationLine = $"        services.AddScoped<I{feature}Repository, {feature}Repository>();";

            var servicesLines = File.ReadAllLines(filePath).ToList();
            int insertIndex = servicesLines.FindLastIndex(line => line.Contains("services.AddScoped<") || line.Contains("services.AddTransient<") || line.Contains("services.AddSingleton<"));

            if (insertIndex < 0)
            {
                insertIndex = servicesLines.FindLastIndex(line => line.Contains(" public void AddServices("));
                for (int i = insertIndex; i < servicesLines.Count(); i++)
                {
                    if (servicesLines[i].Contains("{"))
                    {
                        insertIndex = i + 1;
                        break;
                    }
                }
            }

            if (insertIndex >= 0)
            {
                servicesLines.Insert(insertIndex + 1, registrationLine);
                File.WriteAllLines(filePath, servicesLines);
                Console.WriteLine($"📁 Updated: {filePath} with service registration for {feature}.");
            }



        }
    }
}
