import { create } from 'zustand';
import type { SkyMapItemDto } from '../types/skyMap';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5162';
const SKYMAP_API_URL = `${API_BASE_URL}/api/skymap`;

export interface Location {
  latitude: number;
  longitude: number;
}

interface AstroState {
  time: Date;
  location: Location;
  skyObjects: SkyMapItemDto[];
  selectedObject: SkyMapItemDto | null;
  isLoading: boolean;
  error: string | null;
  setTime: (time: Date) => void;
  setLocation: (location: Location) => void;
  selectObject: (object: SkyMapItemDto | null) => void;
  fetchSkyMap: () => Promise<void>;
}

export const useAstroStore = create<AstroState>((set, get) => ({
  time: new Date(),
  location: { latitude: 49.83, longitude: 24.02 },
  skyObjects: [],
  selectedObject: null,
  isLoading: false,
  error: null,

  setTime: (time) => set({ time }),
  setLocation: (location) => set({ location }),
  selectObject: (object) => set({ selectedObject: object }),

  fetchSkyMap: async () => {
    const { time, location } = get();
    set({ isLoading: true, error: null });

    const params = new URLSearchParams({
      latitude: String(location.latitude),
      longitude: String(location.longitude),
      observationDate: time.toISOString(),
    });

    try {
      const response = await fetch(`${SKYMAP_API_URL}?${params.toString()}`);
      if (!response.ok) {
        throw new Error(`Sky map request failed with status ${response.status}`);
      }
      const data: SkyMapItemDto[] = await response.json();
      set({ skyObjects: data, isLoading: false });
    } catch (err) {
      console.error('[useAstroStore] fetchSkyMap failed:', err);
      set({
        error: err instanceof Error ? err.message : 'Failed to fetch sky map',
        isLoading: false,
      });
    }
  },
}));
