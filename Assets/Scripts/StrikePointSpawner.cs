﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrikePointSpawner : MonoBehaviour
{

    [Header("Prefabs")]
    public GameObject strikePointPrefab;
    public GameObject strikeFXPrefab;

    [Header("Weapon")]
    public Transform weaponTransform;
    public GameObject weaponStage1;
    public GameObject weaponStage2;
    public GameObject weaponStage3;

    [Space]
    [Header("Spawn Rates")]
    public float spawnRate1 = 1.5f;
    public float spawnRate2 = 1.0f;
    public float spawnRate3 = 0.75f;

    [Space]
    [Header("Spawn Boundaries")]
    public float minX = -0.5f;
    public float maxX = 0.5f;
    public float minZ = -0.5f;
    public float maxZ = 0.5f;
    public float yPosition = 0.5f;

    [Space]
    public int numberOfStrikes = 30;
    public ProgressBar progressBar;

    public Text hitText;
    public Text missText;

    private GameObject weapon;

    private GameObject lastSpawned;
    private int strikesRemaining;

    private int hits;
    private int misses;
    private int skips;

    private enum weaponStages { phase1, phase2, phase3 }

    private weaponStages weaponStage;

    public void Start()
    {
        StartNewWeapon();
        StartSpawning();
    }

    private void StartNewWeapon()
    {
        weapon = Instantiate(weaponStage1, weaponTransform.position, weaponTransform.rotation);
    }


    public void StartSpawning()
    {
        hits = 0;
        misses = 0;
        skips = 0;

        hitText.text = hits.ToString();
        missText.text = misses.ToString();

        weaponStage = weaponStages.phase1;
        float strikes = numberOfStrikes / 3;
        float totalTime = spawnRate1 * strikes + spawnRate2 * strikes + spawnRate3 * strikes;
        Debug.Log("totalTime=" + totalTime);
        progressBar.rate = totalTime;
        StartCoroutine(SpawnStrikePoint());
    }


    IEnumerator SpawnStrikePoint()
    {
        strikesRemaining = numberOfStrikes;
        float spawnRate = spawnRate1;

        while (strikesRemaining >= 0)
        {
            float x = Random.Range(minX, maxX);
            float z = Random.Range(minZ, maxZ);
            Vector3 randPosition = new Vector3(x, yPosition, z);
            lastSpawned = Instantiate(strikePointPrefab, randPosition, Quaternion.identity);

            if (weaponStage == weaponStages.phase2)
                spawnRate = spawnRate2;
            else if (weaponStage == weaponStages.phase3)
                spawnRate = spawnRate3;
            yield return new WaitForSeconds(spawnRate);

            if (lastSpawned)
            {
                Debug.Log("Player SKIPPED last StrikePoint");
                DestroyObject(lastSpawned);
                skips++;
                misses++;
                missText.text = misses.ToString();
                // progress bar penalty
            }
            strikesRemaining--;
            float pctLeft = ((float)strikesRemaining / (float)numberOfStrikes) * 100f;
            Debug.Log("pctLeft=" + pctLeft);
            if ( pctLeft < 33f && weaponStage == weaponStages.phase2 )
            {
                Destroy(weapon);
                weaponStage = weaponStages.phase3;
                weapon = Instantiate(weaponStage3, weaponTransform.position, weaponTransform.rotation);
            }
            else if ( pctLeft < 66f && weaponStage == weaponStages.phase1 )
            {
                Destroy(weapon);
                weaponStage = weaponStages.phase2;
                weapon = Instantiate(weaponStage2, weaponTransform.position, weaponTransform.rotation);
            }
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
                    misses++;
                    missText.text = misses.ToString();
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
        hits++;
        hitText.text = hits.ToString();

    }


    private void ShowSparks(Vector3 position)
    {
        GameObject fx = Instantiate(strikeFXPrefab, position, Quaternion.identity);
        Destroy(fx, 2.0f);
    }
}
