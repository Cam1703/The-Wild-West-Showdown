using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator; // Animator del jugador
    [SerializeField] private Animator enemyAnimator;  // Animator del enemigo
    [SerializeField] private SpriteRenderer playerSpriteRenderer; // SpriteRenderer del jugador
    [SerializeField] private SpriteRenderer enemySpriteRenderer;  // SpriteRenderer del enemigo

    //Singleton
    public static AnimationManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Funciones para las animaciones del jugador
    public void PlayPlayerIdle()
    {
        if (playerAnimator != null)
        {
            playerSpriteRenderer.flipX = true;
            playerAnimator.SetTrigger("Player_Idle");
        }
    }

    public void PlayPlayerShoot()
    {
        if (playerAnimator != null)
        {
            // Cambia el sprite del jugador a la posición de disparo
            playerSpriteRenderer.flipX = false;
            SFXManager.Instance.PlaySound("shot");
            playerAnimator.SetTrigger("Player_Shoot");
            playerAnimator.SetTrigger("Player_Idle");
        }
    }

    public void PlayPlayerDamaged()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Player_Damaged");
        }
    }

    public void PlayPlayerDeath()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Player_Death");
        }
    }

    // Funciones para las animaciones del enemigo
    public void PlayEnemyIdle()
    {
        if (enemyAnimator != null)
        {
            enemySpriteRenderer.flipX = false;
            enemyAnimator.SetTrigger("Enemy_Idle");
        }
    }

    public void PlayEnemyShoot()
    {
        if (enemyAnimator != null)
        {
            enemySpriteRenderer.flipX = true;
            SFXManager.Instance.PlaySound("shot");
            enemyAnimator.SetTrigger("Enemy_Shoot");
            enemyAnimator.SetTrigger("Enemy_Idle");
        }
    }

    public void PlayEnemyDamaged()
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("Enemy_Damaged");
        }
    }

    public void PlayEnemyDeath()
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("Enemy_Death");
        }
    }

}