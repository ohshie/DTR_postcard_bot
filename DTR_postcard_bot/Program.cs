﻿using DTR_postcard_bot.AssetManager;
using DTR_postcard_bot.BotClient;
using DTR_postcard_bot.BotClient.Keyboards;
using DTR_postcard_bot.BotClient.Keyboards.Buttons;
using DTR_postcard_bot.BusinessLogic.CardCreator;
using DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler;
using DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler.MediaBatchHandler;
using DTR_postcard_bot.BusinessLogic.CardCreator.MediaHandler.Services;
using DTR_postcard_bot.BusinessLogic.TextContent;
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
            
            var loader = host.Services.GetRequiredService<Loader>();
            await loader.Execute();
            
            var botClient = host.Services.GetRequiredService<BotClient>();
            await botClient.BotOperations();

            await host.WaitForShutdownAsync();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
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
        collection.AddSingleton<ITelegramBotClient>(_ =>
        {
            var token = context.Configuration.GetSection("BotToken").GetValue<string>("BotToken");
            return new TelegramBotClient(token!);
        });
        
        // Asset Manager
        collection.AddSingleton<Loader>();
        collection.AddSingleton<AssetCleaner>();
        collection.AddSingleton<AssetTypeLoader>();
        collection.AddSingleton<AssetLoader>();
        collection.AddSingleton<TextLoader>();

        // Data Layer
        collection.AddDbContext<PostcardDbContext>(s =>
        {
            s.UseSqlite(context.Configuration.GetConnectionString("DefaultConnection"));
        });
        
        collection.AddTransient<CardOperator>();
        collection.AddTransient<AssetOperator>();
        collection.AddTransient<AssetTypeOperator>();
        collection.AddTransient<TextOperator>();
        collection.AddTransient<IRepository<Card>, CardRepository>();
        collection.AddTransient<IRepository<Asset>, AssetRepository>();
        collection.AddTransient<IRepository<AssetType>, AssetTypeRepository>();
        collection.AddTransient<IRepository<Text>, TextRepository>();
        
        // Bot client
        collection.AddTransient<BotClient>();
        collection.AddTransient<BotMessenger>();

        collection.AddTransient<ButtonCreator>();
        collection.AddTransient<CardCreationKeyboard>();
        collection.AddTransient<AssetChoiceKeyboard>();
        
        collection.AddTransient<BotGreetMessage>();
        collection.AddTransient<CallbackFactory>();
        
        // adding stuff to card
        collection.AddTransient<AddElementToCard>();

        collection.AddTransient<IMediaBatchHandler, MediaBatchFromStream>();
        collection.AddTransient<TextAssetHandler>();
        collection.AddTransient<AssembleMediaIntoCard>();

        // Card creation
        collection.AddTransient<StartCardCreation>();
        collection.AddTransient<CancelCardCreation>();
        collection.AddTransient<RequestMedia>();
        
        // Image Processor
        
        // Texts
        collection.AddTransient<ITextContent, MzkTextContent>();
    }
}

