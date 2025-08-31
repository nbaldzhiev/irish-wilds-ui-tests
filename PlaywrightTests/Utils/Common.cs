using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTests.Utils;

/// <summary>
/// Contains methods related to the homepage of the Irish Wilds game.
/// </summary>
public static class Home
{

  /// <summary>
  /// Opens the homepage of the Irish Wilds game.
  /// </summary>
  public static async Task OpenHomepage(IPage page)
  {
    await page.GotoAsync(Settings.EnvironmentURL);

    await Expect(page.Locator(".button__slider-play")).ToBeVisibleAsync();
  }

  /// <summary>
  /// Starts the Irish Wilds game by clicking on the homepage start button. After that, it verifies that the main
  /// elements on the game page are visible.
  /// </summary>
  public static async Task StartGame(IPage page)
  {
    await page.Locator(".button__slider-play").ClickAsync();

    List<string> selectors = [
      "#game__container canvas",
      "[class*='display balance']",
      "[class*='display win']",
      "button.game__menu",
      ".arrows-spin-button",
    ];

    foreach (string selector in selectors)
    {
      await Expect(page.Locator(selector)).ToBeVisibleAsync();
    }
  }
}