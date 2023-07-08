using UnityEngine;
using UnityEngine.Events;

public class DiabloPlayer : MonoBehaviour
{
    public const int MAX_LEVEL = 20;
    public const int MAX_HEALTH = 100;

    public UnityEvent OnMaxLevel = new();
    public UnityEvent OnDeath = new();

    public UnityEvent<int> OnLevelUpdate = new();
    public UnityEvent<int> OnHealthUpdate = new();

    public int Level = 1;
    public int MaxLevel = MAX_LEVEL;
    public ParticleSystem LevelUpEffect;

    public int Health = MAX_HEALTH;

    [SerializeField] public Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();    
    }

    public void LevelUp()
    {
        if (Level < MaxLevel)
        {
            Level++;
            Health = MAX_HEALTH;

            LevelUpEffect.Play();
            OnLevelUpdate.Invoke(Level);
            OnHealthUpdate.Invoke(Health);
        }
        else if (Level == MaxLevel)
        {
            Debug.Log("Max level reached");
            OnMaxLevel.Invoke();
        }
    }

    public void GetDamaged(int damage)
    {
        Health = Mathf.Clamp(Health - damage, 0, Health);
        OnHealthUpdate.Invoke(Health);
        if (Health <= 0)
        {
            Death();
            OnDeath.Invoke();
        }
        _animator.SetTrigger("tHit");
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
