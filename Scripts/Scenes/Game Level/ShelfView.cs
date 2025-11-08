using System.Collections.Generic;
using Godot;

public partial class ShelfView : Node2D {
    private readonly Vector2I _originPosition = new Vector2I(330, 270);
    private readonly int _xOffset = 170;
    private readonly int _yOffset = 200;
    private DvdService _dvdService;
    private PackedSceneRepository _packedSceneRepository;
    private PackedScene _dvdPackedScene;
    private Vector2I? _hoveredSlot;
    private Dictionary<Vector2I, DvdSlot> _dvdSlots = new();

    public void Initialize(DvdService dvdService, PackedSceneRepository packedSceneRepository) {
        _dvdService = dvdService;
        _packedSceneRepository = packedSceneRepository;
        PackedScene dvdPackedScene = _packedSceneRepository.GetPackedScene(PackedSceneId.Dvd);
        Vector2I shelfSize = dvdService.GetShelfSize();
        GD.Print(shelfSize);

        for (int column = 0; column < shelfSize.X; column++) {
            for (int row = 0; row < shelfSize.Y; row++) {
                DvdSlot dvdSlot = new();
                Vector2I position = new Vector2I(column, row);
                dvdSlot.Position = _originPosition + new Vector2I(column * _xOffset, row * _yOffset);
                dvdSlot.Initialize(position);
                dvdSlot.MouseEntered += (Vector2I position) => _hoveredSlot = position;
                dvdSlot.MouseExited += () => _hoveredSlot = null;
                _dvdSlots[position] = dvdSlot;
                AddChild(dvdSlot);

                Dvd dvd = _dvdService.GetDvdFromShelf(new Vector2I(column, row));
                if (dvd != null) {
                    // set sprite based on tier and color of the dvd                    
                }
            }
        }
    }

    public void SetDvdTexture(Vector2I slot, Texture2D texture) {
        _dvdSlots[slot].SetTexture(texture);
    }

    public Vector2I? GetHoveredSlot() {
        return _hoveredSlot;
    }
}