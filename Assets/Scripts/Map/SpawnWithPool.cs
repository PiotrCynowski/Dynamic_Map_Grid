using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace PoolSpawner
{
    public class SpawnWithPool
    {
        readonly private bool collectionChecks = true;
        readonly private int maxPoolSize = 10;
        private int availableElements = 0;

        private Dictionary<int, ObjectPool<GameObject>> poolObjList = new();

        public void AddPoolForGameObject(GameObject _toSpawn, int _id)
        {
            ObjectPool<GameObject> pool = new(() =>
            {
                var obj = GameObject.Instantiate(_toSpawn, Vector3.zero, Quaternion.identity);
                return obj;
            },
            OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);

            poolObjList.Add(_id, pool);
            availableElements++;
        }

        public void ClearPool()
        {
            poolObjList.Clear();
        }

        public void Spawn(int _id)
        {
            poolObjList[_id].Get();
        }
        public GameObject GetSpawnObject(int _id)
        {
            return poolObjList[_id].Get();
        }

        public (int, GameObject) GetRandomSpawnObject()
        {
            int ID = Random.Range(0, availableElements);
            return (ID, poolObjList[ID].Get());
        }

        public void ThisObjReleased(GameObject _obj, int _id)
        {
            poolObjList[_id].Release(_obj);
        }

        private void OnReturnedToPool(GameObject system)
        {
            system.gameObject.SetActive(false);
        }

        private void OnTakeFromPool(GameObject system)
        {
            system.gameObject.SetActive(true);
        }

        private void OnDestroyPoolObject(GameObject system)
        {
            GameObject.Destroy(system.gameObject);
        }
    }
}