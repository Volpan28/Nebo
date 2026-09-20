export interface StarCatalogItemDto {
  id: string;
  properName: string | null;
  rightAscension: number; // hours
  declination: number; // degrees
  distance: number;
  magnitude: number;
  colorIndex: number | null;
}

export interface StarCatalogEntry {
  id: string;
  name: string | null;
  magnitude: number;
  colorIndex: number | null;
  eqVector: [number, number, number];
}
