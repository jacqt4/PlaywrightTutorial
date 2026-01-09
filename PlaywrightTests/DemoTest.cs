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
}
