namespace producer
{
	using System;
	using System.Text;
	using RabbitMQ.Client;

	class Program
	{
		private static int index = 0;
		private static byte b;
		static void Main(string[] args)
		{
			var factory = new ConnectionFactory { HostName = "localhost" };
			using (var connection = factory.CreateConnection())
			{
				using (var channel = connection.CreateModel())
				{
					channel.QueueDeclare("task_queue", true, false, false, null);

					for (var i = 0; i < 1000; i++)
					{
						b = (byte)(index % 9);
						var message = GetMessage(args);
						var body = Encoding.UTF8.GetBytes(message);

						var properties = channel.CreateBasicProperties();
						properties.Priority = b;//(byte)(index % 9);
						properties.DeliveryMode = 2;
						properties.SetPersistent(true);

						channel.BasicPublish("", "task_queue", properties, body);
						Console.WriteLine(" [x] Sent {0}", message);
						index++;
					}
				}
			}
		}

		private static string GetMessage(string[] args)
		{
			return ((args.Length > 0) ? string.Join(" ", args) : b + " Hello World!");
		}
	}
}
