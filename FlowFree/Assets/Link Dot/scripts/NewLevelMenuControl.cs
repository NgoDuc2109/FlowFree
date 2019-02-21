using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class NewLevelMenuControl : MonoBehaviour
{

    [SerializeField]
    private GameObject homePanel, questionPackagePanel, levelPackagePanel;
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private GameObject levelButtonPrefab;
    [SerializeField]
    private List<string> packageName = new List<string>();
    [SerializeField]
    private Transform content;
    public static int currentPackage;

    [SerializeField]
    private GameObject bgOn, bgOff, effectOn, effectOff;
    private void Start()
    {

        if (GameData.getInstance().isBackToLevel == true)
        {
            homePanel.SetActive(false);
            OnClickQuestionPackage(currentPackage);
        }
        if (PlayerPrefs.GetInt("muteEffect") == 1)
        {
            AudioManager.Instance.effectAudioSource.mute = true;
            effectOff.SetActive(true);
            effectOn.SetActive(false);
        }

        if (PlayerPrefs.GetInt("muteBackground") == 1)
        {
            AudioManager.Instance.bgAudioSource.mute = true;
            bgOff.SetActive(true);
            bgOn.SetActive(false);
        }
    }

    public void OnClickQuestionPackage(int number)
    {
        AudioManager.Instance.PlayButtonClip();
        currentPackage = number;
        questionPackagePanel.SetActive(false);
        GameData.difficulty = number;
        levelPackagePanel.SetActive(true);
        nameText.text = packageName[number];
        for (int i = 0; i < content.childCount; i++)
        {
            GameObject child = content.GetChild(i).gameObject;
            Destroy(child);
        }
        content.transform.localScale = Vector3.one;
        for (int i = 0; i < GameData.totalLevel[number]; i++)
        {
            GameObject tbtn = Instantiate(levelButtonPrefab, content.transform) as GameObject;
            tbtn.SetActive(true);
            Text ttext = tbtn.GetComponentInChildren<Text>();
            ttext.text = (i + 1).ToString();
            if (PlayerPrefs.GetInt("linkdot_" + GameData.difficulty + "_" +i) == 1)
            {
                int tStar = PlayerPrefs.GetInt("levelStar_" + GameData.difficulty + i);
                ttext.transform.Find("star" + tStar).GetComponent<Image>().enabled = true;
            }          
            tbtn.name = "level" + (i + 1);
            tbtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => clickLevel(tbtn));
        }
    }
    private void clickLevel(GameObject tbtn)
    {
        AudioManager.Instance.PlayButtonClip();
        GameData.getInstance().cLevel = int.Parse(tbtn.GetComponentInChildren<Text>().text) - 1;
        SceneManager.LoadScene("Game");
    }


    public void OnClickPlayBtn()
    {
        AudioManager.Instance.PlayButtonClip();
        homePanel.SetActive(false);
    }
    public void OnClickBackToPackageBtn()
    {
        AudioManager.Instance.PlayButtonClip();
        questionPackagePanel.SetActive(true);
        levelPackagePanel.SetActive(false);
    }

    public void OnClickBackToHomeBtn()
    {
        AudioManager.Instance.PlayButtonClip();
        homePanel.SetActive(true);
    }
    public void OnClickBgMusicOn()
    {
        PlayerPrefs.SetInt("muteBackground", 1);
        AudioManager.Instance.bgAudioSource.mute = true;
        bgOn.SetActive(false);
        bgOff.SetActive(true);
    }

    public void OnClickBgMusicOff()
    {
        PlayerPrefs.SetInt("muteBackground", 0);
        AudioManager.Instance.bgAudioSource.mute = false;
        bgOn.SetActive(true);
        bgOff.SetActive(false);
    }

    public void OnClickEffectMusicOn()
    {
        PlayerPrefs.SetInt("muteEffect", 1);
        AudioManager.Instance.effectAudioSource.mute = true;
        effectOn.SetActive(false);
        effectOff.SetActive(true);
    }

    public void OnClickEffectMusicOff()
    {
        PlayerPrefs.SetInt("muteEffect", 0);
        AudioManager.Instance.effectAudioSource.mute = false;
        effectOn.SetActive(true);
        effectOff.SetActive(false);
    }
}
