using UnityEditorInternal;
using UnityEngine;
using System.Net.NetworkInformation;
public class GameManager : MonoBehaviour
{
    float Timer = 0f;
    [SerializeField]float TimeLimit;


    void Update()
    {
        Timer += Time.deltaTime;
        
        if (Timer >= TimeLimit)
        {
            Debug.Log("Time's up!");
        }
    }

}
