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

//==============================================================================
// This is the primary communication class
//==============================================================================
// We will make connection and analyze returned packets
//==============================================================================


using System;
using System.Web;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using BonCodeAJP13.TomcatPackets;
using BonCodeAJP13.ServerPackets;
using System.Text.RegularExpressions;





namespace BonCodeAJP13
{




    #region Delegates

    /// <summary>
    /// Delegate of function that will print content of string. Actual implementation will be passed in
    /// </summary>
    public delegate void FlushPacketsDelegate(string FlushContent);
    /// <summary>
    /// Delegate of function that will iterate through collection of packets and process for browser. Actual implementation will be passed in.
    /// </summary>
    public delegate void FlushPacketsCollectionDelegate(BonCodeAJP13PacketCollection collection);
    /// <summary>
    /// Delegate function indicating whether data is being pushed to browser
    /// </summary>
    public delegate bool FlushStatusDelegate();
    /// <summary>
    /// Delegate function indicating server path mapping
    /// </summary>
    public delegate string ServerPathDelegate(String virtualPath);


    #endregion

    /// <summary>
    /// BonCodeAJP13Connection implements the functionally in the server to 
    /// manage one connection to Tomcat, its thread will be spawned from the CallHandler
    /// </summary>
    public class BonCodeAJP13ServerConnection
    {



        #region instance data

        //server and port defaults
        private static string p_Server = BonCodeAJP13Settings.BONCODEAJP13_SERVER;
        private static int p_Port = BonCodeAJP13Settings.BONCODEAJP13_PORT;


        // Synchronizers ...
        private static Mutex p_ConnectionMutex = new Mutex();        
        private System.Timers.Timer p_KeepAliveTimer;
        private volatile bool p_AbortConnection;  //indicate whether this connection is toast and needs to be aborted

        // TCP client only we do not use listeners as we read from the stream immediatly using the client        
        private TcpClient p_TCPClient = null;

        // Net Stream. This is the stream we use in the connection with TcpClient
        private NetworkStream p_NetworkStream=null;

        // Logger is shared. The log file will be located in directory where dll is deployed. Normally inetpub\wwwroot\BIN
        private static BonCodeAJP13Logger p_Logger = null;  

        // Flags 
        private static int p_ConcurrentConnections = 0;         //concurrent connection counter 
        private static long p_ConnectionsCounter = 0;    //life time connections will assign p_ThisConnectionID from this


        //instance data
        private BonCodeAJP13PacketCollection p_PacketsToSend = new BonCodeAJP13PacketCollection();
        private BonCodeAJP13PacketCollection p_PacketsReceived = new BonCodeAJP13PacketCollection();
        private bool p_IsLastPacket = false;
        private long p_TickDelta = 0;
        private long p_LastTick = 0;
        private bool p_IsFlush = false;
        private long p_ThisConnectionID = -1;
        private bool p_SendTermPacket = false; //indicate whether we need to send extra terminator package
        private bool p_IsChunked = false;
        private int p_BytesInBuffer = 0; //counter for bytes to see whether we need to flush data; this is only approximated actual bytes tend to be a few more
        private Stopwatch p_StopWatch = new Stopwatch();
        private string p_LogFilePostFix = "";  //will be appended to log file between user chosen name and date to ensure that we have unique log file names
        private string p_ClientIp = ""; //holds the end-user Ip address

        //this is a function place holder
        private FlushPacketsCollectionDelegate p_FpDelegate;
        
        //this is function place holder
        private FlushStatusDelegate p_FlushInProgress;

        //this is function place holder
        private ServerPathDelegate p_ServerPath;

        #endregion

        #region Properties

        /// <summary>
        /// Aborts the connection with tomcat if we set it to true, this can be accessed from other threads.
        /// </summary>
        public bool AbortConnection
        {
            get { return p_AbortConnection; }
            set { p_AbortConnection = value; }
        }

        /// <summary>
        /// Sets or gets the status of the HTTP Transfer Encoding
        /// </summary>
        public bool ChunkedTransfer
        {
            get { return p_IsChunked; }
            set { p_IsChunked = value; }
        }


        /// <summary>
        /// Sets the TcpClient to use. Will conserve threads
        /// </summary>
        public TcpClient SetTcpClient {
            set { p_TCPClient = value; }
        }

        /// <summary>
        /// Sets the function that will print strings in packets when flush is detected
        /// The delegated function needs to accept one argument of type BonCodeAJP13PacketCollection
        /// </summary>
        public FlushPacketsCollectionDelegate FlushDelegateFunction
        {
            set { this.p_FpDelegate = value; }
        }

        /// <summary>
        /// Sets the function that will indicate whether flushing to browser is already in progress.
        /// </summary>
        public FlushStatusDelegate FlushStatusFunction
        {
            set { this.p_FlushInProgress = value; }
        }

        /// <summary>
        /// Sets the function that will indicate true server path given a relative URi.
        /// </summary>
        public  ServerPathDelegate ServerPathFunction
        {
            set { this.p_ServerPath = value; }
        }


        /// <summary>
        /// Get/Sets the Server
        /// </summary>
        public string Server
        {
            get { return p_Server; }
            set { p_Server = value; }
        }

        /// <summary>
        /// Get/Sets the Port
        /// </summary>
        public int Port
        {
            get { return p_Port; }
            set { p_Port = value; }
        }

        /// <summary>
        /// gets collection of packets received so far
        /// </summary>
        public BonCodeAJP13PacketCollection ReceivedDataCollection
        {
            get { return p_PacketsReceived; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Generic constuctor.
        /// Only initializes intance
        /// DEPRECATED DO NOT USE
        /// </summary>
        public BonCodeAJP13ServerConnection()
        {
            CheckMutex();
        }

        /// <summary>
        /// Constructor with a post fix for log file names
        /// Initializes intance
        /// ONLY USE THIS
        /// </summary>
        public BonCodeAJP13ServerConnection(string logFilePostFix="", string clientIp="")
        {
            p_LogFilePostFix = logFilePostFix;
            p_ClientIp = clientIp;
            Interlocked.Increment(ref p_ConcurrentConnections);
            CheckMutex();
        }

        /// <summary>
        /// Initialize new connection from server to tomcat in new thread.
        /// Delayed connection init. Will wait until connection is initialized
        /// DEPRECATED DO NOT USE
        /// </summary>
        public BonCodeAJP13ServerConnection(BonCodeAJP13Packet singlePacket, bool delayConnection = true)
        {
            CheckMutex();
            //package single package into packets to Send
            BonCodeAJP13PacketCollection packetsToSend = new BonCodeAJP13PacketCollection();
            packetsToSend.Add(singlePacket);
            p_PacketsToSend = packetsToSend; //assign to instance store
            //call connection creation if desired
            if (!delayConnection)
            {
                p_CreateConnection(packetsToSend);
            }

        }
        /// <summary>
        /// Initialize new connection from server to tomcat in new thread.
        /// this connection will run in new thread spawned from the listener thread.
        /// DEPRECATED DO NOT USE
        /// </summary>
        public BonCodeAJP13ServerConnection(BonCodeAJP13Packet singlePacket)
        {
            CheckMutex();
            //package single package into packets to Send
            BonCodeAJP13PacketCollection packetsToSend = new BonCodeAJP13PacketCollection();
            packetsToSend.Add(singlePacket);
            //call connection creation
            p_CreateConnection(packetsToSend);

        }

        /// <summary>
        /// Initialize new connection to tomcat using server and port input
        /// DEPRECATED DO NOT USE
        /// </summary>
        public BonCodeAJP13ServerConnection(string server, int port, BonCodeAJP13Packet singlePacket)
        {
            CheckMutex();
            this.Port = port;
            this.Server = server;
            //create collection and add packet passed
            BonCodeAJP13PacketCollection packetsToSend = new BonCodeAJP13PacketCollection();
            packetsToSend.Add(singlePacket);
            //call connection creation
            p_CreateConnection(packetsToSend);
        }


        /// <summary>
        /// Initialize new connection from server to tomcat in new thread.
        /// We use a packet collection (in case of Form posts) 
        /// DEPRECATED DO NOT USE
        /// </summary>
        public BonCodeAJP13ServerConnection(BonCodeAJP13PacketCollection packetsToSend)
        {
            CheckMutex();
            //call connection creation
            p_CreateConnection(packetsToSend);
        }


        /// <summary>
        /// Initialize new connection to tomcat using server and port input and packet collection
        /// DEPRECATED DO NOT USE
        /// </summary>
        public BonCodeAJP13ServerConnection(string server, int port, BonCodeAJP13PacketCollection packetsToSend)
        {
            CheckMutex();
            this.Port = port;
            this.Server = server;
            //call connection creation
            p_CreateConnection(packetsToSend);
        }


        #endregion

        #region destructor

        /// <summary>
        /// During destruction of class reduce the connection count
        /// Only initializes intance        
        /// </summary>
        ~ BonCodeAJP13ServerConnection()
        {
            Interlocked.Decrement(ref p_ConcurrentConnections);
            p_ConnectionsCounter--;

            p_PacketsReceived.Clear();
            p_PacketsReceived = null;
            p_PacketsToSend.Clear();
            p_PacketsToSend = null;            
            
        }

        #endregion


        #region Methods

        /// <summary>
        /// prep logging facilities
        /// </summary>
        private void CheckMutex()
        {
            if (BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL > BonCodeAJP13LogLevels.BONCODEAJP13_NO_LOG)
            {
                bool initLogger = true;
                string errMsg = "";
                //if we have a log filter we need to satisfy it first, in that case only log when we have a match pattern
                if (BonCodeAJP13Settings.BONCODEAJP13_LOG_IPFILTER.Length > 0 && p_ClientIp.Length > 0)
                {
                    try
                    {
                        //test whether part or parts of clientIp match the filter condition. If we have no match, we need to skip logging.
                        MatchCollection matches = Regex.Matches(p_ClientIp, BonCodeAJP13Settings.BONCODEAJP13_LOG_IPFILTER, RegexOptions.IgnorePatternWhitespace);
                        if (matches.Count == 0)
                        {
                            initLogger = false;
                        }
                    }
                    catch (Exception e)
                    {
                        //for any exception we will initialize logging anyway because of the initLogger flag
                        errMsg = "Your regular expression provided in setting LogIPFilter raised exception:" + e.Message;
                    }
                }
                
                if (initLogger)
                {
                    //default log file name is BonCodeAJP13ConnectionLog.txt in directory of DLL or Windows 
                    p_Logger = new BonCodeAJP13Logger(BonCodeAJP13Settings.BONCODEAJP13_LOG_FILE + p_LogFilePostFix + DateTime.Now.ToString("yyyyMMdd") + ".log", p_ConnectionMutex);
                    //if RegEx contained error message we will also write an error msg
                    if (errMsg.Length > 0)
                    {
                        p_Logger.LogMessage(errMsg);
                    }
                }
            }
        }


        /// <summary>
        /// start connection using packages stored in instance
        /// </summary>
        public void BeginConnection()
        {
            p_CreateConnection(p_PacketsToSend);
        }


        /// <summary>
        /// add a package to the collection of packets to be send to tomcat
        /// </summary>
        public void AddPacketToSendQueue(BonCodeAJP13Packet singlePacket)
        {
            p_PacketsToSend.Add(singlePacket);
        }


        /// <summary>
        /// implement delegated call to return a physical path for a given virtual path 
        /// </summary>
        /// 
        private string ServerPath(String virtualPath)
        {
            //default to return the main path for the request unless we have a delegate function 
            //to resolve it
            string returnPath = BonCodeAJP13Settings.BonCodeAjp13_PhysicalFilePath;
            if (p_ServerPath != null)
            {
                returnPath = p_ServerPath(virtualPath);   
            }

            return returnPath;
        }


        /// <summary>
        /// implement delegated call to flush packets upon detection of flush event 
        /// </summary>
        private void ProcessFlush()
        {
            //only process if we have delegate function assignment
            if (p_FpDelegate != null)
            {
                //we have to wait until previous flush completes before flushing or until our timeout has expired
                if (p_FlushInProgress != null)
                {
                    int maxWaitCount = (BonCodeAJP13Settings.BONCODEAJP13_FLUSH_TIMEOUT * 1000) / 50;
                    int i = 0;
                    while (p_FlushInProgress() && i < maxWaitCount)
                    {
                        i++;
                        Thread.Sleep(50); //wait 50 miliseconds for flush to complete
                    }
                }

                //pass on the packets received to delegate function for processing
                p_FpDelegate(p_PacketsReceived);
                //Delete all packets processed so far
                p_PacketsReceived.Clear();

            }
            

            
        }

        /// <summary>
        /// Create Actual Connection and initiate wait for response within this thread
        /// </summary>
        private void p_CreateConnection(BonCodeAJP13PacketCollection packetsToSend)
        {
            p_AbortConnection = false;        
            p_ConnectionsCounter++;
            p_ThisConnectionID = p_ConnectionsCounter; //assign the connection id



            try
            {
                //create new connection and timer if we maintain connection timeout within this class
                if (p_TCPClient == null)
                {
                    p_TCPClient = new TcpClient(p_Server, p_Port);

                    p_KeepAliveTimer = new System.Timers.Timer();
                    p_KeepAliveTimer.Interval = BonCodeAJP13Consts.BONCODEAJP13_SERVER_KEEP_ALIVE_TIMEOUT;
                    p_KeepAliveTimer.Elapsed += new System.Timers.ElapsedEventHandler(p_KeepAliveTimer_Elapsed);
                    p_KeepAliveTimer.AutoReset = false;
                    p_KeepAliveTimer.Enabled = true; //starts the timer
                }
                //start p_StopWatch
                p_StopWatch.Start();

                //assign package and wait for response
                p_PacketsToSend = packetsToSend;

                //handle connection
                HandleConnection();
            }
            catch (Exception e)
            {
                if (p_Logger != null) p_Logger.LogException(e, "TCP Client level -- Server/Port:" + p_Server + "/" + p_Port.ToString());
                ConnectionError();
                string errMsg = "Connection to Tomcat has been closed. Tomcat may be restarting. Please retry later.";
                if (p_Logger != null)
                {
                    errMsg = errMsg + "<br><small>Administrator: please check your log file for detail on this error if needed.</small>";
                }
                else
                {
                    errMsg = errMsg + "<br><small>Administrator: please enable logging with log level 1 or greater for detail problem capture if needed.</small>";
                }

                if (BonCodeAJP13Settings.BONCODEAJP13_TOMCAT_TCPCLIENT_ERRORMSG.Length > 1) errMsg = BonCodeAJP13Settings.BONCODEAJP13_TOMCAT_TCPCLIENT_ERRORMSG;
                p_PacketsReceived.Add(new TomcatSendBodyChunk(errMsg));
                return;
            }
        }



        /// <summary>
        /// Handler for the Keep Alive timer. Close the connection when timer has elapsed.
        /// </summary>
        private void p_KeepAliveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {            
            p_AbortConnection = true;
            ((Timer)sender).Dispose(); // should be equivalent but rather be safe  p_KeepAliveTimer.Dispose();
            ConnectionError("Timeout on Connection ID " + p_ThisConnectionID, "Time Out");

        }


        /// <summary>
        /// Creates TCP/IP Level network connection to Tomcat
        /// </summary>
        public void HandleConnection()
        {
       
                        
            if (p_Logger != null) p_Logger.LogMessage(String.Format("New Connection {0} of {1} to tomcat: {2} ID: {3} [T-{4}]",p_ConcurrentConnections,BonCodeAJP13Settings.MAX_BONCODEAJP13_CONCURRENT_CONNECTIONS, p_TCPClient.Client.RemoteEndPoint.ToString(), p_ThisConnectionID, Thread.CurrentThread.ManagedThreadId), BonCodeAJP13LogLevels.BONCODEAJP13_LOG_BASIC);

            //get stream set timeouts again (default 30 minutes)
            try
            {
                
                p_NetworkStream = p_TCPClient.GetStream();
                p_NetworkStream.ReadTimeout = BonCodeAJP13Settings.BONCODEAJP13_SERVER_READ_TIMEOUT;
                p_NetworkStream.WriteTimeout = BonCodeAJP13Settings.BONCODEAJP13_SERVER_WRITE_TIMEOUT;
                
            }
            catch (Exception ex)
            {
                if (p_Logger != null) p_Logger.LogException(ex, "Stream level -- Server/Port:" + p_Server + "/" + p_Port.ToString());

                string errMsg = "Stream Connection to Tomcat unavailable. Tomcat may be restarting. Please retry later.";
                if (BonCodeAJP13Settings.BONCODEAJP13_TOMCAT_STREAM_ERRORMSG.Length > 1) errMsg = BonCodeAJP13Settings.BONCODEAJP13_TOMCAT_STREAM_ERRORMSG;
                p_PacketsReceived.Add(new TomcatSendBodyChunk(errMsg));
                
                ConnectionError();
                return;
            }

            //do the rest of the communication (sending packets and receiving response and assembling
            ComunicateWithTomcat();
        }

        private void ComunicateWithTomcat()
        {
            int numOfBytesReceived = 0;
            byte[] receivedPacketBuffer = new byte[BonCodeAJP13Settings.MAX_BONCODEAJP13_PACKET_LENGTH];
            byte[] notProcessedBytes = null;
            int sendPacketCount = 0;           
            p_IsLastPacket = false;
            int listenAfterPacket = 2; //this is standard behavior, we sent first two packets for posts then listen

            if (BonCodeAJP13Settings.BONCODEAJP13_ADOBE_SUPPORT || p_IsChunked) 
            {
                listenAfterPacket = 1;
            }
            

            //send packages. If multiple forward requests (i.e. form data or files) there is a different behavior expected
            if (p_PacketsToSend.Count > 1)
            {
                bool delayWrite = false;
                //TODO: move this loop to a function by itself so we can reuse it.
                foreach (Object oIterate in p_PacketsToSend)
                {
                    //we will continue sending all packets in queue unless tomcat sends us End Comm package
                    if (!p_IsLastPacket)
                    {
                        sendPacketCount++;
                        BonCodeAJP13Packet sendPacket = oIterate as BonCodeAJP13Packet; //only objects derived from this class should be in the collection

                        //send first packet immediatly (most likely a post), unless there is a delay write indicator (will be set during analysis)
                        if (!delayWrite)
                        {
                            p_NetworkStream.Write(sendPacket.GetDataBytes(), 0, sendPacket.PacketLength);
                        }
                        else
                        {
                            delayWrite = false;
                        }

                        //log packet
                        if (p_Logger != null) p_Logger.LogPacket(sendPacket, false, BonCodeAJP13LogLevels.BONCODEAJP13_LOG_HEADERS);

                        //after the second packet (first packet for Adobe) in a packet collection we have to listen and receive a TomcatGetBodyChunk
                        if (sendPacketCount >= listenAfterPacket)
                        {
                            bool subRoutineReadInProgress = true;
                            int sanityCheck = 0;
                            while (subRoutineReadInProgress)
                            {
                                sanityCheck++;
                                numOfBytesReceived = p_NetworkStream.Read(receivedPacketBuffer, 0, receivedPacketBuffer.Length);
                                notProcessedBytes = AnalyzePackage(ref delayWrite, receivedPacketBuffer, true); //no flush processing during sending of data
                                //we expect a 7 byte response except for the last package record, if not record a warning                        
                                if (sendPacketCount != p_PacketsToSend.Count && numOfBytesReceived > 7)
                                {
                                    if (p_Logger != null) p_Logger.LogMessageAndType("Incorrect response received from Tomcat", "warning", BonCodeAJP13LogLevels.BONCODEAJP13_LOG_BASIC);
                                }
                                //check whether we are finished with subroutine
                                if (delayWrite)
                                {
                                    delayWrite = false; // assume that we will exit with next loop unless Analyze package resets this to true;
                                }
                                else
                                {
                                    subRoutineReadInProgress = false;
                                }
                                //in case
                                if (sanityCheck > 500)
                                {
                                    if (p_Logger != null) p_Logger.LogMessageAndType("SubRoutine Communication Process suspicious iterations (>500). This indicates problems with communication to tomcat", "warning", BonCodeAJP13LogLevels.BONCODEAJP13_LOG_BASIC);
                                    subRoutineReadInProgress = false;
                                }
                            }

                        }
                    }
                    else
                    {

                        break;
                    }

                }  
                


                //if the last received message from tomcat is "GET_BODY_CHUNK" we need to send terminator package
                if(p_PacketsReceived[p_PacketsReceived.Count-1] is TomcatGetBodyChunk) {
                    BonCodeAJP13Packet sendPacket = new BonCodeAJP13ForwardRequest(new byte[] { }); //create terminator (empty) package
                    p_NetworkStream.Write(sendPacket.GetDataBytes(), 0, sendPacket.PacketLength);
                    //log packet as it is sent
                    if (p_Logger != null) p_Logger.LogPacket(sendPacket, false, BonCodeAJP13LogLevels.BONCODEAJP13_LOG_HEADERS);
                }


            }
            else if (p_PacketsToSend.Count == 1)
            {
                //send package
                BonCodeAJP13Packet sendPacket = p_PacketsToSend[0] as BonCodeAJP13Packet; //only objects derived from this class should be in the collection
                p_NetworkStream.Write(sendPacket.GetDataBytes(), 0, sendPacket.PacketLength);
                //log each packet as it is sent
                if (p_Logger != null) p_Logger.LogPacket(sendPacket, false, BonCodeAJP13LogLevels.BONCODEAJP13_LOG_HEADERS);
            }
            else
            {
                //nothing to do
                CloseConnectionNoError("Nothing to send. Closing Connection.");
                return;
            }

            

            //switch into Receiving Mode. Receive the TcpServer.response.
            if (!p_IsLastPacket)
            {
                p_NetworkStream.Read(receivedPacketBuffer, 0, 0); //call empty read so we block this thread until we receive a response or we time out
            }
            numOfBytesReceived = 0;
            
     
            try
            {
                int readCount = 0;

                while (p_NetworkStream.CanRead && !p_AbortConnection && !p_IsLastPacket)
                {
                    //check to see whether we need to send extra termination package
                    if (p_SendTermPacket)
                    {
                        p_SendTermPacket = false;
                        BonCodeAJP13ForwardRequest terminatorFR = new BonCodeAJP13ForwardRequest(new byte[] { });
                        p_NetworkStream.Write(terminatorFR.GetDataBytes(), 0, terminatorFR.PacketLength);
                    }
                    
                    //clear reading array
                    Array.Clear(receivedPacketBuffer,0,receivedPacketBuffer.Length);
                 
                    //flush detection by ticks check if we we have no data on channel                    
                    //check whether we need monitor for tomcat flush signs  
                    if (!p_NetworkStream.DataAvailable && BonCodeAJP13Settings.BONCODEAJP13_AUTOFLUSHDETECTION_TICKS > 0)
                    {
                        long elapsedTicks = p_StopWatch.ElapsedTicks;
                        p_TickDelta = elapsedTicks - p_LastTick;
                        p_LastTick = elapsedTicks;
                        if (p_TickDelta > BonCodeAJP13Settings.BONCODEAJP13_AUTOFLUSHDETECTION_TICKS)
                        {
                            //flush has been detected set the flag. We should flush after this receiveBuffer has been processed.
                            //no flush is needed if we see end marker during receiveBuffer processing
                            p_IsFlush = true;
                        }
                    }



                    //read incoming packets until timeout or last package has been received.
                    readCount++;                       
                    //read or wait on next package
                    numOfBytesReceived = p_NetworkStream.Read(receivedPacketBuffer, 0, receivedPacketBuffer.Length);

                    //flush detection by bytes
                    if (BonCodeAJP13Settings.BONCODEAJP13_AUTOFLUSHDETECTION_BYTES > 0)
                    {
                        p_BytesInBuffer = p_BytesInBuffer + numOfBytesReceived;
                        if (p_BytesInBuffer > BonCodeAJP13Settings.BONCODEAJP13_AUTOFLUSHDETECTION_BYTES)
                        {
                            p_IsFlush = true;
                            p_BytesInBuffer = 0;
                        }
                    }

                    

                    //analyze packet so far (adjust bytes from Receiving buffer):combine notProcessed with new Read bytes into new Received buffer if needed                        
                    if (notProcessedBytes != null)
                    {
                        //create tempArray that contains new set of bytes to be send a combination of newly received bytes as well as bytes that we were not able to process yet
                        byte[] tempArray = new byte[numOfBytesReceived + notProcessedBytes.Length];
                        Array.Copy(notProcessedBytes, 0, tempArray, 0, notProcessedBytes.Length);
                        Array.Copy(receivedPacketBuffer, 0, tempArray, notProcessedBytes.Length, numOfBytesReceived);

                        notProcessedBytes = AnalyzePackage(tempArray);
                    }
                    else
                    {
                        //send bytes we received for analysis
                        byte[] tempArray = new byte[numOfBytesReceived];
                        Array.Copy(receivedPacketBuffer, 0, tempArray, 0, numOfBytesReceived);
                        notProcessedBytes = AnalyzePackage(tempArray);
                    }

                }
                    
                //check error condition that tomcat produces sometimes where additional data is sent after end_transmission has been indicated                
                int sanityCheck = 0;
                while (p_NetworkStream.DataAvailable && sanityCheck < 100)
                {
                    //we need to clear the tcp pipe so the next request does not pick up data we will do this up to 2050 times and write warning
                    numOfBytesReceived = p_NetworkStream.Read(receivedPacketBuffer, 0, receivedPacketBuffer.Length);
                    sanityCheck++;
                }
                
                


            }

            catch (System.IO.IOException ex)
            {
                ConnectionError("Server Connection is closing, Read timeout reached and no tomcat activity was detected.", "TimeOut");
                if (p_Logger != null) p_Logger.LogException(ex);
                return;
            }

            if (p_AbortConnection)
            {
                ConnectionError("Server Connection was aborted:" , "Failed");
                return;
            }

            if (numOfBytesReceived == 0)
            {
                // Nothing received from tomcat, log warning
                p_Logger.LogMessageAndType("Empty packet received from tomcat", "warning", BonCodeAJP13LogLevels.BONCODEAJP13_LOG_BASIC);
                //return;
            }

 
            if (p_IsLastPacket == true)
            {
                // keep alive timer needs reset (we are maintaining connection but resetting the timer
                if (p_KeepAliveTimer != null)
                {
                    p_KeepAliveTimer.Stop();
                   
                    p_KeepAliveTimer.Start();
                }

                //CloseConnectionNoError();
               
            }
            else
            {
                //do nothing for now               
            }
        }

         

        /// <summary>
        /// Close connection and its Network stream. Everything is OK.
        /// </summary>
        private void CloseConnectionNoError(string message = "Closing Connection")
        {
            if (p_Logger != null) p_Logger.LogMessage(message, BonCodeAJP13LogLevels.BONCODEAJP13_LOG_DEBUG);
           

            /* eloborate close   */            
            p_TCPClient.Client.Close();
            p_TCPClient.Close();
            p_TCPClient.Client.Shutdown(SocketShutdown.Both);
            p_NetworkStream.Flush();
            p_NetworkStream.Close();
            p_NetworkStream.Dispose();
            p_TCPClient = null;
            p_NetworkStream = null;
           
            //Kill associated timer
            if (p_KeepAliveTimer != null) p_KeepAliveTimer.Dispose();
        }


        /// <summary>
        /// Close connection and its Network stream and invoke the Connection Returned event with failure string.
        /// This function has one override with string that can be passed.
        /// </summary>
        private void ConnectionError()
        {
            if (p_Logger != null) p_Logger.LogMessage("One Connection raised an error", BonCodeAJP13LogLevels.BONCODEAJP13_LOG_ERRORS);
            //attempt to close stream
            try
            {
               // p_NetworkStream.Close();              
            }
            catch (Exception e)
            {
                //log exception
                if (p_Logger != null) p_Logger.LogException(e, "Stream Close:", BonCodeAJP13LogLevels.BONCODEAJP13_LOG_ERRORS);
            }
            //attempt to close client
            try
            {               
                //p_TCPClient.Close();
            }
            catch (Exception e)
            {
                //log exception
                if (p_Logger != null) p_Logger.LogException(e, "TcpClient Close:", BonCodeAJP13LogLevels.BONCODEAJP13_LOG_ERRORS);
            }

            
            //now raise the error for the call handler it will close this connection instead
            if (BonCodeAJP13Settings.BONCODEAJP13_ENABLE_HTTPSTATUSCODES)
            {
                throw new System.InvalidOperationException("connection between Tomcat and IIS experienced error. Please check log.");
            }
        }

        private void ConnectionError(string message="", string messageType="")
        {
            if (p_Logger != null) p_Logger.LogMessageAndType(message, messageType, BonCodeAJP13LogLevels.BONCODEAJP13_LOG_ERRORS);
            //attempt to close stream
            try
            {
               // p_NetworkStream.Close();
            }
            catch (Exception e)
            {
                //log exception
                if (p_Logger != null) p_Logger.LogException(e, "Stream Close:", BonCodeAJP13LogLevels.BONCODEAJP13_LOG_ERRORS);
            }
            //attempt to close client
            try
            {
              // p_TCPClient.Close();
            }
            catch (Exception e)
            {
                //log exception
                if (p_Logger != null) p_Logger.LogException(e, "TcpClient Close:", BonCodeAJP13LogLevels.BONCODEAJP13_LOG_ERRORS);
            }

            //now raise the error for the call handler it will close this connection instead
            if (BonCodeAJP13Settings.BONCODEAJP13_ENABLE_HTTPSTATUSCODES)
            {
                throw new System.InvalidOperationException("connection between Tomcat and IIS experienced error. Please check log. " + messageType + ":" + message);
            }
           
        }
        #endregion

        #region AnalysePackages


        /// <summary>
        /// Primary Signature for Analyzing Package. Review received bytes and put them into the package container.
        /// Added delayWriteIndicator in case write occured in this context we want to delay writing for POST.
        /// </summary>
        private byte[] AnalyzePackage(ref bool delayWriteIndicator, byte[] receiveBuffer, bool skipFlush = false, int iOffset = 0)
        {
            //the packages received by tomcat have to start with the correct package signature start bytes (41 42 or AB)
            int iStart = 0 + iOffset; //offset determines where we start in this package
            byte[] searchFor = new byte[2] { 0x41, 0x42 };
            int packetLength = 0;
            byte[] unalyzedBytes = null;
            int iSafety = 0; //safety check for exit condition (with standard packets and a min packet size of 4 bytes max theoretical packets to be analyze is 8196/4)

            while (iStart >= 0 && iStart <= receiveBuffer.Length - 1 && iSafety <= 2050)
            {
                iSafety++;

                //find packet start bytes (41 42 or AB)
                iStart = ByteSearch(receiveBuffer, searchFor, iStart);
                if (iStart >= 0)
                {
                    //determine end of packet if this is negative we need continue scanning 
                    try
                    {
                        packetLength = GetInt16B(receiveBuffer, iStart + 2);
                    }
                    catch (Exception e)
                    {
                        //log exception
                        if (p_Logger != null) p_Logger.LogException(e, "packet length determination problem", 1);
                    }
                    //check whether we have sufficient data in receive buffer to analyze package
                    if (packetLength + iStart + 4 <= receiveBuffer.Length)
                    {
                        //TODO: check whether packet length is beyond maximum and throw error
                        int packetType = (int)receiveBuffer[iStart + 4];
                        byte[] userData = new byte[packetLength];
                        string adobePath = "";
                        string requestPath = "";

                        //we skip 4-bytes:magic (AB) and packet length markers when determining user data
                        Array.Copy(receiveBuffer, iStart + 4, userData, 0, packetLength);

                        

                        //Detect Correct packet Type and create instance of store
                        switch (packetType)
                        {
                            case BonCodeAJP13TomcatPacketType.TOMCAT_GETBODYCHUNK:
                                p_PacketsReceived.Add(new TomcatGetBodyChunk(userData));
                                //if this command is encountered when only one GET package was previously send (no multi-packets) we need to send a terminator body package
                                if (p_PacketsToSend.Count == 1)
                                {
                                    p_SendTermPacket = true;
                                }

                                //p_NetworkStream.Write(sendPacket.GetDataBytes(), 0, sendPacket.PacketLength);
                                break;
                            case BonCodeAJP13TomcatPacketType.TOMCAT_ENDRESPONSE:
                                p_PacketsReceived.Add(new TomcatEndResponse(userData));
                                //this is the termination indicator we need to stop processing from here on. This can happen 
                                //when we post data as well. Tomcat can indicate a connection stop and we need to stop processing as well.

                                break;
                            case BonCodeAJP13TomcatPacketType.TOMCAT_SENDHEADERS:
                                p_PacketsReceived.Add(new TomcatSendHeaders(userData));
                                break;
                            case BonCodeAJP13TomcatPacketType.TOMCAT_CPONGREPLY:
                                p_PacketsReceived.Add(new TomcatCPongReply(userData));
                                break;
                            case BonCodeAJP13TomcatPacketType.TOMCAT_SENDBODYCHUNK:
                                //only add user data if there is something so we don't create null packets (this condition may not occur)
                                if (userData.Length > 0)
                                {
                                    p_PacketsReceived.Add(new TomcatSendBodyChunk(userData));
                                }
                                else
                                {
                                    //warning
                                    if (p_Logger != null) p_Logger.LogMessage("Received empty user packet in TOMCAT_SENDBODYCHUNK, skipping.", BonCodeAJP13LogLevels.BONCODEAJP13_LOG_DEBUG);
                                }
                                break;

                            case BonCodeAJP13TomcatPacketType.TOMCAT_CFPATHREQUEST:
                                //this is Adobe specific we will need to send back a header
                                p_PacketsReceived.Add(new TomcatPhysicalPathRequest(userData));
                                requestPath = ((TomcatPhysicalPathRequest)p_PacketsReceived[p_PacketsReceived.Count - 1]).GetFilePath();
                                //TODO: move the following into a response queue
                                //prep response and return now CF will ask for two paths one for index.htm one for the actual URI 
                                //The user did not ask for index.htm it is how CF marks root vs final path
                                adobePath = ServerPath(requestPath); //System.Web.HttpContext.Current.Server.MapPath("/yeah") ;//BonCodeAJP13Settings.BonCodeAjp13_PhysicalFilePath;                                
                                if (requestPath == "/index.htm")
                                {
                                    adobePath = BonCodeAJP13Settings.BonCodeAjp13_DocRoot + "index.htm";
                                };
                                //if we have trouble with the path packet we will cause exception
                                BonCodeFilePathPacket pathResponse = null;
                                pathResponse = new BonCodeFilePathPacket(adobePath);

                                /* Degbug sequence START
                                try
                                {
                                    pathResponse = new BonCodeFilePathPacket(adobePath);
                                }
                                catch (Exception e)
                                {
                                    if (p_Logger != null) p_Logger.LogException(e, "Problem determining Adobe path [" + adobePath + "] for: [" + requestPath +"] setting to fake [" + BonCodeAJP13Settings.BonCodeAjp13_DocRoot + "\\index.htm] instead.");
                                    adobePath = BonCodeAJP13Settings.BonCodeAjp13_DocRoot + "index.htm";
                                    pathResponse = new BonCodeFilePathPacket(adobePath);
                                }                                 
                                Debug Sequence END */

                                p_NetworkStream.Write(pathResponse.GetDataBytes(), 0, pathResponse.PacketLength);
                                if (p_Logger != null) p_Logger.LogPacket(pathResponse);
                                delayWriteIndicator = true; //prevent main process from writing to network stream
                                break;
                            default:
                                //we don't know this type of package; add to collection anyway and log it, we will not raise error but continue processing                               
                                p_PacketsReceived.Add(new TomcatReturn(userData));
                                if (p_Logger != null)
                                {
                                    p_Logger.LogMessage("Unknown Packet Type Received, see next log entry:", BonCodeAJP13LogLevels.BONCODEAJP13_LOG_ERRORS);
                                    p_Logger.LogPacket(p_PacketsReceived[p_PacketsReceived.Count - 1], true);
                                };

                                break;
                        }


                        //Log packets Received. Whether the packet will actually be logged depends on log level chosen.
                        if (p_Logger != null) p_Logger.LogPacket(p_PacketsReceived[p_PacketsReceived.Count - 1]);

                        //reset new iStart
                        iStart = iStart + 4 + packetLength - 1;

                        //detect end package
                        if (packetType == BonCodeAJP13TomcatPacketType.TOMCAT_ENDRESPONSE)
                        {
                            p_IsLastPacket = true;
                            p_IsFlush = false;
                        }
                        else
                        {
                            //old flush check position
                            /*
                            //check whether we need monitor for tomcat flush signs  
                            if (BonCodeAJP13Settings.BONCODEAJP13_AUTOFLUSHDETECTION_TICKS > 0)
                            {
                                long elapsedTicks = p_StopWatch.ElapsedTicks;
                                p_TickDelta = elapsedTicks - p_LastTick;
                                p_LastTick = elapsedTicks;
                                if (p_TickDelta > BonCodeAJP13Settings.BONCODEAJP13_AUTOFLUSHDETECTION_TICKS)
                                {
                                    //flush has been detected set the flag. We should flush after this receiveBuffer has been processed.
                                    //no flush is needed if we see end marker during receiveBuffer processing                                 

                                    p_IsFlush = true;
                                }
                            }
                            */

                        }
                    }
                    else
                    {
                        //we need to read more data before we can process. For now mark these bytes as unanalyzed and return to stream reader
                        unalyzedBytes = new byte[receiveBuffer.Length - iStart];
                        Array.Copy(receiveBuffer, iStart, unalyzedBytes, 0, receiveBuffer.Length - iStart);
                        //set breakout conditions
                        iStart = iStart + packetLength - 1;
                        break;
                    }
                }

            }

            //flush skip check 
            if (skipFlush)
            {
                p_IsFlush = false;
            }


            //flush processing
            if (p_IsFlush)
            {
                //do what is needed to flush data so far
                ProcessFlush();

                //reset flush marker
                p_IsFlush = false;
            }

            return unalyzedBytes;
        }

        /// <summary>
        /// Alternate Signature, drop reference needed. Review received bytes and put them into the package container
        /// This signature is maintained so we don't have to change existing code.
        /// </summary>
        private byte[] AnalyzePackage(byte[] receiveBuffer,bool skipFlush = false,int iOffset=0)
        {
            //call primary signature and return bytes while dropping delay write.            
            bool delayWrite = false;
            return AnalyzePackage(ref delayWrite, receiveBuffer, skipFlush, iOffset);

        }

        /// <summary>
        /// Get the Int16 value from the array starting from the pos pos 
        /// Using unsigned integers only range from 0 to 65,535
        /// </summary>
        private int GetInt16B(byte[] Data, int Pos)
        {
            UInt16 Value = 0;
            byte[] ValueData = new byte[sizeof(Int16)];
            Array.Copy(Data, Pos, ValueData, 0, sizeof(Int16));

            //flipping for BigEndian conversion prep
            ValueData = FlipArray(ValueData);
            //Value = BitConverter.ToInt16(ValueData, 0);
            Value = BitConverter.ToUInt16(ValueData, 0);
            return (int) Value;
        }

        /// <summary>
        /// Flips the array to send it using the big endian convention
        /// </summary>
        private static byte[] FlipArray(byte[] Data)
        {
            byte[] FlippedData = new byte[Data.GetLength(0)];

            for (int i = 0; i < Data.GetLength(0); i++)
            {
                FlippedData[Data.GetUpperBound(0) - i] = Data[i];
            }
            return FlippedData;
        }

        /// <summary>
        /// Find byte pattern in given byte array. if not found will return -1. Otherwise will return index of starting match
        /// </summary>
        private static int ByteSearch(byte[] searchIn, byte[] searchBytes, int start = 0)
        {
            int found = -1;
            bool matched = false;
            //only look at this if we have a populated search array and search bytes with a sensible start
            if (searchIn.Length > 0 && searchBytes.Length > 0 && start <= (searchIn.Length - searchBytes.Length) && searchIn.Length >= searchBytes.Length)
            {
                //iterate through the array to be searched
                for (int i = start; i <= searchIn.Length - searchBytes.Length; i++)
                {
                    //if the start bytes match we will start comparing all other bytes
                    if (searchIn[i] == searchBytes[0])
                    {
                        if (searchIn.Length > 1)
                        {
                            //multiple bytes to be searched we have to compare byte by byte
                            matched = true;
                            for (int y = 1; y <= searchBytes.Length - 1; y++)
                            {
                                if (searchIn[i + y] != searchBytes[y])
                                {
                                    matched = false;
                                    break;
                                }
                            }
                            //everything matched up
                            if (matched)
                            {
                                found = i;
                                break;
                            }

                        }
                        else
                        {
                            //search byte is only one bit nothing else to do
                            found = i;
                            break; //stop the loop
                        }

                    }
                }

            }
            return found;
        }



        #endregion
    }
}



