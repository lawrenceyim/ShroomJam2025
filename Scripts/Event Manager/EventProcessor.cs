using System;

namespace EventSystem {
    public abstract class EventProcessor {
        public event Action<GameEvent> Broadcast;
        public abstract void Process(GameEvent gameEvent);

        protected void EmitEvent(GameEvent gameEvent) {
            Broadcast?.Invoke(gameEvent);
        }
    }
}