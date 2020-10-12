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

    private int INDEX_ID = -1;

    public void UpdateSongTitle(string value)
    {
        TMP_SongTitle.text = value;
    }

    public void AssignIndexID(int value)
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

    public void PreviewEntry()
    {
        SongList songList = SongList.Instance;

        songList.GetTMP().text = string.Format("Now playing: {0}",TMP_SongTitle.text);
        songList.GetSecondaryTMP().text = songList.GetTMP().text;

        songList.FadeInImages();
        songList.GetSongCover().sprite = songCover;
        songList.GetSongBackDrop().sprite = songCover;

        songList.GetSongPreviewer().SetSongToPreview(m_audio);
        songList.GetSongPreviewer().PlaySongPreview();
    }
}
