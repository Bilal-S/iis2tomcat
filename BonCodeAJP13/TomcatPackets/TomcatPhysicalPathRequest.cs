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

/*
 * This is a non-standard use of the AJP13 protocol by vendor
 */

using System;
using System.Text;

namespace BonCodeAJP13.TomcatPackets
{
    public class TomcatPhysicalPathRequest:TomcatReturn
    {
        #region data Members
        
        //set packet name overrides base class
        new const string p_PACKET_DESCRIPTION = "COLDFUSION PHYSICAL PATH REQUEST";

        #endregion

        
        #region Properties
        //currently none
        #endregion


        //contructors specific to this type of packet
        #region Constructors

        /// <summary>
        /// Generic Constructor can be used to create empty package       
        /// </summary>
        public TomcatPhysicalPathRequest()
        {

        }

        /// <summary>
        /// constructor with data to initialize      
        /// </summary>
        public TomcatPhysicalPathRequest(byte[] content)
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
        /// return the encoded path string in this packet.     
        /// </summary>
        public string GetFilePath()
        {
            string sReturn = "";
            int pos = 1;

            //pos = GetString(p_ByteStore, ref sReturn, pos);
            sReturn = GetPathString(p_ByteStore, pos);
            return sReturn;
        }


        /// <summary>
        /// Adobe uses iso-8559 enconding for their strings, we work in UTF. We have to convert.
        /// Get the String value from the array starting from the pos index 
        /// String length is first two bytes from starting pos, then terminated by zero char
        /// </summary>
        private string GetPathString(byte[] data, int pos)
        {
            UInt16 realLength = 0;
            pos=GetUInt16(data, ref realLength, pos);
            UTF8Encoding encodingLib = new System.Text.UTF8Encoding();

            //extract the only the string bytes into new array
            byte[] pathBytes = new byte[realLength];
            //Array.Copy(data, pathBytes, realLength);
            Array.Copy(data, pos, pathBytes, 0, realLength);
            //convert encoding standards from iso to utf
            byte[] converted = Encoding.Convert(Encoding.GetEncoding("iso-8859-1"), Encoding.UTF8, pathBytes);
            //get utf-8 string since we want to only deal with UTF-8 in the remainder of this program
            string value = encodingLib.GetString(converted);

            return value; 
        }
        #endregion
    }
}
