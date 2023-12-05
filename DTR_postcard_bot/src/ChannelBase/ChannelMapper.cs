namespace DTR_postcard_bot.ChannelBase;

public class ChannelMapper(ILogger<ChannelMapper> logger)
{
    public long ChannelId;
    
    private readonly Dictionary<string, long> _channelDictionary = new()
    {
        { "mzk", 1 },
    };

    public void SetChannel(string channelTag)
    {
        if (!_channelDictionary.TryGetValue(channelTag, out var mappedTag))
        {
            logger.LogCritical("Channel tag was not set!");
            return;
        }

        ChannelId = mappedTag;
    }
}