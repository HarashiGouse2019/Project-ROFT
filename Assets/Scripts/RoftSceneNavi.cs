using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class RoftSceneNavi : MonoBehaviour
{
    public static RoftSceneNavi Instance;

    [Serializable]
    public class ROFTScenes
    {
        public string m_name;
    }

    public ROFTScenes currentScene;
    public ROFTScenes[] scenes = new ROFTScenes[5];
    public Image loadingIcon;

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        } 
        #endregion
    }

    public void SendMeToScene(string _targetScene)
    {
        //Now this is where we iterate through our array instead of 
        //well... anything else...
        //This is slightly more optimal than... well... anything else I used...
        foreach (ROFTScenes roftScene in scenes)
        {
            if (_targetScene == roftScene.m_name)
            {
                SceneManager.LoadScene(Array.IndexOf(scenes, roftScene));
                return;
            }
        }
    }
}
