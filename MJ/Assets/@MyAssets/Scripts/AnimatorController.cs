using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    private Animator animator;

    public CharacterController xrCharacterController; // el del XR Origin
    public bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        if (xrCharacterController == null)
        {
            Debug.LogWarning("Asignar el CharacterController del XR Origin.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            animator.SetBool("IsDead", true);
            return;
        }

        if (xrCharacterController == null) return;

        Vector3 localVelocity = transform.InverseTransformDirection(xrCharacterController.velocity);

        float horizontal = localVelocity.x;
        float vertical = localVelocity.z;

        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);
        animator.SetFloat("speed", new Vector2(horizontal, vertical).magnitude);
    }

    public void Die()
    {
        isDead = true;
    }
}
