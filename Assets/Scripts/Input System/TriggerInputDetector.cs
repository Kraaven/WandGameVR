using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using TMPro;
public class TriggerInputDetector : MonoBehaviour
    {
        private InputData _inputData;

        private bool LeftPrimaryDown;
        private bool LeftSecondaryDown;

        private bool RightPrimaryDown;
        private bool RightSecondaryDown;

        private Vector2 LeftJoystick;
        private Vector2 RightJoystick;

        private bool leftAflag;
        private bool leftBflag;
        private bool rightAflag;
        private bool rightBflag;
        


        private void Start()
        {
            _inputData = GetComponent<InputData>();
            Debug.Log("Started inputData: " + _inputData);

        }

        // Update is called once per frame
        void Update()
        {
            //To get the input from the controllers
            _inputData._leftController.TryGetFeatureValue(CommonUsages.primaryButton, out LeftPrimaryDown);
            _inputData._leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out LeftSecondaryDown);

            _inputData._rightController.TryGetFeatureValue(CommonUsages.primaryButton, out RightPrimaryDown);
            _inputData._rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out RightSecondaryDown);

            _inputData._leftController.TryGetFeatureValue(CommonUsages.primary2DAxis , out LeftJoystick);
            _inputData._rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out RightJoystick);
        }

        public bool GetLeftPrimary() => LeftPrimaryDown;
        public bool GetLeftSecondary() => LeftSecondaryDown;
        public bool GetRightPrimary() => RightPrimaryDown;
        public bool GetRightSecondary() => RightSecondaryDown;

        public Vector2 GetLeftJoyStick() => LeftJoystick;
        public Vector2 GetRightJoyStick() => RightJoystick;

        public bool GetLeftPrimaryDown()
        {
            if (LeftPrimaryDown)
            {
                if (!leftAflag)
                {
                    leftAflag = true;
                    return true;
                }
            }
            else
            {
                leftAflag = false;
            }

            return false;
        }

        public bool GetLeftSecondaryDown()
        {
            if (LeftSecondaryDown)
            {
                if (!leftBflag)
                {
                    leftBflag = true;
                    return true;
                }
            }
            else
            {
                leftBflag = false;
            }

            return false;
        }

        public bool GetRightPrimaryDown()
        {
            if (RightPrimaryDown)
            {
                if (!rightAflag)
                {
                    rightAflag = true;
                    return true;
                }
            }
            else
            {
                rightAflag = false;
            }

            return false;
        }

        public bool GetRightSecondaryDown()
        {
            if (RightSecondaryDown)
            {
                if (!rightBflag)
                {
                    rightBflag = true;
                    return true;
                }
            }
            else
            {
                rightBflag = false;
            }

            return false;
        }

    }

