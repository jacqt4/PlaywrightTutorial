using Microsoft.Playwright;
using NUnit.Framework;

/// <summary>
/// Example tests using the PlaywrightTestBase for cleaner test code
/// </summary>
public partial class CleanTests : PlaywrightTestBase
{
    [Test]
    public async Task Example_NavigateToPlaywright()
    {
        // No need to create playwright, browser, or page - it's done in Setup
        await Page!.GotoAsync("https://playwright.dev");

        await Assertions.Expect(Page).ToHaveTitleAsync(MyRegex());

        // Take a screenshot using the helper method
        await TakeScreenshot("homepage");
    }

    [Test]
    public async Task Example_SearchFunctionality()
    {
        await Page!.GotoAsync("https://playwright.dev");

        // Use Playwright's search functionality
        await Page.GetByRole(AriaRole.Button, new() { Name = "Search" }).ClickAsync();
        await Page.GetByPlaceholder("Search docs").FillAsync("test");

        // Verify search was performed
        await Assertions.Expect(Page.GetByPlaceholder("Search docs")).ToHaveValueAsync("test");
    }

    [Test]
    public async Task Example_MultipleAssertions()
    {
        await Page!.GotoAsync("https://playwright.dev");

        // Multiple assertions using Playwright's assertion library
        await Assertions.Expect(Page).ToHaveURLAsync(MyRegex1());
        await Assertions.Expect(Page.Locator("text=Get started").First).ToBeVisibleAsync();

        var title = await Page.TitleAsync();
        Assert.That(title, Does.Contain("Playwright"));
    }

    [System.Text.RegularExpressions.GeneratedRegex("Playwright")]
    private static partial System.Text.RegularExpressions.Regex MyRegex();
    [System.Text.RegularExpressions.GeneratedRegex(".*playwright.*")]
    private static partial System.Text.RegularExpressions.Regex MyRegex1();
}
