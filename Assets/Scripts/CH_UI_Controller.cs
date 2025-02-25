using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class CH_UI_Controller : MonoBehaviour
{
    public GameObject[] UI_Panels;
    public Sprite[] Mute_Sprites;
    public Image Mute_Button;
    public AudioMixer audioListener;
    public bool isMute = false;
    public float fadeDuration = 1f;

       // private SS_PlayerMovement playerMovement;

    void Start()
    {
         //playerMovement = FindAnyObjectByType<SS_PlayerMovement>();
        Open_UI(UI_Panels[0]);

    }
    public void Open_UI(GameObject panel)
    {


        foreach (GameObject g in UI_Panels)
        {
            g.SetActive(false);
        }
        panel.SetActive(true);
        Time.timeScale = 1;
        if (!isMute)
        {
            audioListener.SetFloat("Vol", 0);
        }
        UI_functions(panel.name);
        if (panel.activeSelf && panel.name == "Start_Panel")
        {
           // playerMovement.isGameOver = false;
           // playerMovement.ResetPlayerPosition();
        }


    }
    private void UI_functions(string panel_name)
    {
        switch (panel_name)
        {
            case "Start_Panel":
                break;
            case "Game_Panel":
                break;
            case "Pause_Panel":
                Time.timeScale = 0;
                break;
            case "Gameover_Panel":
                Time.timeScale = 1;

                break;
            case "Information_Panel":
                break;
            default:
                break;
        }
    }
    public void Toggle_Mute()
    {
        if (Mute_Button.sprite == Mute_Sprites[0])
        {
            Mute_Button.sprite = Mute_Sprites[1];
            audioListener.SetFloat("Vol", -80f);
            isMute = true;
        }
        else
        {
            Mute_Button.sprite = Mute_Sprites[0];
            audioListener.SetFloat("Vol", 0f);
            isMute = false;
        }

    }
    public void CloseGame()
    {
        Application.Quit();
    }
    public void Click_Sound()
    {
        gameObject.GetComponent<AudioSource>().Play();
    }


}