# PlaywrightTutorial
learning Playwright as we go

## Project Structure
- `PlaywrightTests/` - NUnit test project with Playwright tests
  - `DemoTest.cs` - Comprehensive Playwright test examples
  - `CleanTests.cs` - Clean tests using base class
  - `PlaywrightTestBase.cs` - Base test class with setup/teardown
  - `HomePage.cs` - Page Object Model example
  - `ResultsPage.cs` - Page Object Model example
  - `.runsettings` - NUnit configuration for parallel execution
  - `PlaywrightTests.csproj` - Test project configuration
- `azure-pipelines.yml` - Azure DevOps CI/CD configuration
- `PlaywrightTutorial.md` - 7-lesson learning guide

## Running Tests

Run all tests:
```powershell
dotnet test
```

Run a specific test:
```powershell
dotnet test --filter TestName
```

Run tests in headless mode (faster):
```powershell
dotnet test
```

## Available Tests

The project includes examples of:
- ✓ Basic navigation and assertions
- ✓ Search functionality
- ✓ Locators and interactions
- ✓ Page Object Model
- ✓ Playwright assertions with auto-waiting
- ✓ Dynamic element waiting
- ✓ API testing combined with UI testing
- ✓ Screenshots and video recording
- ✓ Trace capture for debugging

## Requirements
- .NET 10.0
- Microsoft.Playwright.NUnit (v1.57.0)
- NUnit test framework

## Setup

1. Clone the repository
2. Restore packages:
```powershell
dotnet restore
```

3. Build the project:
```powershell
dotnet build
```

4. Install Playwright browsers:
```powershell
pwsh PlaywrightTests/bin/Debug/net10.0/playwright.ps1 install
```

## Debugging

### Screenshots and Videos
Tests automatically capture screenshots and videos in the `bin/Debug/net10.0/` directory.

### Trace Viewer
To view captured traces:
```powershell
pwsh PlaywrightTests/bin/Debug/net10.0/playwright.ps1 show-trace trace.zip
```

Or upload `trace.zip` to: https://trace.playwright.dev

## CI/CD

The project includes an Azure DevOps pipeline configuration (`azure-pipelines.yml`) that:
- Runs tests on every commit
- Publishes test results
- Stores screenshots, videos, and traces as artifacts

## Learning Path

See [PlaywrightTutorial.md](PlaywrightTutorial.md) for a 7-lesson hands-on learning plan covering:
1. Install Playwright + Run Your First Test
2. Locators + Interactions
3. Page Objects (Clean Test Structure)
4. Assertions, Waits, and Auto Wait Magic
5. API Testing + UI Testing Together
6. Screenshots, Videos, and Tracing
7. Run Playwright in Azure Pipelines

## Resources

- [Playwright Documentation](https://playwright.dev)
- [Playwright .NET API](https://playwright.dev/dotnet/)
- [Azure DevOps](https://dev.azure.com)

