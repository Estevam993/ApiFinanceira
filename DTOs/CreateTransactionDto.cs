using System.ComponentModel.DataAnnotations;

namespace ApiFinanceira.DTOs;

public class CreateTransactionDto
{
    [Required] [StringLength(100)] public string Title { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
    public double Value { get; set; }
    
    [Required]
    [RegularExpression("^(IN|OUT)$", ErrorMessage = "O campo Type deve ser 'IN' ou 'OUT'.")]
    public string Type { get; set; }
    
    [Required]
    public int Amount { get; set; }

    public int UserId { get; set; }
}