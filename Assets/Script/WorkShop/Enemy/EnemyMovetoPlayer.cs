using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovetoPlayer : Enemy
{
    [Header("Detection Settings")]
    [SerializeField] private float normalDetectionRange = 10f;
    [SerializeField] private float crouchDetectionRange = 5f; 
    [SerializeField] private float attackRange = 1.5f;

    private Player playerScript; 

    private void Update()
    {
        if (player == null)
        {
            animator.SetBool("Attack", false);
            return;
        }

       
        if (playerScript == null)
        {
            playerScript = player.GetComponent<Player>();
            if (playerScript == null)
            {
                
                Debug.LogError("Enemy cannot find the 'Player' script on the player object!");
                return;
            }
        }
        
        float currentDetectionRange = normalDetectionRange;
        if (playerScript.IsCrouching) 
        {
            currentDetectionRange = crouchDetectionRange;
        }
        float distanceToPlayer = GetDistanPlayer(); 
        if (distanceToPlayer <= currentDetectionRange)
        {  
            Turn(player.transform.position - transform.position);
            timer -= Time.deltaTime;

            if (distanceToPlayer < attackRange) 
            {
                Attack(player);
            }
            else
            {
                animator.SetBool("Attack", false);
                Vector3 direction = (player.transform.position - transform.position).normalized;
                Move(direction);
            }
        }
        else
        {
            animator.SetBool("Attack", false);
            Move(Vector3.zero); 
        }
    }
} 