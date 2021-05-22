<Query Kind="Program" />

using Amazon.S3;
using Amazon.Runtime;
using System.Threading.Tasks;

void Main() {
    DumpMultiS3();
}

async void DumpMultiS3() {
    await new S3Dump().Buckets(new DumpMetadata {
        name = "S3 Buckets",
        rogicalName = "all bucket list"
    });
}