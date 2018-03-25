using Combinatorics;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TspOptimizer
{
    public class GeneticOptimizer : TspOptimizerBase
    {
        private double _minDistance;
        private double _rate;
        private int _number;
        private int _population;
        private int[] _minTour;
        private Random _random;
        private Action<double> _action;
        private PartiallyMappedCrossover _partiallyMappedCrossover;
        private CancellationToken _token;

        public int[] MinTour
        {
            get { return _minTour; }
            set { _minTour = value; }
        }

        public double MinDistance
        {
            get { return _minDistance; }
            set
            {
                _minDistance = value;
                _action?.Invoke(_minDistance);
                _optimalSequence.OnNext(MinTour);
            }
        }

        public GeneticOptimizer(int[] startPermutation, IEuclideanPath euclideanPath, OptimizerConfig config)
            : base(startPermutation, euclideanPath, config)
        {
            _rate = _config.CrossoverRate;
            _number = _startPermutation.Length;
            _population = _config.PopulationSize;
            _random = new Random();
            _partiallyMappedCrossover = new PartiallyMappedCrossover(_number);

            MinDistance = double.MaxValue;
        }

        public override void Start(CancellationToken token, Action<double> action)
        {
            _token = token;

            if (_config.UseBigValleySearch)
                OptimizerInfo.OnNext("Starting Genetic Optimizer with Big-Valley-Search");
            else
                OptimizerInfo.OnNext("Starting Genetic Optimizer without Big-Valley-Search");

            _action = action;
            double[] distances = new double[_population];
            MinTour = Enumerable.Range(0, _number).ToArray();
            int[,] chromosomePool = new int[_population, _number];

            SetPopulationPool(chromosomePool, distances);

            if (!token.IsCancellationRequested)
                OptimizerInfo.OnNext("Starting genetic optimization");

            while (!token.IsCancellationRequested)
            {
                // Forcing delay for visualization
                if (_config.UseDelay)
                {
                    Thread.Sleep(_config.DelayTime);
                }

                if (_random.NextDouble() < _rate)
                {
                    int i, j, parent1, parent2;
                    int[] p1 = new int[_number];
                    int[] p2 = new int[_number];

                    i = _random.Next(_population);
                    j = _random.Next(_population);

                    if (distances[i] < distances[j])
                        parent1 = i;

                    else
                        parent1 = j;

                    i = _random.Next(_population);
                    j = _random.Next(_population);

                    if (distances[i] < distances[j])
                        parent2 = i;

                    else
                        parent2 = j;

                    for (i = 0; i < _number; i++)
                    {
                        p1[i] = chromosomePool[parent1, i];
                        p2[i] = chromosomePool[parent2, i];
                    }

                    int cp1 = -1, cp2 = -1;

                    do
                    {
                        cp1 = _random.Next(_number);
                        cp2 = _random.Next(_number);
                    } while (cp1 == cp2 || cp1 > cp2);

                    ChromosomeSegment segment = new ChromosomeSegment(cp1, cp2);
                    var offSpringPair = _partiallyMappedCrossover.GetCrossCombinedOffSpringPair(segment, new ParentPair<int>(p1, p2));

                    int[] o1 = offSpringPair.ChildA;
                    int[] o2 = offSpringPair.ChildB;

                    double o1Fitness = TourDistance(o1);
                    double o2Fitness = TourDistance(o2);

                    if (o1Fitness < distances[parent1])
                        for (i = 0; i < _number; i++)
                            chromosomePool[parent1, i] = o1[i];

                    if (o2Fitness < distances[parent2])
                        for (i = 0; i < _number; i++)
                            chromosomePool[parent2, i] = o2[i];

                    for (int p = 0; p < _population; p++)
                    {
                        if (distances[p] < MinDistance)
                        {
                            MinDistance = distances[p];

                            for (int n = 0; n < _number; n++)
                                MinTour[n] = chromosomePool[p, n];
                        }
                    }
                }
                else
                {
                    int p;
                    int[] child = new int[_number];

                    int i = _random.Next(_population);
                    int j = _random.Next(_population);

                    if (distances[i] < distances[j])
                        p = i;

                    else
                        p = j;

                    for (int n = 0; n < _number; n++)
                        child[n] = chromosomePool[p, n];


                    child = DoMutation(child);

                    double childDistance = TourDistance(child);

                    int maxIndex = int.MaxValue;
                    double maxDistance = double.MinValue;

                    for (int q = 0; q < _population; q++)
                    {
                        if (distances[q] >= maxDistance)
                        {
                            maxIndex = q;
                            maxDistance = distances[q];
                        }
                    }

                    int[] index = new int[_population];
                    int count = 0;

                    for (int q = 0; q < _population; q++)
                    {
                        if (distances[q] == maxDistance)
                        {
                            index[count++] = q;
                        }
                    }

                    maxIndex = index[_random.Next(count)];

                    if (childDistance < distances[maxIndex])
                    {
                        distances[maxIndex] = childDistance;

                        for (int n = 0; n < _number; n++)
                            chromosomePool[maxIndex, n] = child[n];

                        if (childDistance < MinDistance)
                        {
                            MinDistance = childDistance;

                            for (int n = 0; n < _number; n++)
                                MinTour[n] = child[n];
                        }
                    }
                }
            }

            _optimalSequence.OnCompleted();
            OptimizerInfo.OnNext("Genetic optimization terminated with distance: " + MinDistance.ToString() + " LU");
        }

        private void SetPopulationPool(int[,] chromosomePool, double[] distances)
        {
            if (_config.UseBigValleySearch)
            {
                Tuple<double, int[]>[] randomSequences = new Tuple<double, int[]>[_population];

                ParallelOptions parallelOptions = new ParallelOptions
                {
                    CancellationToken = _token,
                    MaxDegreeOfParallelism = _config.NumberOfCores
                };

                int count = 0;

                try
                {
                    Parallel.For(0, _population, parallelOptions, i =>
                    {
                        int[] sequence = Enumerable.Range(0, _number).ToArray();
                        Helper.Shuffle(sequence);

                        var twoOptOptimizer = new LocalTwoOptOptimizer(sequence, _euclideanPath, _config);
                        twoOptOptimizer.Start(1, true, _config.BigValleySearchRate);

                        var distance = _euclideanPath.GetPathLength(twoOptOptimizer.OptimalSequence, ClosedPath);
                        randomSequences[i] = new Tuple<double, int[]>(distance, twoOptOptimizer.OptimalSequence);

                        if (count++ % 10 == 0)
                        {
                            OptimizerInfo.OnNext("Progress Big-Valley-Search: " +
                                                 Math.Round(count / (double)_population * 100d, 2).ToString() + "%");
                        }
                    });

                    var orderedSequences = randomSequences.OrderBy(tuple => tuple.Item1).ToArray();

                    for (int p = 0; p < _population; p++)
                    {
                        distances[p] = orderedSequences[p].Item1;
                        int[] sequence = orderedSequences[p].Item2;

                        for (int n = 0; n < _number; n++)
                            chromosomePool[p, n] = sequence[n];
                    }

                    MinTour = orderedSequences.First().Item2.ToArray();
                    MinDistance = orderedSequences.First().Item1;
                }
                catch (OperationCanceledException)
                {
                    OptimizerInfo.OnNext("Big-Valley-Search canceled");
                }

                if (!_token.IsCancellationRequested)
                    OptimizerInfo.OnNext("Big-Valley-Search completed");
            }
            else
            {
                for (int p = 0; p < _population; p++)
                {
                    int[] city = Enumerable.Range(0, _number).ToArray();
                    Helper.Shuffle(city);

                    for (int n = 0; n < _number; n++)
                        chromosomePool[p, n] = city[n];

                    distances[p] = TourDistance(city);

                    if (distances[p] < MinDistance)
                    {
                        MinDistance = distances[p];

                        for (int n = 0; n < _number; n++)
                            MinTour[n] = chromosomePool[p, n];
                    }
                }
            }

            if (!_token.IsCancellationRequested)
                OptimizerInfo.OnNext("Population initialized");
        }

        private int[] DoMutation(int[] currentSequence)
        {
            int cp1, cp2;

            do
            {
                cp1 = _random.Next(0, currentSequence.Length - 1);
                cp2 = _random.Next(1, currentSequence.Length);
            } while (cp1 == cp2 || cp1 > cp2);

            return Helper.TwoOptSwap(currentSequence, cp1, cp2);
        }

        private double TourDistance(int[] city)
        {
            return _euclideanPath.GetPathLength(city, ClosedPath);
        }
    }
}
