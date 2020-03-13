using System;
using System.Configuration;
using System.Reflection;
using DbUp;

namespace PaymentGateway.Database
{
    class Program
    {
        static int Main(string[] args)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["PaymentGatewayDb"].ConnectionString; 

            var engine =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();

            ScriptingUpgrader upgradeScriptingEngine = new ScriptingUpgrader(connectionString, engine);


            var result = upgradeScriptingEngine.Run(args); 

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();

                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            return 0;
        }
    }
}
