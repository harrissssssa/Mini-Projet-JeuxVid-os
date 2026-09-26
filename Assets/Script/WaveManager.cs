using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName = "Vague 1";
        public GameObject[] enemyPrefabs;
        public int enemyCount = 5;
        public float spawnInterval = 1f;
    }

    [Header("Vagues")]
    public Wave[] waves;
    public Transform[] spawnPoints;

    [Header("Timing")]
    public float delayBeforeFirstWave = 2f;
    public float delayBetweenWaves = 5f;

    [Header("Niveau suivant")]
    [Tooltip("Nom exact de la scene a charger (doit etre ajoutee dans Build Settings). Laisse vide pour utiliser Next Level Build Index a la place.")]
    public string nextLevelName = "";
    [Tooltip("Utilise seulement si Next Level Name est vide. Index de la scene dans Build Settings.")]
    public int nextLevelBuildIndex = -1;
    [Tooltip("Delai avant de changer de niveau apres la derniere vague")]
    public float levelLoadDelay = 2f;

    private int currentWaveIndex = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        StartCoroutine(WaveRoutine());
    }

    IEnumerator WaveRoutine()
    {
        yield return new WaitForSeconds(delayBeforeFirstWave);

        while (currentWaveIndex < waves.Length)
        {
            Wave wave = waves[currentWaveIndex];
            Debug.Log("Début de la " + wave.waveName);

            yield return StartCoroutine(SpawnWave(wave));

            yield return new WaitUntil(() => AllEnemiesDead());

            Debug.Log(wave.waveName + " terminée !");

            currentWaveIndex++;

            if (currentWaveIndex < waves.Length)
            {
                yield return new WaitForSeconds(delayBetweenWaves);
            }
        }

        Debug.Log("Toutes les vagues sont terminées ! Changement de niveau...");
        yield return StartCoroutine(LoadNextLevelRoutine());
    }

    IEnumerator SpawnWave(Wave wave)
    {
        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy(wave);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    void SpawnEnemy(Wave wave)
    {
        if (wave.enemyPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        GameObject prefab = wave.enemyPrefabs[Random.Range(0, wave.enemyPrefabs.Length)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        activeEnemies.Add(enemy);
    }

    bool AllEnemiesDead()
    {
        activeEnemies.RemoveAll(e => e == null);
        return activeEnemies.Count == 0;
    }

    IEnumerator LoadNextLevelRoutine()
    {
        yield return new WaitForSeconds(levelLoadDelay);

        if (!string.IsNullOrEmpty(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
        }
        else if (nextLevelBuildIndex >= 0)
        {
            SceneManager.LoadScene(nextLevelBuildIndex);
        }
        else
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentIndex + 1);
        }
    }
}