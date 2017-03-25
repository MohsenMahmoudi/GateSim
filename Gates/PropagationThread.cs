using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


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

namespace Gates
{
    /// <summary>
    /// A single thread to manage and the queue of circuit data propagations.
    /// </summary>
    public class PropagationThread 
    {
        /// <summary>
        /// The amount of time, in half-milliseconds, to sleep after each propagation
        /// </summary>
        public static int SLEEP_TIME = 1;
        

        protected Thread pt;
        private SyncQueue.ConcurrentQueue<KeyValuePair<Terminal, Terminal>> toFlip;
        protected static PropagationThread inst;
        

        /// <summary>
        /// Retrieve the instance of the thread, creating and starting it if needed.
        /// </summary>
        public static PropagationThread Instance
        {
            get
            {
                if (inst == null) inst = new PropagationThread();
                return inst;
            }
        }

        /// <summary>
        /// Add a wire propagation to the queue
        /// </summary>
        /// <param name="wire"></param>
        public void Enqueue(KeyValuePair<Terminal, Terminal> wire)
        {
            toFlip.Enqueue(wire);
        }

        /// <summary>
        /// Remove all items from the queue.  Be sure to stop all circuits first
        /// to avoid a race condition.
        /// </summary>
        public void Clear()
        {
            toFlip.Clear();
        }


        protected PropagationThread () 
        {
            toFlip = new SyncQueue.ConcurrentQueue<KeyValuePair<Terminal, Terminal>>();

            pt = new Thread(ThreadMethod);
            pt.Priority = ThreadPriority.BelowNormal;
            pt.IsBackground = true;
            pt.Start();
        }

        /// <summary>
        /// A thread worker which monitors all queued state changes and implements them.
        /// This technique avoids a "depth-first" state change which
        /// could result in a local loop without appropriate global effect.
        /// </summary>
        private void ThreadMethod()
        {
            int SleepSteps = 0;

            KeyValuePair<Terminal, Terminal> wire;
            while (true)
            {
                wire = toFlip.Dequeue();
                if (wire.Key.gate[wire.Key.portNumber] != wire.Value.gate.Output[wire.Value.portNumber])
                    wire.Key.gate[wire.Key.portNumber] = wire.Value.gate.Output[wire.Value.portNumber];

                if (SLEEP_TIME >= 40)
                    Thread.Sleep(SLEEP_TIME / 2); // cut times in half
                else
                {
                    // to help slower times be accepted by the computer
                    // it has somewhat low resolution
                    // if we base it on 20 here, then from 20 to 19 speed doubles
                    // reason: 19 takes two steps to reach 20 / 19 (0, then 1)
                    // so it sleeps 10 avg!
                    // sol: make it larger range

                    // 100 / 19 ~ 5, so avg is now 20
                    // CHANGE: making this 200 instead of 100 so all time is halved
                    if (SleepSteps > 200 / SLEEP_TIME)
                    {
                        Thread.Sleep(100);
                        SleepSteps = 0;
                    }
                    else SleepSteps++;
                    /*DateTime begin = DateTime.Now;
                    do
                    {
                        Thread.Sleep(1);
                    } while ((DateTime.Now - begin).TotalMilliseconds < SLEEP_TIME);
                    */


                }
            }
        }

        /// <summary>
        /// Waits until the propagation queue is empty.
        /// If there is an oscillation in effect, or any clock
        /// is present, this will wait forever.
        /// </summary>
        /// <returns>true if there were outstanding propagations; false if no waiting was needed</returns>
        public bool WaitOnPropagation()
        {
            

            bool didWait, waited = false;
            do
            {
               

                didWait = !toFlip.IsWaiting() || toFlip.HasItems();
                if (didWait)
                    waited = true;

            
                Thread.Sleep(10); // avoid heavy busy-wait
            } while (didWait);



            return waited;
        }
        
    }
}
