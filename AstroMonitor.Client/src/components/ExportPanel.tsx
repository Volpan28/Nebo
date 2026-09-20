import { useState } from 'react';
import { format } from 'date-fns';
import { Download, Loader2, FileText, Table } from 'lucide-react';
import { useReportStore } from '../store/useReportStore';
import { useAstroStore } from '../store/useAstroStore';
import { useSimClockStore } from '../store/useSimClockStore';

export default function ExportPanel() {
    const simulatedTime = useSimClockStore((state) => state.simulatedTime);
    const location = useAstroStore((state) => state.location);
    const { isExporting, message, exportReport } = useReportStore();

    const [selectedDate, setSelectedDate] = useState(format(simulatedTime, 'yyyy-MM-dd'));

    const handleExport = (type: 'pdf' | 'csv') => {
        const date = new Date(selectedDate);
        exportReport(type, date, location.latitude, location.longitude);
    };

    return (
        <>
            <div className="absolute right-4 top-4 z-10 w-64 rounded-lg bg-black/70 p-4 text-white backdrop-blur-md">
                <h2 className="mb-4 flex items-center gap-2 text-lg font-semibold">
                    <Download size={18} />
                    Експорт звіту
                </h2>

                <div className="flex flex-col gap-3">
                    <label className="text-xs text-white/60">Дата спостереження:</label>
                    <input
                        type="date"
                        value={selectedDate}
                        onChange={(e) => setSelectedDate(e.target.value)}
                        disabled={isExporting}
                        className="rounded-md border border-white/20 bg-black/40 px-3 py-2 text-sm text-white focus:border-sky-400 focus:outline-none disabled:opacity-50 [color-scheme:dark]"
                    />

                    <div className="flex gap-2 mt-2">
                        <button
                            onClick={() => handleExport('csv')}
                            disabled={isExporting}
                            className="flex flex-1 items-center justify-center gap-1 rounded bg-emerald-600/80 py-2 text-sm font-medium transition hover:bg-emerald-500 disabled:opacity-50"
                        >
                            <Table size={14} /> CSV
                        </button>
                        <button
                            onClick={() => handleExport('pdf')}
                            disabled={isExporting}
                            className="flex flex-1 items-center justify-center gap-1 rounded bg-rose-600/80 py-2 text-sm font-medium transition hover:bg-rose-500 disabled:opacity-50"
                        >
                            <FileText size={14} /> PDF
                        </button>
                    </div>
                </div>
            </div>

            {message && (
                <div className="absolute bottom-6 right-4 z-50 flex max-w-sm items-center gap-3 rounded-lg border border-white/10 bg-sky-900/90 px-4 py-3 text-sm text-white shadow-lg backdrop-blur-md">
                    {isExporting && <Loader2 className="animate-spin text-sky-300" size={20} />}
                    <p>{message}</p>
                </div>
            )}
        </>
    );
}