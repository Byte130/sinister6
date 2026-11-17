using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{

    public string SceneName;

    void Start()
    {
        var btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(OnButtonPressed);
    }

    public void OnButtonPressed()
    {
        if (!string.IsNullOrEmpty(SceneName))
            Application.Quit();
    }
}
