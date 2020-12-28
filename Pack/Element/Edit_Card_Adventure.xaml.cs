using Make.MODEL;
using MaterialDesignThemes.Wpf;
using Pack.MODEL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pack.Element
{

    /// <summary>
    /// Edit_Card.xaml 的交互逻辑
    /// </summary>
    public partial class Edit_Card_Adventure : UserControl
    {
        Custom_Card_Adventure Origin_Custom_Card;
        public Edit_Card_Adventure()
        {
            InitializeComponent();
            Custom_Card_Adventure.State.IsEnabled = true;
        }


        public void Open_Edit(Custom_Card_Adventure adventureCard)
        {
            Origin_Custom_Card = adventureCard;
            Custom_Card_Adventure.Adventure = adventureCard.Adventure;
            Custom_Card_Adventure.DataContext = adventureCard.Adventure;
            Visibility = Visibility.Visible;
        }
        public void Open_Edit(Adventure adventure)
        {
            Custom_Card_Adventure.Adventure = adventure;
            Custom_Card_Adventure.DataContext = adventure;
            Visibility = Visibility.Visible;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Custom_Card_Adventure.DataContext != null)
            {
                State state = (State)((ComboBox)sender).DataContext;
                state.Name = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Content.ToString();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            States_Select.Visibility = Visibility.Visible;
        }
        public void Button_Clear()
        {
            Close.Click -= Button_Click_2;
            Delete.Click -= Button_Click_3;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Origin_Custom_Card.Adventure.Save();
            GeneralControl.Adventure_Date = DateTime.Now;
            this.Visibility = Visibility.Hidden;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Custom_Card_Adventure.Adventure.Effect_States.Remove((State)(((Button)sender).DataContext));
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            GeneralControl.Adventure_Date = DateTime.Now;
            Pack_General.MainWindow.AdventurePanle.AdventurePanel.Children.Remove(Origin_Custom_Card);
            GeneralControl.Adventures.Remove(Origin_Custom_Card.Adventure);
            GeneralControl.Adventures_ID.Remove(Origin_Custom_Card.Adventure.ID);
            Origin_Custom_Card.Adventure.Delete();
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (Custom_Card_Adventure.Adventure.Effect_States.Count >= Make.MODEL.GeneralControl.MaxStates)
            {
                MessageBox.Show("状态数量已满");
                States_Select.Visibility = Visibility.Hidden;
                return;
            }
            State state = new State
            {
                Name = (sender as Button).Content.ToString(),
                Duration_Immediate = 10,
                Duration_Round = 1
            };
            Custom_Card_Adventure.Adventure.Effect_States.Add(state);
            States_Select.Visibility = Visibility.Hidden;
        }
    }
}
