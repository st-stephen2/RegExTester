using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RegExTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Match currentMatch;

        RegexOptions options
        {
            get
            {
                RegexOptions opts = new RegexOptions();
                if ((bool)chckIgnoreC.IsChecked)
                    opts |= RegexOptions.IgnoreCase;
                if ((bool)chckMultiline.IsChecked)
                    opts |= RegexOptions.Multiline;
                if ((bool)chckExplicit.IsChecked)
                    opts |= RegexOptions.ExplicitCapture;
                if ((bool)chckCompiled.IsChecked)
                    opts |= RegexOptions.Compiled;
                if ((bool)chckSingleline.IsChecked)
                    opts |= RegexOptions.Singleline;
                if ((bool)chckIgnorePW.IsChecked)
                    opts |= RegexOptions.IgnorePatternWhitespace;
                if ((bool)chckRightToLeft.IsChecked)
                    opts |= RegexOptions.RightToLeft;
                if ((bool)chckECMAScript.IsChecked)
                    opts |= RegexOptions.ECMAScript;
                if ((bool)chckCultureInvariant.IsChecked)
                    opts |= RegexOptions.CultureInvariant;
                return opts;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowError(string message)
        {
            if (!string.Equals(lblNotify.Content, message) || lblNotify.Visibility != Visibility.Visible)
            {
                ThicknessAnimation animation =
                        new ThicknessAnimation(
                            new Thickness(30, -28, 30, 0),
                            new Thickness(30, 5, 30, 0),
                            new Duration(new TimeSpan(0, 0, 0, 0, 500)));
                Storyboard sb = new Storyboard();
                sb.Children.Add(animation);
                Storyboard.SetTargetName(animation, lblNotify.Name);
                Storyboard.SetTargetProperty(animation, new PropertyPath(Label.MarginProperty));
                lblNotify.Content = message;
                lblNotify.Visibility = Visibility.Visible;
                sb.Begin(this);
            }
        }

        private void Refresh()
        {
            try
            {
                DropShadowEffect redGlow = new DropShadowEffect();
                DropShadowEffect greenGlow = new DropShadowEffect();
                redGlow.BlurRadius = 8;
                greenGlow.BlurRadius = 8;
                redGlow.ShadowDepth = 0;
                greenGlow.ShadowDepth = 0;
                redGlow.Color = Colors.Red;
                greenGlow.Color = Colors.LightGreen;

                lbGroups.Items.Clear();
                lblResult.Content = "";
                tbExpression.Background = new SolidColorBrush(Colors.White);

                if (string.IsNullOrEmpty(tbText1.Text))
                {
                    lblResult1.Content = "";
                    tbText1.Effect = null;

                }
                else if (Regex.IsMatch(tbText1.Text, tbExpression.Text, options))
                {
                    lblResult1.Content = "Successful Match";
                    lblResult1.Foreground = new SolidColorBrush(Colors.Green);
                    tbText1.Effect = greenGlow;

                    currentMatch = Regex.Match(tbText1.Text, tbExpression.Text, options);

                    int index = 0;
                    foreach (Group g in currentMatch.Groups)
                    {
                        lbGroups.Items.Add(string.Format("{0}: '{1}'", index, g.Value));
                        index++;
                    }
                }
                else
                {
                    lblResult1.Content = "Match Failed";
                    lblResult1.Foreground = new SolidColorBrush(Colors.Red);
                    tbText1.Effect = redGlow;
                }

                lblNotify.Visibility = Visibility.Hidden;
            }
            catch (ArgumentException ex)
            {
                if (ex.Message.StartsWith("parsing"))
                {
                    ShowError(Regex.Match(ex.Message, @"^parsing ""[^""\r\n]*"" - (?<error>.+)$").Groups["error"].ToString());
                    tbExpression.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#75C60000"));
                }
                else
                {
                    ShowError(ex.Message.Replace("\n",""));
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                lblResult.Content = currentMatch.Groups[tbSearch.Text];
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void tbText1_TextChanged(object sender, TextChangedEventArgs e)
        {
            Refresh();
        }

        private void tbExpression_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbText1 != null)
            {
                Refresh();
            }
        }

        private void chckOptions_Checked(object sender, RoutedEventArgs e)
        {
            if (tbText1 != null)
            {
                Refresh();
            }
            if (chckECMAScript.IsChecked == true)
            {
                chckCultureInvariant.IsChecked = false;
                chckCultureInvariant.IsEnabled = false;
                chckExplicit.IsChecked = false;
                chckExplicit.IsEnabled = false;
                chckIgnorePW.IsChecked = false;
                chckIgnorePW.IsEnabled = false;
                chckRightToLeft.IsChecked = false;
                chckRightToLeft.IsEnabled = false;
                chckSingleline.IsChecked = false;
                chckSingleline.IsEnabled = false;
            }
            else
            {
                chckCultureInvariant.IsEnabled = true;
                chckExplicit.IsEnabled = true;
                chckIgnorePW.IsEnabled = true;
                chckRightToLeft.IsEnabled = true;
                chckSingleline.IsEnabled = true;
            }
        }

        private void lblCopy_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(tbExpression.Text);
        }

        private void lblCopy_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(tbExpression, tbExpression.Text, DragDropEffects.Copy);
            }
        }

        private void lblCopy_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {

        }
    }
}
