using UnityEngine;

namespace _Game.Scripts
{
    public class AnimatorManager : MonoBehaviour
    {
        // ToDo: Think of Required Field 
        public Animator animator;
        private int _vertical;
        private int _horizontal;
        private static readonly int MoveAmount = Animator.StringToHash("MoveAmount");

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void PlayTargetAnimation(string targetAnimation, bool isInteracting)
        {
            animator.CrossFade(targetAnimation, 0.2f);
        }

        public void UpdateAnimatorMovementValues(float moveAmount, bool isSprinting)
        {
            //ToDo: Animation Snapping 
            // if you have a value - walk and you cant walk or run (rounds Value)
            // it snaps to the appropriate Animation

            float snappedValue;

            #region snappedValue:

            if (moveAmount > 0f && moveAmount < 0.25f)
            {
                snappedValue = 0f;
            }
            else if (moveAmount > 0.25f && moveAmount < 0.75f)
            {
                snappedValue = .5f;
            }
            else if (moveAmount > 0.75f)
            {
                snappedValue = 1f;
            }

            else
            {
                snappedValue = 0;
            }

            #endregion


            if (isSprinting)
            {
                snappedValue *= 2;
            }

            animator.SetFloat(MoveAmount, snappedValue, .1f, Time.deltaTime);
        }
    }
}