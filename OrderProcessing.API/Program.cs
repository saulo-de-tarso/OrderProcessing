using Microsoft.Extensions.Options;
using OrderProcessing.API.Middlewares;
using OrderProcessing.Application;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Services;
using OrderProcessing.Infrastructure;
using OrderProcessing.Infrastructure.MessageBroker;
using OrderProcessing.Infrastructure.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration)
        );

builder.Services.AddApplication();

builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection(RabbitMqConfiguration.SectionName));

builder.Services.AddSingleton<IMessageBroker>(provider =>
    {
        var config = provider.GetRequiredService<IOptions<RabbitMqConfiguration>>();
        var logger = provider.GetRequiredService<ILogger<RabbitMqMessageBroker>>();

        var broker = new RabbitMqMessageBroker(config, logger);

        broker.InitializeAsync().GetAwaiter().GetResult();
        
        return broker;

    });

builder.Services.AddHostedService<OrderProcessingService>();
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();



builder.Services.AddTransient<ErrorHandlingMiddleware>();


var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseSerilogRequestLogging();

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
