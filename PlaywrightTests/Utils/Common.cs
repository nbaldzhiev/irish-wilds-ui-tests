using System.Threading.Tasks;
using Microsoft.Playwright;
using System.Threading.Tasks;


namespace PlaywrightTests.Utils;

public static class Common
{
  public static async Task OpenHomepage(IPage page)
  {
    await page.GotoAsync(Settings.EnvironmentURL);
  }
}