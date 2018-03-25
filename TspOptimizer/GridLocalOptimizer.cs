using Combinatorics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TspOptimizer
{
    public class GridLocalOptimizer : TspOptimizerBase
    {
        private const double GRID_OFFSET = 1;

        private int _maxCellCount;
        private Point[] _points;
        private Point[] _globalGridPoints;
        private int[] _globalGridPointSequence;
        private CancellationToken _token;
        private Action<double> _action;

        public List<GridCell> GridCells { get; private set; }

        public Point[] GlobalGridPoints
        {
            get { return _globalGridPoints; }
        }

        public int[] GlobalGridPointSequence
        {
            get { return _globalGridPointSequence; }
        }

        public double GlobalMinPath { get; private set; }

        public GridLocalOptimizer(int[] startPermutation, IEuclideanPath euclideanPath, OptimizerConfig config)
            : base(startPermutation, euclideanPath, config)
        {
            // ToDo: Get from config
            _maxCellCount = 16;
            _points = euclideanPath.Points;
        }

        public override void Start(CancellationToken token, Action<double> action)
        {
            _token = token;
            _action = action;
            ConstructCellOrder();
        }

        private void ConstructCellOrder()
        {
            double xSpan = GetXSpan();
            double ySpan = GetYSpan();

            var rectDimensions = RectDecompensition.GetDecompensition(_maxCellCount, xSpan, ySpan);
            var cellCount = rectDimensions.Item1 * rectDimensions.Item2;
            GridCells = new List<GridCell>(cellCount);

            var rectList = GetRectList(rectDimensions);
            rectList.ForEach(rect => GridCells.Add(new GridCell(rect)));

            MapPointsToGridCells();
            SetGlobalGridPoints();
            SetOptSequenceGlobalGridPoints();
            SetConnectionCellPoints();
            DoLocalOptimizationGidCell();
            SetMinValues();
        }

        private void SetMinValues()
        {
            var minSequence = new List<int>();

            foreach (var index in _globalGridPointSequence)
            {
                var curCell = GridCells[index];
                minSequence.AddRange(curCell.GetCompleteOptimizedIndices());
            }

            _action?.Invoke(_euclideanPath.GetPathLength(minSequence.ToArray(), true));
            _optimalSequence.OnNext(minSequence.ToArray());
        }

        private void DoLocalOptimizationGidCell()
        {
            Parallel.ForEach(GridCells, cell =>
            {
                int length = cell.GetCompleteIndices().Length;
                OptimizerInfo.OnNext("Local cell element count: " + length.ToString());
                //var euclideanPath = new FastEuclideanPath(_globalGridPoints);
                ITspOptimizer optimizer = new BranchAndBoundOptimizer(cell.GetCompleteIndices(), _euclideanPath, _config);
                {
                    ClosedPath = false;
                };

                int[] optSequence = null;
                optimizer.OptimalSequence.Subscribe(seq => optSequence = seq);
                var cast = optimizer as BranchAndBoundOptimizer;
                cast.ClosedPath = false;
                cast.Start(_token, _action, 1, length - 2);
                cell.InnerMinSequence = optSequence.Skip(1).Take(length - 2).ToArray();
            });
        }

        private void SetConnectionCellPoints()
        {
            for (int pointIndex = 0; pointIndex < _globalGridPointSequence.Length; pointIndex++)
            {
                var gridCell = GetGridByGlobalPointOptOrder(pointIndex);

                if (pointIndex < _globalGridPointSequence.Length - 1)
                {
                    var nextGridCell = GetGridByGlobalPointOptOrder(pointIndex + 1);
                    SetBestConnectionPair(gridCell, nextGridCell);
                }
                else
                {
                    var nextGridCell = GetGridByGlobalPointOptOrder(0);
                    SetBestConnectionPair(gridCell, nextGridCell);
                }
            }
        }

        private void SetBestConnectionPair(GridCell gridCell, GridCell nextGridCell)
        {
            double minDistance = double.MaxValue;

            int minCurIndex = 0;
            int minNextIndex = 0;

            for (int i = 0; i < gridCell.ElementIndices.Length; i++)
            {
                for (int j = 0; j < nextGridCell.ElementIndices.Length; j++)
                {
                    int curIndex = gridCell.ElementIndices[i];
                    int nextIndex = nextGridCell.ElementIndices[j];

                    if (_euclideanPath.GetDistance(curIndex, nextIndex) < minDistance &&
                        gridCell.InPointIndex != curIndex && nextGridCell.OutPointIndex != nextIndex)
                    {
                        minDistance = _euclideanPath.GetDistance(curIndex, nextIndex);
                        minCurIndex = curIndex;
                        minNextIndex = nextIndex;
                    }
                }
            }

            gridCell.OutPointIndex = minCurIndex;
            nextGridCell.InPointIndex = minNextIndex;
        }

        private GridCell GetGridByGlobalPointOptOrder(int index)
        {
            var curPoint = _globalGridPoints[_globalGridPointSequence[index]];
            return GridCells.FirstOrDefault(cell => (cell.Rectangle.Location - curPoint).Length < 1E-02);
        }

        private void SetOptSequenceGlobalGridPoints()
        {
            var euclideanPath = new FastEuclideanPath(_globalGridPoints);
            var optimizer = TspOptimizerFactory.Create
                (TspOptimizerAlgorithm.BranchAndBoundOptimizer,
                Enumerable.Range(0, _globalGridPoints.Length).ToArray(), euclideanPath, _config);

            optimizer.OptimalSequence.Subscribe(seq => _globalGridPointSequence = seq);
            optimizer.Start(_token, _action);

            GlobalMinPath = euclideanPath.GetPathLength(_globalGridPointSequence, true);
        }

        private void SetGlobalGridPoints()
        {
            _globalGridPoints = GridCells.Where(cell => cell.ElementIndices.Any()).Select(cell => cell.Rectangle.Location).ToArray();
        }

        private void MapPointsToGridCells()
        {
            foreach (var gridCell in GridCells)
            {
                List<int> cellElements = new List<int>();

                for (int i = 0; i < _points.Length; i++)
                {
                    if (gridCell.Rectangle.IsInnerPoint(_points[i]))
                    {
                        cellElements.Add(i);
                    }
                }

                gridCell.ElementIndices = cellElements.ToArray();
            }
        }

        private List<Rect> GetRectList(Tuple<int, int> rectDimensions)
        {
            var cellCount = rectDimensions.Item1 * rectDimensions.Item2;
            var rectList = new List<Rect>(cellCount);

            double minX = _points.Min(pnt => pnt.X) - GRID_OFFSET;
            double minY = _points.Min(pnt => pnt.Y) - GRID_OFFSET;
            double xSpan = GetXSpan();
            double ySpan = GetYSpan();

            for (int i = 0; i < rectDimensions.Item1; i++)
            {
                for (int j = 1; j < rectDimensions.Item2 + 1; j++)
                {
                    var topLeftPoint = new Point(minX + i * xSpan / rectDimensions.Item1,
                                                 minY + j * ySpan / rectDimensions.Item2);

                    rectList.Add(new Rect(topLeftPoint,
                         new Size(xSpan / rectDimensions.Item1, ySpan / rectDimensions.Item2)));
                }
            }

            return rectList;
        }

        private double GetXSpan()
        {
            double minX = _points.Min(pnt => pnt.X);
            double maxX = _points.Max(pnt => pnt.X);

            return maxX - minX + 2 * GRID_OFFSET;
        }

        private double GetYSpan()
        {
            double minY = _points.Min(pnt => pnt.Y);
            double maxY = _points.Max(pnt => pnt.Y);

            return maxY - minY + 2 * GRID_OFFSET;
        }
    }
}
