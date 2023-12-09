using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPanel : MonoBehaviour
{
    public void PlayAgain()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(0);
    }
   
}
