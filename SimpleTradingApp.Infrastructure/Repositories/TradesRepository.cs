

using Microsoft.EntityFrameworkCore;
using SimpleTradingApp.Application.IRepositories;

namespace SimpleTradingApp.Infrastructure.Repositories;

public class TradesRepository : ITradesRepository
{
    private readonly AppDbContext _context;

    public TradesRepository(AppDbContext context)
    {
        _context = context;
    }

}
