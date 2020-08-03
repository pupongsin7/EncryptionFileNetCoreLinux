using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Pojjaman2_EncryptionFile.Interfaces;

namespace Pojjaman2_EncryptionFile.Services
{
    public class EncryptionFileService : IServiceEncryptionFile
    {
        private IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<EncryptionFileService> _logger;

        public EncryptionFileService(ILogger<EncryptionFileService> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _hostingEnvironment = environment;
        }
        public async Task<string> Encryption(string filename, string textFile)
        {
            
               string RootPath = await Task.Run(() => CreateTextFile(filename, textFile));
               string ResEncrypt = await Task.Run(() => EncryptionFile(RootPath, filename));
                
            return ResEncrypt;
        }
        private string CreateTextFile(string filename,string textFile)
        {
            string RootPath = _hostingEnvironment.ContentRootPath;
            string contentPathFile = string.Format(@"{0}{1}", RootPath, @"\FileEncryption\" + filename + ".txt");
            // string contentPathFile = Path.Combine(RootPath, @"\FileEncryption\" + filename + ".txt");
            Encoding encoding = new UTF8Encoding(true); //Or any other Encoding
            if (File.Exists(contentPathFile))
            {
                File.Delete(contentPathFile);
            }
            using (var sw = new StreamWriter(File.Open(contentPathFile, FileMode.CreateNew), encoding))
            {
                sw.WriteLine(textFile);
            }
            return RootPath;
        }
        private string EncryptionFile(string RootPath, string filename)
        {
            string ClassPath1 = @"C:\Program Files (x86)\Java\j2re1.4.2_04\lib\security";
            //string ClassPath1 = @"%JAVA_HOME%\lib\security";
            string SetPath1 = @"set path = C:\Program Files (x86)\Java\j2re1.4.2_04\bin;.;..;";
            string SetClassPath1 = @"set classpath = C:\Program Files (x86)\Java\j2re1.4.2_04\bin;.;..;";

            string PathToFolderEncryptJar = string.Format(@"{0}{1}", RootPath, @"\FileEncryption");
            string CmdString = @"java -jar AESEncrypter.jar " + PathToFolderEncryptJar + @"\" + filename + @".txt " + PathToFolderEncryptJar;

            if (File.Exists(string.Format(@"{0}{1}", ClassPath1, @"\US_export_policy.jar")))
            {
                //string strCmdText = @"cd " + PathToFolderEncryptJar + " && " + SetPath1 + " && " + SetClassPath1 + " && " + CmdString;
                string strCmdText = @"cd " + PathToFolderEncryptJar + " && " + CmdString;
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.WorkingDirectory = @"C:\Windows\System32";
                startInfo.FileName = @"C:\Windows\System32\cmd.exe";
                startInfo.Arguments = "/c " + strCmdText;
                startInfo.UseShellExecute = false;
                process.StartInfo = startInfo;
                process.StartInfo.CreateNoWindow = true;
                process.OutputDataReceived += (s, ea) => Console.WriteLine(ea.Data);
                process.ErrorDataReceived += (s, ea) => Console.WriteLine("ERR: " + ea.Data);

                bool started = process.Start();
                if (!started)
                {
                    //you may allow for the process to be re-used (started = false) 
                    //but I'm not sure about the guarantees of the Exited event in such a case
                    throw new InvalidOperationException("Could not start process: " + process);
                }

                process.WaitForExit();
                return strCmdText;
            }
            else return "FailedEncrypt";
          
           
        }
    }
}