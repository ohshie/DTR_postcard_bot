using DTR_postcard_bot.BotClient;
using DTR_postcard_bot.BotClient.Keyboards;
using DTR_postcard_bot.BusinessLogic.CardCreator;
using DTR_postcard_bot.BusinessLogic.ImageProcessor;
using DTR_postcard_bot.BusinessLogic.TextContent;
using DTR_postcard_bot.ChannelBase;
using DTR_postcard_bot.DataLayer;
using DTR_postcard_bot.DataLayer.DbContext;
using DTR_postcard_bot.DataLayer.Models;
using DTR_postcard_bot.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using (host)
        {
            var dbContext = host.Services.GetRequiredService<PostcardDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
            
            var botClient = host.Services.GetRequiredService<BotClient>();
            await botClient.BotOperations();

            await host.WaitForShutdownAsync();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog((context, configuration) =>
            {
                configuration
                    .MinimumLevel.Warning()
                    .Enrich.FromLogContext()
                    .WriteTo.Console();
            })
            .ConfigureServices(ConfigureServices);

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection collection)
    {
        collection.AddSingleton<ITelegramBotClient>(_ =>
        {
            var token = context.Configuration.GetSection("BotToken").GetValue<string>("BotToken");
            return new TelegramBotClient(token!);
        });

        // Data Layer
        collection.AddDbContext<PostcardDbContext>(s =>
        {
            s.UseSqlite(context.Configuration.GetConnectionString("DefaultConnection")!);
        });
        
        collection.AddTransient<CardOperator>();
        collection.AddTransient<IRepository<Card>, CardRepository>();
        collection.AddTransient<IRepository<Asset>, AssetRepository>();
        
        // Bot client
        collection.AddTransient<BotClient>();
        collection.AddTransient<BotMessenger>();

        collection.AddTransient<GreetingsKeyboard>();
        collection.AddTransient<BgKeyboard>();
        
        collection.AddTransient<BotGreetMessage>();
        collection.AddTransient<CallbackFactory>();

        // Card creation
        collection.AddTransient<StartCardCreation>();
        collection.AddTransient<CancelCardCreation>();
        
        // Image Processor
        collection.AddTransient<FileCleanUp>();
        
        // Texts
        collection.AddTransient<ITextContent, MzkTextContent>();
    }
}

