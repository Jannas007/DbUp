using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DbUp;
using DbUp.Engine;
using DbUp.Helpers;
using DbUp.Support;

namespace DbUp
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = args.FirstOrDefault(x => x.StartsWith("--ConnectionString", StringComparison.OrdinalIgnoreCase));

            connectionString = connectionString.Substring(connectionString.IndexOf("=") + 1).Replace(@"""", string.Empty) + ("Allow User Variables = True;");

            EnsureDatabase.For.MySqlDatabase(connectionString);

            var basePath = args.FirstOrDefault(x => x.StartsWith("--UpdateScripts", StringComparison.OrdinalIgnoreCase));

            var updatePath = "";

            var tablePath = "";

            var sprocPath = "";

            var funcPath = "";

            if (args.Any(a => a.StartsWith("--UpdateScripts", StringComparison.InvariantCultureIgnoreCase)))
            {
               basePath = basePath.Substring(basePath.IndexOf("=") + 1).Replace(@"""", string.Empty);

               tablePath = basePath + "Tables";

               updatePath = basePath + "Updates";

               sprocPath = basePath + "Sprocs";

               funcPath =  basePath + "Functions";

            }
            else
            {
                basePath = "none";
                Console.WriteLine("No values specified for UpdateScript path");
            }
            // Console.WriteLine($"The BasePath is: {basePath}");
            // Console.WriteLine($"The Tablepath is: {tablePath}");
            // Console.WriteLine($"The Updatepath is: {updatePath}");
            // Console.WriteLine($"The Stored Proc path is: {sprocPath}");
            // Console.WriteLine($"The Functions path is: {funcPath}");
            // Console.ReadKey();

            

            var upgradeEngineBuilder = DeployChanges.To
                .MySqlDatabase(connectionString, null)
                // .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), x => x.StartsWith("DbUp.BeforeDeploymentScripts."), new SqlScriptOptions { ScriptType = ScriptType.RunAlways, RunGroupOrder = 0 })
                // .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), x => x.StartsWith("DbUp.DeploymentScripts"), new SqlScriptOptions { ScriptType = ScriptType.RunOnce, RunGroupOrder = 1 })
                // .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), x => x.StartsWith("DbUp.PostDeploymentScripts."), new SqlScriptOptions { ScriptType = ScriptType.RunAlways, RunGroupOrder = 2 })

                .WithScriptsFromFileSystem(tablePath, new SqlScriptOptions { ScriptType = ScriptType.RunOnce})
                .WithScriptsFromFileSystem(updatePath, new SqlScriptOptions { ScriptType = ScriptType.RunOnce })
                .WithScriptsFromFileSystem(sprocPath, new SqlScriptOptions { ScriptType = ScriptType.RunAlways })
                .WithScriptsFromFileSystem(funcPath, new SqlScriptOptions { ScriptType = ScriptType.RunAlways })
                .WithTransactionPerScript()
                .LogToConsole();

            var upgrader = upgradeEngineBuilder.Build();

            Console.WriteLine("Is upgrade required: " + upgrader.IsUpgradeRequired());
            
            if (args.Any(a => a.StartsWith("--PreviewReportPath", StringComparison.InvariantCultureIgnoreCase)))
            {
                // Generate a preview file so Octopus Deploy can generate an artifact for approvals
                var report = args.FirstOrDefault(x => x.StartsWith("--PreviewReportPath", StringComparison.OrdinalIgnoreCase));
                report = report.Substring(report.IndexOf("=") + 1).Replace(@"""", string.Empty);

                var fullReportPath = Path.Combine(report, "UpgradeReport.html");

                Console.WriteLine($"Generating the report at {fullReportPath}");
                
                upgrader.GenerateUpgradeHtmlReport(fullReportPath);
            }
            else
            {
                var result = upgrader.PerformUpgrade();

                // Display the result
                if (result.Successful)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Success!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(result.Error);
                    Console.WriteLine("Failed!");
                }
            }
        }
    }
}