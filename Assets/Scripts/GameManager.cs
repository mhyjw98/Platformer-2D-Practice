using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public StageManager stageManager;
    public GameObject[] stages;

    public AudioClip audioDamaged;
    public AudioClip audioDie;

    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject UIRestartBtn;
    public GameObject UIRetryBtn;
    public GameObject GameOver;
    public GameObject GameClear;
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        stageManager = FindObjectOfType<StageManager>();

        foreach (GameObject stage in stages)
        {
            stage.SetActive(false);
        }

        // 필요한 첫 번째 스테이지를 활성화
        stages[0].SetActive(true);
    }

    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage()
    {
        // Change Stage
        if (stageIndex < stages.Length-1)
        {
            stages[stageIndex].SetActive(false);
            stageIndex++;
            stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE " + (stageIndex + 1);
            health = 3;
            UIhealth[1].color = new Color(1, 1, 1, 1);
            UIhealth[2].color = new Color(1, 1, 1, 1);
        }
        else
        {
            // Game Clear
            Time.timeScale = 0;

            GameClear.SetActive(true);

            UIRestartBtn.SetActive(true);
            Text btnText = UIRestartBtn.GetComponentInChildren<Text>();
        }
        // Calculate Point
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if(health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 1, 1, 0.4f);

            audioSource.clip = audioDamaged;
            audioSource.Play();
        }
        else
        {
            UIhealth[0].color = new Color(1, 1, 1, 0.4f);

            player.OnDie();

            Debug.Log("죽었습니다!");

            audioSource.clip = audioDie;
            audioSource.Play();

            GameOver.SetActive(true);
            UIRetryBtn.SetActive(true);
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            health = 0;
            player.OnDie();

            for (int i = 0; i < UIhealth.Length; i++)
                UIhealth[i].color = new Color(1, 1, 1, 0.4f);

            Debug.Log("죽었습니다!");

            audioSource.clip = audioDie;
            audioSource.Play();

            GameOver.SetActive(true);
            UIRetryBtn.SetActive(true);
        }       
    }

    public void PlayerReposition()
    {
        player.transform.position = new Vector3(0, 0, -1);
        player.VelocityZero();
    }

    public void Retry()
    {
        player.Resurrection();
        GameOver.SetActive(false);
        UIRetryBtn.SetActive(false);
        health = 3;
        stagePoint = 0;

        UIhealth[0].color = new Color(1, 1, 1, 1);
        UIhealth[1].color = new Color(1, 1, 1, 1);
        UIhealth[2].color = new Color(1, 1, 1, 1);

        // 비활성화된 아이템 적들 활성화
        if (stageManager != null)
        {
            stageManager.ResetItemsAndEnemies();
        }
        else
        {
            Debug.LogError("StageManager를 찾을 수 없습니다.");
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
