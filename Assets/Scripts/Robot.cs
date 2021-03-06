﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public GameManager gm;
    public RobotProcessor Processor;
    [SerializeField] private GameObject _Sprite;
    private Rigidbody2D _Rigidbody;
    private float _Speed, _TimeLastJump, TimeJump;
    private bool _OnGround, _Jump, _LeftLiftTrigger, _RightLiftTrigger;
    private Animator _animator;
    private GameObject _Lift;
    void Start()
    {
        _animator = _Sprite.GetComponent<Animator>();
        _Rigidbody = this.gameObject.GetComponent<Rigidbody2D>();
        _Speed = 2f;
        _OnGround = true;
        _Jump = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Let"))
            Processor.Registers.Collision = true;
        else if (collision.gameObject.CompareTag("LeftLiftTrigger"))
            _LeftLiftTrigger = true;
        else if (collision.gameObject.CompareTag("RightLiftTrigger"))
        {
            _RightLiftTrigger = true;
            _Lift = collision.gameObject.transform.parent.gameObject;
        }
        else if (collision.gameObject.CompareTag("Star"))
        {
            Destroy(collision.gameObject);
            gm.StarsCount++;
        }
        else if (collision.gameObject.CompareTag("Finish"))
        {
            Processor.Registers.Play = false;
            Processor.Registers.Motion = 0;
            gm.Finished = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Let"))
            Processor.Registers.Collision = false;
        else if (collision.gameObject.CompareTag("LeftLiftTrigger"))
            _LeftLiftTrigger = false;
        else if (collision.gameObject.CompareTag("RightLiftTrigger"))
            _RightLiftTrigger = false;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            _OnGround = true;
            if (_Jump)
            {
                Processor.Registers.Play = true;
                _Jump = false;
            }
        } 
    }

    void Update()
    {
        if (_LeftLiftTrigger && _RightLiftTrigger)
            Processor.Registers.OnLift = true;
        else
            Processor.Registers.OnLift = false;

        if (Processor.Registers.Motion == 1)
        {
            _animator.SetInteger("State", 1);
            transform.position += Vector3.right * _Speed * Time.deltaTime;

        }
        else if (Processor.Registers.Motion == -1)
        {
            _animator.SetInteger("State", -1);
            transform.position += Vector3.left * _Speed * Time.deltaTime;
        }
        else if (Processor.Registers.Motion == 2)
        {
            if (_OnGround)
            {
                _Jump = true;
                _animator.SetInteger("State", 2);
                _Rigidbody.AddForce(new Vector2(30, 130), ForceMode2D.Impulse);
                Processor.Registers.Motion = 0;
            }
        }
        else if (Processor.Registers.Motion == -2)
        {
            if (_OnGround)
            {
                _Jump = true;
                _animator.SetInteger("State", -2);
                _Rigidbody.AddForce(new Vector2(-30, 130), ForceMode2D.Impulse);
                Processor.Registers.Motion = 0;
            }
        }
        else if (Processor.Registers.Motion == 3)
        {
            if (_Lift)
            {
                _animator.SetInteger("State", 0);
                if (_Lift.GetComponent<Lift>().TopPosition == true)
                {
                    Processor.Registers.Motion = 0;
                    Processor.Registers.Play = true;
                }
                else
                    _Lift.GetComponent<Lift>().MoveUp = true;
            }
        }
        else if (Processor.Registers.Motion == -3)
        {
            if (_Lift)
            {
                _animator.SetInteger("State", 0);
                if (_Lift && _Lift.GetComponent<Lift>().LowerPosition == true)
                {
                    Processor.Registers.Motion = 0;
                    Processor.Registers.Play = true;
                }
                else
                    _Lift.GetComponent<Lift>().MoveDown = true;
            }
        }
        else if (Processor.Registers.Motion == 0)
        {
           _animator.SetInteger("State", 0);
        }
    }
}
