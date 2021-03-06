﻿using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsData : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI
        TMP_SongTitle,
        TMP_Score,
        TMP_Combo,
        TMP_MaxCombo,
        TMP_PerfectValue,
        TMP_GreatValue,
        TMP_GoodValue,
        TMP_OkValue,
        TMP_MissValue,
        TMP_OverallAccuracyValue;

    [SerializeField]
    private Image IMG_Ranking;

    [SerializeField]
    private Image[] IMG_SongCoverArt;


    // Start is called before the first frame update
    void OnEnable()
    {
        MapReader.WrapUp();
        Init();
    }

    void Init()
    {
        TMP_SongTitle.text = RoftPlayer.SongEntity.SongTitleUnicode;
        TMP_Score.text = GameManager.GetScore().ToString();
        TMP_Combo.text = GameManager.GetCombo().ToString();
        TMP_MaxCombo.text = GameManager.GetMaxCombo().ToString();
        TMP_PerfectValue.text = GameManager.GetAccuracyStats()[0].ToString();
        TMP_GreatValue.text = GameManager.GetAccuracyStats()[1].ToString();
        TMP_GoodValue.text = GameManager.GetAccuracyStats()[2].ToString();
        TMP_OkValue.text = GameManager.GetAccuracyStats()[3].ToString();
        TMP_MissValue.text = GameManager.GetAccuracyStats()[4].ToString();
        TMP_OverallAccuracyValue.text = string.Format("{0}%", GameManager.GetOverallAccuracy().ToString("F2", CultureInfo.InvariantCulture));

        foreach (Image img in IMG_SongCoverArt)
            img.sprite = RoftPlayer.SongEntity.BackgroundImage;

        IMG_Ranking.sprite = RankSystem.GetRankGradeSprite();
    }

    public void RetrySong()
    {
        RoftPlayer.StopMusic();
        GameManager.Instance.RestartSong();
        MapReader.Read(MapReader.CachedSongEntityID, MapReader.CachedDifficultyValue);
    }

    public void Back()
    {
        RoftPlayer.StopMusic();
        GameManager.Instance.RestartSong();
    }
}
