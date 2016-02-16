﻿using System;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Rebus.Rpc.Tests;

namespace Rebus.Rpc.Tests
{
    [TestFixture]
    public class ClientServerTest : IsolationTestBase
    {
        [Test]
        public void PingPongTest()
        {
            // Given
            var server = New<Server>() as IServer;
            server.Start("1");
            server.On<Ping>(req => new Pong { Payload = req.Payload });
            var client = New<Client>() as IClient;
            client.Start("1");

            // When
            var resp = (Pong)client.Ask(new Ping { Payload = "123" });

            // Then
            Assert.AreEqual("123", resp.Payload);
        }

        [Serializable]
        public class Ping { public string Payload; }
        [Serializable]
        public class Pong { public string Payload; }

        [Test]
        public void ShardingTest()
        {
            // Given
            var server1 = New<Server>() as IServer;
            server1.Start("1");
            server1.On<ShardRequest>(req => req.Shard == "1" ? new ShardResponse { Payload = req.Payload + "1" } : null);
            var server2 = New<Server>() as IServer;
            server2.Start("2");
            server2.On<ShardRequest>(req => req.Shard == "2" ? new ShardResponse { Payload = req.Payload + "2" } : null);
            var client = New<Client>() as IClient;
            client.Start("1");

            // When
            var resp1 = (ShardResponse)client.Ask(new ShardRequest { Shard = "1", Payload = "A" });
            var resp2 = (ShardResponse)client.Ask(new ShardRequest { Shard = "2", Payload = "B" });

            // Then
            Assert.AreEqual("A1", resp1.Payload);
            Assert.AreEqual("B2", resp2.Payload);
        }

        [Serializable]
        public class ShardRequest { public string Shard; public string Payload; }
        [Serializable]
        public class ShardResponse { public string Payload; }

        [Test]
        public void SeveralClientsPerOneServerTest()
        {
            // Given
            var server1 = New<Server>() as IServer;
            server1.Start("1");
            server1.On<Ping>(req => new Pong { Payload = req.Payload });
            var client1 = New<Client>() as IClient;
            client1.Start("1");
            var client2 = New<Client>() as IClient;
            client2.Start("2");

            // When
            var resp1 = (Pong)client1.Ask(new Ping { Payload = "A" });
            var resp2 = (Pong)client2.Ask(new Ping { Payload = "B" });

            // Then
            Assert.AreEqual("A", resp1.Payload);
            Assert.AreEqual("B", resp2.Payload);
        }
    }
}
