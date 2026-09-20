import { create } from 'zustand';

const MIN_FOV = 0.5;
const MAX_FOV = 100;
const MIN_ALTITUDE = 0;
const MAX_ALTITUDE = 90;

interface CameraState {
  azimuthDeg: number;
  altitudeDeg: number;
  fovDeg: number;
  setAzAlt: (azimuthDeg: number, altitudeDeg: number) => void;
  zoomBy: (factor: number) => void;
}

function clamp(value: number, min: number, max: number): number {
  return Math.min(max, Math.max(min, value));
}

function wrapDegrees(value: number): number {
  return ((value % 360) + 360) % 360;
}

export const useCameraStore = create<CameraState>((set) => ({
  azimuthDeg: 0,
  altitudeDeg: 30,
  fovDeg: 75,

  setAzAlt: (azimuthDeg, altitudeDeg) =>
    set({
      azimuthDeg: wrapDegrees(azimuthDeg),
      altitudeDeg: clamp(altitudeDeg, MIN_ALTITUDE, MAX_ALTITUDE),
    }),

  zoomBy: (factor) =>
    set((state) => ({
      fovDeg: clamp(state.fovDeg * factor, MIN_FOV, MAX_FOV),
    })),
}));
