using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : Singleton<LoadingManager>
{

    public void LoadScene(int sceneNo)
    {
        // 로딩 씬으로 전환
        SceneManager.LoadScene("Loading", LoadSceneMode.Single);
        // 씬 번호를 로딩 씬의 스크립트에 전달
        PlayerPrefs.SetInt("SceneToLoad", sceneNo);
    }
}
