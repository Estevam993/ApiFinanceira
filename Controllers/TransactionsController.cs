using System.Security.Claims;
using ApiFinanceira.Contexts;
using ApiFinanceira.DTOs;
using ApiFinanceira.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiFinanceira.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public TransactionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int GetUserId()
    {
        if (!User.Identity.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("Usuário não autenticado");
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ??
                          User.FindFirst("userId") ??
                          User.FindFirst(ClaimTypes.Sid);

        if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
        {
            throw new UnauthorizedAccessException("User ID não encontrado no token");
        }

        if (int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }

        throw new UnauthorizedAccessException("User ID inválido no token");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetAllByUser()
    {
        var userId = GetUserId();

        return await _context.Transactions
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    [HttpGet("mensal")]
    public async Task<ActionResult<Transaction>> GetMensalTransactions()
    {
        var userId = GetUserId();

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
        var userId = GetUserId();

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