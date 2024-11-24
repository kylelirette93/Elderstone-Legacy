using UnityEngine;
using UnityEngine.EventSystems;

public class PanelReset : MonoBehaviour
{
    void OnEnable()
    {
        // Clear any locked selection to ensure free interaction
        EventSystem.current.SetSelectedGameObject(null);
    }
}