<Query Kind="Expression">
  <Namespace>Amazon.EC2</Namespace>
  <Namespace>Amazon.EC2.Model</Namespace>
  <Namespace>Amazon.RDS</Namespace>
  <Namespace>Amazon.RDS.Model</Namespace>
  <Namespace>Amazon.S3</Namespace>
  <Namespace>Amazon.S3.Model</Namespace>
  <Namespace>Amazon.KeyManagementService</Namespace>
  <Namespace>Amazon.KeyManagementService.Model</Namespace>
  <Namespace>Amazon.SecretsManager</Namespace>
  <Namespace>Amazon.SecretsManager.Model</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

void Main()
{
	// Write code to test your extensions here. Press F5 to compile and run.
}

public static class MyExtensions
{
	// Write custom extension methods here. They will be available to all queries.

}

public class DumpMetadata
{
	public string name;
	public string rogicalName;
	public string description;

	public string DumpCaption
	{
		get
		{
			var r = string.IsNullOrWhiteSpace(rogicalName) ? "" : $" ({rogicalName})";
			return $"{name}{r} {description}";
		}
	}
}

public class Ec2Dump : IDisposable
{
	private AmazonEC2Client ec2Client = new();

	public async Task Ec2(DumpMetadata metadata)
	{
		await Ec2(metadata.name, metadata);
	}

	public async Task Ec2(string name, DumpMetadata metadata)
	{
		var request = new DescribeInstancesRequest()
		{
			Filters = new NameFilter(name)
		};

		var response = await ec2Client.DescribeInstancesAsync(request);
		response.Reservations.First().Instances.First().Dump(metadata.DumpCaption);
	}

	public void Dispose()
	{
		ec2Client?.Dispose();
	}
}

public class SecretsManagerDump : IDisposable
{
	private AmazonSecretsManagerClient secretsManagerClient = new();

	public async Task Secret(DumpMetadata metadata)
	{
		await Secret(metadata.name, metadata);
	}

	public async Task Secret(string name, DumpMetadata metadata)
	{
		var request = new DescribeSecretRequest
		{
			SecretId = name
		};

		var response = await secretsManagerClient.DescribeSecretAsync(request);
		response.Dump(metadata.DumpCaption);
	}

	public void Dispose()
	{
		secretsManagerClient?.Dispose();
	}
}

public class KmsDump : IDisposable
{
	private AmazonKeyManagementServiceClient kmsClient = new();

	public async Task Keys(DumpMetadata metadata)
	{
		// Aliases
		var listAliasesRequest = new ListAliasesRequest()
		{
			
		};

		var aliasesResponse = await kmsClient.ListAliasesAsync(listAliasesRequest);
		var cmkAliases = aliasesResponse.Aliases.Where(a => a.AliasName.StartsWith("alias/aws/") == false);
		cmkAliases.Dump(metadata.DumpCaption);
		
		// Key list
		var listRequest = new ListKeysRequest()
		{
			
		};

		var listResponse = await kmsClient.ListKeysAsync(listRequest);
		
		// details per key
		foreach (var key in listResponse.Keys)
		{
			var keyRequest = new DescribeKeyRequest()
			{
				KeyId = key.KeyId
			};

			var keyResponse = await kmsClient.DescribeKeyAsync(keyRequest);
			var keyMetadata = keyResponse.KeyMetadata;

			// exclude aws management keys
			if (keyMetadata.KeyManager == KeyManagerType.CUSTOMER)
			{
				var dumpMetadata = new DumpMetadata() {
					name = cmkAliases.First(k => k.TargetKeyId == keyMetadata.KeyId).AliasName,
					rogicalName = keyMetadata.Description
				};
				
				await this.Key(key.KeyId, dumpMetadata);
			}
		}
	}

	public async Task Key(string id, DumpMetadata metadata)
	{
		var keyRequest = new DescribeKeyRequest()
		{
			KeyId = id
		};

		var keyResponse = await kmsClient.DescribeKeyAsync(keyRequest);
		keyResponse.KeyMetadata.Dump(metadata.DumpCaption);
	}

	public void Dispose()
	{
		kmsClient?.Dispose();
	}
}

public class RdsDump : IDisposable
{
	private AmazonRDSClient rdsClient = new();

	public async Task RdsCluster(DumpMetadata metadata)
	{
		await RdsCluster(metadata.name, metadata);
	}

	public async Task RdsCluster(string dbCluterIdentifier, DumpMetadata metadata)
	{
		var request = new DescribeDBClustersRequest
		{
			DBClusterIdentifier = dbCluterIdentifier
		};

		var response = await rdsClient.DescribeDBClustersAsync(request);
		response.DBClusters.Dump(metadata.DumpCaption);
	}

	public async Task RdsInstances(DumpMetadata metadata)
	{
		await RdsInstances(metadata.name, metadata);
	}

	public async Task RdsInstances(string dbClusterIdentifier, DumpMetadata metadata)
	{
		var request = new DescribeDBInstancesRequest
		{
			Filters = new DbClusterIdentifierFilter(dbClusterIdentifier)
		};

		var response = await rdsClient.DescribeDBInstancesAsync(request);
		response.DBInstances.Dump(metadata.DumpCaption);
	}

	public async Task RdsProxy(DumpMetadata metadata)
	{
		await RdsProxy(metadata.name, metadata);
	}

	public async Task RdsProxy(string name, DumpMetadata metadata)
	{
		var request = new DescribeDBProxiesRequest
		{
			DBProxyName = name
		};

		var response = await rdsClient.DescribeDBProxiesAsync(request);
		response.DBProxies.Dump(metadata.DumpCaption);
	}

	class DbClusterIdentifierFilter : List<Amazon.RDS.Model.Filter>
	{
		public DbClusterIdentifierFilter(string dbClusterIdentifier)
		{
			this.Add(new Amazon.RDS.Model.Filter { Name = "db-cluster-id", Values = new List<string> { dbClusterIdentifier } });
		}
	}

	public void Dispose()
	{
		rdsClient?.Dispose();
	}
}

public class S3Dump : IDisposable
{
	private AmazonS3Client s3Client = new();
	
	public async Task Buckets(DumpMetadata metadata)
	{
		var response = await s3Client.ListBucketsAsync();
		response.Buckets.Dump(metadata.DumpCaption);	
	}

	public void Dispose()
	{
		s3Client?.Dispose();
	}
}

public class VpcDump : IDisposable
{
	private AmazonEC2Client ec2Client = new();
	
	public async Task Vpc(DumpMetadata metadata)
	{
		await Vpc(metadata.name, metadata);
	}

	public async Task Vpc(string nameAsFilter, DumpMetadata metadata)
	{
		var request = new DescribeVpcsRequest()
		{
			Filters = new NameFilter(nameAsFilter)
		};

		var response = await ec2Client.DescribeVpcsAsync(request);
		response.Vpcs.First().Dump(metadata.DumpCaption);
	}

	public async Task Subnet(DumpMetadata metadata)
	{
		await Subnet(metadata.name, metadata);
	}

	public async Task Subnet(string nameAsFilter, DumpMetadata metadata)
	{
		var request = new DescribeSubnetsRequest
		{
			Filters = new NameFilter(nameAsFilter)
		};

		var response = await ec2Client.DescribeSubnetsAsync(request);
		response.Subnets.Dump(metadata.DumpCaption);
	}

	public async Task Endpoint(DumpMetadata metadata)
	{
		await Endpoint(metadata.name, metadata);
	}

	public async Task Endpoint(string nameAsFilter, DumpMetadata metadata)
	{
		var request = new DescribeVpcEndpointsRequest
		{
			Filters = new NameFilter(nameAsFilter)
		};

		var response = await ec2Client.DescribeVpcEndpointsAsync(request);
		response.VpcEndpoints.Dump(metadata.DumpCaption);
	}

	public async Task Routetable(DumpMetadata metadata)
	{
		await Routetable(metadata.name, metadata);
	}

	public async Task Routetable(string nameAsFilter, DumpMetadata metadata)
	{
		var request = new DescribeRouteTablesRequest
		{
			Filters = new NameFilter(nameAsFilter)
		};

		var response = await ec2Client.DescribeRouteTablesAsync(request);
		response.RouteTables.Dump(metadata.DumpCaption);
	}

	public async Task InternetGateway(DumpMetadata metadata)
	{
		await InternetGateway(metadata.name, metadata);
	}

	public async Task InternetGateway(string nameAsFilter, DumpMetadata metadata)
	{
		var request = new DescribeInternetGatewaysRequest
		{
			Filters = new NameFilter(nameAsFilter)
		};

		var response = await ec2Client.DescribeInternetGatewaysAsync(request);
		response.InternetGateways.Dump(metadata.DumpCaption);
	}

	public async Task SecurityGroups(string vpcId, DumpMetadata metadata)
	{
		var request = new DescribeSecurityGroupsRequest
		{
			Filters = new VpcIdFilter(vpcId)
		};

		var response = await ec2Client.DescribeSecurityGroupsAsync(request);
		response.SecurityGroups.Dump(metadata.DumpCaption);
	}

	public async Task SecurityGroup(DumpMetadata metadata)
	{
		await SecurityGroup(metadata.name, metadata);
	}

	public async Task SecurityGroup(string groupNameAsFilter, DumpMetadata metadata)
	{
		var request = new DescribeSecurityGroupsRequest
		{
			Filters = new SecurityGroupNameFilter(groupNameAsFilter)
		};

		var response = await ec2Client.DescribeSecurityGroupsAsync(request);
		response.SecurityGroups.First().Dump(metadata.DumpCaption);
	}

	class VpcIdFilter : List<Amazon.EC2.Model.Filter>
	{
		public VpcIdFilter(string id)
		{
			this.Add(new Amazon.EC2.Model.Filter { Name = "vpc-id", Values = new List<string> { id } });
		}
	}

	class SecurityGroupNameFilter : List<Amazon.EC2.Model.Filter>
	{
		public SecurityGroupNameFilter(string groupName)
		{
			this.Add(new Amazon.EC2.Model.Filter { Name = "group-name", Values = new List<string> { groupName } });
		}
	}

	public void Dispose()
	{
		ec2Client?.Dispose();
	}
}

class NameFilter : List<Amazon.EC2.Model.Filter>
{
	public NameFilter(string name)
	{
		this.Add(new Amazon.EC2.Model.Filter { Name = "tag:Name", Values = new List<string> { name } });
	}
}

#region Advanced - How to multi-target

// The NET5 symbol is active when the query runs on .NET 5 or later.

#if NET5
// Code that requires .NET 5 or later
#endif

#endregion
