using CodeGen.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Services
{
    public class EntityService
    {
        /// <summary>
        /// This method generates an Entity file based on the specified template name, relative path, feature name, and base path.
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="relativePath"></param>
        /// <param name="feature"></param>
        /// <param name="basePath"></param>
        public static void GenerateEntity(string inputName)
        {
            Console.Write($"Do you want to create the Entity file? (y/n): ");
            var answer = Console.ReadLine()?.Trim().ToLower();

            if (answer == "y" || answer == "yes")
            {
                Utilities.GenerateFromTemplate("Entity", $"src/Domain/Entities/{inputName}.cs", inputName);
                Utilities.GenerateFromTemplate("EntityConfiguration", $"src/Infrastructure/Persistence/Configurations/AppicationDbConfigurations/{inputName}Configuration.cs", inputName);
                UpdateDbContexts(inputName);
            }
            else
            {
                Console.WriteLine($"Skipped creating Entity file.");
            }
        }

        /// <summary>
        /// This method updates the ApplicationDbContext to include a DbSet for the specified feature.
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        static bool UpdateDbContexts(string feature)
        {
            var dbContextDir = Path.Combine(Utilities.basePath(), "src", "Infrastructure", "Persistence", "Contexts");
            var dbContextPath = Path.Combine(dbContextDir, "ApplicationDbContext.cs");
            // Ensure the directory exists  
            Directory.CreateDirectory(dbContextDir);
            if (!File.Exists(dbContextPath))
            {
                string templateContent = Utilities.GetTemplateContent("ApplicationDbContext");
                File.WriteAllText(dbContextPath, templateContent.Trim());
            }

            //string templatePath = GetTemplatePath("ApplicationDbContextTemplate");
            var dbContextContent = File.ReadAllLines(dbContextPath);

            var dbSetLine = $"        public DbSet<{feature}> {feature}s {{ get; set; }}";

            // Check if DbSet already exists  
            if (dbContextContent.Any(line => line.Contains($"DbSet<{feature}>")))
            {
                Console.WriteLine("⚠️ DbSet already exists in ApplicationDbContext.");
                return false;
            }

            // Find the last index where a DbSet is declared  
            int insertIndex = Array.FindLastIndex(dbContextContent, line => line.Trim().StartsWith("public DbSet<"));

            if (insertIndex == -1)
            {
                insertIndex = Array.FindLastIndex(dbContextContent, line =>
                line.Trim().Contains("public ApplicationDbContext") && line.Contains("base(options)"));
                // Find closing brace `}` of the constructor to insert after
                //int insertIndex = -1;
                for (int i = insertIndex; i < dbContextContent.Count(); i++)
                {
                    if (dbContextContent[i].Contains("}") || dbContextContent[i].Contains("}}"))
                    {
                        insertIndex = i + 1;
                        break;
                    }
                }
            }

            if (insertIndex != -1)
            {
                var updatedContent = dbContextContent.ToList();
                updatedContent.Insert(insertIndex + 1, dbSetLine);
                File.WriteAllLines(dbContextPath, updatedContent);
                Console.WriteLine("🛠 Updated ApplicationDbContext.cs");
            }

            return true;
        }

    }
}
