namespace Links.Models;

public sealed record LinkTag
{
    public string Name { get; set; }
    public int Count { get; set; }

    public float Weight { get; set; }

    // Needed for deserialization (... could use Json Constructor instead?)
    public LinkTag() { }

    public LinkTag(string name, int count)
    {
        Name = name;
        Count = count;
    }

    public LinkTag(string name, int count, float weight)
    {
        Name = name.Trim();
        Count = count;
        Weight = float.Round(weight, 6);
    }

    public static LinkTag Create(string name)
    {
        return new LinkTag(name, 1, 0);
    }
}