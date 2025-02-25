using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement; 

public class CH_Knife : MonoBehaviour
{
    private CH_Player_Controll control;
    public ParticleSystem particle;
    private Rigidbody2D rb;
    private float originalGravityScale;
    private bool thrownCorrectly = true;

    public float spinTorque = 5f;
    public float pullForce = 10f;

    private void Awake()
    {
        control = FindObjectOfType<CH_Player_Controll>();
        rb = GetComponent<Rigidbody2D>();
        originalGravityScale = rb.gravityScale;
    }

    public bool IsThrownCorrectly()
    {
        return thrownCorrectly;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (control.IsGameOver()) return;

        if (collision.gameObject.CompareTag("enemy") || collision.gameObject.CompareTag("knife"))
        {
            control.sound[1].Play();


            ApplyForces(Vector2.down);
              control.GameOver();
           
            return;
        }

       


       else if (collision.gameObject.CompareTag("part"))
        {
           

            SpriteRenderer partRenderer = collision.gameObject.GetComponent<SpriteRenderer>();

         
            if (partRenderer != null)
            {
              
                Color partColor = partRenderer.color;

               
                SpriteRenderer knifeIconRenderer = transform.Find("KnifeIcon")?.GetComponent<SpriteRenderer>();


                if (knifeIconRenderer != null)
                {
                   
                    Color knifeIconColor = knifeIconRenderer.color;

                    
                    if (partColor == knifeIconColor)
                    {
                      
                        particle.Play();
                        //StartCoroutine(control.CameraShake(control.shakeDuration, control.shakeMagnitude));
                        control.AddScore();
                        control.sound[0].Play();
                        thrownCorrectly = true;

                    }
                    else
                    {
                        thrownCorrectly = false;


                        ApplyForces(Vector2.down);
                        control.sound[1].Play();
                        control.GameOver();
                       


                    }
                    
                }
               
            }

            if (thrownCorrectly)
            {
                Transform parentTransform = collision.transform.parent;
                transform.SetParent(parentTransform, true);

            }

         
        }


    }


    public void ResetGravityScale()
    {

        rb.gravityScale = originalGravityScale;

        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }

    private void ApplyForces(Vector2 direction)
    {
        rb.gravityScale = 0.3f;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.AddTorque(spinTorque, ForceMode2D.Impulse);
        rb.AddForce(direction * pullForce, ForceMode2D.Impulse);
      
    }

}
    


