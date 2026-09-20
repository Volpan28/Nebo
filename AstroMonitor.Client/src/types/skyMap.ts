export interface SkyMapItemDto {
  id: string;
  name: string;
  category: 'Planet' | 'Moon' | 'Deep Sky Object' | 'Star' | 'Constellation' | string;
  rightAscension: number;
  declination: number;
  altitude: number;
  azimuth: number;
  magnitude: number;
  textureUrl: string;
}