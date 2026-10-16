using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Normal,
    Tense,
    Chase
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState currentState = GameState.Normal;

    public AudioManager audioManager;
    public LightManager lightManager;
    public EnemyAIFull enemyaifull;
    public EfectsManager efectsManager;

    private float sceneTimer = 0f;
    private bool tenseTriggered = false;


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        sceneTimer += Time.deltaTime;

        // 120 segundos = 2 minutos
        if (!tenseTriggered && sceneTimer >= 60f)
        {
            tenseTriggered = true;

            // solo pasa a Tense si NO estás en Chase
            if (currentState == GameState.Normal)
            {
                SetState(GameState.Tense);
            }
        }
    }

    public void SetState(GameState newState)
    {
        // si ya estás en Chase, no vuelvas a Tense
        if (currentState == GameState.Chase && newState == GameState.Tense)
            return;

        currentState = newState;

        switch (currentState)
        {
            case GameState.Normal:
                break;

            case GameState.Tense:
                audioManager.SetTenseMusic();

                lightManager.SetLightIntensity(0.6f);
                lightManager.StartFlickerTense();
                break;

            case GameState.Chase:
                audioManager.SetChaseMusic();
                
                efectsManager.SetGrito();
                efectsManager.SetPasos();

                lightManager.SetLightIntensity(0.3f);
                lightManager.StartFlickerChase();
                
                if (enemyaifull != null)
                {
                    enemyaifull.ActivarIA();
                }
                break;
        }
    }
}
