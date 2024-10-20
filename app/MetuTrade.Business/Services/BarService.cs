using MetuTrade.Core.Entities;
using MetuTrade.DataAccess;

namespace MetuTrade.Business.Services;

public class BarService
{
    private readonly BarRepository _barRepository;

    public BarService(BarRepository barRepository)
    {
        _barRepository = barRepository;
    }

    public async Task UploadAsync(List<Bar> bars)
    {
        foreach (var bar in bars)
        {
            await _barRepository.UpdateAsync(bar);
        }
        await _barRepository.SaveChangesAsync();
    }
}