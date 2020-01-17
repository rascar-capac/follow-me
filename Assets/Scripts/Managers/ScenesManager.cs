using System.Collections;
using System.Collections.Generic;
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

    Scene currentScene ;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        currentScene = SceneManager.GetActiveScene();

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            Debug.Log("Loading scene: " + SceneManager.GetSceneByBuildIndex(i).name);
            // skip if is current scene since we don't want it twice
            if (currentScene.buildIndex == i) continue;

            // Skip if scene is already loaded
            if (SceneManager.GetSceneByBuildIndex(i).IsValid()) continue;

            SceneManager.LoadScene(i, LoadSceneMode.Additive);
            Debug.Log("Loaded");
            // or depending on your usecase
            //SceneManager.LoadSceneAsync(i, LoadSceneMode.Additive);
        }

        //      LoadScene("PlayGround_scene");
        //      LoadScene("Tribe_scene");
        //      LoadScene("Player_scene");
        //      LoadScene("Camera_scene");
        //      LoadScene("Lights_scene");
        //      LoadScene("Target_scene");
        //      LoadScene("Items_scene");
        //LoadScene("UI_scene");
    }

	public void LoadScene(string SceneName)
    {
        Scene scene = SceneManager.GetSceneByName(SceneName);
        //this[SceneName] = scene;
        if (!scene.isLoaded)
        {
            SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
        }

        
        //SceneManager.MergeScenes(scene, currentScene);
    }

}
