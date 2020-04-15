using System;
using System.Security.Cryptography;
using Serilog.Debugging;
using Serilog.Events;

namespace K9Nano.Logging
{
    public abstract class SerializerBase : ISerializer
    {
        protected const string SourceContextPropertyName = "SourceContext";
        protected const string ThreadIdPropertyName = "ThreadId";
        protected const string ApplicationPropertyName = "Application";
        protected const string MethodPropertyName = "Method";
        protected const string MachineNamePropertyName = "MachineName";


        public abstract LogEntity Deserialize(byte[] data);
        public abstract byte[] Serialize(LogEntity entity);

        public virtual bool TryDeserialize(byte[] data, out LogEntity result)
        {
            try
            {
                result = Deserialize(data);
                return true;
            }
            catch (Exception e)
            {
                SelfLog.WriteLine($"Deserialize failed: {e}");
                result = null;
                return false;
            }
        }

        public virtual LogEntity Map(LogEvent logEvent)
        {
            var entity = new LogEntity
            {
                Timestamp = logEvent.Timestamp.ToUnixTimeMilliseconds(),
                Level = logEvent.Level
            };

            entity.Message = logEvent.RenderMessage();

            entity.Exception = FormatException(logEvent.Exception);
            
            if (logEvent.Properties.TryGetValue(SourceContextPropertyName, out var sourceContext))
            {
                var sourceContextValue = ((ScalarValue)sourceContext).Value.ToString();
                entity.Category = sourceContextValue;
            }

            if (logEvent.Properties.TryGetValue(ThreadIdPropertyName, out var threadId))
            {
                var threadIdValue = ((ScalarValue)threadId).Value.ToString();
                entity.ThreadId = threadIdValue;
            }

            if (logEvent.Properties.TryGetValue(MethodPropertyName, out var methodName))
            {
                var methodNameValue = ((ScalarValue)methodName).Value.ToString();
                entity.Method = methodNameValue;
            }

            if (logEvent.Properties.TryGetValue(MachineNamePropertyName, out var machineName))
            {
                var machineNameValue = ((ScalarValue)machineName).Value.ToString();
                entity.Machine = machineNameValue;
            }

            // todo
            if (logEvent.Properties.TryGetValue(ApplicationPropertyName, out var appName))
            {
                var appNameValue = ((ScalarValue)appName).Value.ToString();
                entity.Application = appNameValue;
            }

            return entity;
        }

        protected virtual string FormatException(Exception ex)
        {
            if (ex == null) return null;
            return ex.Message;
        }
    }
}