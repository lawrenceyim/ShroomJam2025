using System;

public class MerchandiseUtil {
    private static readonly int _numberOfColors = Enum.GetNames(typeof(MerchandiseColor)).Length;
    private static readonly int _numberOfMerchandiseTypes = Enum.GetNames(typeof(MerchandiseType)).Length;
    private static readonly Random _random = new();

    public static MerchandiseColor GetRandomMerchandiseColor() {
        int choice = _random.Next(0, _numberOfColors);
        return (MerchandiseColor)choice;
    }

    public static MerchandiseType GetRandomMerchandiseType() {
        int choice = _random.Next(0, _numberOfMerchandiseTypes);
        return (MerchandiseType)choice;
    }
}