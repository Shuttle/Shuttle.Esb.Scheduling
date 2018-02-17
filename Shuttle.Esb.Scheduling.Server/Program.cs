using Shuttle.Core.ServiceHost;

namespace Shuttle.Esb.Scheduling.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ServiceHost.Run<Host>();
        }
    }
}