using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public GameObject mainPerson;
    public Camera mainCamera;

    private bool pressed = false;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !pressed) {
            OnMouseDown();
            pressed = true;
        } else if (Input.GetMouseButtonUp(1)){
            pressed = false;
        }
    }

    void OnMouseDown()
    {
        Debug.Log("Clicked");


        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit raycastHit)) {
            mainPerson.GetComponent<QFight.Unit>().SetTargetPoint(raycastHit.point);
        }
    }
}
