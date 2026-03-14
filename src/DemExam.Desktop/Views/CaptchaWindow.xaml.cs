using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DemExam.Desktop.Views;

public partial class CaptchaWindow : Window
{
    private readonly List<Image> _pieces = [];
    private readonly Random _random = new();
    private Image? _dragged;

    private bool _isDragging;
    private Point _offset;

    public CaptchaWindow()
    {
        InitializeComponent();

        Loaded += (s, e) => InitializePuzzle();
    }

    public bool IsCaptchaPassed { get; private set; }

    private void InitializePuzzle()
    {
        PuzzleCanvas.Children.Clear();
        _pieces.Clear();

        var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Captcha");

        for (var i = 1; i <= 4; i++)
        {
            var img = new Image
            {
                Width = 80,
                Height = 80,
                Tag = i,
                Source = new BitmapImage(new Uri(Path.Combine(folder, $"{i}.png")))
            };

            Canvas.SetLeft(img, _random.Next(0, 120));
            Canvas.SetTop(img, _random.Next(0, 120));

            img.MouseDown += Image_MouseDown;
            img.MouseMove += Image_MouseMove;
            img.MouseUp += Image_MouseUp;

            PuzzleCanvas.Children.Add(img);
            _pieces.Add(img);
        }
    }

    private void Image_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _dragged = sender as Image;

        if (_dragged != null)
        {
            _isDragging = true;
            _offset = e.GetPosition(_dragged);
            _dragged.CaptureMouse();
        }
    }

    private void Image_MouseMove(object sender, MouseEventArgs e)
    {
        if (_isDragging && _dragged != null && e.LeftButton == MouseButtonState.Pressed)
        {
            var pos = e.GetPosition(PuzzleCanvas);
            Canvas.SetLeft(_dragged, pos.X - _offset.X);
            Canvas.SetTop(_dragged, pos.Y - _offset.Y);
        }
    }

    private void Image_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            _dragged?.ReleaseMouseCapture();
            _dragged = null;
        }
    }

    private void ButtonCheck_OnClick(object sender, RoutedEventArgs e)
    {
        Point[] targets = [new(0, 0), new(80, 0), new(0, 80), new(80, 80)];
        var ok = true;

        for (var i = 0; i < _pieces.Count; i++)
        {
            var left = Canvas.GetLeft(_pieces[i]);
            var top = Canvas.GetTop(_pieces[i]);

            if (Math.Abs(left - targets[i].X) > 7 || Math.Abs(top - targets[i].Y) > 7)
            {
                ok = false;
                break;
            }
        }

        if (ok)
        {
            IsCaptchaPassed = true;
            ResultText.Text = "Успешно!";

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                DialogResult = true;
            };
            timer.Start();
        }
        else
        {
            ResultText.Text = "Неверно!";
        }
    }

    private void ButtonRefresh_OnClick(object sender, RoutedEventArgs e)
    {
        InitializePuzzle();
    }
}