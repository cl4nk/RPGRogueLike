using UnityEngine;
using UnityEngine.UI;

namespace UI.Compass
{
    public class Point : MonoBehaviour
    {
        private float angleViewCompass;
        public int instanceID;
        private float maxValueCompass;
        public Transform objTransform;
        private float posXCompass;

        private float widthCompass;

        private float widthPoint;

        private void Start()
        {
            widthPoint = GetComponent<Graphic>().rectTransform.rect.width;

            widthCompass = transform.parent.GetComponent<Graphic>().rectTransform.rect.width;
            posXCompass = transform.parent.transform.localPosition.x;

            Compass compass = transform.parent.GetComponent<Compass>();
            angleViewCompass = compass.AngleViewCompass;
            maxValueCompass = compass.MaxValue;
        }

        public void UpdatePosition(float angle)
        {
            Vector3 pos = transform.localPosition;
            pos.x = angle * ((widthCompass - widthPoint) / 2) / (maxValueCompass - angleViewCompass / 2);
            transform.localPosition = pos;
        }

        public bool IsInsideCompass()
        {
            return posXCompass + widthCompass / 2 > transform.localPosition.x &&
                   transform.localPosition.x > posXCompass - widthCompass / 2;
        }
    }
}