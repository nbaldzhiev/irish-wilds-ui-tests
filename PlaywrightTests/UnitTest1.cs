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
    await Home.OpenHomepage(Page);

    await Home.StartGame(Page);

    await GameActions.AssertBalanceAmountIsCorrect(page: Page, amount: Settings.InitialBalanceAmount);
    await GameActions.AssertWinAmountIsCorrect(page: Page, amount: "0.00");
  }
}