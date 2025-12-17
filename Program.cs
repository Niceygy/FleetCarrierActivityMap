using System.Drawing;

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

        public double DistanceTo(Node3D other)
        {
            double dx = X - other.X;
            double dy = Y - other.Y;
            double dz = Z - other.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
    }

    static class Program
    {
        static async Task Main()
        {
            List<string> systemscsv = await Database.LoadStarSystemsAsync();
            List<string> carriercsv = await Database.LoadCarriersAsync();

            double maxWeight = 0;

            List<Node3D> nodes = LoadSystems(systemscsv, carriercsv, ref maxWeight);

            Console.WriteLine("Loaded to nodes, processing...");

            double radius = 15;
            if (File.Exists("html/plot.csv"))
            {
                Console.WriteLine("Deleted old plot.csv");
                File.Delete("html/plot.csv");
            }
            StreamWriter writer = new("html/plot.csv");
            writer.WriteLine("x,y,z,opacity,label");

            foreach (var node in nodes)
            {
                double neighborhoodWeight = ComputeNeighborhoodWeight(node, nodes, radius);
                double opacity = GetOpacity(0, maxWeight, neighborhoodWeight);
                if (opacity > 0.05)
                {
                    writer.WriteLine($"{node.X},{node.Y},{node.Z},{opacity:F4},{node.DisplayName} has {node.Weight} carriers");
                }
            }

            writer.Close();
            Console.WriteLine("Complete");
        }

        public static List<Node3D> LoadSystems(List<string> starcsv, List<string> fccsv, ref double maxWeight)
        {
            List<Node3D> nodes = [];
            Dictionary<string, double> carriers = [];

            Console.WriteLine("Loading carriers.csv...");

            foreach (string csvline in fccsv)
            {
                string[] line = csvline.Split(",");
                double weight = double.Parse(line[1]);
                carriers.Add(line[0], weight);
                if (weight > maxWeight) { maxWeight = weight; }
            }

            Console.WriteLine("Loading systems.csv...");

            foreach (string csvline in starcsv)
            {
                string[] line = csvline.Split(",");

                string Sys = line[0];
                double X = double.Parse(line[1]);
                double Y = double.Parse(line[2]);
                double Z = double.Parse(line[3]);


                if (carriers.TryGetValue(Sys, out double weight))
                {
                    Node3D newNode = new(X, Y, Z, weight, Sys);
                    nodes.Add(newNode);
                }
            }

            Console.WriteLine($"Loaded {nodes.Count} systems");

            return nodes;
        }

        // Source - https://stackoverflow.com/a/4335157
        // Posted by decyclone
        // Retrieved 2025-12-17, License - CC BY-SA 2.5

        static double GetOpacity(double rangeStart, double rangeEnd, double nodeValue)
        {
            if (rangeStart >= rangeEnd) return 0;

            double max = rangeEnd - rangeStart;
            double value = nodeValue - rangeStart;

            // Exponential scaling: normalized value in [0,1], then apply exponential
            double normalized = Math.Max(0, Math.Min(1, value / max));
            double expScale = Math.Pow(normalized, 2); // You can adjust the exponent (e.g., 2 for quadratic, >2 for steeper)
            double opacity = /*Math.Ceiling(*/(100 * expScale) / 10;

            return opacity;// > 0.2 ? 255 : 1;
        }


        public static double ComputeNeighborhoodWeight(Node3D node, List<Node3D> allNodes, double radius)
        {
            var neighbors = allNodes
                .Where(n => n != node && node.DistanceTo(n) <= radius)
                .ToList();

            if (neighbors.Count == 0) return node.Weight; // fallback

            double weightedSum = 0;
            double weightFactorSum = 0;

            foreach (var neighbor in neighbors)
            {
                double d = node.DistanceTo(neighbor);
                double factor = 1.0 / (d + 1e-6); // avoid divide by zero
                weightedSum += neighbor.Weight * factor;
                weightFactorSum += factor;
            }

            return weightedSum / weightFactorSum;
        }

    }

}
