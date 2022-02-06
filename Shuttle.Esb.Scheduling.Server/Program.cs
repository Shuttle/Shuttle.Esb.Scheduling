using System.Data.Common;
using System.Data.SqlClient;
using Shuttle.Core.ServiceHost;

namespace Shuttle.Esb.Scheduling.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);

            ServiceHost.Run<Host>();
        }
    }
}