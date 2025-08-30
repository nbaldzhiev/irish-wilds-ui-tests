using Microsoft.Playwright.NUnit;

using PlaywrightTests.Utils;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class BalanceUpdates : PageTest
{
  /// <summary>
  /// Open the homepage of the Irish Wilds game, starts the game and verifies the initial balances before each test.
  /// </summary>
  [SetUp]
  public async Task Setup()
  {
    // set the expect timeout to 15s in order to have control over it
    SetDefaultExpectTimeout(15_000);
  
    await Home.OpenHomepage(Page);
    await Home.StartGame(Page);
  
    await GameActions.AssertBalanceAmountIsCorrect(page: Page, amount: Settings.InitialBalanceAmount);
    await GameActions.AssertWinAmountIsCorrect(page: Page, amount: "0.00");
  }

  /// <summary>
  /// Verifies that the win amount updates accordingly upon winning spins.
  /// </summary>
  [Test]
  public async Task Win()
  {
    await GameActions.TriggerSpin(Page);
  }
}