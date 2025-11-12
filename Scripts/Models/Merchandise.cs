public class Merchandise {
    public readonly int Tier;
    public readonly MerchandiseColor Color;
    public readonly MerchandiseType Type;

    public Merchandise(int tier, MerchandiseColor color, MerchandiseType type) {
        Tier = tier;
        Color = color;
        Type = type;
    }

    public override string ToString() {
        return $"Tier: {Tier}, Color: {Color}, Type: {Type}";
    }
}