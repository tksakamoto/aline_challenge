namespace TransactionService.MessageBrokers;

public interface IKafkaConsumer {

    void ConsumeMessages(string topic, string groupId, Action<string> action);

}