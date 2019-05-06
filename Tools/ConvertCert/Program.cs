using System;
using System.Collections.Generic;
using System.IO;

namespace ConvertCert
{
    class Program
    {
        static void Main(string[] args)
        {
            // Start Application.
            Console.WriteLine("----------------------------");
            Console.WriteLine("------- ConvertCert --------");
            Console.WriteLine("----------------------------");
            Console.WriteLine("Version V1.0.0");

            // Verify Argument.
            if (args.Length != 3)
            {
                Console.WriteLine("Error Args !");
                return;
            }

            // Verify Argument 1.
            string strRepositoryFrom = args[0];
            if (Directory.Exists(strRepositoryFrom) == false)
            {
                Console.WriteLine("Error Directory From !");
                return;
            }

            // Verify Argument 2.
            string strRepositoryTo = args[1];
            if (Directory.Exists(strRepositoryTo) == false)
            {
                Console.WriteLine("Error Directory To !");
                return;
            }

            // Verify Argument 3.
            string pfxPassword = args[2];
            if (pfxPassword == "")
            {
                Console.WriteLine("Error Password !");
                return;
            }

            // Work.
            ConvertDirectory(strRepositoryFrom, strRepositoryTo, pfxPassword);
        }

        private static void ConvertDirectory(string strRepositoryFrom, string strRepositoryTo, string pfxPassword)
        {
            // List of Files.
            string[] files = Directory.GetFiles(strRepositoryFrom, "*.pfx");
            foreach (string strFileFrom in files)
            {
                // Construct the secrets file.
                string strSecretName = Path.GetFileNameWithoutExtension(strFileFrom);

                // Construct the final file.
                string strFileTo = Path.Combine(strRepositoryTo, "k8s-secret-" + strSecretName + ".json");

                // Work File.
                ConvertFile(strFileFrom, strFileTo, pfxPassword);
            }
        }

        private static void ConvertFile(string strFileFrom, string strFileTo, string pfxPassword)
        {
            // Load File.
            byte[] bytes = null;
            try
            {
                bytes = File.ReadAllBytes(strFileFrom);
            }
            catch(Exception) {}
            if (bytes == null)
            {
                Console.WriteLine("Error Read File : " + strFileFrom);
                return;
            }

            // Convert.
            string pfxString = Convert.ToBase64String(bytes);
            string[] jsonString = ConvertJson(pfxString, pfxPassword);

            // Save File.
            try
            {
                File.WriteAllLines(strFileTo, jsonString, System.Text.Encoding.UTF8);
            }
            catch (Exception)
            {
                Console.WriteLine("Error Write File : " + strFileTo);
                return;
            }
        }

        private static string[] ConvertJson(string pfxString, string pfxPassword)
        {
            List<string> listLine = new List<string>();
            listLine.Add("{");
            listLine.Add("	\"Kubernete_CertData\": \"" + pfxString+ "\",");
            listLine.Add("	\"Kubernete_CertPwd\": \"" + pfxPassword + "\"");
            listLine.Add("}");
            return listLine.ToArray();
        }
    }
}
