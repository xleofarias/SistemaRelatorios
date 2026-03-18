using Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RelatoriosAPI.Data;
using RelatoriosAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add Context
var connection = Environment.GetEnvironmentVariable("RelatoriosAPI") ?? builder.Configuration.GetConnectionString("RelatoriosAPI");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connection,
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null
        )
    )   
);

//Add MassTransit (RABBIT)
var connectionRabbit = Environment.GetEnvironmentVariable("ConnectionRabbit") ?? builder.Configuration.GetConnectionString("ConnectionRabbit");
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(connectionRabbit);
    });
});

var app = builder.Build();

app.MapPost("/api/relatorios", async (AppDbContext db, IPublishEndpoint publish) =>
{
    var novoRelatorio = new Relatorio
    {
        Id = Guid.NewGuid(),
        Tipo = "Vendas",
        Status = "Pendente",
        CreatedAt = DateTime.UtcNow
    };

    db.Add(novoRelatorio);
    await db.SaveChangesAsync();

    var evento = new RelatoriosCreatedEvent(
        novoRelatorio.Id,
        novoRelatorio.Tipo,
        "leofarias.bliz@gmail.com",
        novoRelatorio.CreatedAt);

    await publish.Publish(evento);

    return Results.Accepted($"/api/relatorios/{novoRelatorio.Id}/Status", new
    {
        Mensagem = "Seu relatório entrou na fila de processamento!",
        Protocolo = novoRelatorio.Id,
        Status = novoRelatorio.Status
    });
});

app.MapGet("api/relatorios/{id:guid}/Status", async (Guid id, AppDbContext db) =>
{
    var relatorio = await db.Relatorios.FindAsync(id);

    if(relatorio == null)
    {
        return Results.NotFound($"Relatório com o id: {id} não encontrado");
    }

    return Results.Ok(new {Status = relatorio.Status});
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
