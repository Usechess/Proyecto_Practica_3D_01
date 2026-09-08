using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private Vector2 respawnPointSize = new Vector2(1.0f, 1.0f);

    public int playerCurrentHealth;
    void Start()
    {
        playerCurrentHealth = maxHealth;
    }
    void Update()
    {
        if (playerCurrentHealth <= 0)
        {
            Debug.Log("DIED");
            Respawn();
        }
    }
    public void GetDamage(int damage)
    {
        playerCurrentHealth -= damage;
    }

    public void Respawn()
    {
        transform.position = respawnPoint.position;

        playerCurrentHealth = maxHealth;
        PlayerMovement playerRespawn = gameObject.GetComponent<PlayerMovement>();
        playerRespawn.RespawnData();
    }
}
