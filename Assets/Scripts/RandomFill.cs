using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RandomFill : MonoBehaviour, IPointerClickHandler
{
    public int width;
    public int height;
    public GameObject unitPrefab;

    List<GameObject> units = new List<GameObject>();

    void Start()
    {
        FillRandom();
    }

    void FillRandom() 
    {
        Clear();

        /*
         * loop through width and height
         * based on a random value, instantiate a new unit
         * place the new unit with x and y
         * add the unit to the units list
        */

        for (int x = 0; x < width; x++) 
        {
            for (var y = 0; y < height; y++) 
            {
                if (Random.value > .5f)
                {
                    GameObject unit = Instantiate(unitPrefab, gameObject.transform, true);
                    Vector3 pos = new Vector3(x - width * .5f + .5f, y - height * .5f + .5f, -.5f);
                    unit.transform.SetPositionAndRotation(pos, Quaternion.identity);
                    units.Add(unit);
                }
            }
        }
    }

    void Clear() 
    {
        foreach (var unit in units) {
            Destroy(unit);
        }
        units.Clear();
    }

    public void OnPointerClick(PointerEventData pointerEventData) 
    {
        FillRandom();
    }

    void Update()
    {
        
    }
}
