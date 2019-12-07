using System.Collections.Generic;

static class TestUtils
{
    public static string MessageLists(string message, List<string> expected, List<string> result)
    {
        return $"{message}\n   Expected: {DisplayList(expected)}\n   Got: {DisplayList(result)}\n ========== \n";
    }

    public static string DisplayList(List<string> list)
    {
        return $"[{string.Join(", ", list)}]";
    }
}
