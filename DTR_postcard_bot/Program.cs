﻿using DTR_postcard_bot.AssetManager;
using DTR_postcard_bot.BotClient;
using DTR_postcard_bot.BotClient.Keyboards;
using DTR_postcard_bot.BotClient.Keyboards.Buttons;
using DTR_postcard_bot.BusinessLogic.CardCreator;
using DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler;
using DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler.Services;
using DTR_postcard_bot.DAL;
using DTR_postcard_bot.DAL.DbContext;
using DTR_postcard_bot.DAL.Repository.IRepositories;
using DTR_postcard_bot.DAL.UoW;
using DTR_postcard_bot.DAL.UoW.IUoW;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DTR_postcard_bot;

static class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using (host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            
            var dbContext = services.GetRequiredService<PostcardDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
            
            var loader = services.GetRequiredService<Loader>();
            var initializationSuccess = await loader.Execute();

            if (!initializationSuccess) return;
            
            var botClient = services.GetRequiredService<BotClient.BotClient>();
            await botClient.BotOperations();

            await host.WaitForShutdownAsync();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, builder) =>
            {
                builder.SetBasePath("/app/app/")
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .UseSerilog((_, configuration) =>
            {
                configuration
                    .MinimumLevel.Warning()
                    .Enrich.FromLogContext()
                    .WriteTo.Console();
            })
            .ConfigureServices(ConfigureServices);

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection collection)
    {
        collection.AddScoped<ITelegramBotClient>(_ =>
        {
            var token = context.Configuration.GetSection("BotToken").GetValue<string>("BotToken");
            return new TelegramBotClient(token!);
        });
        
        // Data Layer
        collection.AddDbContext<PostcardDbContext>(s =>
        {
            s.UseSqlite(context.Configuration.GetConnectionString("DefaultConnection"));
        });
        
        collection.AddScoped<IUnitOfWork, UnitOfWork>();
        
        collection.AddScoped<ICardRepository, CardRepository>();
        collection.AddScoped<IAssetRepository, AssetRepository>();
        collection.AddScoped<IAssetTypeRepository, AssetTypeRepository>();
        collection.AddScoped<ITextRepository, TextRepository>();
        collection.AddScoped<IStatRepository, StatRepository>();
        
        // Asset Manager
        collection.AddTransient<Loader>();
        collection.AddTransient<AssetCleaner>();
        collection.AddTransient<AssetTypeLoader>();
        collection.AddTransient<AssetLoader>();
        collection.AddTransient<TextLoader>();
        
        // Bot client
        collection.AddTransient<BotClient.BotClient>();
        collection.AddTransient<BotMessenger>();

        collection.AddTransient<ButtonCreator>();
        collection.AddTransient<CardCreationKeyboard>();
        collection.AddTransient<AssetChoiceKeyboard>();
        
        collection.AddTransient<BotGreetMessage>();
        collection.AddTransient<CallbackFactory>();
        
        // adding stuff to card
        collection.AddTransient<AddElementToCard>();

        if (context.Configuration.GetValue<bool>("AssetsFromUrl"))
        {
            collection.AddTransient<IMediaBatchHandler, MediaBatchFromUri>();
        }
        else
        {
            collection.AddTransient<IMediaBatchHandler, MediaBatchFromStream>();
        }
        
        collection.AddTransient<TextAssetHandler>();
        collection.AddTransient<AssembleMediaIntoCard>();

        // Card creation
        collection.AddTransient<StartCardCreation>();
        collection.AddTransient<CancelCardCreation>();
        collection.AddTransient<RequestMedia>();
        collection.AddTransient<CompleteAndSendCard>();
  
        // Texts
        collection.AddTransient<TextContent>();
    }
}