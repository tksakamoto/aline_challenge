using System.Text.Json;
using TransactionService.Data;
using TransactionService.Enums;
using TransactionService.MessageBrokers;
using TransactionService.Models;

namespace TransactionService.Services;

public class TransactionCreateService : IHostedService
{
    private readonly ILogger<TransactionCreateService> _logger;
    private readonly IConfiguration _config;
    private readonly IServiceProvider _serviceProvider;
    

    public TransactionCreateService(ILogger<TransactionCreateService> ilogger, IConfiguration config,
        IServiceProvider serviceProvider){

        _logger = ilogger;
        _config = config;
        _serviceProvider = serviceProvider;
    }

    
    public Task StartAsync(CancellationToken cancellationToken) {

        Task tsk = new Task(() => {

            using var scopeService = _serviceProvider.CreateScope();            
            var kafkaConsumer = scopeService.ServiceProvider.GetRequiredService<IKafkaConsumer>();
            var kafkaProducer = scopeService.ServiceProvider.GetRequiredService<IKafkaProducer>();
            var transactionDb = scopeService.ServiceProvider.GetRequiredService<ITransactionData>();
            
            var topicCreate = _config.GetSection("KafkaTopicCreateTransaction").Value;
            var topicFraudCheck = _config.GetSection("KafkaTopicFraudCheckTransaction").Value;
            var groupId = _config.GetSection("KafkaGroupIdCreateTransaction").Value;
            

            kafkaConsumer.ConsumeMessages(topicCreate!, groupId!, async (message) => {

                var transaction = JsonSerializer.Deserialize<TransactionModel>(message);
                if (transaction != null) {

                    transaction.TransactionTypeId = transaction.TransferTypeId;
                    transaction.TransactionStatusId = (int)TransactionStatusEnum.pending;
                    transaction.TransactionStatus = null;
                    transaction.TransactionType = null;

                    transactionDb.CreateTransaction(transaction!);
                    _logger.LogInformation($"Transaction Id: {transaction.TransactionExternalId} - Created.");
                    
                    await kafkaProducer.ProduceMessageAsync(topicFraudCheck!, JsonSerializer.Serialize(transaction), cancellationToken);                    
                }                
            });

        }, cancellationToken);

        tsk.Start();

        _logger.LogInformation("Transaction Create Service is Running...");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Transaction Create Service...");
        return Task.CompletedTask;
    }


}