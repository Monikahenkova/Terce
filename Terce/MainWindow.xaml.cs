using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading;

namespace Terce
{
    public partial class MainWindow : Window
    {
        private Random random;
        private DispatcherTimer timer;
        private List<Ellipse> targets;

        public MainWindow()
        {
            InitializeComponent();

            random = new Random();
            targets = new List<Ellipse>();

            // Spuštění generování terčů každých 1.5 sekundy
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1.5);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Generování nového terče
            Ellipse target = GenerateTarget();
            targets.Add(target);
            Canvas.Children.Add(target);
        }

        private Ellipse GenerateTarget()
        {
            // Generování náhodných hodnot pro velikost a umístění terče
            double size = random.Next(20, 100);
            double x = random.NextDouble() * (Canvas.ActualWidth - size);
            double y = random.NextDouble() * (Canvas.ActualHeight - size);

            // Vytvoření terče
            Ellipse target = new Ellipse();
            target.Width = size;
            target.Height = size;
            target.Fill = Brushes.Pink;
            target.Stroke = Brushes.Black;
            target.StrokeThickness = 1;
            target.Margin = new Thickness(x, y, 0, 0);

            // Přidání obsluhy kliknutí na terč
            target.MouseLeftButtonUp += Target_MouseLeftButtonUp;

            return target;
        }

        private void Target_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Zasáhnutí terče
            Ellipse target = (Ellipse)sender;
            targets.Remove(target);
            Canvas.Children.Remove(target);
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Zasáhnutí terče při kliknutí na plátno (pokud je terč na té pozici)
            Point mousePosition = e.GetPosition(Canvas);
            Ellipse target = GetTargetAtPosition(mousePosition);
            if (target != null)
            {
                targets.Remove(target);
                Canvas.Children.Remove(target);
            }
        }

        private Ellipse GetTargetAtPosition(Point position)
        {
            // Zjištění terče na dané pozici
            foreach (Ellipse target in targets)
            {
                Rect targetBounds = new Rect(target.Margin.Left, target.Margin.Top, target.Width, target.Height);
                if (targetBounds.Contains(position))
                {
                    return target;
                }
            }
            return null;
        }
    }
}
