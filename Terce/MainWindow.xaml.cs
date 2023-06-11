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
        private int score;

        private DispatcherTimer timeoutTimer;
        private bool targetClicked;

        private System.Media.SoundPlayer shootSoundPlayer;


        public MainWindow()
        {
            InitializeComponent();

            // Vytvoření objektu ImageBrush a načtení obrázku
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("img/srdce.jpg", UriKind.Relative));

            // Nastavení objektu ImageBrush jako pozadí Canvasu
            Canvas.Background = imageBrush;

            random = new Random();
            targets = new List<Ellipse>();
            score = 0;

            // Spuštění generování terčů každé 0,5 sekundy
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.5);
            timer.Tick += Timer_Tick;
            timer.Start();


            // Inicializace časovače pro sledování timeoutu
            timeoutTimer = new DispatcherTimer();
            timeoutTimer.Interval = TimeSpan.FromSeconds(1.5);
            timeoutTimer.Tick += TimeoutTimer_Tick;

            shootSoundPlayer = new System.Media.SoundPlayer();
            shootSoundPlayer.SoundLocation = "sounds/sparkle.wav"; 
            
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Generování nového terče
            Ellipse target = GenerateTarget();
            targets.Add(target);
            Canvas.Children.Add(target);

            // Spuštění časovače pro timeout
            timeoutTimer.Start();
            targetClicked = false;
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
            target.Margin = new Thickness(x, y, 0, 0);

            // Nastavení obrázku srdce jako pozadí terče
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("img/srdicko.gif", UriKind.Relative)); 
            target.Fill = imageBrush;


            target.MouseLeftButtonUp += Target_MouseLeftButtonUp;

            return target;
        }

        private void Target_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Zasáhnutí terče - zvýšení skóre
            score++;
            UpdateScoreUI();

            Ellipse target = (Ellipse)sender;
            targets.Remove(target);
            Canvas.Children.Remove(target);

            // Zastavení časovače pro timeout
            timeoutTimer.Stop();
            targetClicked = true;

            shootSoundPlayer.Play();


        }

        private void TimeoutTimer_Tick(object sender, EventArgs e)
        {
            // Timeout - terč nebyl zasažen včas
            if (!targetClicked && targets.Count > 0)
            {
                Ellipse target = targets[0];
                targets.Remove(target);
                Canvas.Children.Remove(target);

                score--;
                UpdateScoreUI();
            }

            // Spuštění dalšího časovače pro generování terčů
            timer.Start();
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Zasáhnutí terče při kliknutí na plátno (pokud je terč na té pozici) - snížení skóre
            Point mousePosition = e.GetPosition(Canvas);
            Ellipse target = GetTargetAtPosition(mousePosition);
            if (target != null)
            {
                score--;
                UpdateScoreUI();

                targets.Remove(target);
                Canvas.Children.Remove(target);

                // Zastavení časovače pro timeout
                timeoutTimer.Stop();
                targetClicked = true;

               
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

        private void UpdateScoreUI()
        {

            ScoreTextBlock.Text = $"Score: {score}";
            ScoreTextBlock.Foreground = Brushes.DeepPink;
            ScoreTextBlock.FontWeight = FontWeights.Medium;

        }


    }
}

