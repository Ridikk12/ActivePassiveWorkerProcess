using ActivePasive;
using ActivePassive.Data;
using ActivePassive.Data.Interfaces;
using ActivePassive.Services;
using ActivePassive.Services.Interfaces;
using Azure.Data.Tables;
using Microsoft.Extensions.Azure;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<RegisterInstanceWorkerProcess>();
        services.AddHostedService<Worker>();
    }).ConfigureServices((hostContext,serviceCollection) =>
    {
        serviceCollection.AddAzureClients(x =>
        {
            var connectionString = hostContext.Configuration["ConnectionStrings:StorageAccount"];
            var tableName = hostContext.Configuration["InstanceRegistrationTableName"];

            x.AddClient<TableClient, TableClientOptions>((options, credential, provider) =>
                new TableClient(connectionString, tableName));
        });
        serviceCollection.AddScoped<IRegisterInstanceRepository, RegisterInstanceRepository>();
        serviceCollection.AddScoped<IRegisterInstanceService, RegisterInstanceService>();
    })
    .Build();


await host.RunAsync();
