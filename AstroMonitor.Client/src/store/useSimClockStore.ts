import { create } from 'zustand';

interface SimClockState {
  simulatedTime: Date;
  isPlaying: boolean;
  timeScale: number; // 1 = real-time
  setSimulatedTime: (time: Date) => void;
  setTimeScale: (scale: number) => void;
  play: () => void;
  pause: () => void;
  togglePlaying: () => void;
  resetToNow: () => void;
}

export const useSimClockStore = create<SimClockState>((set) => ({
  simulatedTime: new Date(),
  isPlaying: false,
  timeScale: 1,

  setSimulatedTime: (simulatedTime) => set({ simulatedTime }),
  setTimeScale: (timeScale) => set({ timeScale }),
  play: () => set({ isPlaying: true }),
  pause: () => set({ isPlaying: false }),
  togglePlaying: () => set((state) => ({ isPlaying: !state.isPlaying })),
  resetToNow: () => set({ simulatedTime: new Date() }),
}));
