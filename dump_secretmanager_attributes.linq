<Query Kind="Program" />

using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Amazon.Runtime;
using System.Threading.Tasks;

async void Main()
{
	using (var dumper = new SecretsManagerDump())
	{
		await dumper.Secret(new DumpMetadata
		{
			name = "rds-secrets",
			rogicalName = "for RDS authentication"
		});
	};
}
