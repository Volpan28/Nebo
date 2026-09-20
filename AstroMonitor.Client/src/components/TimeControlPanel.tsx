import { addHours, format } from 'date-fns';
import { Pause, Play, RotateCcw } from 'lucide-react';
import { useRef, useState } from 'react';
import { useSimClockStore } from '../store/useSimClockStore';

export default function TimeControlPanel() {
  const simulatedTime = useSimClockStore((state) => state.simulatedTime);
  const setSimulatedTime = useSimClockStore((state) => state.setSimulatedTime);
  const isPlaying = useSimClockStore((state) => state.isPlaying);
  const togglePlaying = useSimClockStore((state) => state.togglePlaying);
  const resetToNow = useSimClockStore((state) => state.resetToNow);

  const [offsetHours, setOffsetHours] = useState(0);
  const sliderBaseRef = useRef(simulatedTime);

  const handleDateChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const [year, month, day] = event.target.value.split('-').map(Number);
    if (!year || !month || !day) return;

    const next = new Date(simulatedTime);
    next.setFullYear(year, month - 1, day);

    sliderBaseRef.current = next;
    setOffsetHours(0);
    setSimulatedTime(next);
  };

  const handleTimeChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const [hours, minutes, seconds] = event.target.value.split(':').map(Number);
    if (hours === undefined || minutes === undefined) return;

    const next = new Date(simulatedTime);
    next.setHours(hours, minutes, seconds ?? 0, 0);

    sliderBaseRef.current = next;
    setOffsetHours(0);
    setSimulatedTime(next);
  };

  const handleOffsetChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const hours = Number(event.target.value);
    setOffsetHours(hours);
    setSimulatedTime(addHours(sliderBaseRef.current, hours));
  };

  const handleReset = () => {
    const now = new Date();
    sliderBaseRef.current = now;
    setOffsetHours(0);
    resetToNow();
  };

  return (
    <div className="absolute bottom-6 left-1/2 z-10 flex -translate-x-1/2 flex-col items-center gap-3 rounded-2xl border border-white/10 bg-black/70 px-6 py-4 text-white backdrop-blur-md">
      <div className="flex flex-wrap items-center justify-center gap-3">
        <button
          type="button"
          onClick={togglePlaying}
          className="flex items-center gap-1 rounded-md border border-white/20 px-2 py-1 text-xs text-white/80 transition hover:bg-white/10"
        >
          {isPlaying ? <Pause size={12} /> : <Play size={12} />}
        </button>
        <input
          type="date"
          value={format(simulatedTime, 'yyyy-MM-dd')}
          onChange={handleDateChange}
          className="rounded-md border border-white/20 bg-black/40 px-2 py-1 text-sm text-white [color-scheme:dark]"
        />
        <input
          type="time"
          step="1"
          value={format(simulatedTime, 'HH:mm:ss')}
          onChange={handleTimeChange}
          className="rounded-md border border-white/20 bg-black/40 px-2 py-1 text-sm text-white [color-scheme:dark]"
        />
        <button
          type="button"
          onClick={handleReset}
          className="flex items-center gap-1 rounded-md border border-white/20 px-2 py-1 text-xs text-white/80 transition hover:bg-white/10"
        >
          <RotateCcw size={12} />
          Now
        </button>
      </div>

      <div className="flex w-full max-w-md items-center gap-3">
        <span className="w-10 text-right text-xs text-white/60">-12h</span>
        <input
          type="range"
          min={-12}
          max={12}
          step={0.25}
          value={offsetHours}
          onChange={handleOffsetChange}
          className="h-1 flex-1 cursor-pointer appearance-none rounded-full bg-white/20 accent-sky-400"
        />
        <span className="w-10 text-xs text-white/60">+12h</span>
      </div>
    </div>
  );
}
