using Microsoft.Playwright;

public class ResultsPage(IPage page)
{
    private readonly IPage _page = page;

    public ILocator ResultsList => _page.Locator("#b_results");

    public ILocator FirstResult => _page.Locator("#b_results li.b_algo h2 a").First;

    public async Task<string> GetFirstResultText()
    {
        return await FirstResult.TextContentAsync() ?? string.Empty;
    }

    public async Task ClickFirstResult()
    {
        await FirstResult.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task<int> GetResultsCount()
    {
        return await _page.Locator("#b_results li.b_algo").CountAsync();
    }
}
