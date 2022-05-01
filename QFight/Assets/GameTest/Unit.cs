using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MOVE_TYPE
{
    UNIT,
    POINT
}

namespace QFight
{
    public class Unit : MonoBehaviour
    {
        public float moveSpeed = 1f;
        private Vector3 target = Vector3.zero;
        private GameObject targetUnit;
        public CharacterController cc;

        public Animator animator;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (target != Vector3.zero)
            {
                MoveTo(target, MOVE_TYPE.POINT);
            }
            else if (targetUnit != null)
            {
                MoveTo(targetUnit);
            }

        }

        public void SetTargetPoint(Vector3 _target)
        {
            target = _target;
            targetUnit = null;

        }

        public void SetTargetUnit(GameObject _target)
        {
            target = Vector3.zero;
            targetUnit = _target;
        }

        void MoveTo(Vector3 _target, MOVE_TYPE moveType)
        {
            Vector3 offset = _target - transform.position;

            //Get the difference.
            if (offset.magnitude > .1f)
            {
                //If we're further away than .1 unit, move towards the target.
                //The minimum allowable tolerance varies with the speed of the object and the framerate. 
                // 2 * tolerance must be >= moveSpeed / framerate or the object will jump right over the stop.
                offset = offset.normalized * moveSpeed;
                //normalize it and account for movement speed.
                cc.Move(offset * Time.deltaTime);
                transform.rotation = Quaternion.LookRotation(offset * Time.deltaTime);
                //actually move the character.
                animator.SetInteger("state", 1);
            }
            else if (moveType == MOVE_TYPE.POINT)
            {
                animator.SetInteger("state", 0);
                SetTargetPoint(Vector3.zero);
            }
        }

        void MoveTo(GameObject _targetUnit)
        {
            Vector3 _target = _targetUnit.transform.position;
            Vector3 offset = _target - transform.position;

            MoveTo(_target, MOVE_TYPE.UNIT);
        }
    }
}


