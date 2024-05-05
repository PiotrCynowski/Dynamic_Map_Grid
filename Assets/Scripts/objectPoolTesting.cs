using GameMap.Generator;
using UnityEngine;

public class objectPoolTesting : MonoBehaviour
{
    public int ID;
    int spawned;

    public void Test() {

        GameObject newElement;
        newElement = MapDataManager.Instance.poolMapElements.GetSpawnObject(ID);
        newElement.transform.position = transform.position + (spawned * Vector3.left);
        spawned++;
    }


}
