using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiFinanceira.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] [MaxLength(100)] public string Name { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; }

    public DateTime DateCreated { get; set; } = DateTime.Now;

    public bool Active { get; set; } = true;
    
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}