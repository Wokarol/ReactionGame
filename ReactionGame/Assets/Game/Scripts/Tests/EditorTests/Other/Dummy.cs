using System.Collections.Generic;

public class Dummy
{
    public readonly string name;

    public Dummy(string name) => this.name = name;

    // Utilities
    public static List<Dummy> GetList(int number)
    {
        var result = new List<Dummy>(number);
        for (int i = 0; i < number; i++) {
            result.Add(new Dummy($"[{number}]"));
        }
        return result;
    }
}
