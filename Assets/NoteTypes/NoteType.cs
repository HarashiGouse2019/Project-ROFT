using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NoteType", menuName = "Note Type")]
public class NoteType : ScriptableObject
{
    public Sprite typeTexture;

    [ColorUsage(true)]
    public Color typeColor;
}
