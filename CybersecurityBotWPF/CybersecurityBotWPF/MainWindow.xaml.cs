using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Media;

namespace CybersecurityBotWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly ChatBot _bot = new ChatBot();

        public MainWindow()
        {
            InitializeComponent();
            // Greeting user on startup
            AddBotBubble(" Welcome to the Cybersecurity Awareness Bot!\n\nBefore we begin, what is your name?");

            // Playing voice greeting
            SoundPlayer player = new SoundPlayer("greeting.wav");
            player.Play();
        }

        //Send button click
        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        // Enter key to send
        private void TxtUserInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                SendMessage();
        }

        //Clear chat button
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ChatPanel.Children.Clear();
            _bot.Reset();
            AddBotBubble("Chat cleared! What is your name?");
        }

        // Core send logic 
        private void SendMessage()
        {
            string input = userInput.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            // Show user bubble
            AddUserBubble(input);
            userInput.Clear();

            // Get bot response
            string response = _bot.GetResponse(input);
            AddBotBubble(response);
        }

        // This is WhatsApp style USER bubble
        private void AddUserBubble(string text)
        {
            string time = DateTime.Now.ToString("HH:mm");

            var outerStack = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(80, 4, 4, 4)
            };

            var bubble = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0, 92, 26)),
                CornerRadius = new CornerRadius(14, 14, 0, 14),
                Padding = new Thickness(12, 8, 12, 6),
                MaxWidth = 500
            };

            var inner = new StackPanel();

            inner.Children.Add(new TextBlock
            {
                Text = text,
                Foreground = Brushes.White,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap
            });

            // Timestamp + tick
            var meta = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 3, 0, 0)
            };

            meta.Children.Add(new TextBlock
            {
                Text = time,
                Foreground = new SolidColorBrush(Color.FromArgb(160, 255, 255, 255)),
                FontSize = 10
            });

            meta.Children.Add(new TextBlock
            {
                Text = "",
                Foreground = new SolidColorBrush(Color.FromRgb(0, 200, 0)),
                FontSize = 10
            });

            inner.Children.Add(meta);
            bubble.Child = inner;
            outerStack.Children.Add(bubble);

            ChatPanel.Children.Add(outerStack);
            chatScrollViewer.ScrollToBottom();
        }

        // WhatsApp-style BOT bubble
        private void AddBotBubble(string text)
        {
            string time = DateTime.Now.ToString("HH:mm");

            var outerStack = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(4, 4, 80, 4)
            };

            // Bot label
            outerStack.Children.Add(new TextBlock
            {
                Text = " CyberBot",
                Foreground = new SolidColorBrush(Color.FromRgb(0, 180, 0)),
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(4, 0, 0, 3)
            });

            var bubble = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                CornerRadius = new CornerRadius(14, 14, 14, 0),
                Padding = new Thickness(12, 8, 12, 6),
                MaxWidth = 560
            };

            var inner = new StackPanel();

            inner.Children.Add(new TextBlock
            {
                Text = text,
                Foreground = Brushes.Black,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 20
            });

            inner.Children.Add(new TextBlock
            {
                Text = time,
                Foreground = new SolidColorBrush(Color.FromArgb(150, 0, 0, 0)),
                FontSize = 10,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 3, 0, 0)
            });

            bubble.Child = inner;
            outerStack.Children.Add(bubble);

            ChatPanel.Children.Add(outerStack);
            chatScrollViewer.ScrollToBottom();
        }
    }
    }
