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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Choinka
{
    public partial class MainWindow : Window
    {

        private IkonaWZasobnikach ikonaWZasobniku;
        private DispatcherTimer countDownTimer;
        private DateTime christmasDate;
        private bool isCountDownEnabled = true;

        private DispatcherTimer snowTimer;
        private bool isSnowEnabled = true;

        private readonly Random random = new Random();
        private List<string> wishes;

        private MediaPlayer musicPlayer;
        private bool isMusicEnabled = true;

        private MediaPlayer soundEffectPlayer; 
        private List<string> musicPlaylist;    
        private int currentTrackIndex = 0;

        private DispatcherTimer sparkTimer;
        private Random sparkRandom = new Random();
        private bool isBlizzardMode = false; 

        public MainWindow()
        {
            LoadSettings();
            InitializeComponent();
            ikonaWZasobniku = new IkonaWZasobnikach(this);
            InitializeWishes();
            InitializeCountDown();
            InitializeSnow();
            InitializeMusic();
            InitializeSoundEffects();
            InitializeSparks();
        }

        private void LoadSettings()
        {
            isSnowEnabled = Properties.Settings.Default.IsSnowEnabled;
            isMusicEnabled = Properties.Settings.Default.IsMusicEnabled;
            isCountDownEnabled = Properties.Settings.Default.IsCountDownEnabled;
        }

        #region API dla ikony w zasobach

        public bool IsSnowEnabled => isSnowEnabled;
        public bool IsCountDownEnabled => isCountDownEnabled;
        public bool IsMusicEnabled => isMusicEnabled;

        public void SetSnowEnabled(bool enabled)
        {
            isSnowEnabled = enabled;

            if(snowTimer == null)
                return;
            if (isSnowEnabled)
                snowTimer.Start();
            else
            {
                snowTimer.Stop();
                if (SnowCanvas != null)
                    SnowCanvas.Children.Clear();
            }
        }

        public void SetCountDownEnabled(bool enabled)
        {
            isCountDownEnabled = enabled;

            if(countDownTimer == null)
                return;

            if(isCountDownEnabled)
            {
                countDownTimer.Start();
                CountDownPanel.Visibility = Visibility.Visible;
                UpdateCountDownText();
            }
            else
            {
                countDownTimer.Stop();

                if (CountDownText != null)
                    CountDownText.Text = "";
                CountDownPanel.Visibility = Visibility.Hidden;
            }
        }

        public void SetMusicEnabled(bool enabled)
        {
            isMusicEnabled = enabled;
            if (musicPlayer == null)
                return;

            if(isMusicEnabled)
            {
                try
                {
                    musicPlayer.Play();
                }
                catch 
                {
                    isMusicEnabled = false;
                }
            }
            else

            {
                musicPlayer.Stop();
            }
        }

        public void ShowWishNow()
        {
            ShowRandomWish();
        }

        #endregion

        #region Życzenia

        private void InitializeWishes()
        {
            wishes = new List<string>()
            {
                "Wesołych Świąt i Szczęśliwego Nowego Roku!",
                "Dużo zdrowia, spokoju i ciepła rodzinnego w te Święta!",
                "Spełnienia marzeń i samych dobrych chwil w nadchodzącym roku!",
                "Niech te Święta będą pełne radości i uśmiechu!",
                "Magicznych Świąt Bożego Narodzenia!"
            };
        }

        private void ShowRandomWish()
        {
            if (soundEffectPlayer != null && soundEffectPlayer.Source != null)
            {
                soundEffectPlayer.Position = TimeSpan.Zero; 
                soundEffectPlayer.Play();
            }
            int index = random.Next(wishes.Count);
            string wish = wishes[index];

            MessageBox.Show(wish, "Choinka 2026", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region Przenoszenie okna
        private bool czyPrzenoszenie = false;
        private Cursor kursor = null;
        private Point punktPoczątkowy;


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                ShowRandomWish();
                return;
            }

            if (e.ButtonState == Mouse.LeftButton)
            {
                czyPrzenoszenie = true;
                kursor= this.Cursor;
                Cursor = Cursors.Hand;
                punktPoczątkowy = e.GetPosition(this);
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (czyPrzenoszenie)
            {
                Vector przesunięcie = e.GetPosition(this) - punktPoczątkowy;
                Left += przesunięcie.X;
                Top += przesunięcie.Y;
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (czyPrzenoszenie)
            {
                Cursor = kursor;
                czyPrzenoszenie = false;
            }
        }
        #endregion

        #region zamykanie okna
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        bool zakończonaAnimacjaZnikania = false;
        private void okno_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!zakończonaAnimacjaZnikania)
            {
                Storyboard scenorysZnikaniaOkna = this.Resources["scenorysZnikaniaOkna"] as Storyboard;
                scenorysZnikaniaOkna.Begin();
                e.Cancel = true;
            }
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            Properties.Settings.Default.IsSnowEnabled = isSnowEnabled;
            Properties.Settings.Default.IsMusicEnabled = isMusicEnabled;
            Properties.Settings.Default.IsCountDownEnabled = isCountDownEnabled;
            Properties.Settings.Default.Save();

            zakończonaAnimacjaZnikania = true;
            ikonaWZasobniku.Usuń();
            Close();
        }
        #endregion

        #region Odliczanie do Świąt

        private void InitializeCountDown()
        {
            christmasDate = GetNextChristmasDate();

            countDownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            countDownTimer.Tick += countDownTimer_Tick;

            if (isCountDownEnabled)
            {
                countDownTimer.Start();
            }

            UpdateCountDownText();
        }

        private void UpdateCountDownText()
        {
            if (CountDownText == null)
            {
                return;
            }

            TimeSpan remaining = christmasDate - DateTime.Now;

            if (remaining <= TimeSpan.Zero)
            {
                CountDownText.Text = "To już Święta! 🎄";
                return;
            }

            int days = remaining.Days;
            int hours = remaining.Hours;
            int minutes = remaining.Minutes;

            CountDownText.Text = $"Do Świąt zostało: {days} dni {hours} godzin {minutes} minut";
        }

        private void countDownTimer_Tick(object sender, EventArgs e)
        {
            UpdateCountDownText();
        }

        private DateTime GetNextChristmasDate()
        {
            int year = DateTime.Now.Year;

            DateTime thisYearCristmas = new DateTime(year, 12, 24, 0, 0, 0);

            if(DateTime.Now > thisYearCristmas)
            {
                return new DateTime(year + 1, 12, 24, 0, 0, 0);
            }

            return thisYearCristmas;
        }

        #endregion

        #region śnieg
        
        private void InitializeSnow()
        {
            snowTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(350)
            };
            snowTimer.Tick += snowTimer_Tick;

            if (isSnowEnabled) 
                snowTimer.Start();
        }

        private void snowTimer_Tick(object sender, EventArgs e)
        {
            CreateSnowflake();
        }

        private void CreateSnowflake()
        {
            if (SnowCanvas == null)
                return;

            double windowWidth = ActualWidth;
            double windowHeight = ActualHeight;

            if (windowWidth <= 0 || windowHeight <= 0)
            {
                return;
            }

            double size = random.Next(3, 9);
            double startX = random.NextDouble() * (windowWidth - size);

            Ellipse snowflake = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = Brushes.White,
                Opacity = 0.8
            };

            Canvas.SetLeft(snowflake, startX);
            Canvas.SetTop(snowflake, -size);

            SnowCanvas.Children.Add(snowflake);

            double endY = windowHeight + size;
            double durationSeconds = isBlizzardMode ? random.Next(1, 3) : random.Next(6, 11);

            DoubleAnimation fallAnimation = new DoubleAnimation
            {
                From = -size,
                To = endY,
                Duration = TimeSpan.FromSeconds(durationSeconds),
                FillBehavior = FillBehavior.Stop
            };

            fallAnimation.Completed += (s, e) =>
            {
                SnowCanvas.Children.Remove(snowflake);
            };

            snowflake.BeginAnimation(Canvas.TopProperty, fallAnimation);
        }

        #endregion

        #region muzyka świąteczna

        private void InitializeMusic()
        {
            musicPlayer = new MediaPlayer();
            musicPlaylist = new List<string>
            {
                "christmas.mp3",   
                "christmas2.mp3",
                "christmas3.mp3"
            };

            musicPlayer.MediaEnded += (s, e) =>
            {
                if (isMusicEnabled)
                {
                    PlayNextSong();
                }
            };

            PlayTrack(currentTrackIndex);
        }

        private void PlayTrack(int index)
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string songName = musicPlaylist[index];
                string musicPath = System.IO.Path.Combine(baseDir, "Resources", "songs", songName);

                if (System.IO.File.Exists(musicPath))
                {
                    musicPlayer.Open(new Uri(musicPath, UriKind.Absolute));
                    musicPlayer.Volume = 0.5;
                    if (isMusicEnabled)
                    {
                        musicPlayer.Play();
                    }
                }
            }
            catch 
            {
            }
        }
        public void PlayNextSong()
        {
            currentTrackIndex++;
            if (currentTrackIndex >= musicPlaylist.Count)
            {
                currentTrackIndex = 0;
            }

            PlayTrack(currentTrackIndex);
        }
        

        private void InitializeSoundEffects()
        {
            soundEffectPlayer = new MediaPlayer();
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string soundPath = System.IO.Path.Combine(baseDir, "Resources", "songs", "hohoho.mp3");

                if (System.IO.File.Exists(soundPath))
                {
                    soundEffectPlayer.Open(new Uri(soundPath, UriKind.Absolute));
                    soundEffectPlayer.Volume = 1.0;
                }
            }
            catch {}
        }

        #endregion

        #region iskry i ogni
        private void InitializeSparks()
        {
            sparkTimer = new DispatcherTimer();
            sparkTimer.Interval = TimeSpan.FromMilliseconds(60); 
            sparkTimer.Tick += SparkTimer_Tick;
        }

        private void SparkTimer_Tick(object sender, EventArgs e)
        {
            double centerX = Canvas.GetLeft(StarHotspot) + (StarHotspot.Width / 2);
            double centerY = Canvas.GetTop(StarHotspot) + (StarHotspot.Height / 2);

            Ellipse spark = new Ellipse
            {
                Width = sparkRandom.Next(3, 7), 
                Height = sparkRandom.Next(3, 7),
                Fill = Brushes.Gold, 
                Opacity = 1
            };
            spark.Effect = new System.Windows.Media.Effects.DropShadowEffect { Color = Colors.Orange, BlurRadius = 8, ShadowDepth = 0 };

            Canvas.SetLeft(spark, centerX);
            Canvas.SetTop(spark, centerY);
            SparkCanvas.Children.Add(spark);

            double endX = centerX + sparkRandom.Next(-100, 100);
            double endY = centerY + sparkRandom.Next(-50, 120); 

            DoubleAnimation moveX = new DoubleAnimation(centerX, endX, TimeSpan.FromSeconds(0.7));
            DoubleAnimation moveY = new DoubleAnimation(centerY, endY, TimeSpan.FromSeconds(0.7));

            DoubleAnimation fade = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.7));

            fade.Completed += (s, ev) => SparkCanvas.Children.Remove(spark);

            spark.BeginAnimation(Canvas.LeftProperty, moveX);
            spark.BeginAnimation(Canvas.TopProperty, moveY);
            spark.BeginAnimation(OpacityProperty, fade);
        }

        private void StarHotspot_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation glowAnim = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(400));
            glowAnim.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };
            StarGlow.BeginAnimation(OpacityProperty, glowAnim);

            sparkTimer.Start();

            isBlizzardMode = true;
            if (snowTimer != null && isSnowEnabled) snowTimer.Interval = TimeSpan.FromMilliseconds(70); 

            ToggleTreeLights(true);
        }

        private void StarHotspot_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation glowAnim = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(600));
            StarGlow.BeginAnimation(OpacityProperty, glowAnim);

            sparkTimer.Stop();

            isBlizzardMode = false;
            if (snowTimer != null && isSnowEnabled) snowTimer.Interval = TimeSpan.FromMilliseconds(350); // Обычная скорость

            ToggleTreeLights(false);
        }
        private void ToggleTreeLights(bool turnOn)
        {
            double targetOpacity = turnOn ? 1.0 : 0.3;
            Color targetShadowColor = turnOn ? Colors.Gold : Colors.Transparent;

            foreach (UIElement child in LightsCanvas.Children)
            {
                if (child is Ellipse bulb)
                {
                    DoubleAnimation anim = new DoubleAnimation(targetOpacity, TimeSpan.FromMilliseconds(500));
                    bulb.BeginAnimation(OpacityProperty, anim);

                    if (bulb.Effect is System.Windows.Media.Effects.DropShadowEffect shadow)
                    {
                        if (shadow.IsFrozen)
                        {
                            shadow = shadow.Clone(); 
                            bulb.Effect = shadow;    
                        }
                        ColorAnimation shadowAnim = new ColorAnimation(targetShadowColor, TimeSpan.FromMilliseconds(500));
                        shadow.BeginAnimation(System.Windows.Media.Effects.DropShadowEffect.ColorProperty, shadowAnim);
                    }
                }
            }
        }
        #endregion

    }
}
