using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTests.Utils;

/// <summary>
/// Contains methods related to the actions within the Irish Wilds game.
/// </summary>
public static class GameActions
{
  /// <summary>
  /// Asserts that the balance amount is correct.
  /// </summary>
  /// <param name="amount">The expected amount as seen in the UI, e.g. "1,980.00"</param>
  public static async Task AssertBalanceAmountIsCorrect(
    IPage page, string amount, int expectTimeout = Settings. ExpectTimeout
  )
  {
    await Expect(page.Locator(".balance .amount")).ToHaveTextAsync($"${amount}", new() { Timeout = expectTimeout });
  }

  /// <summary>
  /// Asserts that the win amount is correct.
  /// </summary>
  /// <param name="amount">The expected amount as seen in the UI, e.g. "0.00"</param>
  public static async Task AssertWinAmountIsCorrect(
    IPage page, string amount, int expectTimeout = Settings.ExpectTimeout
  )
  {
    await Expect(page.Locator(".win .amount")).ToHaveTextAsync($"${amount}", new() { Timeout = expectTimeout });
  }

  /// <summary>
  /// Triggers a spin using the spin arrow button.
  /// </summary>
  /// <param name="isUnplacedBetModalExpected">
  // Whether or not the unplaced bet error modal is expected. If true and found, the modal is closed.
  // </param>
  public static async Task TriggerSpin(IPage page, bool isUnplacedBetModalExpected)
  {
    const string unplacedBetMsgTxt = "Bet was not placed";
    ILocator errorModal = page.Locator(".modal__window > ._error-popup");

    await page.Locator("button.arrows-spin-button").ClickAsync();

    if (isUnplacedBetModalExpected)
    {
      await Expect(errorModal.GetByText(unplacedBetMsgTxt)).ToBeVisibleAsync();
      await errorModal.Locator("button", new() { HasText = "OK" }).ClickAsync();
    }
  }
}