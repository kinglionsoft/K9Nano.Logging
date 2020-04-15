using System;
using ProtoBuf;
using Serilog.Events;

namespace K9Nano.Logging
{
    [ProtoContract]
    [Serializable]
    public class LogEntity
    {
        [ProtoMember(1)]
        public LogEventLevel Level { get; set; }
        [ProtoMember(2)]
        public string Timestamp { get; set; }
        [ProtoMember(3)]
        public string Machine { get; set; }
        [ProtoMember(4)]
        public string Application { get; set; }
        [ProtoMember(5)]
        public string Category { get; set; }
        [ProtoMember(6)]
        public string Method { get; set; }
        [ProtoMember(7)]
        public string Message { get; set; }
        [ProtoMember(8)]
        public string Exception { get; set; }
        [ProtoMember(9)]
        public string ThreadId { get; set; }

        public override string ToString()
        {
            return $"{Timestamp} {Level} {Machine} {Application} {Category} {Method} {Message} {Exception}";
        }
    }
}