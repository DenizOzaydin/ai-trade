using MetuTrade.Business.Services;
using MetuTrade.Business.Settings;
using MetuTrade.DataAccess;
using MetuTrade.DataAccess.Market;
using MetuTrade.WebApi.Services;
using Microsoft.EntityFrameworkCore;

namespace MetuTrade.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddHttpClient();
        builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.Configure<BinanceSettings>(builder.Configuration.GetSection("BinanceSettings"));
        builder.Services.AddScoped<BinanceService>();
        builder.Services.AddScoped<BarService>();
        builder.Services.AddScoped<BarRepository>();
        builder.Services.AddSingleton<BinanceBackgroundService>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
