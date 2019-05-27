using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour
{
    public bool LowerPosition, TopPosition;
    private float _speed = 2f;
    public bool MoveUp, MoveDown;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("TopLiftTrigger"))
            TopPosition = true;
        else if (collision.gameObject.CompareTag("BottomLiftTrigger")) 
            LowerPosition = true;
    }
    
    void Update()
    {
        if (MoveUp && !TopPosition)
            this.transform.position += Vector3.up * _speed * Time.deltaTime;
        else if (MoveDown && !LowerPosition)
            this.transform.position += Vector3.down * _speed * Time.deltaTime;
    }
}
