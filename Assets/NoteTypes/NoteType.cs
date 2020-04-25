using UnityEngine;

[CreateAssetMenu(fileName = "New NoteType", menuName = "Note Type")]
public class NoteType : ScriptableObject
{
    public Sprite typeOuterTexture;
    public Sprite typeInnerTexture;

    [ColorUsage(true)]
    public Color typeColor;
}
