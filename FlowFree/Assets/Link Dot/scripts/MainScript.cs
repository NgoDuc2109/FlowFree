using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SimpleJson;
using DG.Tweening;
using linkDot;
using System.Collections.Generic;
using System;

public class MainScript : MonoBehaviour
{
    [SerializeField]
    private GameObject bgOn, bgOff, effectOn, effectOff;
    public static MainScript Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //data
    public int timeCount = 120;//how much time you left.The more time,the better score
    void Start()
    {
        StartCoroutine("waitAsecond");

        //fadeOut ();
        all_game = GameObject.Find("all_game");
        init();//for test
        StartCoroutine("nextframe");
        //gameWin();
    }

    //GameObject all_level;//levelmenu container
    GameObject all_game;
    IEnumerator nextframe()
    {
        yield return new WaitForEndOfFrame();

    }

    public void init()
    {
        GameData.getInstance().currentScene = 2;
        GameObject.Find("linkdot").GetComponent<LinkDot>().init();
        refreshView();
    }


    /// <summary>
    /// check if win every second
    /// </summary>
    /// <returns>The asecond.</returns>
    IEnumerator waitAsecond()
    {
        yield return new WaitForSeconds(1);

        if (timeCount > 0)
        {
            if (GameData.getInstance().isWin == false)
            {
                timeCount--;
                StartCoroutine("waitAsecond");
            }
        }
    }

    /// <summary>
    /// Refreshs the view.
    /// </summary>
    public void refreshView()
    {
        //GameObject.Find("btnTip").GetComponentInChildren<Text>().text = GameData.getInstance().tipRemain.ToString();
        GameObject.Find("lb_level").GetComponent<Text>().text = (GameData.getInstance().cLevel + 1).ToString();
        StopAllCoroutines();
        totalPath.Clear();
        StartCoroutine(Initial());
        timeCount = 60;
        StartCoroutine(waitAsecond());
        if (PlayerPrefs.GetInt("muteEffect") == 1)
        {
            effectOff.SetActive(true);
            effectOn.SetActive(false);
        }

        if (PlayerPrefs.GetInt("muteBackground") == 1)
        {
            bgOff.SetActive(true);
            bgOn.SetActive(false);
        }
    }



    //handler event
    public void OnRetryClick()
    {
        AudioManager.Instance.PlayButtonClip();
        if (GameData.instance.isLock) return;
        // Add event handler code here
        GameObject.Find("linkdot").GetComponent<LinkDot>().init();
    }

    public GameObject panelWin;//win panel gameobject
    WinPanel winpanel;//winpanel controller
                      /// <summary>
                      /// when game wins.
                      /// </summary>
                      /// 
    public void gameWin()
    {
        AudioManager.Instance.PlayWinClip();
        int threeStar = 10;
        int twoStar = threeStar + 5;//
        int oneStar = threeStar + 10;//

        int starGet = 0;
        if ((60 - timeCount) <= threeStar)
        {
            starGet = 3;
        }
        else if ((60 - timeCount) > threeStar && (60 - timeCount) <= twoStar)
        {
            starGet = 2;
        }
        else if ((60 - timeCount) > twoStar && (60 - timeCount) <= oneStar)
        {
            starGet = 1;
        }
        else
        {
            starGet = 1;
        }

        panelWin.SetActive(true);

        winpanel = panelWin.GetComponent<WinPanel>();
        winpanel.showHidePanel(starGet);

        GameData.instance.levelStates[GameData.difficulty][GameData.instance.cLevel] = 1;
        PlayerPrefs.SetInt("linkdot_" + GameData.difficulty + "_" + GameData.instance.cLevel, 1);
        if (PlayerPrefs.GetInt("linkdot_" + GameData.difficulty + "_" + GameData.instance.cLevel) < starGet)
        {
            PlayerPrefs.SetInt("levelStar_" + GameData.difficulty + GameData.instance.cLevel, starGet);
        }

    }

    public GameObject wrongPanel, settingPanel;
    public void OnClickMenu()
    {
        //AudioManager.Instance.PlayButtonClip();
        GameData.instance.isBackToLevel = true;
        SceneManager.LoadScene("LevelScene");
    }

    public void ShowWrongPanel()
    {
        wrongPanel.SetActive(true);
        AudioManager.Instance.PlayWrongClip();
        wrongPanel.transform.localScale = Vector2.zero;
        wrongPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InCubic);
    }
    public void OnClickHideWrongPanel()
    {
        AudioManager.Instance.PlayButtonClip();
        wrongPanel.transform.DOScale(Vector3.zero, .4f);
        GameData.instance.isWrong = false;
        StartCoroutine(Delay());
    }

    public void OnClickSettingBtn()
    {
        AudioManager.Instance.PlayButtonClip();
        settingPanel.SetActive(true);
        GameData.instance.isPause = true;
    }

    public void OnClickHideSettingBtn()
    {
        AudioManager.Instance.PlayButtonClip();
        settingPanel.SetActive(false);
        GameData.instance.isPause = false;
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(.4f);
        wrongPanel.SetActive(false);
    }

    public void OnClickBgMusicOn()
    {
        AudioManager.Instance.PlayButtonClip();
        PlayerPrefs.SetInt("muteBackground", 1);
        AudioManager.Instance.bgAudioSource.mute = true;
        bgOn.SetActive(false);
        bgOff.SetActive(true);
    }

    public void OnClickBgMusicOff()
    {
        AudioManager.Instance.PlayButtonClip();
        PlayerPrefs.SetInt("muteBackground", 0);
        AudioManager.Instance.bgAudioSource.mute = false;
        bgOn.SetActive(true);
        bgOff.SetActive(false);
    }

    public void OnClickEffectMusicOn()
    {
        AudioManager.Instance.PlayButtonClip();
        PlayerPrefs.SetInt("muteEffect", 1);
        AudioManager.Instance.effectAudioSource.mute = true;
        effectOn.SetActive(false);
        effectOff.SetActive(true);
    }

    public void OnClickEffectMusicOff()
    {
        AudioManager.Instance.PlayButtonClip();
        PlayerPrefs.SetInt("muteEffect", 0);
        AudioManager.Instance.effectAudioSource.mute = false;
        effectOn.SetActive(true);
        effectOff.SetActive(false);
    }

    public IEnumerator Initial()
    {
        yield return new WaitForSeconds(0.1f);
        numberHint = 0;
        for (int i = 0; i < GameData.getInstance().dotPoses.Count; i++)
        {
            List<Position> path = new List<Position>();
            for (int j = 0; j < GameData.getInstance().dotPoses[i]["v"].Count; j++)
            {
                Position position = new Position();
                position.pos0 = GameData.getInstance().dotPoses[i]["v"][j]["x"];//
                position.pos1 = GameData.getInstance().dotPoses[i]["v"][j]["y"];//

                if (position.pos0 == null || position.pos0 == "") position.pos0 = "0";
                if (position.pos1 == null || position.pos1 == "") position.pos1 = "0";
                path.Add(position);
            }
            totalPath.Add(path);
        }

    }

    private struct Position
    {
        public string pos0;
        public string pos1;
    }

    List<List<Position>> totalPath = new List<List<Position>>();

    int numberHint = 0;
    public void OnClickHintBtn()
    {
        if (numberHint >= totalPath.Count)
        {
            numberHint = 0;
        }
        AudioManager.Instance.PlayButtonClip();
        for (int t = 0; t < totalPath[numberHint].Count - 1; t++)
        {
            int x = Int32.Parse(totalPath[numberHint][t + 1].pos0) - Int32.Parse(totalPath[numberHint][t].pos0);
            int y = Int32.Parse(totalPath[numberHint][t + 1].pos1) - Int32.Parse(totalPath[numberHint][t].pos1);
            GameObject tlink = null;
            switch (x)
            {
                case -1:
                    tlink = GameObject.Find("linkl" + totalPath[numberHint][t].pos0 + "_" + totalPath[numberHint][t].pos1);
                    break;
                case 1:
                    tlink = GameObject.Find("linkr" + totalPath[numberHint][t].pos0 + "_" + totalPath[numberHint][t].pos1);
                    break;
                default:
                    break;
            }

            switch (y)
            {
                case -1:
                    tlink = GameObject.Find("linkd" + totalPath[numberHint][t].pos0 + "_" + totalPath[numberHint][t].pos1);
                    break;
                case 1:
                    tlink = GameObject.Find("linku" + totalPath[numberHint][t].pos0 + "_" + totalPath[numberHint][t].pos1);
                    break;
                default:
                    break;
            }

            if (tlink != null)
            {
                SpriteRenderer tSprite = tlink.GetComponent<SpriteRenderer>();
                tSprite.color = GameData.instance.colors[numberHint + 1];
                tSprite.DOFade(0, 1f);
            }
        }
        if (numberHint < totalPath.Count)
        {
            numberHint++;
        }
    }
}

