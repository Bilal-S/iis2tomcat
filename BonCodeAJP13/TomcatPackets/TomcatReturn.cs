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

//==============================================================================
// This is the base class for packets received from Tomcat
//==============================================================================
// Do not use this class directly. If possible select an appropriate return type
// and create specific packets for each return type
//==============================================================================


using System;

namespace BonCodeAJP13.TomcatPackets
{
    /// <summary>
    /// Base class for packets received from tomcat, don't create an instance of this class directly
    /// </summary>
    public class TomcatReturn : BonCodeAJP13Packet
    {
        #region data Members

        //set packet name overrides base class
        public new const string p_PACKET_DESCRIPTION = "GENERIC TOMCAT RETURN";

        #endregion

        //additional properties from base class if any
        #region Properties
        #endregion


        //contructors specific to this type of packet
        #region Constructors
        /// <summary>
        /// Generic Constructor can be used to create empty package       
        /// </summary>
        public TomcatReturn()
        {

        }

        /// <summary>
        /// constructor with data to initialize      
        /// </summary>
        public TomcatReturn(byte[] content)
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
        /// Sets the raw data of the packet in an array of bytes
        /// </summary>
        public void SetDataBytes(byte[] content) {
            p_ByteStore = content;   
        }


        #endregion
    }
}
