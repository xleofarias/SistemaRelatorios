using MassTransit;
using Microsoft.EntityFrameworkCore;
using RelatoriosWorker.Consumer;
using RelatoriosWorker.Data;

var builder = Host.CreateApplicationBuilder(args);

//Add Context
var connectionString = Environment.GetEnvironmentVariable("RelatoriosAPI") ?? builder.Configuration.GetConnectionString("RelatoriosAPI");
builder.Services.AddDbContext<WorkerDbContext>(options =>
{
    options.UseNpgsql(connectionString,
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null
        ));
});

//Add MassTransit (RABBIT)
var connectionRabbit = Environment.GetEnvironmentVariable("ConnectionRabbit") ?? builder.Configuration.GetConnectionString("ConnectionRabbit");
builder.Services.AddMassTransit(x =>
{
    //Registrando o Consumer
    x.AddConsumer<RelatorioSolicitadoConsumer>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        // URL do CloudAMQP
        cfg.Host(connectionRabbit);

        // Ela cria as filas automaticamente no RabbitMQ baseando-se no nome do Consumidor.
        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();
host.Run();
