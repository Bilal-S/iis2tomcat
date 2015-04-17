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
 * This is a non-standard AJP1.3 package. Supporting Vendor (Adobe) 
 * extension to protocol for physical path transfer
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace BonCodeAJP13.ServerPackets
{
    public class BonCodeFilePathPacket : BonCodeAJP13Packet
    {
        #region data Members
        //set packet name overrides base class
        new const string p_PACKET_DESCRIPTION = "FILE PATH PACKET";

        #endregion

        //additional properties from base class if any
        #region Properties
        #endregion


        //contructors specific to this type of packet
        #region Constructors
        
        /// <summary>
        /// Generic Constructor can be used to create empty package       
        /// </summary>
        public BonCodeFilePathPacket() { }


        /// <summary>
        /// constructor with path provided. we will use default application physical 
        /// path if empty string is passed as path.
        /// </summary>
        public BonCodeFilePathPacket(string sPath) {
            
            if (sPath.Length == 0) sPath = BonCodeAJP13Settings.BonCodeAjp13_DocRoot;
            //get path length in bytes in case we have Unicode this differs from sPath.Length
            //*int iPathBytes = Encoding.UTF8.GetByteCount(sPath);
            
            //Adobe uses iso encoding so we have to convert things into it

            int iPathBytes = Encoding.GetEncoding("iso-8859-1").GetByteCount(sPath);
            

            //this byte will prefix the string. But does not correctly account for 
            //length if path is more than 256 characters. We are implementing this
            //in this fashion because this is how Adobe uses it
            byte bytePrefix = System.Convert.ToByte(iPathBytes % 256);
            //allocate all needed space. We do not worry about max packet size. This 
            //protocol extension does not handle multi-part packages so we need to transfer 
            //in one shot and hope tomcat site will be able to handle.
            byte[] aUserData = new byte[iPathBytes + 5];
            int pos = 0;
            p_UserDataLength = Convert.ToUInt16(iPathBytes + 1); //have to include bytePrefix
            //write out the package
            pos = SetByte(aUserData, BonCodeAJP13Markers.BONCODEAJP13_PACKET_START, pos);
            pos = SetByte(aUserData, BonCodeAJP13Markers.BONCODEAJP13_PACKET_START2, pos);
            pos = SetUInt16(aUserData, p_UserDataLength, pos);
            pos = SetByte(aUserData, bytePrefix, pos);
            pos = SetSimpleString(aUserData, sPath, pos); //this will omit length marker

            //this packet is not well formed as per AJP13 put follows the vendor's extension 
            //should have had propper formatted string data

            p_ByteStore = aUserData;
            p_PacketLength = p_ByteStore.Length;

        
        }




        #endregion

        #region methods


        /// <summary>
        /// Set the String value in the array starting from the pos index 
        /// The length prefix will be omitted.
        /// This is Adobe specific and we need to convert string type to ISO-8559-1
        /// </summary>
        protected override int SetSimpleString(byte[] data, string value, int pos)
        {

            //Adobe uses iso-8859-1 in its packets rather then UTF-8            
            byte[] stringBytes = Encoding.GetEncoding("iso-8859-1").GetBytes(value);
            
            //set adobe file path bytes (this is not zero terminated)
            return SetSimpleByteArray(data, stringBytes, pos);
        }

        /// <summary>
        /// Return string of user data enclosed in packet. This is different based on each packet type 
        /// The Adobe specific packet is lacking a string terminator thus we only subtract three bytes
        /// </summary>
        public override string GetUserDataString()
        {
            //generically the first three bytes and the last byte are control bytes and we will not return them
            //byte[0]=type, byte[1,2] = string length, 
            //last byte should be (0x00) = string terminator IS NOT USED BY Adobe
            if (p_ByteStore.Length > 3)
            {

                return System.Text.Encoding.UTF8.GetString(p_ByteStore, 3, p_ByteStore.Length - 3);
            }
            else
            {
                return "";
            }
        }

        #endregion
    }
}
