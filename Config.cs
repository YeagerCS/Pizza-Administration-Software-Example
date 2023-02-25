using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Pizzer
{
    public class Config
    {
        public IMongoDatabase getConnection()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("PizzaDb");

            return database;
        }
    }
}
