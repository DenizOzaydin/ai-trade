using MetuTrade.Business.Services;
using MetuTrade.Business.Settings;
using MetuTrade.DataAccess;
using MetuTrade.WebApi.Hubs;
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
        builder.Services.AddBinance();
        builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.Configure<BinanceSettings>(builder.Configuration.GetSection("BinanceSettings"));
        builder.Services.AddScoped<BinanceHttpService>();
        builder.Services.AddScoped<BarService>();
        builder.Services.AddScoped<BarRepository>();
        builder.Services.AddScoped<BotRepository>();
        builder.Services.AddSingleton<BinanceBackgroundDownloadService>();
        builder.Services.AddSingleton<BinanceBackgroundSocketService>();

        builder.Services.AddSignalR();
        builder.Services.AddControllers();

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

        app.UseStaticFiles();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<AdminHub>("/hubs/admin");

        app.Run();
    }
}
