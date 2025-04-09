using UnityEngine;

public class Wind : MonoBehaviour
{
    public Vector3 Direction;
    public float Strength;

    public Vector3 CalculateWindResistance(Vector3 carPosition, Vector3 carVelocity)
    {
        // Рассчитываем сопротивление ветра в зависимости от направления и силы ветра
        Vector3 relativeWind = Direction.normalized * Strength - carVelocity;
        return relativeWind * 0.1f; // Простой расчет сопротивления ветра
    }
}
