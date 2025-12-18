using System.Drawing;

namespace FCAT
{
    static class Program
    {
        static async Task Main()
        {
            List<string> systemscsv = await Database.LoadStarSystemsAsync(10);
            List<string> carriercsv = await Database.LoadCarriersAsync(10);

            double maxWeight = 0;

            List<Node3D> nodes = Calc.LoadNodes(systemscsv, carriercsv, ref maxWeight);

            Console.WriteLine("Loaded to nodes, processing...");

            double radius = 15;
            if (File.Exists("html/plot.csv"))
            {
                Console.WriteLine("Deleted old plot.csv");
                File.Delete("html/plot.csv");
            }
            StreamWriter writer = new("html/plot.csv");
            writer.WriteLine("x,y,z,opacity,label");

            Parallel.ForEach(nodes, node =>
            {
                double neighborhoodWeight = Calc.ComputeNeighborhoodWeight(node, nodes, radius);
                double opacity = node.GetOpacity(0, maxWeight, neighborhoodWeight);
                if (opacity > 5)
                {
                    lock (writer)
                    {
                        writer.WriteLine($"{node.X},{node.Y},{node.Z},{opacity:F4},{node.DisplayName} has {node.Weight} carriers");
                    }
                }
            });


            writer.Close();
            Console.WriteLine("Complete");
        }
    }
}
