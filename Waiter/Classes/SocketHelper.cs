using System;
using Java.Lang;
using Java.Net;
using SocketIO.Client;
using Waiter.Activities;
using Socket = SocketIO.Client.Socket;

namespace Waiter.Classes
{
    class SocketHelper
    {
        private Socket _socket;

        public Socket GetSocket()
        {
            if (_socket == null)
            {
                try
                {
                    IO.Options opts = new IO.Options();
                    opts.ForceNew = true;

                    _socket = IO.Socket(MainActivity.WebApi, opts);
                    _socket.On(Socket.EventConnect, data =>
                    {
                        Console.WriteLine("SOCKET CONNECTED TO " + MainActivity.WebApi);
                    });
                }
                catch (URISyntaxException e)
                {
                    Console.WriteLine(e);
                    throw new RuntimeException();
                }
            }
            return _socket;
        }
    }
}