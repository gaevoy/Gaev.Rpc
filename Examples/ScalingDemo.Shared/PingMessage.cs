﻿namespace Gaev.RebbitMqCommandBus.Shared
{
    public class PingMessage
    {
        public string Sender { get; set; }
        public string Payload { get; set; }
    }
}
