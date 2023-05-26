using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TransactionService.Enums;

namespace TransactionService.Models;

[Table("TransactionStatus")]
public class TransactionStatusModel {

    [Key]
    public int TransactionStatusId { get; set; }

    [Required]
    [MaxLength(20)]
    public string? Name { get; set; }

    public TransactionStatusModel() {         
    }

    public TransactionStatusModel(TransactionStatusEnum statusEnum) {
        this.TransactionStatusId = (int)statusEnum;
        this.Name = statusEnum.ToString();
    }
    
}