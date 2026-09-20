import { create } from 'zustand';
import { equatorialToUnitVector } from '../utils/equatorial';
import type { StarCatalogEntry, StarCatalogItemDto } from '../types/starCatalog';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5162';
const STAR_CATALOG_API_URL = `${API_BASE_URL}/api/stars/Star/catalog`;

interface StarCatalogState {
  stars: StarCatalogEntry[];
  isLoading: boolean;
  error: string | null;
  fetchCatalog: () => Promise<void>;
}

export const useStarCatalogStore = create<StarCatalogState>((set) => ({
  stars: [],
  isLoading: false,
  error: null,

  fetchCatalog: async () => {
    set({ isLoading: true, error: null });

    try {
      const response = await fetch(`${STAR_CATALOG_API_URL}?maxMagnitude=6.5`);
      if (!response.ok) {
        throw new Error(`Star catalog request failed with status ${response.status}`);
      }
      const data: StarCatalogItemDto[] = await response.json();

      // Equatorial unit vector is computed once here, at load time — never recomputed
      // per frame, which is what lets the whole catalog render as a single rotating group.
      const stars: StarCatalogEntry[] = data.map((star) => ({
        id: star.id,
        name: star.properName,
        magnitude: star.magnitude,
        colorIndex: star.colorIndex,
        eqVector: equatorialToUnitVector({ raHours: star.rightAscension, decDeg: star.declination }),
      }));

      set({ stars, isLoading: false });
    } catch (err) {
      console.error('[useStarCatalogStore] fetchCatalog failed:', err);
      set({
        error: err instanceof Error ? err.message : 'Failed to fetch star catalog',
        isLoading: false,
      });
    }
  },
}));
