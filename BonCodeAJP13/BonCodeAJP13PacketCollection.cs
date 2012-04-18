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

using System.Collections;


namespace BonCodeAJP13
{
    /// <summary>
    /// Used to hold a collection of Received packets. 
    /// Can contain multiple tomcat or server packets.
    /// </summary>
    public class BonCodeAJP13PacketCollection : CollectionBase
    {

        public BonCodeAJP13Packet this[int index]
        {
            get
            {
                return ((BonCodeAJP13Packet)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(BonCodeAJP13Packet value)
        {
            return (List.Add(value));
        }

        public int IndexOf(BonCodeAJP13Packet value)
        {
            return (List.IndexOf(value));
        }

        public void Insert(int index, BonCodeAJP13Packet value)
        {
            List.Insert(index, value);
        }

        public void Remove(BonCodeAJP13Packet value)
        {
            List.Remove(value);
        }

        public bool Contains(BonCodeAJP13Packet value)
        {
            // If value is not of type BonCodeAJP13Packet, this will return false.
            return (List.Contains(value));
        }

    }
}
