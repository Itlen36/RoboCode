using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public GameManager gm;
    public GameObject RobotProcessopObg;
    private RobotProcessor _Processor;
    private Rigidbody2D _Rigidbody;
    private float _Speed, _TimeLastJump, TimeJump;
    private bool _OnGround, _Jump;
    void Start()
    {
        _Processor = RobotProcessopObg.GetComponent<RobotProcessor>();
        _Rigidbody = this.gameObject.GetComponent<Rigidbody2D>();
        _Speed = 2f;
        _OnGround = true;
        _Jump = false; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Let")
        {
            _Processor.Registers.Collision = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Let")
        {
            _Processor.Registers.Collision = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            _OnGround = true;
            if (_Jump)
            {
                _Processor.Registers.Play = true;
                _Jump = false;
            }
        } else if (collision.gameObject.tag == "Star")
        {
            gm.StarsCount++;
        } else if(collision.gameObject.tag == "Finish")
        {
            gm.Finished = true;
        }
    }

    void Update()
    {
        if (_Processor.Registers.Motion == 1)
            transform.position += Vector3.right * _Speed * Time.deltaTime;
        else if (_Processor.Registers.Motion == -1)
            _Rigidbody.velocity = Vector2.left * _Speed * Time.deltaTime;
        else if (_Processor.Registers.Motion == 2)
        {
            if (_OnGround)
            {
                _Jump = true;
                _Rigidbody.AddForce(new Vector2(20, 150), ForceMode2D.Impulse);
                _Processor.Registers.Motion = 0;
            }
        }
        else if (_Processor.Registers.Motion == -2)
        {
            if (_OnGround)
            {
                _Jump = true;
                _Rigidbody.AddForce(new Vector2(-20, 150), ForceMode2D.Impulse);
                _Processor.Registers.Motion = 0;
            }
        }
    }
}
