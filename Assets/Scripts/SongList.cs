using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SongList : MonoBehaviour
{
    public static SongList Instance;

    public enum SORTING
    {
        NONE,
        NAME,
        ARTIST,
        INITIAL_DIFFICULTY
    }

    public SORTING sortingOrder = SORTING.NONE;

    [SerializeField]
    private SongContent content;

    [SerializeField]
    private PreviewSong songPreviewer;

    [SerializeField]
    private TextMeshProUGUI TMP_SongTitle;

    [SerializeField]
    private Image songCover, songBackDrop;

    [SerializeField]
    private TMP_Dropdown TMPDD_SortingOrder;

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
        #region Variable Declaration
        Song_Entity currentEntity;

        List<Song_Entity> orderedSongs = new List<Song_Entity>();
        #endregion

        #region Sorting
        switch (sortingOrder)
        {
            case SORTING.NONE:
                orderedSongs = MusicManager.Songs;
                break;

            case SORTING.NAME:
                var nameSorting = from song
                                  in MusicManager.Songs
                                  orderby song.SongTitle
                                  select song;

                orderedSongs = nameSorting.ToList();
                break;

            case SORTING.ARTIST:
                var artistSorting = from song
                                    in MusicManager.Songs
                                    orderby song.SongArtist, song.SongTitle
                                    select song;

                orderedSongs = artistSorting.ToList();
                break;

            case SORTING.INITIAL_DIFFICULTY:
                var diffSorting = from song
                                  in MusicManager.Songs
                                  orderby song.InitialDifficultyRating, song.SongArtist
                                  select song;

                orderedSongs = diffSorting.ToList();
                break;

            default:
                orderedSongs = MusicManager.Songs;
                break;
        }
        #endregion

        #region Initialization
        for (int index = 0; index < orderedSongs.Count; index++)
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
                    currentEntity = orderedSongs[index];
                    songEntry.UpdateSongTitle(string.Format("{0} - {1}", currentEntity.SongArtistUnicode, currentEntity.SongTitleUnicode));
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
        #endregion
    }

    public void ReInit()
    {
        //Iterate through pooled objects, and set them inactive;
        foreach (GameObject log in content.objectPooler.pooledObjects)
        {
            if (log.activeInHierarchy)
                log.SetActive(false);
        }

        //Update Sorting Order from DropDown value
        sortingOrder = (SORTING)TMPDD_SortingOrder.value;

        //Initialize again
        Init();
    }

    public void FadeInImages()
    {
        StartCoroutine(FadeInImagesCycle());
    }

    private IEnumerator FadeInImagesCycle()
    {
        for (float i = 0; i < 255; i += 10f)
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
