using UnityEngine;

public class PlayerKillerOutOfBounds : MonoBehaviour
{
    private int damage = 10000000;
    void Start()
    {

    }
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            Debug.Log("I just got Hit: " + damage);
            health.GetDamage(damage);
        }
    }
}
