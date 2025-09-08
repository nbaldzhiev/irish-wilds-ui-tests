using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTests.Utils;

/// <summary>Contains assertion methods related to the Irish Wilds game.</summary>
public static class GameAssertions
{
  /// <summary>Asserts that the balance amount is correct.</summary>
  /// <param name="amount">The expected amount as seen in the UI, e.g. 1,980.00</param>
  public static async Task AssertBalanceAmountIsCorrect(
    IPage page, double amount, int expectTimeout = Settings.ExpectTimeout
  )
  {
    string formatted = amount.ToString("N2");
    await Expect(page.Locator(".balance .amount")).ToHaveTextAsync($"${formatted}", new() { Timeout = expectTimeout });
  }

  /// <summary>Asserts that the win amount is correct.</summary>
  /// <param name="amount">The expected amount as seen in the UI, e.g. 0.00</param>
  public static async Task AssertWinAmountIsCorrect(
    IPage page, double amount, int expectTimeout = Settings.ExpectTimeout
  )
  {
    string formatted = amount.ToString("N2");
    await Expect(page.Locator(".win .amount")).ToHaveTextAsync($"${formatted}", new() { Timeout = expectTimeout });
  }
}