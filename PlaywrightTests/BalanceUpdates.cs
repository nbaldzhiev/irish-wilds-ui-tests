using Microsoft.Playwright;
using NUnit.Framework;

using PlaywrightTests.Utils;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class BalanceUpdates
{

  private IPlaywright _playwright;
  private IBrowser _browser;
  private IBrowserContext _context;
  private IPage _page;

  /// <summary>
  /// Creates a new browser & browser context depending on the selected device type.
  /// Starts trace recording (upon any test result). Trace zips are located in bin/Debug/netX/playwright-traces/
  /// Open the homepage of the Irish Wilds game, starts the game and verifies the initial balances before each test.
  /// </summary>
  [SetUp]
  public async Task Setup()
  {
    var headless = bool.Parse(Environment.GetEnvironmentVariable("HEADLESS") ?? "true");
    var deviceName = Environment.GetEnvironmentVariable("DEVICE");

    if (string.IsNullOrEmpty(deviceName))
    {
      throw new Exception("Setup failed: Argument `deviceName` cannot be null");
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
      Title = TestContext.CurrentContext.Test.ClassName + "." + TestContext.CurrentContext.Test.Name,
      Screenshots = true,
      Snapshots = true,
      Sources = true
    });

    // set the expect timeout to 15s in order to have control over it
    // SetDefaultExpectTimeout(15_000);

    await Home.OpenHomepage(_page);
    await Home.StartGame(_page);

    await GameActions.AssertBalanceAmountIsCorrect(page: _page, amount: Settings.InitialBalanceAmount);
    await GameActions.AssertWinAmountIsCorrect(page: _page, amount: "0.00");
  }

  /// <summary>
  /// Stops the trace recording started in the setup.
  /// Gracefully close up the browser context & browser itself after creating them manually in the setup phase.
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
        $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}.zip"
      )
    });

    await _context.CloseAsync();
    await _browser.CloseAsync();
  }

  /// <summary>
  /// Verifies that the win amount updates accordingly upon winning spins.
  /// </summary>
  [Test]
  public async Task Win()
  {
    await GameActions.TriggerSpin(_page);
  }

  [Test]
  public async Task Loss()
  {
    await GameActions.AssertWinAmountIsCorrect(_page, "0.00")
  }
}