namespace CheekyStork.Extensions
{
    // This lets you easily add a color to a string. E.g. "This text is red".Color("red")
    public static class StringExtensions
    {
        public static string Color(this string str, string color)
        {
            return $"<color={color}>{str}</color>";
        }
    }
}