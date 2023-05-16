using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TransactionService.Data;
using TransactionService.MessageBrokers;
using TransactionService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var pgConnectionBuilder = new NpgsqlConnectionStringBuilder();
pgConnectionBuilder.ConnectionString = builder.Configuration.GetConnectionString("PgDbConnection");
pgConnectionBuilder.TrustServerCertificate = true;



builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(pgConnectionBuilder.ConnectionString));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IKafkaConsumer, KafkaConsumer>();
builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();
builder.Services.AddScoped<ITransactionData, TransactionData>();
builder.Services.AddHostedService<TransactionCreateService>();
builder.Services.AddHostedService<TransactionFraudCheckService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
