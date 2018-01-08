using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikePointSpawner : MonoBehaviour
{

    public GameObject strikePointPrefab;
    public GameObject strikeFXPrefab;

    public float spawnRate = 1.0f; // in seconds
    public float minX = -0.5f;
    public float maxX = 0.5f;
    public float minY = -0.5f;
    public float maxY = 0.5f;
    public float zPosition = 0.5f;

    public int numberOfStrikes = 30;

    private GameObject lastSpawned;
    private int strikesRemaining;


    public void Start()
    {
        StartSpawning();
    }


    public void StartSpawning()
    {
        StartCoroutine(SpawnStrikePoint());
    }


    IEnumerator SpawnStrikePoint()
    {
        strikesRemaining = numberOfStrikes;

        while (strikesRemaining >= 0)
        {
            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
            Vector3 randPosition = new Vector3(x, y, zPosition);
            lastSpawned = Instantiate(strikePointPrefab, randPosition, Quaternion.identity);

            yield return new WaitForSeconds(spawnRate);
            if (lastSpawned)
            {
                Debug.Log("Player SKIPPED last StrikePoint");
                DestroyObject(lastSpawned);
                // progress bar penalty
            }
            strikesRemaining--;
        }
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject.CompareTag("StrikePoint"))
                {
                    PlayerStrike();
                }
                else
                {
                    Debug.Log("Player MISSED last StrikePoint");
                    // progress bar penalty
                }
            }
        }
    }


    public void PlayerStrike()
    {
        ShowSparks(lastSpawned.transform.position);
        Destroy(lastSpawned);
        lastSpawned = null;
        Debug.Log("Player HIT last StrikePoint");
        // no progress bar penalty
    }


    private void ShowSparks(Vector3 position)
    {
        GameObject fx = Instantiate(strikeFXPrefab, position, Quaternion.identity);
        Destroy(fx, 2.0f);
    }
}
