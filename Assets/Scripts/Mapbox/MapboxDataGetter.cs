using System;
using System.Collections;
using System.Collections.Generic;

public class MapboxDataGetter {
	private static string accessToken = "pk.eyJ1Ijoib2RkaXR5b3ZlcnNlZXIxMyIsImEiOiJjaXV0NDJ2ajEwMG14MnlzNTZqOGFrbnFvIn0.-nReyEBZ_MDp_k-HZvjdgg";
	private static int zoomLevel = 18;

//	public static void GetOSMData (double n, double s, double e, double w) {
//		List<int> nwTileCoords = WorldToTilePos (n, w, zoomLevel);
//		List<int> seTileCoords = WorldToTilePos (s, e, zoomLevel);
//
//		for (int x = nwTileCoords[0]; x <= nwTileCoords[0]; x++) {
//			for (int y = seTileCoords[1]; y <= seTileCoords[1]; y++) {
//				string vectorTileURL = MapboxVectorTileURL (x, y, zoomLevel);
//
//			}
//		}
//	}

//	private static void MapboxTerrainTileURL(int x, int y, int z) {
//		return "https://api.mapbox.com/v4/mapbox.terrain-rgb/" + z + "/" + x + "/" + y + ".pngraw?access_token=" + accessToken;
//	}

	private static string MapboxVectorTileURL(int x, int y, int z) {
		return "http://a.tiles.mapbox.com/v4/mapbox.mapbox-streets-v7/" + z + "/" + x + "/" + y + ".mvt?access_token=" + accessToken;
	}


//	private static string GetMapboxOSMTileURL(int x, int y, int z) {
//		string mapboxTerrainDataURL = "https://api.mapbox.com/v4/mapbox.terrain-rgb/" + z + "/" + x + "/" + y + 
//			".pngraw?access_token=" + accessToken;
//	}

	private static List<int> WorldToTilePos(double lon, double lat, int zoom)
	{
		float x = (float)((lon + 180.0) / 360.0 * (1 << zoom));
		float y = (float)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) + 
			1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));

		int tileX = (int)Math.Round (x);
		int tileY = (int)Math.Round (y);

		return new List<int> () { tileX, tileY };
	}

	private static List<double> TileToWorldPos(double tile_x, double tile_y, int zoom) 
	{
		double n = Math.PI - ((2.0 * Math.PI * tile_y) / Math.Pow(2.0, zoom));

		float lat = (float)((tile_x / Math.Pow(2.0, zoom) * 360.0) - 180.0);
		float lon = (float)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));

		return new List<double>() {lat, lon};;
	}
}

