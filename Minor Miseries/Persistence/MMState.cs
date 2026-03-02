namespace Minor_Miseries.Persistence
{
    [Serializable]
    internal class MMState
    {
        public float AnimalStressScore = 0f;
        public float AnimalStressTimer = -1f;
        public float HoursSinceLastAffliction = 0f;
        public float HoursSpentMoving = 0f;
        public float HoursOverloaded = 0f;
    }
}