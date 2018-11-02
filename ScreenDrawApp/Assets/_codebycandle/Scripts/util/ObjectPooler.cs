using System.Collections.Generic;
using UnityEngine;

namespace Codebycandle.ScreenDrawApp
{
    public class ObjectPooler:MonoBehaviour
    {
        public static ObjectPooler current;
        public GameObject prefab;
        public bool willGrow = true;

        List<GameObject> pooledObjects;

        private int _poolCount;
        public int poolCount
        {
            get
            {
                return _poolCount;
            }
        }

        public void Init(int itemCount = 10,
                            int minSpecificPrefabCount = -1
                            /* Debris.Kind specificPrefabDebrisKind = Debris.Kind.Blue */)
        {
            pooledObjects = new List<GameObject>();

            int rNum = -1;
            if (minSpecificPrefabCount > 0 && minSpecificPrefabCount <= itemCount)
            {
                rNum = minSpecificPrefabCount;
            }

            for (int i = 0; i < itemCount; i++)
            {
                GameObject obj = null;

                /*
                if (rNum > 0 && i < rNum)
                {
                    obj = (GameObject)Instantiate(GetPrefabByDebrisKind(specificPrefabDebrisKind));
                }
                else
                {
                    obj = (GameObject)Instantiate(GetRandomPrefab());
                }
                */

                obj = (GameObject)Instantiate(prefab);

                obj.SetActive(false);
                pooledObjects.Add(obj);
            }

            _poolCount = itemCount;
        }

        public GameObject GetPooledObject()
        {
            if (pooledObjects == null)
            {
                Init();
            }

            for (int i = 0; i < pooledObjects.Count; i++)
            {
                if (!pooledObjects[i].activeInHierarchy)
                {
                    return pooledObjects[i];
                }
            }

            if (willGrow)
            {
                GameObject obj = (GameObject)Instantiate(prefab);
                pooledObjects.Add(obj);

                return obj;
            }

            return null;
        }

        void Awake()
        {
            current = this;
        }

        /*
        private GameObject GetPrefabByDebrisKind(Debris.Kind kind)
        {
            foreach (GameObject go in prefabs)
            {
                Debris debris = go.GetComponent<Debris>();
                if (debris != null)
                {
                    if (debris.kind == kind)
                    {
                        return go;
                    }
                }
            }

            return null;
        }

        private GameObject GetRandomPrefab()
        {
            int index = UnityEngine.Random.Range(0, prefabs.Length);

            return prefabs[index];
        }
        */
    }
}