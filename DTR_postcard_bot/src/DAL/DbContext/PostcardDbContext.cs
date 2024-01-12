using DTR_postcard_bot.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DTR_postcard_bot.DAL.DbContext;

public class PostcardDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public required DbSet<Card> Cards { get; set; }
    public required DbSet<Asset> Assets { get; set; }
    public required DbSet<AssetType> AssetTypes { get; set; }
    public required DbSet<Text> Texts { get; set; }
    public required DbSet<Stat> Stats { get; set; }
    
    public PostcardDbContext(DbContextOptions<PostcardDbContext> options) : base(options) {}
}