using Microsoft.Playwright;
using NUnit.Framework;

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
    /// Directory where video recordings will be saved. Defaults to "test-videos/".
    /// </summary>
    protected string VideoRecordingDirectory { get; set; }

    /// <summary>
    /// Creates a new instance of PlaywrightTestBase with optional video recording directory
    /// </summary>
    /// <param name="videoRecordingDirectory">Directory for video recordings. Defaults to "test-videos/"</param>
    public PlaywrightTestBase(string videoRecordingDirectory = "test-videos/")
    {
        VideoRecordingDirectory = videoRecordingDirectory;
    }

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
            RecordVideoDir = VideoRecordingDirectory,
            
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
            await Page.ScreenshotAsync(new() 
            { 
                Path = $"screenshots/{TestContext.CurrentContext.Test.Name}_{name}.png" 
            });
        }
    }
}
