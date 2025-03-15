using UnityEngine;

public class Attacks : MonoBehaviour
{
    private Animator _animator;
	private float _castCoolDown = 0f;
	private float _meleeCoolDown = 0f;
	public float castCooldownTime = 2f; 
	public float meleeCooldownTime = 2f; 
	void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
		if (_castCoolDown > 0)
		{
			_castCoolDown -= Time.deltaTime;
			_animator.SetFloat("CastCooldown", _castCoolDown);
		}
		if (_meleeCoolDown > 0)
		{
			_meleeCoolDown -= Time.deltaTime;
			_animator.SetFloat("MeleeCooldown", _meleeCoolDown);
		}
	}


    public void CastSpell()
    {
		if (_castCoolDown <= 0) 
		{
			_animator.SetTrigger("CastSpell");
			_castCoolDown = castCooldownTime; 
			_animator.SetFloat("CastCooldown", _castCoolDown);
		}
	}

	public void MelleAttack()
	{
		if (_meleeCoolDown <= 0)
		{
			_animator.SetTrigger("MeleeAttack");
			_meleeCoolDown = meleeCooldownTime;
			_animator.SetFloat("MeleeCooldown", meleeCooldownTime);
		}
	}
}
