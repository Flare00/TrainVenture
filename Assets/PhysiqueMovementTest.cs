using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysiqueMovementTest : MonoBehaviour
{
    public float speed = 0.1f;
    public Vector3 min = new(-2, -2, -2);
    public Vector3 max = new(2, 2, 2);
    private Vector3 sens = new(1, 1, 1);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float val = speed * Time.deltaTime;
        Vector3 pos = transform.position + (sens * val);

        for (int i = 0; i < 3; i++)
        {
            if (pos[i] > max[i])
            {
                sens[i] = -1;
            } else if (pos[i] < min[i])
            {
                sens[i] = 1;
            }
        }

        transform.position = pos;
    }
}
