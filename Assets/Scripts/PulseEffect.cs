using System.Collections;
using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    public Transform m_transform;
    public float waitDuration = 0.05f;
    public float shrinkRate = 0.05f;
    Vector3 scale;
    Vector3 normalScale;
    bool waiting = false;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        m_transform = GetComponent<Transform>();
        scale = m_transform.localScale;
        normalScale = scale;
    }

    //When I hit a note, this function will be called
    public void DoPulseReaction(float _size = 0.2f)
    {
        Vector3 pulseSize = new Vector3(_size, _size, _size);
        scale += pulseSize;
        m_transform.localScale = scale;
        StartCoroutine(Run());
    }

   IEnumerator Run()
    {
        const float SIXTYITH_OF_SEC = (1f / 60f);

        while (true)
        {
            if (!GameManager.Instance.IsGamePaused)
            {
                scale = new Vector3(scale.x - shrinkRate, scale.y - shrinkRate, 1f);
                m_transform.localScale = scale;

                if (scale.x <= normalScale.x && scale.y <= normalScale.y)
                {
                    m_transform.localScale = scale;
                    StopAllCoroutines();
                }
            }
            yield return new WaitForSeconds(SIXTYITH_OF_SEC);
        }
    }
}
