using ArmadilloAssault.Configuration.Scenes;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.GameState.Battle.Environment.Flows
{
    public static class FlowManager
    {
        public static FlowJson FlowConfiguration { get; private set; }
        public static List<Flow> Flows { get; private set; }

        public static void Initialize(FlowJson flowJson)
        {
            FlowConfiguration = flowJson;
            Flows = [];

            if (flowJson != null)
            {
                AddNewFlow(0);
                AddNewFlow(FlowConfiguration.Size.X * 2);
            }
        }

        public static void UpdateFlows()
        {
            Flows.RemoveAll(flow => flow.X > 1920);

            if (Flows.All(flow => flow.X >= 0) && FlowConfiguration != null)
            {
                AddNewFlow(-FlowConfiguration.Size.X * 2);
            }

            foreach (var flow in Flows)
            {
                flow.X++;
            }
        }

        private static void AddNewFlow(int x)
        {
            Flows.Add(new Flow(FlowConfiguration, x));
        }
    }
}
