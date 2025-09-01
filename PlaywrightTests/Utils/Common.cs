namespace PlaywrightTests.Utils;

/// <summary>Contains common methods related that can be used throughout any of the tests.</summary>
public static class Common
{
  /// <summary>Parses and returns a given amount without the currency symbol.</summary>
  public static float ParseAmount(string amount)
  {
    Assert.That(amount, Is.Not.Null.And.Not.Empty);
    return float.Parse(amount[1..]);
  }
}