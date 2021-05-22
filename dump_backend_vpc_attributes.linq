<Query Kind="Program" />

using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;
using System.Threading.Tasks;

async void Main() {
    using (var dumper = new VpcDump()) {
        await dumper.Vpc(new DumpMetadata {
            name = "backend-vpc",
            rogicalName = "backend VPC"
        });

        await dumper.Subnet(nameAsFilter: "backend-vpc-private-subnet*", new DumpMetadata {
            name = "backend-vpc-private-subnet1,2",
            rogicalName = "backend VPC subnet"
        });

        await dumper.Routetable(new DumpMetadata {
            name = "backend-vpc-routetable",
            rogicalName = "backend VPC route table"
        });

        await dumper.InternetGateway(new DumpMetadata {
            name = "backend-vpc-igw",
            rogicalName = "backend VPC Internet Gateway"
        });

        await dumper.Endpoint(new DumpMetadata {
            name = "gateway-vpc-endpoint-for-s3",
            rogicalName = "endpoint for s3"
        });

        await dumper.Endpoint(nameAsFilter: "rds-proxy-endpoint*", new DumpMetadata {
            name = "rds-proxy-endpoint-001,002",
            rogicalName = "endpoint for RDS Proxy"
        });

        await dumper.Endpoint(new DumpMetadata {
            name = "secretmanager-endpoint",
            rogicalName = "endpoint for Secrets Manager"
        });

        await dumper.SecurityGroups(vpcId: "vpc-xxx", new DumpMetadata {
            name = "security group",
            rogicalName = "security group for backend VPC"
        });
    }
}
