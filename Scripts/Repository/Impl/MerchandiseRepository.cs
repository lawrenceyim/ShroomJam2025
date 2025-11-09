using System.Collections.Generic;
using Godot;
using RepositorySystem;

public class MerchandiseRepository : IRepository {
    private int _numberOfRows = 2;
    private int _numberOfColumns = 2;

    private Merchandise _heldMerchandise;
    private Dictionary<Vector2I, Merchandise> _shelf = new Dictionary<Vector2I, Merchandise>();

    public void RemoveDvd(Vector2I position) {
        _shelf.Remove(position);
    }

    public void AddDvd(Merchandise merchandise, Vector2I position) {
        _shelf[position] = merchandise;
    }

    public Merchandise GetDvd(Vector2I position) {
        return _shelf.GetValueOrDefault(position, null);
    }

    public void SetHeldDvd(Merchandise heldMerchandise) {
        _heldMerchandise = heldMerchandise;
    }

    public Merchandise GetHeldDvd() {
        return _heldMerchandise;
    }

    public void RemoveHeldDvd() {
        _heldMerchandise = null;
    }

    public Vector2I GetShelfSize() {
        return new Vector2I(_numberOfColumns, _numberOfRows);
    }

    public void SetShelfSize(Vector2I shelfSize) {
        _numberOfColumns = shelfSize.X;
        _numberOfRows = shelfSize.Y;
    }
}