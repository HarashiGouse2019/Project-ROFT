using UnityEngine;

public class AppearEffect : CloseInEffect
{
    public KeyCode assignedKeyBind;

    //public SpriteRenderer sprite;
    public SpriteRenderer[] overlaySprites;
    public SpriteRenderer childSprite;

    public GameObject assignedCircle;

    const int completeOpacity = 20000;

    Color originalOverlayAppearance;

    float time;
    void Awake()
    {
        Instance = this;

        //Get sprite renderers
        sprite = GetComponent<SpriteRenderer>();
        overlaySprites = GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer overlaySprite in overlaySprites)
        {
            if (overlaySprite != sprite)
                childSprite = overlaySprite;
        }


        if (Key_Layout.Instance != null && Key_Layout.Instance.layoutMethod == Key_Layout.LayoutMethod.Region_Scatter)
        {
            //We want these completely transparent from start
            childSprite.color = new Color(childSprite.color.r, childSprite.color.g, childSprite.color.b, 0f);
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f);

            //Assign this to orignal variable
            originalAppearance = sprite.color;
            originalOverlayAppearance = childSprite.color;
        }
    }

    private void OnEnable()
    {

        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);


        foreach (SpriteRenderer overlaySprite in overlaySprites)
        {
            if (overlaySprite != sprite)
                childSprite = overlaySprite;
        }


        if (Key_Layout.Instance.layoutMethod == Key_Layout.LayoutMethod.Region_Scatter)
        {


            //We want these completely transparent from start
            childSprite.color = new Color(childSprite.color.r, childSprite.color.g, childSprite.color.b, 0f);
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f);

            //Assign this to orignal variable
            originalAppearance = sprite.color;
            originalOverlayAppearance = childSprite.color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!RoftPlayer.Instance.record)
            AppearOn();
    }

    void AppearOn()
    {
        float appearanceRate = GetPercentage() + 0.02f;

        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, appearanceRate);
        if (childSprite != null)
            childSprite.color = new Color(childSprite.color.r, childSprite.color.g, childSprite.color.b, appearanceRate);

        if (!assignedCircle.activeInHierarchy)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0);
            childSprite.color = new Color(childSprite.color.r, childSprite.color.g, childSprite.color.b, 0);
            assignedKeyBind = KeyCode.None;
            DelayDestroy(0.5f);
        }
    }

    protected override float GetPercentage()
    {
        percentage = ((RoftPlayer.musicSource.timeSamples) - offsetStart) / (initiatedNoteSample - offsetStart);
        return percentage;
    }

    private void OnDisable()
    {
        sprite.color = originalAppearance;

        if(childSprite!=null)
            
            childSprite.color = originalOverlayAppearance;
        percentage = 0;
        assignedKeyBind = KeyCode.None;
        initiatedNoteSample = 0;
        initiatedNoteOffset = 0;
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        if (assignedCircle != null)
            assignedCircle = null;
    }

    void DelayDestroy(float _duration)
    {

        time += Time.deltaTime;
        if (time > _duration)
        {
            time = 0;
            gameObject.SetActive(false);
        }
    }
}
