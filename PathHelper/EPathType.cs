namespace PathHelper
{
    public enum TspPathType
    {
        /// <summary>
        /// Uniform distributed 2D point cloud
        /// </summary>
        Uniform2DRandom,

        /// <summary>
        /// Normal distributed 2D point cloud
        /// </summary>
        Normal2DRandom,

        /// <summary>
        /// Uniform distributed 3D point cloud
        /// </summary>
        Uniform3DRandom,

        /// <summary>
        /// Normal distributed 3D point cloud
        /// </summary>
        Normal3DRandom,

        /// <summary>
        /// Circle path
        /// </summary>
        Circle,

        /// <summary>
        /// Big valley path
        /// </summary>
        BigValleyRandom
    }
}
