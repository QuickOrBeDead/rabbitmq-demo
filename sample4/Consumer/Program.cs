﻿using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var sleepMs = 0;
if (args.Length > 0)
{
    _ = int.TryParse(args[0], out sleepMs);
}

var factory = new ConnectionFactory { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: "sample4",
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: new Dictionary<string, object> { { "x-single-active-consumer", true } });

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("Received '{0}'", message);

            Thread.Sleep(sleepMs);
        };
    channel.BasicConsume(queue: "sample4", autoAck: true, consumer: consumer);

    Console.WriteLine("Press [enter] to exit.");
    Console.ReadLine();
}