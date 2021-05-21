<Query Kind="Program" />

void Main()
{
	var options = new Amazon.Runtime.CredentialManagement.CredentialProfileOptions
	{
		AccessKey = "AKI...", // Access Key
		SecretKey = "FVy...", // Don't commit with Secret Key
	};
	var profile = new Amazon.Runtime.CredentialManagement.CredentialProfile("default", options);
	profile.Region = Amazon.RegionEndpoint.APNortheast1; // Tokyo Region
	var netSDKFile = new Amazon.Runtime.CredentialManagement.NetSDKCredentialsFile();
	netSDKFile.RegisterProfile(profile);
}