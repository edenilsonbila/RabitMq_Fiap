using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabitMq_Fiap
{
    class Program
    {
        static void Main(string[] args)
        {


            var factory = new ConnectionFactory();

            factory.UserName = "";
            factory.Password = "";
            factory.HostName = "";
            factory.Port = 5672;
            factory.VirtualHost = "/";
            factory.ConsumerDispatchConcurrency = 10;//Quantidade de threads maximas, por padrao é 1

            var connection = factory.CreateConnection();//Usar com Singleton


            LerMensagem(connection);


            Console.ReadKey();
        }

        public static void EnviarMensagens(IConnection connection)
        {
            using (var channel = connection.CreateModel())
            {

                for (int i = 0; i < 100000; i++)
                {
                    var body = Encoding.UTF8.GetBytes("mensagem de texto: " + i);
                    channel.BasicPublish("e-denilson", routingKey: "", body: body);
                }

            }
        }

        public static void LerMensagem(IConnection connection)
        {
            var channel = connection.CreateModel();
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += Consumidor_Received;

            channel.BasicConsume("fiap", false, consumer);
        }


        private static void Consumidor_Received(object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body.Span);

            Console.WriteLine("Mensagem recebida: " + message);

            var channel = ((EventingBasicConsumer)sender).Model;

            channel.BasicAck(e.DeliveryTag, multiple: false);
        }
    }
}
