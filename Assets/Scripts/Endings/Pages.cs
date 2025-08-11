using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pages : MonoBehaviour
{
    public void Home()
    {
        GameManager.Instance.SetState(eState.Main);
    }
}
