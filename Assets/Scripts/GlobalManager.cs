using UnityEngine;
using DefaultNamespace; // Needed to see the Arena script

public class GameManager : MonoBehaviour
{
    [Header("Settings")]
    public int enemiesToKill = 1; // Set to 0 to "Auto-Detect" all enemies in scene
    
    [Header("References")]
    public Arena arena; // Drag your '競技場' object here

    private int currentKills = 0;

    void Start()
    {
        // AUTO-DETECT MODE:
        // If you leave enemiesToKill at 0, it automatically counts 
        // how many objects tagged "Enemy" are in the scene at the start.
        if (enemiesToKill == 0)
        {
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            enemiesToKill = allEnemies.Length;
            Debug.Log($"Auto-detected {enemiesToKill} enemies.");
        }
    }

    public void ReportEnemyDeath()
    {
        
    }
}