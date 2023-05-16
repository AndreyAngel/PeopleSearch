namespace PeopleSearch.Infrastructure.RecommenderSystem;

public class Prediction
{
    public int UserNumber { get; set; }

    public int ItemNumber { get; set; }

    public double PredictedGrade { get; set; }

    public int Grade { get; set; }
}
