using Godot;
using Godot.Collections;
using RepositorySystem;

public partial class Texture2dRepository : Node, IAutoload, IRepository {
    public static string AutoloadPath { get; } = "/root/Texture2dRepository";

    [Export]
    private Dictionary<Texture2dId, Texture2D> _textures;

    public Texture2D GetTexture(Texture2dId id) {
        return _textures[id];
    }
}

public enum Texture2dId {
    CustomerPlaceholder = 0,
    MerchandisePlaceholder = 1,
    RichManHappy = 2,
    RichManNeutral = 3,
    RichManAngry = 4,
    RichWomanHappy = 5,
    RichWomanNeutral = 6,
    RichWomanAngry = 7,
    AverageManHappy = 8,
    AverageManNeutral = 9,
    AverageManAngry = 10,
    AverageWomanHappy = 11,
    AverageWomanNeutral = 12,
    AverageWomanAngry = 13,
    PoorManHappy = 14,
    PoorManNeutral = 15,
    PoorManAngry = 16,
    PoorWomanHappy = 17,
    PoorWomanNeutral = 18,
    PoorWomanAngry = 19,
    SlotPlaceholder = 20,
    BlueDvd1 = 21,
    BlueDvd2 = 22,
    BlueDvd3 = 23,
    BlueDvd4 = 24,
    BlueDvd5 = 25,
    RedDvd1 = 26,
    RedDvd2 = 27,
    RedDvd3 = 28,
    RedDvd4 = 29,
    RedDvd5 = 30,
    GreenDvd1 = 31,
    GreenDvd2 = 32,
    GreenDvd3 = 33,
    GreenDvd4 = 34,
    GreenDvd5 = 35,
    BlueDvdBlank = 36,
    RedDvdBlank = 37,
    GreenDvdBlank = 38,
    BadEnding = 39,
    NeutralEnding = 40,
    GoodEnding = 41,
}