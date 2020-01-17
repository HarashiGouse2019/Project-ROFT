using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SceneNavi : MonoBehaviour
{
    [Serializable]
    public class ROFTScenes
    {
        public string m_name;
    }

    public ROFTScenes currentScene;
    public ROFTScenes[] scenes = new ROFTScenes[5];

    public void SendMeToScene(string _targetScene)
    {
        //Now this is where we iterate through our array instead of 
        //well... anything else...
        //This is slightly more optimal than... well... anything else I used...
        foreach(ROFTScenes roftScene in scenes)
        {
            if (_targetScene == roftScene.m_name)
            {
                SceneManager.LoadScene(Array.IndexOf(scenes, roftScene));
                return;
            }
        }
    }
}
