using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenuButton : MonoBehaviour
{

    void Start()
    {
        var btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(OnButtonPressed);
    }

    public void OnButtonPressed()
    {
        SceneManager.LoadScene(0);
    }
}