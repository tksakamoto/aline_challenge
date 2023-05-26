namespace TransactionService.Dtos;


public class TransactionReadDto {

    public Guid? TransactionExternalId { get; set; }
    public TransactionTypeReadDto? TransactionType { get; set; }
    public TransactionStatusReadDto? TransactionStatus { get; set; }
    public decimal? Value { get; set; }
    public DateTime? CreatedAt { get; set; }

}