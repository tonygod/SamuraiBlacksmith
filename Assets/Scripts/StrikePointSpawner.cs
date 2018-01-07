﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikePointSpawner : MonoBehaviour
{

    public GameObject strikePointPrefab;

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

        while (strikesRemaining > 0)
        {
            yield return new WaitForSeconds(spawnRate);
            if (lastSpawned)
            {
                Debug.Log("Player missed last StrikePoint");
                DestroyObject(lastSpawned);
            }

            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
            Vector3 randPosition = new Vector3(x, y, zPosition);
            lastSpawned = Instantiate(strikePointPrefab, randPosition, Quaternion.identity);
            strikesRemaining--;
        }
    }
}