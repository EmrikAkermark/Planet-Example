using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SmoothFill : MonoBehaviour, IPointerClickHandler
{
    public int width;
    public int height;
    public int depth;
    public GameObject wallPrefab;
    public float scale = 1f;
    public int smoothLevel = 5;
    public float density = .5f;

    public int[,,] map;
    public int[,,] smoothMap;
    List<GameObject> walls = new List<GameObject>();

    private int layerCounter = 0;

    void Start()
    {
        Generate();
    }

    public void Generate() 
    {
        Debug.Log("Started Generating");
        Reset();
        FillRandom();
        Smooth(smoothLevel, 0);
        AddLayer(depth);
        Draw();
        Debug.Log("End");

    }

    void FillRandom()
    {

        /*
         * loop through width and height
         * based on a random value, plot map
        */

        for (int x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                if (Random.value < density)
                {
                    map[x, y, 0] = 1;
                }
            }
        }
    }

    void Smooth(int level = 1, int currentDepth = 0)
    {
        /*
         * loop through width and height
         * check if there is a wall
         * thicken the wall with gradient
         * cut off gradient
        */

        smoothMap = new int[width, height, depth + 1];

        for (int x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                    if (map[x, y, currentDepth] == 1)
                    {
                        //if edge, don't add gradient
                        if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                        {
                        smoothMap[x, y, currentDepth] += 1;
                            continue;
                        }

                        //add gradient values
                        smoothMap[x - 1, y, currentDepth] += 1;
                        smoothMap[x + 1, y, currentDepth] += 1;
                        smoothMap[x, y + 1, currentDepth] += 1;
                        smoothMap[x, y - 1, currentDepth] += 1;
                        smoothMap[x, y, currentDepth] += 1;
                    }
            }
        }

        //cut off gradient
        for (int x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                map[x, y, currentDepth] = smoothMap[x, y, currentDepth] < 2 ? 1 : 0;
            }
        }

        //recursion, calls itself until done
        level--;
        if (level > 0)
            Smooth(level, currentDepth);
    }

    void AddLayer(int layers)
    {
        /*
         * Go through layer laid out
         * Check if surrounded
         * If so, randomly add wall       
         */
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y, layerCounter] == 1)
                {
                    if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                    {
                        continue;
                    }
                     else if (map[x +1, y, layerCounter] == 1 && map[x - 1, y, layerCounter] == 1 && map[x, y - 1, layerCounter] == 1 && map[x, y + 1, layerCounter] == 1)
                    {
                        if(Random.value > density)
                        {
                            map[x, y, layerCounter + 1] = 1;

                        }
                    }
                }
            }
        }

        //Smooths the new layer and and calls itself to lay new layer
        ++layerCounter;
        if(layerCounter < layers)
        {
            Smooth(smoothLevel, layerCounter);
            AddLayer(layers);
        }
    }

    void Draw()
    {
        /*
         * loop through width and height and depth
         * check if there is a wall
         * instantiate walls accordingly
        */

        for (int x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (map[x, y, z] == 1 && map[x, y, z + 1] != 1 )
                    {
                        GameObject wall = Instantiate(wallPrefab, gameObject.transform, true);
                        Vector3 pos = new Vector3(x * scale - width * .5f * scale + scale, z * scale + scale * .5f * scale + scale, y * scale - height * .5f * scale + scale);
                        wall.transform.SetPositionAndRotation(pos, Quaternion.identity);
                        wall.transform.localScale = new Vector3(scale, scale, scale);
                        walls.Add(wall);
                    }
                }
            }
        }
    }

    void Reset()
    {
        map = new int[width, height, depth + 1];
            
        foreach (var wall in walls)
        {
            Destroy(wall);
        }
        walls.Clear();
        layerCounter = 0;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Debug.Log("Found Click");
        Generate();
    }
}
