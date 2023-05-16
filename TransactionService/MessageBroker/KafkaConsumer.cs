using Confluent.Kafka;

namespace TransactionService.MessageBrokers;

public class KafkaConsumer : IKafkaConsumer
{
    private readonly ILogger<KafkaConsumer>? _logger;    
    private ConsumerConfig consumerConfig;

    public KafkaConsumer(IConfiguration config, ILogger<KafkaConsumer> logger)
    {
        _logger = logger;
        consumerConfig = new ConsumerConfig();
        config.Bind("KafkaConfiguration", consumerConfig);
    }

    public void ConsumeMessages(string topic, string groupId, Action<string> action)
    {
        consumerConfig.GroupId = groupId;
        consumerConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
        consumerConfig.EnableAutoOffsetStore = false;

        using (var consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build())
        {
            while(true)
            {
                if(consumer.Subscription.Count == 0)
                    consumer.Subscribe(topic);
                    
                try
                {
                    var response = consumer.Consume();
                    action(response.Message.Value);
                    consumer.StoreOffset(response);
                
                } catch (Exception ex) 
                {
                    consumer.Unsubscribe();
                    _logger!.LogError(ex, $"Error consumming message");                    
                }
            }
        }
    }

}