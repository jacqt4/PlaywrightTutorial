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
}
