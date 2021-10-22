using System;

namespace CovidMeetsHogwarts
{
    public class Edge
    {
        // Attributes
        private (Node source, Node destination) endpoints; // source and destination
        // can be interchanged because we're working with undirected graphs
        
        // Methods
        // - constructor
        public Edge(Node source, Node destination)
        {
            endpoints = (source, destination);
        }
        
        // - getters
        public (Node source, Node destination) GetEndpoints()
        {
            return endpoints;
        }

        // - == and != operators overload
        public static bool operator== (Edge edge1, Edge edge2)
        {
            if (edge1 is null)
            {
                if (edge2 is null)
                    return true;
                else
                {
                    return false;
                }
            }
            else
            {
                if (edge2 is null)
                    return false;
                else
                {
                    return (edge1.endpoints.source == edge2.endpoints.source &&
                            edge1.endpoints.destination == edge2.endpoints.destination)
                           || (edge1.endpoints.source == edge2.endpoints.destination &&
                               edge1.endpoints.destination == edge2.endpoints.source);
                }
            }
        }

        public static bool operator!= (Edge edge1, Edge edge2)
        {
            if (edge1 is null)
            {
                if (edge2 is null)
                    return false;
                else
                {
                    return true; 
                }
            }
            else
            {
                if (edge2 is null)
                    return true;
                else
                {
                    return (edge1.endpoints.source != edge2.endpoints.source &&
                            edge1.endpoints.destination != edge2.endpoints.destination) &&
                           (edge1.endpoints.source != edge2.endpoints.destination  ||
                            edge1.endpoints.destination != edge2.endpoints.source);
                }
            }
        }

        /// <summary>
        /// represent edge by its end points in DOT language
        /// </summary>
        /// <returns>string describing this edge in DOT language followed by a newline character</returns>
        public override string ToString()
        {
            return "    " + endpoints.source + " " + "--" + " " + endpoints.destination + "\n";
        }
    }
}