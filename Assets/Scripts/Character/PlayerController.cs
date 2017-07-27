using System;
using Item;
using Managers;
using Spells;
using UI;
using UnityEngine;

namespace Character
{
    [Serializable]
    public class PlayerController : MonoBehaviour
    {
        private readonly float jumpSpeed = 6f;

        private readonly float smooth = 200f;

        private readonly float speed = 10f;
        private float centralMovement;

        private Vector3 gravity = Vector3.zero;

        private bool jump;
        private float lateralMovement;

        private bool leftHandIsRaise;

        // Move to script for MecAnim
        private Transform rightHandAnchor, leftHandAnchor;

        public PlayerController()
        {
            IsMoving = false;
        }

        public Player Player { get; private set; }

        public bool IsMoving { get; private set; }

        public Equipment Equipment { get; private set; }

        public PlayerInventory Inventory { get; private set; }

        private void Awake()
        {
            GameManager.Instance.PlayerInstance = this;
        }

        private void Start()
        {
            RegisterInputFunctions();

            Equipment = GetComponent<Equipment>();
            Inventory = GetComponent<PlayerInventory>();

            Player = GetComponent<Player>();
            rightHandAnchor = transform.Find("CamHolder/PlayerCamera/RightHand");
            leftHandAnchor = transform.Find("CamHolder/PlayerCamera/LeftHand");

            InitEquipment();

            Equipment.onEquipmentChange += LoadEquipment;

            UIGame.Instance.lockControl += UnregisterControlFunctions;
            UIGame.Instance.unlockControl += RegisterControlFunctions;
        }

        private void LoadEquipment(WeaponItemData weapon)
        {
            if (weapon.weaponType == WeaponItemData.WEAPON_TYPE.WEAPON)
            {
                if (rightHandAnchor.childCount != 0)
                    Destroy(rightHandAnchor.GetChild(0).gameObject);

                if (Equipment.RightWeapon != null)
                {
                    GameObject item = Instantiate(Equipment.RightWeapon.prefab);
                    item.transform.SetParent(rightHandAnchor);
                    item.transform.localPosition = Vector3.zero;
                    item.transform.localRotation = Quaternion.identity * Quaternion.Euler(new Vector3(90, 0, 0));
                }
            }

            else if (weapon.weaponType == WeaponItemData.WEAPON_TYPE.SHIELD)
            {
                if (leftHandAnchor.childCount != 0)
                    Destroy(leftHandAnchor.GetChild(0).gameObject);

                if (Equipment.LeftWeapon != null)
                {
                    GameObject item = Instantiate(Equipment.LeftWeapon.prefab);
                    item.transform.SetParent(leftHandAnchor);
                    item.transform.localPosition = Vector3.zero;
                    item.transform.localRotation = Quaternion.identity;
                }
            }
        }

        private void InitEquipment()
        {
            if (Equipment.LeftWeapon != null)
            {
                GameObject item = Instantiate(Equipment.LeftWeapon.prefab);
                item.transform.SetParent(leftHandAnchor);
                item.transform.localPosition = Vector3.zero;
            }

            if (Equipment.RightWeapon != null)
            {
                GameObject item = Instantiate(Equipment.RightWeapon.prefab);
                item.transform.SetParent(rightHandAnchor);
                item.transform.localPosition = Vector3.zero;
            }
        }


        private void RegisterInputFunctions()
        {
            InputManager input = InputManager.Instance;

            input.leftHandIsDown += RaiseLeftHand;
            input.leftHandIsUp += GetDownLeftHand;
            input.rightHandIsDown += RaiseRightHand;
            input.spellIsDown += LaunchSpell;
            input.inventoryIsDown += ShowInventory;
            input.jumpIsDown += Jump;

            input.selectSpell1IsDown += SelectSpell1;
            input.selectSpell2IsDown += SelectSpell2;
            input.selectSpell3IsDown += SelectSpell3;
            input.selectSpell4IsDown += SelectSpell4;
            input.selectSpell5IsDown += SelectSpell5;
            input.selectSpell6IsDown += SelectSpell6;
            input.selectSpell7IsDown += SelectSpell7;
            input.selectSpell8IsDown += SelectSpell8;
            input.selectSpell9IsDown += SelectSpell9;

            input.central += SetCentralMovement;
            input.lateral += SetLateralMovement;
            input.horizontal += SetRotationX;
        }

        private void UnregisterInputFunctions()
        {
            InputManager input = InputManager.Instance;

            input.leftHandIsDown -= RaiseLeftHand;
            input.leftHandIsUp -= GetDownLeftHand;
            input.rightHandIsDown -= RaiseRightHand;
            input.spellIsDown -= LaunchSpell;
            input.inventoryIsDown -= ShowInventory;
            input.jumpIsDown -= Jump;

            input.selectSpell1IsDown -= SelectSpell1;
            input.selectSpell2IsDown -= SelectSpell2;
            input.selectSpell3IsDown -= SelectSpell3;
            input.selectSpell4IsDown -= SelectSpell4;
            input.selectSpell5IsDown -= SelectSpell5;
            input.selectSpell6IsDown -= SelectSpell6;
            input.selectSpell7IsDown -= SelectSpell7;
            input.selectSpell8IsDown -= SelectSpell8;
            input.selectSpell9IsDown -= SelectSpell9;

            input.central -= SetCentralMovement;
            input.lateral -= SetLateralMovement;
            input.horizontal -= SetRotationX;
        }

        private void RegisterControlFunctions()
        {
            InputManager input = InputManager.Instance;

            input.leftHandIsDown += RaiseLeftHand;
            input.leftHandIsUp += GetDownLeftHand;
            input.rightHandIsDown += RaiseRightHand;
            input.spellIsDown += LaunchSpell;
            input.jumpIsDown += Jump;

            input.selectSpell1IsDown += SelectSpell1;
            input.selectSpell2IsDown += SelectSpell2;
            input.selectSpell3IsDown += SelectSpell3;
            input.selectSpell4IsDown += SelectSpell4;
            input.selectSpell5IsDown += SelectSpell5;
            input.selectSpell6IsDown += SelectSpell6;
            input.selectSpell7IsDown += SelectSpell7;
            input.selectSpell8IsDown += SelectSpell8;
            input.selectSpell9IsDown += SelectSpell9;

            input.central += SetCentralMovement;
            input.lateral += SetLateralMovement;
            input.horizontal += SetRotationX;
        }

        public void UnregisterControlFunctions()
        {
            InputManager input = InputManager.Instance;

            input.leftHandIsDown -= RaiseLeftHand;
            input.leftHandIsUp -= GetDownLeftHand;
            input.rightHandIsDown -= RaiseRightHand;
            input.spellIsDown -= LaunchSpell;
            input.jumpIsDown -= Jump;

            input.selectSpell1IsDown -= SelectSpell1;
            input.selectSpell2IsDown -= SelectSpell2;
            input.selectSpell3IsDown -= SelectSpell3;
            input.selectSpell4IsDown -= SelectSpell4;
            input.selectSpell5IsDown -= SelectSpell5;
            input.selectSpell6IsDown -= SelectSpell6;
            input.selectSpell7IsDown -= SelectSpell7;
            input.selectSpell8IsDown -= SelectSpell8;
            input.selectSpell9IsDown -= SelectSpell9;

            input.central -= SetCentralMovement;
            input.lateral -= SetLateralMovement;
            input.horizontal -= SetRotationX;
        }

        private void FixedUpdate()
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                CheckIsMoving();

                Vector3 moveDirection = transform.TransformDirection(new Vector3(lateralMovement, 0, centralMovement));

                moveDirection *= speed;

                if (!GetComponent<CharacterController>().isGrounded)
                {
                    gravity += Physics.gravity * Time.deltaTime;
                }
                else
                {
                    gravity = Vector3.zero;

                    if (jump)
                    {
                        gravity.y = jumpSpeed;
                        jump = false;
                    }
                }

                moveDirection += gravity;

                GetComponent<CharacterController>().Move(moveDirection * Time.deltaTime);
            }
            else
            {
                IsMoving = false;
            }
        }

        private void CheckIsMoving()
        {
            if (centralMovement == 0 && lateralMovement == 0)
                IsMoving = false;
            else
                IsMoving = true;
        }

        private void SetCentralMovement(float axis)
        {
            centralMovement = axis;
        }

        private void SetLateralMovement(float axis)
        {
            lateralMovement = axis;
        }

        private void SetRotationX(float axis)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                float rotationY = transform.localEulerAngles.y + axis * smooth * Time.deltaTime;
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotationY,
                    transform.localEulerAngles.z);
            }
        }

        private void RaiseLeftHand()
        {
            if (Player.State == Character.STATES.Neutral && Equipment.LeftWeapon != null)
                if (leftHandAnchor.GetChild(0).tag == "Weapon" && !leftHandIsRaise)
                {
                    Player.State = Character.STATES.LiftedShield;
                    GetDownRightHand();
                    leftHandAnchor.GetComponent<Animation>().Play("LeftHandRaiseShield");
                    leftHandIsRaise = true;
                }
        }

        private void GetDownLeftHand()
        {
            if (Equipment.LeftWeapon != null && leftHandIsRaise)
            {
                Player.State = Character.STATES.Neutral;
                leftHandAnchor.GetComponent<Animation>().Play("LeftHandGetDownShield");
                leftHandIsRaise = false;
            }
        }

        private void RaiseRightHand()
        {
            if (Player.State == Character.STATES.Neutral && Equipment.RightWeapon != null)
                if (rightHandAnchor.GetChild(0).tag == "Weapon" || rightHandAnchor.GetChild(0).tag == "ThrowingKnife")
                {
                    if (rightHandAnchor.GetChild(0).tag == "Weapon")
                        Player.Attack();
                    else if (rightHandAnchor.GetChild(0).tag == "ThrowingKnife")
                        Player.LaunchWeapon(rightHandAnchor.GetChild(0).gameObject);

                    StartCoroutine(Player.CouldownAttack());
                    GetDownLeftHand();
                    rightHandAnchor.GetComponent<Animation>().Play();
                }
        }

        private void GetDownRightHand()
        {
        }

        private void LaunchSpell()
        {
            GetComponent<SpellsLauncher>().UseSpell();
        }

        private void ShowInventory()
        {
            UIGame.Instance.ToggleInventoryWindow(Inventory);
        }

        private void Jump()
        {
            jump = true;
        }

        private void Crouch()
        {
            // TODO Crouch
        }

        private void Stand()
        {
            // TODO Stand
        }

        private void SelectSpell1()
        {
            // TODO SelectSpell1
            GetComponent<SpellsLauncher>().SetCurrentSpell(0);
        }

        private void SelectSpell2()
        {
            // TODO SelectSpell2
            GetComponent<SpellsLauncher>().SetCurrentSpell(1);
        }

        private void SelectSpell3()
        {
            // TODO SelectSpell3
            GetComponent<SpellsLauncher>().SetCurrentSpell(2);
        }

        private void SelectSpell4()
        {
            // TODO SelectSpell4
            GetComponent<SpellsLauncher>().SetCurrentSpell(3);
        }

        private void SelectSpell5()
        {
            // TODO SelectSpell5
            GetComponent<SpellsLauncher>().SetCurrentSpell(4);
        }

        private void SelectSpell6()
        {
            // TODO SelectSpell6
            GetComponent<SpellsLauncher>().SetCurrentSpell(5);
        }

        private void SelectSpell7()
        {
            // TODO SelectSpell7
            GetComponent<SpellsLauncher>().SetCurrentSpell(6);
        }

        private void SelectSpell8()
        {
            // TODO SelectSpell8
            GetComponent<SpellsLauncher>().SetCurrentSpell(7);
        }

        private void SelectSpell9()
        {
            // TODO SelectSpell9
            GetComponent<SpellsLauncher>().SetCurrentSpell(8);
        }
    }
}