using System.Collections.Generic;
using Magic.Modules;
//using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Playables;

public class GestureModuleAnimator : MonoBehaviour
{
    private GestureModule gesture;
    private Quaternion initialTransform;
    private GestureModule.GestureDirection gestureDirection;
    public GameObject gestureAnim;
    public PlayableDirector animClip;
    private bool acceptingInput = true;
    private float animationLength;
    public float delayBeforeNextInput;
    public bool returnToCenter;

    private void Start() 
    {
        initialTransform = gestureAnim.transform.localRotation;
        if (animClip != null)
        {
            animationLength = (float)animClip.duration;
        }
        else
        {
            animationLength = 2f;
        }    
    }
    void Update()
    {
        if (gesture == null)
        {
            gesture = FindFirstObjectByType<GestureModule>();
        }
        if (gesture != null&&acceptingInput)
        {
            gestureDirection = gesture.direction;
            if (gestureDirection != GestureModule.GestureDirection.none)
            {
                if (gestureDirection == GestureModule.GestureDirection.up)
                {
                    gestureAnim.transform.localRotation = Quaternion.Euler(initialTransform.eulerAngles.x,initialTransform.eulerAngles.y,initialTransform.eulerAngles.z);
                }
                else if (gestureDirection == GestureModule.GestureDirection.down)
                {
                    gestureAnim.transform.localRotation = Quaternion.Euler(initialTransform.eulerAngles.x,initialTransform.eulerAngles.y+180f,initialTransform.eulerAngles.z);
                }
                else if (gestureDirection == GestureModule.GestureDirection.right)
                {
                    gestureAnim.transform.localRotation = Quaternion.Euler(initialTransform.eulerAngles.x,initialTransform.eulerAngles.y+90f,initialTransform.eulerAngles.z+90f);
                }
                else if (gestureDirection == GestureModule.GestureDirection.left)
                {
                    gestureAnim.transform.localRotation = Quaternion.Euler(initialTransform.eulerAngles.x,initialTransform.eulerAngles.y-90f,initialTransform.eulerAngles.z-90f);
                }
                animClip.Play();
                StartCoroutine(WaitForAnimationToEnd());
                acceptingInput = false;
            }
        }
    }

    public IEnumerator<WaitForSeconds> WaitForAnimationToEnd()
    {
        yield return new WaitForSeconds(animationLength);
        if (returnToCenter) gestureAnim.transform.localRotation = initialTransform;
        animClip.Stop();
        animClip.time = 0f;
        yield return new WaitForSeconds(delayBeforeNextInput);
        acceptingInput = true;
    }
}
