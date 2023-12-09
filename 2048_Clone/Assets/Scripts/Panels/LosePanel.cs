using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LosePanel : MonoBehaviour
{
    public void Restart()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(0);
    }

}
