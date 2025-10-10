using ApiFinanceira.Contexts;
using ApiFinanceira.DTOs;
using ApiFinanceira.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceira.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public TransactionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetAll()
    {
        return await _context.Transactions.ToListAsync();
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetAllByUser(int userId)
    {
        return await _context.Transactions
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    [HttpGet("mensal/{userId}")]
    public async Task<ActionResult<Transaction>> GetMensalTransactions(int userId)
    {
        DateTime pastThirtyDays = DateTime.Today.AddDays(-30);

        try
        {
            var userExist = await _context.Users.AnyAsync(u => u.Id == userId);

            if (!userExist)
            {
                throw new ArgumentException("User not found");
            }

            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .Where(t => t.DateCreated >= pastThirtyDays && t.DateCreated <= DateTime.Today)
                .ToListAsync();

            double total = 0;

            foreach (var transaction in transactions)
            {
                total += transaction.Value;
            }

            total = Math.Round(total, 3);

            return Ok(new { message = "Success on get last mouth transactions.", total, transactions });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to get mensal transactions.", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<Transaction>> Create(CreateTransactionDto transactionDto)
    {
        var userId = transactionDto.UserId;

        var userExist = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExist)
        {
            return NotFound(new { message = "User not found." });
        }

        var transaction = new Transaction
        {
            Title = transactionDto.Title,
            Value = transactionDto.Value,
            UserId = userId
        };

        try
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Transaction created!", transaction });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Failed to save transaction.", error = ex.Message });
        }
    }
}