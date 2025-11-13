using System;
using Godot;

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

    public static Texture2dId GetMerchandiseTextureId(MerchandiseColor color, MerchandiseType type, int rarity) {
        return color switch {
            MerchandiseColor.Blue => type switch {
                MerchandiseType.DVD => rarity switch {
                    5 => Texture2dId.BlueDvd5,
                    4 => Texture2dId.BlueDvd4,
                    3 => Texture2dId.BlueDvd2,
                    2 => Texture2dId.BlueDvd2,
                    _ => Texture2dId.BlueDvd1,
                },
                _ => Texture2dId.MerchandisePlaceholder
            },
            MerchandiseColor.Red => type switch {
                MerchandiseType.DVD => rarity switch {
                    5 => Texture2dId.RedDvd5,
                    4 => Texture2dId.RedDvd4,
                    3 => Texture2dId.RedDvd2,
                    2 => Texture2dId.RedDvd2,
                    _ => Texture2dId.RedDvd1,
                },
                _ => Texture2dId.MerchandisePlaceholder
            },
            MerchandiseColor.Green => type switch {
                MerchandiseType.DVD => rarity switch {
                    5 => Texture2dId.GreenDvd5,
                    4 => Texture2dId.GreenDvd4,
                    3 => Texture2dId.GreenDvd2,
                    2 => Texture2dId.GreenDvd2,
                    _ => Texture2dId.GreenDvd1,
                },
                _ => Texture2dId.MerchandisePlaceholder
            },
            _ => Texture2dId.MerchandisePlaceholder
        };
    }
}