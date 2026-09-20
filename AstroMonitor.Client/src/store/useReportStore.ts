import { create } from 'zustand';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5162';

interface ReportState {
    isExporting: boolean;
    message: string | null;
    exportReport: (type: 'pdf' | 'csv', date: Date, lat: number, lon: number) => Promise<void>;
}

export const useReportStore = create<ReportState>((set) => ({
    isExporting: false,
    message: null,

    exportReport: async (type, date, lat, lon) => {
        set({
            isExporting: true,
            message: `Генерація ${type.toUpperCase()} звіту. Це може зайняти декілька хв, будь ласка, зачекайте...`
        });

        try {
            const params = new URLSearchParams({
                latitude: String(lat),
                longitude: String(lon),
                observationDate: date.toISOString(),
            });

            const url = `${API_BASE_URL}/api/reports/${type}?${params.toString()}`;

            const response = await fetch(url);

            if (!response.ok) {
                throw new Error(`Помилка сервера: ${response.status}`);
            }

            const blob = await response.blob();
            const downloadUrl = window.URL.createObjectURL(blob);

            const a = document.createElement('a');
            a.href = downloadUrl;
            a.download = `AstroMonitor_Report_${date.toISOString().split('T')[0]}.${type}`;
            document.body.appendChild(a);
            a.click();
            a.remove();

            window.URL.revokeObjectURL(downloadUrl);

            set({ message: `Звіт ${type.toUpperCase()} успішно завантажено!` });
        } catch (error) {
            console.error('[useReportStore]', error);
            set({ message: `Помилка експорту ${type.toUpperCase()}. Спробуйте ще раз.` });
        } finally {
            set({ isExporting: false });
            setTimeout(() => set({ message: null }), 5000);
        }
    }
}));