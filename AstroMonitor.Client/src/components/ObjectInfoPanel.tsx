import { X } from 'lucide-react';
import { useAstroStore } from '../store/useAstroStore';

export default function ObjectInfoPanel() {
  const selectedObject = useAstroStore((state) => state.selectedObject);
  const selectObject = useAstroStore((state) => state.selectObject);

  if (!selectedObject) {
    return null;
  }

  return (
      <div className="absolute left-4 top-32 z-10 w-64 rounded-lg bg-black/70 p-4 text-white backdrop-blur-md">
        <div className="flex items-start justify-between gap-2">
          <h2 className="text-lg font-semibold">{selectedObject.name}</h2>
          <button
              type="button"
              onClick={() => selectObject(null)} // Це також скине виділення на сцені
              aria-label="Close"
              className="text-white/60 transition hover:text-white"
          >
            <X size={18} />
          </button>
        </div>

        <dl className="mt-3 space-y-1.5 text-sm">
          <div className="flex justify-between">
            <dt className="text-white/50">Category</dt>
            <dd>{selectedObject.category}</dd>
          </div>
          <div className="flex justify-between">
            <dt className="text-white/50">Magnitude</dt>
            <dd>{selectedObject.magnitude.toFixed(2)}</dd>
          </div>
          <div className="flex justify-between">
            <dt className="text-white/50">Altitude</dt>
            <dd>{selectedObject.altitude.toFixed(2)}°</dd>
          </div>
          <div className="flex justify-between">
            <dt className="text-white/50">Azimuth</dt>
            <dd>{selectedObject.azimuth.toFixed(2)}°</dd>
          </div>
        </dl>
      </div>
  );
}