using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SpawnerAssetsOnTerrain : MonoBehaviour
{
	[Header("Area Options")]
	public Vector3 _areaSize = new Vector3(10, 10, 10);
	public Color _areaDebugColor = Color.green;
	[Header("Assets Options")]
	public List<GameObject> _prefabsList;
	public int _spawnCount;
	List<GameObject> _spawnList;
	[Header("Terrain Options")]
	public Terrain _terrain;
	public float _groundOffsetY = 0;
	TerrainData _terrainData;


	private void Awake()
	{
		_spawnList = new List<GameObject>();
	}

	private void OnEnable()
	{
		_terrainData = _terrain.terrainData;	
	}

	private void OnValidate()
	{
		_terrainData = _terrain.terrainData;
	}

	[Button("Spawn")]
	void SpawnPrefabsOnTerrain()
	{
		for (int i = 0; i < _spawnCount; i++)
		{
			_spawnList.Add(Instantiate(_prefabsList[Random.Range(0, _prefabsList.Count)], RandomPosition(), Quaternion.identity, transform));
		}
	}

	[Button("Clear")]
	void ClearSpawnAssets()
	{
		for (int i = 0; i < _spawnList.Count; i++)
		{
			DestroyImmediate(_spawnList[i]);
		}
		_spawnList.Clear();
	}

	Vector3 RandomPosition()
	{
		float xRandomPos = Random.Range(transform.position.x, transform.position.x + _areaSize.x);
		float zRandomPos = Random.Range(transform.position.z, transform.position.z + _areaSize.z);
		float yPos = _terrainData.GetHeight((int)xRandomPos, (int)zRandomPos) + (_prefabsList[0].transform.lossyScale.y) - _groundOffsetY;

		return new Vector3(xRandomPos, yPos , zRandomPos);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = _areaDebugColor;
		Gizmos.DrawWireCube(transform.position + (_areaSize / 2), _areaSize);
	}
}
