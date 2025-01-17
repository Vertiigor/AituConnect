var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AituConnectAPI>("aituconnectapi");

builder.Build().Run();
