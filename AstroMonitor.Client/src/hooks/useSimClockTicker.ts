import { useEffect } from 'react';
import { useSimClockStore } from '../store/useSimClockStore';

/**
 * Advances the simulated clock using requestAnimationFrame deltas so it stays
 * decoupled from wall-clock time and in step with the render loop. Mount once
 * near the app root.
 */
export function useSimClockTicker() {
  useEffect(() => {
    let rafId = 0;
    let lastTimestamp = performance.now();

    const tick = (timestamp: number) => {
      const deltaMs = timestamp - lastTimestamp;
      lastTimestamp = timestamp;

      const { isPlaying, timeScale, simulatedTime, setSimulatedTime } = useSimClockStore.getState();
      if (isPlaying) {
        setSimulatedTime(new Date(simulatedTime.getTime() + deltaMs * timeScale));
      }

      rafId = requestAnimationFrame(tick);
    };

    rafId = requestAnimationFrame(tick);
    return () => cancelAnimationFrame(rafId);
  }, []);
}
