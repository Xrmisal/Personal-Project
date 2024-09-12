using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private float powerupTimer = 10f, speedMultiplier = 1.5f;

    public int waveNum = 9, damageMultiplier = 2;
    public int enemyLim = 15, bossLim = 0, largeLim = 2, mediumLim = 8, smallLim = 10, enemiesRemaining;
    public int bossIncrease = 0, largeIncrease = 0, mediumIncrease = 0;

    public GameObject TitleUI, GameUI, OverUI, WinUI, OpenScreen, DifficultyScreen, PauseUI, TutUI;
    public TextMeshProUGUI WaveCount, Health, PowerupText, EnemyAmount;
    public PlayerController playerController;
    public SpawnManager spawnManager;
    public bool hasPowerup = false, gameOn = false, restartGame = false, isActive = false;
    public Camera mainCamera;
    public ParticleSystem smallEX, mediumEX, largeEX, BossEX;
    public AudioClip enemyDie, music2, music3, music4, music5;
    public Texture stage2, stage3, stage4, stage5;
    public Renderer backgroundRenderer;

    public AudioSource managerAudio, cameraAudio;

    private void Awake()
    {
        WinUI.SetActive(false);
        OverUI.SetActive(false);
        GameUI.SetActive(false);
        TitleUI.SetActive(true);
        DifficultyScreen.SetActive(false);
        OpenScreen.SetActive(true);
        managerAudio = GetComponent<AudioSource>();
        cameraAudio = mainCamera.GetComponent<AudioSource>();
        backgroundRenderer = GameObject.Find("Floor").GetComponent<Renderer>();
    }

    private void Update()
    {
        CheckWaveOver();
    }
    public void RandomSpawnBalancer()
    {
        enemyLim = 15 + waveNum - 1;
        if (waveNum % 5 == 0)
            bossIncrease += 2;
        if (waveNum % 2 == 0)
            largeIncrease += 2;
        if (waveNum % 4 == 0)
            mediumIncrease += 3;
        if (waveNum % 5 == 0 && spawnManager.spawnSpeed > 1.0f)
        {
            Debug.Log("Increasing spawn rate");
            spawnManager.spawnSpeed -= 0.2f;
        }
        smallLim = 10;
        largeLim = 2 + largeIncrease;
        mediumLim = 8 + mediumIncrease;
        bossLim = 0 + bossIncrease;
        
    }
    public void PauseGame()
    {
        Debug.Log("Pause game");
        if (Time.timeScale == 1.0f)
        {
            Time.timeScale = 0.0f;
            GameUI.SetActive(false);
            PauseUI.SetActive(true);
        }
    }
    public void ContinueGame()
    {
        GameUI.SetActive(true);
        PauseUI.SetActive(false);
        Time.timeScale = 1.0f;
    }
    public void UpdateRemainingEnemies()
    {
        EnemyAmount.text = ("Enemies: " + enemiesRemaining);
    }
    public void IncreaseWave()
    {
        if (waveNum < 25)
        {
            Debug.Log("Number increase");
            waveNum++;
            WaveCount.text = "Wave " + waveNum;
            Debug.Log("Moving to balancer");
            RandomSpawnBalancer();
        }
        enemiesRemaining = enemyLim;
        UpdateRemainingEnemies();
        switch (waveNum)
        {
            case (5):
                backgroundRenderer.material.SetTexture("_MainTex", stage2);
                cameraAudio.clip = music2;
                cameraAudio.Play();
                break;
            case (10):
                backgroundRenderer.material.SetTexture("_MainTex", stage3);
                cameraAudio.clip = music3;
                cameraAudio.Play();
                break;
            case (15):
                backgroundRenderer.material.SetTexture("_MainTex", stage4);
                cameraAudio.clip = music4;
                cameraAudio.Play();
                break;
            case (20):
                backgroundRenderer.material.SetTexture("_MainTex", stage5);
                cameraAudio.clip = music5;
                cameraAudio.Play();
                break;
        }
        if (waveNum == 25)
        {
            GameWin();
        }
    }

    public void ApplyPowerup(string powerType)
    {
        if (powerType.Contains("Health"))
        {
            playerController.Health += 20;
            if (playerController.Health > playerController.maxHealth)
                playerController.Health = playerController.maxHealth;
            AlterHealth();
        }
        else
        {
            playerController.powerupIndicator.SetActive(true);
            StartCoroutine(TimedPowerup(powerType));
        }
    }

    IEnumerator TimedPowerup(string powerType)
    {
 
        if (powerType.Contains("Speed"))
        {
            PowerupText.text = "Speed Boost";
            Debug.Log("Speed up");
            playerController.Speed *= speedMultiplier;
            yield return new WaitForSeconds(powerupTimer);
            PowerupText.text = "";
            playerController.powerupIndicator.SetActive(false);
            playerController.Speed /= speedMultiplier;
            Debug.Log("Speed normalised");
        }
        if (powerType.Contains("Damage"))
        {
            PowerupText.text = "Damage Boost";
            Debug.Log("Damage up");
            playerController.HitPower *= damageMultiplier;
            yield return new WaitForSeconds(powerupTimer);
            PowerupText.text = "";
            playerController.powerupIndicator.SetActive(false);
            playerController.HitPower /= damageMultiplier;
            Debug.Log("Damage normalised");
        }
    }
    public void StartOptions()
    {
        OpenScreen.SetActive(false);
        DifficultyScreen.SetActive(true);
    }
    public void GoBack()
    {
        DifficultyScreen.SetActive(false);
        OpenScreen.SetActive(true);
    }
    public void StartGame(int difficulty)
    {

        TitleUI.SetActive(false);
        GameUI.SetActive(true);
        spawnManager.CreatePlayer();
        enemiesRemaining = enemyLim;
        playerController = GameObject.Find("Player(Clone)").GetComponent<PlayerController>();
        playerController.gameManager = this;
        if (difficulty == 0)
        {
            spawnManager.spawnSpeed = 3.0f;
        }
        else if (difficulty == 1)
        {
            spawnManager.spawnSpeed = 2.4f;
            playerController.Health = 80;
            playerController.maxHealth = 80;
            powerupTimer = 5;
            AlterHealth();
        }
        else if (difficulty == 2)
        {
            spawnManager.spawnSpeed = 1.8f;
            playerController.Health = 50;
            playerController.maxHealth = 50;
            powerupTimer = 5;
            AlterHealth();
        } 
        Health.text = playerController.Health.ToString();
        UpdateRemainingEnemies();
        isActive = true;
        StartCoroutine(spawnManager.SpawnPowerup());
        StartCoroutine(StartWave());
        StartCoroutine(HideTutText());
        cameraAudio.Play();
    }
    IEnumerator HideTutText()
    {
        yield return new WaitForSeconds(10);
        TutUI.SetActive(false);
    }
    IEnumerator StartWave()
    {
        Debug.Log("Starting waves");
        while (enemyLim != 0)
        {
            yield return new WaitForSeconds(spawnManager.spawnSpeed);
            spawnManager.CreateRandomEnemy();
        }
    }

    void CheckWaveOver()
    {
        if (enemyLim == 0 && spawnManager.spawnedEnemies == 0)
        {
            StopCoroutine(StartWave());
            if (playerController.Health < playerController.maxHealth)
            {
                playerController.Health += 20;
                if (playerController.Health > playerController.maxHealth)
                    playerController.Health = playerController.maxHealth;
            }
            AlterHealth();
            Debug.Log("Increasing wave");
            IncreaseWave();
            StartCoroutine(StartWave());
        }
    }
    void GameWin()
    {
        GameUI.SetActive(false);
        WinUI.SetActive(true);
        StopAllCoroutines();
        mainCamera.GetComponent<AudioSource>().Pause();
        isActive = false;
    }

    public void GameOver()
    {
        GameUI.SetActive(false);
        OverUI.SetActive(true);
        StopAllCoroutines();
        mainCamera.GetComponent<AudioSource>().Pause();
        isActive = false;
    }
    
    public void AlterHealth()
    {
        Health.text = playerController.Health.ToString();
    }
    public void EndGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1.0f;
    }
    public void DeathExplosion(GameObject location)
    {
        managerAudio.PlayOneShot(enemyDie, 0.1f);
        if (location.name.Contains("Chick"))
        {
            Debug.Log("Small explosion");
            smallEX.transform.position = location.transform.position;
            smallEX.Play();
        }
        if (location.name.Contains("Rooster"))
        {
            Debug.Log("Medium explosion");
            mediumEX.transform.position = location.transform.position;
            mediumEX.Play();
        }
        if (location.name.Contains("Cow"))
        {
            Debug.Log("Large explosion");
            largeEX.transform.position = location.transform.position;
            largeEX.Play();
        }
        if (location.name.Contains("Horse"))
        {
            Debug.Log("Boss explosion");
            BossEX.transform.position = location.transform.position;
            BossEX.Play();
        }
    }
}
