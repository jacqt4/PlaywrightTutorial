using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;

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

    [Test]
    public async Task LoginWithPlaywrightAssertions()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
        var page = await browser.NewPageAsync();

        // Navigate to login demo page
        await page.GotoAsync("https://the-internet.herokuapp.com/login");

        // Fill in credentials using semantic locators
        await page.GetByLabel("Username").FillAsync("tomsmith");
        await page.GetByLabel("Password").FillAsync("SuperSecretPassword!");

        // Click login button using role-based locator
        await page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();

        // Use Playwright's built-in assertion with auto-waiting
        await Assertions.Expect(page.Locator("#flash")).ToBeVisibleAsync();
        await Assertions.Expect(page.Locator("#flash")).ToContainTextAsync("You logged into a secure area!");

        // Verify we're on the secure page
        await Assertions.Expect(page).ToHaveURLAsync(new Regex(".*/secure$"));
    }

    [Test]
    public async Task WaitForDynamicElement()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
        var page = await browser.NewPageAsync();

        // Navigate to a page with dynamically loaded content
        await page.GotoAsync("https://the-internet.herokuapp.com/dynamic_loading/1");

        // Click the Start button to trigger dynamic loading
        await page.GetByRole(AriaRole.Button, new() { Name = "Start" }).ClickAsync();

        // Playwright automatically waits for the element to be visible (up to timeout)
        // This element appears after a 5 second delay, so we need a longer timeout
        await Assertions.Expect(page.Locator("#finish")).ToBeVisibleAsync(new() { Timeout = 10000 });
        await Assertions.Expect(page.Locator("#finish h4")).ToHaveTextAsync("Hello World!");

        // Alternative: You can also use explicit wait
        await page.Locator("#finish").WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var finishText = await page.Locator("#finish h4").TextContentAsync();
        Assert.That(finishText, Is.EqualTo("Hello World!"));
    }

    [Test]
    public async Task ApiTest()
    {
        using var playwright = await Playwright.CreateAsync();
        var request = await playwright.APIRequest.NewContextAsync();

        var response = await request.GetAsync("https://api.github.com/repos/microsoft/playwright");
        Assert.That(response.Status, Is.EqualTo(200));

        var json = await response.JsonAsync();
        Console.WriteLine(json?.GetProperty("name"));
    }

    [Test]
    public async Task CombineApiAndUiTest()
    {
        using var playwright = await Playwright.CreateAsync();

        // Step 1: Call the API to get repository information
        var request = await playwright.APIRequest.NewContextAsync();
        var response = await request.GetAsync("https://api.github.com/repos/microsoft/playwright");
        Assert.That(response.Status, Is.EqualTo(200));

        // Step 2: Extract the repository name from the API response
        var json = await response.JsonAsync();
        var repoFullName = json?.GetProperty("full_name").GetString();
        var repoDescription = json?.GetProperty("description").GetString();

        Console.WriteLine($"Repository: {repoFullName}");
        Console.WriteLine($"Description: {repoDescription}");

        Assert.That(repoFullName, Is.Not.Null.And.Not.Empty);

        // Step 3: Use the API data in a UI test
        var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
        var page = await browser.NewPageAsync();

        // Navigate to the GitHub repository page using the API data from the API
        await page.GotoAsync($"https://github.com/{repoFullName}");

        // Verify the page loaded correctly by checking title and URL
        await Assertions.Expect(page).ToHaveTitleAsync(new Regex(".*playwright.*", RegexOptions.IgnoreCase));
        await Assertions.Expect(page).ToHaveURLAsync(new Regex(".*microsoft/playwright.*"));

        // Verify the page content contains the repository name
        var pageContent = await page.ContentAsync();
        Assert.That(pageContent, Does.Contain("playwright").IgnoreCase,
            "Page should contain the repository name from the API");

        Console.WriteLine($"✓ API returned: {repoFullName}");
        Console.WriteLine($"✓ Successfully navigated to: {page.Url}");
        Console.WriteLine($"✓ Page title: {await page.TitleAsync()}");
    }
}
