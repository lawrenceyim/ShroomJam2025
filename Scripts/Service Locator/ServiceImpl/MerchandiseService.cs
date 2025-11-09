using Godot;
using ServiceSystem;

public class MerchandiseService : IService {
    private MerchandiseRepository _merchandiseRepository;

    public MerchandiseService(MerchandiseRepository merchandiseRepository) {
        _merchandiseRepository = merchandiseRepository;
    }

    public Vector2I GetShelfSize() {
        return _merchandiseRepository.GetShelfSize();
    }

    public Merchandise GetDvdFromShelf(Vector2I position) {
        return _merchandiseRepository.GetDvd(position);
    }

    public Merchandise GetHeldDvd() {
        return _merchandiseRepository.GetHeldDvd();
    }

    public void SetHeldDvd(Merchandise merchandise) {
        _merchandiseRepository.SetHeldDvd(merchandise);
    }

    public void SetShelfDvd(Merchandise merchandise, Vector2I position) {
        _merchandiseRepository.AddDvd(merchandise, position);
    }
}