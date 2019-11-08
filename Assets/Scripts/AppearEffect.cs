using UnityEngine;

public class AppearEffect : NoteEffect
{
    public float initiatedNoteSample;
    public float initiatedNoteOffset;
    public float offsetStart;
    public const float completeOpacity = 3000f;

    public KeyCode assignedKeyBind;

    public SpriteRenderer sprite;

    Color originalAppearance;

    void Awake()
    {
        Instance = this;
        sprite = GetComponent<SpriteRenderer>();
        originalAppearance = sprite.color;
    }

    private void OnEnable()
    {
        initiatedNoteSample = noteSample;
        initiatedNoteOffset = noteOffset;
    }

    // Update is called once per frame
    void Update()
    {
        if (!EditorToolClass.Instance.record)
            AppearOn();
    }

    void AppearOn()
    {
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, GetPercentage() - 0.25f);
    }

    protected override float GetPercentage()
    {
        percentage = ((EditorToolClass.musicSource.timeSamples - completeOpacity) - offsetStart) / (initiatedNoteSample - offsetStart - completeOpacity);
        return percentage;
    }

    private void OnDisable()
    {
        sprite.color = originalAppearance;
        percentage = 0;
        assignedKeyBind = KeyCode.None;
        initiatedNoteSample = 0;
        initiatedNoteOffset = 0;
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }
}
