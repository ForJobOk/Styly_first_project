using UnityEngine;

/// <summary>
/// デバッグ用キー
/// </summary>
public class DebugOnEditor : MonoBehaviour
{
    void Update()
    {
        if(!Application.isEditor) return;
        
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Time.timeScale *= 2f;
            Debug.Log($"TimeScale:{Time.timeScale}");
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            Time.timeScale = 1f;
            Debug.Log($"TimeScale:{Time.timeScale}");
        }
    }
}
