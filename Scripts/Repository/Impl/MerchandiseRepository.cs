using System.Collections.Generic;
using Godot;
using RepositorySystem;

public class MerchandiseRepository : IRepository {
    private int _numberOfRows = 2;
    private int _numberOfColumns = 6;

    private Merchandise _heldMerchandise;
    private Dictionary<Vector2I, Merchandise> _shelf = new Dictionary<Vector2I, Merchandise>();

    public void SetMerchandise(Merchandise merchandise, Vector2I position) {
        _shelf[position] = merchandise;
    }

    public Merchandise GetMerchandise(Vector2I position) {
        return _shelf.GetValueOrDefault(position, null);
    }

    public void SetHeldMerchandise(Merchandise heldMerchandise) {
        _heldMerchandise = heldMerchandise;
    }

    public Merchandise GetHeldMerchandise() {
        return _heldMerchandise;
    }

    public Vector2I GetShelfSize() {
        return new Vector2I(_numberOfColumns, _numberOfRows);
    }

    public void SetShelfSize(Vector2I shelfSize) {
        _numberOfColumns = shelfSize.X;
        _numberOfRows = shelfSize.Y;
    }
}