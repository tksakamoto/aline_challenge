using System.Text;
using System.Text.Json;
using AutoMapper;
using TransactionService.Data;
using TransactionService.Dtos;
using TransactionService.Enums;
using TransactionService.MessageBrokers;
using TransactionService.Models;

namespace TransactionService.Services;

public class TransactionFraudCheckService : IHostedService
{
    private readonly ILogger<TransactionFraudCheckService> _logger;
    private readonly IConfiguration _config;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapper _mapper;

    public TransactionFraudCheckService(ILogger<TransactionFraudCheckService> ilogger, IConfiguration config,
        IServiceProvider serviceProvider, IMapper mapper) {
        
        _logger = ilogger;
        _config = config;
        _serviceProvider = serviceProvider;
        _mapper = mapper;
    }
    

    public Task StartAsync(CancellationToken cancellationToken)
    {   
        Task tsk = new Task(() =>
        {
            using var scopeService = _serviceProvider.CreateScope();            
            var kafkaConsumer = scopeService.ServiceProvider.GetRequiredService<IKafkaConsumer>();
            var transactionDb = scopeService.ServiceProvider.GetRequiredService<ITransactionData>();            
            var topicFraudCheck = _config.GetSection("KafkaTopicFraudCheckTransaction").Value;
            var groupId = _config.GetSection("KafkaGroupIdFraudCheckTransaction").Value;
            var fraudCheckBase = _config.GetSection("FraudCheckBaseAddres").Value;
            var fraudCheckApi = _config.GetSection("FraudCheckApi").Value;

            kafkaConsumer.ConsumeMessages(topicFraudCheck!, groupId!, (message) =>
            {                
                var transaction = JsonSerializer.Deserialize<TransactionModel>(message);
                if (transaction != null)
                {
                    var client = new HttpClient() { BaseAddress = new Uri(fraudCheckBase!)};                        
                    var transactionFraudCheckDto = _mapper.Map<TransactionFraudCheckDto>(transaction);
                    var content = new StringContent(JsonSerializer.Serialize(transactionFraudCheckDto), Encoding.UTF8, "application/json");
                    var response = client.PostAsync(fraudCheckApi!, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        var checkedTransaction = JsonSerializer.Deserialize<TransactionModel>(
                            response.Content.ReadAsStringAsync().Result, 
                            new JsonSerializerOptions{ PropertyNameCaseInsensitive = true }
                        );
                        
                        transaction.TransactionStatusId = checkedTransaction!.TransactionStatusId;

                        transactionDb.UpdateTransaction(transaction);
                        _logger.LogInformation($"Transaction Id: {transaction.TransactionExternalId} - Has been checked with status Id: {transaction.TransactionStatusId}.");

                    } else {                                                
                        var errorMessage = $"Error Calling FraudApi: StatusCode {response.StatusCode}-\n Response: {response}";                        
                        throw new HttpRequestException(errorMessage);
                    }                        
                }                
            });

        }, cancellationToken);

        tsk.Start();

        _logger.LogInformation("Transaction Transaction Fraud Check Service is Running...");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        
        _logger.LogInformation("Stopping Transaction Fraud Check Service...");
        return Task.CompletedTask;
    }


}