using UnityEngine;
using UnityEngine.Events;

public class DiabloPlayer : MonoBehaviour
{
    public const int MAX_LEVEL = 20;
    public const int MAX_HEALTH = 100;

    public UnityEvent OnMaxLevel = new();
    public UnityEvent OnDeath = new();

    public UnityEvent<int> OnLevelUpdate = new();
    public UnityEvent<int, int> OnHealthUpdate = new();

    public int Level = 1;
    public int MaxLevel = MAX_LEVEL;
    public ParticleSystem LevelUpEffect;

    public int Health = MAX_HEALTH;

    [SerializeField] public Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        ReferenceHolder.Request<DiabloUIManager>(Initialize);
    }

    private void Initialize(DiabloUIManager diabloUIManager)
    {
        var healthUI = diabloUIManager.PlayerHealthUI;
        OnHealthUpdate.AddListener(healthUI.SetHealth);
    }

    public void LevelUp()
    {
        if (Level < MaxLevel)
        {
            Level++;

            LevelUpEffect.Play();
            OnLevelUpdate.Invoke(Level);
            SetHeath(MAX_HEALTH);
        }
        else if (Level == MaxLevel)
        {
            Debug.Log("Max level reached");
            OnMaxLevel.Invoke();
        }
    }

    public void GetDamaged(int damage)
    {
        SetHeath(Health - damage);
        if (Health <= 0)
        {
            Death();
            OnDeath.Invoke();
        }
        _animator.SetTrigger("tHit");
    }

    private void SetHeath(int health)
    {
        Health = Mathf.Clamp(health, 0, MAX_HEALTH);
        OnHealthUpdate.Invoke(Health, MAX_HEALTH);
    }

    private void Death()
    {
        _animator.SetBool("bDeath", true);
    }

    public void PlayMoveAnim()
    {
        _animator.SetTrigger("tMove");
    }

    public void PlayAttackAnim()
    {
        _animator.SetTrigger("tAttack");
    }
}
