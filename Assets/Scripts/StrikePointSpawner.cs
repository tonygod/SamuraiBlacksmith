using System.Collections;
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
    [Header("Sounds")]
    public AudioClip missSound;
    public AudioClip hitSound;

    [Space]
    [Header("Parameters")]
    public int numberOfStrikes = 30;

    [Space]
    [Header("UI References")]
    public ProgressBar progressBar;
    public Text hitText;
    public Text missText;
    public GameObject scrollPanel;
    public Image scrollImage;
    public GameObject closeButton1;
    public GameObject closeButton2;
    public GameObject customerPanel;
    public Text customerText;
    public Text qualityText;

    [Space]
    [Header("Customer (script-driven)")]
    public Customer customer;
    public Weapon weapon;

    private GameObject thisWeapon;
    private Image customerImage;

    private bool gameStarted = false;
    private GameObject lastSpawned;
    private int strikesRemaining;
    private int hits;
    private int misses;
    private int skips;

    private enum weaponStages { phase1, phase2, phase3 }

    private weaponStages weaponStage;
    private CustomerSelector cSel;
    private AudioSource sounds;

    private void Awake()
    {
        cSel = GetComponent<CustomerSelector>();
        customerImage = customerPanel.GetComponent<Image>();
        scrollPanel.SetActive(false);
        customerPanel.SetActive(false);
        progressBar.gameObject.SetActive(false);
        sounds = GetComponent<AudioSource>();
    }


    private void Start()
    {
        CustomerEnter();
    }


    public void CustomerEnter()
    {
        progressBar.ResetProgressBar();
        progressBar.gameObject.SetActive(false);
        customer = cSel.SelectNextCustomer();
        if ( customer == null )
        {
            SceneMgr.instance.LoadScene("1_Menu");
            return;
        }
        weapon = customer.weapon;
        customerImage.sprite = customer.idleSprite;
        customerImage.color = Color.white;
        customerPanel.SetActive(true);
        //customerPanel.GetComponent<Fade>().Reset();
        //customerPanel.GetComponent<Fade>().FadeOut();
        Invoke("ShowScroll", 3);
    }


    private void ShowScroll()
    {
        customerPanel.SetActive(false);
        qualityText.gameObject.SetActive(false);
        scrollImage.sprite = customer.scrollSprite;
        customerText.text = customer.needWeaponText;
        closeButton1.SetActive(true);
        closeButton2.SetActive(false);
        scrollPanel.SetActive(true);
    }

    public void StartGame()
    {
        progressBar.gameObject.SetActive(true);
        progressBar.StartProgressBar();
        StartNewWeapon();
        StartSpawning();
        gameStarted = true;
    }

    private void StartNewWeapon()
    {
        thisWeapon = Instantiate(weapon.stage1, weaponTransform.position, weaponTransform.rotation);
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

        while (strikesRemaining > 0)
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
                Destroy(thisWeapon);
                weaponStage = weaponStages.phase3;
                thisWeapon = Instantiate(weapon.stage3, weaponTransform.position, weaponTransform.rotation);
            }
            else if ( pctLeft < 66f && weaponStage == weaponStages.phase1 )
            {
                Destroy(thisWeapon);
                weaponStage = weaponStages.phase2;
                thisWeapon = Instantiate(weapon.stage2, weaponTransform.position, weaponTransform.rotation);
            }
        }

        EndGame();
    }


    private void EndGame()
    {
        gameStarted = false;
        EvaluateQuality();
    }

    private void EvaluateQuality()
    {
        float quality = Mathf.Round((float)hits / (float)numberOfStrikes * 100);
        Debug.Log("Final weapon quality: " + quality);
        qualityText.text = "Quality: " + quality + "%";
        qualityText.gameObject.SetActive(true);
        if (quality >= 70)
            customerText.text = customer.goodText;
        else if (quality >= 40)
            customerText.text = customer.okText;
        else
            customerText.text = customer.badText;

        thisWeapon.SetActive(false);
        closeButton1.SetActive(false);
        closeButton2.SetActive(true);
        scrollPanel.SetActive(true);
    }


    void Update()
    {
        if (gameStarted == false)
            return;
        
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
                    sounds.clip = missSound;
                    sounds.Play();
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
        sounds.clip = hitSound;
        sounds.Play();
    }


    private void ShowSparks(Vector3 position)
    {
        GameObject fx = Instantiate(strikeFXPrefab, position, Quaternion.identity);
        Destroy(fx, 2.0f);
    }
}
