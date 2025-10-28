using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiFinanceira.Models;

public class Transaction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] [MaxLength(100)] public string Title { get; set; }

    public double Value { get; set; }
    

    public DateTime DateCreated { get; set; } = DateTime.Now;
    
    public string Type { get; set; }
    
    public int Amount { get; set; }

    public int UserId { get; set; }
    
    public User User { get; set; }
}