//scratchpad program
//only used for testing calls

using System;
using System.Collections.Specialized;
using System.Net.Sockets;
using BonCodeAJP13;
using BonCodeAJP13.ServerPackets;
using BonCodeAJP13.TomcatPackets;


namespace ConsoleTCPIP
{
    class Program
    {
        private static string myURL = "/mura/index.cfm";
        private static string myServer = "railoBuilder";
        private static string myPort = "8009";
        //mura/index.cfm
        //private static string myData = "1234020502020008485454502f312e3100000f2f6d7572612f696e6465782e63666d0000033a3a310000033a3a310000096c6f63616c686f7374000050000008a006000a4b6565702d416c69766500a00800013000a00100032a2f2a00000f6163636570742d656e636f64696e6700000d677a69702c206465666c61746500000f6163636570742d6c616e6775616765000005656e2d555300a00900ae434649443d65383862343238622532446661353925324434366564253244623863392532443961306366366666666137323b204346544f4b454e3d303b2053455455505355424d4954425554544f4e3d4138303445413043313145433141363137453738423844453641414532424634433b2053455455505355424d4954425554544f4e434f4d504c4554453d41434433463541313646464138353542363743393234444634323141313234393800a00b00096c6f63616c686f737400a00e00974d6f7a696c6c612f342e302028636f6d70617469626c653b204d53494520382e303b2057696e646f7773204e5420362e313b2054726964656e742f342e303b20534c4343323b202e4e455420434c5220322e302e35303732373b202e4e455420434c5220332e352e33303732393b202e4e455420434c5220332e302e33303732393b204d656469612043656e74657220504320362e3029000300000004000000060006616a7031337700ff";
        //dump.jsp
        //private static string myData = "1234019b02020008485454502f312e310000092f64756d702e6a737000000d3137322e31362e33302e31373700000d3137322e31362e33302e31373700000877696e3774657374000050000009a006000a6b6565702d616c69766500000a4b6565702d416c69766500000331313500a00800013000a001003f746578742f68746d6c2c6170706c69636174696f6e2f7868746d6c2b786d6c2c6170706c69636174696f6e2f786d6c3b713d302e392c2a2f2a3b713d302e3800000e4163636570742d4368617273657400001e49534f2d383835392d312c7574662d383b713d302e372c2a3b713d302e3700000f4163636570742d456e636f64696e6700000d677a69702c206465666c61746500000f4163636570742d4c616e677561676500000e656e2d75732c656e3b713d302e3500a00b000877696e377465737400a00e004a4d6f7a696c6c612f352e30202857696e646f7773204e5420362e313b20574f5736343b2072763a322e302e3129204765636b6f2f32303130303130312046697265666f782f342e302e31000300000004000000060006616a7031337700ff";
        private static string myData = "1234008c02020008485454502f312e310000092f64756d702e6a737000001c666538303a3a313566383a346663343a666136633a6364366125313000001c666538303a3a313566383a346663343a666136633a63643661253130000008647462696c616c31000050000001a008000130000900036f6666000200092f64756d702e6a737000060006616a7031337700ff";

        static void Main(string[] args)
        {
            //KeyTest();
            //sample setting
            //Console.WriteLine("Sample Setting " + Properties.Settings.Default.SampleSetting);

            //read parameters from console first parameter is server second port
            if (args.Length >= 1)
            {
                myURL = (string)args[0];
            }
            if (args.Length >= 2) {
                myServer = (string)args[1];
            }
            if (args.Length >= 3)
            {
                myPort = (string)args[2];
            }

            string myDNS = myServer; // +":" + myPort;
            ushort iPort = Convert.ToUInt16(myPort);

            //test search
            byte[] sourceBytes = new byte[20] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A };
            byte[] searchBytes = new byte[2] { 0x02, 0x04 };

            int foundPos = ByteSearch(sourceBytes, searchBytes, 0);

            Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));
            Console.WriteLine(BonCodeAJP13Settings.BONCODEAJP13_LOG_DIR);

            ConnectViaString(myDNS, myData);

             

            //test config
            //Test myTest = new Test();
            //myTest.fHeaderTest();

            //log level
            //Console.WriteLine("Log Level " + BonCodeAJP13Settings.BONCODEAJP13_LOG_LEVEL);

            
            //create intance of forward request
            BonCodeAJP13ForwardRequest FR = new BonCodeAJP13ForwardRequest(BonCodeAJP13HTTPMethods.BONCODEAJP13_GET,
                                        "HTTP/1.1",
                                        myURL,
                                        "::1",
                                        "::1", myServer,iPort ,false,1);

            //create cping request
            BonCodeAJP13ForwardRequest FR2 = new BonCodeAJP13ForwardRequest(BonCodeAJP13HTTPMethods.BONCODEAJP13_GET,
                                       "HTTP/1.1",
                                       myURL,
                                       "::1",
                                       "::1", myServer, iPort, false, 1);

            byte[] testBytes = FR.GetDataBytes();  //this returns the contructed databytes

            //byte[] testBytes = FR.WriteServerTestPacket();
            
            


            Console.WriteLine("Server: {0} , Port: {1}, URL: {2}", myServer, myPort, myURL);

            //call server request
            BonCodeAJP13ServerConnection sconn = new BonCodeAJP13ServerConnection(FR,true);
            sconn.Server = myServer;
            sconn.Port = System.Convert.ToInt32(myPort);
            sconn.FlushDelegateFunction = PrintFlush;  //this function will do the printing to console
            
            

            //run connection
            sconn.BeginConnection();



            
            //write the response to screen that has not been flushed yet           
            foreach (Object oIterate in sconn.ReceivedDataCollection)
            {
                BonCodeAJP13Packet Packet = oIterate as BonCodeAJP13Packet; //only objects derived from this class should be in the collection
                Console.WriteLine(Packet.GetDataString());
            }
            Console.WriteLine("Last Size:" + sconn.ReceivedDataCollection.Count);
            
            //call connect function
            //ConnectViaString(myDNS, myData);
            //Connect(myDNS, testBytes);
            
            int a = 2;
            a++;

        }

        static void KeyTest()
        {
            NameValueCollection nvc = new NameValueCollection();

            nvc.Add("one", "A");
            nvc.Add("two", "AA");
            nvc.Add("three", "AAA");
            

            Console.WriteLine("Key Length" + nvc.AllKeys.Length);

            string b = nvc["two"];
            string c = nvc["four"];


        }

        //this will be passed in as delegate
        static void PrintFlush(BonCodeAJP13PacketCollection flushCollection)
        {
            foreach (TomcatReturn flushPacket in flushCollection)
            {
                //TODO: check by packet type and do different processing before calling flush
                //TomcatSendHeaders tcsh = new TomcatSendHeaders();
                if (flushPacket is TomcatSendHeaders)
                {
                    TomcatSendHeaders tcsh = (TomcatSendHeaders)flushPacket;
                    NameValueCollection tcHeaders = tcsh.GetHeaders();

                    for (int i = 0; i < tcHeaders.AllKeys.Length; i++)
                    {
                        Console.WriteLine(tcHeaders[i]);
                    }

                }
                else
                {   
                    //generic flush of data
                    string outString = flushPacket.GetUserDataString();
                    Console.WriteLine(outString);
                }

                           



            }         
            
            
        }

        //accept a string message instead of byte message
        static void ConnectViaString(string server, string message)
        {
            //call connect method after converting message to bytes
            Byte[] convData = StringToBytes(message);

            Connect(server, convData);

        }

        
        //accept byte message
        static void Connect(string server, byte[] message)
        {
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                Int32 port = 8009;
                TcpClient client = new TcpClient(server, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                //Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                Byte[] data = message;
                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);
               
                Console.WriteLine("Sent bytes: {0}", message.Length);

               

                // buffer to store the response bytes. We will get responses in 8KB packets
                data = new Byte[8196];

                // String to store the response ASCII representation.
                String responseData = String.Empty;
                Int32 bytes;
                // Receive the TcpServer.response. 
                stream.Read(data,0,0); //call empty read so we block until we receive a response
                // Read the first batch of the TcpServer response bytes.
                bool bGoOn = true;
                //try until error
                while (bGoOn && client.Connected)
                {
                    try
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                        Console.WriteLine("Received: {0}", responseData);
                    }
                    catch (Exception)
                    {
                        bGoOn = false;                      
                       
                    }
                }
                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }


        static void MultiPackackeWrite()
        {
            byte[] pack1 = new byte[4] { 0x01, 0x02, 0x03, 0x04 };
            byte[] pack2 = new byte[4] { 0x05, 0x06, 0x07, 0x08 };
            byte[] pack3 = new byte[4] { 0x09, 0x0A, 0x0B, 0x0C };
            
            //client
            TcpClient client = new TcpClient(); //myServer, Convert.ToInt32(myPort));
            client.SendBufferSize=4;
            client.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Expedited, 1);
            client.Connect(myServer, Convert.ToInt32(myPort));
            client.Client.UseOnlyOverlappedIO = true;
            //write to client
            client.Client.Send(pack1);
            
            client.Client.Send(pack2);
            client.Client.Send(pack3);

            client.Close();


        }



        private static byte[] StringToBytes(string inputString)
        {
            // allocate byte array based on half of string length
            int numBytes = (inputString.Length) / 2;
            byte[] bytes = new byte[numBytes];

            // loop through the string - 2 bytes at a time converting it to decimal equivalent and store in byte array
            // x variable used to hold byte array element pos
            for (int x = 0; x < numBytes; ++x)
            {
                bytes[x] = Convert.ToByte(inputString.Substring(x * 2, 2), 16);
            }

            // return the finished byte array of decimal values
            return bytes;
        }

        private static int ByteSearch(byte[] searchIn, byte[] searchBytes, int start = 0)
        {
            int found = -1;
            bool matched = false;
            //only look at this if we have a populated search array and search bytes with a sensible start
            if (searchIn.Length > 0 && searchBytes.Length > 0 && start <= (searchIn.Length - searchBytes.Length) && searchIn.Length >= searchBytes.Length)
            {
                //iterate through the array to be searched
                for (int i = start; i <= searchIn.Length - searchBytes.Length; i++)
                {
                    //if the start bytes match we will start comparing all other bytes
                    if (searchIn[i] == searchBytes[0])
                    {
                        if (searchIn.Length > 1)
                        {
                            //multiple bytes to be searched we have to compare byte by byte
                            matched = true;
                            for (int y = 1; y <= searchBytes.Length - 1; y++)
                            {
                                if (searchIn[i + y] != searchBytes[y])
                                {
                                    matched = false;
                                    break;
                                }
                            }
                            //everything matched up
                            if (matched)
                            {
                                found = i;
                                break;
                            }

                        }
                        else
                        {
                            //search byte is only one bit nothing else to do
                            found = i;
                            break; //stop the loop
                        }

                    }
                }

            }
            return found;
        }



    }
}
