using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

public class AssetsSpawnerOnTerrain : MonoBehaviour
{
	[Header("Box Options")]
	public Vector3 _boxSize = new Vector3(10, 10, 10);
	public Color _boxColor = Color.green;
	[Header("Assets Options")]
	public List<GameObject> _prefabsList;
	public int _spawnCount;
	[Header("Terrain Options")]
	public Terrain _terrain;
	[Range(0, 1)] public float _groundOffset = 0;
	TerrainData _terrainData;


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
			Instantiate(_prefabsList[Random.Range(0, _prefabsList.Count)], RandomPosition(), Quaternion.identity, transform);
		}
	}

	Vector3 RandomPosition()
	{
		float xRandomPos = Random.Range(transform.position.x, transform.position.x + _boxSize.x);
		float zRandomPos = Random.Range(transform.position.z, transform.position.z + _boxSize.z);
		float yPos = _terrainData.GetHeight((int)xRandomPos, (int)zRandomPos) + (_prefabsList[0].transform.lossyScale.y);

		return new Vector3(xRandomPos, yPos , zRandomPos);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = _boxColor;
		Gizmos.DrawWireCube(transform.position + (_boxSize / 2), _boxSize);
	}
}
