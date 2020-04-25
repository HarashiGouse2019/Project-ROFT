using System.Collections;
using UnityEngine;

public class AccuracySign : MonoBehaviour
{
    [SerializeField] private Sprite[] signs = new Sprite[5];

    private SpriteRenderer spriteRender;

    private float time = 0;

    private bool active;

    private const float showDuration = 10f;

    //AccuracySignUpdate()
    public IEnumerator accuracySignRoutine;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRender = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGamePaused)
        {
            FadeOut();
            if (active) ShowFor(showDuration, 0.5f);
        }
    }

    //The image/sprite used to show accuracy
    public void ShowSign(int index)
    {
        active = true;
        spriteRender.sprite = signs[index];
    }

    //Duration as for how long it should show on screen
    void ShowFor(float _duration, float _rate = 0.1f)
    {
        time += _rate;
        if (time > _duration)
        {
            active = false;
            time = 0;
            gameObject.SetActive(false);
        }
    }

    void FadeOut()
    {
        spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, 1f / time);
    }
}
