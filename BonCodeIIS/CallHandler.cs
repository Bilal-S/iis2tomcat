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
using System.Collections.Specialized;
using System.Net.Sockets;
using System.Web;
using BonCodeAJP13;
using BonCodeAJP13.ServerPackets;
using BonCodeAJP13.TomcatPackets;
using System.Net;
using System.Threading;
using System.Security.Cryptography.X509Certificates;




namespace BonCodeIIS
{
    public class BonCodeCallHandler : IHttpHandler
    {
        
        //instance variables
        private HttpContext p_Context;
        private TcpClient p_TcpClient = null;
        private static int p_InstanceCount =0;
        private static bool p_isReusable = true;
        private bool p_FlagKillConnection = false;
        private bool p_FlushInProgress = false;


        #region destructor
        /// <summary>
        /// Clean up code when we are killing this object
        /// </summary>
        ~ BonCodeCallHandler()        
        {
            try
            {
                KillConnection();
                Interlocked.Decrement(ref p_InstanceCount); 
           
            }
            catch (Exception exp)
            {                
                RecordSysEvent("Error during call handler destruction: " + exp.Message, EventLogEntryType.Error);
            }
      
        }


        #endregion

        #region constructor
        /// <summary>
        /// Constructor will maintain instance count
        /// </summary>
        public BonCodeCallHandler()
        {
            Interlocked.Increment(ref p_InstanceCount);
            //p_InstanceCount++;
        }

        #endregion

        /// <summary>
        /// Declaring ourselves as reusable, so threads can be pooled
        /// </summary>
        public bool IsReusable
        {
            get { return p_isReusable; }
        }

       

        /// <summary>
        /// Main process hook for IIS invocation.
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            //TODO: application scope move: need to see whether config data is better saved in app scope after initial reading. Would need a reset mechanism if cached this way
            //System.Web.HttpContext.Current.Application.Lock();
            //System.Web.HttpContext.Current.Application["Server"] = "localhost";
            //System.Web.HttpContext.Current.Application.UnLock();
            
            //check execution
            string executionFeedback = CheckExecution(context);
            bool blnProceed = true;
            bool isChunkedTransfer = false;
            int sourcePort = p_InstanceCount;  //init with count will override with port if later available

           
            try
            {
                if (executionFeedback.Length == 0)
                {
                    //determine web doc root if needed                    
                    //set shared settings (global mutable variables). Not ideal solution. Will need to refactor later.
                    if (BonCodeAJP13Settings.BONCODE_DOCROOT_OVERRIDE.Length > 0)
                    {
                        BonCodeAJP13Settings.BonCodeAjp13_DocRoot = BonCodeAJP13Settings.BONCODE_DOCROOT_OVERRIDE;
                    }
                    else
                    {
                        BonCodeAJP13Settings.BonCodeAjp13_DocRoot = System.Web.HttpContext.Current.Server.MapPath("~");
                    }

                    
                    //in some circumstances invalid path data can be supplied by client if so we will set the path to blank when exception occurs, e.g. http://project/group:master...master
                    try
                    {
                        BonCodeAJP13Settings.BonCodeAjp13_PhysicalFilePath = context.Request.PhysicalPath;
                    }
                    catch (Exception exp)
                    {
                        BonCodeAJP13Settings.BonCodeAjp13_PhysicalFilePath = "";
                        RecordSysEvent("Setting blank AJP physical path: " + exp.Message, EventLogEntryType.Warning);
                    }

                    //check whether we are resuable, we discard and re-establish connections if MAX_BONCODEAJP13_CONCURRENT_CONNECTIONS is set to zero
                    if (BonCodeAJP13Settings.MAX_BONCODEAJP13_CONCURRENT_CONNECTIONS == 0)
                    {
                        p_isReusable = false;
                    }
                    //determine whether we are declaring ourself as part of a reusable pool. If not we need to also take steps to 
                    //kill connections if we are close to the max of pool we maintain a ten thread margin
                    //this allows limited processing to continue even if we are close to maxed out on connections
                    if (p_isReusable && BonCodeAJP13Settings.MAX_BONCODEAJP13_CONCURRENT_CONNECTIONS < (p_InstanceCount + 10))
                    {
                        p_isReusable = false; //new connections will be dropped immediatly
                    };


                    //assign reference to context to an instance handler
                    p_Context = context;
                    long streamLen = context.Request.InputStream.Length;
                    //create TcpClient to pass to AJP13 processor, this will re-use connection until this instance is destroyed
                    if (p_TcpClient == null)
                    {
                        try
                        {
                            p_TcpClient = new TcpClient(BonCodeAJP13Settings.BONCODEAJP13_SERVER, BonCodeAJP13Settings.BONCODEAJP13_PORT);
                        }
                        catch (Exception e)
                        {
                            //check whether we had issues connecting to tomcat
                            blnProceed = false;
                            string errMsg = "Error connecting to Apache Tomcat instance.<hr>Please check that a Tomcat server is running at given location and port.<br>Details:<br>" + e.Message + "<br><small><small><br>You can change this message by changing TomcatConnectErrorURL setting in setting file.</small></small>";
                            //use the PrintEror function, it will check for redirect already
                            RecordSysEvent("Connection error 1: " + e.Message, EventLogEntryType.Error);
                            PrintError(context, errMsg, e.Message + " " + e.StackTrace);                

                        }


                        //determine whether we will need to remove the connection later
                        if (!p_isReusable)
                        {
                            p_FlagKillConnection = true;
                        }
                        else
                        {
                            p_FlagKillConnection = false;
                        }

                    }
                    else
                    {
                        //check whether existing TCP/IP connection is still working. If tomcat is restarted the connection needs to be reset here as well
                        if (!p_TcpClient.Connected)
                        {
                            KillConnection();
                            p_TcpClient = new TcpClient(BonCodeAJP13Settings.BONCODEAJP13_SERVER, BonCodeAJP13Settings.BONCODEAJP13_PORT);
                        }

                    }

                    if (blnProceed)
                    {
                        //check for chunked transfer
                        if (context.Request.ServerVariables["HTTP_TRANSFER_ENCODING"] != null && context.Request.ServerVariables["HTTP_TRANSFER_ENCODING"] == "chunked")
                        {
                            isChunkedTransfer = true;
                        }

                        //determine instance id
                        /*
                        UInt32 instanceId = 0;                       
                        try
                        {
                            //instanceId = Convert.ToInt16(context.Request.ServerVariables["INSTANCE_ID"]);
                            instanceId = Convert.ToUInt32(System.Web.Hosting.HostingEnvironment.ApplicationHost.GetSiteID());                            
                        }
                        catch (Exception err) {
                            instanceId = 0;                            
                            RecordSysEvent("Cannot set instanceId setting to zero instead: " + err.Message, EventLogEntryType.Warning);
                        } // empty catch instanceId of zero indicates error
                        */
                                                
                        //initialize AJP13 protocol connection
                        string logFilePostFix = "_" + context.Request.ServerVariables["INSTANCE_ID"] + "_" + context.Server.MachineName + "_";
                        string clientIp = GetRemoteAddr(context.Request.ServerVariables);
                        BonCodeAJP13ServerConnection sconn = new BonCodeAJP13ServerConnection(logFilePostFix, clientIp);
                        sconn.FlushDelegateFunction = PrintFlush;  //this function will do the transfer to browser if we use Flush detection, we pass as delegate
                        sconn.FlushStatusFunction = IsFlushing; //will let the implementation know if flushing is still in progress
                        sconn.SetTcpClient = p_TcpClient;
                        sconn.ChunkedTransfer = isChunkedTransfer;
                        //TODO: bind this into log file name  
                      
                        //check for header data support and retrieve virtual directories if needed
                        String virPaths = "";
                        if (BonCodeAJP13Settings.BONCODEAJP13_HEADER_SUPPORT)
                        {
                            virPaths = GetVDirs();
                        }
                        

                        //check for Adobe support
                        if (BonCodeAJP13Settings.BONCODEAJP13_ADOBE_SUPPORT)
                        {
                            sconn.ServerPathFunction = ServerPath;
                        }
                        //setup basic information (base ForwardRequest package)            
                        sourcePort = ((IPEndPoint)p_TcpClient.Client.LocalEndPoint).Port;
                        BonCodeAJP13ForwardRequest FR = null; 
                        //if we are in SSL mode we need to check for client certificates
                        if (context.Request.IsSecureConnection && context.Request.ClientCertificate.IsPresent)                        {

                            FR = new BonCodeAJP13ForwardRequest(context.Request.ServerVariables, context.Request.PathInfo, sourcePort, virPaths, GetClientCert(context));
                        }
                        else
                        {
                            FR = new BonCodeAJP13ForwardRequest(context.Request.ServerVariables, context.Request.PathInfo, sourcePort, virPaths);
                        }

                        sconn.AddPacketToSendQueue(FR);



                        //determine if extra ForwardRequests are needed. 
                        //We need to create a collection of Requests (for form data and file uploads etc.) 
                        //TODO: think about streaming support. The reading would be posted to a different thread that continues the reading process while
                        //      the AJP handler continues writing the packets back to tomcat
                        if (context.Request.ContentLength > 0 || isChunkedTransfer)
                        {
                            // need to create a collection of forward requests to package data in      
                            int maxPacketSize = BonCodeAJP13Settings.MAX_BONCODEAJP13_USERDATA_LENGTH - 1;
                            int numOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(context.Request.ContentLength / Convert.ToDouble(maxPacketSize))));
                            int iStart = 0;
                            int iCount = 0;

                            //for chunked transfer we use stream length to determine number of packets
                            if (isChunkedTransfer)
                            {
                                numOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(streamLen / Convert.ToDouble(maxPacketSize)))); ;
                            }

                            for (int i = 1; i <= numOfPackets; i++)
                            {
                                //we need to breakdown data into multiple FR packages to tomcat
                                if (i * maxPacketSize <= streamLen)
                                {
                                    //we are in the middle of transferring data grab next 8188 (if default packet size) bytes and create package
                                    iStart = (i - 1) * (maxPacketSize);
                                    iCount = Convert.ToInt32(maxPacketSize);
                                }
                                else
                                {
                                    //last user package
                                    iStart = (i - 1) * (maxPacketSize);
                                    iCount = Convert.ToInt32(streamLen) - iStart;
                                }
                                //add package to collection
                                byte[] streamInput = new byte[iCount];
                                context.Request.InputStream.Read(streamInput, 0, iCount); //stream pointer moves with each read so we allways start at zero position
                                sconn.AddPacketToSendQueue(new BonCodeAJP13ForwardRequest(streamInput));

                            }
                            //add an empty Forward Request packet as terminator to collection if multiple packets are used
                            //sconn.AddPacketToSendQueue(new BonCodeAJP13ForwardRequest(new byte[] { }));

                        }

                        //run connection (send and receive cycle)

                        try
                        {
                            sconn.BeginConnection();
                        }

                        catch (Exception e)
                        {
                            //we have an error do the dump on screen since we are not logging but allso kill connection 
                            RecordSysEvent("Connection error 2: " + e.Message, EventLogEntryType.Error);
                            PrintError(context, ".", e.Message + " " + e.StackTrace);
                            KillConnection(); //remove TCP cache good after timeouts
                        }

                        //write the response to browser (if not already flushed)
                        PrintFlush(sconn.ReceivedDataCollection);

                        //kill connections if we are not reusing connections or other problems occurred
                        if (p_FlagKillConnection)
                        {
                            KillConnection();
                        }

                        //dispose the sconn explictily
                        sconn = null;

                    }; // proceed is true

                    //dispose of context explicity
                    p_Context = null;

                }
                else
                {
                    //execution was denied by logic, only print message
                    context.Response.Write(executionFeedback);
                }
            }

            catch (InvalidOperationException e)
            {
                //TODO: check with Dominic whether the behavior is correct here
                //this is where we set the status code to 500
                //context.Response.StatusCode = 500;
                KillConnection(); //remove TCP connection and cache
                PrintError(context, ".", e.Message + " " + e.StackTrace);
            }
            catch (HttpException e)
            {
                //if we have web exception display reasonsa maxRequest length exception, e.g. 3004 then display different message otherwise normal message
                
                KillConnection(); //remove TCP connection and cache
                string strErr = "IIS Web Processing Exception (" + e.GetHttpCode().ToString() + "):<hr>" + e.Message + "<br><small>For maximum request size limit errors please have administrator adjust maxRequestLength and/or maxAllowedContentLength.</small><br>";
                PrintError(context, strErr, e.StackTrace);
           
            }
            catch (Exception e)  //Global Exception catcher
            {
                KillConnection(); //remove TCP connection and cache
                PrintError(context, ".", e.Message + " " + e.StackTrace);

            }

            //if we are directed to do aggressive garbage collection we will do so now 
            if (BonCodeAJP13Settings.BONCODEAJP13_FORCE_GC)
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
            }    
            

        }

       

        /// <summary>
        /// Return the mapping of or URi to physical server path, including virtual directories etc.
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        string ServerPath(String virtualPath)
        {
            return System.Web.HttpContext.Current.Server.MapPath(virtualPath);
        }

        /// <summary>
        /// Show whether output to browser is in progress (we don't want multiple outputs to browser)
        /// </summary>
        bool IsFlushing()
        {
            return p_FlushInProgress;
        }

        /// <summary>
        /// Function to be passed as delegate to BonCodeAJP13 process
        /// Will pass packet collection content to user browser and flush
        /// </summary> 
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        void PrintFlush(BonCodeAJP13PacketCollection flushCollection)
        {
            p_FlushInProgress = true;
            
            string keyName = "";
            string keyValue = "";
            
           
            bool isBinary = false;
            long contentLength = 0; //only assigned if content is known
            long transferredBytes = 0;

            foreach (TomcatReturn flushPacket in flushCollection)
            {
                try
                {
                    //check by packet type and do different processing before calling flush                
                    if (flushPacket is TomcatSendHeaders)
                    {
                        TomcatSendHeaders tcshPackage = (TomcatSendHeaders)flushPacket;
                        //get Headers
                        NameValueCollection tomcatHeaders = tcshPackage.GetHeaders();
                        
                        //iterate through headers and set
                        for (int i = 0; i < tomcatHeaders.AllKeys.Length; i++)
                        {
                            keyName = tomcatHeaders.AllKeys[i];
                            keyValue = tomcatHeaders[keyName];


                            //check for repeated headers of the same type they are seperated by pipe+comma combination
                            string[] sHeaders = keyValue.Split(new string[] { "|," }, StringSplitOptions.None);
                            string tempValue = "";
                            if (sHeaders.Length > 1)
                            {
                                //check for multiple headers of same type returned, e.g. cookies                                
                                for (int i2 = 0; i2 < sHeaders.Length; i2++)
                                {                                   

                                    if (i2 == sHeaders.Length - 1)
                                    {
                                        tempValue = sHeaders[i2].Substring(0, sHeaders[i2].Length - 1); //last array element
                                    }
                                    else
                                    {
                                        tempValue = sHeaders[i2]; //regular array element
                                    }
                                    p_Context.Response.AddHeader(keyName, tempValue);
                                }
                            }
                           

                            else
                            {
                                //single header remove pipe character at the end   
                                tempValue = keyValue.Substring(0, keyValue.Length - 1);
                                p_Context.Response.AddHeader(keyName, tempValue);

                            }

                            //check for binary or text disposition
                            if (!isBinary && (keyName == "Content-Type" || keyName == "Content-Encoding"))
                            {
                                //set encoding seperatly if needed
                                if (keyName == "Content-Encoding" && ( tempValue.Contains("gzip") || tempValue.Contains("deflate") ))
                                {
                                    isBinary = true;
                                }
                                else
                                {
                                    isBinary = TestBinary(keyValue);
                                }                              

                            }
                            //check for known content length
                            if (keyName == "Content-Length")
                            {
                                try
                                {
                                    contentLength = System.Convert.ToInt64(tempValue);
                                }
                                catch (Exception e) {
                                    contentLength = 0;
                                    RecordSysEvent("Setting content-length to zero: " + e.Message, EventLogEntryType.Warning);
                                };


                            }


                            //check whether we can represent a given header in native IIS Response context (currently only used for server side redirects)
                            IISNativeHeaders(keyName, tempValue);


                        }
                        //set response status code
                        if (BonCodeAJP13Settings.BONCODEAJP13_ENABLE_HTTPSTATUSCODES)
                        {
                            int respStatus = tcshPackage.GetStatus();
                            //is the status to be returned is an error status >=400 then we need set the response flag and kill conn flags
                            //we will mark this as to be killed if status indicates error to ensure that stream cache is removed and cannot be reused by other connections
                            if (respStatus >= 400)
                            {
                                if (BonCodeAJP13Settings.BONCODEAJP13_SKIP_IISCUSTOMERRORS)
                                {
                                    p_Context.Response.TrySkipIisCustomErrors = true;
                                }
                                p_FlagKillConnection = true; //we are only marking here to ensure that we finish writing as much as possible to stream before closing
                            }
                            //set the actual Status code on the response
                            p_Context.Response.StatusCode = respStatus;
                                                        

                        }

                    }
                    else if (flushPacket is TomcatEndResponse)
                    {
                        //if this is the last package and we know the content length we need to write empty strings
                        //this is a fix if content-length is misrepresented by tomcat
                        if (contentLength > 0 && transferredBytes < contentLength)
                        {
                            string fillEmpty = new string(' ', System.Convert.ToInt32(contentLength - transferredBytes));
                            p_Context.Response.Write(fillEmpty);

                        }

                        //if servlet container did not set content length, we were not flushing and we have non-binary content avoid chunked transfer by setting actual content-length (given no flush)                       
                        if (    !isBinary &&   
                                contentLength == 0 &&  
                                BonCodeAJP13Settings.BONCODEAJP13_AUTOFLUSHDETECTION_BYTES == 0 && 
                                BonCodeAJP13Settings.BONCODEAJP13_AUTOFLUSHDETECTION_TICKS == 0 && 
                                transferredBytes > 0)
                        {
                            try
                            {
                                p_Context.Response.AddHeader("Content-Length", transferredBytes.ToString());
                            } catch (Exception e)
                            {
                                RecordSysEvent("Error missing final content-length: " + e.Message, EventLogEntryType.Error);
                            }
                        }
                        
                    }
                    else if (flushPacket is TomcatPhysicalPathRequest)
                    {
                        //do nothing here Adobe introduced this package, other parts of code respond to this packet when it is detected
                    }

                    else if (flushPacket is TomcatSendBodyChunk)
                    {
                        transferredBytes = transferredBytes + flushPacket.Length;
                        if (flushPacket.Length > 0)
                            p_Context.Response.BinaryWrite(flushPacket.GetUserDataBytes());

                    }
                }
                catch (Exception e)
                {
                    //display error
                    RecordSysEvent("Error writing headers: " + e.Message, EventLogEntryType.Error);
                    PrintError(p_Context, ".", e.Message + " " + e.StackTrace);

                }

            } //loop over packets

            //attempt to flush now
            try
            { 
                //send contents to browser
                p_Context.Response.Flush();
            }
            catch (Exception e)
            {
                //do nothing. Mostly this occurs if the browser already closed connection with server or headers were already transferred 
                RecordSysEvent("Error during spool to client (browser may have navigated away): " + e.Message + " " + e.StackTrace, EventLogEntryType.Warning);
                //we don't need to printerror as the client is gone already
                //PrintError(p_Context, "", e.Message + " " + e.StackTrace);
            }

            //remove the flush collection reference 
            flushCollection = null;

            //signal that we have finished the flush process
            p_FlushInProgress = false;
        } //end print flush


        /// <summary>
        /// If we recognize certain headers that IIS supports we will write them into the response stream using IIS notation as well.        
        /// </summary> 
        private void IISNativeHeaders(string headerName, string headerValue) {
            try
            {
                //switch block
                switch (headerName)
                {
                    case "Location":
                    case "Content-Location":
                        //in cases where we are restricted from writing status codes we will do a server side redirect when we detect the right headers
                        if (!BonCodeAJP13Settings.BONCODEAJP13_ENABLE_HTTPSTATUSCODES)
                        {
                            p_Context.Response.Redirect(headerValue);
                        }
                        break;
                }
            } catch (Exception e)
            {
                RecordSysEvent("IISNativeHeaders setting: " + e.Message, EventLogEntryType.Warning);
            } 
        }

        /// <summary>
        /// Test whether the user content is expected to be binary or text.
        /// Returns true if binary, false if text.
        /// DEPRICATED: WE ALWAYS ASSUME BINARY AND DO NOT CHANGE ENCODING
        /// </summary>           
        private bool TestBinary(string contentType)
        {
            bool retVal = true;
            //if we have compression we do not need to evaluate further, assume binary
            if (!p_Context.Response.ContentEncoding.EncodingName.Contains("gzip"))
            {
                contentType = contentType.ToLower();
                for (int i = 0; i < BonCodeAJP13Settings.BONCODEAJP13_TEXT_MARK.Length; i++)
                {
                    //if we find a text identifier in the content type we return 
                    if (contentType.Contains(BonCodeAJP13Settings.BONCODEAJP13_TEXT_MARK[i]))
                    {
                        retVal = false;
                        break;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Determine if local call and print error on screen.
        /// PublicError will be displayed to all users
        /// LocalError will be displayed to local users (such as stack trace)
        /// TODO: This will be extended in the future to log errors to file.
        /// </summary>
        private void PrintError(HttpContext context,String strPublicErr=".", String strLocalErr="")
        {
            string strErrorCode = "502"; //we use the Bad Gateway HTTP error code to indicate that we had a connection issue with Tomcat, in the future an error code could be passed in.
            string strBindChar = "?"; // url parameter seperator is either question mark or ampersand

            // mark this thread as to be killed since it produced error
            p_FlagKillConnection = true;
            
            //set a constant for public error if we used period in argument
            if (strPublicErr == ".")
            {
                strPublicErr = "Generic Connector Communication Error: <hr>Please check and adjust your setup:<br>Ensure that Tomcat is running on given host and port.<br>If this is a timeout error consider adjusting IIS timeout by changing executionTimeout attribute in web.config (see manual).";
            }

            //we will need to redirect to alternate URL if we have connect error URL setting defined -- we will add an errorcode and detail
            if (BonCodeAJP13Settings.BONCODEAJP13_TOMCAT_DOWN_URL.Length > 5)
            {
                
                //does the connect error URL contain URL parameters already change the bind character
                if (BonCodeAJP13Settings.BONCODEAJP13_TOMCAT_DOWN_URL.IndexOf('?') >= 0)
                {
                    strBindChar = "&";
                }
                //create composite error data for URL                
                if (IsLocalIP(GetKeyValue(context.Request.ServerVariables, "REMOTE_ADDR")))
                {
                    strPublicErr = strPublicErr + "-" + strLocalErr;
                    //truncate error message to 1200 characters for now, this will grow a bit with encoding but we want to be below 2000 characters as many gateways restrict URL parameters
                    strPublicErr = HttpUtility.UrlPathEncode(strPublicErr.Substring(0, Math.Min(strPublicErr.Length, 1199))); 
                    //create fully formed URL
                    string strFullUrl = BonCodeAJP13Settings.BONCODEAJP13_TOMCAT_DOWN_URL + strBindChar + "errorcode=" + strErrorCode + "&detail=" + strPublicErr ;
                    //redirect to fully formed ULR
                    context.Response.Redirect(strFullUrl);
                }

            }
            else
            {
                //we have no redirect setting defined --> request we will output error code 502 to client
                try
                {
                    context.Response.TrySkipIisCustomErrors = true;
                    context.Response.Write(strPublicErr);
                    if (IsLocalIP(GetKeyValue(context.Request.ServerVariables, "REMOTE_ADDR")))
                    {
                        context.Response.Write("<br><pre>" + strLocalErr + "</pre>");
                    }

                    //we might get an error during HTTP status code change when flushing is enabled or headers have already been sent
                    try
                    {                        
                        context.Response.StatusCode = 502;
                    }
                    catch(Exception exp )
                    {
                        RecordSysEvent("PrintError setting Statuscode: " + exp.Message, EventLogEntryType.Warning);
                    };


                } catch (Exception e)
                {
                    RecordSysEvent("PrintError failed: " + e.Message, EventLogEntryType.Error);
                }
            }


        }


        /// <summary>
        /// Logic that checks whether a request should be run, this is before we pass it to tomcat.
        /// all custom logic should be placed into this method
        /// </summary>
        private string CheckExecution(HttpContext context)
        {
            
            NameValueCollection httpHeaders = context.Request.ServerVariables;
            NameValueCollection queryParams = context.Request.QueryString;
            string retVal = "";
            try
            {
                //check for remote administrator use
                //----------------------------------
                if (!BonCodeAJP13Settings.BONCODEAJP13_ENABLE_REMOTE_MANAGER)
                {
                   
                    string uriPath = GetKeyValue(httpHeaders, "SCRIPT_NAME").ToLowerInvariant();
                    //not local call
                    if (uriPath != null && !IsLocalIP(GetKeyValue(httpHeaders, "REMOTE_ADDR")))
                    {

                        //check whether we have funky characters in URL this indicates a canonicalization attempt to obfuscate call
                        if (context.Request.Path.IndexOf('\\') >= 0)
                        {
                            retVal = "Access from remote not allowed (0). Use standard Url format.";
                        };

                        //determine whether we are trying to access an admin URL
                        for (int i = 0; i < BonCodeAJP13Settings.BONCODEAJP13_MANAGER_URLS.Length; i++)
                        {
                            if (uriPath.Length >= BonCodeAJP13Settings.BONCODEAJP13_MANAGER_URLS[i].Length)
                            {
                                //check for starting URi pattern
                                if (uriPath.Substring(0, BonCodeAJP13Settings.BONCODEAJP13_MANAGER_URLS[i].Length).ToLower() == BonCodeAJP13Settings.BONCODEAJP13_MANAGER_URLS[i])
                                {
                                    retVal = "Access from remote not allowed (1).";
                                    break;
                                }                                

                            }                            
                        } // end of fixed check

                        if (retVal.Length == 0)
                        {
                            for (int i = 0; i < BonCodeAJP13Settings.BONCODEAJP13_MANAGER_FLEXURLS.Length; i++)
                            {
                                if (uriPath.Length >= BonCodeAJP13Settings.BONCODEAJP13_MANAGER_FLEXURLS[i].Length)
                                {
                                    //check for any part URi pattern
                                    if (uriPath.ToLower().IndexOf(BonCodeAJP13Settings.BONCODEAJP13_MANAGER_FLEXURLS[i]) != -1)
                                    {
                                        retVal = "Access from remote not allowed (2).";
                                        break;
                                    }

                                }
                            }
                        } // end of flex check
                    }
                }
                //end of check for remote administrator use
                //----------------------------------

                //check for version request
                //----------------------------------
                if (queryParams["BonCodeConnectorVersion"] != null )
                {                    
                    //has to be local call
                    if (IsLocalIP(GetKeyValue(httpHeaders, "REMOTE_ADDR")))
                    {
                        string usePath = BonCodeAJP13Logger.GetAssemblyDirectory() + "\\BonCodeAJP13.settings";
                        retVal += "BonCodeAJP Connector Version " + BonCodeAJP13Consts.BONCODEAJP13_CONNECTOR_VERSION + "<br>";
                        if (System.IO.File.Exists(usePath)) {
                            retVal = retVal + "<small>using setting file in " + usePath + "</small>";
                        } else {
                              retVal = retVal + "<small>using generated defaults. No setting file in " + usePath + "</small>" ;
                        }

                   
                    }

                }

                //end of check for version request
                //----------------------------------

                //check for virtual directory list (can be used only when header data support is enabled)
                //----------------------------------
                if (queryParams["BonCodeVDirList"] != null)
                {
                    //has to be local call
                    if (IsLocalIP(GetKeyValue(httpHeaders, "REMOTE_ADDR")))
                    {
                        UInt32 siteInstanceId = 0;
                        String vpaths = ";";
                        
                        try
                        {
                            //get instance
                            siteInstanceId = System.Convert.ToUInt32(GetKeyValue(httpHeaders, "INSTANCE_ID"));

                            //calling function to start retrieval of data or get from specialized class
                            vpaths = GetVDirs();
                        } catch (Exception err) {
                            //we are only catching this in case there is an issue with instanceId or VDirs stuff
                            //instanceId of zero indicates trouble
                            RecordSysEvent("Cannot determine siteInstanceId): " + err.Message, EventLogEntryType.Warning);

                        }
               
                        //for display
                        if (vpaths == ";")
                        {
                            retVal += "no virtual directories found.";
                        }
                        else if (vpaths == "err")
                        {
                            retVal += "The handler does not have needed privilidges to retrieve virtual directories.<br>";
                            retVal += "Change Application Pool Identity to Local/System or set up read permissions (see manual).<br>";
                        } 
                        else
                        {
                            String[] dispPaths = vpaths.Split(new char[] { ';' });
                            retVal += "<pre>" + dispPaths.Length.ToString() +" virtual directories for site (" + siteInstanceId.ToString() + ") <br>";
                            retVal += "-----------------------------------------------<br>";
                            retVal += String.Join("<br>", dispPaths).Replace(",", " ==> ") + "<br>";
                            retVal += "</pre>";                  
                        }                   


                    }

                }

                //end of check for virtual directory list
                //----------------------------------


                //check for check for event log source registration
                //----------------------------------
                if (queryParams["BonCodeEventLogPrep"] != null)
                {
                       
                    if (RegisterWindowsEventSource())
                    {
                        retVal += "<br>Windows Application Event log preparation succeeded. Please filter for 'BonCodeConnector' or EventID 417 events.";
                    } else
                    {
                        retVal += "<br>Cannot access Windows Application Event log. Please change the Application Pool Identity to LocalSystem and retry. Once successfull, you can revert the Application pool identity.";
                    }
             
                }

                //end of check for event log source registration
                //----------------------------------



            }
            catch (Exception e)
            {
                //mark exception
                retVal += ":exception - run locally for detailed message <br>";
                //return stack trace if local call
                if (IsLocalIP(GetKeyValue(httpHeaders, "REMOTE_ADDR"))) {
                    retVal += "<br>---<br>" + e.Message + "<br>---<br>";
                    retVal += e.StackTrace;
                }
                RecordSysEvent("Check Execution sequence error: " + e.Message, EventLogEntryType.Error);
            }


            return retVal;
        }

        //used for testing
        private string GetHeaders(NameValueCollection httpHeaders)
        {
                        
            string strReturn = "";


            string keyName = "";
            string keyValue = "";
            for (int i = 0; i < httpHeaders.AllKeys.Length; i++)  //for (int i = 0; i < httpHeaders.AllKeys.Length; i++)
            {
                keyName = httpHeaders.AllKeys[i];
                keyValue = httpHeaders[keyName];
                //only process if this key is not on the skip key list
                strReturn = strReturn + "<b>" + i + ".</b> Key " + keyName  + ": " + HttpUtility.HtmlEncode(keyValue) + "<br>";
     
            }

            return strReturn + "<hr noshade>";
        }


        /// <summary>
        /// Determine whether a given address is local to this machine  
        /// </summary>
        private static bool IsLocalIP(string hostName)
        {
            bool retVal = false;
            try
            {   
                // get references to this machine (self)
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
                // get references to the provided hostName
                IPAddress[] publicIPs = Dns.GetHostAddresses(hostName);

                // test whether the given hostName matches to any of the machine references
                foreach (IPAddress hostIP in publicIPs)
                {
                    //  localhost test
                    if (IPAddress.IsLoopback(hostIP))
                    {
                        retVal = true;
                        break;
                    }
                    // iterater through localIPs and check whether it matches one of the hostName IPs
                    foreach (IPAddress localIP in localIPs)
                    {
                        if (hostIP.Equals(localIP))
                        {
                            retVal = true;
                            break;
                        }

                    }
                }
            }
            catch (Exception) 
            { //do nothing, assume we do not match
            }
            return retVal;
        }



        /// <summary>
        /// Returns the zero node string of a key existing in the collection. Return null if key is not defined.   
        /// </summary>
        private string GetKeyValue(NameValueCollection httpHeaders, string keyName)
        {
            string retVal = null;
            if (httpHeaders[keyName] != null)
            {
                string[] keyValues = httpHeaders.GetValues(keyName);
                retVal = keyValues[0];
            }

            return retVal;
        }

        /// <summary>
        /// Kills TCP/IP connection to tomcat.   
        /// </summary>
        private void KillConnection()
        {
            try
            {
                if (p_TcpClient != null)
                {
                    //only closing client -- stream should already be closed
                    p_TcpClient.Client.Close();
                    p_TcpClient.Close();
                    //p_TcpClient.Client.Shutdown(SocketShutdown.Both);                    
                    p_TcpClient = null;
                }
                
            }
            catch (Exception exp)
            {        
                //attempt to add to Application log        
                RecordSysEvent("Error during KillConnection: " + exp.Message,EventLogEntryType.Error);
            }
        }


        /// <summary>
        /// Use http client cert and return x509 representation
        /// </summary>
        /// <param name="context">The HTTP context</param>
        /// <returns>X509 embedded client cert</returns>
        private static X509Certificate GetClientCert(HttpContext context)
        {
            X509Certificate cert = null;
            if (context.Request.IsSecureConnection)
            {
                if (context.Request.ClientCertificate.IsPresent) {
                    cert = new X509Certificate(context.Request.ClientCertificate.Certificate);                    
                }
            }

            return cert;
        }


        /// <summary>
        /// Check for override on REMOTE_ADDR and return the value from designated header instead.  
        /// Intermediaries such as Proxy Servers and Load Balancers hide the REMOTE_ADDR but add alternate headers.
        /// Most likely the HTTP_X_FORWARDED_FOR header is used for this.
        /// A similar function is in class: BonCodeAJP13ForwardRequest
        /// </summary>
        private string GetRemoteAddr(NameValueCollection httpHeaders)
        {
            string retVal = GetKeyValue(httpHeaders, "REMOTE_ADDR");
            //HTTP_X_FORWARDED_FOR
            if (BonCodeAJP13Settings.BONCODEAJP13_REMOTEADDR_FROM != "")
            {
                try
                {
                    string tempVal = GetKeyValue(httpHeaders, BonCodeAJP13Settings.BONCODEAJP13_REMOTEADDR_FROM);
                    if (tempVal != "") retVal = tempVal.Split(new char[] { ',' })[0];
                    //TODO: interate through and find left most non-private address
                    //(Left(testIP,3) NEQ "10.")  AND   (Left(testIP,7) NEQ "172.16.")   AND   (Left(testIP,8) NEQ "192.168.")   
                }
                catch
                {
                    //we will not return an alternate value in case of error
                }
            }

            return retVal;
        }

        /// <summary>
        /// Register Event Log Source  - we need this prior to logging events in System Application log
        /// We will also need to temporarily change the application pool identity to NT AUTHORITY\SYSTEM
        /// </summary>
        private bool RegisterWindowsEventSource()
        {
            try
            {
                string sSource;
                string sLog;              
                sSource = "BonCodeConnector";
                sLog = "Application";
                //create event source
                if (!EventLog.SourceExists(sSource))
                {
                    EventLog.CreateEventSource(sSource, sLog);
                }

                return true;
            } catch
            {

                return false;
            }
        } 


        /// <summary>
        /// Record an event in system in Application event log  
        /// </summary>
        private void RecordSysEvent(string message, EventLogEntryType eType=EventLogEntryType.Information) {
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
                    EventLog.WriteEntry(sSource, sEvent, eType, 417);
                } catch
                {
                    //do nothing for now
                }

            }
        }

        /// <summary>
        /// Get virtual directory mapping information  
        /// </summary>
        private String GetVDirs()
        {
            
            //get from cache or retrieve from system
            String vpaths = GetSavedData("vpaths");
            if (vpaths == "")
            {
                //we have not retrieved the virtual path yet retrieve and save
                vpaths = BonCodeIIS.WebManagement.GetVirtualDirectories();                
                SaveData("vpaths", vpaths);
            }

            return vpaths;        
        }


        /// <summary>
        /// Save Data (String) in Application Memory   
        /// </summary>
        private void SaveData(string keyName, string keyValue)
        {
            System.Web.HttpContext.Current.Application.Lock();
            System.Web.HttpContext.Current.Application[keyName] = keyValue;
            System.Web.HttpContext.Current.Application.UnLock();
        }


        /// <summary>
        /// Get Saved Data (String) from Application Memory   
        /// </summary>
        private string GetSavedData(string keyName)
        {
            return Convert.ToString(System.Web.HttpContext.Current.Application[keyName]);
        }


       

    }
}
