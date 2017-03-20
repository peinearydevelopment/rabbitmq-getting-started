namespace consumer
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Threading;
	using RabbitMQ.Client;

	class Program
	{
		static void Main(string[] args)
		{
			var factory = new ConnectionFactory { HostName = "localhost" };
			using (var connection = factory.CreateConnection())
			{
				using (var channel = connection.CreateModel())
				{
					channel.QueueDeclare("task_queue", true, false, false, null);
					channel.BasicQos(0, 1, false);

					var consumer = new QueueingBasicConsumer(channel);
					//var arguments = new Dictionary<string, object> { { "x-priority", 10 } };
					//channel.BasicConsume("task_queue", true, "consumer-tag", arguments, consumer);
					channel.BasicConsume("task_queue", true, consumer);

					Console.WriteLine(" [*] Waiting for messages. \n To exit press CTRL+C");

					while (true)
					{
						var ea = consumer.Queue.Dequeue();

						var body = ea.Body;
						var message = Encoding.UTF8.GetString(body);
						Console.WriteLine(" [x] Received {0}", message);

						var dots = message.Split('.').Length - 1;
						Thread.Sleep(1 * 1000);

						Console.WriteLine(" [x] Done");

						//channel.BasicAck(ea.DeliveryTag, true);
					}
				}
			}
		}
	}
}
