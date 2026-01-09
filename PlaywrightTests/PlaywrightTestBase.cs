using Microsoft.Playwright;
using NUnit.Framework;
using System.IO;

/// <summary>
/// Base class for Playwright tests with common setup and teardown
/// Inherit from this class to avoid repetitive browser/page initialization
/// </summary>
public class PlaywrightTestBase
{
    protected IPlaywright? Playwright { get; private set; }
    protected IBrowser? Browser { get; private set; }
    protected IBrowserContext? Context { get; private set; }
    protected IPage? Page { get; private set; }
    
    /// <summary>
    /// Directory path for saving screenshots. Defaults to "screenshots/"
    /// Override this property in derived classes to customize the location
    /// </summary>
    protected virtual string ScreenshotDirectory { get; } = "screenshots/";

    [SetUp]
    public async Task Setup()
    {
        // Initialize Playwright
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        
        // Launch browser (set Headless = true for CI/CD)
        Browser = await Playwright.Chromium.LaunchAsync(new()
        {
            Headless = false // Set to true for headless mode
        });

        // Create a new browser context (isolated session)
        Context = await Browser.NewContextAsync(new()
        {
            // Optional: Set viewport size
            ViewportSize = new() { Width = 1280, Height = 720 },
            
            // Optional: Record video for failed tests
            RecordVideoDir = "test-videos/",
            
            // Optional: Set user agent
            // UserAgent = "Custom User Agent"
        });

        // Create a new page
        Page = await Context.NewPageAsync();
    }

    [TearDown]
    public async Task Teardown()
    {
        // Close page, context, browser in reverse order
        if (Page != null)
            await Page.CloseAsync();
            
        if (Context != null)
            await Context.CloseAsync();
            
        if (Browser != null)
            await Browser.CloseAsync();
            
        Playwright?.Dispose();
    }

    /// <summary>
    /// Helper method to take a screenshot with a descriptive name
    /// </summary>
    protected async Task TakeScreenshot(string name)
    {
        if (Page != null)
        {
            // Ensure the screenshot directory exists
            Directory.CreateDirectory(ScreenshotDirectory);
            
            await Page.ScreenshotAsync(new() 
            { 
                Path = Path.Combine(ScreenshotDirectory, $"{TestContext.CurrentContext.Test.Name}_{name}.png")
            });
        }
    }
}
