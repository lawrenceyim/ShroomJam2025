using System.Collections.Generic;
using Godot;

public partial class ShelfView : Node2D {
    [Export]
    private PackedScene _merchandiseSlotPackedScene;

    private readonly Vector2I _originPosition = new Vector2I(-500, -210);

    private readonly int _xOffset = 170;
    private readonly int _yOffset = 205;
    private MerchandiseService _merchandiseService;
    private PackedSceneRepository _packedSceneRepository;
    private Vector2I? _hoveredSlot;
    private Dictionary<Vector2I, MerchandiseSlot> _merchandiseSlots = new();
    private Texture2dRepository _texture2dRepository;

    public void Initialize(MerchandiseService merchandiseService, Texture2dRepository texture2dRepository) {
        _merchandiseService = merchandiseService;
        _texture2dRepository = texture2dRepository;
        Vector2I shelfSize = merchandiseService.GetShelfSize();

        for (int column = 0; column < shelfSize.X; column++) {
            for (int row = 0; row < shelfSize.Y; row++) {
                MerchandiseSlot merchandiseSlot = _merchandiseSlotPackedScene.Instantiate() as MerchandiseSlot;
                Vector2I position = new Vector2I(column, row);
                merchandiseSlot.Initialize(position);
                merchandiseSlot.Position = _originPosition + new Vector2I(column * _xOffset, row * _yOffset);
                merchandiseSlot.Initialize(position);
                merchandiseSlot.MouseEntered += (Vector2I position) => { _hoveredSlot = position; };
                merchandiseSlot.MouseExited += () => _hoveredSlot = null;
                _merchandiseSlots[position] = merchandiseSlot;
                AddChild(merchandiseSlot);

                Merchandise merchandise = _merchandiseService.GetMerchandiseFromShelf(new Vector2I(column, row));
                RefreshShelfMerchandiseTexture(position);
            }
        }
    }

    public void SetMerchandiseTexture(Vector2I position, Texture2D texture) {
        _merchandiseSlots[position].SetTexture(texture);
    }

    public void RefreshShelfMerchandiseTexture(Vector2I position) {
        Merchandise merchandise = _merchandiseService.GetMerchandiseFromShelf(position);
        if (merchandise is not null) {
            Texture2dId textureId = MerchandiseUtil.GetMerchandiseTextureId(merchandise.Color, merchandise.Type, merchandise.Tier);
            SetMerchandiseTexture(position, _texture2dRepository.GetTexture(textureId));
        }
        else {
            SetMerchandiseTexture(position, _texture2dRepository.GetTexture(Texture2dId.SlotPlaceholder));
        }
    }

    public Vector2I? GetHoveredSlot() {
        return _hoveredSlot;
    }
}