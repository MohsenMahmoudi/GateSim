using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 *  Copyright (C) 2011 Steve Kollmansberger
 * 
 *  This file is part of Logic Gate Simulator.
 *
 *  Logic Gate Simulator is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Logic Gate Simulator is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Logic Gate Simulator.  If not, see <http://www.gnu.org/licenses/>.
 */


namespace GatesWpf
{
    /// <summary>
    /// Implements a topological sort for the purpose of ordering IC dependencies.
    /// This allows ICs to be saved and read in a linear order. 
    /// See http://en.wikipedia.org/wiki/Topological_sorting
    /// </summary>
    public class TopologicalSort
    {
        // see http://en.wikipedia.org/wiki/Topological_sorting
        /*
         * L ← Empty list that will contain the sorted nodes
S ← Set of all nodes

function visit(node n)
if n has not been visited yet then
    mark n as visited
    for each node m with an edge from n to m do
        visit(m)
    add n to L

for each node n in S do
visit(n)
*/
        private IEnumerable<Gates.IC> ics;
        private List<Gates.IC> results;
        private List<string> visited;

        private void Visit(Gates.IC ic)
        {
            if (!visited.Contains(ic.Name))
            {
                visited.Add(ic.Name);
                foreach (Gates.AbstractGate ag in ic.Circuit)
                {
                    if (ag is Gates.IC)
                    {
                        Visit((Gates.IC)ag);
                    }
                }
                results.Add(ic);
            }
        }

        /// <summary>
        /// Performs a recursive topological sort of a set of ICs.
        /// The result list may include ICs not in the original ics input
        /// if they are used somewhere within the ics input.
        /// </summary>
        /// <param name="ics"></param>
        /// <returns></returns>
        public List<Gates.IC> Sort(IEnumerable<Gates.IC> ics)
        {
            this.ics = ics;
            results = new List<Gates.IC>();
            visited = new List<string>();

            foreach (Gates.IC ic in ics)
                Visit(ic);

            return results;

        }

        /// <summary>
        /// Performs a recursive topological sort of a set of ICs.
        /// The result list may include ICs not in the original ics input
        /// if they are used somewhere within the ics input.  All results
        /// will be dereferenced against the IC list provided so that
        /// the result IC instances are "canonical".
        /// </summary>
        /// <param name="ics"></param>
        /// <param name="icl"></param>
        /// <returns></returns>
        public List<UIGates.IC> Sort(IEnumerable<UIGates.IC> ics, ICList icl)
        {
            List<Gates.IC> gics = new List<Gates.IC>();
            foreach (UIGates.IC ic in ics)
                gics.Add((Gates.IC)ic.AbGate);

            Sort(gics);

            List<UIGates.IC> res = new List<GatesWpf.UIGates.IC>();
            foreach (Gates.IC gic in results)
            {
                res.Add(icl.GetIC(gic.Name));
            }
            return res;
        }

        /// <summary>
        /// Sort all ICs used within a given IC, deferencing against an IC list
        /// for canonical instances.
        /// </summary>
        /// <param name="ic"></param>
        /// <param name="icl"></param>
        /// <returns></returns>
        public List<UIGates.IC> Sort(UIGates.IC ic, ICList icl)
        {
            return Sort(new UIGates.IC[] { ic }, icl);
        }


        
    }
}
