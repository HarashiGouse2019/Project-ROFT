using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SongEntry : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI TMP_SongTitle;

    [SerializeField]
    private Image ranking;

    [SerializeField]
    private Sprite songCover;

    [SerializeField]
    private AudioClip m_audio;

    private uint INDEX_ID = uint.MaxValue;

    public void UpdateSongTitle(string value)
    {
        TMP_SongTitle.text = value;
    }

    public void AssignIndexID(uint value)
    {
        INDEX_ID = value;
    }

    public void UpdateSongCover(Sprite image)
    {
        songCover = image;
    }

    public void UpdateSongClip(AudioClip song)
    {
        m_audio = song;
    }

    public void UpdateRanking(Sprite image)
    {
        ranking.sprite = image;
    }

    /// <summary>
    /// Preview song selected by the player
    /// </summary>
    public void PreviewEntry()
    {
        SongList songList = SongList.Instance;

        songList.GetSongNavigator().SetSongEntityPosition((uint)INDEX_ID);

        songList.GetTMP().text = string.Format("Now playing: {0}",TMP_SongTitle.text);
        songList.GetSecondaryTMP().text = songList.GetTMP().text;

        songList.FadeInImages();

        foreach (Image img in songList.GetSongCovers())
            img.sprite = songCover;
        
        songList.GetSongPreviewer().SetSongToPreview(m_audio);
        songList.GetSongPreviewer().PlaySongPreview();
    }
}
