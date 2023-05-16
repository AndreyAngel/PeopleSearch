namespace PeopleSearch.Infrastructure.RecommenderSystem;

public static class SVD
{
    /// <summary>
    /// Скорость градиентного спуска (шаг)
    /// </summary>
    private static double _step = 0.15;

    /// <summary>
    /// Коэффициент изменения скорости градиентного спуска
    /// </summary>
    private static readonly double _deltaStep = 0.9;

    /// <summary>
    /// Верхний порог разницы предыдущей и текущей среднеквадратичных ошибок (RMSE)
    /// </summary>
    private static double _threshold = 10;

    /// <summary>
    /// Коэффициент изменения верхнего порога разницы предыдущей и текущей среднеквадратичных ошибок (RMSE)
    /// </summary>
    private static readonly double _deltaThreshold = 0.5;

    /// <summary>
    /// L1 регуляризатор
    /// </summary>
    private static readonly double L1 = 0;

    /// <summary>
    /// L2 регуляризатор
    /// </summary>
    private static readonly double L2 = 0;

    /// <summary>
    /// Целевая точность
    /// </summary>
    private static readonly double _eps = 0.0001;

    /// <summary>
    /// Длина вескторов факторизации
    /// </summary>
    private static int _lenFactors;

    /// <summary>
    /// Матрица рейтингов
    /// </summary>
    private static List<List<double>> _ratingMatrix;

    /// <summary>
    /// Матрица векторов факторизации пользователей
    /// </summary>
    private static readonly List<List<double>> _userFactorsMatrix = new();

    /// <summary>
    /// Матрица векторов факторизации предметов
    /// </summary>
    private static readonly List<List<double>> _itemFactorsMatrix = new();

    /// <summary>
    /// Вектор бызовых предикторов пользователей
    /// </summary>
    private static readonly List<double> _userBaselinePredictors = new();

    /// <summary>
    /// Вектор базовых предикторов предметов
    /// </summary>
    private static readonly List<double> _itemBaselinePredictors = new();

    public static void Initialize(List<List<double>> matrix, int conutFactors)
    {
        _ratingMatrix = matrix;
        _lenFactors = conutFactors;

        // Constructing the matrix of user's latent factors by iteratively
        // appending the rows being constructed to the list of rows MF_UserRow
        for (int User = 0; User < _ratingMatrix.Count; User++)
        {
            // Declare a list of items MF_UserRow rated by the current user
            List<double> MF_UserRow = new();

            // Add the set of elements equal to 0 to the list of items MF_UserRow.
            // The number of elements being added is stored in Factors variable
            MF_UserRow.AddRange(Enumerable.Repeat(0.00, _lenFactors));

            // Append the current row MF_UserRow to the matrix of factors MF_User
            _userFactorsMatrix.Insert(User, MF_UserRow);
        }

        // Constructing the matrix of item's latent factors by iteratively
        // appending the rows being constructed to the list of rows MF_ItemRow
        for (int Item = 0; Item < _ratingMatrix[0].Count; Item++)
        {
            // Declare a list of items MF_ItemRow rated by the current item
            List<double> MF_ItemRow = new();

            // Add the set of elements equal to 0 to the list of items MF_ItemRow
            // The number of elements being added is stored in Factors variable
            MF_ItemRow.AddRange(Enumerable.Repeat(0.00, _lenFactors));

            // Append the current row MF_ItemRow to the matrix of factors MF_Item
            _itemFactorsMatrix.Insert(Item, MF_ItemRow);
        }

        // Intializing the first elements of the matrices of user's
        // and item's factors with values 0.1 and 0.05
        _userFactorsMatrix[0][0] = 0.1;
        _itemFactorsMatrix[0][0] = _userFactorsMatrix[0][0] / 2;

        // Construct the vector of users baseline predictors by
        // appending the set of elements equal to 0.The number of elements being
        // appended is equal to the actual number of rows in the matrix of ratings
        _userBaselinePredictors.AddRange(Enumerable.Repeat(0.00, _ratingMatrix.Count));

        // Construct the vector of items baseline predictors by appending
        // the set of elements equal to 0. The number of elements appended
        // is equal to the actual number of rows in the matrix of ratings
        _itemBaselinePredictors.AddRange(Enumerable.Repeat(0.00, _ratingMatrix[0].Count));

        Learn();
    }

    public static List<Prediction> Predict()
    {
        List<Prediction> predictions = new();

        // Computing the average rating for the entire domain of rated items
        double AvgRating = GetAverageRating(_ratingMatrix);

        // Iterating through the MatrixUI matrix of ratings
        for (int User = 0; User < _ratingMatrix.Count; User++)
            for (int Item = 0; Item < _ratingMatrix[0].Count; Item++)

                // For each rating given to the current item by the current user 
                // we're performing a check if the current item is unknown
                if (_ratingMatrix[User][Item] == 0)
                {
                    // If so, compute the rating for the current 
                    // unrated item used baseline estimate formula (2)
                    _ratingMatrix[User][Item] = AvgRating + _userBaselinePredictors[User] +
                        _itemBaselinePredictors[Item] + GetProduct(_userFactorsMatrix[User], _itemFactorsMatrix[Item]);

                    // Output the original rating estimated for the current item 
                    // and the rounded value of the following rating

                    predictions.Add(new Prediction()
                    {
                        UserNumber = User,
                        ItemNumber = Item,
                        PredictedGrade = _ratingMatrix[User][Item],
                        Grade = (int)Math.Round(_ratingMatrix[User][Item])
                    });
                }

        return predictions;
    }


    private static void Learn()
    {
        // Initializing the iterations loop counter variable
        int Iterations = 0;

        // Initializing the RMSE and RMSE_New variables to store
        // current and previous values of RMSE
        double RMSE = 0.00;
        double RMSE_New = 1.00;

        // Computing the average rating for the entire domain of rated items
        double AvgRating = GetAverageRating(_ratingMatrix);

        // Iterating the process of the ratings prediction model update until
        // the value of difference between the current and previous value of RMSE
        // is greater than the value of error precision accuracy EPS (e.g. the learning
        // process has converged).
        while (Math.Abs(RMSE - RMSE_New) > _eps)
        {
            // Assign the previously obtained value of RMSE to the RMSE variable
            // Assign the variable RMSE_New equal to 0
            RMSE = RMSE_New;
            RMSE_New = 0;

            // Iterate through the matrix of ratings and for each existing rating compute
            // the error value and perform the stochastic gradient descent to update 
            // the main parameters of the ratings prediction model for the current user and item
            for (int User = 0; User < _ratingMatrix.Count; User++)
            {
                for (int Item = 0; Item < _ratingMatrix[0].Count; Item++)

                    // Perform a check if the current rating in the matrix of ratings is unknown.
                    // If not, perform the following steps to adjust the values of baseline
                    // predictors and factorization vectors
                    if (_ratingMatrix[User][Item] > 0)
                    {
                        // Compute the value of estimated rating using formula (2)
                        double Rating = AvgRating + _userBaselinePredictors[User] +
                        _itemBaselinePredictors[Item] + GetProduct(_userFactorsMatrix[User], _itemFactorsMatrix[Item]);

                        // Compute the error value as the difference 
                        // between the existing and estimated ratings
                        double Error = _ratingMatrix[User][Item] - Rating;

                        // Add the value of error square to the current value of RMSE
                        RMSE_New += Math.Pow(Error, 2);

                        // Update the value of average rating for the entire domain of ratings
                        // by performing stochastic gradient descent using formulas (7.1-5)
                        AvgRating += _step * (Error - L1 * AvgRating);

                        // Update the value of baseline predictor of the current user
                        // by performing stochastic gradient descent using formulas (7.1-5)
                        _userBaselinePredictors[User] = _userBaselinePredictors[User] +
                                                        _step * (Error - L1 * _userBaselinePredictors[User]);

                        // Update the value of baseline predictor of the current item 
                        // by performing stochastic gradient descent using formulas (7.1-5)
                        _itemBaselinePredictors[Item] = _itemBaselinePredictors[Item] +
                                                        _step * (Error - L1 * _itemBaselinePredictors[Item]);

                        // Update each component of the factorization vector for 
                        // the current user and item
                        for (int Factor = 0; Factor < _lenFactors; Factor++)
                        {
                            // Adjust the value of the current component of the user's 
                            // factorization vector by performing stochastic gradient 
                            // descent using formulas (7.1-5)
                            _userFactorsMatrix[User][Factor] += _step * (Error * _itemFactorsMatrix[Item][Factor] +
                                                                L2 * _userFactorsMatrix[User][Factor]);

                            // Adjust the value of the current component of the item's 
                            // factorization vector by performing stochastic gradient 
                            // descent using formulas (7.1-5)
                            _itemFactorsMatrix[Item][Factor] += _step * (Error * _userFactorsMatrix[User][Factor] +
                                                                L2 * _itemFactorsMatrix[Item][Factor]);
                        }
                    }
            }

            // Compute the current value of RMSE (root means square error)
            RMSE_New = Math.Sqrt(RMSE_New / (_ratingMatrix.Count * _ratingMatrix[0].Count));

            // Performing a check if the difference between the values 
            // of current and previous values of RMSE exceeds the given threshold
            if (RMSE_New > RMSE - _threshold)
            {
                // If so, reduce the values of training speed and threshold 
                // by multiplying each value by the value of specific coefficients
                _step *= _deltaStep; _threshold *= _deltaThreshold;
            }

            Iterations++; // Increment the iterations loop counter variable
        }
    }

    private static double GetProduct(List<double> userFactorizationVector, List<double> itemFactorizationVector)
    {
        // Initialize the variable that is used to
        // store the inner product of two factorization vectors
        double Product = 0.00;

        // Iterating through the two factorization vectors
        for (int Index = 0; Index < _lenFactors; Index++)

            // Compute the value of product of the two components
            // of those vectors having the same value of index and
            // add this value to the value of the variable Product
            Product += userFactorizationVector[Index] * itemFactorizationVector[Index];

        return Product;
    }

    private static double GetAverageRating(List<List<double>> Matrix)
    {
        // Initialize the variables Sum and Count to store the values of
        // sum of existing ratings in matrix of ratings and the count of
        // existing ratings respectively
        double Sum = 0;
        int Count = 0;

        // Iterating through the matrix of ratings
        for (int User = 0; User < Matrix.Count; User++)
            for (int Item = 0; Item < Matrix[User].Count; Item++)

                // For each rating performing a check if the current rating is unknown
                if (Matrix[User][Item] > 0)
                {
                    // If not, add the value of the current rating to the
                    // value of variable Sum
                    Sum += Matrix[User][Item];

                    // Increment the loop counter variable of existing ratings by 1
                    Count++;
                }

        // Compute and return the value of average
        // rating for the entire domain of existing ratings
        return Sum / Count;
    }
}
