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
 * This package shoud receive a CPong response back from Tomcat
  * 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace BonCodeAJP13.ServerPackets
{
    public class BonCodeAJP13CPing : BonCodeAJP13Packet
    {
        #region data Members
        //set packet name overrides base class
        new const string p_PACKET_DESCRIPTION = "CPING PACKET";

        #endregion

        //additional properties from base class if any
        #region Properties
        #endregion


        //contructors specific to this type of packet
        #region Constructors
        
        /// <summary>
        /// Constructor for CPing      
        /// </summary>
        public BonCodeAJP13CPing() {

         
            //allocate all needed space. We already know CPING request is 5 bytes             
            byte[] aUserData = new byte[5];
            int pos = 0;
            p_UserDataLength = Convert.ToUInt16(1); //only the packet type byte
            //write out the package
            pos = SetByte(aUserData, BonCodeAJP13Markers.BONCODEAJP13_PACKET_START, pos);
            pos = SetByte(aUserData, BonCodeAJP13Markers.BONCODEAJP13_PACKET_START2, pos);
            pos = SetUInt16(aUserData, p_UserDataLength, pos);
            pos = SetByte(aUserData, BonCodeAJP13ServerPacketType.SERVER_PING, pos);
            

            //update generic class data
            p_ByteStore = aUserData;
            p_PacketLength = p_ByteStore.Length;
        
        }





        #endregion
    }
}
