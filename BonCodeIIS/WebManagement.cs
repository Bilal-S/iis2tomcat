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
 * This class will handle all IIS management interrogations 
 * And may cache environment data
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Web.Administration;

namespace BonCodeIIS
{
    class WebManagement
    {


        /// <summary>
        /// Check Virtual Directories and return them as string array
        /// If we do not have sufficient permissions return error string otherwise root folder mapping
        /// Currently the siteId cannot exceed the range of the sites array, so if IIS has 10 sites your max siteId can only be 10 not 13.
        /// This will cause problems with manually edited siteIds
        /// </summary>
        public static String GetVirtualDirectories()
        {
            //need to put try catch and catch permission 
            String retVal="";

            //Site defaultSite = manager.Sites["Default Web Site"];
            //string poolId = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            //potential role permissions poolIds
            //IIS APPPOOL\DefaultAppPool = Application Pool ID
            //NT AUTHORITY\SYSTEM = Local System
            //NT AUTHORITY\NETWORK SERVICE
            //NT AUTHORITY\LOCAL SERVICE

            
            try
            {
                string siteName = System.Web.Hosting.HostingEnvironment.ApplicationHost.GetSiteName();
                ServerManager manager = new ServerManager();
                Site currentSite = manager.Sites[siteName];
                int countOfDirs = 0;
               
                foreach (Application app in currentSite.Applications)
                {
                    //Console.WriteLine("Found application with the following path: {0}", app.Path);
                    //Console.WriteLine("Virtual Directories:");
                    if (app.VirtualDirectories.Count > 0)
                    {
                        foreach (VirtualDirectory vdir in app.VirtualDirectories)
                        {
                            //we only want the non-root doc paths
                            if (vdir.Path != "/")
                            {
                                retVal += vdir.Path + "," + vdir.PhysicalPath + ";";
                                countOfDirs++;
                            }
                            /*
                            Console.WriteLine("  Virtual Directory: {0}", vdir.Path);
                            Console.WriteLine("   |-PhysicalPath = {0}", vdir.PhysicalPath);
                            Console.WriteLine("   |-LogonMethod  = {0}", vdir.LogonMethod);
                            Console.WriteLine("   +-UserName     = {0}\r\n", vdir.UserName);
                            */
                        }
                    }
                }
                //if we have no virtual dirs just return pipe
                if (countOfDirs == 0)
                {
                    retVal = ";";
                }
                else
                {
                    //remove last pipe
                    retVal = retVal.TrimEnd(';');
                }
                
            }
            catch (UnauthorizedAccessException)
            {
                //we only return the error
                retVal="err";
            }



            return retVal;
        } 

    }
}
