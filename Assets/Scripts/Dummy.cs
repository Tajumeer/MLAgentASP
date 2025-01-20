using TMPro;
using UnityEngine;

public class Dummy : MonoBehaviour
{

    [SerializeField] private float m_maxHealth;
    private TextMeshPro m_HP_Text;
    private float m_dummyHealth;
    public bool IsDead;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_dummyHealth = m_maxHealth;
        m_HP_Text = GetComponent<TextMeshPro>();
        m_HP_Text.text = m_dummyHealth.ToString();
    }

    public void ResetDummy()
    {
        IsDead = false;
        m_dummyHealth = m_maxHealth;
    }

    public void RecieveDamage(int _damage)
    {
        m_dummyHealth -= _damage;
        m_HP_Text.text = m_dummyHealth.ToString();

        Debug.LogWarning("Dummy has taken Damage HP Left is " + m_dummyHealth);

        if (m_dummyHealth <= 0f)
        {
            IsDead = true;
            Debug.LogError("Dummy has been killed");
        }
    }

}
