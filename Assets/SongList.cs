using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SongList : MonoBehaviour
{
    public static SongList Instance;
    [SerializeField]
    private SongContent content;

    [SerializeField]
    private PreviewSong songPreviewer;

    [SerializeField]
    private TextMeshProUGUI TMP_SongTitle;

    [SerializeField]
    private Image songCover, songBackDrop;

    GameObject songEntryObj;
    SongEntry songEntry;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        Song_Entity currentEntity;
        for (int index = 0; index < MusicManager.Songs.Count; index++)
        {
            try
            {
                songEntryObj = content.GetObjectPooler().GetMember("Log");

                songEntry = songEntryObj.GetComponent<SongEntry>();

                if (songEntryObj != null && !songEntryObj.activeInHierarchy)
                {
                    //Set it to Active
                    songEntryObj.SetActive(true);

                    //We will not reposition it. ObjectPooler from SongContent should put the entries for us.
                    //Assigning important values.
                    currentEntity = MusicManager.Songs[index];
                    songEntry.UpdateSongTitle(string.Format("{0} - {1}",currentEntity.SongArtist,currentEntity.SongTitle));
                    songEntry.UpdateSongCover(currentEntity.BackgroundImage);
                    songEntry.UpdateSongClip(currentEntity.AudioFile);
                    songEntry.UpdateRanking(null);

                    //Assign Index ID
                    songEntry.AssignIndexID(index);
                }
            }
            catch (IOException e)
            {
                Debug.LogException(e);
            }
        }
    }

    public void FadeInImages()
    {
        StartCoroutine(FadeInImagesCycle());
    }

    private IEnumerator FadeInImagesCycle()
    {
        for(float i = 0; i < 255; i+=10f)
        {
            songCover.color = new Color(songCover.color.r, songCover.color.g, songCover.color.b, i / 255f);
            songBackDrop.color = new Color(songBackDrop.color.r, songBackDrop.color.g, songBackDrop.color.b, i / 255f);

            yield return null;
        }
    }

    public TextMeshProUGUI GetTMP() => TMP_SongTitle;
    public Image GetSongCover() => songCover;
    public Image GetSongBackDrop() => songBackDrop;
    public PreviewSong GetSongPreviewer() => songPreviewer;
}
