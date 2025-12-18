namespace FCAT
{
    public class Node3D(double x, double y, double z, double weight, string displayName = "none")
    {
        public string DisplayName { get; set; } = displayName;
        public double X { get; set; } = x; public double Y { get; set; } = y; public double Z { get; set; } = z; public double Weight { get; set; } = weight;

        public bool IsNodeValid()
        {
            return DisplayName != "none";
        }

        /// <summary>
        /// Visual indicator of the 'weight' of a node
        /// </summary>
        /// <param name="rangeStart">0</param>
        /// <param name="rangeEnd">Largest weight</param>
        /// <param name="nodeValue">Node's neighborhood weight</param>
        /// <returns>double weight</returns>
        public double GetOpacity(double rangeStart, double rangeEnd, double nodeValue)
        {
            if (rangeStart >= rangeEnd) return 0;

            double max = rangeEnd - rangeStart;
            double value = nodeValue - rangeStart;

            double normalized = Math.Max(0, Math.Min(1, value / max));
            double expScale = Math.Pow(normalized, 3);
            double opacity = 10 * expScale;

            opacity = /*Math.Round(*/opacity * 255;

            return opacity;
        }

        public double DistanceTo(Node3D other)
        {
            double dx = X - other.X;
            double dy = Y - other.Y;
            double dz = Z - other.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
    }
}