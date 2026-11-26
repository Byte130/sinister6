using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] float timer = 0f;
    [SerializeField] float timeLimit;


    public GameObject PauseMenu;

    void Start()
    {
        timeLimit = Random.Range(45f, 75f);
        PauseMenu.SetActive(false);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeLimit)
        {
            PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
            foreach (PlayerMovement player in players)
            {
                player.BroadcastGemStatus();
                if (player.BroadcastGemStatus().Item1 && player.BroadcastGemStatus().Item2)
                {
                    Debug.Log(player.BroadcastGemStatus().Item2 + " has won!");
                    SceneManager.LoadScene(3);
                }
                else if (player.BroadcastGemStatus().Item1 && !player.BroadcastGemStatus().Item2)
                {
                    Debug.Log(player.BroadcastGemStatus().Item2 + " has won!");
                    SceneManager.LoadScene(2);
                }
                else
                {
                    Debug.Log("No one has won!");
                }
            }
        }
    }
}