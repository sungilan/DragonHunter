using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor.SearchService;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private GameObject bar;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI tooltipText;
    private bool backGroundImageAndLoop;
    private float LoopTime;
    [SerializeField] private GameObject[] backgroundImages;
    [SerializeField] private string[] tooltips;
    [Range(0,1f)]public float vignetteEfectVolue; // Must be a value between 0 and 1
    AsyncOperation async;
    Image vignetteEfect;

    private void Start()
    {
        vignetteEfect = transform.Find("VignetteEfect").GetComponent<Image>();
        vignetteEfect.color = new Color(vignetteEfect.color.r,vignetteEfect.color.g,vignetteEfect.color.b,vignetteEfectVolue);

        if (backGroundImageAndLoop)
        {
            StartCoroutine(transitionImage());
        }
            
        int sceneToLoad = PlayerPrefs.GetInt("SceneToLoad", -1);
        if (sceneToLoad >= 0)
        {
            StartCoroutine(LoadingCourutine(sceneToLoad));
        }
    }

    // The pictures change according to the time of
    IEnumerator transitionImage ()
    {
        for (int i = 0; i < backgroundImages.Length; i++)
        {
            yield return new WaitForSeconds(LoopTime);
            for (int j = 0; j < backgroundImages.Length; j++)
                backgroundImages[j].SetActive(false);
            backgroundImages[i].SetActive(true);           
        }
    }

    // Activate the scene 
    private IEnumerator LoadingCourutine (int sceneNo)
    {
        async = SceneManager.LoadSceneAsync(sceneNo);
        async.allowSceneActivation = false;

        if (tooltipText != null && tooltips.Length > 0)
        {
            int randomIndex = Random.Range(0, tooltips.Length);
            tooltipText.text = tooltips[randomIndex];  // 랜덤 툴팁 출력
        }

        // Continue until the installation is completed
        while (async.isDone == false)
        {
            bar.transform.localScale = new Vector3(async.progress,0.9f,1);

            if (loadingText != null)
                loadingText.text = (100 * bar.transform.localScale.x).ToString("####") + "%";

            if (async.progress == 0.9f)
            {
                bar.transform.localScale = new Vector3(1, 0.9f, 1);
                async.allowSceneActivation = true;
            }
            yield return null;
        }
    }

}
