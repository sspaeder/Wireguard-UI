using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WireGuard.Core.PlugIn
{
    /// <summary>
    /// Basic class for pages to implement some basics
    /// </summary>
    public class BasePage : Page
    {
        /// <summary>
        /// Variable to control the animation time
        /// </summary>
        private int animationTime = 600;

        /// <summary>
        /// Variable to slow down the animation
        /// </summary>
        private double decelaration = 0.98;

        /// <summary>
        /// Method to animate the page in
        /// </summary>
        public async Task AnimateIn()
        {
            Storyboard sb = new Storyboard();

            sb.MarginAnimation(
                new System.Windows.Thickness(2*ActualWidth, 0, 0, 0),
                new System.Windows.Thickness(0,0,0,0),
                animationTime,
                decelaration
                );

            sb.Begin(this);

            await Task.Delay(animationTime);
        }

        /// <summary>
        /// Method to animate the page out
        /// </summary>
        public async Task AnimateOut()
        {
            Storyboard sb = new Storyboard();

            sb.MarginAnimation(
                new System.Windows.Thickness(0, 0, 0, 0),
                new System.Windows.Thickness(-2*ActualWidth, 0, 0, 0),
                animationTime,
                decelaration
                );

            sb.Begin(this);

            await Task.Delay(animationTime);
        }
    }
}
