public enum StatModType
{
    Flat = 100,
    PercentAdd = 200,
    PercentMult = 300,
}
public class StatModifier
{
    public float value;
    public readonly StatModType Type;
    public readonly int order;
    public readonly object source;

    public StatModifier(float value, StatModType type, int order, object source)
    {
        this.value = value;
        Type = type;
        this.order = order;
        this.source = source;
    }
    public StatModifier(float value, StatModType type): this(value, type, (int)type, null) { }
    public StatModifier(float value, StatModType type, int order) : this(value, type, order, null) { }
    public StatModifier(float value, StatModType type, object source) : this(value, type, (int)type, source) { }
}
