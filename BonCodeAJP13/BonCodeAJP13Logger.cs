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
        private static String p_filenameDateFormat = "yyyyMMdd";
        private static String p_timestampFormat = "yyyy-MM-dd HH:mm:ss ";

        /// <summary>
        /// Initialize the Logger with the File Name and Mutex to guard the access of this file.
        /// </summary>
        public BonCodeAJP13Logger(string fileName, Mutex loggerMutex)
        {
            p_Mut = loggerMutex;
            p_FileName = fileName;
            if (p_FileName == null)
                throw new ArgumentNullException("File Name cannot be null");
            if (p_FileName == "")
                throw new ArgumentException("File Name cannot be empty");

            //finalize file translatedPath to be in the same directory as dll
            p_FileName = GetLogDir() + "\\" + p_FileName;

            if (!File.Exists(p_FileName))
            {   // log version to new file
                using (StreamWriter logStream = File.AppendText(p_FileName))
                {
                    logStream.WriteLine(DateTime.Now.ToString(p_timestampFormat) + "BonCode AJP Connenctor version " + BonCodeAJP13Consts.BONCODEAJP13_CONNECTOR_VERSION);
                }
            }
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
                    using (StreamWriter logStream = File.AppendText(p_FileName))
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
                        logStream.Close();

                    }
                    p_Mut.ReleaseMutex();
                }
                catch (Exception fileException)
                {
                    RecordSysEvent("Error during log write : " + fileException.Message, EventLogEntryType.Error);
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
                    using (StreamWriter logStream = File.AppendText(p_FileName))
                    {                        
                        logStream.WriteLine(DateTime.Now.ToString(p_timestampFormat) + message);
                        logStream.Flush();
                        logStream.Close();
                    }
                    p_Mut.ReleaseMutex();
                }   
                catch (Exception fileException)                
                {
                    RecordSysEvent("Error during log write : " + fileException.Message, EventLogEntryType.Error);
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
                    using (StreamWriter logStream = File.AppendText(p_FileName))
                    {                       
                        logStream.WriteLine(DateTime.Now.ToString(p_timestampFormat) + messageType + " " + message);
                        logStream.Flush();
                        logStream.Close();
                    }
                    p_Mut.ReleaseMutex();
                }
                catch (Exception fileException)
                {
                    RecordSysEvent("Error during log write : " + fileException.Message, EventLogEntryType.Error);
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
            //only log packets if logging level allows
            if (BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL > BonCodeAJP13LogLevels.BONCODEAJP13_NO_LOG && BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL >= minLogLevel || logAllways)
            {
                try {

                    p_Mut.WaitOne();
                    using (StreamWriter logStream = File.AppendText(p_FileName))
                    {
                    
                        //log packet headers only
                        if (BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL == BonCodeAJP13LogLevels.BONCODEAJP13_LOG_HEADERS)
                        {                            
                            logStream.WriteLine(DateTime.Now.ToString(p_timestampFormat) + packet.ToString() + " " + packet.PrintPacketHeader());
                           
                            logStream.Flush();
                            logStream.Close();
                        };

                        //logs full packets. Log files may grow big in this case
                        if (BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL == BonCodeAJP13LogLevels.BONCODEAJP13_LOG_DEBUG)
                        {                            
                            logStream.WriteLine(DateTime.Now.ToString(p_timestampFormat) + packet.ToString() + " " + packet.PrintPacketHeader());
                            logStream.WriteLine(packet.PrintPacket());
                            logStream.WriteLine("");
                         
                            logStream.Flush();
                            logStream.Close();
                        };                    

                    }
                    p_Mut.ReleaseMutex();

                }
                catch (Exception fileException)
                {
                    RecordSysEvent("Error during log write : " + fileException.Message, EventLogEntryType.Error);
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
                filename = GetLogDir() + "\\" + filename + DateTime.Now.ToString(p_filenameDateFormat) + ".log";

                using (StreamWriter logStream = File.AppendText(filename))
                {
                    
                    logStream.WriteLine(String.Format("{0}[T-{1}] {2}", DateTime.Now.ToString(p_timestampFormat), Thread.CurrentThread.ManagedThreadId, message));
                    logStream.Flush();
                    logStream.Close();
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
            if (strPath.Contains("GAC_MSIL\\BonCodeAJP13\\1.0.0.0")) strPath = Environment.GetEnvironmentVariable("windir"); //Environment.GetFolderPath(Environment.SpecialFolder.System);
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
            string sSource;
            string sEvent;
            sSource = "BonCodeConnector";
            sEvent = message;

            //we only record events when event source exists
            if (EventLog.SourceExists(sSource))
            {
                //record in event log
                try
                {
                    EventLog.WriteEntry(sSource, sEvent, eType, 418);
                }
                catch
                {
                    //do nothing for now
                }

            }
        }


    }
}
