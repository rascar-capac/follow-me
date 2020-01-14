using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsSpawnerOnTerrain : MonoBehaviour
{
	[Header("Box Options")]
	public Vector3 _boxSize = new Vector3(10, 10, 10);
	public Color _boxColor = Color.green;
	[Header("Assets Options")]
	public List<GameObject> _prefabsList;
	[Header("Terrain Options")]
	public Terrain _terrain;
	TerrainData _terrainData;

	float[,] _terrainHeights;

	private void Awake()
	{
		_terrainData = _terrain.terrainData;
		_terrainHeights = _terrainData.GetHeights(0, 0, (int)_terrainData.size.x, (int)_terrainData.size.y);
	}

	void SpawnPrefabsOnTerrain()
	{
		Instantiate(_prefabsList[Random.Range(0, _prefabsList.Count)]);
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = _boxColor;
		Gizmos.DrawWireCube(transform.position, _boxSize);
	}
}
