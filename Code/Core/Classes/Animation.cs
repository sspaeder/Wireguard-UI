using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace WireGuard.Core
{
    /// <summary>
    /// Class for Anmation helper functions
    /// </summary>
    static class Animation
    {
        /// <summary>
        /// Mehtod to FadeIn a control/window
        /// </summary>
        /// <param name="storyBoard">Storyboard which the animation should be added</param>
        /// <param name="time">Time in milliseconds in which the animation will take place</param>
        public static void FadeIn(this Storyboard storyBoard, int time = 350)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0;
            da.To = 1;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(time));

            Storyboard.SetTargetProperty(da, new PropertyPath("Opacity"));
            storyBoard.Children.Add(da);
        }

        /// <summary>
        /// Mehtod to FadeOut a control/window
        /// </summary>
        /// <param name="storyBoard">Storyboard which the animation should be added</param>
        /// <param name="time">Time in milliseconds in which the animation will take place</param>
        public static void FadeOut(this Storyboard storyBoard, int time = 350)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = 1;
            da.To = 0;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(time));

            Storyboard.SetTargetProperty(da, new PropertyPath("Opacity"));
            storyBoard.Children.Add(da);
        }

        /// <summary>
        /// Method to animate the margin of an element
        /// </summary>
        /// <param name="storyBoard">Storyboard which the animation should be added</param>
        /// <param name="from">The starting point of the element to animate</param>
        /// <param name="to">The target point of the element to animate</param>
        /// <param name="time">Time in milliseconds in which the animation will take place</param>
        /// <param name="deceleration">Slows down the animation timeline</param>
        public static void MarginAnimation(this Storyboard storyBoard, Thickness from, Thickness to, int time = 350, double deceleration = 0)
        {
            ThicknessAnimation ta = new ThicknessAnimation();
            ta.From = from;
            ta.To = to;
            ta.Duration = new Duration(TimeSpan.FromMilliseconds(time));

            storyBoard.DecelerationRatio = deceleration;

            Storyboard.SetTargetProperty(ta, new PropertyPath("Margin"));
            storyBoard.Children.Add(ta);
        }
    }
}
