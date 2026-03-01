using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Magic.Modules
{
    public class ToneListener : MonoBehaviour
    {
        private ToneModule toneModule;

        public UnityEvent onToneSet;

        public GameObject objectToMove;
        public float minYPosition;
        public float maxYPosition;
        public float movementDuration;
        public bool reverseDirection;
        public List<ParticleSystem> particleEffects;

        private void OnEnable()
        {
            toneModule = FindObjectOfType<ToneModule>();
            if (toneModule != null)
            {
                toneModule.onToneSet.AddListener(OnToneSet);
            }
            else
            {
                Debug.LogWarning("ToneModule not found in the scene.");
            }
        }

        private void OnDisable()
        {
            if (toneModule != null)
            {
                toneModule.onToneSet.RemoveListener(OnToneSet);
            }
        }

        // This function will be called when the event is invoked
        private void OnToneSet(int frequency, int time)
        {
            Debug.Log("Tone set with frequency: " + frequency + " and time: " + time);
            StartCoroutine(MoveObject(frequency, time));
        }

        private IEnumerator MoveObject(int frequency, int time)
        {
            float startYPosition = objectToMove.transform.localPosition.y;
            float middleYPosition = reverseDirection ? minYPosition : maxYPosition;
            float targetYPosition = Mathf.Lerp(minYPosition, maxYPosition, frequency / 3000f);

            float halfDuration = movementDuration / 2f;

            Vector3 startPosition = new Vector3(objectToMove.transform.localPosition.x, startYPosition, objectToMove.transform.localPosition.z);
            Vector3 middlePosition = new Vector3(objectToMove.transform.localPosition.x, middleYPosition, objectToMove.transform.localPosition.z);
            Vector3 targetPosition = new Vector3(objectToMove.transform.localPosition.x, targetYPosition, objectToMove.transform.localPosition.z);

            float elapsedTime = 0f;

            // Move to the middle position
            while (elapsedTime < halfDuration)
            {
                objectToMove.transform.localPosition = Vector3.Lerp(startPosition, middlePosition, elapsedTime / halfDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            objectToMove.transform.localPosition = middlePosition;

            // Move to the target position
            elapsedTime = 0f;
            while (elapsedTime < halfDuration)
            {
                objectToMove.transform.localPosition = Vector3.Lerp(middlePosition, targetPosition, elapsedTime / halfDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            objectToMove.transform.localPosition = targetPosition;

            TriggerParticleEffects(time);

            // Wait for three times the duration of the tone
            yield return new WaitForSeconds(time / 1000f * 3);

            // Lerp back to the opposite of the middle position
            elapsedTime = 0f;
            float returnDuration = time / 1000f * 3;
            Vector3 returnPosition = new Vector3(objectToMove.transform.localPosition.x, reverseDirection ? maxYPosition : minYPosition, objectToMove.transform.localPosition.z);
            
            while (elapsedTime < returnDuration)
            {
                objectToMove.transform.localPosition = Vector3.Lerp(targetPosition, returnPosition, elapsedTime / returnDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            objectToMove.transform.localPosition = returnPosition;
        }

        private void TriggerParticleEffects(int time)
        {
            float durationInSeconds = time / 1000f;

            foreach (ParticleSystem ps in particleEffects)
            {
                var main = ps.main;
                main.duration = durationInSeconds;
                ps.Play();
            }

            if (onToneSet != null)
            {
                onToneSet.Invoke();
            }
        }
    }
}
