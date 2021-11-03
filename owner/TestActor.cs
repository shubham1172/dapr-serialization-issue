using System;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using Dapr.Client;

namespace owner
{
    public interface ITestActor : IActor
    {
        Task PublishTestDataAsync();
    }

    public record TestData
    {
        public int int32 { get; set; }
        public uint uint32 { get; set; }
        public long int64 { get; set; }
        public ulong uint64 { get; set; }
        public string base64long { get; set; }
        public string base64int { get; set; }
    }

    internal class TestActor : Actor, ITestActor
    {
        private readonly DaprClient _client;
        public TestActor(ActorHost host, DaprClient client)
            : base(host)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task PublishTestDataAsync()
        {
            var rnd = new Random();
            for (var i = 0; i < 5; i++)
            {
                var r = CreateRecord();
                Console.WriteLine($"Publishing record {i+1}/5: {System.Text.Json.JsonSerializer.Serialize(r)}");
                await _client.PublishEventAsync("pubsub", "TESTTOPIC", r);
                await Task.Delay(TimeSpan.FromSeconds(rnd.NextDouble() + 0.1));
            }
        }

        private TestData CreateRecord()
        {
            var time64 = DateTime.Now.Ticks;
            var time32 = (int)(time64 % int.MaxValue);

            return new TestData
            {
                int32 = time32,
                uint32 = (uint)time32,
                int64 = time64,
                uint64 = (ulong)time64,
                base64long = Convert.ToBase64String(BitConverter.GetBytes(time64)),
                base64int = Convert.ToBase64String(BitConverter.GetBytes(time32)),
            };
        }
    }
}