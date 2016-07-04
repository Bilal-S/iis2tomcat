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
namespace BonCodeAJP13.TomcatPackets
{
    public class TomcatGetBodyChunk:TomcatReturn
    {
        #region data Members
        
        //set packet name overrides base class
        new const string p_PACKET_DESCRIPTION = "TOMCAT GET BODY CHUNK RETURN";

        #endregion

        
        #region Properties
        //currently none
        #endregion


        //contructors specific to this type of packet
        #region Constructors

        /// <summary>
        /// Generic Constructor can be used to create empty package       
        /// </summary>
        public TomcatGetBodyChunk()
        {

        }

        /// <summary>
        /// constructor with data to initialize      
        /// </summary>
        public TomcatGetBodyChunk(byte[] content)
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
        /// A Get Body Chunk message may also contain the length of the desired body chunk to be transmitted.
        /// If no desired length is specficied we will use the Maximum package length possible
        /// </summary>
        public int GetRequestedLength()
        {
            int RLEN = BonCodeAJP13Settings.MAX_BONCODEAJP13_USERDATA_LENGTH;
            if (p_UserDataLength == 3) 
            {
                ushort tempLength=0;
                GetInt16(p_ByteStore,ref tempLength,5);
                RLEN = tempLength; //implicit cast
            }

            return RLEN;
        }
        #endregion
    }
}
