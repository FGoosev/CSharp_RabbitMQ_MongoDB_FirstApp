using MongoDB.Bson;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var db = mongoClient.GetDatabase("bigData");
            var collectionStudents = db.GetCollection<BsonDocument>("Students");

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
                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (sender, e) =>
                {
                    var body = e.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    var bson = BsonDocument.Parse(message);
                    collectionStudents.InsertOne(bson);
                    Console.WriteLine("Get message: {0}", message);
                };
                channel.BasicConsume(queue: "dev-q",
                                     autoAck: true,
                                     consumer: consumer
                    );

                Console.WriteLine("Subscribed to the queue dev-q");
                Console.ReadLine();
            }
        }
    }
}
