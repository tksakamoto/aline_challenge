using System.ComponentModel.DataAnnotations;
using TransactionService.Enums;

namespace TransactionService.Dtos;

public class TransactionCreateDto {

    [Required]
    public Guid AccountExternalIdDebit { get; set; }    
    [Required]
    public Guid AccountExternalIdCredit { get; set; }

    [Required]
    //[EnumDataType(typeof(TransactionTypeEnum))]    
    public int TransferTypeId { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Value { get; set; }

}