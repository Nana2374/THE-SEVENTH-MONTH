using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingController : MonoBehaviour
{
    public string levelToLoad = "Lvl1"; // Set this to your level scene name

    private void Start()
    {
        StartCoroutine(LoadLevelAfterDelay());
    }

    private IEnumerator LoadLevelAfterDelay()
    {
        // Wait for 3 seconds while showing the loading screen
        yield return new WaitForSeconds(3f);

        // Load the level scene asynchronously
        AsyncOperation levelOperation = SceneManager.LoadSceneAsync(levelToLoad);
        levelOperation.allowSceneActivation = false;

        // Optionally, wait until the level scene is fully loaded
        while (!levelOperation.isDone)
        {
            yield return null;
        }
    }
}
