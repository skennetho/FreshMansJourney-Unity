using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Attack()
    {
        _animator.Play("Attack");
    }

    public void Die()
    {
        StartCoroutine(DieCo());
    }

    private IEnumerator DieCo()
    {
        float dieAnimSeconds = 1.0f;
        yield return new WaitForSeconds(dieAnimSeconds);
    }
}