import { useState, useMemo } from 'react';
import { useAstroStore } from '../store/useAstroStore';
import { useCameraStore } from '../store/useCameraStore';
import type { SkyMapItemDto } from '../types/skyMap';

export default function SearchPanel() {
    const [query, setQuery] = useState('');
    const skyObjects = useAstroStore((state) => state.skyObjects);
    const selectObject = useAstroStore((state) => state.selectObject);
    const setAzAlt = useCameraStore((state) => state.setAzAlt);

    const filteredObjects = useMemo(() => {
        if (!query.trim()) return [];
        const lower = query.toLowerCase();
        return skyObjects
            .filter((obj) => obj.name.toLowerCase().includes(lower))
            .slice(0, 5);
    }, [query, skyObjects]);

    const handleSelect = (obj: SkyMapItemDto) => {
        selectObject(obj);
        setAzAlt(obj.azimuth, obj.altitude);
        setQuery('');
    };

    return (
        <div className="absolute left-4 top-16 z-20 w-64">
            <input
                type="text"
                placeholder="Пошук об'єктів (напр. Ur)..."
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                className="w-full rounded-lg border border-white/20 bg-black/70 px-3 py-2 text-sm text-white backdrop-blur-md focus:border-sky-400 focus:outline-none"
            />
            {filteredObjects.length > 0 && (
                <ul className="mt-1 overflow-hidden rounded-lg border border-white/10 bg-black/80 backdrop-blur-md">
                    {filteredObjects.map((obj) => (
                        <li
                            key={obj.id}
                            onClick={() => handleSelect(obj)}
                            className="flex cursor-pointer items-center justify-between px-3 py-2 text-sm text-white/85 transition hover:bg-white/20 hover:text-white"
                        >
                            <span>{obj.name}</span>
                            <span className="text-xs text-white/40">{obj.category}</span>
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
}