using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGraphPanel : MonoBehaviour
{
    [SerializeField] private GameObject graphPanel;

    public void TogglePanel()
    {
        graphPanel.SetActive(!graphPanel.activeSelf);
    }
}
