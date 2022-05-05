
using Amazon;
using Amazon.Kinesis.Model;
using Amazon.Runtime;
using EasyNetQ;
using Shared.Models;
using SmileTV.Models;
using System.Text;

bool running = true;

while (running)
{
    Console.Write("Enter Command (or Menu Name): ");
    var menu = Console.ReadLine();

    if (menu == "quit")
        break;
    else if (menu == "?")
    {
        ShowHelp();
        continue;
    }
    else if (menu == "cls")
    {
        Console.Clear();
        continue;
    }
    else if (menu == "kinesis")
    {
        await InitialKinesis();
        continue;
    }
    else if (menu.StartsWith("random"))
    {
        string[] nums = menu.Split(' ');
        int min = 5;
        int max = 100;

        if (nums.Length > 1)
        {
            min = int.Parse(nums[1]);
            max = int.Parse(nums[2]);
        }

        GetRandom(min, max);
        continue;
    }
    else if (menu.StartsWith("asyncrandom"))
    {
        string[] nums = menu.Split(' ');
        int min = 5;
        int max = 100;

        if (nums.Length > 1)
        {
            min = int.Parse(nums[1]);
            max = int.Parse(nums[2]);
        }

        GetRandomAsync(min, max);
        Console.WriteLine("This First");
        continue;
    }
    else if (menu.StartsWith("recieve"))
    {
        using (var bus = RabbitHutch.CreateBus("host=localhost"))
        {
            bus.SendReceive.Send<RecieveOne>("recieve-one", new Shared.Models.RecieveOne() { Name = "Ben" });
            bus.SendReceive.Send<RecieveTwo>("recieve-one", new Shared.Models.RecieveTwo() { NickName = "TBen" });
        }
        continue;
    }
    else if (menu.StartsWith("topic"))
    {
        using (var bus = RabbitHutch.CreateBus("host=localhost"))
        {
            bus.PubSub.Publish<TopicOne>(new TopicOne() { Name = "Topic One" }, "topic.topic-one");
            bus.PubSub.Publish<TopicOne>(new TopicOne() { Name = "Topic Two" }, "topic.topic-two");
        }
        continue;
    }

    Console.Write("Enter Button Position: ");
    var pos = Console.ReadLine();

    Console.Write("Enter New Caption: ");
    var caption = Console.ReadLine();

    using (var bus = RabbitHutch.CreateBus("host=localhost"))
    {
        var msg = new ButtonUpdate() { menuName = menu, pos = int.Parse(pos), newCaption = caption };
        bus.PubSub.Publish<ButtonUpdate>(msg);
    }
}

Console.WriteLine("Press Any Key To Exit");
Console.ReadKey();

async Task InitialKinesis()
{
    //arn:aws:kinesis:eu-west-2:587376775772:stream/bens-stream

    Amazon.Kinesis.AmazonKinesisClient client = new Amazon.Kinesis.AmazonKinesisClient(RegionEndpoint.EUWest2);

    byte[] bytes = Encoding.UTF8.GetBytes("Message From Ben at: " + DateTime.Now.ToShortTimeString());

    PutRecordRequest request = new PutRecordRequest();
    request.StreamName = "bens-stream";
    request.PartitionKey = "part01";

    MemoryStream stream = new MemoryStream(bytes);
    request.Data = stream;

    PutRecordResponse response = await client.PutRecordAsync(request);
    Console.WriteLine("part01 -> " + response.ShardId);
}   

void ShowHelp()
{
    Console.WriteLine("Options");
    Console.WriteLine("no option - enter Button Menu Name followed by Buttom Position followed by new Caption to Update buttons (EastNetQ Publish/Subscribe).");
    Console.WriteLine("'random [min] [max]' - use EasyNetQ RPC to get random number between min and max options.");
    Console.WriteLine("'asyncrandom [min] [max]' - use EasyNetQ RPC to get random number between min and max options (async).");
    Console.WriteLine("'recieve' - use EasyNetQ Send/Recieve to Send and Recieve queue.");
    Console.WriteLine("'topic' - use EasyNetQ Publish/Subscribe to topic based queue.");
    Console.WriteLine();
}

void GetRandom(int min, int max)
{
    using (var bus = RabbitHutch.CreateBus("host=localhost; timeout=60"))
    {
        RandomRequest msg = new RandomRequest()
        {
            Min = min,
            Max = max
        };

        var resp = bus.Rpc.Request<RandomRequest, RandomResponse>(msg);

        Console.WriteLine("Number: " + resp.Number);
    }
}

async void GetRandomAsync(int min, int max)
{
    using (var bus = RabbitHutch.CreateBus("host=localhost; timeout=10"))
    {
        RandomRequest msg = new RandomRequest()
        {
            Min = min,
            Max = max
        };

        var resp = await bus.Rpc.RequestAsync<RandomRequest, RandomResponse>(msg);

        Console.WriteLine("Number: " + resp.Number);
    }
}