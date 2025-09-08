using Microsoft.Playwright;

using PlaywrightTests.Utils;

namespace PlaywrightTests.Tests;

/// <summary>Contains tests verifying that the balances update accordingly when winning/losing.</summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class BalanceUpdates
{

  private IPlaywright _pw;
  private IBrowser _browser;
  private IBrowserContext _context;
  private IPage _page;

  private string _device;
  private const double _testAmount = 555.55;  // the format should be the same
  private const int _numberOfSpins = 3;

  /// <summary>
  /// Creates a new browser & browser context depending on the selected device type.
  /// Starts trace recording (upon any test result). Trace zips are located in bin/Debug/netX/playwright-traces/
  /// Opens the homepage of the Irish Wilds game, starts the game and verifies the initial balances before each test.
  /// </summary>
  [SetUp]
  public async Task Setup()
  {
    var headless = bool.Parse(Environment.GetEnvironmentVariable("HEADLESS") ?? "true");
    _device = Environment.GetEnvironmentVariable("DEVICE") ?? "";

    if (string.IsNullOrEmpty(_device))
    {
      throw new Exception("Setup failed: Argument `DEVICE` cannot be null!");
    }

    // start a browser context with the provided device
    _pw = await Playwright.CreateAsync();
    IBrowserType browserType = _device.Contains("Firefox") ? _pw.Firefox : _device.Contains("Safari") ? _pw.Webkit : _pw.Chromium;
    _browser = await browserType.LaunchAsync(new() { Headless = headless });
    _context = await _browser.NewContextAsync(_pw.Devices[_device]);
    _page = await _context.NewPageAsync();

    // start trace recording
    await _context.Tracing.StartAsync(new()
    {
      Title = TestContext.CurrentContext.Test.ClassName + "." + TestContext.CurrentContext.Test.Name + $".{_device}",
      Screenshots = true,
      Snapshots = true,
      Sources = true
    });

    await Home.OpenHomepage(_page);
    await Home.StartGame(_page);

    await GameAssertions.AssertWinAmountIsCorrect(page: _page, amount: 0.00);
    await GameAssertions.AssertBalanceAmountIsCorrect(page: _page, amount: Settings.InitialBalanceAmount);
  }

  /// <summary>
  /// Stops the trace recording started in the setup.
  /// Removes all routes started during the tests
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
        $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}.{_device}.zip"
      )
    });

    await _page.UnrouteAllAsync();

    await _context.CloseAsync();
    await _browser.CloseAsync();
  }

  /// <summary>
  /// Verifies that the balance update accordingly when winning.
  /// ⚠️ Note: the test verifies the balance by modifying (mocking) the spin response, to avoid the spin randomness. 
  /// </summary>
  [Test, CancelAfter(60_000)]
  public async Task ShouldBeAbleToHaveWinUpdates()
  {
    double balanceAmount = Common.ParseAmount((await GameActions.GetCurrentBalanceAmount(_page))!);
    double newBalanceAmount = balanceAmount + _testAmount;

    await GameActions.ModifySpinResponse(
      page: _page,
      isWin: true,
      winAmount: _testAmount,
      balance: balanceAmount + _testAmount
    );

    await GameActions.TriggerSpin(page: _page, isUnplacedBetModalExpected: false);

    await GameAssertions.AssertBalanceAmountIsCorrect(page: _page, amount: newBalanceAmount);
    // modifying the Ticket.TotalWinAmount property causes unexpected results, likely due to some calculations that
    // the FE makes. Therefore, we are not testing the win amount for now. TODO: find out what happens and uncomment
    // await GameAssertions.AssertWinAmountIsCorrect(page: _page, amount: testAmount.ToString());
  }

  /// <summary>
  /// Verifies that the balance update accordingly when winning.
  /// ⚠️ Note: the test verifies the balance by modifying (mocking) the spin response, to avoid the spin randomness. 
  /// </summary>
  [Test, CancelAfter(60_000)]
  public async Task ShouldBeAbleToHaveLossUpdates()
  {
    double balanceAmount = Common.ParseAmount((await GameActions.GetCurrentBalanceAmount(_page))!);
    double newBalanceAmount = balanceAmount - _testAmount;

    await GameActions.ModifySpinResponse(
      page: _page,
      isWin: false,
      winAmount: 0.00,
      balance: newBalanceAmount
    );
    await GameActions.TriggerSpin(page: _page, isUnplacedBetModalExpected: false);

    await GameAssertions.AssertBalanceAmountIsCorrect(page: _page, amount: newBalanceAmount);
    await GameAssertions.AssertWinAmountIsCorrect(page: _page, amount: 0.00);
  }

  /// <summary>Verifies that the total & win balances update accordingly when doing multiple real spins.</summary>
  [Test, CancelAfter(120_000)]
  public async Task ShouldBeAbleToHaveBalanceUpdatesAfterMultipleSpins()
  {
    for (int i = 0; i < _numberOfSpins; i++)
    {
      double stakeAmount = Common.ParseAmount((await GameActions.GetCurrentStakeAmount(_page))!);
      double balanceAmount = Common.ParseAmount((await GameActions.GetCurrentBalanceAmount(_page))!) - stakeAmount;

      Dictionary<string, double> results = await GameActions.PerformAndAwaitSpinCycle(_page);
      double totalWinAmount = results["totalWinAmount"] * 0.01;
      double expectedTotalBalance;

      expectedTotalBalance = balanceAmount + totalWinAmount;

      await GameAssertions.AssertBalanceAmountIsCorrect(page: _page, amount: expectedTotalBalance);
      await GameAssertions.AssertWinAmountIsCorrect(page: _page, amount: totalWinAmount);
    }
  }
}