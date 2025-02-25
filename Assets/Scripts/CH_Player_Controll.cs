using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CH_Player_Controll : MonoBehaviour
{

    
    public List<AudioSource> sound;

    [HideInInspector] public Transform knifeSpawnPoint;
    [HideInInspector] public GameObject circleObject;
    [HideInInspector] public GameObject backround;

    [SerializeField] private GameObject knifePrefab;
    [SerializeField] private float bgRotationSpeed;
    [SerializeField] private float force = 10f;
    [SerializeField] private GameObject Player;
    [SerializeField] private float Speed = 1f;
    [SerializeField] private TextMeshProUGUI[] scores;
    [SerializeField] private List<ParticleSystem> circleParticle;
    [SerializeField] private List<GameObject> circlePart;     
    [SerializeField] private int maxKnives = 6;
    [SerializeField] private Color[] circleColors = { Color.red, Color.blue };     
    [SerializeField] private GameObject knifeUIImagePrefab;
    [SerializeField] private Transform knifeUIParent;
    [SerializeField] private GameObject GameOverFace;

    private List<Image> knifeUIElements;
    private List<GameObject> knives;
    private float startingSpeed = 100f; 
    private float speedIncrement = 10f; 
    private float maxSpeed = 200f;
    private CH_Controls controls;
   // private bool isStart = false;  
    private CH_UI_Controller uiController; 
    private int knivesThrown = 0;
    private int currentKnifeIndex = 0;    
    private List<Color> activeCircleColors;   
    private GameObject lastActivatedFace;
    private GameObject lastActivatedMobi;
    private float spawnchance = 1f;
    private int roundCounter = 0;
    private int SpawnDelay = 3;
    private bool isGameOver = false;


    [Header("GameOverScores")]
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private TextMeshProUGUI gameOverMobiScoreText;

    


    [SerializeField] private Transform cameraTransform;
    [HideInInspector] public float shakeDuration = 0.2f;
    [HideInInspector] public float shakeMagnitude = 0.1f;

    

   
    private void Awake()
    {
        controls = new CH_Controls();
       // uiController = FindObjectOfType<CH_UI_Controller>();
      
        activeCircleColors = new List<Color>(); 
        
        ChangeColors();
        SpawnKnives();
        InitializeKnifeUI();
        SetKnifeUIColors();
    }

    private void Update()
    {
        /*if (!IsGamePanelActive())
        {
            return;
        }*/

        if (GameManager.Instance.GameState)
        {
            Vector3 rotation = circleObject.transform.localEulerAngles;
            rotation.z += Time.deltaTime * Speed;
            circleObject.transform.localEulerAngles = rotation;

            Vector3 bgrotation = backround.transform.localEulerAngles;
            bgrotation.z += Time.deltaTime * bgRotationSpeed;
            backround.transform.localEulerAngles = bgrotation;
        }
    }

   /* private bool IsGamePanelActive()
    {
        foreach (GameObject panel in uiController.UI_Panels)
        {
            if (panel.activeSelf && panel.name == "Game_Panel")
            {
               isStart = true;
                return true;
            }
        }

        return false;
    }*/

    public void Gamestart()
    {
       
        
    }
    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Touch.performed += OnTap;
    }

    private void OnDisable()
    {
        controls.Player.Touch.performed -= OnTap;
        controls.Disable();
    }

    public void OnTap(InputAction.CallbackContext context)
    {
        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        if (IsPointerOverUIObject(touchPosition))
        {

            return;
        }

        if (GameManager.Instance.GameState && knivesThrown < maxKnives)  
        {
            LaunchKnife();
        }
    }


    private bool IsPointerOverUIObject(Vector2 touchPosition)
    {

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = touchPosition;


        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);


        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("UIButton"))
            {
                return true;
            }
        }

        return false;
    }

    private void SpawnKnives()
    {
        knives = new List<GameObject>();
        Quaternion knifeRotation = Quaternion.Euler(180f, 0f, 0f);
        for (int i = 0; i < maxKnives; i++)
        {
            GameObject knife = Instantiate(knifePrefab, knifeSpawnPoint.position, knifeRotation, Player.transform);
            knife.SetActive(false);
            SetRandomKnifeColor(knife);
            knives.Add(knife);
        }

        if (knives.Count > 0)
        {
            knives[0].SetActive(true);
        }

        if (knives.Count > 1)
        {
            knives[knives.Count - 1].SetActive(false);
        }
    }

    private Color SetRandomKnifeColor(GameObject knife)
    {
       
        SpriteRenderer knifeRenderer = knife.GetComponent<SpriteRenderer>();
        if (knifeRenderer != null)
        {
            knifeRenderer.color = Color.white;
        }

        
        Color randomColor = Color.white;
        if (activeCircleColors.Count > 0)
        {
            randomColor = activeCircleColors[Random.Range(0, activeCircleColors.Count)];

            
            Transform knifeIconTransform = knife.transform.Find("KnifeIcon");
            if (knifeIconTransform != null)
            {
                SpriteRenderer knifeIconSpriteRenderer = knifeIconTransform.GetComponent<SpriteRenderer>();
                if (knifeIconSpriteRenderer != null)
                {
                    knifeIconSpriteRenderer.color = randomColor;

                    ParticleSystem knifeParticleSystem = knife.GetComponentInChildren<ParticleSystem>();
                    if (knifeParticleSystem != null)
                    {
                        var mainModule = knifeParticleSystem.main;
                        mainModule.startColor = randomColor;
                    }
                }
            }
        }

        return randomColor;
    }


    private void InitializeKnifeUI()
    {
        knifeUIElements = new List<Image>();
        for (int i = 0; i < maxKnives; i++)
        {
            GameObject knifeUI = Instantiate(knifeUIImagePrefab, knifeUIParent);
            Image knifeUIImage = knifeUI.GetComponent<Image>();
            knifeUIElements.Add(knifeUIImage);
        }
        UpdateKnifeUI();
    }
    private void SetKnifeUIColors()
    {
        for (int i = 0; i < knifeUIElements.Count; i++)
        {
            Color knifeColor = SetRandomKnifeColor(knives[i]);
            knifeUIElements[i].color = knifeColor;
        }
    }




    private void UpdateKnifeUI()
    {
        for (int i = 0; i < knifeUIElements.Count; i++)
        {
            if (i < currentKnifeIndex)
            {
                Color transparentColor = new Color(1f, 1f, 1f, 0f);
                knifeUIElements[i].color = transparentColor;
            }
            else
            {
                knifeUIElements[i].color = knives[i].transform.Find("KnifeIcon").GetComponent<SpriteRenderer>().color;
            }
        
        }
    }


    private void LaunchKnife()
    {
        if (currentKnifeIndex < knives.Count)
        {
            GameObject knife = knives[currentKnifeIndex];
            knife.SetActive(true);
            knife.transform.rotation = Quaternion.Euler(0, 0, 180);
            Vector2 direction = (circleObject.transform.position - knifeSpawnPoint.position).normalized;
            Rigidbody2D knifeRb = knife.GetComponent<Rigidbody2D>();
            knifeRb.linearVelocity = direction * force;

            knivesThrown++;

            UpdateKnifeUI();

            
            Color knifeColor = knife.transform.Find("KnifeIcon").GetComponent<SpriteRenderer>().color;
            knifeUIElements[currentKnifeIndex].color = knifeColor;

            StartCoroutine(WaitForKnifeToFinish(knife));
        }
    }

    private IEnumerator WaitForKnifeToFinish(GameObject knife)
    {
        Rigidbody2D knifeRb = knife.GetComponent<Rigidbody2D>();

       
        while (knifeRb.linearVelocity.magnitude > 0.1f)
        {
            yield return null;
        }

        currentKnifeIndex++;
        UpdateKnifeUI();

      
        if (currentKnifeIndex < knives.Count)
        {
            knives[currentKnifeIndex].SetActive(true);
        }

        if (currentKnifeIndex == maxKnives)
        {
            if (CheckAllKnivesThrownCorrectly())
            {
               
                foreach (var ps in circleParticle)
                {
                    ps.Play();
                }

                circleObject.SetActive(false);
                GameManager.Instance.GameState = false;
                sound[4].Play();
                StartCoroutine(DelayBeforeRestart(1f));
            }
            else
            {
               
                GameOver();
            }
        }
    }

    private bool CheckAllKnivesThrownCorrectly()
    {
        foreach (GameObject knife in knives)
        {
            Transform knifeIconTransform = knife.transform.Find("KnifeIcon");
            if (knifeIconTransform != null)
            {
                SpriteRenderer knifeIconSpriteRenderer = knifeIconTransform.GetComponent<SpriteRenderer>();
                if (knifeIconSpriteRenderer != null)
                {
                    Color knifeIconColor = knifeIconSpriteRenderer.color;

                    foreach (var circlePartObject in circlePart)
                    {
                        SpriteRenderer circlePartSpriteRenderer = circlePartObject.GetComponent<SpriteRenderer>();
                        if (circlePartSpriteRenderer != null)
                        {
                            if (circlePartSpriteRenderer.color == knifeIconColor)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }



    private IEnumerator DelayBeforeRestart(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.Instance.GameState = true;
        RestartGame();
        Speed = Mathf.Min(startingSpeed + speedIncrement, maxSpeed);
        startingSpeed = Speed;
        circleObject.SetActive(true);
        
    }

    private void ChangeColors()
    {
        activeCircleColors.Clear(); 
        Color randomColor;

        foreach (Transform child in circleObject.transform)
        {

  
                if (child.CompareTag("part")) 
            {
                 randomColor = circleColors[Random.Range(0, circleColors.Length)];
                randomColor.a = 1f;
                SpriteRenderer childRenderer = child.GetComponent<SpriteRenderer>();

                if (childRenderer != null)
                {
                  
                    childRenderer.color = randomColor;

                   
                    {
                        activeCircleColors.Add(randomColor);
                    }

                }
                UpdateParticleSystemColors();

            }

        }
        
    }
    private void UpdateParticleSystemColors()
    {
        
        if (activeCircleColors.Count > 0)
        {
            foreach (ParticleSystem particleSystem in circleParticle)
            {
                if (particleSystem != null)
                {
                    var mainModule = particleSystem.main;
                   
                    Color randomColor = activeCircleColors[Random.Range(0, activeCircleColors.Count)];
                    mainModule.startColor = randomColor;
                }
            }
        }
    }

    private IEnumerator DelayedGameOver(float delay)
    {

        yield return new WaitForSeconds(delay);
        ResetKnives();
       

        GameOverFace.SetActive(false);
        uiController.Open_UI(uiController.UI_Panels[3]);
       
    }

    public void GameOver()
    {
        if (isGameOver) return; 
        roundCounter = 0;
        isGameOver = true; 
       
        uiController.UI_Panels[1].SetActive(false);
        GameManager.Instance.GameState = false;
       
        GameOverScore();
        GameOverFace.SetActive(true);
        sound[2].Play();
        StartCoroutine(DelayedGameOver(1f));
    }

  

    public void RestartGame()
    {
        Speed = 100; 
        ChangeColors();
        ResetKnives();
        isGameOver = false;
        FindObjsInCircle();
    }

    public void GameRestart()
    {
     
        Reset_Scores();
    }

    private void ResetKnives()
    {
        foreach (GameObject knife in knives)
        {
          
            knife.SetActive(false);
            Rigidbody2D knifeRb = knife.GetComponent<Rigidbody2D>();
            knifeRb.linearVelocity = Vector2.zero;
            knifeRb.isKinematic = false;
            knife.transform.SetParent(Player.transform);
            knife.transform.position = knifeSpawnPoint.position;
            knife.transform.rotation = Quaternion.Euler(180f, 0f, 0f);
            SetRandomKnifeColor(knife);

            knife.GetComponent<CH_Knife>().ResetGravityScale();
            
        }
        knivesThrown = 0;
        currentKnifeIndex = 0;
        UpdateKnifeUI();

        if (knives.Count > 0)
        {
            knives[0].SetActive(true);
        }
    }

    private void FindObjsInCircle()
    {
        List<GameObject> enemies = new List<GameObject>();
        List<GameObject> faceObjects = new List<GameObject>();
        List<GameObject> mobiObj = new List<GameObject>();

        foreach (Transform child in circleObject.transform)
        {
            if (child.CompareTag("enemy"))
            {
                enemies.Add(child.gameObject);
            }
            if (child.CompareTag("face"))
            {
                faceObjects.Add(child.gameObject); 
            }
            if (child.CompareTag("Mobi"))
            {
                mobiObj.Add(child.gameObject);
            }
        }

        
        foreach (GameObject enemy in enemies)
        {
            enemy.SetActive(false);
        }

       
        for (int i = enemies.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            GameObject temp = enemies[i];
            enemies[i] = enemies[randomIndex];
            enemies[randomIndex] = temp;
        }

       
        int activeEnemyCount = Random.Range(2, 5);
        int activatedEnemies = 0;

        foreach (GameObject enemy in enemies)
        {
            if (activatedEnemies >= activeEnemyCount)
            {
                break;
            }

          
            if (Random.value > 0.8f)
            {
                enemy.SetActive(true);
                activatedEnemies++;
            }
        }

       
        if (lastActivatedFace != null)
        {
            lastActivatedFace.SetActive(false);
        }

     
        if (activeCircleColors.Count >= 4 && faceObjects.Count > 0)  
        {
            
            int randomIndex = Random.Range(0, faceObjects.Count);
            lastActivatedFace = faceObjects[randomIndex];
            lastActivatedFace.SetActive(true);
        }

        roundCounter++;

        
        if (roundCounter >= SpawnDelay)
        {
            roundCounter = 0; 

           
            if (lastActivatedMobi != null)
            {
                lastActivatedMobi.SetActive(false);
            }

           
            if (mobiObj.Count > 0 && Random.value <= spawnchance) 
            {
               
                int randomIndex = Random.Range(0, mobiObj.Count);
                lastActivatedMobi = mobiObj[randomIndex];
                lastActivatedMobi.SetActive(true);
            }
        }
        else
        {
           
            if (lastActivatedMobi != null)
            {
                lastActivatedMobi.SetActive(false);
                lastActivatedMobi = null; 
            }
        }
    }


    // Score
    public void AddScore()
    {
        scores[0].text = (int.Parse(scores[0].text) + 10).ToString();
    }

    public void AddMobiScore()
    {
        scores[1].text = (int.Parse(scores[1].text) + 1).ToString();
    }

    public void Reset_Scores()
    {
       
        scores[0].text = "0";
        scores[1].text = "0";
    }

    public void UpdateGameOverScores(string score, string mobiScore)
    {
        gameOverScoreText.text = score;
        gameOverMobiScoreText.text = mobiScore;
    }
    public void GameOverScore()
    {
        string currentScore = scores[0].text;
        string currentMobiScore = scores[1].text;

        UpdateGameOverScores(currentScore, currentMobiScore);

    }

    // camerashake
    public IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPosition = cameraTransform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            cameraTransform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;

            yield return null;
        }

        cameraTransform.localPosition = originalPosition;
    }
    public bool IsGameOver()
    {
        return isGameOver; 
    }

}
