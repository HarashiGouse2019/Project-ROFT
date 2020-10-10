using UnityEngine;
using UnityEngine.EventSystems;

public class SoundEffect : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField]
    private string m_name;

    // Start is called before the first frame update
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Play(m_name);    
    }
}
