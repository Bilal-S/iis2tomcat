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


using System.Collections.Specialized;

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

        

        //no specific method for this class
        public string TestHeader()
        {
            return "Test Header";
        }

        #endregion
    }
}
