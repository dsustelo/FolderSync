using System;
namespace FolderSynchronizerProgram
{
    // Class to log messages to a file and to the console
    public class Logger
    {
        private static string _logFilePath;

        public Logger()
        {
            return;
        }
        public Logger(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        // Method to log messages to a file and to the console
        public void LogAndConsole(string message)
        {
            // Create a string with the current date and time and the message
            string messageAux = $"{DateTime.Now}" + " - " + message;

            Console.WriteLine(messageAux);

            string logFolderPath = Path.GetDirectoryName(_logFilePath);

            // Check if the log folder exists, if not, create it
            if (!Directory.Exists(logFolderPath))
            {
                Console.WriteLine("Logs folder doesn't exist. I will create it for you.");
                Directory.CreateDirectory(logFolderPath);

                if (!Directory.Exists(logFolderPath))
                {
                    Console.WriteLine("Error creating log folder, please create it mannualy.");
                    return;
                }
                else
                {
                    Console.WriteLine("Log folder created sucessfully!");
                }
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(_logFilePath, true))
                {
                    writer.WriteLine(messageAux);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error writing to log file. The file is in use by another process: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }
    }
}