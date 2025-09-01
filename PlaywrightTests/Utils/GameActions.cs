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
    const string path = "**/Client/Action?ClientToken=*PlayerTokenId=*";
    const string dummyGameState = "{\"InitialData\":null,\"AccumulatedWinAmount\":0.0,\"CurrActionWinnings\":0.0,\"CurrentAwardBet\":200.0,\"UserData\":{\"PreviousFinalMatrixBase\":[[3,9,7,10],[10,6,7,4],[0,0,0,9],[7,2,2,9],[1,9,7,6]],\"PreviousFinalMatrixFeatureSpin\":null,\"FinalMatrixBase\":[[6,1,1,7],[12,2,2,10],[7,8,10,3],[8,6,7,3],[7,6,7,5]],\"FinalMatrixFeatureSpin\":null},\"StopMatrix\":[[6,1,1,7],[12,15,2,10],[7,8,10,16],[8,6,7,16],[7,6,7,18]],\"FinalMatrix\":[[6,1,1,7],[12,2,2,10],[7,8,10,3],[8,6,7,3],[7,6,7,5]],\"Lines\":null,\"FreeSpins\":null,\"Respins\":null,\"Scatters\":null,\"Wilds\":null,\"Gamble\":null,\"BetChance\":null,\"CollectionTrayCurrent\":{\"StopTray\":[0,0,0,0,1],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,1,1,1,1],\"FinalTray\":[0,1,1,1,1],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":200.0},\"CollectionTraysBaseGame\":[{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":10.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":20.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":50.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":100.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":150.0},{\"StopTray\":[0,0,0,0,1],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,1,1,1,1],\"FinalTray\":[0,1,1,1,1],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":200.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":250.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":300.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":400.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":500.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":750.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":1000.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":1250.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":1500.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":2000.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":2500.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":5000.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":7500.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":10000.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":12000.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":14000.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":16000.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":18000.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":20000.0},{\"StopTray\":[0,0,0,0,0],\"TrayAfterClear\":[0,0,0,0,0],\"TrayAfterCollection\":[0,0,0,0,0],\"FinalTray\":[0,0,0,0,0],\"WillBeSaved\":false,\"WasSaved\":false,\"Stake\":22500.0}],\"EngineInfo\":{\"Version\":\"1.0.0.0\",\"GameCode\":\"IWD\"}}";

    await page.RouteAsync(path, async route =>
    {
      // fetch original response & its body
      // pass on the modified response
      await route.FulfillAsync();
    });
  }
}


