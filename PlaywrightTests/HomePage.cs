using Microsoft.Playwright;

public class HomePage(IPage page)
{
    private readonly IPage _page = page;

    public ILocator SearchBox => _page.Locator("input[name='q']");

    public async Task Search(string text)
    {
        await SearchBox.FillAsync(text);
        await _page.Keyboard.PressAsync("Enter");
    }

    public async Task<ResultsPage> SearchAndWaitForResults(string text)
    {
        await Search(text);
        await _page.WaitForURLAsync("**/search?**");
        return new ResultsPage(_page);
    }
}