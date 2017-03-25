using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;
using System.ComponentModel;
using GatesWpf.UIGates;


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
    /// Allows UI elements to be faded in and out.
    /// </summary>
    public static class Fader
    {

        public static void AnimateOpacity(double dest, params UIElement[] target)
        {
            if (dest > 0)
                foreach (UIElement t in target)
                    t.Visibility = Visibility.Visible;

            DoubleAnimation da = new DoubleAnimation(dest, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
            da.AccelerationRatio = 0.2;
            da.DecelerationRatio = 0.2;

            IEnumerable<Storyboard> sbs = target.Select(t =>
            {
                Storyboard sb = new Storyboard();
                sb.Children.Add(da);

                Storyboard.SetTarget(da, t);
                Storyboard.SetTargetProperty(da, new PropertyPath(UIElement.OpacityProperty));
                sb.FillBehavior = FillBehavior.Stop;

                sb.Completed += (sender2, e2) =>
                {
                    t.Opacity = dest;
                    if (dest == 0)
                        t.Visibility = Visibility.Collapsed;

                };

                return sb;
            });

            foreach (Storyboard sb in sbs)
            {
                sb.Begin();
            }


        }

        public static void AnimateGateOpacity(double dest, params Gate[] target)
        {
            if (dest > 0)
                foreach (UIElement t in target)
                    t.Visibility = Visibility.Visible;

            DoubleAnimation da = new DoubleAnimation(dest, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
            da.AccelerationRatio = 0.2;
            da.DecelerationRatio = 0.2;

            IEnumerable<Storyboard> sbs = target.Select(t =>
            {
                Storyboard sb = new Storyboard();
                sb.Children.Add(da);

                Storyboard.SetTarget(da, t);
                Storyboard.SetTargetProperty(da, new PropertyPath(UIElement.OpacityProperty));
                sb.FillBehavior = FillBehavior.Stop;

                sb.Completed += (sender2, e2) =>
                {
                    t.Opacity = dest;
                    if (dest == 0)
                        t.Visibility = Visibility.Collapsed;

                };

                return sb;
            });

            foreach (Storyboard sb in sbs)
            {
                sb.Begin();
            }
        }
    }
}
