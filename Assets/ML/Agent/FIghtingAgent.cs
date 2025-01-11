using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections;
using System.Net.Mime;
using UnityEditor.Experimental.GraphView;

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
    [SerializeField] private SpriteRenderer m_sr;
    private Rigidbody2D m_rb;


    private float m_lastBlockTimer;
    private float m_endBlock;
    private float m_previousDistance;
    private int m_maxHealth;
    private bool m_isAttacking;
    private bool m_isRewardedForRange = false;
    private Color m_startingColor;
    private Vector2 m_startingPos;


    public override void Initialize()
    {
        base.Initialize();
        m_rb = GetComponent<Rigidbody2D>();
        m_startingPos = this.transform.position;
        m_startingColor = m_sr.color;
        m_maxHealth = m_health;
    }

    public override void OnEpisodeBegin()
    {
        transform.position = m_startingPos;
        m_rb.linearVelocity = Vector2.zero;
        m_isRewardedForRange = false;
        m_opponent.GetComponent<Dummy>().ResetDummy();

        Debug.Log(m_attackCooldown);
    }
    private void Start()
    {
        var behaviorParams = GetComponent<Unity.MLAgents.Policies.BehaviorParameters>();

    }

    public override void CollectObservations(VectorSensor _sensor)
    {
        _sensor.AddObservation(transform.position.x);
        _sensor.AddObservation(m_opponent.position.x);

        float distanceToOpponent = Vector2.Distance(transform.position, m_opponent.position);
        _sensor.AddObservation(distanceToOpponent);

        _sensor.AddObservation( m_isAttacking ? 0f : 1f);
    }

    private void FixedUpdate()
    {
        RequestDecision();
      
    }

    private IEnumerator AttackCooldown()
    {
        m_isAttacking = true;
        yield return new WaitForSeconds(m_attackCooldown);
        m_isAttacking = false;
    }

    public override void OnActionReceived(ActionBuffers _actions)
    {
        float moveX = _actions.ContinuousActions[0];

        m_rb.linearVelocity = new Vector2(moveX * m_movementSpeed, m_rb.linearVelocityY);

        int attackAction = _actions.DiscreteActions[0];

        float distanceToTarget = Vector2.Distance(transform.position, m_opponent.position);

        if (distanceToTarget <= m_attackRange)
        {
            if (!m_isRewardedForRange)
            {
                AddReward(1f);
                m_isRewardedForRange = true;
            }
        }
        else
        {
            m_isRewardedForRange = false;
            AddReward(-0.001f);
        }

        if (attackAction == 1)
        {
            if (distanceToTarget > m_attackRange)
            {
                AddReward(-1f);
                return;
            }

            if (!m_isAttacking)
            {

                Attack();

            }
            else
                AddReward(-.5f);
        }
    }

    private void Attack()
    {
        if (!m_isAttacking)
        {

            StartCoroutine(AttackCooldown());
            Dummy enemy = m_opponent.GetComponent<Dummy>();

            if (enemy != null)
            {
                enemy.RecieveDamage(m_attackDamage);
                AddReward(1f);


                if (enemy.IsDead)
                {
                    AddReward(5f);
                    EndEpisode();
                }
            }
            else
            {
                AddReward(-0.1f);
            }
        }
    }
}