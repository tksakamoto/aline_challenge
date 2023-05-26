using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TransactionService.Enums;

namespace TransactionService.Models;

[Table("TransactionType")]
public class TransactionTypeModel {

    [Key]
    public int TransactionTypeId { get; set; }

    [Required]
    [MaxLength(20)]
    public string? Name { get; set; }

    public TransactionTypeModel(){
    }

    public TransactionTypeModel(TransactionTypeEnum typeEnum){  
        this.TransactionTypeId = (int)typeEnum;
        this.Name = typeEnum.ToString();        
    }
    
}