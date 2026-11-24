using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class playerManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoint;
    [SerializeField] private float spawns=0;
    [SerializeField] private float maxPlayers = 4;

    private Dictionary<Gamepad, GameObject> gamepadToPlayer = new Dictionary<Gamepad, GameObject>();
    private Dictionary<Gamepad, int> gamepadToSpawnIndex = new Dictionary<Gamepad, int>();
    private bool[] spawnPointOccupied;

    void Start()
    {
        spawnPointOccupied = new bool[spawnPoint.Length];
    }

    void Update()
    {
        if (Gamepad.all.Count == 0)
            return;
        foreach (var gamePad in Gamepad.all)
        {
            if (gamePad != null && gamePad.startButton.wasPressedThisFrame && !gamepadToPlayer.ContainsKey(gamePad) && spawns < maxPlayers)
            {
                SpawnPlayer(gamePad);
            }
        }
    }

    private void SpawnPlayer(Gamepad gamePad)
    {
        if (spawnPoint.Length == 0) return;
        
        List<int> availableSpawnIndices = new List<int>();
        for (int i = 0; i < spawnPointOccupied.Length; i++)
        {
            if (!spawnPointOccupied[i])
            {
                availableSpawnIndices.Add(i);
            }
        }
        
        if (availableSpawnIndices.Count == 0) return;
        
        int availableSpawnIndex = availableSpawnIndices[Random.Range(0, availableSpawnIndices.Count)];
        
        Transform spawnLocation = spawnPoint[availableSpawnIndex];
        
        GameObject newPlayer = Instantiate(playerPrefab, spawnLocation.position, Quaternion.identity);
        
        var playerInput = newPlayer.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.SwitchCurrentControlScheme("Gamepad", gamePad);
            playerInput.camera = Camera.main;
        }
        
        // Determine team based on spawn index (0-1: false, 2-3: true)
        bool teamValue = availableSpawnIndex >= 2;
        var playerMovement = newPlayer.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.TeamSelect(teamValue);
        }
        
        gamepadToPlayer[gamePad] = newPlayer;
        gamepadToSpawnIndex[gamePad] = availableSpawnIndex;
        spawnPointOccupied[availableSpawnIndex] = true;
    }

    public void OnPlayerJoined()
    {
        Debug.Log("Player Joined");
        spawns++;
    }

    public void OnPlayerLeft(Gamepad gamePad)
    {
        Debug.Log("Player Left");
        if (gamepadToSpawnIndex.ContainsKey(gamePad))
        {
            spawnPointOccupied[gamepadToSpawnIndex[gamePad]] = false;
            gamepadToSpawnIndex.Remove(gamePad);
        }
        gamepadToPlayer.Remove(gamePad);
        spawns--;
    }
}
