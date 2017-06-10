using DB;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace communication.Core
{
    public static class ServerWiring
    {
        //private static ServerWiring instance = new ServerWiring();
        private static IServer server = null;
        private static object syncObject = new object();

        public static void initServer(IMedTrainDBContext db)
        {
            lock (syncObject)
            {
                if (server == null)
                {
                    server = new ServerImpl(db);
                }
            }
        }

        public static IServer getInstance()
        {
            lock (syncObject)
            {
                if (server == null)
                {
                    server = new ServerImpl(new FakeMedTrainDBContext(1));
                }
            }
            return server;
        }
    }
}