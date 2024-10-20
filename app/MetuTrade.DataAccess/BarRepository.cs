namespace MetuTrade.DataAccess;

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MetuTrade.Core.Entities;

public class BarRepository
{
    private readonly ApplicationContext _context;

    public BarRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<int> Count()
    {
        return await _context.Set<Bar>().CountAsync();
    }

    public async Task DeleteAsync(string symbol, string interval, long openTime)
    {
        Bar? bar = await GetByIdAsync(symbol, interval, openTime);
        if (bar != null)
        {
            _context.Set<Bar>().Remove(bar);
        }
    }

    public async Task<List<Bar>> GetAllAsync()
    {
        return await _context.Set<Bar>().ToListAsync();
    }

    public async Task<List<Bar>> GetByFilterAsync(Expression<Func<Bar, bool>> filter, int? limit)
    {
        if (limit == null) return await _context.Set<Bar>().Where(filter).OrderBy(e => e.OpenTime).ToListAsync();
        List<Bar> bars = await _context.Set<Bar>().Where(filter).OrderByDescending(e => e.OpenTime).Take((int)limit).ToListAsync();
        bars.Reverse();
        return bars;
    }

    public async Task<Bar?> GetByIdAsync(string symbol, string interval, long openTime)
    {
        return await _context.Set<Bar>().AsNoTracking().Where(e => e.Symbol == symbol && e.Interval == interval && e.OpenTime == openTime).FirstOrDefaultAsync();
    }

    public async Task<List<Bar>> GetBySymbolAndIntervalAsync(string symbol, string interval)
    {
        return await _context.Set<Bar>().AsNoTracking().Where(e => e.Symbol == symbol && e.Interval == interval).ToListAsync();
    }

    public async Task<List<Bar>> GetBySymbolAsync(string symbol)
    {
        return await _context.Set<Bar>().AsNoTracking().Where(e => e.Symbol == symbol).ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Bar bar)
    {
        Bar? exist = await GetByIdAsync(bar.Symbol, bar.Interval, bar.OpenTime);
        if (exist == null)
        {
            await _context.AddAsync(bar);
        }
        else
        {
            _context.Entry(exist).State = EntityState.Detached;
            _context.Update(bar);
        }
    }
}
