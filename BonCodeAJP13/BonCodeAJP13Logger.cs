/*
 *  Copyright (c) 2011 by Bilal Soylu
 *  Bilal Soylu licenses this file to You under the 
 *  Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

/*************************************************************************
 * Description: IIS-to-Tomcat connector                                  *
 * Author:      Bilal Soylu <bilal.soylu[at]gmail.com>                   *
 * Version:     1.0                                                      *
 *************************************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;



namespace BonCodeAJP13
{
    /// <summary>
    /// Used to Log packets and events in the AJPv13 connection
    /// </summary>
    public class BonCodeAJP13Logger
    {
        private Mutex p_Mut;
        private string p_FileName;
        private string p_FileNamePattern;  // Original filename pattern for date rolling
        private static String p_filenameDateFormat = "yyyyMMdd";
        private static String p_timestampFormat = "yyyy-MM-dd HH:mm:ss ";

        /// <summary>
        /// Initialize the Logger with the File Name and Mutex to guard the access of this file.
        /// </summary>
        public BonCodeAJP13Logger(string fileName, Mutex loggerMutex)
        {
            // Validate mutex is not null
            if (loggerMutex == null)
                throw new ArgumentNullException("loggerMutex", "Mutex cannot be null");
            
            p_Mut = loggerMutex;
            p_FileNamePattern = fileName;  // Store original filename for date rolling
            
            if (p_FileNamePattern == null)
                throw new ArgumentNullException("File Name cannot be null");
            if (p_FileNamePattern == "")
                throw new ArgumentException("File Name cannot be empty");

            // Build dated log file path using Path.Combine for proper path handling
            p_FileName = GetDatedLogPath(p_FileNamePattern);

            // Acquire mutex before checking/creating file to prevent race condition
            try
            {
                p_Mut.WaitOne();
                if (!File.Exists(p_FileName))
                {   // log version to new file
                    using (StreamWriter logStream = File.AppendText(p_FileName))
                    {
                        logStream.WriteLine(DateTime.Now.ToString(p_timestampFormat) + "BonCode AJP Connector version " + BonCodeAJP13Consts.BONCODEAJP13_CONNECTOR_VERSION);
                        logStream.Flush();
                        logStream.Close();  // Explicit close for immediate resource release (using will also Dispose)
                    }
                }
            }
            catch (Exception ex)
            {
                RecordSysEvent("Error during logger initialization : " + ex.Message, EventLogEntryType.Error);
            }
            finally
            {
                try { p_Mut.ReleaseMutex(); } 
                catch (Exception ex) { RecordSysEvent("Error releasing mutex during initialization : " + ex.Message, EventLogEntryType.Warning); }
            }
        }

        /// <summary>
        /// Generates a dated log file path from a base filename pattern.
        /// Adds date suffix before the file extension for daily log rotation.
        /// </summary>
        private string GetDatedLogPath(string baseFileName)
        {
            string dateSuffix = DateTime.Now.ToString(p_filenameDateFormat);
            string dir = GetLogDir();
            string baseName = Path.GetFileNameWithoutExtension(baseFileName);
            string ext = Path.GetExtension(baseFileName);
            return Path.Combine(dir, baseName + "-" + dateSuffix + ext);
        }

        /// <summary>
        /// Gets the current dated log file path (recalculates date for midnight rollover support).
        /// </summary>
        private string GetCurrentLogPath()
        {
            return GetDatedLogPath(p_FileNamePattern);
        }

        /// <summary>
        /// Log an Exception message to the log file with optional string
        /// </summary>
        public void LogException(Exception e, string message = "", int minLogLevel = BonCodeAJP13LogLevels.BONCODEAJP13_LOG_ERRORS)
        {
            //allways log exceptions if logging is enabled.
            if (BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL > BonCodeAJP13LogLevels.BONCODEAJP13_NO_LOG )
            {
                try
                {
                    p_Mut.WaitOne();
                    // Use GetCurrentLogPath() for date rolling support (midnight rollover)
                    using (StreamWriter logStream = File.AppendText(GetCurrentLogPath()))
                    {

                        logStream.WriteLine(DateTime.Now.ToString(p_timestampFormat) + BonCodeAJP13Consts.BONCODEAJP13_CONNECTOR_VERSION + " ERROR ");

                        logStream.WriteLine(message);
                        logStream.WriteLine(e.Message);
                        logStream.WriteLine(e.StackTrace);
                        /*
                        if (BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL == BonCodeAJP13LogLevels.BONCODEAJP13_LOG_DEBUG)
                        {
                            logStream.WriteLine(e.StackTrace);
                        }
                        */
                        logStream.Flush();
                        logStream.Close();  // Explicit close for immediate resource release (maybe redundant since using will also Dispose)

                    }
                }
                catch (Exception fileException)
                {
                    RecordSysEvent("Error during log write : " + fileException.Message, EventLogEntryType.Error);
                }
                finally
                {
                    try { p_Mut.ReleaseMutex(); } 
                    catch (Exception ex) { RecordSysEvent("Error releasing mutex in LogException : " + ex.Message, EventLogEntryType.Warning); }
                }
            }
        }

        /// <summary>
        /// Log string message
        /// Either minLogLevel or BONCODEAJP13_LOG_LEVEL need to be met
        /// </summary>
        public void LogMessage(string message, int minLogLevel = BonCodeAJP13LogLevels.BONCODEAJP13_LOG_ERRORS)
        {
            if (BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL > BonCodeAJP13LogLevels.BONCODEAJP13_NO_LOG && BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL >= minLogLevel)
            {
                try {
                    p_Mut.WaitOne();
                    // Use GetCurrentLogPath() for date rolling support (midnight rollover)
                    using (StreamWriter logStream = File.AppendText(GetCurrentLogPath()))
                    {
                        logStream.WriteLine(DateTime.Now.ToString(p_timestampFormat) + message);
                        logStream.Flush();
                        logStream.Close();  // Explicit close for immediate resource release (using will also Dispose)
                    }
                }
                catch (Exception fileException)
                {
                    RecordSysEvent("Error during log write : " + fileException.Message, EventLogEntryType.Error);
                }
                finally
                {
                    try { p_Mut.ReleaseMutex(); } 
                    catch (Exception ex) { RecordSysEvent("Error releasing mutex in LogMessage : " + ex.Message, EventLogEntryType.Warning); }
                }

            }
        }

        /// <summary>
        /// Log message and type designation
        /// </summary>
        public void LogMessageAndType(string message, string messageType, int minLogLevel = BonCodeAJP13LogLevels.BONCODEAJP13_LOG_ERRORS)
        {
            if (BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL > BonCodeAJP13LogLevels.BONCODEAJP13_NO_LOG && BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL >= minLogLevel)
            {
                try {
                    p_Mut.WaitOne();
                    // Use GetCurrentLogPath() for date rolling support (midnight rollover)
                    using (StreamWriter logStream = File.AppendText(GetCurrentLogPath()))
                    {
                        logStream.WriteLine(DateTime.Now.ToString(p_timestampFormat) + messageType + " " + message);
                        logStream.Flush();
                        logStream.Close();  // Explicit close for immediate resource release (using will also Dispose)
                    }
                }
                catch (Exception fileException)
                {
                    RecordSysEvent("Error during log write : " + fileException.Message, EventLogEntryType.Error);
                }
                finally
                {
                    try { p_Mut.ReleaseMutex(); } 
                    catch (Exception ex) { RecordSysEvent("Error releasing mutex in LogMessageAndType : " + ex.Message, EventLogEntryType.Warning); }
                }
            }

        }


        /// <summary>
        /// Log one packet by calling its PrintPacket method. Only if BONCODEAJP13_LOG_LEVEL >= 1
        /// if logAllways is set packet will be logged regardless of log level.
        /// Packet logging only occurs if we have exception, Log Headers or Log Debug
        /// </summary>
        public void LogPacket(BonCodeAJP13Packet packet, bool logAllways = false, int minLogLevel = BonCodeAJP13LogLevels.BONCODEAJP13_LOG_ERRORS)
        {
            //only log packets if logging level allows (parentheses added for correct operator precedence)
            if ((BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL > BonCodeAJP13LogLevels.BONCODEAJP13_NO_LOG && BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL >= minLogLevel) || logAllways)
            {
                try {

                    p_Mut.WaitOne();
                    // Use GetCurrentLogPath() for date rolling support (midnight rollover)
                    using (StreamWriter logStream = File.AppendText(GetCurrentLogPath()))
                    {

                        //log packet headers only
                        if (BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL == BonCodeAJP13LogLevels.BONCODEAJP13_LOG_HEADERS)
                        {
                            logStream.WriteLine(DateTime.Now.ToString(p_timestampFormat) + packet.ToString() + " " + packet.PrintPacketHeader());

                            logStream.Flush();
                            logStream.Close();  // Explicit close for immediate resource release (using will also Dispose)
                        };

                        //logs full packets. Log files may grow big in this case
                        if (BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL == BonCodeAJP13LogLevels.BONCODEAJP13_LOG_DEBUG)
                        {
                            logStream.WriteLine(DateTime.Now.ToString(p_timestampFormat) + packet.ToString() + " " + packet.PrintPacketHeader());
                            logStream.WriteLine(packet.PrintPacket());
                            logStream.WriteLine("");

                            logStream.Flush();
                            logStream.Close();  // Explicit close for immediate resource release (using will also Dispose)
                        };

                    }

                }
                catch (Exception fileException)
                {
                    RecordSysEvent("Error during log write : " + fileException.Message, EventLogEntryType.Error);
                }
                finally
                {
                    try { p_Mut.ReleaseMutex(); } 
                    catch (Exception ex) { RecordSysEvent("Error releasing mutex in LogPacket : " + ex.Message, EventLogEntryType.Warning); }
                }
            }

        }

        /// <summary>
        /// Debug log writing method using synchronized stream writer class. This logs to seperate file. Does not use mutex.
        /// </summary>        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public static void LogDebug(String message, String filename = "debug-")
        {
            try
            {
                // Use Path.Combine for proper path handling
                string datedFilename = Path.Combine(GetLogDir(), filename + DateTime.Now.ToString(p_filenameDateFormat) + ".log");

                using (StreamWriter logStream = File.AppendText(datedFilename))
                {
                    
                    logStream.WriteLine(String.Format("{0}[T-{1}] {2}", DateTime.Now.ToString(p_timestampFormat), Thread.CurrentThread.ManagedThreadId, message));
                    logStream.Flush();
                    logStream.Close();  // Explicit close for immediate resource release (using will also Dispose)
                }
            }
            catch (Exception ex) {
                RecordSysEvent("Error during log write : " + ex.Message, EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Log a collection of packets by calling their PrintPacket methods
        /// Requires BONCODEAJP13_LOG_LEVEL >= 2
        /// </summary>
        public void LogPackets(BonCodeAJP13PacketCollection packets, bool logAllways = false, int minLogLevel = BonCodeAJP13LogLevels.BONCODEAJP13_LOG_ERRORS)
        {
            foreach (BonCodeAJP13Packet packet in packets)
            {
                this.LogPacket(packet,logAllways,minLogLevel);
            }
        }

        /// <summary>
        /// Return directory in which the current code resides. If the assembly is in Global Assembly Cache (GAC) we will return windows/system32 dir
        /// No terminating slash will be returned.
        /// </summary>
        public static string GetAssemblyDirectory()
        {
            string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);           
            string strPath = System.IO.Path.GetDirectoryName(uri.Uri.LocalPath);

            //check whether path is accessible (it is not when in GAC)            
            if (strPath.Contains("GAC_MSIL\\BonCodeAJP13\\")) strPath = Environment.GetEnvironmentVariable("windir"); //Environment.GetFolderPath(Environment.SpecialFolder.System);
            //if (!Directory.Exists(strPath)) strPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
            return strPath;

        }

        /// <summary>
        /// Return directory in which the log file will be placed.
        /// No terminating slash will be returned.
        /// </summary>
        public static string GetLogDir()
        {
            string retValue = GetAssemblyDirectory();
            if (BonCodeAJP13Settings.BONCODEAJP13_LOG_DIR.Length >= 3)
            {
                retValue = BonCodeAJP13Settings.BONCODEAJP13_LOG_DIR;
            }

            return retValue;
        }

        /// <summary>
        /// Record an event in system in Application event log  
        /// A similar function is CallHandler class
        /// </summary>
        private static void RecordSysEvent(string message, EventLogEntryType eType = EventLogEntryType.Information)
        {
            string sSource = "BonCodeConnector";

            try
            {
                //we only record events when event source exists
                if (EventLog.SourceExists(sSource))
                {
                    EventLog.WriteEntry(sSource, message, eType, 418);
                }
            }
            catch
            {
                //EventLog not accessible (e.g. Security/State logs inaccessible under IIS)
            }
        }


    }
}
