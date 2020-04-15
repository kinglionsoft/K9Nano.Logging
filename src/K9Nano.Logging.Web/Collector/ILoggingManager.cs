namespace K9Nano.Logging.Web.Collector
{
    public interface ILoggingManager
    {
        void Post(byte[] data);
    }
}