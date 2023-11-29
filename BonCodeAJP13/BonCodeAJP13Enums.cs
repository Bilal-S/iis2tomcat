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


namespace BonCodeAJP13
{

    ///////////////////////////////////////////////////////////////////////////////
    ////// BonCodeAJP13 packet structure ...
    //////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// packet special byte definitions, End and Start, the protocol algorithms allow us to use them inside the packet as well.
    /// </summary>
    public struct BonCodeAJP13Markers
    {

        public const byte BONCODEAJP13_PACKET_START = 0x12;
        public const byte BONCODEAJP13_PACKET_START2 = 0x34;
        public const byte BONCODEAJP13_BYTE_HEADER_MARKER = 0xA0;
        public const string BONCODEAJP13_PROTOCOL_MARKER = "ajp13w";        

    }


    /// <summary>
    /// Types of packets from server to tomcat.
    /// </summary>
    public struct BonCodeAJP13ServerPacketType
    {
        //following packet types from server to tomcat
        public const byte SERVER_FORWARD_REQUEST = 0x02;    //Begin the request-processing cycle with the following data
        public const byte SERVER_SHUTDOWN = 0x07;           //The web server asks the container to shut itself down. Only available via localhost
        public const byte SERVER_PING = 0x08;               //The web server asks the container to take control (secure login phase).
        public const byte SERVER_CPING = 0x0A;              //The web server asks the container to respond quickly with a CPong (decimal 9).

        //following packet types from tomcat to server

    }

    /// <summary>
    /// Types of packets from tomcat to server.
    /// </summary>
    public struct BonCodeAJP13TomcatPacketType
    {
        //following packet types from tomcat to server
        public const byte TOMCAT_SENDBODYCHUNK = 0x03;      //Send a chunk of the body from the servlet container to the web server (and onto the browser). 
        public const byte TOMCAT_SENDHEADERS = 0x04;        //Send the response headers from the servlet container to the web server (and presumably, onto the browser).
        public const byte TOMCAT_ENDRESPONSE = 0x05;        //Marks the end of the response (and thus the request-handling cycle).
        public const byte TOMCAT_GETBODYCHUNK = 0x06;       //Get further data from the request if it hasn't all been transferred yet.
        public const byte TOMCAT_CPONGREPLY = 0x09;         //The reply to a CPing request       
        public const byte TOMCAT_CFPATHREQUEST = 0x0F;      //Vendor specific extension. Not part of standard AJP13
    }

   


    ///////////////////////////////////////////////////////////////////////////////
    // BonCodeAJP13 HTTP binary headers, these are the headers we can exchange in binary format
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Headers for which we have a binary format. returns only the second byte, the first byte is allways assumed to be 0xA0
    /// If header is not in the list, it will be exchanged as string
    /// </summary>
    public struct BonCodeAJP13HTTPHeaders
    {

        public const byte BONCODEAJP13_ACCEPT = 0x01;           //accept 0xA001 SC_REQ_ACCEPT 
        public const byte BONCODEAJP13_ACCEPT_CHARSET = 0x02;   //accept-charset 0xA002 SC_REQ_ACCEPT_CHARSET 
        public const byte BONCODEAJP13_ACCEPT_ENCODING = 0x03;  //accept-encodingLib 0xA003 SC_REQ_ACCEPT_ENCODING 
        public const byte BONCODEAJP13_ACCEPT_LANGUAGE = 0x04;  //accept-language 0xA004 SC_REQ_ACCEPT_LANGUAGE 
        public const byte BONCODEAJP13_AUTHORIZATION = 0x05;    //authorization 0xA005 SC_REQ_AUTHORIZATION 
        public const byte BONCODEAJP13_CONNECTION = 0x06;       //connection 0xA006 SC_REQ_CONNECTION 
        public const byte BONCODEAJP13_CONTENT_TYPE = 0x07;     //content-type 0xA007 SC_REQ_CONTENT_TYPE 
        public const byte BONCODEAJP13_CONTENT_LENGTH = 0x08;   //content-length 0xA008 SC_REQ_CONTENT_LENGTH 
        public const byte BONCODEAJP13_COOKIE = 0x09;           //cookie 0xA009 SC_REQ_COOKIE 
        public const byte BONCODEAJP13_COOKIE2 = 0x0A;          //cookie2 0xA00A SC_REQ_COOKIE2 
        public const byte BONCODEAJP13_HOST = 0x0B;             //host 0xA00B SC_REQ_HOST 
        public const byte BONCODEAJP13_PRAGMA = 0x0C;           //pragma 0xA00C SC_REQ_PRAGMA 
        public const byte BONCODEAJP13_REFERER = 0x0D;          //referer 0xA00D SC_REQ_REFERER 
        public const byte BONCODEAJP13_USER_AGENT = 0x0E;       //user-agent 0xA00E SC_REQ_USER_AGENT 
        
    }


    ///////////////////////////////////////////////////////////////////////////////
    // BonCodeAJP13 HTTP binary methods, these are the methods we can exchange in binary format
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// HTTP method for which we have a binary format. 
    /// Used within forward requests
    /// </summary>
    public struct BonCodeAJP13HTTPMethods
    {

        public const byte BONCODEAJP13_OPTIONS = 0x01;          // OPTIONS 1 
        public const byte BONCODEAJP13_GET = 0x02;              // GET 2 
        public const byte BONCODEAJP13_HEAD = 0x03;             // HEAD 3 
        public const byte BONCODEAJP13_POST = 0x04;             // POST 4 
        public const byte BONCODEAJP13_PUT = 0x05;              // PUT 5 
        public const byte BONCODEAJP13_DELETE = 0x06;           // DELETE 6 
        public const byte BONCODEAJP13_TRACE = 0x07;            // TRACE 7 
        public const byte BONCODEAJP13_PROPFIND = 0x08;         // PROPFIND 8 
        public const byte BONCODEAJP13_PROPPATCH = 0x09;        // PROPPATCH 9 
        public const byte BONCODEAJP13_MKCOL = 0x0A;            // MKCOL 10 
        public const byte BONCODEAJP13_COPY = 0x0B;             // COPY 11 
        public const byte BONCODEAJP13_MOVE = 0x0C;             // MOVE 12 
        public const byte BONCODEAJP13_LOCK = 0x0D;             // LOCK 13 
        public const byte BONCODEAJP13_UNLOCK = 0x0E;           // UNLOCK 14 
        public const byte BONCODEAJP13_ACL = 0x0F;              // ACL 15 
        public const byte BONCODEAJP13_REPORT = 0x10;           // REPORT 16 
        public const byte BONCODEAJP13_VERSION_CONTROL = 0x11;  // VERSION-CONTROL 17 
        public const byte BONCODEAJP13_CHECKIN = 0x12;          // CHECKIN 18 
        public const byte BONCODEAJP13_CHECKOUT = 0x13;         // CHECKOUT 19 
        public const byte BONCODEAJP13_UNCHECKOUT = 0x14;       // UNCHECKOUT 20 
        public const byte BONCODEAJP13_SEARCH = 0x15;           // SEARCH 21 
        public const byte BONCODEAJP13_MKWORKSPACE = 0x16;      // MKWORKSPACE 22 
        public const byte BONCODEAJP13_UPDATE = 0x17;           // UPDATE 23 
        public const byte BONCODEAJP13_LABEL = 0x18;            // LABEL 24 
        public const byte BONCODEAJP13_MERGE = 0x19;            // MERGE 25 
        public const byte BONCODEAJP13_BASELINE_CONTROL = 0x1A; // BASELINE_CONTROL 26 
        public const byte BONCODEAJP13_MKACTIVITY = 0x1B;       // MKACTIVITY 27 
        public const byte BONCODEAJP13_SC_M_JKSTORED = 0xFF;       // Any other VERB 255 to Stroe in Attributes

    }

    ///////////////////////////////////////////////////////////////////////////////
    // BonCodeAJP13 HTTP binary attributes, these are the attributes we can exchange in binary format
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// attributes for which we have a binary format. Used within forward requests.
    /// If attribute is not in the list, it will be exchanged as string (attribute/value pair)
    /// </summary>
    public struct BonCodeAJP13HTTPAttributes
    {
       public const byte BONCODEAJP13_CONTEXT = 0x01;          // ?context 0x01 Not currently implemented 
       public const byte BONCODEAJP13_SERVLET_PATH = 0x02;     // ?servlet_path 0x02 Not currently implemented 
       public const byte BONCODEAJP13_REMOTE_USER = 0x03;      // ?remote_user 0x03  
       public const byte BONCODEAJP13_AUTH_TYPE = 0x04;        // ?auth_type 0x04  
       public const byte BONCODEAJP13_QUERY_STRING = 0x05;     // ?query_string 0x05  
       public const byte BONCODEAJP13_JVM_ROUTE = 0x06;        // ?jvm_route 0x06  
       public const byte BONCODEAJP13_SSL_CERT = 0x07;         // ?ssl_cert 0x07  
       public const byte BONCODEAJP13_SSL_CIPHER = 0x08;       // ?ssl_cipher 0x08  
       public const byte BONCODEAJP13_SSL_SESSION = 0x09;      // ?ssl_session 0x09  
       public const byte BONCODEAJP13_REQ_ATTRIBUTE = 0x0A;    // ?req_attribute 0x0A Name (the name of the attribut follows). This one will be used to send named attribute pairs
       public const byte BONCODEAJP13_SSL_KEY_SIZE = 0x0B;     // ?ssl_key_size 0x0B  
       public const byte BONCODEAJP13_SECRET = 0x0C;           // ?secret -- request secret needs to match requiredSecret on Tomcat AJP connection defintion in server.xml
       public const byte BONCODEAJP13_STORED_METHOD = 0x0D;    // Used to handle non Default HTTP Methods
       public const byte BONCODEAJP13_ACCEPT = 0xFF;           // are_done 0xFF request_terminator. This one is appended automatically to packet.

    }

    ///////////////////////////////////////////////////////////////////////////////
    // BonCodeAJP13 Tomcat Send Headers HTTP binary headers
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// BonCodeAJP13 Tomcat Send Headers HTTP binary headers. Two bytes, the first byte is allways assumed to be 0xA0.
    /// If header is not in the list, it will be exchanged as string (header/value pair)
    /// </summary>
    public struct BonCodeAJP13TomcatHeaders
    {
        public const byte BONCODEAJP13_CONTENT_TYPE = 0x01;     
        public const byte BONCODEAJP13_CONTENT_LANGUAGE = 0x02;    
        public const byte BONCODEAJP13_CONTENT_LENGTH = 0x03;        
        public const byte BONCODEAJP13_DATE = 0x04;        
        public const byte BONCODEAJP13_LAST_MODIFIED = 0x05;     
        public const byte BONCODEAJP13_LOCATION = 0x06;       
        public const byte BONCODEAJP13_SET_COOKIE = 0x07;         
        public const byte BONCODEAJP13_SET_COOKIE2 = 0x08;       
        public const byte BONCODEAJP13_SERVLET_ENGINE = 0x09;      
        public const byte BONCODEAJP13_STATUS = 0x0A;    
        public const byte BONCODEAJP13_WWW_AUTHENTICATE = 0x0B;       
        

    }

    ///////////////////////////////////////////////////////////////////////////////
    // Settings ........
    ///////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// General Settings that are flexible and can be assigned through config file 
    /// </summary>
    public struct BonCodeAJP13Settings
    {
        //Adobe Mode will also change packet size and path info header
        public static bool BONCODEAJP13_ADOBE_SUPPORT = Properties.Settings.Default.EnableAdobeMode; //FALSE

        // The TCP port number used by BonCodeAJP13.    
        public static int BONCODEAJP13_PORT = Properties.Settings.Default.Port; //8009

        // The tomcat server used by BonCodeAJP13.    
        public static string BONCODEAJP13_SERVER = Properties.Settings.Default.Server; // "localhost";

        //autoflush detection threshold in time-ticks
        public static long BONCODEAJP13_AUTOFLUSHDETECTION_TICKS = Properties.Settings.Default.FlushThresholdTicks; // 0;
        //autoflush detection by byte count. Determine whether we should attempt to detect http flushes. The number of bytes to accumulate in buffer before we spool to client. If zero, this feature is disabled.
        public static long BONCODEAJP13_AUTOFLUSHDETECTION_BYTES = Properties.Settings.Default.FlushThresholdBytes;  //0

        //Define the maximum concurrent connections in the server. This should correspond to Max JK thread count on Apache
        public static int MAX_BONCODEAJP13_CONCURRENT_CONNECTIONS = Properties.Settings.Default.MaxConnections; //200

        // Log level. 0=none, 1=Basic (startup/shutdown/error), 2=Headers, 3=Debug and packet Contents
        public static int BONCODEAJP13_LOG_LEVEL = Properties.Settings.Default.LogLevel; // 0;
        // Log dir. If blank we will attempt to use dll location dir. IMPORTANT: I_USR user will need read/write rights in this directory
        public static string BONCODEAJP13_LOG_DIR = Properties.Settings.Default.LogDir; // Empty String;
        // Log File. If blank we will attempt to use "BonCodeAJP13ConnectionLog.txt"  IMPORTANT: I_USR user will need read/write rights in this directory
        public static string BONCODEAJP13_LOG_FILE = Properties.Settings.Default.LogFile; // BonCodeAJP13Connection [siteId] and [date] and [.log] will be appended

        //remote administration (enabled by default)
        public static bool BONCODEAJP13_ENABLE_REMOTE_MANAGER = Properties.Settings.Default.EnableRemoteAdmin; // true

        //TODO: implement this functionality: are we compressing streams automatically when possible (disabled by default)
        public static bool BONCODEAJP13_AUTOCOMPRESS = Properties.Settings.Default.AutoCompression; // false

        //if the received content type contains any of this in declaration then we will handle content as text, otherwise as binary data
        public static string[] BONCODEAJP13_TEXT_MARK = new string[] { "text", "xml", "html", "plain" };

        //protect remote execution of manager for tomcat, railo, and others using these signatures
        public static string[] BONCODEAJP13_MANAGER_URLS = new string[] { "/manager/","/host-manager","/web-inf", "/meta-inf", "/pms" }; //these cannot be at the start of the URi
        public static string[] BONCODEAJP13_MANAGER_FLEXURLS = new string[] {"/lucee/admin", "/railo-context/admin/", "/bluedragon/administrator/", "/cfide/administrator", "/cfide/adminapi", "/cfide/componentutils" }; //these cannot be anywhere in the URi path

        //enable HeaderDataSupport. Will send non-standard data in header to support cfml operations -- currently adds X-Tomcat-DocRoot
        public static bool BONCODEAJP13_HEADER_SUPPORT = Properties.Settings.Default.EnableHeaderDataSupport; //false
        public static string BonCodeAjp13_DocRoot = ""; //will be set in CallHandler and override can be passed in with alternate setting DocRoot
        public static string BonCodeAjp13_PhysicalFilePath = ""; //will be set in CallHandler

        //suppressed header list, this should be a comma seperated value (CSV) list of HTTP headers we will not sent to tomcat
        public static string BONCODEAJP13_BLACKLIST_HEADERS = Properties.Settings.Default.HeaderBlacklist; //blank

        //white header list, this should be a comma seperated value (CSV) list of HTTP headers. Only headers on this list will be sent to tomcat
        public static string BONCODEAJP13_WHITELIST_HEADERS = Properties.Settings.Default.HeaderWhitelist; //blank

        //timeout waiting on flush to complete 30 seconds. TODO: add setting option in file
        public static int BONCODEAJP13_FLUSH_TIMEOUT = 30;

        //TCP timouts
        public static int BONCODEAJP13_SERVER_READ_TIMEOUT = Properties.Settings.Default.ReadTimeOut;// now set to zero=infinite, previously 1200000. read timeout 2 minutes
        public static int BONCODEAJP13_SERVER_WRITE_TIMEOUT = Properties.Settings.Default.WriteTimeOut; // now set to zero=infinite, previously 30000. write timeout 30s

        //Force SSL. Force this from tomcat; this will still accept HTTP inbound all responses will be redirected on port 443 by tomcat. JSession cookie will be issued only securely.
        public static bool BONCODEAJP13_FORCE_SECURE_SESSION = Properties.Settings.Default.ForceSecureSession; //false

        //IP rewrite from header 
        //If another HTTP header contains valid IP instead of REMOTE_ADDR, it should be provided here. Common scenario uses HTTP_X_FORWARDED_FOR as alternate reference.
        public static string BONCODEAJP13_REMOTEADDR_FROM = Properties.Settings.Default.ResolveRemoteAddrFrom; //blank

        //Allow Empty Headers
        //By default the connector only sends HTTP headers that contain a value. If you need to see all headers all the time, you need to change this to True. Default False.
        public static bool BONCODEAJP13_ALLOW_EMTPY_HEADERS = Properties.Settings.Default.AllowEmptyHeaders; //false

        //Path info header: default changes based on Adobe support
        public static string BONCODEAJP13_PATHINFO_HEADER = BONCODEAJP13_ADOBE_SUPPORT ? "path-info" : Properties.Settings.Default.PathInfoHeader; //xajp-path-info

        //HTTP status codes
        public static bool BONCODEAJP13_ENABLE_HTTPSTATUSCODES = Properties.Settings.Default.EnableHTTPStatusCodes; //true

        //Tomcat Error URL (will redirect to this page if tomcat is not available)
        public static string BONCODEAJP13_TOMCAT_DOWN_URL = Properties.Settings.Default.TomcatConnectErrorURL; //blank

        //TCPStreamErrorMessage
        public static string BONCODEAJP13_TOMCAT_STREAM_ERRORMSG = Properties.Settings.Default.TCPStreamErrorMessage; //blank

        //TCPClientErrorMessage
        public static string BONCODEAJP13_TOMCAT_TCPCLIENT_ERRORMSG = Properties.Settings.Default.TCPClientErrorMessage; //blank

        //URL path prefix such as /axis that will be prefixed to any call from IIS to tomcat. Allows for easier mapping.
        public static string BONCODEAJP13_PATH_PREFIX = Properties.Settings.Default.PathPrefix; //blank

        //DISABLED:Adobe mode change of packet size. Needs to corresdpond with Apache Tomcat packetSize. Default changes based on Adobe Support
        public static int MAX_BONCODEAJP13_PACKET_LENGTH = BONCODEAJP13_ADOBE_SUPPORT ? 65531 : Properties.Settings.Default.PacketSize; //8192 or Adobe hardcode 65531
        
        //packet size and user packet size
        //public static int MAX_BONCODEAJP13_PACKET_LENGTH =  Properties.Settings.Default.PacketSize; //8192
        public static int MAX_BONCODEAJP13_USERDATA_LENGTH = MAX_BONCODEAJP13_PACKET_LENGTH - 6;

        //fingerprint
        public static bool BONCODEAJP13_ENABLE_CLIENTFINGERPRINT = Properties.Settings.Default.EnableClientFingerPrint; // false
        public static string BONCODEAJP13_FINGERPRINTHEADERS = Properties.Settings.Default.FPHeaders; //CSV list of headers

        //Skip IIS Custom headers
        public static bool BONCODEAJP13_SKIP_IISCUSTOMERRORS = Properties.Settings.Default.SkipIISCustomErrors; //false 

        //Log IP Filter
        public static string BONCODEAJP13_LOG_IPFILTER = Properties.Settings.Default.LogIPFilter; // empty string

        //The AJP connection request secret. Both Tomcat and Boncode can use a shared secret to secure the connection. This also needs to be added as requiredSecret on Tomcat side. If this is set wrong you will only see blank pages and http 403s from tomcat.
        public static string BONCODEAJP13_REQUEST_SECRET = Properties.Settings.Default.RequestSecret; //blank = disabled

        //garbage collection setting
        public static bool BONCODEAJP13_FORCE_GC = Properties.Settings.Default.EnableAggressiveGC; // false

        //ModCfmlSecret The shared secret to be used with Tomcat mod_cfml valve. No new contexts in Tomcat will be created if this is not the same on both sides.
        public static string BONCODE_MODCFML_SECRET = Properties.Settings.Default.ModCFMLSecret; // empty string

        //DocRoot Override
        public static string BONCODE_DOCROOT_OVERRIDE = Properties.Settings.Default.DocRoot; // empty string
    }  


    ///////////////////////////////////////////////////////////////////////////////
    // Constants: only changeable through re-compiling. If a const should
    // need to be more flexbile we need to change them to a setting instead.
    ///////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// General Protocol Consts like timeout, packet length, idle time consts ... 
    /// Some of these are not used currently
    /// </summary>
    public struct BonCodeAJP13Consts
    {

        //connector version identifier
        public const string BONCODEAJP13_CONNECTOR_VERSION = "1.0.42";

        // Version number for the BonCodeAJP13 Protocol.    
        public const byte BONCODEAJP13_PROTOCOL_VERSION = 13;

        //Defines the current minimum data length for a BonCodeAJP13 packet.
        public const int MIN_BONCODEAJP13_PACKET_LENGTH = 5;

        //Defines the current maximum data length for a BonCodeAJP13 packets.
        
        //public const int MAX_BONCODEAJP13_PACKET_LENGTH = 8192;         //this is including control bytes
        //public const int MAX_BONCODEAJP13_USERDATA_LENGTH = 8186;       //maximum number of user data bytes that can be packaged into Forward Request, exludes all control bytes. If length bytes are included this would be 8188
        
        // The max period of time that the BonCodeAJP13 listner can stay listening without any new connection (not used).
        public const int BONCODEAJP13_LISTENER_MAX_IDLE_TIME = 3600000;
       
        // Define Send/Receive Timeout for connection to be kept alive if invoked in process.               
        public const int BONCODEAJP13_SERVER_KEEP_ALIVE_TIMEOUT = 1800000; //keep alive for 30 minutes

        
    }   

    /// <summary>
    /// Log Level enumarations (0-4) 
    /// </summary>
    public struct BonCodeAJP13LogLevels
    {
        public const int BONCODEAJP13_NO_LOG = 0;
        public const int BONCODEAJP13_LOG_ERRORS = 1;
        public const int BONCODEAJP13_LOG_BASIC = 2;
        public const int BONCODEAJP13_LOG_HEADERS = 3;
        public const int BONCODEAJP13_LOG_DEBUG = 4;
    }

    

 

}
