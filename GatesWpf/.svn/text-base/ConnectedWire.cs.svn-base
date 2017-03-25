using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;


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
    /// This wire can update its position based on the gates it is actually connected to.
    /// It also updates its value when the origin terminal changes value.
    /// </summary>
    public class ConnectedWire : Wire
    {

        private Gate.TerminalID origin, dest;
        private Gates.AbstractGate originGate, destGate;

        public Gate.TerminalID OriginTerminalID
        {
            get
            {
                return origin;
            }
        }

        public Gate.TerminalID DestTerminalID
        {
            get
            {
                return dest;
            }
        }

        public  Gates.AbstractGate OriginGate 
        {
            get
            {
                return originGate;
            }
        }

        public Gates.AbstractGate DestinationGate
        {
            get
            {
                return destGate;
            }
        }
        public ConnectedWire(Gates.AbstractGate originGate, Gate.TerminalID origin, Gates.AbstractGate destGate, Gate.TerminalID dest)
            : base()
        {

            if (origin.isInput || !dest.isInput)
            {
                throw new ArgumentException("Can only connect output (origin) to input (dest)");
            }

            Value = false;
            this.originGate = originGate;
            this.destGate = destGate;
            this.origin = origin;
            this.dest = dest;
            //originGate.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(originGate_PropertyChanged);
            originGate.PropertyChanged += EventDispatcher.CreateBatchDispatchedHandler(originGate, originGate_PropertyChanged);
            Connect();
            
            
        }

        private void originGate_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Value = ShowTrueFalse && originGate.Output[origin.ID];
        }

        public void Connect()
        {
            Origin = origin.t.TranslatePoint(new Point(5, 5), this);
            Destination = dest.t.TranslatePoint(new Point(5, 5), this);
        }


        private bool _showTF = true;
        public bool ShowTrueFalse
        {
            get
            {
                return _showTF;
            }
            set
            {
                _showTF = value;
                originGate_PropertyChanged(null, null);
            }
        }
    }
}
