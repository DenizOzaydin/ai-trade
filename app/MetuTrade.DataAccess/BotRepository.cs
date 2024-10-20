using MetuTrade.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetuTrade.DataAccess
{
    public class BotRepository
    {
        private readonly ApplicationContext _context;

        public BotRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<int> Count()
        {
            return await _context.Set<Bar>().CountAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Bot? bot = await GetByIdAsync(id);
            if (bot != null)
            {
                _context.Set<Bot>().Remove(bot);
            }
        }

        public async Task<List<Bot>> GetAllAsync()
        {
            return await _context.Set<Bot>().ToListAsync();
        }

        public async Task<Bot?> GetByIdAsync(int id)
        {
            return await _context.Set<Bot>().AsNoTracking().Where(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Bot bot)
        {
            Bot? exist = await GetByIdAsync(bot.Id);
            if (exist == null)
            {
                await _context.AddAsync(bot);
            }
            else
            {
                _context.Entry(exist).State = EntityState.Detached;
                _context.Update(bot);
            }
        }
    }
}
