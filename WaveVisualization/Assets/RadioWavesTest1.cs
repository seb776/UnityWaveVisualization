using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorConvert
{
    public static Vector3 FromInt(Vector3Int v)
    {
        return new Vector3((float)v.x, (float)v.y, (float)v.z);
    }
}

public class EmittersData
{
    public Vector3 Position;
}

public class RadioWavesTest1 : MonoBehaviour
{
    public Vector3Int GridSize;
    public List<GameObject> Emitters;
    public int CubeCountPerUnityUnit;
    public Material CubeMaterial;
    public float MainFrequency;
    public float MaxOpacity;

    public float RefreshDelay;

    private GameObject[,,] _cubes;
    private float[,,] _fieldValues;


    void Start()
    {
        StartCoroutine("_radioWavesCorout");
    }

    private void _generateGrid()
    {
        _cubes = new GameObject[GridSize.x * CubeCountPerUnityUnit, GridSize.y * CubeCountPerUnityUnit, GridSize.z * CubeCountPerUnityUnit];
        _fieldValues = new float[GridSize.x * CubeCountPerUnityUnit, GridSize.y * CubeCountPerUnityUnit, GridSize.z * CubeCountPerUnityUnit];
        for (int i = 0; i < _cubes.GetLength(0); ++i)
        {
            for (int j = 0; j < _cubes.GetLength(1); ++j)
            {
                for (int k = 0; k < _cubes.GetLength(2); ++k)
                {
                    var cucube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    Vector3 newPos = (Vector3.one / (float)CubeCountPerUnityUnit);

                    newPos.Scale(new Vector3((float)i, (float)j, (float)k));

                    cucube.transform.position = newPos;
                    cucube.transform.localScale = Vector3.one / (float)CubeCountPerUnityUnit;


                    cucube.GetComponent<MeshRenderer>().material = CubeMaterial;
                    _cubes[i, j, k] = cucube;
                }
            }
        }

    }

    IEnumerator _radioWavesCorout()
    {
        _generateGrid();
        while (true)
        {
            for (int i = 0; i < _cubes.GetLength(0); ++i)
            {
                for (int j = 0; j < _cubes.GetLength(1); ++j)
                {
                    for (int k = 0; k < _cubes.GetLength(2); ++k)
                    {
                        var cubePos = _cubes[i, j, k].transform.position;
                        var cubeMat = _cubes[i, j, k].GetComponent<MeshRenderer>().material;
                        _fieldValues[i, j, k] = 0.0f;
                        for (int l = 0; l < Emitters.Count; ++l)
                        {
                            _fieldValues[i, j, k] += Mathf.Sin((cubePos - Emitters[l].transform.position).magnitude * MainFrequency + Time.realtimeSinceStartup);

                        }
                        var fieldVal = _fieldValues[i, j, k];
                        Vector3 col = new Vector3(1.0f, 0.0f, 0.0f); // Neg
                                                                     //if (MaxOpacity)
                        if (fieldVal < 0.0f)
                            col = new Vector3(0.0f, 0.0f, 1.0f);

                        cubeMat.color = new Color(col.x, col.y, col.z,  MaxOpacity*Mathf.Clamp01(Mathf.Abs(fieldVal)));
                    }
                }
            }

            yield return new WaitForSeconds(RefreshDelay);
        }
    }

    void Update()
    {

    }
}
