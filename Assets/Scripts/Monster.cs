using System;
using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public Action OnDeath;
    [SerializeField] private Animator _animator;
    public int Damage = 20;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void Attack(DiabloPlayer player)
    {
        _animator.SetTrigger("tAttack");
        FacePlayer(player);
        player.GetDamaged(Damage);
    }

    public void Die()
    {
        _animator.SetTrigger("tDeath");
        StartCoroutine(DieCo());
    }

    private IEnumerator DieCo()
    {
        float dieAnimSeconds = 1.0f;
        yield return new WaitForSeconds(dieAnimSeconds);
        OnDeath?.Invoke();
        gameObject.SetActive(false);
    }

    public void FacePlayer(DiabloPlayer player)
    {
        var x = player.transform.localScale.x * -1;
        transform.localScale = new Vector3(x, 1, 1);
    }
}