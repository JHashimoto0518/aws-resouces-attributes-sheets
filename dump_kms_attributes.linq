<Query Kind="Program" />

using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using Amazon.Runtime;
using System.Threading.Tasks;

async void Main()
{
	using (var dumper = new KmsDump())
	{
		await dumper.Keys(new DumpMetadata
		{
			name = "CMK　list",
			rogicalName = "customer master key list"
		});
	}
}

