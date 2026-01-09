# PlaywrightTutorial
learning Playwright as we go

## Project Structure
- `PlaywrightTests/` - NUnit test project with Playwright tests
  - `DemoTest.cs` - Sample Playwright tests
  - `PlaywrightTests.csproj` - Test project configuration

## Running Tests

To run the Playwright tests:
```powershell
dotnet test
```

Or to run a specific test project:
```powershell
dotnet test PlaywrightTests/PlaywrightTests.csproj
```

## Requirements
- .NET 10.0
- Microsoft.Playwright.NUnit (v1.57.0)
- NUnit test framework

## Setup

After cloning, restore packages and install Playwright browsers:
```powershell
dotnet restore
pwsh PlaywrightTests/bin/Debug/net10.0/playwright.ps1 install
```

## Learning Path

See [PlaywrightTutorial.md](PlaywrightTutorial.md) for a 7-lesson hands-on learning plan.
