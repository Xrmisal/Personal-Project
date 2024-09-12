using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    SOActorModel actorModel; // Gets reference to player SO
    GameObject playerObject, enemyObject, powerup;  // Gets reference to player obj in scene
    public GameObject[] enemySpawns;
    public GameObject [] powerups;
    public GameManager gameManager;
    public float spawnSpeed = 3f;
    public int spawnedEnemies;
    private bool enemyChosen = false;

    [SerializeField]
    private string playerActorName;
    [SerializeField]
    private string[] enemyNames;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {

    }
    public void CreatePlayer() // Define default player
    {
        actorModel = Object.Instantiate(Resources.Load(playerActorName)) as SOActorModel;
        playerObject = GameObject.Instantiate(actorModel.actor);
        playerObject.transform.SetParent(this.transform);

        GameObject startPos = GameObject.Find("Starting Position");
        playerObject.transform.position = startPos.transform.position;
        playerObject.transform.rotation = startPos.transform.rotation;

        // Define player properties using actor template
        playerObject.GetComponent<IActorTemplate>().ActorStats(actorModel);

    }
    public void CreateRandomEnemy()
    {
        enemyChosen = false;
        Debug.Log("Creating enemy");
        int randomEnemy = 0;
        while (!enemyChosen)
        {
            int loops = 1;
            Debug.Log("Entering loop " + loops);
            randomEnemy = Random.Range(0, 4);
            switch (randomEnemy)
            {
                case (0):
                    if (gameManager.smallLim != 0)
                    {
                        gameManager.smallLim--;
                        enemyChosen = true;
                    }
                    break;
                case (1):
                    if (gameManager.mediumLim != 0)
                    {
                        gameManager.mediumLim--;
                        enemyChosen = true;
                    }
                    break;
                case (2):
                    if (gameManager.largeLim != 0)
                    {
                        gameManager.largeLim--;
                        enemyChosen = true;
                    }
                    break;
                case (3):
                    if (gameManager.bossLim != 0)
                    {
                        gameManager.bossLim--;
                        enemyChosen = true;
                    }
                    break;
            }
            loops++;
        }
        actorModel = Object.Instantiate(Resources.Load(enemyNames[randomEnemy])) as SOActorModel;
        enemyObject = Object.Instantiate(actorModel.actor);
        enemyObject.transform.SetParent(this.transform);
        
        int randomSpawn = Random.Range(0, 7);
        GameObject startPos = enemySpawns[randomSpawn];
        enemyObject.transform.position = startPos.transform.position;
        if (Random.Range(0, 2) == 1)
            enemyObject.transform.eulerAngles = new Vector3(0, 90);
        else
            enemyObject.transform.eulerAngles = new Vector3(0, -90);
        enemyObject.GetComponent<IActorTemplate>().ActorStats(actorModel);
        enemyObject.GetComponent<EnemyController>().spawnManager = this;
        enemyObject.GetComponent<EnemyController>().gameManager = gameManager;
        spawnedEnemies++;
        gameManager.enemyLim--;
    }

    public IEnumerator SpawnPowerup()
    {
        while (gameManager.isActive)
        {
            Debug.Log("Powerup spawned");
            int randomPowerup;
            if (gameManager.playerController.Health == gameManager.playerController.maxHealth)
                randomPowerup = Random.Range(1, 3);
            else
                randomPowerup = Random.Range(0, 3);
            powerup = Instantiate(powerups[randomPowerup]);
            Vector3 randomPos;
            randomPos = new Vector3 (Random.Range(-11f, 11f), 1f, Random.Range(-4f, 0.5f));
            powerup.transform.position = randomPos;
            yield return new WaitForSeconds(30);
        }
    }
}
