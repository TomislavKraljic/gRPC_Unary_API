using Dummy;
using Greet;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    class Program
    {
        const string target = "127.0.0.1:50051";
        static async Task Main(string[] args)
        {
            Channel channel = new Channel(target, ChannelCredentials.Insecure);

            await channel.ConnectAsync().ContinueWith((task) =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                    Console.WriteLine("The client connected successfully");
            });

            var client = new GreetingService.GreetingServiceClient(channel);

            var greeting = new Greeting()
            {
                FirstName = "Tomislav",
                LastName = "Kraljic"
            };

            // Unary
            //var request = new GreetingRequest() { Greeting = greeting };

            //var response = client.Greet(request);


            // Server Streaming
            //var request = new GreetManyTimesRequest() { Greeting = greeting };
            //var response = client.greetManyTimes(request);

            //while ( await response.ResponseStream.MoveNext())
            //{
            //    Console.WriteLine(response.ResponseStream.Current.Result);
            //    await Task.Delay(300);
            //}

            //Console.WriteLine(response.Result);


            //  Client Streaming
            var request = new LongGreetRequest() { Greeting = greeting };
            var stream = client.LongGreet();

            foreach(int i in Enumerable.Range(1, 10))
            {
                await stream.RequestStream.WriteAsync(request);
            }

            await stream.RequestStream.CompleteAsync();

            var response = await stream.ResponseAsync;

            Console.WriteLine(response.Result); ;

            channel.ShutdownAsync().Wait();

            Console.ReadKey();
        }
    }
}
