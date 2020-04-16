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
        public long Timestamp { get; set; }
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

        public string GetDatetimeString()
        {
            var date = DateTimeOffset.FromUnixTimeMilliseconds(Timestamp);
            return date.ToString("o");
        }

        public string GetLevel()
        {
            switch (Level)
            {
                case LogEventLevel.Verbose:
                    return "V";
                case LogEventLevel.Debug:
                    return "D";
                case LogEventLevel.Information:
                    return "I";
                case LogEventLevel.Warning:
                    return "W";
                case LogEventLevel.Error:
                    return "E";
                case LogEventLevel.Fatal:
                    return "F";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string ToString()
        {
            return $"{GetDatetimeString()}|{GetLevel()}|{Machine}|{Application}|{Category}|{Method}|{Message}|{Exception}";
        }
    }
}