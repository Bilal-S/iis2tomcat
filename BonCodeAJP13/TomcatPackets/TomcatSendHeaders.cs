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


using System.Collections.Specialized;
using System;

namespace BonCodeAJP13.TomcatPackets
{
    public class TomcatSendHeaders:TomcatReturn
    {
        #region data Members
        
        //set packet name overrides base class
        new const string p_PACKET_DESCRIPTION = "TOMCAT SEND HEADERS RETURN";

        #endregion

        
        #region Properties
        //currently none
        #endregion


        //contructors specific to this type of packet
        #region Constructors

        /// <summary>
        /// Generic Constructor can be used to create empty package       
        /// </summary>
        public TomcatSendHeaders()
        {

        }

        /// <summary>
        /// constructor with data to initialize      
        /// </summary>
        public TomcatSendHeaders(byte[] content)
        {
            p_ByteStore = content;
            try
            {
                p_PacketLength = content.Length;
                p_UserDataLength = System.Convert.ToUInt16(content.Length - 4);
            }
            catch
            {
                //do nothing for now
            }
        }        

        #endregion

        #region Methods
        /// <summary>
        /// return headers received      
        /// </summary>
        public NameValueCollection GetHeaders()
        {
            NameValueCollection retHeaders = new NameValueCollection();
            int pos = 3;            
            string requestMessage = "";
            ushort numOfHeaders = 0;
            string keyName = "";
            string keyValue = "";

            if (p_ByteStore.Length > 2)  //first two bytes are for code and status
            {
                pos = GetString(p_ByteStore, ref requestMessage, pos);
                //get number of headers
                pos = GetUInt16(p_ByteStore, ref numOfHeaders, pos);
                //loop through headers as pairs
                for (int i = 0; i < numOfHeaders; i++)
                {
                    if (p_ByteStore[pos] == BonCodeAJP13Markers.BONCODEAJP13_BYTE_HEADER_MARKER)
                    {
                        //translate byte header to string header and advance byte pointer
                        keyName = BonCodeAJP13PacketHeaders.GetTomcatHeaderString(p_ByteStore[pos + 1]);
                        pos = pos + 2;
                    } else {
                        //get string header
                        pos = GetString(p_ByteStore, ref keyName, pos);
                    }
                    //get value string
                    pos = GetString(p_ByteStore, ref keyValue, pos);
                    //set into return
                    if (keyName != "")
                    {
                        retHeaders.Add(keyName, keyValue + "|"); //we are adding as a NameValue collection does not allow for duplicate keys, it will turn them into a 
                    }

                }

            }

            return retHeaders;
        }

        /// <summary>
        /// return request status
        /// </summary>
        public int GetStatus()
        {
            ushort retVal = 0;
            if (p_ByteStore.Length > 2)  //byte 1 and 2 are status bytes
            {
                 GetInt16(p_ByteStore, ref retVal, 1);
            }

            return System.Convert.ToInt16(retVal);
        }


        /// <summary>
        /// override to base class return header information about a packet.
        /// we add this for logging, so we can print the headers we are sending to client
        /// </summary>
        public override string PrintPacketHeader()
        {
            string strPckHead = "";
            string keyName = "";
            string keyValue = "";
            NameValueCollection tomcatHeaders = GetHeaders();

            strPckHead =  p_ByteStore.Length.ToString() + " bytes";
            //output headers if present
            if (tomcatHeaders != null)
            {
                for (int i = 0; i < tomcatHeaders.AllKeys.Length; i++)  //for (int i = 0; i < httpHeaders.AllKeys.Length; i++)
                {
                    keyName = tomcatHeaders.AllKeys[i];
                    keyValue = tomcatHeaders[keyName];

                    //check for repeated headers of the same type they are seperated by pipe+comma combination
                    string[] sHeaders = keyValue.Split(new string[] { "|," }, StringSplitOptions.None);
                    string finalKeyValue = "";
                    if (sHeaders.Length > 1)
                    {
                        //check for multiple headers of same type returned, e.g. cookies                                
                        for (int i2 = 0; i2 < sHeaders.Length; i2++)
                        {

                            if (i2 == sHeaders.Length - 1)
                            {
                                finalKeyValue = sHeaders[i2].Substring(0, sHeaders[i2].Length - 1); //last array element
                            }
                            else
                            {
                                finalKeyValue = sHeaders[i2]; //regular array element
                            }
                           
                        }
                    }

                    else
                    {
                        //single header remove pipe character at the end   
                        finalKeyValue = keyValue.Substring(0, keyValue.Length - 1);
                    }

                    strPckHead = strPckHead + "\r\n > " + keyName + " : " + finalKeyValue + "";
                    
                }
            }

            return strPckHead;
        }

        //only a test method here
        public string TestHeader()
        {
            return "Test Header " + p_PACKET_DESCRIPTION;
        }

        #endregion
    }
}
