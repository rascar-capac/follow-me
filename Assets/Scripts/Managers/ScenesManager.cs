using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : Singleton<ScenesManager>
{
    [HideInInspector]
    public Scene PlayerScene;
    [HideInInspector]
    public Scene TribeScene;
    [HideInInspector]
    public Scene PlaygroundScene;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        LoadScene("PlayGround_scene");
        LoadScene("Tribe_scene");
        LoadScene("Player_scene");
        LoadScene("Camera_scene");
        LoadScene("Lights_scene");
        LoadScene("Target_scene");
        LoadScene("Items_scene");
    }

    public void LoadScene(string SceneName)
    {
        Scene scene = SceneManager.GetSceneByName(SceneName);
        this[SceneName] = scene;
        if (!scene.isLoaded)
        {
            SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
        }
        else
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.MergeScenes(scene, currentScene);
        }
    }

}
