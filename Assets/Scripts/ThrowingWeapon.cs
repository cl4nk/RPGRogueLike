using UnityEngine;

public class ThrowingWeapon : MonoBehaviour
{
    public enum THROWINGWEAPONSTATES
    {
        Neutral = 0,
        Launch,
        NbStates
    }

    private Vector3 direction;
    [SerializeField] private float speed = 20f;
    private Character.Character thrower;
    private THROWINGWEAPONSTATES weaponState = THROWINGWEAPONSTATES.Neutral;

    public void Launch(Character.Character character, Vector3 dir)
    {
        transform.forward = dir;
        transform.RotateAround(transform.position, character.transform.right, 90f);

        direction = dir;
        thrower = character;
        weaponState = THROWINGWEAPONSTATES.Launch;
    }

    private void FixedUpdate()
    {
        if (weaponState == THROWINGWEAPONSTATES.Launch)
            transform.position += direction * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (weaponState == THROWINGWEAPONSTATES.Launch)
            if (collision.gameObject.layer == LayerMask.NameToLayer("Character") &&
                collision.gameObject.name != thrower.name)
            {
                Character.Character touchedCharac = collision.gameObject.GetComponent<Character.Character>();
                touchedCharac.UndergoAttack(thrower.MakeDamage(), thrower);
            }
            else if (collision.gameObject.tag == "Wall")
            {
                Destroy(gameObject);
            }
    }
}