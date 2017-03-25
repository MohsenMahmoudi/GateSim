using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows;
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

namespace GatesWpf
{
    // things will be called by a non-UI thread
    // from the propagation thread in the circuit
    // So we can't just update the values
    // solution: ask the dispatcher to have the UI do it
    // we use BeginInvoke to ensure the request doesn't block
    // the propagation thread. 

    /// <summary>
    /// Just a quick wrapper class to allow events which occur in a non-UI thread
    /// to be "transferred" over to the UI thread so they can perform UI updates.
    /// </summary>
    public class EventDispatcher
    {
        public static Dispatcher BatchDispatcher { get; set; }
        protected static Dictionary<KeyValuePair<Gates.AbstractGate, PropertyChangedEventHandler>, Action> BatchNotifications = new Dictionary<KeyValuePair<Gates.AbstractGate, PropertyChangedEventHandler>, Action>();
        
        protected static Thread bw;


        protected static void bw_DoWork()
        {
            DateTime LastBatchDispatch = DateTime.Now;
            while (true)
            {
                if (BatchDispatcher != null)
                {
                    List<Action> toBeDispatched = new List<Action>();
                    lock (BatchNotifications)
                    {
                        toBeDispatched.AddRange(BatchNotifications.Values);
                        
                        BatchNotifications.Clear();
                    }

                    BatchDispatcher.BeginInvoke(new Action(() =>
                    {

                        foreach (var act in toBeDispatched)
                            act(); // execute the action from the queue



                    }));
                }
                System.Threading.Thread.Sleep(100);

            }

        }

        public static PropertyChangedEventHandler CreateBatchDispatchedHandler(Gates.AbstractGate g, PropertyChangedEventHandler handler)
        {
            if (bw == null)
            {
                bw = new Thread(bw_DoWork);
                bw.IsBackground = true;
                bw.Priority = ThreadPriority.BelowNormal;
                bw.Start();
            }
            return new PropertyChangedEventHandler((sender, e) =>
            {
                lock (BatchNotifications)
                {
                    BatchNotifications[new KeyValuePair<Gates.AbstractGate, PropertyChangedEventHandler>(g, handler)] = new Action(() => { handler(sender, e); });
                    
                }
            });
        }

       

        public static PropertyChangedEventHandler CreateDispatchedHandler(Dispatcher disp, PropertyChangedEventHandler handler)
        {

            return CreateDispatchedHandler(disp, System.Windows.Threading.DispatcherPriority.ApplicationIdle, handler);
            
        }

        public static PropertyChangedEventHandler CreateDispatchedHandler(Dispatcher disp, DispatcherPriority dp, PropertyChangedEventHandler handler)
        {


            return new PropertyChangedEventHandler((sender, e) =>
            {

                disp.BeginInvoke(handler, dp, sender, e);

            }
                );
        }


    }
}
