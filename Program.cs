using System.Security.Cryptography;

namespace FolderSync
{
    class FolderSync
    {
        private static string logFilePath = "";
        static void Main(string[] args)
        {
            string sourcePath = args[0];
            string replicaPath = args[1];
            int syncInterval = int.Parse(args[2]);
            FolderSync.logFilePath = args[3];

            SynchronizeCallBack(sourcePath, replicaPath);

            System.Timers.Timer executetimer = new System.Timers.Timer();
            executetimer.Interval = TimeSpan.FromSeconds(syncInterval).TotalMilliseconds;
            executetimer.Elapsed += (sender, e) => SynchronizeCallBack(sourcePath, replicaPath);
            executetimer.Start();

            Console.WriteLine("Press Enter at any time to exit.");
            Console.ReadLine();
        }

        private static void SynchronizeCallBack(string sourcePath, string replicaPath)
        {
            if (VerifyFolderExistence(sourcePath, replicaPath))
            {
                Synchronyze(sourcePath, replicaPath);
            };
        }

        static bool VerifyFolderExistence(string sourcePath, string replicaPath)
        {
            if (!Directory.Exists(sourcePath))
            {
                LogAndConsole("Error. Source folder doesn't exist.");
                return false;
            }

            if (!Directory.Exists(replicaPath))
            {
                LogAndConsole("Replica folder doesn't exist. I will create it for you :)");
                Directory.CreateDirectory(replicaPath);
                if (!Directory.Exists(replicaPath))
                {
                    LogAndConsole("Error creating replica folder :(, please create it mannualy.");
                    return false;
                }
                else
                {
                    LogAndConsole("Replica folder created sucessfully!");
                    return true;
                }
            }
            return true;
        }

        static void Synchronyze(string sourcePath, string replicaPath)
        {
            string[] sourceFiles = Directory.GetFiles(sourcePath);
            string[] replicaFiles = Directory.GetFiles(replicaPath);
            int error = 0;

            foreach (string sourceFile in sourceFiles)
            {
                string sourceFileName = Path.GetFileName(sourceFile);
                string replicaFile = Path.Combine(replicaPath, sourceFileName);

                if (!File.Exists(replicaFile))
                {
                    File.Copy(sourceFile, replicaFile);
                    if (CalculateMD5(sourceFile) == CalculateMD5(replicaFile))
                    {
                        LogAndConsole($"File {sourceFileName} copied to replica folder.");
                    }
                    else
                    {
                        LogAndConsole($"Error copying file {sourceFileName} to replica folder.");
                        error = 1;
                    }
                }
                else
                {
                    if (CalculateMD5(sourceFile) != CalculateMD5(replicaFile))
                    {
                        File.Copy(sourceFile, replicaFile, true);
                        if (CalculateMD5(sourceFile) == CalculateMD5(replicaFile))
                        {
                            LogAndConsole($"File {sourceFileName} updated in replica folder.");
                        }
                        else
                        {
                            LogAndConsole($"Error updating file {sourceFileName} in replica folder.");
                            error = 1;
                        }
                    }
                }
            }

            foreach (string replicaFile in replicaFiles)
            {
                string replicaFileName = Path.GetFileName(replicaFile);
                string sourceFile = Path.Combine(sourcePath, replicaFileName);

                if (!File.Exists(sourceFile))
                {
                    File.Delete(replicaFile);
                    if (!File.Exists(replicaFile))
                    {
                        LogAndConsole($"File {replicaFileName} deleted from replica folder.");
                    }
                    else
                    {
                        LogAndConsole($"Error deleting file {replicaFileName} from replica folder.");
                        error = 1;
                    }
                }
            }
            if (error == 0)
            {
                LogAndConsole("Synchronization completed successfully!");
            }
            else
            {
                LogAndConsole("Synchronization completed with errors!");
            }
            return;
        }

        static string CalculateMD5(string filePath)
        {
            using var md5 = MD5.Create();
            using var fileStream = File.OpenRead(filePath);
            byte[] hash = md5.ComputeHash(fileStream);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        private static void LogAndConsole(string message)
        {
            string msg = $"{DateTime.Now}" + " - " + message;

            Console.WriteLine(msg);

            string logPath = Path.GetDirectoryName(FolderSync.logFilePath);

            if (!Directory.Exists(logPath))
            {
                Console.WriteLine("Logs folder doesn't exist. I will create it for you :)");
                Directory.CreateDirectory(logPath);

                if (!Directory.Exists(logPath))
                {
                    Console.WriteLine("Error creating log folder :(, please create it mannualy.");
                    return;
                }
                else
                {
                    Console.WriteLine("Replica folder created sucessfully!");
                }
            }

            using (StreamWriter writer = new StreamWriter(FolderSync.logFilePath, true))
            {
                writer.WriteLine(msg);
            }

        }
    }
}