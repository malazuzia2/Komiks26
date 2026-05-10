using UnityEngine;

public class SnakeProceduralAnim : MonoBehaviour
{
    public Transform[] snakeBones;
    public float boneDistance = 0.15f;
    public float smoothSpeed = 12f;

    [Header("Poprawka Rotacji")]
    public Vector3 rotationOffset = new Vector3(90, 0, 0);

    // Tablica do przechowywania pozycji z poprzedniej klatki
    private Vector3[] bonePositions;

    void Start()
    {
        if (snakeBones.Length < 2) return;

        bonePositions = new Vector3[snakeBones.Length];
        for (int i = 0; i < snakeBones.Length; i++)
        {
            bonePositions[i] = snakeBones[i].position;
        }
    }

    // U¿ywamy FixedUpdate lub LateUpdate, ¿eby Animator nie przeszkadza³
    void LateUpdate()
    {
        if (snakeBones == null || snakeBones.Length < 2) return;

        // Pierwsza koœæ (g³owa) jest zawsze tam, gdzie j¹ przesuniesz
        bonePositions[0] = snakeBones[0].position;

        for (int i = 1; i < snakeBones.Length; i++)
        {
            // Obliczamy gdzie koœæ POWINNA byæ
            Vector3 targetPos = bonePositions[i - 1];
            Vector3 currentPos = bonePositions[i];

            Vector3 direction = (targetPos - currentPos).normalized;

            if (direction != Vector3.zero)
            {
                // Wyznaczamy now¹ pozycjê wirtualn¹
                bonePositions[i] = Vector3.Lerp(currentPos, targetPos - direction * boneDistance, Time.deltaTime * smoothSpeed);

                // Aplikujemy pozycjê i rotacjê do fizycznej koœci w Unity
                snakeBones[i].position = bonePositions[i];

                Quaternion lookRot = Quaternion.LookRotation(direction);
                snakeBones[i].rotation = Quaternion.Slerp(snakeBones[i].rotation, lookRot * Quaternion.Euler(rotationOffset), Time.deltaTime * smoothSpeed);
            }
        }
    }
}