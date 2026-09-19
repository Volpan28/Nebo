export interface SkyMapItemDto {
  id: string;
  name: string;
  category: 'Planet' | 'Moon' | 'Deep Sky Object' | 'Star' | string;
  altitude: number;
  azimuth: number;
  magnitude: number;
  textureUrl: string;
}
