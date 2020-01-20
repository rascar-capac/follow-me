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
        //Debug.Log("LOGGGG >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><<<");
        currentScene = SceneManager.GetActiveScene();
        //Debug.Log("Current scene : " + currentScene.name);
        //Debug.Log($"Scenes count : {SceneManager.sceneCountInBuildSettings}");

        //for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        //{
        //    Scene scene = SceneManager.GetSceneByBuildIndex(i);
        //    if (scene == null)
        //        Debug.Log("scene is null" );

        //    Debug.Log($"Loading scene index {i}: {scene.name}" );

        //    // skip if is current scene since we don't want it twice
        //    if (currentScene.buildIndex == i)
        //    {
        //        Debug.Log($"Is the current scene...not loaded.");
        //        continue;
        //    }
        //    // Skip if scene is already loaded
        //    if (scene.IsValid() && scene.isLoaded)
        //    {
        //        Debug.Log($"already loaded...skipped.");
        //        continue;
        //    }
        //    else
        //    {
        //        Debug.Log($"Loading....");
        //        SceneManager.LoadScene(i, new LoadSceneParameters( LoadSceneMode.Additive));
        //    }
        // or depending on your usecase
        //SceneManager.LoadSceneAsync(i, LoadSceneMode.Additive);
        //}

        LoadScene("PlayGround_scene");
        LoadScene("Tribe_scene");
        LoadScene("Player_scene");
        LoadScene("Camera_scene");
        LoadScene("Lights_scene");
        LoadScene("Target_scene");
        LoadScene("Items_scene");
        LoadScene("UI_scene");
        //Debug.Log("END LOGGGG >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><<<");
}

public void LoadScene(string SceneName)
    {
        Scene scene = SceneManager.GetSceneByName(SceneName);
        //Debug.Log("verify scene async " + scene.name);
        //this[SceneName] = scene;
        if (!scene.isLoaded)
        {
            //Debug.Log("Loading scene async " + scene.name);
            SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
        }

        
        //SceneManager.MergeScenes(scene, currentScene);
    }

}
