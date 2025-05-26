using OrderProcessing.Infrastructure;
using OrderProcessing.Infrastructure.MessageBroker;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Bind RabbitMQ configuration
builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection(RabbitMqConfiguration.SectionName));

// OR if you want to inject the configuration directly
builder.Services.AddSingleton(builder.Configuration.GetSection(RabbitMqConfiguration.SectionName).Get<RabbitMqConfiguration>());

var app = builder.Build();

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
