using System;
using System.Collections;
using System.Text;

using FieldStat.CodeModel;

namespace FieldStat
{
    class CodeRank
    {
        //double errorTolerance = 9.8813129168249309e-324;
        int maxIterations = 50;
        double damping = 0.85;

        private Hashtable GraphToHashtable(NodeGraph graph)
        {
            Hashtable rankTable = new Hashtable();
            foreach (Node n in graph.Nodes)
            {
                rankTable[n] = n.Rank;
            }
            return rankTable;
        }

        private void HashtableToGraph(NodeGraph graph, Hashtable rankTable)
        {
            foreach (Node n in graph.Nodes)
            {
                n.Rank = (double)rankTable[n];
            }
        }

        public void CalculateRank( NodeGraph graph )
        {
            double error = 0.0;

            Hashtable rankTable = GraphToHashtable(graph);
            // Update Rank to this table and then swap after each iteration.
            Hashtable tempTable = GraphToHashtable(graph);

            for( int iteration = 0; iteration < maxIterations; iteration++ )
            {
                foreach( Node n in graph.Nodes )
                {
                    double rank = (double)rankTable[n];

                    // compute ARi
                    double r = 0.0;
                    foreach( Node c in n.Children )
                    {
                        if( c.IsLeaf )
                            continue;
                        double childRank = (double)rankTable[c];
                        r += childRank / n.Children.Count;
                    }
                    // add sourceRank
                    double newRank = 1 - damping + damping * r;
                    tempTable[n] = newRank;

                    // compute deviation
                    error += Math.Abs( rank - newRank );
                }
                // Update Rank for this iteration.  (Swap memories)
                Hashtable temp = rankTable;
                rankTable = tempTable;
                tempTable = temp;
            }
            HashtableToGraph(graph, rankTable);
        }
    }
}
