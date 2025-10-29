using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ApiFinanceira.Models;

namespace ApiFinanceira.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Adicione suas DbSets (tabelas) aqui
    public DbSet<User> Users { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<AlbumReview> AlbumReview { get; set; }
}