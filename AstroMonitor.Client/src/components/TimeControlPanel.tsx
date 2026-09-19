import { addHours, format } from 'date-fns';
import { RotateCcw } from 'lucide-react';
import { useRef, useState } from 'react';
import { useAstroStore } from '../store/useAstroStore';

export default function TimeControlPanel() {
  const time = useAstroStore((state) => state.time);
  const setTime = useAstroStore((state) => state.setTime);
  const fetchSkyMap = useAstroStore((state) => state.fetchSkyMap);
  const isLoading = useAstroStore((state) => state.isLoading);

  const [offsetHours, setOffsetHours] = useState(0);
  const sliderBaseRef = useRef(time);
  const sliderDebounceRef = useRef<number | null>(null);

  const applyTime = (nextTime: Date) => {
    setTime(nextTime);
    fetchSkyMap();
  };

  const handleDateChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const [year, month, day] = event.target.value.split('-').map(Number);
    if (!year || !month || !day) return;

    const next = new Date(time);
    next.setFullYear(year, month - 1, day);

    sliderBaseRef.current = next;
    setOffsetHours(0);
    applyTime(next);
  };

  const handleTimeChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const [hours, minutes, seconds] = event.target.value.split(':').map(Number);
    if (hours === undefined || minutes === undefined) return;

    const next = new Date(time);
    next.setHours(hours, minutes, seconds ?? 0, 0);

    sliderBaseRef.current = next;
    setOffsetHours(0);
    applyTime(next);
  };

  const handleOffsetChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const hours = Number(event.target.value);
    setOffsetHours(hours);

    const next = addHours(sliderBaseRef.current, hours);
    setTime(next);

    if (sliderDebounceRef.current !== null) {
      window.clearTimeout(sliderDebounceRef.current);
    }
    sliderDebounceRef.current = window.setTimeout(() => {
      fetchSkyMap();
    }, 150);
  };

  const handleReset = () => {
    const now = new Date();
    sliderBaseRef.current = now;
    setOffsetHours(0);
    applyTime(now);
  };

  return (
    <div className="absolute bottom-6 left-1/2 z-10 flex -translate-x-1/2 flex-col items-center gap-3 rounded-2xl border border-white/10 bg-black/70 px-6 py-4 text-white backdrop-blur-md">
      <div className="flex flex-wrap items-center justify-center gap-3">
        <input
          type="date"
          value={format(time, 'yyyy-MM-dd')}
          onChange={handleDateChange}
          className="rounded-md border border-white/20 bg-black/40 px-2 py-1 text-sm text-white [color-scheme:dark]"
        />
        <input
          type="time"
          step="1"
          value={format(time, 'HH:mm:ss')}
          onChange={handleTimeChange}
          className="rounded-md border border-white/20 bg-black/40 px-2 py-1 text-sm text-white [color-scheme:dark]"
        />
        {isLoading && <span className="text-xs text-white/50">Loading...</span>}
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
