using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private CharacterController controller;

    void Start()
    {
        // On s'assure d'avoir un CharacterController sur le joueur
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            gameObject.AddComponent<CharacterController>();
            controller = GetComponent<CharacterController>();
        }
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }
}
