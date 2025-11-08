using Godot;
using ServiceSystem;

public class DvdService : IService {
    private DvdRepository _dvdRepository;

    public DvdService(DvdRepository dvdRepository) {
        _dvdRepository = dvdRepository;
    }

    public Vector2I GetShelfSize() {
        return _dvdRepository.GetShelfSize();
    }

    public Dvd GetDvdFromShelf(Vector2I position) {
        return _dvdRepository.GetDvd(position);
    }

    public Dvd GetHeldDvd() {
        return _dvdRepository.GetHeldDvd();
    }

    public void SetHeldDvd(Dvd dvd) {
        _dvdRepository.SetHeldDvd(dvd);
    }

    public void SetShelfDvd(Dvd dvd, Vector2I position) {
        _dvdRepository.AddDvd(dvd, position);
    }
}