using UnityEngine;

public class Dummy : MonoBehaviour
{

    [SerializeField] private float m_dummyHealth;
    private float m_maxHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_dummyHealth = m_maxHealth;
    }

    public void RecieveDamage(int _damage)
    {
        m_dummyHealth -= _damage;

        if (m_dummyHealth <= 0f)
        {
            Destroy(this);
        }
    }

}
