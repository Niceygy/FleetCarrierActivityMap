namespace FCAT
{
    static class Calc
    {
        public static List<Node3D> LoadNodes(List<string> starcsv, List<string> fccsv, ref double maxWeight)
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