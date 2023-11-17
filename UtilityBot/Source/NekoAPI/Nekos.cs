namespace UtilityBot.Source.NekoAPI;

public class NekosApi
{
    private readonly NekoClient _client = new NekoClient();
    public async Task<NekoResult<NekoAction>> GetHug()
        => await _client.GetGenericEndpoint<NekoResult<NekoAction>>("hug");
    
    public async Task<NekoResult<NekoAction>> GetCuddle()
        => await _client.GetGenericEndpoint<NekoResult<NekoAction>>("cuddle");
    
    public async Task<NekoResult<NekoAction>> GetKiss()
        => await _client.GetGenericEndpoint<NekoResult<NekoAction>>("kiss");
    
    public async Task<NekoResult<NekoAction>> GetPat()
        => await _client.GetGenericEndpoint<NekoResult<NekoAction>>("pat");
    
    public async Task<NekoResult<NekoImage>> GetNeko(long count)
        => await _client.GetGenericEndpoint<NekoResult<NekoImage>>($"neko?amount={count}");
}