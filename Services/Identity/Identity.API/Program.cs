using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace XJeunot.PhysicalStoreApps.Services.Identity.API
{
    public class Program
    {
        public static readonly string CLUSTER_AZURE = "Azure";
        public static readonly string CLUSTER_KUBERNETE_ALONE = "KuberneteAlone";

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    var builtConfig = config.Build();
                    var configurationBuilder = new ConfigurationBuilder();

                    /*
                     * Azure Cluster.
                     */
                    if (builtConfig["ClusterMode"] == Program.CLUSTER_AZURE)
                    {
                        string strAzureADApplicationId = string.Empty;
                        string strAzureADPassword = string.Empty;

                        if (builtConfig.GetSection("Azure:AzureADApplicationId").Exists() &&
                            !string.IsNullOrEmpty(builtConfig.GetSection("Azure:AzureADApplicationId").Value))
                        {
                            strAzureADApplicationId = builtConfig.GetSection("Azure:AzureADApplicationId").Value;
                        }

                        if (builtConfig.GetSection("Azure:AzureADPassword").Exists() &&
                            !string.IsNullOrEmpty(builtConfig.GetSection("Azure:AzureADPassword").Value))
                        {
                            strAzureADPassword = builtConfig.GetSection("Azure:AzureADPassword").Value;
                        }

                        if ((strAzureADApplicationId != string.Empty) &&
                            (strAzureADPassword != string.Empty))
                        {
                            configurationBuilder.AddAzureKeyVault(
                                $"https://{builtConfig["Azure:KeyVaultName"]}.vault.azure.net/",
                                strAzureADApplicationId,
                                strAzureADPassword);
                        }
                        else
                        {
                            configurationBuilder.AddAzureKeyVault(
                                $"https://{builtConfig["Azure:KeyVaultName"]}.vault.azure.net/");
                        }
                    }

                    /*
                     * Kubernete Alone Cluster.
                     */
                    if (builtConfig["ClusterMode"] == Program.CLUSTER_KUBERNETE_ALONE)
                    {
                        configurationBuilder.AddJsonFile(builtConfig["K8s:SecretFile"], optional: false);
                    }

                    configurationBuilder.AddEnvironmentVariables();
                    config.AddConfiguration(configurationBuilder.Build());
                })
                .ConfigureKestrel((context, options) =>
                {
                    options.Listen(IPAddress.Any, 443, listenOptions =>
                    {
                        listenOptions.UseHttps(BuildX509Certificate2(context.Configuration));
                    });
                });

        private static X509Certificate2 BuildX509Certificate2(IConfiguration configuration)
        {
            if ((configuration["ClusterMode"] == Program.CLUSTER_AZURE) ||
                (configuration["ClusterMode"] == Program.CLUSTER_KUBERNETE_ALONE))
            {
                string strCertificateSecretName = configuration["Cluster:CertificateSecretName"];
                string strCertificateSecretPassword = configuration["Cluster:CertificateSecretPassword"];
                string strCertificateData = configuration[strCertificateSecretName];
                string strCertificatePassword = configuration[strCertificateSecretPassword];
                var pfxBytes = Convert.FromBase64String(strCertificateData);
                return new X509Certificate2(pfxBytes, strCertificatePassword);
            }
            else
            {
                return new X509Certificate2(
                    configuration.GetSection("KestrelNoClusterHttpsCertFile:Path").Value,
                    configuration.GetSection("KestrelNoClusterHttpsCertFile:Password").Value);
            }
        }
    }
}
