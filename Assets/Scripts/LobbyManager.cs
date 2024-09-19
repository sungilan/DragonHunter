using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private Button loadButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button missionButton;
    [SerializeField] private Canvas ShopCanvas;
    [SerializeField] private Canvas MissionCanvas;
    
    [SerializeField] TextMeshProUGUI nicknameText;
    [SerializeField] TextMeshProUGUI goldText;

    private void Start()
    {
        loadButton.onClick.AddListener(OnLoadButtonClick);
        shopButton.onClick.AddListener(OnShopButtonClick);
        missionButton.onClick.AddListener(OnMissionButtonClick);
    }
    private void Update()
    {
        nicknameText.text = DataManager.Instance.nickname;
        goldText.text = DataManager.Instance.gold.ToString();
    }

    private void OnLoadButtonClick()
    {
        LoadingManager.Instance.LoadScene(3);
    }
    private void OnShopButtonClick()
    {
        ShopCanvas.gameObject.SetActive(true);
    }
    private void OnMissionButtonClick()
    {
        MissionCanvas.gameObject.SetActive(true);
    }
}
