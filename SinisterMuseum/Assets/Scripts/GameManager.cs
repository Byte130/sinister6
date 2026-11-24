using UnityEditorInternal;
using UnityEngine;
using System.Net.NetworkInformation;
public class GameManager : MonoBehaviour
{
    [SerializeField] float timer = 0f;
    [SerializeField] float timeLimit;

    void Start()
    {
        timeLimit = Random.Range(45f, 75f);
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
                if(player.BroadcastGemStatus().Item1 && player.BroadcastGemStatus().Item2)
                {
                    Debug.Log(player.BroadcastGemStatus().Item2 + " has won!");
                    // You can add additional logic here to handle the end of the game, such as stopping player movement, showing a victory screen, etc.
                }
                else if (player.BroadcastGemStatus().Item1 && !player.BroadcastGemStatus().Item2)
                {
                    Debug.Log(player.BroadcastGemStatus().Item2 + " has won!");
                    // You can add additional logic here to handle the end of the game, such as stopping player movement, showing a victory screen, etc.
                }
                else
                {
                    Debug.Log("No one has won!");
                }
            }
        }
    }
}
