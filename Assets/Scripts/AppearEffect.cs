using UnityEngine;

public class AppearEffect : NoteEffect
{
    public float initiatedNoteSample;
    public float initiatedNoteOffset;
    public float offsetStart;

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
        transform.localScale = new Vector3(1f, 1f, 1f);
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, GetPercentage());
    }

    protected override float GetPercentage()
    {
        percentage = (EditorToolClass.musicSource.timeSamples - offsetStart) / (initiatedNoteSample - offsetStart);
        return percentage;
    }

    private void OnDisable()
    {
        sprite.color = originalAppearance;
        percentage = 0;
        assignedKeyBind = KeyCode.None;
        initiatedNoteSample = 0;
        initiatedNoteOffset = 0;
    }
}
