namespace TransactionService.MessageBrokers;

public interface IKafkaProducer
{
    Task ProduceMessageAsync(string topic, string value, CancellationToken cancellationToken);

}