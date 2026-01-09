# Playwright Learning Plan (C# Edition)

A 7-lesson, hands on learning plan focused on practical examples rather than heavy reading. Designed for C# developers who want to learn Playwright by doing. Please note that after I created the code, I went back and forth with Copilot on perfecting it. Thus, the actual code will not always match exactly what is in this tutorial.

## LESSON 1 — Install Playwright + Run Your First Test

### Goal
- Set up Playwright locally and run your first browser test.

### Steps
- Create a new test project:

1. Create a new NUnit test project
dotnet new nunit -n PlaywrightDemo
cd PlaywrightDemo
dotnet build

2. Add the Microsoft.Playwright package
dotnet add package Microsoft.Playwright

3. Install Playwright browsers using PowerShell
pwsh ./bin/Debug/net10.0/playwright.ps1 install

4. Create a basic test using C#:
```csharp
using Microsoft.Playwright; 
using NUnit.Framework;

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
```

### Challenge
Modify the test to search for a term and assert results appear.

## LESSON 2 — Locators + Interactions

### Goal
Learn how to click, type, and assert elements.

### Example
```csharp
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
```

### Challenge
Click the first result and assert the new page title.

## LESSON 3 — Page Objects (Clean Test Structure)

### Goal
Organize tests using the Page Object Model.

### Example
HomePage.cs
```csharp
public class HomePage
{
    private readonly IPage _page;
    public HomePage(IPage page) => _page = page;

    public ILocator SearchBox => _page.Locator("input[name='q']");
    public async Task Search(string text)
    {
        await SearchBox.FillAsync(text);
        await _page.Keyboard.PressAsync("Enter");
    }
}
```

DemoTest.cs
```csharp
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
```

### Challenge
Add a ResultsPage class and assert the first result text.

## LESSON 4 — Assertions, Waits, and Auto Wait Magic

### Goal
Understand Playwright’s built in waiting and assertion system.

### Example
```csharp
await page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();
await Expect(page.Locator("#welcome")).ToBeVisibleAsync();
```

### Challenge
Write a test that waits for a dynamic element to appear.

## LESSON 5 — API Testing + UI Testing Together

### Goal
Use Playwright’s API testing capabilities.

### Example
```csharp
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
```

### Challenge
Call an API, extract a value, and use it inside a UI test.

## LESSON 6 — Screenshots, Videos, and Tracing

### Goal
Capture artifacts for debugging.

### Example
```csharp
await page.ScreenshotAsync(new() { Path = "homepage.png" });
await page.Video?.SaveAsAsync("testvideo.webm");
```

### Challenge
Enable tracing and open the trace viewer.

(this is not done yet, I have to learn about Azure DevOps first)
## LESSON 7 — Run Playwright in Azure Pipelines

### Goal
Execute Playwright tests in Azure DevOps.

### Example YAML
```yaml
pool:
  vmImage: 'windows-latest'

steps:
- task: UseDotNet@2
  inputs:
    packageType: 'sdk'
    version: '8.x'

- script: dotnet restore
- script: dotnet build --configuration Release

- script: pwsh bin/Debug/net*/playwright.ps1 install

- script: dotnet test --logger "trx"
```
### To use this in Azure DevOps:
Push the azure-pipelines.yml to your repository

In Azure DevOps:

- Go to Pipelines → New Pipeline
- Select your repository
- Choose "Existing Azure Pipelines YAML file"
- Select azure-pipelines.yml
- Click Run
- View results:

Test results appear in the Tests tab
Screenshots/videos/traces are in the Artifacts section

### Challenge
Publish Playwright HTML reports as pipeline artifacts.
## BONUS — Clean Test Structure with Base Classes

### Goal
Eliminate repetitive setup code and make tests cleaner and more maintainable.

### The Problem
Every test requires the same boilerplate:
```csharp
using var playwright = await Playwright.CreateAsync();
var browser = await playwright.Chromium.LaunchAsync(...);
var page = await browser.NewPageAsync();
// actual test code
```

### The Solution: PlaywrightTestBase

Create a base class that handles setup and teardown:

**PlaywrightTestBase.cs**
```csharp
using Microsoft.Playwright;
using NUnit.Framework;

public class PlaywrightTestBase
{
    protected IPlaywright? Playwright { get; private set; }
    protected IBrowser? Browser { get; private set; }
    protected IPage? Page { get; private set; }

    [SetUp]
    public async Task Setup()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new() { Headless = false });
        Page = await Browser.NewPageAsync();
    }

    [TearDown]
    public async Task Teardown()
    {
        if (Page != null) await Page.CloseAsync();
        if (Browser != null) await Browser.CloseAsync();
        Playwright?.Dispose();
    }
}
```

### Usage Example

**CleanTests.cs**
```csharp
public class CleanTests : PlaywrightTestBase
{
    [Test]
    public async Task NavigateToPlaywright()
    {
        // No setup needed - Page is already available!
        await Page!.GotoAsync("https://playwright.dev");
        await Assertions.Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));
    }

    [Test]
    public async Task SearchFunctionality()
    {
        await Page!.GotoAsync("https://playwright.dev");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Search" }).ClickAsync();
        await Page.GetByPlaceholder("Search docs").FillAsync("test");
        
        await Assertions.Expect(Page.GetByPlaceholder("Search docs"))
            .ToHaveValueAsync("test");
    }
}
```

### Benefits
- ✓ Less boilerplate code
- ✓ Tests are easier to read and write
- ✓ Consistent setup/teardown across all tests
- ✓ Easy to add common utilities (screenshots, logging, etc.)
- ✓ Better for scaling test suites

### Challenge
Add a helper method to the base class for taking screenshots with test names.

## BONUS — Parallel Test Execution

### Goal
Speed up test execution by running tests in parallel.

### Configuration

Create `.runsettings` file:
```xml
<RunSettings>
  <NUnit>
    <NumberOfTestWorkers>4</NumberOfTestWorkers>
  </NUnit>
</RunSettings>
```

### Usage
```powershell
dotnet test --settings PlaywrightTests/.runsettings
```

### Benefits
- ✓ 4x faster test execution (with 4 workers)
- ✓ Better resource utilization
- ✓ Essential for large test suites

### Important Notes
- Each test gets its own browser instance (already isolated)
- Tests must be independent (no shared state)
- Great for CI/CD pipelines
## This plan provides a practical, example driven path to learning Playwright with C#, focusing on real tasks rather than heavy documentation.