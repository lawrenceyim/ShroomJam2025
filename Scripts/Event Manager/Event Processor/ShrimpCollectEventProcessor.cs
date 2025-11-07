using EventSystem;

public class ShrimpCollectEventProcessor : EventProcessor {
	public override void Process(GameEvent gameEvent) {
		CollectShrimpEvent shrimpCollectEvent = (CollectShrimpEvent)gameEvent;
		// PersistentData.IncreaseFreshShrimps(1);
		EmitEvent(shrimpCollectEvent);
	}
}
