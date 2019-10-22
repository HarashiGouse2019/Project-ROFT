using UnityEngine;

public class AccuracySign : MonoBehaviour
{
    public Sprite[] signs = new Sprite[5];

    private SpriteRenderer spriteRender;

    private float time = 0;

    private bool active;

    private const float showDuration = 10f;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRender = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Debug.Log(time);
        FadeOut();
        if (active) ShowFor(showDuration, 0.5f);
    }

    public void ShowSign(string _sign)
    {
        active = true;
        switch (_sign.ToLower())
        {
            case "perfect":
                spriteRender.sprite = signs[0];
                break;

            case "great":
                spriteRender.sprite = signs[1];
                break;

            case "good":
                spriteRender.sprite = signs[2];
                break;

            case "ok":
                spriteRender.sprite = signs[3];
                break;

            case "miss":
                spriteRender.sprite = signs[4];
                break;
        }


    }

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
        spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, 1 / time);
    }
}
