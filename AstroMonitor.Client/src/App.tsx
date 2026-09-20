import { useEffect } from 'react';
import ObjectInfoPanel from './components/ObjectInfoPanel';
import SearchPanel from './components/SearchPanel';
import ExportPanel from './components/ExportPanel'; 
import SkyCanvas from './components/SkyCanvas';
import TimeControlPanel from './components/TimeControlPanel';
import { useStarCatalogStore } from './store/useStarCatalogStore';
import { useSimClockTicker } from './hooks/useSimClockTicker';

function App() {
    const fetchCatalog = useStarCatalogStore((state) => state.fetchCatalog);

    useEffect(() => {
        fetchCatalog();
    }, [fetchCatalog]);

    useSimClockTicker();

    return (
        <div className="relative h-screen w-screen overflow-hidden text-white">
            <SkyCanvas />

            <SearchPanel />
            <ExportPanel /> 
            <TimeControlPanel />
            <ObjectInfoPanel />
        </div>
    );
}

export default App;