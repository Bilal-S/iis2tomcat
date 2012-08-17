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
        }

        /// <summary>
        /// Log an Exception message to the log file with optional string
        /// </summary>
        public void LogException(Exception e, string message = "", int onlyAboveLogLevel = BonCodeAJP13LogLevels.BONCODEAJP13_NO_LOG)
        {
            //allways log exceptions if logging is enabled.
            if (BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL > BonCodeAJP13LogLevels.BONCODEAJP13_NO_LOG )
            {
                p_Mut.WaitOne();
                using (StreamWriter logStream = File.AppendText(p_FileName))
                {

                    logStream.WriteLine("------------------------------------------------------------------------");
                    logStream.WriteLine("Error at: " + DateTime.Now.ToShortDateString() + "  " + DateTime.Now.ToLongTimeString() + "  " + BonCodeAJP13Consts.BONCODEAJP13_CONNECTOR_VERSION);
                    logStream.WriteLine(message);
                    logStream.WriteLine(e.Message);
                    if (BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL == BonCodeAJP13LogLevels.BONCODEAJP13_LOG_DEBUG)
                    {
                        logStream.WriteLine(e.StackTrace);
                    }
                    logStream.Flush();
                    logStream.Close();

                }
                p_Mut.ReleaseMutex();
            }
        }

        /// <summary>
        /// Log string message
        /// Either onlyAboveLogLevel or BONCODEAJP13_LOG_LEVEL need to be met
        /// </summary>
        public void LogMessage(string message, int onlyAboveLogLevel = BonCodeAJP13LogLevels.BONCODEAJP13_NO_LOG)
        {
            if (BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL > BonCodeAJP13LogLevels.BONCODEAJP13_NO_LOG && BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL > onlyAboveLogLevel)
            {
                p_Mut.WaitOne();
                using (StreamWriter logStream = File.AppendText(p_FileName))
                {
                    logStream.WriteLine("------------------------------------------------------------------------");
                    logStream.WriteLine(message + "     at: " + DateTime.Now.ToShortDateString() + "  " + DateTime.Now.ToLongTimeString());
                    logStream.Flush();
                    logStream.Close();
                }
                p_Mut.ReleaseMutex();
            }
        }

        /// <summary>
        /// Log message and type designation
        /// </summary>
        public void LogMessageAndType(string message, string messageType, int onlyAboveLogLevel = BonCodeAJP13LogLevels.BONCODEAJP13_NO_LOG)
        {
            if (BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL > BonCodeAJP13LogLevels.BONCODEAJP13_NO_LOG && BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL > onlyAboveLogLevel)
            {
                p_Mut.WaitOne();
                using (StreamWriter logStream = File.AppendText(p_FileName))
                {
                    //logStream.WriteLine(" ");
                    logStream.WriteLine("------------------------------------------------------------------------");                    
                    logStream.WriteLine(messageType + " at:  " + DateTime.Now.ToShortDateString() + "  " + DateTime.Now.ToLongTimeString());
                    logStream.WriteLine(message);
                    logStream.Flush();
                    logStream.Close();
                }
                p_Mut.ReleaseMutex();
            }

        }


        /// <summary>
        /// Log one packet by calling its PrintPacket method. Only if BONCODEAJP13_LOG_LEVEL >= 1
        /// if logAllways is set packet will be logged regardless of log level
        /// </summary>
        public void LogPacket(BonCodeAJP13Packet packet, bool logAllways = false, int onlyAboveLogLevel = BonCodeAJP13LogLevels.BONCODEAJP13_NO_LOG)
        {
            //only log packets if logging level allows
            if (BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL > BonCodeAJP13LogLevels.BONCODEAJP13_NO_LOG && BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL > onlyAboveLogLevel || logAllways)
            {
                p_Mut.WaitOne();
                using (StreamWriter logStream = File.AppendText(p_FileName))
                {
                    
                    if (BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL == BonCodeAJP13LogLevels.BONCODEAJP13_LOG_HEADERS)
                    {
                        logStream.WriteLine("-- Packet Info:" + packet.ToString() + " at: " + DateTime.Now.ToShortDateString() + "    " + DateTime.Now.ToLongTimeString());
                        logStream.WriteLine(packet.PrintPacketHeader());
                        logStream.WriteLine("");
                        logStream.Flush();
                        logStream.Close();
                    };
                    //logs full packets. Log files may grow big in this case
                    if (BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL == BonCodeAJP13LogLevels.BONCODEAJP13_LOG_DEBUG)
                    {
                        logStream.WriteLine("-- Packet Info:" + packet.ToString() + " at: " + DateTime.Now.ToShortDateString() + "    " + DateTime.Now.ToLongTimeString());
                        logStream.WriteLine(packet.PrintPacket());
                        logStream.WriteLine("");
                        logStream.Flush();
                        logStream.Close();
                    };                    

                }
                p_Mut.ReleaseMutex();
            }

        }

        /// <summary>
        /// Log a collection of packets by calling their PrintPacket methods
        /// Requires BONCODEAJP13_LOG_LEVEL >= 2
        /// </summary>
        public void LogPackets(BonCodeAJP13PacketCollection packets, bool logAllways = false, int onlyAboveLogLevel = BonCodeAJP13LogLevels.BONCODEAJP13_NO_LOG)
        {
            foreach (BonCodeAJP13Packet packet in packets)
            {
                this.LogPacket(packet,logAllways,onlyAboveLogLevel);
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
            string translatedPath = Uri.UnescapeDataString(uri.Path);
            string strPath = System.IO.Path.GetDirectoryName(translatedPath);
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


    }
}
