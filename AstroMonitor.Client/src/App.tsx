import { useEffect } from 'react';
import ObjectInfoPanel from './components/ObjectInfoPanel';
import SkyCanvas from './components/SkyCanvas';
import TimeControlPanel from './components/TimeControlPanel';
import { useAstroStore } from './store/useAstroStore';

function App() {
    const fetchSkyMap = useAstroStore((state) => state.fetchSkyMap);

    useEffect(() => {
        fetchSkyMap();
    }, [fetchSkyMap]);

    return (
        <div className="relative h-screen w-screen overflow-hidden text-white">
            <SkyCanvas />

            <TimeControlPanel />
            <ObjectInfoPanel />
        </div>
    );
}

export default App;