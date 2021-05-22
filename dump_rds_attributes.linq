<Query Kind="Program" />

using Amazon.RDS;
using Amazon.RDS.Model;
using Amazon.Runtime;
using System.Threading.Tasks;

async void Main() {
    using (var dumper = new RdsDump()) {
        await dumper.RdsCluster(new DumpMetadata {
            name = "rds-cluster",
            rogicalName = "rds cluster"
        });

        await dumper.RdsInstances(dbClusterIdentifier: "rds-cluster", new DumpMetadata {
            name = "rds-cluster instances",
            rogicalName = "RDS instances"
        });

        await dumper.RdsProxy(new DumpMetadata {
            name = "rds-proxy",
            rogicalName = "RDS Proxy"
        });
    }
}
