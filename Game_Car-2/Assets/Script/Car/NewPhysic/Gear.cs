
[System.Serializable]
public class Gear 
{
    
        public float minRPM; 
        public float maxRPM ; 
        public float gearRatio ; 

   
    public float CalculateMaxSpeed(float maxRPM)
    {
        // maxSpeed = (maxRPM / minRPM) * gearRatio
        // Здесь мы предполагаем, что на максимальных оборотах maxRPM машина может достигнуть максимальной скорости
        return (maxRPM / minRPM) * gearRatio;
    }

    // Метод для вычисления минимальной скорости на текущей передаче
    public float CalculateMinSpeed(float minRPM)
    {
        // minSpeed = (minRPM / maxRPM) * gearRatio
        // Это упрощенная версия, чтобы найти минимальную скорость для передачи.
        return (minRPM / maxRPM) * gearRatio;
    }
}
