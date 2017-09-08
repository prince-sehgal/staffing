using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Web;
using System.Configuration;

namespace StaffingSite.MongoDB
{
    public class MongoDBConnector : IDisposable
    {
        MongoClient Client = null;
        MongoServer Server = null;
        MongoDatabase Database = null;

        public MongoDatabase GetDatabase()
        {
            if (Database == null)
            {
                Client = new MongoClient(string.Format("mongodb://{0}", ConfigurationManager.AppSettings["MongoDBServer"]));
                Server = Client.GetServer();
                Database = Server.GetDatabase("staffing");
            }
            return Database;
        }

        public void Dispose()
        {
            Server.Disconnect();
            GC.SuppressFinalize(this);
        }
    }
}