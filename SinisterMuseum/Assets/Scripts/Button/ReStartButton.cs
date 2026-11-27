using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ReStartButton : MonoBehaviour
{
    void Start()
    {
        var btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(OnButtonPressed);
    }

    public void OnButtonPressed()
    {
            SceneManager.LoadScene(1);
    }
}