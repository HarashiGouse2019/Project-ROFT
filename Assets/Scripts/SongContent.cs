using UnityEngine;

public class SongContent : MonoBehaviour
{
    [SerializeField]
    public ObjectPooler objectPooler;

    public ObjectPooler GetObjectPooler() => objectPooler;
}
