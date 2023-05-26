namespace TransactionService.Dtos;

public class TransactionFraudCheckDto {

    public Guid? TransactionExternalId { get; set; }

    public Guid? AccountExternalIdDebit { get; set; }

    public Guid? AccountExternalIdCredit { get; set; }

    public int? TransferTypeId { get; set; }

    public decimal? Value { get; set; }

    public int? TransactionStatusId { get; set; }

    public int? TransactionTypeId { get; set; }

}