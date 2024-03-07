using System;
using System.Security.Cryptography;

namespace FolderSynchronizerProgram
{
    // Class to synchronize the source folder with the replica folder
    public class FolderSynchronizer()
    {
        // Method to synchronize the source folder with the replica folder
        public void Synchronize(string sourcePath, string replicaPath)
        {
            int error = 0;
            Logger log = new Logger();

            // Call the method to synchronize the directories and files
            SynchronizeDirectories(error, sourcePath, replicaPath);
            SynchronizeFiles(error, sourcePath, replicaPath);

            // Log the result of the synchronization
            if (error == 0)
            {
                log.LogAndConsole("Synchronization completed successfully!");
            }
            else
            {
                log.LogAndConsole("Synchronization completed with errors!");
            }
            return;
        }

        // Method to synchronize the files
        private void SynchronizeFiles(int error, string sourcePath, string replicaPath)
        {
            // Get the files from the source and replica folders
            string[] sourceFiles = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
            string[] replicaFiles = Directory.GetFiles(replicaPath, "*", SearchOption.AllDirectories);

            // Call the methods to copy or update the files and delete the files
            CopyUpdateFile(error, sourceFiles, sourcePath, replicaPath);
            DeleteFile(error, replicaFiles, sourcePath, replicaPath);
            return;
        }

        // Method to copy or update the files
        private void CopyUpdateFile(int error, string[] sourceFiles, string sourcePath, string replicaPath)
        {
            Logger log = new Logger();

            // Loop through the files in the source folder
            foreach (string sourceFilePath in sourceFiles)
            {
                // Get the file name and the replica file path
                string sourceFileName = Path.GetFileName(sourceFilePath);
                string replicaFilePath = Path.Combine(replicaPath, sourceFilePath.Remove(0, sourcePath.Length + 1));

                // If the file doesn't exist in the replica folder, copy it
                if (!File.Exists(replicaFilePath))
                {
                    try
                    {
                        File.Copy(sourceFilePath, replicaFilePath);
                    }
                    catch (IOException ex)
                    {
                        log.LogAndConsole($"{ex.Message}");
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        log.LogAndConsole($"{ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        log.LogAndConsole($"{ex.Message}");
                    }

                    if (CalculateMD5(sourceFilePath) == CalculateMD5(replicaFilePath))
                    {
                        log.LogAndConsole($"File {sourceFileName} copied to replica folder.");
                    }
                    else
                    {
                        log.LogAndConsole($"Error copying file {sourceFileName} to replica folder.");
                        error = 1;
                    }
                }
                else // If the file exists in the replica folder, update it
                {
                    if (CalculateMD5(sourceFilePath) != CalculateMD5(replicaFilePath))
                    {
                        try
                        {
                            File.Copy(sourceFilePath, replicaFilePath, true);
                        }
                        catch (IOException ex)
                        {
                            log.LogAndConsole($"{ex.Message}");
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            log.LogAndConsole($"{ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            log.LogAndConsole($"{ex.Message}");
                        }

                        if (CalculateMD5(sourceFilePath) == CalculateMD5(replicaFilePath))
                        {
                            log.LogAndConsole($"File {sourceFileName} updated in replica folder.");
                        }
                        else
                        {
                            log.LogAndConsole($"Error updating file {sourceFileName} in replica folder.");
                            error = 1;
                        }
                    }
                    else
                    {
                        log.LogAndConsole($"File {sourceFileName} already exists in replica folder and is up to date.");
                    }
                }
            }
            return;
        }

        // Method to delete the files
        private void DeleteFile(int error, string[] replicaFiles, string sourcePath, string replicaPath)
        {
            Logger log = new Logger();

            // Loop through the files in the replica folder
            foreach (string replicaFilePath in replicaFiles)
            {
                // Get the file name and the source file path
                string replicaFileName = Path.GetFileName(replicaFilePath);
                string sourceFilePath = Path.Combine(sourcePath, replicaFilePath.Remove(0, replicaPath.Length + 1));

                // If the file doesn't exist in the source folder, delete it
                if (!File.Exists(sourceFilePath))
                {
                    try
                    {
                        File.Delete(replicaFilePath);
                    }
                    catch (IOException ex)
                    {
                        log.LogAndConsole($"{ex.Message}");
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        log.LogAndConsole($"{ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        log.LogAndConsole($"{ex.Message}");
                    }

                    if (!File.Exists(replicaFilePath))
                    {
                        log.LogAndConsole($"File {replicaFileName} deleted from replica folder.");
                    }
                    else
                    {
                        log.LogAndConsole($"Error deleting file {replicaFileName} from replica folder.");
                        error = 1;
                    }
                }
            }
            return;
        }

        // Method to synchronize the directories
        private void SynchronizeDirectories(int error, string sourcePath, string replicaPath)
        {
            // Get the directories from the source and replica folders
            string[] sourceDirectories = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);
            string[] replicaDirectories = Directory.GetDirectories(replicaPath, "*", SearchOption.AllDirectories);

            // Call the methods to create or delete the directories
            CreateDirectory(error, sourceDirectories, sourcePath, replicaPath);
            DeleteDirectory(error, replicaDirectories, sourcePath, replicaPath);
            return;
        }

        // Method to create the directories
        private void CreateDirectory(int error, string[] sourceDirectories, string sourcePath, string replicaPath)
        {
            Logger log = new Logger();

            // Loop through the directories in the source folder
            foreach (string sourceDirectoriePath in sourceDirectories)
            {
                // Get the relative source path sub directory and the replica path sub directory
                string relativeSourcePathSubDir = sourceDirectoriePath.Substring(sourcePath.Length);
                relativeSourcePathSubDir = relativeSourcePathSubDir.TrimStart('\\');
                string replicaPathSubDir = Path.Combine(replicaPath, relativeSourcePathSubDir);

                // If the directory doesn't exist in the replica folder, create it
                if (!Directory.Exists(replicaPathSubDir))
                {
                    try
                    {
                        Directory.CreateDirectory(replicaPathSubDir);
                    }
                    catch (IOException ex)
                    {
                        log.LogAndConsole($"{ex.Message}");
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        log.LogAndConsole($"{ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        log.LogAndConsole($"{ex.Message}");
                    }

                    if (Directory.Exists(replicaPathSubDir))
                    {
                        log.LogAndConsole($"Directory {relativeSourcePathSubDir} created in replica folder.");
                        SynchronizeDirectories(error, sourcePath, replicaPath);
                        return;
                    }
                    else
                    {
                        log.LogAndConsole($"Error creating directory {relativeSourcePathSubDir} in replica folder.");
                        error = 1;
                    }
                }
            }
            return;
        }

        // Method to delete the directories
        private void DeleteDirectory(int error, string[] replicaDirectories, string sourcePath, string replicaPath)
        {
            Logger log = new Logger();

            // Loop through the directories in the replica folder
            foreach (string replicaDirectoriePath in replicaDirectories)
            {
                // Get the relative replica path sub directory and the source path sub directory
                string relativeReplicaPathSubDir = replicaDirectoriePath.Substring(replicaPath.Length);
                relativeReplicaPathSubDir = relativeReplicaPathSubDir.TrimStart('\\');
                string sourcePathSubDir = Path.Combine(sourcePath, relativeReplicaPathSubDir);

                // If the directory doesn't exist in the source folder, delete it
                if (!Directory.Exists(sourcePathSubDir))
                {
                    try
                    {
                        Directory.Delete(replicaDirectoriePath, true);
                    }
                    catch (IOException ex)
                    {
                        log.LogAndConsole($"{ex.Message}");
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        log.LogAndConsole($"{ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        log.LogAndConsole($"{ex.Message}");
                    }

                    if (!Directory.Exists(replicaDirectoriePath))
                    {
                        log.LogAndConsole($"Directory {relativeReplicaPathSubDir} deleted from replica folder.");
                        SynchronizeDirectories(error, sourcePath, replicaPath);
                        return;
                    }
                    else
                    {
                        log.LogAndConsole($"Error deleting directory {relativeReplicaPathSubDir} from replica folder.");
                        error = 1;
                    }
                }
            }
            return;
        }

        // Method to calculate the MD5 hash of a file
        private string CalculateMD5(string filePath)
        {
            Logger log = new Logger();

            try
            {
                using MD5 md5 = MD5.Create();
                using FileStream fileStream = File.OpenRead(filePath);
                byte[] hash = md5.ComputeHash(fileStream);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
            catch (IOException ex)
            {
                log.LogAndConsole($"Error calculating MD5 for file {filePath}. The file is in use by another process: {ex.Message}");
                Random random = new Random();
                return random.Next().ToString();
            }
            catch (UnauthorizedAccessException ex)
            {
                log.LogAndConsole($"Error calculating MD5 for file {filePath}. Access denied: {ex.Message}");
                Random random = new Random();
                return random.Next().ToString();
            }
            catch (Exception ex)
            {
                log.LogAndConsole($"Error calculating MD5 for file {filePath}: {ex.Message}");
                Random random = new Random();
                return random.Next().ToString();
            }
        }
    }
}