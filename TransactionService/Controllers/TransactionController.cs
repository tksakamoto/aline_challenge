using System.Text.Json;
using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using TransactionService.Data;
using TransactionService.Dtos;
using TransactionService.Enums;
using TransactionService.MessageBrokers;
using TransactionService.Models;

namespace TransactionService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ILogger<TransactionController> _logger;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly ITransactionData _transactionDb;
    private readonly IMapper _mapper;

    public TransactionController(IConfiguration config, ILogger<TransactionController> logger,
        IKafkaProducer kafkaProducer, ITransactionData transactionDb, IMapper mapper){

        _config = config;
        _logger = logger;
        _kafkaProducer = kafkaProducer;
        _transactionDb = transactionDb;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult> PostTransaction(CancellationToken cancellationToken, TransactionCreateDto transactionCreateDto){

        try
        {   
            var TransationType = _transactionDb.GetTransactionTypeById(transactionCreateDto.TransferTypeId);

            if (TransationType == null)
                return BadRequest("Invalid TransferTypeId.");            

            var topic = _config.GetSection("KafkaTopicCreateTransaction").Value;
            var transactionModel = _mapper.Map<TransactionModel>(transactionCreateDto);

            transactionModel.TransactionExternalId = Guid.NewGuid();
            transactionModel.CreatedAt = DateTime.Now;
            transactionModel.TransactionStatus = new TransactionStatusModel(TransactionStatusEnum.pending);
            transactionModel.TransactionType = new TransactionTypeModel((TransactionTypeEnum)transactionCreateDto.TransferTypeId);

            await _kafkaProducer.ProduceMessageAsync(topic!, JsonSerializer.Serialize(transactionModel), cancellationToken);


            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Request has been cancelled..");
                throw new OperationCanceledException();
            }
            return Created("api/[controller]", _mapper.Map<TransactionReadDto>(transactionModel));

        } catch (Exception ex)
        {
            _logger.LogError(ex, "Error PostTransaction endpoint...");
            return StatusCode(StatusCodes.Status500InternalServerError); 
        } 
    }



    [HttpGet("{id}")]
    public ActionResult<TransactionModel> GetTransactionById(Guid id){
        
        try {
            
            var transaction = _transactionDb.GetTransactionByExternalId(id);
            
            if(transaction == null)
                return NotFound();

            var transactionReadDto = _mapper.Map<TransactionReadDto>(transaction);
            return Ok(transactionReadDto);

        } catch (Exception ex)
        {
            _logger.LogError(ex, "Error PostTransaction endpoint...");
            return StatusCode(StatusCodes.Status500InternalServerError); 
        }
    }

   
}