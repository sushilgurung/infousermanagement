using CodeGen.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Services
{
    internal class InterfaceServices
    {
        /// <summary>
        /// This method generates an Services file based on the specified input name.
        /// </summary>
        /// <param name="inputName"></param>
        internal static void GenerateInterface(string inputName)
        {
            Console.Write($"Do you want to create the Service file? (y/n): ");
            var answer = Console.ReadLine()?.Trim().ToLower();

            if (answer == "y" || answer == "yes")
            {
                Utilities.GenerateFromTemplate("Interface", $"src/Application/Interfaces/Services/I{inputName}Service.cs", inputName);
                Utilities.GenerateFromTemplate("Service", $"src/Infrastructure/Persistence/Services/{inputName}Service.cs", inputName);
                UpdateServiceRegistration(inputName);
            }
            else
            {
                Console.WriteLine($"Skipped creating Entity file.");
            }
        }

        /// <summary>
        /// This method updates the ServiceRegistration file to include a service registration for the specified feature.
        /// </summary>
        /// <param name="feature"></param>
        internal static void UpdateServiceRegistration(string feature)
        {
            var basePath = Utilities.basePath();
            var filePath = Path.Combine(basePath, "src", "Infrastructure", "Persistence", "ServiceRegister", "ServiceRegistration.cs");
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} does not exist. Please create it first.");
                return;
            }
            var registrationLine = $"        services.AddScoped<I{feature}Service, {feature}Service>();";

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
