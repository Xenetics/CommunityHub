using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentPanelAnimEvents : MonoBehaviour
{
    public void ProxySetState()
    {
        GameManager.Instance.SetState(UIManager.Instance.NextState);
    }
}
