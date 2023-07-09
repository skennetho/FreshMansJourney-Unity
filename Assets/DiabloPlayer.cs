using UnityEngine;
using UnityEngine.Events;

public class DiabloPlayer : MonoBehaviour
{
    public const int MAX_LEVEL = 10;
    public const int MAX_HEALTH = 100;

    public UnityEvent OnMaxLevel = new();
    public UnityEvent OnDeath = new();

    public UnityEvent<int> OnLevelUpdate = new();
    public UnityEvent<int, int> OnHealthUpdate = new();
    public UnityEvent<int, int> OnExpChange = new();

    public int Level = 1;
    public int MaxLevel = MAX_LEVEL;
    public ParticleSystem LevelUpEffect;

    public int Health = MAX_HEALTH;
    public int Exp = 0;
    public int MaxExp = 0;

    [SerializeField] public Animator _animator;

    private void LevelUp()
    {
        if (Level < MaxLevel)
        {
            Level++;

            LevelUpEffect.Play();
            OnLevelUpdate.Invoke(Level);
            SetHeath(MAX_HEALTH);

            MaxExp = Level;
            Exp = 0;
            OnExpChange.Invoke(Exp, MaxExp);
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

    private void AddExp(int exp)
    {
        Exp += exp;
        if(Exp >= MaxExp)
        {
            LevelUp();
        }
        else
        {
            OnExpChange.Invoke(Exp, MaxExp);
        }
    }

    private void Death()
    {
        _animator.SetBool("bDeath", true);
    }

    public void PlayMoveAnim()
    {
        _animator.SetTrigger("tMove");
    }

    public void Attack(Monster monster)
    {
        _animator.SetTrigger("tAttack");
        monster.Die();
        AddExp(1);
    }

    public void FaceDirection(Direction direction)
    {
        if (direction == Direction.Left)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (direction == Direction.Right)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
