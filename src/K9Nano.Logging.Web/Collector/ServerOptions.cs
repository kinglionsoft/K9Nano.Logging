﻿namespace K9Nano.Logging.Web.Collector
{
    public sealed class ServerOptions
    {
        public int Port { get; set; } = 6253;

        public int KeepDays { get; set; } = 30;
    }
}