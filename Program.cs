using System;
namespace FolderSynchronizerProgram
{
    // Entry point
    public class SynchronizerMain
    {
        static void Main(string[] args)
        {
            // Check if the command has the right number of arguments and if the time is a number
            // If the command is correct, start the synchronization process
            if (args.Length != 4)
            {
                Console.WriteLine("The command must have 3 arguments, Source path, Replica path, time of synchronization in seconds, and log file path eg. FolderSync.exe c:\\SourceFolder c:\\ReplicaFolder 5 c:\\LogSync\\log.ext");
                return;
            }
            else if (!int.TryParse(args[2], out _))
            {
                Console.WriteLine("The time must be a number!");
                return;
            }
            else
            {
                // Get the source path, replica path, time of synchronization and log file path from the command
                string sourcePath = args[0];
                string replicaPath = args[1];
                int syncInterval = int.Parse(args[2]);
                Logger log = new Logger(args[3]);

                SynchronizeCallBack(sourcePath, replicaPath);

                // Create a timer to synchronize the folders every x seconds
                System.Timers.Timer executeTimer = new System.Timers.Timer();
                executeTimer.Interval = TimeSpan.FromSeconds(syncInterval).TotalMilliseconds;
                executeTimer.Elapsed += (sender, e) => SynchronizeCallBack(sourcePath, replicaPath);
                executeTimer.Start();

                // Wait for the user to press enter to exit
                Console.WriteLine("Press Enter at any time to exit.");
                Console.ReadLine();
            }
        }

        // Call the Synchronize method from the FolderSynchronizer class
        private static void SynchronizeCallBack(string sourcePath, string replicaPath)
        {
            // Verify if the source and replica folders exist
            if (VerifyFolderExistence(sourcePath, replicaPath))
            {
                FolderSynchronizer fileSynchronizer = new FolderSynchronizer();
                fileSynchronizer.Synchronize(sourcePath, replicaPath);
            }
        }

        // Method to verify if the source and replica folders exist
        private static bool VerifyFolderExistence(string sourcePath, string replicaPath)
        {
            Logger log = new Logger();

            if (!Directory.Exists(sourcePath))
            {
                log.LogAndConsole("Error. Source folder doesn't exist.");
                return false;
            }

            if (!Directory.Exists(replicaPath))
            {
                log.LogAndConsole("Replica folder doesn't exist. I will create it for you.");
                Directory.CreateDirectory(replicaPath);
                if (!Directory.Exists(replicaPath))
                {
                    log.LogAndConsole("Error creating replica folder, please create it manually.");
                    return false;
                }
                else
                {
                    log.LogAndConsole("Replica folder created sucessfully!");
                    return true;
                }
            }
            return true;
        }
    }
}