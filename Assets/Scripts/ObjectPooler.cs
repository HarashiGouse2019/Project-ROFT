using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPooler : MonoBehaviour
{

    public static ObjectPooler Instance;

    //Spawn inside an object or not, making that object the parent of the object being spawned
    public bool spawnInParent = false;

    //If all object are out of the pool, dynamically change the size
    public bool overridePoolSize = false;

    [System.Serializable]
    public class ObjectPoolItem
    {
        public string name;
        public int size;
        public GameObject prefab;
        public bool expandPool;
    }

    public List<ObjectPoolItem> itemsToPool;
    public List<GameObject> pooledObjects;
    
    public int poolIndex;

    void Awake()
    {
        Instance = this;
        InitObjectPooler(spawnInParent);
    }

    private void Start()
    {
        //if (readFromMusicManager == true)
        //{
        //    OverridePoolSizeOf("Song Panel", MusicManager.Instance.getMusic.Length);
        //}
    }

    private void Update()
    {

        //for (int musicIndex = 0; musicIndex < pooledObjects.Count; musicIndex++)
        //    if (!pooledObjects[musicIndex].activeInHierarchy)
        //    {
        //        const float defaultSpread = -1.5f;

        //        pooledObjects[musicIndex].SetActive(true);

        //        Image IMG_SONGPANEL = pooledObjects[musicIndex].GetComponent<Image>();

        //        RectTransform IMG_TRANSFORM = IMG_SONGPANEL.rectTransform;

        //        IMG_TRANSFORM.position = new Vector3(IMG_TRANSFORM.position.x, IMG_TRANSFORM.position.y + (defaultSpread * musicIndex));

        //    }
        //    else
        //        break;

    }


    /// <summary>
    /// Start creating Object Pools
    /// </summary>
    /// <param name="_spawnInParent"></param>
    void InitObjectPooler(bool _spawnInParent = false)
    {
        pooledObjects = new List<GameObject>();
        foreach (ObjectPoolItem item in itemsToPool)
        {
            for (int i = 0; i < item.size; i++)
            {
                GameObject newMember;
                if (_spawnInParent)
                    newMember = Instantiate(item.prefab, gameObject.transform);
                else
                    newMember = Instantiate(item.prefab);

                if (newMember.GetComponent<KeyId>() != null)
                    newMember.GetComponent<KeyId>().keyID = i;

                newMember.SetActive(false);
                item.prefab.name = item.name;
                pooledObjects.Add(newMember);
            }

        }
    }

    /// <summary>
    /// Grab an object in a specified pool by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject GetMember(string name)
    {
        #region Iteration
        for (int i = 0; i < pooledObjects.Count; i++)
        {

            if (!pooledObjects[i].activeInHierarchy && (name + "(Clone)") == pooledObjects[i].name)
            {
                poolIndex = i;
                return pooledObjects[i];
            }
        }
        #endregion

        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (name == item.prefab.name)
            {
                if (item.expandPool)
                {
                    GameObject newMember = Instantiate(item.prefab);
                    newMember.SetActive(false);
                    pooledObjects.Add(newMember);
                    poolIndex = pooledObjects.Count - 1;
                    return newMember;
                }
            }
        }
        Debug.LogWarning("We couldn't find a prefab of this name");
        return null;
    }

    /// <summary>
    /// Override a pool size to a different value
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_size"></param>
    void OverridePoolSizeOf(string _name, int _size)
    {
        for (int i = 0; i < itemsToPool.Count; i++)
        {
            if (itemsToPool[i].name == _name)
                itemsToPool[i].size = _size;
        }
    }
}
