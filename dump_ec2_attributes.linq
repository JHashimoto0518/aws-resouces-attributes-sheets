<Query Kind="Program">
  <NuGetReference>AWSSDK.Core</NuGetReference>
  <NuGetReference>AWSSDK.EC2</NuGetReference>
  <NuGetReference>AWSSDK.S3</NuGetReference>
</Query>

using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;
using System.Threading.Tasks;

async void Main() {
    using (var dumper = new Ec2Dump()) {
        await dumper.Ec2(new DumpMetadata {
            name = "bastion-ec2",
            rogicalName = "bastion instance"
        });
    };
}
