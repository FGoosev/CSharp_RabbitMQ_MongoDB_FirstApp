using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using MongoDB.Bson;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var counter = 0;
            var random = new Random();
            var ages = new List<int> { 17,18,19,20 };
            var facults = new List<string> { "InPIT", "InETIP", "FTI", "URBAS" };
            var names = new List<string> { "Ivan", "Petr", "Stepan", "Anna" };
            var surnames = new List<string> { "Petrov", "Ivanov", "Prodov", "Gresulov" };
            do
            {
                int timeToSlip = new Random().Next(1000,3000);
                Thread.Sleep(timeToSlip);
                int indexAge = random.Next(ages.Count);
                int indexName = random.Next(names.Count);
                int indexSurname = random.Next(surnames.Count);
                int indexFacult = random.Next(facults.Count);

                var document = new BsonDocument()
                {
                    { "name", "MongoDB" },
                    { "type", "Database" },
                    { "count", 1 },
                    { "info", new BsonDocument
                    {
                        { "Name", names[indexName] },
                        { "Surname", surnames[indexSurname] },
                        { "Age", ages[indexAge] },
                        { "University", "SSTU" },
                        { "faculty", facults[indexFacult] }
                    }}
                };
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "dev-q",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null
                        );
                    

                    var body = Encoding.UTF8.GetBytes(document.ToString());
                    channel.BasicPublish(exchange: "",
                                         routingKey: "dev-q",
                                         basicProperties: null,
                                         body: body
                        );

                    Console.WriteLine("Message publish");
                }
            } while (true);
            
        }
    }
}
