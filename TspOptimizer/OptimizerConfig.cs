namespace TspOptimizer
{
    public class OptimizerConfig
    {
        public bool UseDelay { get; set; }
        public int DelayTime { get; set; }
        public double CoolingRate { get; set; }
        public bool UseBigValleySearch { get; set; }
        public int PopulationSize { get; set; }
        public double CrossoverRate { get; set; }
    }
}
