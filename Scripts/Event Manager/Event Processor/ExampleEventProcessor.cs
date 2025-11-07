using Godot;

namespace EventSystem {
    public class ExampleEventProcessor : EventProcessor {
        public override void Process(GameEvent gameEvent) {
            ExampleGameEvent exampleGameEvent = (ExampleGameEvent)gameEvent;
            GD.Print(exampleGameEvent.Message);
        }
    }
}