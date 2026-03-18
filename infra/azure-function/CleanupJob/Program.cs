using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults() // <-- required for Azure Functions
    .ConfigureServices(services =>
    {
        services.AddHttpClient(); // registers HttpClient for DI
    })
    .Build();

host.Run();