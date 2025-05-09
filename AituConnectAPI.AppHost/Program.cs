var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MessageListenerService>("messagelistenerservice");

builder.AddProject<Projects.MessageProducerService>("messageproducerservice");

builder.Build().Run();
