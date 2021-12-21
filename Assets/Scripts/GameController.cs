using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI inforText;
    private void Start()
    {
        GetComponentInChildren<Toggle>().isOn = false;
        //ToggleInfo(false);
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void ToggleInfo(bool toggleValue)
    {
        inforText.gameObject.SetActive(toggleValue);
    }
}
