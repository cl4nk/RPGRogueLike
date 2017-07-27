using UnityEngine;

namespace Managers
{
    public class InputManager : MonoBehaviour
    {
        public delegate void InputAxis(float axis);

        public delegate void InputKey();

        public delegate void selecSpell(int indexSpell);


        private static InputManager instance;
        private InputKey inputLock;

        private bool isBroadcasting = true;

        private bool isLocked;

        private TimeManager timeMgr;

        public static InputManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<InputManager>();

                return instance;
            }
        }

        public bool IsBroadcasting
        {
            get { return isBroadcasting; }
            set { isBroadcasting = value; }
        }

        public event InputKey leftHandIsDown;
        public event InputKey leftHandIsUp;
        public event InputKey rightHandIsDown;
        public event InputKey rightHandIsUp;
        public event InputKey spellIsDown;
        public event InputKey spellIsUp;

        public event InputKey useIsDown;
        public event InputKey inventoryIsDown;
        public event InputKey shopIsDown;
        public event InputKey characIsDown;
        public event InputKey jumpIsDown;
        public event InputKey menuIsDown;

        public event InputKey selectSpell1IsDown;
        public event InputKey selectSpell2IsDown;
        public event InputKey selectSpell3IsDown;
        public event InputKey selectSpell4IsDown;
        public event InputKey selectSpell5IsDown;
        public event InputKey selectSpell6IsDown;
        public event InputKey selectSpell7IsDown;
        public event InputKey selectSpell8IsDown;
        public event InputKey selectSpell9IsDown;

        public event selecSpell selecSpellIsDown;

        // For player move
        public event InputAxis central;

        public event InputAxis lateral;

        // For camera move
        public event InputAxis vertical;

        public event InputAxis horizontal;

        private void Awake()
        {
            timeMgr = TimeManager.Instance;
        }

        private void Update()
        {
            if (isBroadcasting)
                CheckMenuInput();
        }

        private void FixedUpdate()
        {
            if (isBroadcasting)
                CheckInput();
        }

        public void LockAllInputs()
        {
            isLocked = true;
            inputLock = null;
        }

        private void LockInput(InputKey inputToLock)
        {
            isLocked = true;
            inputLock = inputToLock;
        }

        public void UnlockKey()
        {
            isLocked = false;
            inputLock = null;
        }

        public void LockInventory()
        {
            LockInput(inventoryIsDown);
            timeMgr.State = TimeManager.StateGame.Paused;
        }

        public void LockCharacMenu()
        {
            LockInput(characIsDown);
        }

        public int GetCountSubscribersUseIsDown()
        {
            if (useIsDown != null)
                return useIsDown.GetInvocationList().Length;

            return -1;
        }

        private void CheckInput()
        {
            if (!isLocked)
            {
                CheckMoveInput();
                CheckSpellInput();
                CheckCameraInput();
                CheckActionInput();
            }
        }

        private void CheckActionInput()
        {
            if (Input.GetButtonDown("LeftHand") && leftHandIsDown != null)
                leftHandIsDown();
            else if (Input.GetButtonUp("LeftHand") && leftHandIsUp != null)
                leftHandIsUp();

            if (Input.GetButtonDown("RightHand") && rightHandIsDown != null)
                rightHandIsDown();
            else if (Input.GetButtonUp("RightHand") && rightHandIsUp != null)
                rightHandIsUp();

            if (Input.GetButtonDown("LaunchSpell") && spellIsDown != null)
                spellIsDown();
            else if (Input.GetButtonUp("LaunchSpell") && spellIsUp != null)
                spellIsUp();

            if (Input.GetButtonDown("Use") && useIsDown != null)
                useIsDown();
            else if (Input.GetButtonDown("Jump") && jumpIsDown != null &&
                     GetComponent<CharacterController>().isGrounded)
                jumpIsDown();
        }

        private void CheckMenuInput()
        {
            if (isLocked)
            {
                if (Input.GetButtonDown("Inventory") && inventoryIsDown != null && inputLock == inventoryIsDown)
                {
                    inventoryIsDown();
                    UnlockKey();
                    timeMgr.State = TimeManager.StateGame.Running;
                }
                else if (Input.GetButtonDown("Shop") && shopIsDown != null && inputLock == inventoryIsDown)
                {
                    shopIsDown();
                }

                else if (Input.GetButtonDown("Characteristics") && characIsDown != null && inputLock == characIsDown)
                {
                    characIsDown();
                    UnlockKey();
                    timeMgr.State = TimeManager.StateGame.Running;
                }

                else if (Input.GetButtonDown("Menu") && menuIsDown != null && inputLock == menuIsDown)
                {
                    menuIsDown();
                    UnlockKey();
                    timeMgr.State = TimeManager.StateGame.Running;
                }
            }
            else
            {
                if (Input.GetButtonDown("Inventory") && inventoryIsDown != null)
                {
                    inventoryIsDown();
                    LockInput(inventoryIsDown);
                    timeMgr.State = TimeManager.StateGame.Paused;
                }
                else if (Input.GetButtonDown("Shop") && shopIsDown != null)
                {
                    shopIsDown();
                }

                else if (Input.GetButtonDown("Characteristics") && characIsDown != null)
                {
                    LockInput(characIsDown);
                    timeMgr.State = TimeManager.StateGame.Paused;
                    characIsDown();
                }

                else if (Input.GetButtonDown("Menu") && menuIsDown != null)
                {
                    menuIsDown();
                    LockInput(menuIsDown);
                    timeMgr.State = TimeManager.StateGame.Paused;
                }
            }
        }


        private void CheckMoveInput()
        {
            if (central != null)
                central(Input.GetAxis("Central"));
            if (lateral != null)
                lateral(Input.GetAxis("Lateral"));
        }

        private void CheckSpellInput()
        {
            if (Input.GetButtonDown("Spell1") && selectSpell1IsDown != null)
            {
                selectSpell1IsDown();
                selecSpellIsDown(0);
            }
            else if (Input.GetButtonDown("Spell2") && selectSpell2IsDown != null)
            {
                selectSpell2IsDown();
                selecSpellIsDown(1);
            }
            else if (Input.GetButtonDown("Spell3") && selectSpell3IsDown != null)
            {
                selectSpell3IsDown();
                selecSpellIsDown(2);
            }
            else if (Input.GetButtonDown("Spell4") && selectSpell4IsDown != null)
            {
                selectSpell4IsDown();
                selecSpellIsDown(3);
            }
            else if (Input.GetButtonDown("Spell5") && selectSpell5IsDown != null)
            {
                selectSpell5IsDown();
                selecSpellIsDown(4);
            }
            else if (Input.GetButtonDown("Spell6") && selectSpell6IsDown != null)
            {
                selectSpell6IsDown();
                selecSpellIsDown(5);
            }
            else if (Input.GetButtonDown("Spell7") && selectSpell7IsDown != null)
            {
                selectSpell7IsDown();
                selecSpellIsDown(6);
            }
            else if (Input.GetButtonDown("Spell8") && selectSpell8IsDown != null)
            {
                selectSpell8IsDown();
                selecSpellIsDown(7);
            }
            else if (Input.GetButtonDown("Spell9") && selectSpell9IsDown != null)
            {
                selectSpell9IsDown();
                selecSpellIsDown(8);
            }
        }

        private void CheckCameraInput()
        {
            if (horizontal != null)
                horizontal(Input.GetAxis("Mouse X"));
            if (vertical != null)
                vertical(Input.GetAxis("Mouse Y"));
        }
    }
}