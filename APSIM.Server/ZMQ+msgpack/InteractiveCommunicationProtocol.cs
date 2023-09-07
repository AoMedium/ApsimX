using APSIM.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using NetMQ;
using NetMQ.Sockets;
using Models.Core;
using System.Dynamic;
using Models;

namespace APSIM.ZMQServer.IO
{
    /// <summary>
    /// This class handles the communications protocol .
    /// </summary>
    public class InteractiveComms : ICommProtocol
    {
        private const int protocolVersionMajor = 2; // Increment every time there is a breaking protocol change
        private const int protocolVersionMinor = 0; // Increment every time there is a non-breaking protocol change, set to 0 when the major version changes

        private GlobalServerOptions options;

        /// <summary>
        /// Create a new <see cref="ZMQCommunicationProtocol" /> instance which uses the
        /// specified connection stream.
        /// </summary>
        /// <param name="conn"></param>
        public InteractiveComms(GlobalServerOptions _options)
        {
            options = _options;
        }

        /// <summary>
        /// Wait for a command from the connected clients.
        /// </summary>
        public void doCommands(ApsimEncapsulator apsim)
        {
            while (true) 
            {
                try
                {
                        string[] args = {"[Synchroniser].Script.Identifier = " + options.IPAddress + ":" + options.Port};
                        Console.WriteLine("args=" + args[0]);
                        apsim.Run(args);
                        apsim.WaitForStateChange();
                        if (apsim.getErrors()?.Count > 0)
                        {
                            throw new AggregateException("Simulation Error", apsim.getErrors());
                        }
                }
            catch (Exception ex)
            {
                string msgBuf = "ERROR\n" + ex.ToString();
                if (options.Verbose) { Console.WriteLine(msgBuf); }
            }
            break; // temporary fixme
            }
        }
        public void Dispose()
        {
        }
    }
}
