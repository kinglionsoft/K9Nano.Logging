namespace LogCenter.Web.Collector
{
    public sealed class ServerOptions
    {
        public bool UseLibuv { get; set; } = false;

        public int Port { get; set; } = 6253;
    }
}