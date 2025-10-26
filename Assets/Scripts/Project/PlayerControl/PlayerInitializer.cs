using UnityEngine;
using Project.PlayerControl.Movement;
using System;
using System.Collections;

namespace Project.PlayerControl
{
    /// <summary>
    /// For now just simply intialize at awake
    /// </summary>
    public sealed class PlayerInitializer : MonoBehaviour
    {
        [SerializeField] InputSystem.InputSystemEventStorage inputSystemEventStorage;
        [SerializeField] PlayerInputEventIndices playerInputEventIndices;
        [SerializeField] PhysicsMaterial2D noFrictionPhysicsMaterial;

        [Header("Player Movement")]
        [SerializeField] PlayerMovementStats playerMovementStats;
        [SerializeField] ScriptableMovementHandlerFactory movementHandlerFactory;

        [Header("Player Anim Hashes")]
        [SerializeField] string horizontalDirectionHash = "HorizontalDirection";
        [SerializeField] string verticalDirectionHash = "VerticalDirection";
        [SerializeField] string jumpTriggerHash = "JumpTrigger";
        [SerializeField] RuntimeAnimatorController playerAnimatorController;

        private PlayerInput_InputSystem m_playerInput;
        private PlayerMovementController m_playerController;

        void Awake()
        {
            m_playerInput = new PlayerInput_InputSystem(
                eventIndices: playerInputEventIndices,
                converter: new PlayerSafeInputConverter(
                    wrapper: new PlayerInputAxisConverter()
                )
            );

        }

        IEnumerator Start()
        {
            yield return InitializePlayerController(new GameObject("PlayerController"));
        }

        IEnumerator InitializePlayerController(GameObject playerController)
        {

            // set up physics & movement
            Collider2D collider = playerController.AddComponent<CapsuleCollider2D>();
            collider.sharedMaterial = noFrictionPhysicsMaterial;

            // Rigidbody2D rigidbody = playerController.AddComponent<Rigidbody2D>();
            // rigidbody.freezeRotation = true;
            // rigidbody.gravityScale = 0f;
            // rigidbody.mass = 0f;

            // GameObject playerFeet = new GameObject("PlayerFeet");
            // GameObject playerHead = new GameObject("PlayerHead");
            // yield return null;
            // playerFeet.transform.SetParent(playerController.transform);
            // playerHead.transform.SetParent(playerController.transform);

            // IPlayerMovementHandler playerMovementHandler = new Player2DMovementHandler(null);

            IPlayerMovementHandler playerMovementHandler = movementHandlerFactory.CreateMovementHandler(playerMovementStats, playerController.transform);

            // set up rendering component
            SpriteRenderer renderer = playerController.AddComponent<SpriteRenderer>();

            Animator animator = playerController.AddComponent<Animator>();
            animator.runtimeAnimatorController = playerAnimatorController;


            Player2DAnimatorHandler.PlayerAnimatorHash playerAnimatorHash = new()
            {
                HorizontalDirection = Animator.StringToHash(horizontalDirectionHash),
                VerticalDirection = Animator.StringToHash(verticalDirectionHash),
                JumpTrigger = Animator.StringToHash(jumpTriggerHash),
            };
            IPlayerRenderHandler playerRenderHandler = new Player2DAnimatorHandler(animator, playerAnimatorHash, renderer);
            yield return null;
            // finalize
            m_playerController = playerController.AddComponent<PlayerMovementController>();
            m_playerController.Initialize(m_playerInput, playerMovementHandler, playerRenderHandler);
        }

        void OnEnable()
        {
            m_playerInput.OnRegisteredTo(inputSystemEventStorage);
        }

        void OnDisable()
        {
            m_playerInput.OnUnregisteredTo(inputSystemEventStorage);
        }
    }
}