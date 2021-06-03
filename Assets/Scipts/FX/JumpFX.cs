using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpFX : MonoBehaviour
{
    //所有FX通用的功能
    public void Finish()
    {
        gameObject.SetActive(false);
    }
}
