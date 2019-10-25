using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPooler : MonoBehaviour
{

    public static ObjectPooler Instance;

    public bool overridePoolSize;

    public bool readFromMusicManager;

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
    }

    private void Start()
    {
        if (readFromMusicManager == true)
        {
            OverridePoolSizeOf("Song Panel", MusicManager.manager.getMusic.Length);
        }

        InitObjectPooler();
    }

    private void Update()
    {
        if (readFromMusicManager)
        {
            for (int musicIndex = 0; musicIndex < pooledObjects.Count; musicIndex++)
                if (!pooledObjects[musicIndex].activeInHierarchy)
                {
                    const float defaultSpread = -1.5f;

                    pooledObjects[musicIndex].SetActive(true);

                    Image IMG_SONGPANEL = pooledObjects[musicIndex].GetComponent<Image>();

                    RectTransform IMG_TRANSFORM = IMG_SONGPANEL.rectTransform;

                    IMG_TRANSFORM.position = new Vector3(IMG_TRANSFORM.position.x, IMG_TRANSFORM.position.y + (defaultSpread * musicIndex));
                    
                }
                else
                    break;
        }
    }

    void InitObjectPooler()
    {
        pooledObjects = new List<GameObject>();
        foreach (ObjectPoolItem item in itemsToPool)
        {
            for (int i = 0; i < item.size; i++)
            {
                GameObject newMember = Instantiate(item.prefab, gameObject.transform);
                newMember.SetActive(false);
                item.prefab.name = item.name;
                pooledObjects.Add(newMember);
            }

        }
    }

    public GameObject GetMember(string name)
    {
        #region Iteration
        for (int i = 0; i < pooledObjects.Count; i++)
        {

            if (!pooledObjects[i].activeInHierarchy && (name + "(Clone)") == pooledObjects[i].name)
            {
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
                    Debug.Log("Ran out of members in pool. Creating more members!!!");
                    GameObject newMember = Instantiate(item.prefab);
                    newMember.SetActive(false);
                    pooledObjects.Add(newMember);
                    return newMember;
                }
            }
        }
        Debug.LogWarning("We couldn't find a prefab of this name");
        return null;
    }

    void OverridePoolSizeOf(string _name, int _size)
    {
        for (int i = 0; i < itemsToPool.Count; i++)
        {
            if (itemsToPool[i].name == _name)
                itemsToPool[i].size = _size;
        }
    }
}
