/*
 *  Copyright (c) 2011 by Bilal Soylu
 *  Bilal Soylu licenses this file to You under the 
 *  Creative Commons License, Version 3.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *      http://creativecommons.org/licenses/by/3.0/
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
 * Version:     0.9                                                      *
 *************************************************************************/

using System;
using System.Collections.Specialized;
using System.Net.Sockets;
using System.Web;
using BonCodeAJP13;
using BonCodeAJP13.ServerPackets;
using BonCodeAJP13.TomcatPackets;
using System.Net;



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
                p_InstanceCount--;
            }
            catch (Exception)
            {
                //do nothing for now
            }
      
        }


        #endregion

        #region constructor
        /// <summary>
        /// Constructor will maintain instance count
        /// </summary>
        public BonCodeCallHandler()
        {
            p_InstanceCount++;
        }

        #endregion

        /// <summary>
        /// Declaring ourselfes as reusable, so threads can be pooled
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

            
            //check execution
            string executionFeedback = CheckExecution(context.Request.ServerVariables);
            bool blnProceed = true;

            /* debug: dump headers
            string strOut = GetHeaders(context.Request.ServerVariables);
            context.Response.Write(strOut);
            */

            if (executionFeedback.Length == 0)
            {
                //determine web doc root if needed
                if (BonCodeAJP13Settings.BONCODEAJP13_HEADER_SUPPORT)
                {
                    BonCodeAJP13Settings.BonCodeAjp13_DocRoot = System.Web.HttpContext.Current.Server.MapPath("~");
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
                        string errMsg = "Error connecting to Apache Tomcat instance.<hr>Please check that a Tomcat server is running at given location and port.<br>Details:<br>" + e.Message;
                        context.Response.Write(errMsg);
                        blnProceed = false;

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

                if (blnProceed)
                {
                    //initialize AJP13 protocol connection
                    BonCodeAJP13ServerConnection sconn = new BonCodeAJP13ServerConnection();
                    sconn.FlushDelegateFunction = PrintFlush;  //this function will do the transfer to browser if we use Flush detection, we pass as delegate
                    sconn.FlushStatusFunction = IsFlushing; //will let the implementation know if flushing is still in progress
                    sconn.SetTcpClient = p_TcpClient;
                    //setup basic information (base ForwardRequest package)            
                    BonCodeAJP13ForwardRequest FR = new BonCodeAJP13ForwardRequest(context.Request.ServerVariables);
                    sconn.AddPacketToSendQueue(FR);

                    //determine if extra ForwardRequests are needed. 
                    //We need to create a collection of Requests (for form data and file uploads etc.) 
                    if (context.Request.ContentLength > 0)
                    {
                        // need to create a collection of forward requests to package data in                               
                        int numOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(context.Request.ContentLength / Convert.ToDouble(BonCodeAJP13Consts.MAX_BONCODEAJP13_USERDATA_LENGTH))));
                        int iStart = 0;
                        int iCount = 0;
                        for (int i = 1; i <= numOfPackets; i++)
                        {
                            //we need to breakdown data into multiple FR packages to tomcat
                            if (i * BonCodeAJP13Consts.MAX_BONCODEAJP13_USERDATA_LENGTH <= streamLen)
                            {
                                //we are in the middle of transferring data grab next 8188 bytes and create package
                                iStart = (i - 1) * BonCodeAJP13Consts.MAX_BONCODEAJP13_USERDATA_LENGTH;
                                iCount = Convert.ToInt32(BonCodeAJP13Consts.MAX_BONCODEAJP13_USERDATA_LENGTH);
                            }
                            else
                            {
                                //last user package
                                iStart = (i - 1) * BonCodeAJP13Consts.MAX_BONCODEAJP13_USERDATA_LENGTH;
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
                    sconn.BeginConnection();

                    //write the response to browser (if not already flushed)
                    PrintFlush(sconn.ReceivedDataCollection);

                    //kill connections if we are not reusing connections
                    if (p_FlagKillConnection)
                    {
                        KillConnection();
                    }
                }; // proceed is true

            }
            else
            {
                //execution was denied by logic, only print message
                context.Response.Write(executionFeedback);

            }
            

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
                                catch (Exception) {
                                    contentLength = 0;
                                };


                            }


                            //check whether we can represent a given header in native IIS Response context
                            IISNativeHeaders(keyName, tempValue);


                        }
                        //set response status code
                        p_Context.Response.StatusCode = tcshPackage.GetStatus();

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

                    }
                    else if (flushPacket is TomcatSendBodyChunk)
                    {
                        transferredBytes = transferredBytes + flushPacket.GetUserDataBytes().Length;
                        p_Context.Response.BinaryWrite(flushPacket.GetUserDataBytes()); 
                        
                    }
                }
                catch (Exception)
                {
                    //TODO: add exception handler logging                    
                    p_Context.Response.Write("Error in transfer of data from tomcat to browser.");

                }

            } //loop over packets

            //attempt to flush now
            try
            {
                p_Context.Response.Flush();
            }
            catch (Exception)
            {
                //do nothing. Mostly this occurs if the browser already closed connection with server or headers were already transferred
                bool errFlag = true;
            }

            p_FlushInProgress = false;
        } //end print flush


        /// <summary>
        /// If we recognize certain headers that IIS supports we will write them into the response stream using IIS notation as well.        /// 
        /// </summary> 
        private void IISNativeHeaders(string headerName, string headerValue) { 
            //switch blocked
            switch (headerName)
            {
                case "Location": case "Content-Location":
                    p_Context.Response.Redirect(headerValue);
                    break;               
                    
            }
                    

        }

        /// <summary>
        /// Test whether the user content is expected to be binary or text.
        /// Returns true if binary, false if text.
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
        /// Logic that checks whether a request should be run, this is before we pass it to tomcat.
        /// all custom logic should be placed into this method
        /// </summary>
        private string CheckExecution(NameValueCollection httpHeaders)
        {
            string retVal = "";
            try
            {
                //check for remote administrator use
                //----------------------------------
                if (!BonCodeAJP13Settings.BONCODEAJP13_ENABLE_REMOTE_MANAGER)
                {
                    string uriPath = GetKeyValue(httpHeaders, "SCRIPT_NAME");
                    //not local call
                    if (uriPath != null && !IsLocalIP(GetKeyValue(httpHeaders, "REMOTE_ADDR")))
                    {
                        //determine whether we are trying to access an admin URL
                        for (int i = 0; i < BonCodeAJP13Settings.BONCODEAJP13_MANAGER_URLS.Length; i++)
                        {
                            if (uriPath.Length >= BonCodeAJP13Settings.BONCODEAJP13_MANAGER_URLS[i].Length &&
                                uriPath.Substring(0,BonCodeAJP13Settings.BONCODEAJP13_MANAGER_URLS[i].Length).ToLower() == BonCodeAJP13Settings.BONCODEAJP13_MANAGER_URLS[i] )
                            {
                                retVal = "Access from remote not allowed.";
                                break;
                            }
                        }
                    }

                }
                //end of check for remote administrator use
                //----------------------------------

            }
            catch (Exception)
            {
                //do nothing
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
                    //stream
                    p_TcpClient.GetStream().Flush();
                    p_TcpClient.GetStream().Dispose();
                    p_TcpClient.GetStream().Close();                   
                    //client
                    p_TcpClient.Client.Close();
                    p_TcpClient.Close();
                    p_TcpClient.Client.Shutdown(SocketShutdown.Both);
                    p_TcpClient = null;
                }
                
            }
            catch (Exception)
            {
                //do nothing for now
            }
        }


    }
}
