using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTests.Utils;

/// <summary>Contains methods related to the actions within the Irish Wilds game.</summary>
public static class GameActions
{
  /// <summary>Triggers a spin using the spin arrow button.</summary>
  /// <param name="isUnplacedBetModalExpected">
  /// Whether or not the unplaced bet error modal is expected. If true and found, the modal is closed.
  /// </param>
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

  /// <summary>Gets and returns the current stake amount, including the currency symbol.</summary>
  public static async Task<string?> GetCurrentStakeAmount(IPage page, int expectTimeout = Settings.ExpectTimeout)
  {
    return await page.Locator(".stake .amount").TextContentAsync(new() { Timeout = expectTimeout });
  }

  /// <summary>Gets and returns the current balance amount, including the currency symbol.</summary>
  public static async Task<string?> GetCurrentBalanceAmount(IPage page, int expectTimeout = Settings.ExpectTimeout)
  {
    return await page.Locator(".balance .amount").TextContentAsync(new() { Timeout = expectTimeout });
  }

  /// <summary>
  // Modifies the response returned after spinning by changing the balance and win properties.
  /// ⚠️ Note: this method needs to be called *before* the action that triggers the response.
  // </summary>
  /// <param name="isWin">Whether or not the spin is a winning one.</param>
  /// <param name="balance">The value to modify the total balance to.</param>
  /// <param name="winAmount">The value to modify the win amount to.</param>
  public static async Task ModifySpinResponse(IPage page, bool isWin, double balance, double winAmount)
  {
    await page.RouteAsync("**/Client/Action?ClientToken=*PlayerTokenId=*", async route =>
    {
      // fetch original response & its body
      IAPIResponse response = await route.FetchAsync();
      string body = await response.TextAsync();
      JsonNode bodyJson = JsonNode.Parse(body)!;

      // modify the response body
      bodyJson["Ticket"]!["IsWin"] = isWin.ToString();
      bodyJson["Ticket"]!["TotalWinAmount"] = winAmount.ToString();

      bodyJson["Balance"]!["BalanceAfter"] = balance.ToString();
      bodyJson["Balance"]!["RealBalance"] = balance.ToString();
      bodyJson["Balance"]!["TotalWinAmount"] = winAmount.ToString();

      string updatedBody = bodyJson.ToJsonString();

      // pass on the modified response
      await route.FulfillAsync(new()
      {
        // Pass all fields from the response.
        Response = response,
        // Override response body.
        Body = updatedBody,
      });
    });
  }
}


