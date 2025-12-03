using System;
using System.Drawing;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;

namespace Choinka
{
    class IkonaWZasobnikach
    {
        private NotifyIcon notifyIcon;
        private MainWindow okno;

        public IkonaWZasobnikach(MainWindow okno)
        {
            this.okno = okno;

            string nazwaIkony = "santa.ico";
            string nazwaAplikacji = Application.ProductName;

            System.Windows.Resources.StreamResourceInfo sri =
                System.Windows.Application.GetResourceStream(
                    new Uri("/" + nazwaAplikacji + ";component/Resources/icony/" + nazwaIkony,
                        UriKind.RelativeOrAbsolute));

            Icon ikona = new Icon(sri.Stream);

            ContextMenuStrip menu = TworzMenu();

            notifyIcon = new NotifyIcon
            {
                Icon = ikona,
                Text = "Choinka 2026",
                ContextMenuStrip = menu,
                Visible = true
            };

            notifyIcon.DoubleClick += (s, e) =>
            {
                notifyIcon.BalloonTipTitle = "Choinka 2026";
                notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon.BalloonTipText = "Wesołych Świąt!";
                notifyIcon.ShowBalloonTip(3000);
            };

            okno.MouseRightButtonDown += (s, e) =>
            {
                System.Windows.Point p = okno.PointToScreen(e.GetPosition(okno));
                menu.Show((int)p.X, (int)p.Y);
            };
        }

        private ContextMenuStrip TworzMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            ToolStripMenuItem ukryjItem = new ToolStripMenuItem("Ukryj");
            ukryjItem.Click += (s, e) => { okno.Hide(); };
            menu.Items.Add(ukryjItem);

            ToolStripMenuItem przywrocItem = new ToolStripMenuItem("Przywróć");
            przywrocItem.Click += (s, e) => { okno.Show(); };
            menu.Items.Add(przywrocItem);

            ToolStripMenuItem zamknijItem = new ToolStripMenuItem("Zamknij");
            zamknijItem.Click += (s, e) => { okno.Close(); };
            menu.Items.Add(zamknijItem);

            menu.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem wishItem = new ToolStripMenuItem("Pokaż życzenia");
            wishItem.Click += (s, e) =>
            {
                okno.ShowWishNow();
            };
            menu.Items.Add(wishItem);

            ToolStripMenuItem snowItem = new ToolStripMenuItem("Śnieg");
            snowItem.CheckOnClick = true;
            snowItem.Checked = okno.IsSnowEnabled;
            snowItem.CheckedChanged += (s, e) =>
            {
                okno.SetSnowEnabled(snowItem.Checked);
            };
            menu.Items.Add(snowItem);

            ToolStripMenuItem countdownItem = new ToolStripMenuItem("Odliczanie");
            countdownItem.CheckOnClick = true;
            countdownItem.Checked = okno.IsCountDownEnabled;
            countdownItem.CheckedChanged += (s, e) =>
            {
                okno.SetCountDownEnabled(countdownItem.Checked);
            };
            menu.Items.Add(countdownItem);

            ToolStripMenuItem musicItem = new ToolStripMenuItem("Muzyka świąteczna");
            musicItem.CheckOnClick = true;
            musicItem.Checked = okno.IsMusicEnabled;
            musicItem.CheckedChanged += (s, e) =>
            {
                okno.SetMusicEnabled(musicItem.Checked);
            };
            menu.Items.Add(musicItem);

            ToolStripMenuItem nextSongItem = new ToolStripMenuItem("Następna piosenka");
            nextSongItem.Click += (s, e) =>
            {
                okno.PlayNextSong();
            };
            menu.Items.Add(nextSongItem);

            return menu;
        }

        public bool Widoczna
        {
            get { return notifyIcon.Visible; }
            set { notifyIcon.Visible = value; }
        }

        public void Usuń()
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            notifyIcon = null;
        }
    }
}
