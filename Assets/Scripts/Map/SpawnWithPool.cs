using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace PoolSpawner {
    public class SpawnWithPool {
        readonly bool collectionChecks = true;
        readonly int maxPoolSize = 10;
        int availableElements = 0;

        Dictionary<int, ObjectPool<GameObject>> poolObjList = new();
        public void AddPoolForGameObject(GameObject _toSpawn, int _id) {
            ObjectPool<GameObject> pool = new(() => {
                var obj = GameObject.Instantiate(_toSpawn, Vector3.zero, Quaternion.identity);
                return obj;
            },
            OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);

            poolObjList.Add(_id, pool);
            availableElements++;
        }

        public void ClearPool() {
            poolObjList.Clear();
        }

        public void Spawn(int _id) {
            poolObjList[_id].Get();
        }
        public GameObject GetSpawnObject(int _id) {
            return poolObjList[_id].Get();
        }

        public (int, GameObject) GetRandomSpawnObject() {
            int ID = Random.Range(0, availableElements);
            return (ID,poolObjList[ID].Get());
        }

        public void ThisObjReleased(GameObject _obj, int _id) {
            poolObjList[_id].Release(_obj);
        }

        #region poolOperations
        void OnReturnedToPool(GameObject system) {
            system.gameObject.SetActive(false);
        }

        void OnTakeFromPool(GameObject system) {
            system.gameObject.SetActive(true);
        }

        // If the pool capacity is reached then any items returned will be destroyed.
        void OnDestroyPoolObject(GameObject system) {
            GameObject.Destroy(system.gameObject);
        }
        #endregion  
    }
}