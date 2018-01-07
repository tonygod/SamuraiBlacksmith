using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeWeapon : MonoBehaviour {

    void Update()
    {
        if ( Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                string hitName = hit.collider.gameObject.name;
                if (hitName == "StrikePoint")
                {
                    Debug.Log("Hit StrikePoint");
                    Debug.DrawLine(ray.origin, hit.point);
                }
            }
        }
    }
}
