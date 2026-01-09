using Microsoft.Playwright;
using NUnit.Framework;

public class DemoTest
{
    [Test]
    public async Task OpenGoogle()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
        var page = await browser.NewPageAsync();
        await page.GotoAsync("https://www.google.com");
        Assert.That(await page.TitleAsync(), Does.Contain("Google"));
    }

    [Test]
    public async Task SearchGoogle()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
        var page = await browser.NewPageAsync();
        await page.GotoAsync("https://www.google.com");

        // Search for a term
        await page.Locator("textarea[name='q']").FillAsync("Playwright testing");
        await page.Keyboard.PressAsync("Enter");

        // Wait for and assert search results appear
        await page.Locator("#search").WaitForAsync();
        Assert.That(await page.Locator("#search").IsVisibleAsync(), Is.True);
    }

    [Test]
    public async Task SearchBing()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();

        await page.GotoAsync("https://www.bing.com");
        await page.Locator("input[name='q']").FillAsync("Playwright C#");
        await page.Keyboard.PressAsync("Enter");

        // Wait for URL to change (indicating search happened)
        // This is more reliable because Bing's page structure uses 
        // hidden elements that Playwright's visibility checks don't handle well.
        await page.WaitForURLAsync("**/search?**");

        // Check that we have search results
        var hasResults = await page.Locator("#b_results li").CountAsync() > 0;
        Assert.That(hasResults, Is.True, "Search results should be present");
    }

    [Test]
    public async Task SearchBingAndClickFirstResult()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
        var page = await browser.NewPageAsync();

        await page.GotoAsync("https://www.bing.com");
        await page.Locator("input[name='q']").FillAsync("Playwright C#");
        await page.Keyboard.PressAsync("Enter");

        // Wait for search results
        await page.WaitForURLAsync("**/search?**");

        // Click the first result
        var firstResult = page.Locator("#b_results li.b_algo h2 a").First;
        await firstResult.ClickAsync();

        // Wait for navigation to complete
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // Assert the new page title is not empty and doesn't contain "Bing"
        var newTitle = await page.TitleAsync();
        Assert.That(newTitle, Is.Not.Empty, "New page should have a title");
        Assert.That(newTitle.ToLower(), Does.Not.Contain("bing"), "Should have navigated away from Bing");
    }

    [Test]
    public async Task SearchWithPageObject()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();

        var home = new HomePage(page);
        await page.GotoAsync("https://www.bing.com");
        await home.Search("Playwright C#");
    }

    [Test]
    public async Task SearchWithPageObjectAndAssertFirstResult()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
        var page = await browser.NewPageAsync();

        await page.GotoAsync("https://www.bing.com");

        var homePage = new HomePage(page);
        var resultsPage = await homePage.SearchAndWaitForResults("Playwright C#");

        // Assert first result text exists
        var firstResultText = await resultsPage.GetFirstResultText();
        Assert.That(firstResultText, Is.Not.Empty, "First result should have text");
        Assert.That(firstResultText.Length, Is.GreaterThan(5), "First result text should be meaningful");

        // Assert we have multiple results
        var resultsCount = await resultsPage.GetResultsCount();
        Assert.That(resultsCount, Is.GreaterThan(0), "Should have search results");
    }
}
