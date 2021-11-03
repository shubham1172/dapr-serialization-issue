using System;
using System.Linq;
using System.Threading.Tasks;
using Dapr;
using Microsoft.AspNetCore.Mvc;

namespace replicaone
{
    public class TestController
    {
        [Topic("pubsub", "TESTTOPIC")]
        [HttpPost("testevent")]
        public Task<ActionResult> TestEvent([FromBody] owner.TestData record)
        {
            Console.WriteLine($"Got event            : {System.Text.Json.JsonSerializer.Serialize(record)}");
            return Task.FromResult<ActionResult>(new OkResult());
        }
    }
}