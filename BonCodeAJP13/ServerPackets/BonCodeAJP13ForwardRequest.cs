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
using System.Collections;
using System.Collections.Specialized;
using System.Linq;

namespace BonCodeAJP13.ServerPackets
{
    /// <summary>
    /// This is the implementation of a Forward Request Package
    /// A server Forward Request package initiates the conversation from Webserver to Tomcat
    /// </summary>
    public class BonCodeAJP13ForwardRequest : BonCodeAJP13Packet
    {
        #region data Members
        //set packet name overrides base class
        new const string p_PACKET_DESCRIPTION = "FORWARD REQUEST";
        private Hashtable p_RawHeadersTranlator = null;

        #endregion

        //additional properties from base class if any
        #region Properties
        #endregion


        //contructors specific to this type of packet
        #region Constructors

        /// <summary>
        /// Generic Constructor can be used to create empty package       
        /// </summary>
        public BonCodeAJP13ForwardRequest() {}

        /// <summary>
        /// Creates a Body only Forward Request Package. This is normally a follow on to 
        /// an initial request that has all the header data. We can only send 8186 bytes at a time.
        /// </summary>
        public BonCodeAJP13ForwardRequest(byte[] content) {

            if (content.Length >= 0 && content.Length <= BonCodeAJP13Settings.MAX_BONCODEAJP13_USERDATA_LENGTH)
            {
                WritePacket(content);
            }
            else
            {
                throw new Exception("Invalid BonCodeAJP13ForwardRequest content received. New content cannot exceed " + BonCodeAJP13Settings.MAX_BONCODEAJP13_USERDATA_LENGTH + " bytes. Provided content length is " + content.Length + " bytes.");
            }

        
        }

        /// <summary>
        /// Constructor without HTTP Headers. Used for testing URL calls only.  Will not transfer Query-String.      
        /// </summary>
        public BonCodeAJP13ForwardRequest(  byte method,
                                            string protocol,
                                            string req_uri,
                                            string remote_addr,
                                            string remote_host,
                                            string server_name,
                                            ushort server_port,
                                            bool is_ssl,
                                            int num_headers=1 )
        {


                     

            //allways override headers to be one. The test packet will have content-length header set to zero.
            num_headers = 1; 

            //call actual writing of forward request packet. It will be stored in in p_ByteStore instance var
            WritePacketTest(method, protocol, req_uri, remote_addr, remote_host, server_name, server_port, is_ssl, num_headers);

            
        }

        /// <summary>
        /// Constructor with HTTP Headers       
        /// </summary>
        public BonCodeAJP13ForwardRequest(NameValueCollection httpHeaders)
        {

            WritePacket(httpHeaders,"");

        }

        /// <summary>
        /// Constructor with HTTP Headers and PathInfo     
        /// </summary>
        public BonCodeAJP13ForwardRequest(NameValueCollection httpHeaders, String pathInfo)
        {

            WritePacket(httpHeaders,pathInfo,0);

        }

        /// <summary>
        /// Constructor with HTTP Headers, PathInfo, and Source Port
        /// </summary>
        public BonCodeAJP13ForwardRequest(NameValueCollection httpHeaders, String pathInfo, int sourcePort)
        {

            WritePacket(httpHeaders, pathInfo, sourcePort);

        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates forward request package based on byte array. This is normally a follow on package to initial forward request.       
        /// </summary>
        private void WritePacket(byte[] transferContent)
        {
            if (transferContent.Length > 0 && transferContent.Length <= BonCodeAJP13Settings.MAX_BONCODEAJP13_USERDATA_LENGTH)
            {
                p_ByteStore = new byte[transferContent.Length + 6];
                int pos = 2;
                p_ByteStore[0] = BonCodeAJP13Markers.BONCODEAJP13_PACKET_START;
                p_ByteStore[1] = BonCodeAJP13Markers.BONCODEAJP13_PACKET_START2;
                pos = SetInt16(p_ByteStore, Convert.ToUInt16(transferContent.Length + 2), pos); //overall length
                pos = SetInt16(p_ByteStore, Convert.ToUInt16(transferContent.Length), pos); //user data length
                pos = SetSimpleByteArray(p_ByteStore, transferContent, pos);
                p_PacketLength = p_ByteStore.Length;
            }
            else if (transferContent.Length == 0)
            {
                //create empty package, this has four bytes (acts as string terminator)
                p_ByteStore = new byte[4];
                p_ByteStore[0] = BonCodeAJP13Markers.BONCODEAJP13_PACKET_START;
                p_ByteStore[1] = BonCodeAJP13Markers.BONCODEAJP13_PACKET_START2;
                p_ByteStore[2] = 0x00;
                p_ByteStore[3] = 0x00;
                p_PacketLength = 4;                
            };
            
           
          
        }


        /// <summary>
        /// Creates forward request package based on header data       
        /// </summary>
        private void WritePacket(NameValueCollection httpHeaders, String pathInfo, int sourcePort=0)
        {
            //set defaults first
            bool is_ssl = false;
            
            //set values from header information
            string protocol = GetKeyValue(httpHeaders, "SERVER_PROTOCOL");   // "HTTP/1.1"            
            int num_headers = httpHeaders.AllKeys.Length; // -lstSystemBlacklist.Length;
            byte method = BonCodeAJP13PacketHeaders.GetMethodByte(GetKeyValue(httpHeaders, "REQUEST_METHOD"));
            string req_uri = GetKeyValue(httpHeaders, "SCRIPT_NAME");
            string remote_addr = GetRemoteAddr(httpHeaders);  // GetKeyValue(httpHeaders, "REMOTE_ADDR");  GetRemoteAddr(httpHeaders);
            string remote_host = GetKeyValue(httpHeaders, "REMOTE_HOST");
            string server_name = GetKeyValue(httpHeaders, "HTTP_HOST"); //BonCodeAJP13Settings.BONCODEAJP13_SERVER;
            ushort server_port = System.Convert.ToUInt16(GetKeyValue(httpHeaders, "SERVER_PORT"));   // System.Convert.ToUInt16(BonCodeAJP13Settings.BONCODEAJP13_PORT);

            //check whether ssl is on
            string sslCheck = GetKeyValue(httpHeaders, "HTTPS");
            if (sslCheck != null && sslCheck == "on") {
                is_ssl = true;
            }
         

            //call alternate method to complete writing of forward request packet. Final data will be stored in in p_ByteStore instance var
            WritePacket(method, protocol, req_uri, remote_addr, remote_host, server_name, server_port, is_ssl, num_headers, httpHeaders, pathInfo, sourcePort);
        }



        /// <summary>
        /// Creates actual forward request package and stores in p_ByteStore of the instance       
        /// </summary>
        private void WritePacket(byte method,
                            string protocol,
                            string req_uri,
                            string remote_addr,
                            string remote_host,
                            string server_name,
                            ushort server_port,
                            bool is_ssl,
                            int num_headers,
                            NameValueCollection httpHeaders,
                            String realPathInfo ="",
                            int sourcePort=0)
        {
            //locals
            int pos = 0;
            byte attributeByte = 0x00;
            byte[] aUserData = new byte[BonCodeAJP13Settings.MAX_BONCODEAJP13_PACKET_LENGTH]; //allocate full number of bytes for processing
            int packetFillBytes = 14; //bytes used to complete package
            int expectedPacketSize = 0;

            NameValueCollection goodHeaders = CheckHeaders(httpHeaders); //determine headers to be transferred
            num_headers = goodHeaders.AllKeys.Length; //we will allways send the path info 
            PopulateRawHeaders(httpHeaders["ALL_RAW"]);

            
            //add one more header if setting enable setting is used
            if (BonCodeAJP13Settings.BONCODEAJP13_HEADER_SUPPORT ) num_headers++;    
            //path info in alternate header
            if (BonCodeAJP13Settings.BONCODEAJP13_PATHINFO_HEADER != "") num_headers++;    

  
            //add a mapping prefix if one is provided
            if (BonCodeAJP13Settings.BONCODEAJP13_PATH_PREFIX.Length > 2)
            {
                req_uri = BonCodeAJP13Settings.BONCODEAJP13_PATH_PREFIX + req_uri;
            }

            //write a packet
            // ============================================================
            pos = SetByte(aUserData, BonCodeAJP13ServerPacketType.SERVER_FORWARD_REQUEST, pos); // all have to start with this            
            pos = SetByte(aUserData, method, pos);  //method: e.g. we have clicked on URL
            pos = SetString(aUserData, protocol, pos); //protocol
            pos = SetString(aUserData, req_uri, pos); //uri
            pos = SetString(aUserData, remote_addr, pos); //remote addr
            pos = SetString(aUserData, remote_host, pos); //remote host
            pos = SetString(aUserData, server_name, pos); //server name
            pos = SetInt16(aUserData, server_port, pos); //port
            pos = SetByte(aUserData, Convert.ToByte(is_ssl), pos); //is ssl
            pos = SetInt16(aUserData, System.Convert.ToUInt16(num_headers), pos); //number of headers
            //pos = SetInt16(aUserData, System.Convert.ToUInt16(goodHeaders.AllKeys.Length), pos); //number of headers
            //iterate through headers and add to packet
            string keyName = "";
            string keyValue = "";
            
            //add optional headers            
            if (BonCodeAJP13Settings.BONCODEAJP13_HEADER_SUPPORT)
            {
                keyName = "x-tomcat-docroot"; //"X-Tomcat-DocRoot";                
                keyValue = BonCodeAJP13Settings.BonCodeAjp13_DocRoot; // System.Web.HttpContext.Current.Server.MapPath("~"); alternatly we could use "appl-physical-path" http var
                pos = SetString(aUserData, keyName.ToLower(), pos);                
                pos = SetString(aUserData, keyValue, pos);

            }
            //path info alternate header determination            
            if (BonCodeAJP13Settings.BONCODEAJP13_PATHINFO_HEADER != "") {
                keyName = BonCodeAJP13Settings.BONCODEAJP13_PATHINFO_HEADER; //"xajp-path-info";                                
                keyValue = realPathInfo; // httpHeaders["PATH_INFO"];
                pos = SetString(aUserData, keyName.ToLower(), pos);
                pos = SetString(aUserData, keyValue, pos);                
            }
             
            //TODO Remove this    
            /*
            keyName = "xajp-setting-drive"; 
            keyValue = BonCodeAJP13Logger.GetAssemblyDirectory();
            pos = SetString(aUserData, keyName.ToLower(), pos);
            pos = SetString(aUserData, keyValue, pos);

            keyName = "xajp-setting-file";
            keyValue = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            pos = SetString(aUserData, keyName.ToLower(), pos);
            pos = SetString(aUserData, keyValue, pos);
             */
            //END REMOVE THIS


            //all other headers
            for (int i = 0; i < goodHeaders.AllKeys.Length; i++)            
            {
                keyName = goodHeaders.AllKeys[i];
                keyValue = goodHeaders[keyName];                
                expectedPacketSize = keyName.Length + keyValue.Length + pos + packetFillBytes;

                if (expectedPacketSize < BonCodeAJP13Settings.MAX_BONCODEAJP13_PACKET_LENGTH)
                {
                    //add byte or string header name               
                    if (BonCodeAJP13PacketHeaders.GetHeaderBytes(keyName) != null)
                    {
                        //byte header
                        pos = SetSimpleByteArray(aUserData, BonCodeAJP13PacketHeaders.GetHeaderBytes(keyName), pos);
                    }
                    else
                    {
                        //string header (remove HTTP prefix this is added by IIS) and change underscore                       
                        pos = SetString(aUserData, GetCorrectHeaderName(keyName), pos);
                    }
                    //add value if keyName is not empty string
                    pos = SetString(aUserData, keyValue, pos);
                }
                else
                {
                    //raise error:
                    throw new Exception("Invalid content length. Last header processed [" + keyName + "]. Please reconfigure BonCode Connector and Apache Tomcat to allow larger transfer packets. Your max allowed content length is " + BonCodeAJP13Settings.MAX_BONCODEAJP13_USERDATA_LENGTH + " bytes. Provided content length would be at least " + expectedPacketSize + " bytes. Clearing cookies may allow you proceed.");

                }
               
                
            }


            //ATTRIBUTES FOLLOW: Second iteration through headers Some header values have to be passed as attributes REMOTE_USER, AUTH_TYPE, QUERY_STRING

            for (int i = 0; i < httpHeaders.AllKeys.Length; i++)
            {
                keyName = httpHeaders.AllKeys[i];
                keyValue = httpHeaders[keyName];
                expectedPacketSize = keyName.Length + keyValue.Length + pos + packetFillBytes;
                attributeByte = BonCodeAJP13PacketHeaders.GetAttributeByte(keyName);
                //check whether this is byte attribute if so wee need to add the header as attribute to packet
                if (attributeByte != 0x00 && keyValue != "")
                {
                    if (expectedPacketSize < BonCodeAJP13Settings.MAX_BONCODEAJP13_PACKET_LENGTH)
                    {
                        pos = SetByte(aUserData, attributeByte, pos); //attribute marker
                        pos = SetString(aUserData, keyValue, pos); //attribute value                    
                    }
                    else
                    {
                        throw new Exception("Invalid content length. Last header processed [" + keyName + "]. Please reconfigure BonCode Connector and Apache Tomcat to allow larger transfer packets. Your max allowed content length is " + BonCodeAJP13Settings.MAX_BONCODEAJP13_USERDATA_LENGTH + " bytes. Provided content length would be at least " + expectedPacketSize + " bytes.");
                    }
                }
            }
            
            //add secure session attribute
            if (BonCodeAJP13Settings.BONCODEAJP13_FORCE_SECURE_SESSION)
            {
                pos = SetByte(aUserData, BonCodeAJP13HTTPAttributes.BONCODEAJP13_SSL_SESSION, pos); //attribute marker
                pos = SetString(aUserData, "on", pos); //attribute value                
            }

            //add constant attribute for AJP13 JVM Route
            /*
            pos = SetByte(aUserData, BonCodeAJP13HTTPAttributes.BONCODEAJP13_JVM_ROUTE, pos); //attribute marker
            pos = SetString(aUserData, BonCodeAJP13Markers.BONCODEAJP13_PROTOCOL_MARKER, pos); //attribute value
            */


            //add pathinfo attempt to bypass tomcat bug
            /*
            pos = SetByte(aUserData, BonCodeAJP13HTTPAttributes.BONCODEAJP13_REQ_ATTRIBUTE, pos); //attribute marker
            pos = SetString(aUserData, "path_info", pos); //attribute name
            pos = SetString(aUserData, httpHeaders["PATH_INFO"], pos); //attribute value    
             */

            //START ADOBE MODIFICATIONS

            //add Adobe ColdFusion 10 specific attributes, this is backwards since this data is already available, adobe chose to
            //transfer again in attributes because of problems with ISAPI constructed connector

            //SERVER_SOFTWARE VIA ATTRIBUTES
            //pos = SetByte(aUserData, 0x0E, pos); //attribute marker for web server
            //pos = SetString(aUserData, httpHeaders["SERVER_SOFTWARE"], pos); //attribute value

            
            //END OF ADOBE MODIFICATIONS

            //Remote port marker seems to be a common element transferred in attributes
            if (sourcePort > 0)
            {        
                pos = SetByte(aUserData, BonCodeAJP13HTTPAttributes.BONCODEAJP13_REQ_ATTRIBUTE, pos); //attribute marker
                pos = SetString(aUserData, "AJP_REMOTE_PORT", pos); //attribute name
                pos = SetString(aUserData, sourcePort.ToString(), pos); //attribute value 
            }

            //add packet terminator
            pos = SetByte(aUserData, 0xFF, pos); //marks the end of user packet

            //assess length of package and type
            int pLength = pos; // true length of user data
            p_UserDataLength = Convert.ToUInt16(pLength);


            //assemble full package now in the final array container of the object
            p_ByteStore = new byte[pos + 4]; // this is the true length as we add magic and length bytes
            int pos2 = 0;
            pos2 = SetByte(p_ByteStore, BonCodeAJP13Markers.BONCODEAJP13_PACKET_START, pos2);
            pos2 = SetByte(p_ByteStore, BonCodeAJP13Markers.BONCODEAJP13_PACKET_START2, pos2);
            pos2 = SetUInt16(p_ByteStore, p_UserDataLength, pos2);
            Array.Copy(aUserData, 0, p_ByteStore, 4, pos); //only copy relevant data values from temporary store            
            //determine overall packet length
            p_PacketLength = p_ByteStore.Length;
        }

        /// <summary>
        /// evaluates the headers passed from webserver and checks whether they can be 
        /// accepted by tomcat as is
        /// </summary>
        public NameValueCollection CheckHeaders(NameValueCollection httpHeaders)
        {
            //locals
            NameValueCollection cleanHeaders = new NameValueCollection();
            string keyName = "";
            string keyValue = "";
            string[] lstSystemBlacklist = new string[] { "PATH_TRANSLATED", "INSTANCE_META_PATH", "APPL_MD_PATH", "AUTH_TYPE", "REMOTE_USER", "REQUEST_METHOD", "REMOTE_ADDR", "REMOTE_HOST", "ALL_HTTP", "ALL_RAW", "QUERY_STRING", "ACCEPT", "ACCEPT_CHARSET", "ACCEPT_ENCODING", "ACCEPT_LANGUAGE", "AUTHORIZATION", "CONNECTION", "HTTP_CONTENT_TYPE", "HTTP_CONTENT_LENGTH", "PRAGMA", "REFERER", "USER_AGENT" };  //list of headers that will be skipped because they are already processed through other means, duplicate, or not needed          
            string[] lstAllowBlank = new string[] { "" };  //send also if blank
            string[] lstUserWhitelist = null; //if we have data here, only these headers will be sent

            //check for whitelist as specified by users
            if (BonCodeAJP13Settings.BONCODEAJP13_WHITELIST_HEADERS.Length > 5)
            {
                lstUserWhitelist = BonCodeAJP13Settings.BONCODEAJP13_WHITELIST_HEADERS.Split(new char[] { ',' });         
            }

            //"HTTP_CONNECTION","CONTENT_LENGTH","HTTP_ACCEPT","HTTP_ACCEPT_ENCODING","HTTP_ACCEPT_LANGUAGE","HTTP_COOKIE","HTTP_HOST","HTTP_USER_AGENT","HTTP_ACCEPT_CHARSET"
            //check for headers that should not be sent based on user settings (assume headers are more than 5 characters
            if ((BonCodeAJP13Settings.BONCODEAJP13_BLACKLIST_HEADERS.Length) > 5)
            {
                string[] lstUserBlacklist = BonCodeAJP13Settings.BONCODEAJP13_BLACKLIST_HEADERS.Split(new char[] {','});
                int lshOriginalSize = lstSystemBlacklist.Length;
                Array.Resize<string>(ref lstSystemBlacklist, lshOriginalSize + lstUserBlacklist.Length);
                Array.Copy(lstUserBlacklist, 0, lstSystemBlacklist, lshOriginalSize, lstUserBlacklist.Length);                
                
            }

            //iterate and ensure rules are met
            for (int i = 0; i < httpHeaders.AllKeys.Length; i++)
            {
                keyName = httpHeaders.AllKeys[i];
                keyValue = httpHeaders[keyName];
                //only process if this key is not on the skip key list
                if (!lstSystemBlacklist.Contains(keyName))
                {
                    //if we have a white list of headers check against it or if not process header
                    if ((lstUserWhitelist == null) || (lstUserWhitelist.Length > 0 && lstUserWhitelist.Contains(keyName))) {

                        //clear keyvalue if key needs to be passed in attributes
                        if (BonCodeAJP13PacketHeaders.GetAttributeByte(keyName) != 0x00)
                        {
                            //skip key if it is one of the known attributes this will be added in attributes section
                            keyName = "";
                            keyValue = "";
                        }

                        //only pass on key if it is populated unless special exeption                    
                        if (BonCodeAJP13Settings.BONCODEAJP13_ALLOW_EMTPY_HEADERS ||  keyValue != "" || lstAllowBlank.Contains(keyName))
                        {
                            if (keyName != "")
                            {
                                cleanHeaders.Add(keyName, keyValue);
                            }
                        }  
                    }
                    
                   
                   
                }//blacklist failure
            }


            return cleanHeaders;
        }

        /// <summary>
        /// Creates test forward request package and stores in p_ByteStore of the object       
        /// </summary>
        private void WritePacketTest(byte method = 0x02,
                            string protocol = "HTTP/1.1",
                            string req_uri = "/",
                            string remote_addr ="::1",
                            string remote_host ="::1",
                            string server_name ="localhost",
                            ushort server_port = 80,
                            bool is_ssl = false,
                            int num_headers=1) 
        {
            //create request in bytes. first create user data
            int pos = 0;
            byte[] aUserData = new byte[BonCodeAJP13Settings.MAX_BONCODEAJP13_PACKET_LENGTH]; //allocate full number of bytes for processing

            //add a mapping prefix if one is provided
            if (BonCodeAJP13Settings.BONCODEAJP13_PATH_PREFIX.Length > 2)
            {
                req_uri = BonCodeAJP13Settings.BONCODEAJP13_PATH_PREFIX.Length + "/" + req_uri;
            }

            // set protocol required data points
            // ============================================================
            pos = SetByte(aUserData, BonCodeAJP13ServerPacketType.SERVER_FORWARD_REQUEST, pos); // all have to start with this            
            pos = SetByte(aUserData, BonCodeAJP13HTTPMethods.BONCODEAJP13_GET, pos);  //method: e.g. we have clicked on URL
            pos = SetString(aUserData, protocol, pos); //protocol
            pos = SetString(aUserData, req_uri, pos); //uri

            pos = SetString(aUserData, remote_addr, pos); //remote addr
            pos = SetString(aUserData, remote_host, pos); //remote host
            pos = SetString(aUserData, server_name, pos); //server name
            pos = SetInt16(aUserData, server_port, pos); //port
            
            pos = SetByte(aUserData, Convert.ToByte(is_ssl), pos); //is ssl
            pos = SetInt16(aUserData, System.Convert.ToUInt16(num_headers), pos); //number of headers
            //add content lenth as the only header. 
            pos = SetByte(aUserData, 0xA0, pos); //header prefix (once per header)
            pos = SetByte(aUserData, BonCodeAJP13HTTPHeaders.BONCODEAJP13_CONTENT_LENGTH, pos); //id of header
            pos = SetString(aUserData, "0", pos); //add length as a string value

            //add packet terminator
            pos = SetByte(aUserData, 0xFF, pos); //marks the end of user packet

            //assess length of package and type
            int pLength = pos; // true length of user data
            p_UserDataLength = Convert.ToUInt16(pLength);


            //assemble full package now in the final array container of the object
            p_ByteStore = new byte[pos + 4]; // this is the true length as we add magic and length bytes
            int pos2 = 0;
            pos2 = SetByte(p_ByteStore, BonCodeAJP13Markers.BONCODEAJP13_PACKET_START, pos2);
            pos2 = SetByte(p_ByteStore, BonCodeAJP13Markers.BONCODEAJP13_PACKET_START2, pos2);
            pos2 = SetUInt16(p_ByteStore, p_UserDataLength, pos2);
            Array.Copy(aUserData, 0, p_ByteStore, 4, pos); //only copy relevant data values from temporary store            
            //determine overall packet length
            p_PacketLength = p_ByteStore.Length;
        }


        /// <summary>
        /// Check whether a key is defined in the Name Value collection      
        /// </summary>
        private bool KeyExists(NameValueCollection httpHeaders, string keyName)
        {
            bool retVal = false;
            if (httpHeaders[keyName] != null)
            {
                retVal = true;
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
        /// Check for override on REMOTE_ADDR and return the value from designated header instead.  
        /// Intermediaries such as Proxy Servers and Load Balancers hide the REMOTE_ADDR but add alternate headers.
        /// Most likely the HTTP_X_FORWARDED_FOR header is used for this.
        /// </summary>
        private string GetRemoteAddr(NameValueCollection httpHeaders)
        {
            string retVal = GetKeyValue(httpHeaders, "REMOTE_ADDR");
            //HTTP_X_FORWARDED_FOR
            if (BonCodeAJP13Settings.BONCODEAJP13_REMOTEADDR_FROM != "") {
                try
                {
                    string tempVal = GetKeyValue(httpHeaders,BonCodeAJP13Settings.BONCODEAJP13_REMOTEADDR_FROM);
                    if (tempVal != "" && tempVal != null) retVal = tempVal.Split(new char[] { ',' })[0];
                    //TODO: interate through and find left most non-private address
                    //(Left(testIP,3) NEQ "10.")  AND   (Left(testIP,7) NEQ "172.16.")   AND   (Left(testIP,8) NEQ "192.168.")   
                } catch  {
                    //we will not return an alternate value in case of error
                }
            }

            return retVal;
        }


        /// <summary>
        /// Populate the p_RawHeadersTranlator hashtable with correct data from Raw Headers
        /// </summary> 
        private  void PopulateRawHeaders(String rawHeaders)
        {
            if (this.p_RawHeadersTranlator == null && rawHeaders.Length > 0)
            {
                string[] lstRawHeaders = rawHeaders.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                
                this.p_RawHeadersTranlator = new Hashtable();
                
                foreach (String rawHeader in lstRawHeaders)
                {
                    string rawKeyName = rawHeader.Split(new char[] { ':' })[0];
                    this.p_RawHeadersTranlator.Add(rawKeyName.ToLower(), rawKeyName);
                }

            }

        }


        /// <summary>
        /// Return a correct case for the HTTP header. IIS upper cases all header names
        /// this may throw of some Java programs. We redo the character case for all headers 
        /// that were initially sent in by browser or client in to their orignal case.
        /// If this is an IIS generated header we will lower case the name and return it with escaped underscores.        
        /// </summary> 
        private string GetCorrectHeaderName(String headerKey)
        {
            
            string retVal = headerKey.ToLower();

            if (retVal.StartsWith("http_")) retVal = retVal.Remove(0, 5); //IIS adds the HTTP_ prefix we need to remove
            retVal = retVal.Replace("_", "-"); //escape underscores to dashes
            if (this.p_RawHeadersTranlator.ContainsKey(retVal))
            {
                //inbound data had this header, use that spelling and casing
                retVal = (string)this.p_RawHeadersTranlator[retVal];
            };
            

            return retVal;
        }

        #endregion

        


    }
}
