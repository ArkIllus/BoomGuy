using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //单例模式
    public static GameManager instance;

    private PlayerController player;

    private Door doorExit;

    public List<Enemy> enemies = new List<Enemy>();

    public bool gameOver;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        //有可能场景中没有门（或者玩家）
        //player = FindObjectOfType<PlayerController>();
        //doorExit = FindObjectOfType<Door>(); //只有一个门
    }

    public void Update()
    {
        //if (player != null)
        //开始界面没有给玩家赋予代码，也没有GameOver的Panel(菜单)
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            //Debug.Log("SceneManager.GetActiveScene().buildIndex != 0");
            gameOver = player.isDead;
            UIManager.instance.GameOverUI(gameOver);
        }
    }

    public void RestartScene()
    {
        //(重新)加载当前（Active）场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
        //重新开始 玩家满血
        PlayerPrefs.DeleteKey("playerHealth"); //TODO:看广告才满血
    }

    public void NewGame()
    {
        //清楚PlayerPrefs数据
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(1);
    }

    public void ContinueGame()
    {
        //如果没有存档则 = 新的游戏
        if (PlayerPrefs.HasKey("nextSceneIndex"))
            SceneManager.LoadScene(PlayerPrefs.GetInt("nextSceneIndex"));
        else
            NewGame();
    }

    //仅在真实设备上有效
    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        //原本如果按暂停回到主菜单，时间会被暂停，直到按暂停菜单的Resume时间才会继续，我们不希望这样
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    //观察者模式
    public void GetPlayer(PlayerController controller)
    {
        player = controller;
    }
    //TODO:如果有多个门？
    public void GetDoorExit(Door door)
    {
        doorExit = door;
    }
    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }
    public void EnemyDead(Enemy enemy)
    {
        enemies.Remove(enemy);
        //TODO：2个敌人只死掉一个也会enemies.Count == 0 ？？？
        if (enemies.Count == 0) {
            SavaData();
            //允许关卡没有出口门
            if (doorExit != null)
                doorExit.OpenDoor();
        }
    }

    //过关时保存PlayerPrefs数据
    public void SavaData()
    {
        PlayerPrefs.SetFloat("playerHealth", player.health);
        PlayerPrefs.SetInt("nextSceneIndex", SceneManager.GetActiveScene().buildIndex + 1);
        PlayerPrefs.Save();
    }

    //使用PlayerPrefs加载血量(如果是第一个scene则初始化血量)
    //在PlayerController的Start()中调用
    public float LoadHealth()
    {
        if (!PlayerPrefs.HasKey("playerHealth"))
        {
            PlayerPrefs.SetFloat("playerHealth", 3f);
        }

        float currentHealth = PlayerPrefs.GetFloat("playerHealth");

        return currentHealth;
    }

    //进入下一关(下一个场景)
    //最后一个scene如果有出口门，进门会报错
    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
