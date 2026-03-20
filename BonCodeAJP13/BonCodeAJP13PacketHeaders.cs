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
// This class will provide services to translate HTTP headers, Attributes, and Methods
//==============================================================================
// Tomcat AJP 1.3 has facilities to accept binary encoded headers
// This class will assist in translating communictation into/from binary to
// standard HTTP headers, attributes (sub group of headers) and HTTP Methods
//==============================================================================

using System.Collections.Concurrent;


namespace BonCodeAJP13
{
    /// <summary>
    /// Helper functions to translate headers and attributes to and from AJP 1.3 binary format
    /// </summary>
    class BonCodeAJP13PacketHeaders
    {
        #region Instance Data
            //declare translation dictionaries (thread-safe via ConcurrentDictionary)
            private static ConcurrentDictionary<string, byte[]> p_HTranslator = null;
            private static ConcurrentDictionary<string, byte> p_ATranslator = null;
            private static ConcurrentDictionary<string, byte> p_MStringTranslator = null;
            private static ConcurrentDictionary<byte, string> p_MByteTranslator = null;
            private static ConcurrentDictionary<byte, string> p_THeadTranslator = null;
        #endregion


        #region Constructor
            /// <summary>
            /// Constructor. Will initialize translation tables
            /// </summary>
            static BonCodeAJP13PacketHeaders()
            {
                PopulateHeaderTranslation();
                PopulateAttributeTranslation();
                PopulateStringToByteMethodTranslation();
                //has to be run after stringToByte
                PopulateByteToStringMethodTranslation();
                PopulateTomcatHeaders();
            }


        #endregion



        #region Methods

            /// <summary>
            /// Return a byte representation for the header key if known. Return null if not known.
            /// </summary> 
            public static byte[] GetHeaderBytes(string headerKey)
            {
                byte[] retVal = null;

                if (p_HTranslator.TryGetValue(headerKey, out retVal))
                {
                    return retVal;
                }
                // this part will always return null
                return retVal;
            }

            /// <summary>
            /// Return a byte representation for the Attribute key if known. Returns 0x00 if not known.
            /// </summary> 
            public static byte GetAttributeByte(string attributeKey)
            {
                byte retVal = 0x00;

                if (p_ATranslator.TryGetValue(attributeKey, out retVal))
                {
                    return retVal;
                }
                // this is 0x00
                return retVal;
            }


            /// <summary>
            /// Return a byte representation for the Method key if known. Returns 0x02 (GET) if not known so the transfer can succeed anyway. 
            /// </summary> 
            public static byte GetMethodByte(string methodKey)
            {
                //Change the Default from "BONCODEAJP13_GET" to "BONCODEAJP13_SC_M_JKSTORED",  This will allow us to check and Store the VERB in the BONCODEAJP13_STORED_METHOD Attribute
                byte retVal = BonCodeAJP13HTTPMethods.BONCODEAJP13_SC_M_JKSTORED;  

                if (p_MStringTranslator.TryGetValue(methodKey, out retVal))
                {
                    return retVal;
                }

                return BonCodeAJP13HTTPMethods.BONCODEAJP13_SC_M_JKSTORED;
            }

            /// <summary>
            /// Return a string representation for the Method byte key if known. 
            /// Returns empty string if not known. 
            /// </summary> 
            public static string GetMethodString(byte methodKey)
            {
                string retVal = "";

                if (p_MByteTranslator.TryGetValue(methodKey, out retVal))
                {
                    return retVal;
                }

                return "";
            }


            /// <summary>
            /// Return a string representation for returned header. Returns empty string if not known. 
            /// </summary> 
            public static string GetTomcatHeaderString(byte headerKey)
            {
                string retVal = "";
                if (p_THeadTranslator.TryGetValue(headerKey, out retVal))
                {
                    return retVal;
                }

                return "";
            }


            /// <summary>
            /// Populate the p_HTranslator table with correct data
            /// </summary>            
            private static void PopulateHeaderTranslation()
            {
                //create and populate concurrent dictionary
                if (p_HTranslator == null)
                {
                    p_HTranslator = new ConcurrentDictionary<string, byte[]>();
                    p_HTranslator.TryAdd("HTTP_ACCEPT", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_ACCEPT });
                    p_HTranslator.TryAdd("HTTP_ACCEPT_CHARSET", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_ACCEPT_CHARSET });
                    p_HTranslator.TryAdd("HTTP_ACCEPT_ENCODING", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_ACCEPT_ENCODING });
                    p_HTranslator.TryAdd("HTTP_ACCEPT_LANGUAGE", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_ACCEPT_LANGUAGE });
                    p_HTranslator.TryAdd("HTTP_AUTHORIZATION", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_AUTHORIZATION });

                    p_HTranslator.TryAdd("HTTP_CONNECTION", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_CONNECTION });
                    p_HTranslator.TryAdd("CONTENT_TYPE", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_CONTENT_TYPE });
                    p_HTranslator.TryAdd("CONTENT_LENGTH", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_CONTENT_LENGTH });
                    p_HTranslator.TryAdd("HTTP_COOKIE", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_COOKIE });
                    p_HTranslator.TryAdd("HTTP_COOKIE2", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_COOKIE2 }); //HTTP_COOKIE2 IS NOT IMPLEMENTED IN IIS7, included here for completeness sake                   
                    p_HTranslator.TryAdd("HTTP_HOST", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_HOST });

                    p_HTranslator.TryAdd("HTTP_PRAGMA", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_PRAGMA }); //HTTP_PRAGMA IS NOT IMPLEMENTED IN IIS7, included here for completeness sake
                    p_HTranslator.TryAdd("HTTP_REFERER", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_REFERER });
                    p_HTranslator.TryAdd("HTTP_USER_AGENT", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_USER_AGENT });

                }

            }

            /// <summary>
            /// Populate the p_ATranslator table with correct data
            /// </summary>  
            private static void PopulateAttributeTranslation()
            {
                if (p_ATranslator == null)
                {
                    p_ATranslator = new ConcurrentDictionary<string, byte>();

                    p_ATranslator.TryAdd("HTTP_CONTEXT", BonCodeAJP13HTTPAttributes.BONCODEAJP13_CONTEXT); //HTTP_CONTEXT IS NOT IMPLEMENTED IN IIS7 in this fashion, included here for completeness sake
                    //servlet path is already transferred via regular headers, Tomcat has not implemented Path_Info for AJP protocol
                    //p_ATranslator.TryAdd("PATH_INFO", BonCodeAJP13HTTPAttributes.BONCODEAJP13_SERVLET_PATH); //PATH_INFO though specified here is not implemented on tomcat side. This is currently is a known defect. Once tomcat implements this just change the name back.
                    p_ATranslator.TryAdd("REMOTE_USER", BonCodeAJP13HTTPAttributes.BONCODEAJP13_REMOTE_USER);

                    p_ATranslator.TryAdd("AUTH_TYPE", BonCodeAJP13HTTPAttributes.BONCODEAJP13_AUTH_TYPE);
                    p_ATranslator.TryAdd("QUERY_STRING", BonCodeAJP13HTTPAttributes.BONCODEAJP13_QUERY_STRING);
                    p_ATranslator.TryAdd("JVM_ROUTE", BonCodeAJP13HTTPAttributes.BONCODEAJP13_JVM_ROUTE); //JVM_ROUTE IS NOT IMPLEMENTED IN IIS7, included here for completeness sake
                    p_ATranslator.TryAdd("SSL_CLIENTCERT", BonCodeAJP13HTTPAttributes.BONCODEAJP13_SSL_CERT); //SSL_CERT is a protocol misnomer. We transfer client certificate (x509v3).

                    p_ATranslator.TryAdd("SSL_CIPHER", BonCodeAJP13HTTPAttributes.BONCODEAJP13_SSL_CIPHER); //SSL_CIPHER IS NOT IMPLEMENTED IN THIS FORM, IIS7+ Uses multiple data points. All SSL data is transferred via HTTP headers
                    p_ATranslator.TryAdd("SSL_SESSION", BonCodeAJP13HTTPAttributes.BONCODEAJP13_SSL_SESSION); //indicate whether we are using HTTPS session. This impacts how the container responds. It will force SSL and all cookies will be secure.
                    p_ATranslator.TryAdd("HTTPS_SECRETKEYSIZE", BonCodeAJP13HTTPAttributes.BONCODEAJP13_SSL_KEY_SIZE); 


                }

            }

            /// <summary>
            /// Populate the p_MStringTranslator table with correct data. Not all methods are supported by IIS, but all are included for completeness purposes
            /// </summary>  
            private static void PopulateStringToByteMethodTranslation()
            {
                if (p_MStringTranslator == null)
                {
                    p_MStringTranslator = new ConcurrentDictionary<string, byte>();

                    p_MStringTranslator.TryAdd("OPTIONS", BonCodeAJP13HTTPMethods.BONCODEAJP13_OPTIONS);
                    p_MStringTranslator.TryAdd("GET", BonCodeAJP13HTTPMethods.BONCODEAJP13_GET);
                    p_MStringTranslator.TryAdd("HEAD", BonCodeAJP13HTTPMethods.BONCODEAJP13_HEAD);
                    p_MStringTranslator.TryAdd("POST", BonCodeAJP13HTTPMethods.BONCODEAJP13_POST);
                    p_MStringTranslator.TryAdd("PUT", BonCodeAJP13HTTPMethods.BONCODEAJP13_PUT);
                    p_MStringTranslator.TryAdd("DELETE", BonCodeAJP13HTTPMethods.BONCODEAJP13_DELETE);
                    p_MStringTranslator.TryAdd("TRACE", BonCodeAJP13HTTPMethods.BONCODEAJP13_TRACE);
                    p_MStringTranslator.TryAdd("PROPFIND", BonCodeAJP13HTTPMethods.BONCODEAJP13_PROPFIND);
                    p_MStringTranslator.TryAdd("PROPPATCH", BonCodeAJP13HTTPMethods.BONCODEAJP13_PROPPATCH);
                    p_MStringTranslator.TryAdd("MKCOL", BonCodeAJP13HTTPMethods.BONCODEAJP13_MKCOL);
                    p_MStringTranslator.TryAdd("COPY", BonCodeAJP13HTTPMethods.BONCODEAJP13_COPY);
                    p_MStringTranslator.TryAdd("MOVE", BonCodeAJP13HTTPMethods.BONCODEAJP13_MOVE);
                    p_MStringTranslator.TryAdd("LOCK", BonCodeAJP13HTTPMethods.BONCODEAJP13_LOCK);
                    p_MStringTranslator.TryAdd("UNLOCK", BonCodeAJP13HTTPMethods.BONCODEAJP13_UNLOCK);
                    p_MStringTranslator.TryAdd("ACL", BonCodeAJP13HTTPMethods.BONCODEAJP13_ACL);
                    p_MStringTranslator.TryAdd("REPORT", BonCodeAJP13HTTPMethods.BONCODEAJP13_REPORT);
                    p_MStringTranslator.TryAdd("VERSION_CONTROL", BonCodeAJP13HTTPMethods.BONCODEAJP13_VERSION_CONTROL);
                    p_MStringTranslator.TryAdd("CHECKIN", BonCodeAJP13HTTPMethods.BONCODEAJP13_CHECKIN);
                    p_MStringTranslator.TryAdd("CHECKOUT", BonCodeAJP13HTTPMethods.BONCODEAJP13_CHECKOUT);
                    p_MStringTranslator.TryAdd("UNCHECKOUT", BonCodeAJP13HTTPMethods.BONCODEAJP13_UNCHECKOUT);
                    p_MStringTranslator.TryAdd("SEARCH", BonCodeAJP13HTTPMethods.BONCODEAJP13_SEARCH);
                    p_MStringTranslator.TryAdd("MKWORKSPACE", BonCodeAJP13HTTPMethods.BONCODEAJP13_MKWORKSPACE);
                    p_MStringTranslator.TryAdd("UPDATE", BonCodeAJP13HTTPMethods.BONCODEAJP13_UPDATE);
                    p_MStringTranslator.TryAdd("LABEL", BonCodeAJP13HTTPMethods.BONCODEAJP13_LABEL);
                    p_MStringTranslator.TryAdd("MERGE", BonCodeAJP13HTTPMethods.BONCODEAJP13_MERGE);
                    p_MStringTranslator.TryAdd("BASELINE_CONTROL", BonCodeAJP13HTTPMethods.BONCODEAJP13_BASELINE_CONTROL);
                    p_MStringTranslator.TryAdd("MKACTIVITY", BonCodeAJP13HTTPMethods.BONCODEAJP13_MKACTIVITY);

                }

            }


            /// <summary>
            /// Populate the p_MbyteTranslator table with correct data. 
            /// Not all methods are supported by IIS, but all are included for completeness purposes
            /// </summary>  
            private static void PopulateByteToStringMethodTranslation()
            {
                if (p_MStringTranslator != null && p_MByteTranslator == null)
                {
                    p_MByteTranslator = new ConcurrentDictionary<byte, string>();
                    //iterater through allready defined string translator dictionary and create the reverse keys 
                    //for Byte translation
                    foreach (var kvp in p_MStringTranslator) {
                        p_MByteTranslator.TryAdd(kvp.Value, kvp.Key);
                    }
                }

            }

            /// <summary>
            /// Populate the p_THeadTranslator table with correct data. These are binary return responses in headers.
            /// </summary> 
            private static void PopulateTomcatHeaders()
            {
                if (p_THeadTranslator == null)
                {
                    p_THeadTranslator = new ConcurrentDictionary<byte, string>();

                    p_THeadTranslator.TryAdd(BonCodeAJP13TomcatHeaders.BONCODEAJP13_CONTENT_TYPE, "Content-Type");
                    p_THeadTranslator.TryAdd(BonCodeAJP13TomcatHeaders.BONCODEAJP13_CONTENT_LANGUAGE, "Content-Language");
                    p_THeadTranslator.TryAdd(BonCodeAJP13TomcatHeaders.BONCODEAJP13_CONTENT_LENGTH, "Content-Length");
                    p_THeadTranslator.TryAdd(BonCodeAJP13TomcatHeaders.BONCODEAJP13_DATE, "Date");
                    p_THeadTranslator.TryAdd(BonCodeAJP13TomcatHeaders.BONCODEAJP13_LAST_MODIFIED, "Last-Modified");
                    p_THeadTranslator.TryAdd(BonCodeAJP13TomcatHeaders.BONCODEAJP13_LOCATION, "Location"); //Content-Location
                    p_THeadTranslator.TryAdd(BonCodeAJP13TomcatHeaders.BONCODEAJP13_SET_COOKIE, "Set-Cookie");  //need post processing in header thus we do not use Set-Cookie as name
                    p_THeadTranslator.TryAdd(BonCodeAJP13TomcatHeaders.BONCODEAJP13_SET_COOKIE2, "Set-Cookie"); //need post processing in header thus we do not use Set-Cookie as name
                    p_THeadTranslator.TryAdd(BonCodeAJP13TomcatHeaders.BONCODEAJP13_SERVLET_ENGINE, "Servlet-Engine");
                    p_THeadTranslator.TryAdd(BonCodeAJP13TomcatHeaders.BONCODEAJP13_STATUS, "Status");
                    p_THeadTranslator.TryAdd(BonCodeAJP13TomcatHeaders.BONCODEAJP13_WWW_AUTHENTICATE, "WWW-Authenticate");


                }

            }

        #endregion
        
    }
}