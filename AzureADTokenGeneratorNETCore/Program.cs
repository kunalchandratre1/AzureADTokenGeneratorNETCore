using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;

namespace AzureADTokenGeneratorNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                RunAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static async Task RunAsync()
        {
            // Even if this is a console application here, a daemon application is a confidential client application
            IConfidentialClientApplication app;
            app = ConfidentialClientApplicationBuilder.Create("Copy your Azure AD registered - Application ID")
                    .WithClientSecret("Azure AD registered application - secret key")
                    .WithAuthority(new Uri("https://login.microsoftonline.com/YourAzureADTenantID"))
                    .Build();

            // With client credentials flows the scopes is ALWAYS of the shape "resource/.default", as the 
            // application permissions need to be set statically (in the portal or by PowerShell), and then granted by
            // a tenant administrator. 
            string[] scopes = new string[] { "https://api.loganalytics.io/.default" };

            AuthenticationResult result = null;
            try
            {
                result = await app.AcquireTokenForClient(scopes)
                    .ExecuteAsync();
                Console.WriteLine("Token acquired");
                Console.WriteLine(result.AccessToken);
            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
            {
                // Invalid scope. The scope has to be of the form "https://resourceurl/.default"
                // Mitigation: change the scope to be as expected                
                Console.WriteLine("Scope provided is not supported");
            }
        }
    }
}
