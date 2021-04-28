namespace Model
{
    public interface IEdible
    {
        float nutritionValue
        {
            get;
        }

        bool isEaten
        {
            get;
        }

        float GetEaten();
    }
}