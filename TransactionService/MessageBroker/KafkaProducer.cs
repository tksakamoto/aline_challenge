using Confluent.Kafka;

namespace TransactionService.MessageBrokers;

public class KafkaProducer : IKafkaProducer
{
    private readonly ILogger<KafkaProducer>? _logger;
    
    private ProducerConfig producerConfig;    

    public KafkaProducer(IConfiguration config, ILogger<KafkaProducer> logger)
    {
        _logger = logger;
        producerConfig = new ProducerConfig();        
        config.Bind("KafkaConfiguration", producerConfig);
    }


    public async Task ProduceMessageAsync(string topic, string value, CancellationToken cancellationToken) {
        
        try{        
            
            using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
            var message = new Message<Null, string>() { Value = value };
            await producer!.ProduceAsync(topic, message, cancellationToken);
            
        } catch (Exception ex) {
            _logger!.LogError(ex, "Error Producing Message...");
            throw ex;
        }        

    }
}