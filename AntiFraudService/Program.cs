using AntiFraudService.Models;
using TransactionService.Enums;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();


app.MapPost("/api/fraudcheck", (TransactionModel transaction) =>
{
    
    transaction.TransactionStatusId = transaction.Value <= 1000 ? (int)TransactionStatusEnum.approved : (int)TransactionStatusEnum.rejected;
    
    return Results.Ok(transaction);
    
});

app.Run();
