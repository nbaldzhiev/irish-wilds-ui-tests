using Microsoft.Playwright;

using PlaywrightTests.Utils;

namespace PlaywrightTests;


/// <summary>
/// Contains tests verifying that the balances update accordingly when winning/losing.
///
/// ⚠️⚠️
/// Please note: the tests verify the balances by modifying the spin response, not actually relying on a winning spin.
/// This is done so that the tests aren't reliant on the results of spins, which are random and non-deterministic.
/// ⚠️⚠️
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class BalanceUpdates
{

  private IPlaywright _playwright;
  private IBrowser _browser;
  private IBrowserContext _context;
  private IPage _page;
  private string deviceName;

  /// <summary>
  /// Creates a new browser & browser context depending on the selected device type.
  /// Starts trace recording (upon any test result). Trace zips are located in bin/Debug/netX/playwright-traces/
  /// Open the homepage of the Irish Wilds game, starts the game and verifies the initial balances before each test.
  /// </summary>
  [SetUp]
  public async Task Setup()
  {
    var headless = bool.Parse(Environment.GetEnvironmentVariable("HEADLESS") ?? "true");
    deviceName = Environment.GetEnvironmentVariable("DEVICE") ?? "";

    if (string.IsNullOrEmpty(deviceName))
    {
      throw new Exception("Setup failed: Argument `deviceName` cannot be null!");
    }


    // start a browser context with the provided device
    _playwright = await Playwright.CreateAsync();
    _browser = await _playwright.Chromium.LaunchAsync(new()
    {
      Headless = headless
    });
    _context = await _browser.NewContextAsync(_playwright.Devices[deviceName]);
    _page = await _context.NewPageAsync();

    // start trace recording
    await _context.Tracing.StartAsync(new()
    {
      Title = TestContext.CurrentContext.Test.ClassName + "." + TestContext.CurrentContext.Test.Name + $".{deviceName}",
      Screenshots = true,
      Snapshots = true,
      Sources = true
    });

    await Home.OpenHomepage(_page);
    await Home.StartGame(_page);

    await GameAssertions.AssertBalanceAmountIsCorrect(page: _page, amount: Settings.InitialBalanceAmount);
    await GameAssertions.AssertWinAmountIsCorrect(page: _page, amount: "0.00");
  }

  /// <summary>
  /// Stops the trace recording started in the setup.
  /// Gracefully closes up the browser context & browser objects after creating them manually in the setup phase.
  /// </summary>
  [TearDown]
  public async Task Teardown()
  {
    // trace zips are stored in bin/Debug/netX/playwright-traces/
    await _context.Tracing.StopAsync(new()
    {
      Path = Path.Combine(
        TestContext.CurrentContext.WorkDirectory,
        "playwright-traces",
        $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}.{deviceName}.zip"
      )
    });

    await _context.CloseAsync();
    await _browser.CloseAsync();
  }

  /// <summary>Verifies that the balance update accordingly when winning.</summary>
  [Test]
  public async Task ShouldBeAbleToHaveWinUpdates()
  {
    // note: the assertions can break if the win amount is of varying format. TODO: improve on that in the future to
    // make it more flexible
    const double newWinAmount = 900.99;
    double balanceAmount = Common.ParseAmount((await GameActions.GetCurrentBalanceAmount(_page))!);
    double newBalanceAmount = balanceAmount + newWinAmount;

    await GameActions.ModifySpinResponse(
      page: _page,
      isWin: true,
      winAmount: newWinAmount,
      balance: balanceAmount + newWinAmount
    );
    await GameActions.TriggerSpin(page: _page, isUnplacedBetModalExpected: false);

    await GameAssertions.AssertBalanceAmountIsCorrect(page: _page, amount: newBalanceAmount.ToString());
    await GameAssertions.AssertWinAmountIsCorrect(page: _page, amount: newWinAmount.ToString());
  }

  /// <summary>Verifies that the balance update accordingly when losing.</summary>
  [Test]
  public async Task ShouldBeAbleToHaveLossUpdates()
  {
    double newBalanceAmount = double.Parse(Settings.InitialBalanceAmount) - 1000;

    await GameActions.ModifySpinResponse(
      page: _page,
      isWin: false,
      winAmount: 0.00,
      balance: newBalanceAmount
    );
    await GameActions.TriggerSpin(page: _page, isUnplacedBetModalExpected: false);

    await GameAssertions.AssertBalanceAmountIsCorrect(page: _page, amount: newBalanceAmount.ToString());
    await GameAssertions.AssertWinAmountIsCorrect(page: _page, amount: "0.00");
  }
}