using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections;

public class FIghtingAgent : Agent
{
    [SerializeField] private float m_movementSpeed;
    [SerializeField] private int m_health;
    [SerializeField] private int m_attackDamage;
    [SerializeField] private float m_attackRange;
    [SerializeField] private float m_attackCooldown;
    [SerializeField] private float m_blockCooldown;
    [SerializeField] private float m_blockTimer;


    [SerializeField] private Transform m_opponent;
    [SerializeField] private Rigidbody2D m_rb;
    [SerializeField] private SpriteRenderer m_sr;

    private float m_lastAttackTimer;
    private float m_lastBlockTimer;
    private float m_endBlock;
    private int m_maxHealth;
    private bool m_isBlocking;
    private Color m_startingColor;


    public override void Initialize()
    {
        base.Initialize();
        m_startingColor = m_sr.color;
        m_maxHealth = m_health;
    }

    private void Start()
    {
        var behaviorParams = GetComponent<Unity.MLAgents.Policies.BehaviorParameters>();
        Debug.Log($"{gameObject.name} - Continuous Actions: {behaviorParams.BrainParameters.ActionSpec.NumContinuousActions}");
        Debug.Log($"{gameObject.name} - Discrete Actions: {behaviorParams.BrainParameters.ActionSpec.NumDiscreteActions}");
    }

    public override void CollectObservations(VectorSensor _sensor)
    {
        base.CollectObservations(_sensor);

        _sensor.AddObservation(transform.position.x - m_opponent.position.x);

        _sensor.AddObservation(m_health / m_maxHealth);
        _sensor.AddObservation(m_opponent.GetComponent<FIghtingAgent>().m_health / m_opponent.GetComponent<FIghtingAgent>().m_maxHealth);

        Debug.Log($"{gameObject.name} - Observations:");
    }

    private void FixedUpdate()
    {

        RequestDecision();
        //Vector2 rbVelo = m_rb.linearVelocity;
        //Debug.Log("RB Velocity: " + rbVelo);
    }

    public override void OnActionReceived(ActionBuffers _actions)
    {
        int attack = _actions.DiscreteActions[0];
        int block = _actions.DiscreteActions[1];

        float moveX = _actions.ContinuousActions[0];
        Debug.Log($"moveX Action Value: {moveX}");


        float distanceToOpponent = Mathf.Abs(transform.position.x - m_opponent.position.x);


        //  m_rb.linearVelocity = new Vector2(moveX * m_movementSpeed, m_rb.linearVelocityY);

        if (distanceToOpponent > m_attackRange)
        {
            m_rb.linearVelocity = new Vector2(moveX * m_movementSpeed, m_rb.linearVelocityY);
        }
        else
        {
            m_rb.linearVelocity = new Vector2(0, m_rb.linearVelocityY);

        }


        Debug.Log(moveX);

        if (attack == 1)
            Attack();
        if (block == 1)
            Block();

        if (distanceToOpponent <= m_attackRange)
        {
            AddReward(0.05f);
        }
        else
        {
            AddReward(-0.01f);
        }
    }

    private void Attack()
    {
        Debug.Log("Attack Called");

        if (Time.time - m_lastAttackTimer < m_attackCooldown)
        {
            return;
        }

        float distanceToOpponent = Vector2.Distance(transform.position, m_opponent.position);
        StartCoroutine(FlashColor(Color.yellow, 0.2f));

        if (distanceToOpponent <= m_attackRange)
        {


            FIghtingAgent opponentAgent = m_opponent.GetComponent<FIghtingAgent>();
            if (opponentAgent != null && opponentAgent.m_isBlocking == false)
            {
                opponentAgent.TakeDamage(m_attackDamage);
                AddReward(.1f);
            }
        }
        else
        {
            AddReward(-0.1f);
            Debug.Log($"{gameObject.name} tried to attack but missed!");
        }

        m_sr.color = m_startingColor;

        m_lastAttackTimer = Time.time;
    }

    private IEnumerator FlashColor(Color _flashColor, float _duration)
    {
        m_sr.color = _flashColor;

        yield return new WaitForSeconds(_duration);

        m_sr.color = m_startingColor;
    }

    private void Update()
    {
        if (m_isBlocking && Time.time >= m_endBlock)
        {
            m_isBlocking = false;
        }


    }

    private void Block()
    {

        Debug.Log("Block Called");

        if (Time.time - m_lastBlockTimer < m_blockCooldown)
        {
            return;
        }

        m_isBlocking = true;
        StartCoroutine(FlashColor(Color.blue, m_blockTimer));


        m_endBlock = Time.time + m_blockTimer;
    }

    public void TakeDamage(int _damage)
    {
        if (!m_isBlocking)
        {
            m_health -= _damage;
            StartCoroutine(FlashColor(Color.red, 0.2f));
            if (m_health <= 0)
            {
                Debug.Log($"{gameObject.name} is deafeated!");
                AddReward(-.1f);
                EndEpisode();
            }
        }
        if (m_isBlocking)
        {
            AddReward(.075f);
        }
    }
}
