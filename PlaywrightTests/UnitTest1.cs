using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

using PlaywrightTests.Utils;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ExampleTest : PageTest
{
  [OneTimeSetUp]
  public void GlobalSetup()
  {
    SetDefaultExpectTimeout(15_000);
  }

  [Test]
  public async Task HasTitle()
  {
    await Common.OpenHomepage(Page);

    await Common.StartGame(Page);
  }

  [Test]
  public async Task GetStartedLink()
  {
    await Page.GotoAsync("https://playwright.dev");

    // Click the get started link.
    await Page.GetByRole(AriaRole.Link, new() { Name = "Get started" }).ClickAsync();

    // Expects page to have a heading with the name of Installation.
    await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Installation" })).ToBeVisibleAsync();
  }
}