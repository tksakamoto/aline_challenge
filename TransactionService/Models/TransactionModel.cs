using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransactionService.Models;

[Table("Transaction")]
public class TransactionModel {

    [Key]
    public Guid? TransactionExternalId { get; set; }

    [Required]
    public Guid? AccountExternalIdDebit { get; set; }
    
    [Required]
    public Guid? AccountExternalIdCredit { get; set; }
    
    [Required]    
    public int? TransferTypeId { get; set; }

    [Required]
    public decimal? Value { get; set; }

    [Required]
    public int? TransactionStatusId { get; set; }

    [Required]
    public int? TransactionTypeId { get; set; }

    [Required]
    public DateTime? CreatedAt { get; set; }
    
    
    public TransactionStatusModel? TransactionStatus { get; set; }
    
    public TransactionTypeModel? TransactionType { get; set; }

}