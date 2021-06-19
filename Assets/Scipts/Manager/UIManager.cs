using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //单例模式
    public static UIManager instance;

    public GameObject healthBar; //在Unity中赋值

    [Header("UI Elements")]
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;
    public Slider bossHealthBar;

    public void Awake()
    {
        // ???
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject); //删除代码挂载的游戏物体
                                 //防止切换场景后，出现2个Manger

        //切换场景时Manager不被删除
        //DontDestroyOnLoad
    }

    public void UpdateHealth(float curHealth)
    {
        //3格血条
        int maxHealthBar = 3;

        for (int i = 0; i < Mathf.Min(curHealth, maxHealthBar); i++)
        {
            healthBar.transform.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = (int)curHealth; i < maxHealthBar; i++)
        {
            healthBar.transform.GetChild(i).gameObject.SetActive(false);
        }

        //switch的写法
        //switch (curHealth)
        //{
        //    case 3:
        //        healthBar.transform.GetChild(0).gameObject.SetActive(true);
        //        healthBar.transform.GetChild(1).gameObject.SetActive(true);
        //        healthBar.transform.GetChild(2).gameObject.SetActive(true);
        //        break;
        //    case 2:
        //        break;
        //    case 1:
        //        break;
        //    case 0:
        //        break;
        //}
    }

    //TODO:Boss出现时才亮血条
    public void SetBossHealthBar(float health)
    {
        bossHealthBar.maxValue = health;
    }

    public void UpdateBossHealthBar(float health)
    {
        bossHealthBar.value = health;
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);

        Time.timeScale = 0; //暂停时间 （0~1 可减缓时间）
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);

        Time.timeScale = 1;
    }

    public void GameOverUI(bool gameOver)
    {
        gameOverPanel.SetActive(gameOver);
    }
}
