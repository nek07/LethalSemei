// using UnityEngine;
// using Mirror;
//
// [RequireComponent(typeof(PredictedRigidbody))]
// [RequireComponent(typeof(Animator))]
// [RequireComponent(typeof(NetworkAnimator))]
// public class PlayerPredictionController : NetworkBehaviour
// {
//     [Header("Movement")]
//     public float speed = 5f;
//     public float jumpForce = 8f;
//
//     private Rigidbody rb;
//     private Animator animator;
//
//     private float inputX;
//     private float inputY;
//     private bool isJumping;
//
//     void Awake()
//     {
//         rb = GetComponent<Rigidbody>();
//         animator = GetComponent<Animator>();
//     }
//
//     void FixedUpdate()
//     {
//         if (!isLocalPlayer) return;
//
//         inputX = Input.GetAxis("Horizontal");
//         inputY = Input.GetAxis("Vertical");
//
//         Vector3 input = new Vector3(inputX, 0, inputY).normalized;
//         Vector3 move = input * speed;
//
//         // Движение
//         rb.MovePosition(rb.position + move * Time.fixedDeltaTime);
//
//         // Прыжок (с простой проверкой на "в воздухе")
//         if (Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(rb.velocity.y) < 0.1f)
//         {
//             rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
//             isJumping = true;
//         }
//
//         // Сброс прыжка, когда упали
//         if (rb.velocity.y < -0.1f && isJumping)
//         {
//             isJumping = false;
//         }
//
//         UpdateAnimation();
//     }
//
//     void UpdateAnimation()
//     {
//         // 🧠 Передаём значения в Animator — NetworkAnimator сам рассылает
//         animator.SetFloat("inputX", inputX);
//         animator.SetFloat("inputY", inputY);
//         animator.SetBool("IsJumping", isJumping);
//
//         if (Input.GetMouseButtonDown(0))
//         {
//             animator.SetTrigger("Attack"); // работает через NetworkAnimator
//         }
//     }
// }