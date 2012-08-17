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

using System.Collections;


namespace BonCodeAJP13
{
    /// <summary>
    /// Helper functions to translate headers and attributes to and from AJP 1.3 binary format
    /// </summary>
    class BonCodeAJP13PacketHeaders
    {
        #region Instance Data
            //declare translation hash tables
            private static Hashtable p_HTranslator = null;
            private static Hashtable p_ATranslator = null;
            private static Hashtable p_MTranslator = null;
            private static Hashtable p_THeadTranslator = null;
        #endregion


        #region Constructor
            /// <summary>
            /// Constructor. Will initialize translation tables
            /// </summary>
            static BonCodeAJP13PacketHeaders()
            {
                PopulateHeaderTranslation();
                PopulateAttributeTranslation();
                PopulateMethodTranslation();
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

                if (p_HTranslator.ContainsKey(headerKey))
                {
                    retVal =(byte[]) p_HTranslator[headerKey];
                }

                return retVal;
            }

            /// <summary>
            /// Return a byte representation for the Attribute key if known. Returns 0x00 if not known.
            /// </summary> 
            public static byte GetAttributeByte(string attributeKey)
            {
                byte retVal = 0x00;

                if (p_ATranslator.ContainsKey(attributeKey))
                {
                    retVal = (byte)p_ATranslator[attributeKey];
                }

                return retVal;
            }


            /// <summary>
            /// Return a byte representation for the Method key if known. Returns 0x02 (GET) if not known so the transfer can succeed anyway. 
            /// </summary> 
            public static byte GetMethodByte(string methodKey)
            {
                byte retVal = BonCodeAJP13HTTPMethods.BONCODEAJP13_GET;

                if (p_MTranslator.ContainsKey(methodKey))
                {
                    retVal = (byte)p_MTranslator[methodKey];
                }

                return retVal;
            }


            /// <summary>
            /// Return a string representation for returned header. Returns empty string if not known. 
            /// </summary> 
            public static string GetTomcatHeaderString(byte headerKey)
            {
                string retVal = "";
                if (p_THeadTranslator.ContainsKey(headerKey))
                {
                    retVal = (string)p_THeadTranslator[headerKey];
                }

                return retVal;
            }


            /// <summary>
            /// Populate the p_HTranslator table with correct data
            /// </summary>            
            private static void PopulateHeaderTranslation()
            {
                //create and populate hashtable
                if (p_HTranslator == null)
                {
                    p_HTranslator = new Hashtable();
                    p_HTranslator.Add("HTTP_ACCEPT", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_ACCEPT });
                    p_HTranslator.Add("HTTP_ACCEPT_CHARSET", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_ACCEPT_CHARSET });
                    p_HTranslator.Add("HTTP_ACCEPT_ENCODING", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_ACCEPT_ENCODING });
                    p_HTranslator.Add("HTTP_ACCEPT_LANGUAGE", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_ACCEPT_LANGUAGE });
                    p_HTranslator.Add("HTTP_AUTHORIZATION", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_AUTHORIZATION });

                    p_HTranslator.Add("HTTP_CONNECTION", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_CONNECTION });
                    p_HTranslator.Add("CONTENT_TYPE", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_CONTENT_TYPE });
                    p_HTranslator.Add("CONTENT_LENGTH", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_CONTENT_LENGTH });
                    p_HTranslator.Add("HTTP_COOKIE", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_COOKIE });
                    p_HTranslator.Add("HTTP_COOKIE2", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_COOKIE2 }); //HTTP_COOKIE2 IS NOT IMPLEMENTED IN IIS7, included here for completeness sake                   
                    p_HTranslator.Add("HTTP_HOST", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_HOST });

                    p_HTranslator.Add("HTTP_PRAGMA", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_PRAGMA }); //HTTP_PRAGMA IS NOT IMPLEMENTED IN IIS7, included here for completeness sake
                    p_HTranslator.Add("HTTP_REFERER", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_REFERER });
                    p_HTranslator.Add("HTTP_USER_AGENT", new byte[2] { 0xA0, BonCodeAJP13HTTPHeaders.BONCODEAJP13_USER_AGENT });

                }

            }

            /// <summary>
            /// Populate the p_ATranslator table with correct data
            /// </summary>  
            private static void PopulateAttributeTranslation()
            {
                if (p_ATranslator == null)
                {
                    p_ATranslator = new Hashtable();

                    p_ATranslator.Add("HTTP_CONTEXT", BonCodeAJP13HTTPAttributes.BONCODEAJP13_CONTEXT); //HTTP_CONTEXT IS NOT IMPLEMENTED IN IIS7 in this fashion, included here for completeness sake
                    //servlet path is already transferred via regular headers, Tomcat has not implemented Path_Info for AJP protocol
                    //p_ATranslator.Add("PATH_INFO", BonCodeAJP13HTTPAttributes.BONCODEAJP13_SERVLET_PATH); //PATH_INFO though specified here is not implemented on tomcat side. This is currently is a known defect. Once tomcat implements this just change the name back.
                    p_ATranslator.Add("REMOTE_USER", BonCodeAJP13HTTPAttributes.BONCODEAJP13_REMOTE_USER);

                    p_ATranslator.Add("AUTH_TYPE", BonCodeAJP13HTTPAttributes.BONCODEAJP13_AUTH_TYPE);
                    p_ATranslator.Add("QUERY_STRING", BonCodeAJP13HTTPAttributes.BONCODEAJP13_QUERY_STRING);
                    p_ATranslator.Add("JVM_ROUTE", BonCodeAJP13HTTPAttributes.BONCODEAJP13_JVM_ROUTE); //JVM_ROUTE IS NOT IMPLEMENTED IN IIS7, included here for completeness sake
                    p_ATranslator.Add("SSL_CERT", BonCodeAJP13HTTPAttributes.BONCODEAJP13_SSL_CERT); //SSL_CERT IS NOT IMPLEMENTED IN THIS FORM, IIS7 Uses multiple data point

                    p_ATranslator.Add("SSL_CIPHER", BonCodeAJP13HTTPAttributes.BONCODEAJP13_SSL_CIPHER); //SSL_CIPHER IS NOT IMPLEMENTED IN THIS FORM, IIS7 Uses multiple data point
                    p_ATranslator.Add("SSL_SESSION", BonCodeAJP13HTTPAttributes.BONCODEAJP13_SSL_SESSION); //indicate whether we are using HTTPS session. This impacts how the container responds. It will force SSL and all cookies will be secure.
                    p_ATranslator.Add("HTTPS_SECRETKEYSIZE", BonCodeAJP13HTTPAttributes.BONCODEAJP13_SSL_KEY_SIZE); 


                }

            }

            /// <summary>
            /// Populate the p_MTranslator table with correct data. Not all methods are supported by IIS, but all are included for completeness purposes
            /// </summary>  
            private static void PopulateMethodTranslation()
            {
                if (p_MTranslator == null)
                {
                    p_MTranslator = new Hashtable();

                    p_MTranslator.Add("OPTIONS", BonCodeAJP13HTTPMethods.BONCODEAJP13_OPTIONS);
                    p_MTranslator.Add("GET", BonCodeAJP13HTTPMethods.BONCODEAJP13_GET);
                    p_MTranslator.Add("HEAD", BonCodeAJP13HTTPMethods.BONCODEAJP13_HEAD);
                    p_MTranslator.Add("POST", BonCodeAJP13HTTPMethods.BONCODEAJP13_POST);
                    p_MTranslator.Add("PUT", BonCodeAJP13HTTPMethods.BONCODEAJP13_PUT);
                    p_MTranslator.Add("DELETE", BonCodeAJP13HTTPMethods.BONCODEAJP13_DELETE);
                    p_MTranslator.Add("TRACE", BonCodeAJP13HTTPMethods.BONCODEAJP13_TRACE);
                    p_MTranslator.Add("PROPFIND", BonCodeAJP13HTTPMethods.BONCODEAJP13_PROPFIND);
                    p_MTranslator.Add("PROPPATCH", BonCodeAJP13HTTPMethods.BONCODEAJP13_PROPPATCH);
                    p_MTranslator.Add("MKCOL", BonCodeAJP13HTTPMethods.BONCODEAJP13_MKCOL);
                    p_MTranslator.Add("COPY", BonCodeAJP13HTTPMethods.BONCODEAJP13_COPY);
                    p_MTranslator.Add("MOVE", BonCodeAJP13HTTPMethods.BONCODEAJP13_MOVE);
                    p_MTranslator.Add("LOCK", BonCodeAJP13HTTPMethods.BONCODEAJP13_LOCK);
                    p_MTranslator.Add("UNLOCK", BonCodeAJP13HTTPMethods.BONCODEAJP13_UNLOCK);
                    p_MTranslator.Add("ACL", BonCodeAJP13HTTPMethods.BONCODEAJP13_ACL);
                    p_MTranslator.Add("REPORT", BonCodeAJP13HTTPMethods.BONCODEAJP13_REPORT);
                    p_MTranslator.Add("VERSION_CONTROL", BonCodeAJP13HTTPMethods.BONCODEAJP13_VERSION_CONTROL);
                    p_MTranslator.Add("CHECKIN", BonCodeAJP13HTTPMethods.BONCODEAJP13_CHECKIN);
                    p_MTranslator.Add("CHECKOUT", BonCodeAJP13HTTPMethods.BONCODEAJP13_CHECKOUT);
                    p_MTranslator.Add("UNCHECKOUT", BonCodeAJP13HTTPMethods.BONCODEAJP13_UNCHECKOUT);
                    p_MTranslator.Add("SEARCH", BonCodeAJP13HTTPMethods.BONCODEAJP13_SEARCH);
                    p_MTranslator.Add("MKWORKSPACE", BonCodeAJP13HTTPMethods.BONCODEAJP13_MKWORKSPACE);
                    p_MTranslator.Add("UPDATE", BonCodeAJP13HTTPMethods.BONCODEAJP13_UPDATE);
                    p_MTranslator.Add("LABEL", BonCodeAJP13HTTPMethods.BONCODEAJP13_LABEL);
                    p_MTranslator.Add("MERGE", BonCodeAJP13HTTPMethods.BONCODEAJP13_MERGE);
                    p_MTranslator.Add("BASELINE_CONTROL", BonCodeAJP13HTTPMethods.BONCODEAJP13_BASELINE_CONTROL);
                    p_MTranslator.Add("MKACTIVITY", BonCodeAJP13HTTPMethods.BONCODEAJP13_MKACTIVITY);

                }

            }

            /// <summary>
            /// Populate the p_THeadTranslator table with correct data. These are binary return responses in headers.
            /// </summary> 
            private static void PopulateTomcatHeaders()
            {
                if (p_THeadTranslator == null)
                {
                    p_THeadTranslator = new Hashtable();

                    p_THeadTranslator.Add(BonCodeAJP13TomcatHeaders.BONCODEAJP13_CONTENT_TYPE, "Content-Type");
                    p_THeadTranslator.Add(BonCodeAJP13TomcatHeaders.BONCODEAJP13_CONTENT_LANGUAGE, "Content-Language");
                    p_THeadTranslator.Add(BonCodeAJP13TomcatHeaders.BONCODEAJP13_CONTENT_LENGTH, "Content-Length");
                    p_THeadTranslator.Add(BonCodeAJP13TomcatHeaders.BONCODEAJP13_DATE, "Date");
                    p_THeadTranslator.Add(BonCodeAJP13TomcatHeaders.BONCODEAJP13_LAST_MODIFIED, "Last-Modified");
                    p_THeadTranslator.Add(BonCodeAJP13TomcatHeaders.BONCODEAJP13_LOCATION, "Content-Location");
                    p_THeadTranslator.Add(BonCodeAJP13TomcatHeaders.BONCODEAJP13_SET_COOKIE, "Set-Cookie");  //need post processing in header thus we do not use Set-Cookie as name
                    p_THeadTranslator.Add(BonCodeAJP13TomcatHeaders.BONCODEAJP13_SET_COOKIE2, "Set-Cookie"); //need post processing in header thus we do not use Set-Cookie as name
                    p_THeadTranslator.Add(BonCodeAJP13TomcatHeaders.BONCODEAJP13_SERVLET_ENGINE, "Servlet-Engine");
                    p_THeadTranslator.Add(BonCodeAJP13TomcatHeaders.BONCODEAJP13_STATUS, "Status");
                    p_THeadTranslator.Add(BonCodeAJP13TomcatHeaders.BONCODEAJP13_WWW_AUTHENTICATE, "WWW-Authenticate");


                }

            }

        #endregion
        
    }
}
