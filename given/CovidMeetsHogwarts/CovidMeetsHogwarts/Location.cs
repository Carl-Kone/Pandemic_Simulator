using System;
using System.Collections.Generic;

namespace CovidMeetsHogwarts
{
    public class Location
    {
        // Attributes
        private Graph map; // Marauder's map
        private List<Human> humans; // students, teachers, staff, ...

        // Methods
        // - constructor
        public Location(Graph map, int numberOfHumans, bool randomValues)
        {
            this.map = map;
            humans = new List<Human>();
            
            while (numberOfHumans > 0)
            {
                AddHuman(randomValues);
                numberOfHumans--;
            }
                
        }
        
        // - getters
        public List<Human> GetHumans()
        {
            return humans;
        }

        /// <summary>
        /// create a human and add it to 'humans' list.
        /// if given parameter randomValues is set to true,
        /// then set hygiene, socialDistance and travellingRate attributes to random values.
        /// set them to the predefined constants in Human.cs otherwise (e.g. GLOBAL_HYGIENE).
        /// </summary>
        /// <param name="randomValues">boolean that decides whether random values are used or not</param>
        void AddHuman(bool randomValues)
        {
            Human human;
            Random rand = new Random();
            if (randomValues)
            {
                human = new Human(rand.NextDouble(),rand.NextDouble(),rand.NextDouble());
            }
            else
            {
                human = new Human(Human.GLOBAL_SOCIAL_DISTANCE,Human.GLOBAL_SOCIAL_DISTANCE,Human.GLOBAL_TRAVELLING_RATE);
            }
            
            int randnum = rand.Next(0,  map.GetNodes().Count);
            Node randNode = map.GetNodes()[randnum];
            human.SetCurrentSpot(randNode);
            randNode.GetHumans().Add(human);
            humans.Add(human);
        }

        /// <summary>
        /// translate location's graph in dot language so it could be used
        /// in generated DOT file with desired format.
        /// </summary>
        /// <returns>string of dot language that represents this location's graph</returns>
        public override string ToString()
        { 
            // get total of each SIR
            double totalSusceptible = 0;
            double totalInfectious = 0;
            double totalRemoved = 0;

            // get all the nodes DOT format along the way
            string nodesFormat = "";
            
            foreach (var node in map.GetNodes())
            {
                totalSusceptible += node.GetSIRCount(Human.SIR.SUSCEPTIBLE);
                totalInfectious += node.GetSIRCount(Human.SIR.INFECTIOUS);
                totalRemoved += node.GetSIRCount(Human.SIR.REMOVED);
                
                nodesFormat += node.Format();
            }

            int total = humans.Count;
            double standardHeight = 100; // of a row in a node's table
            
            // get the beginning of the graph's declaration in DOT language
            string header =  string.Format("graph {0} {{\n" +
                                 "\trankdir = LR;\n" +
                                 "\tlayout = \"fdp\";\n" +
                                 "\t{0}\n" +
                                 "\t[\n" +
                                 "\t\tshape = none\n" +
                                 "\t\tpos = \"0,0\"\n" +
                                 "\t\tlabel = <<table border=\"1\" cellspacing=\"0\">\n" +
                                 "\t\t\t    <th><td port=\"port1\" border=\"1\">{0}</td></th>\n" +
                                 "\t\t\t    <tr><td port=\"port2\" border=\"1\" bgcolor=\"lightskyblue\" height=\"{1}\">{2}</td></tr>\n" +
                                 "\t\t\t    <tr><td port=\"port3\" border=\"1\" bgcolor=\"tomato\" height=\"{3}\">{4}</td></tr>\n" +
                                 "\t\t\t    <tr><td port=\"port4\" border=\"1\" bgcolor=\"gray80\" height=\"{5}\">{6}</td></tr>\n" +
                                 "\t\t        </table>>\n" +
                                 "\t]\n", 
                                 map.GetName(), 
                                 totalSusceptible / total * standardHeight, 
                                 totalSusceptible,
                                 totalInfectious / total * standardHeight, 
                                 totalInfectious,
                                 totalRemoved / total * standardHeight, 
                                 totalRemoved);

            // get all the edges DOT format
            string edgesFormat = "";
            foreach (var edge in map.GetEdges())
            {
                edgesFormat += edge;
            }

            // combine all the formats
            string res = header + nodesFormat + edgesFormat + "}";
            return res;
        }
    }
}