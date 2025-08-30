using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTests.Utils;

public static class Common
{
  public static async Task OpenHomepage(IPage page)
  {
    await page.GotoAsync(Settings.EnvironmentURL);

    await Expect(page.Locator(".button__slider-play")).ToBeVisibleAsync();
  }

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