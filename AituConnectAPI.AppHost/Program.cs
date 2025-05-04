var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AituConnectAPI>("aituconnectapi");

builder.AddProject<Projects.MessageListenerService>("messagelistenerservice");

builder.Build().Run();
