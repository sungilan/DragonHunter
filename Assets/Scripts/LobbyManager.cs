using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private Button loadButton;

    private void Start()
    {
        loadButton.onClick.AddListener(OnLoadButtonClick);
    }

    private void OnLoadButtonClick()
    {
        LoadingManager.Instance.LoadScene(3);
    }
}
