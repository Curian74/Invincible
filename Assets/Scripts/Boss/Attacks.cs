using UnityEngine;

public class Attacks : MonoBehaviour
{
    private Animator _animator;
	private float _castCoolDown = 0f;
	public float castCooldownTime = 2f; 
	void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
		if (_castCoolDown > 0)
		{
			_castCoolDown -= Time.deltaTime;
			_animator.SetFloat("CastCoolDown", _castCoolDown);
		}
	}


    public void CastSpell()
    {
		if (_castCoolDown <= 0) 
		{
			_animator.SetTrigger("CastSpell");
			_castCoolDown = castCooldownTime; 
			_animator.SetFloat("CastCoolDown", _castCoolDown);
		}
	}
}
