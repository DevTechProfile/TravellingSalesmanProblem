using Combinatorics;
using PathHelper;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TspOptimizer;

namespace Visualizer
{
    public class TravellingSalesmanProblemViewModel : BindableBase
    {
        const string _intialNumberOfPointsString = "200";

        private List<Point> _currentPath;
        private Point[] _initialPath;
        private string _plotTitle;
        private int[] _shuffledTour;
        private IEuclideanPath _euclideanPath;
        private string _info;
        private CancellationTokenSource _tokenSource;
        private TspOptimizerAlgorithm _selectedOptimizer;
        private TspPathType _selectedPathType;
        private string _pathLength;
        private string _numberOfPoints;
        private bool _startButtonEnable;
        private bool _useDelay;
        private string _delayTime;
        private bool _useBigValleySearch;
        private string _coolingRate;
        private string _populationSize;
        private string _crossoverRate;
        private string _numberOfCores;
        private string _infoOutput;
        private bool _isLogsChanged;
        private string _bigValleySearchRate;

        public IEnumerable<TspOptimizerAlgorithm> OptimizerSet
        {
            get { return Enum.GetValues(typeof(TspOptimizerAlgorithm)).Cast<TspOptimizerAlgorithm>(); }
        }

        public IEnumerable<TspPathType> PathTypeSet
        {
            get { return Enum.GetValues(typeof(TspPathType)).Cast<TspPathType>(); }
        }

        public TspOptimizerAlgorithm SelectedOptimizer
        {
            get { return _selectedOptimizer; }
            set { _selectedOptimizer = value; RaisePropertyChanged(); }
        }

        public TspPathType SelectedPathType
        {
            get { return _selectedPathType; }
            set { _selectedPathType = value; RaisePropertyChanged(); }
        }

        public List<Point> CurrentPath
        {
            get { return _currentPath; }
            set { _currentPath = value; RaisePropertyChanged(); }
        }

        public string PlotTitle
        {
            get { return _plotTitle; }
            set { _plotTitle = value; RaisePropertyChanged(); }
        }

        public string Info
        {
            get { return _info; }
            set { _info = value; RaisePropertyChanged(); }
        }

        public string PathLength
        {
            get { return _pathLength; }
            set { _pathLength = value; RaisePropertyChanged(); }
        }

        public string NumberOfPoints
        {
            get { return _numberOfPoints; }
            set
            {
                _numberOfPoints = value;
                RaisePropertyChanged();
            }
        }

        public bool StartButtonEnable
        {
            get { return _startButtonEnable; }
            set { _startButtonEnable = value; RaisePropertyChanged(); }
        }

        public bool UseDelay
        {
            get { return _useDelay; }
            set { _useDelay = value; RaisePropertyChanged(); }
        }

        public string DelayTime
        {
            get { return _delayTime; }
            set { _delayTime = value; RaisePropertyChanged(); }
        }

        public string CoolingRate
        {
            get { return _coolingRate; }
            set { _coolingRate = value; RaisePropertyChanged(); }
        }

        public bool UseBigValleySearch
        {
            get { return _useBigValleySearch; }
            set { _useBigValleySearch = value; RaisePropertyChanged(); }
        }

        public string PopulationSize
        {
            get { return _populationSize; }
            set { _populationSize = value; RaisePropertyChanged(); }
        }

        public string CrossoverRate
        {
            get { return _crossoverRate; }
            set { _crossoverRate = value; RaisePropertyChanged(); }
        }

        public string NumberOfCores
        {
            get { return _numberOfCores; }
            set { _numberOfCores = value; RaisePropertyChanged(); }
        }

        public string BigValleySearchRate
        {
            get { return _bigValleySearchRate; }
            set { _bigValleySearchRate = value; RaisePropertyChanged(); }
        }

        public string InfoOutput
        {
            get { return _infoOutput; }
            set { _infoOutput = value; RaisePropertyChanged(); }
        }

        public bool IsLogsChanged
        {
            get { return _isLogsChanged; }
            set { _isLogsChanged = value; RaisePropertyChanged(); }
        }

        public ICommand StartPathOptimizationCommand { get; }

        public ICommand StopPathOptimizationCommand { get; }

        public ICommand ShufflePathCommand { get; }

        public ICommand PathTypeChangedCommand { get; }

        public TravellingSalesmanProblemViewModel()
        {
            StartPathOptimizationCommand = new DelegateCommand(OnAlgorithmStart);
            StopPathOptimizationCommand = new DelegateCommand(OnAlgorithmStop);
            ShufflePathCommand = new DelegateCommand(OnShufflePath);
            PathTypeChangedCommand = new DelegateCommand(OnPathTypeChanged);

            SelectedPathType = TspPathType.BigValleyRandom;
            SelectedOptimizer = TspOptimizerAlgorithm.GeneticOptimizer;
            NumberOfPoints = _intialNumberOfPointsString;
            StartButtonEnable = true;
            InfoOutput = "Optimizer info...";

            //Config
            UseDelay = false;
            DelayTime = "1";
            CoolingRate = "0.95";
            UseBigValleySearch = true;
            PopulationSize = "500";
            CrossoverRate = "0.1";
            NumberOfCores = "16";
            BigValleySearchRate = "0.2";
        }

        private void OnPathTypeChanged()
        {
            int numberOfPoints = VerifyInput();
            var path = PathGeneratorFactory.Create(SelectedPathType, new Size(10, 10), numberOfPoints);

            if (path == null)
            {
                Info = "Path generator not yet implemented :-(";
                return;
            }

            _initialPath = path.Select(coordinate => coordinate.To2DPoint()).ToArray();
            _euclideanPath = new FastEuclideanPath(_initialPath);
            _shuffledTour = Enumerable.Range(0, _initialPath.Length).ToArray();

            PlotTour(Enumerable.Range(0, _initialPath.Length).ToArray());
        }

        private void UpdateInfo(string entry)
        {
            StringBuilder builder = new StringBuilder(InfoOutput);
            builder.Append(Environment.NewLine);
            builder.Append(entry);

            InfoOutput = builder.ToString();
            IsLogsChanged = true;
        }

        private int VerifyInput()
        {
            int numberOfPoints = Convert.ToInt32(_intialNumberOfPointsString);

            if (string.IsNullOrWhiteSpace(NumberOfPoints))
            {
                return numberOfPoints;
            }

            try
            {
                numberOfPoints = Convert.ToInt32(NumberOfPoints);
            }
            catch (Exception)
            {
                NumberOfPoints = _intialNumberOfPointsString;
                return numberOfPoints;
            }

            return numberOfPoints;
        }

        private void OnAlgorithmStop() => _tokenSource?.Cancel();

        private void OnShufflePath()
        {
            _shuffledTour = Enumerable.Range(0, _initialPath.Length).ToArray();
            Helper.Shuffle(_shuffledTour);
            PlotTour(_shuffledTour);
        }

        private void OnAlgorithmStart()
        {
            StartButtonEnable = false;
            var config = new OptimizerConfig()
            {
                UseDelay = UseDelay,
                DelayTime = Convert.ToInt32(DelayTime),
                NumberOfCores = Convert.ToInt32(NumberOfCores),
                CoolingRate = Convert.ToDouble(CoolingRate, CultureInfo.InvariantCulture),
                UseBigValleySearch = UseBigValleySearch,
                PopulationSize = Convert.ToInt32(PopulationSize),
                CrossoverRate = Convert.ToDouble(CrossoverRate, CultureInfo.InvariantCulture),
                BigValleySearchRate = Convert.ToDouble(BigValleySearchRate, CultureInfo.InvariantCulture)
            };

            ITspOptimizer optimizer = TspOptimizerFactory.Create(SelectedOptimizer, _shuffledTour, _euclideanPath, config);

            if (optimizer == null)
            {
                Info = "Algorithm not yet implemented :-(";
                return;
            }

            optimizer.OptimizerInfo.Subscribe(UpdateInfo);
            optimizer.OptimalSequence.Subscribe(PlotTour);

            _tokenSource = new CancellationTokenSource();
            CancellationToken token = _tokenSource.Token;

            var context = TaskScheduler.FromCurrentSynchronizationContext();

            Info = "Optimizer started...";
            Task task = Task.Factory.StartNew(() =>
            {
                optimizer.Start(token, UpdatePathLength);
            }, token).ContinueWith((t) =>
            {
                Info = "Optimizer canceled...";
                StartButtonEnable = true;
            }, context);
        }

        private void PlotTour(int[] tour)
        {
            CurrentPath = new List<Point>(tour.Select(i => _initialPath[i]));
        }

        private void UpdatePathLength(double pathLength)
        {
            PathLength = "Path length: " + Math.Round(pathLength, 2).ToString() + " LU";
        }
    }
}
