using UnityEngine;

public class Dummy : MonoBehaviour
{

    [SerializeField] private float m_maxHealth;
    private float m_dummyHealth;
    public bool IsDead;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_dummyHealth = m_maxHealth;
    }

    public void ResetDummy()
    {
        IsDead = false;
        m_dummyHealth = m_maxHealth;
    }

    public void RecieveDamage(int _damage)
    {
        m_dummyHealth -= _damage;

        Debug.LogWarning("Dummy has taken Damage HP Left is " + m_dummyHealth);

        if (m_dummyHealth <= 0f)
        {
            IsDead = true;
            Debug.LogError("Dummy has been killed");
        }
    }

}
