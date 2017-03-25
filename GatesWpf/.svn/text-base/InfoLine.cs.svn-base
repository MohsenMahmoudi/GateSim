using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;


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
    /// Provides an attached property to specify one-line help information.
    /// Set the attached property on a UIElement, and MouseEnter/MouseLeave will
    /// be applied and used to set the info line value.
    /// </summary>
    public class InfoLine : INotifyPropertyChanged
    {
        private InfoLine() { } // prohibit construction

        private static InfoLine _inst = null;
        private string _infoline;


        /// <summary>
        /// Singleton class has only one instance.
        /// </summary>
        /// <returns></returns>
        public static InfoLine GetInstance()
        {
            if (_inst == null)
                _inst = new InfoLine();

            return _inst;
        }

        /// <summary>
        /// The current line to display, if any.
        /// </summary>
        public string CurrentInfoLine
        {
            get
            {
                return _infoline;
            }
        }

        public static void SetInfo(UIElement element, string value)
        {
            element.SetValue(InfoProperty, value);
        }

        public static string GetInfo(UIElement element)
        {
            return (string)element.GetValue(InfoProperty);
        }

        public static readonly DependencyProperty InfoProperty =
            DependencyProperty.RegisterAttached("Info", typeof(string), typeof(InfoLine), new UIPropertyMetadata("", InfoLine.InfoChanged));

        private static void InfoChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var infoSource = obj as UIElement;
            if (infoSource != null)
            {
                string x = e.NewValue as string;
                if (string.IsNullOrEmpty(x))
                {
                    infoSource.MouseEnter -= GetInstance().infoSource_MouseEnter;
                    infoSource.MouseLeave -= GetInstance().infoSource_MouseLeave;
                }
                else
                {
                    infoSource.MouseEnter += GetInstance().infoSource_MouseEnter;
                    infoSource.MouseLeave += GetInstance().infoSource_MouseLeave;
                }
            }
            
        }

        private void infoSource_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _infoline = "";
            NotifyPropertyChanged("CurrentInfoLine");
        }

        private void infoSource_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _infoline = GetInfo(sender as UIElement);
            NotifyPropertyChanged("CurrentInfoLine");
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #endregion
    }
}
