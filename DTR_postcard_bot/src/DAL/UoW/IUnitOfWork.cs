using DTR_postcard_bot.DAL.Repository.IRepositories;

namespace DTR_postcard_bot.DAL.UoW.IUoW;

public interface IUnitOfWork
{
    ICardRepository Cards { get; }
    IAssetRepository Assets { get; }
    IAssetTypeRepository AssetTypes { get; }
    IStatRepository Stats { get; }
    ITextRepository Texts { get; }

    Task CompleteAsync();
}