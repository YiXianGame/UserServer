using Make.MODEL;
using MaterialDesignThemes.Wpf;
using Pack.MODEL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
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
    public partial class Edit_Card_SkillCard : UserControl
    {
        Custom_Card_SkillCard Origin_Custom_Card;
        public Edit_Card_SkillCard()
        {
            InitializeComponent();
            Custom_Card.State.IsEnabled = true;
        }

        public void Open_Edit(Custom_Card_SkillCard custom_Card)
        {
            Origin_Custom_Card = custom_Card;
            Custom_Card.SkillCardsModel = custom_Card.SkillCardsModel;
            Custom_Card.DataContext = custom_Card.SkillCardsModel.SkillCards[0];
            Custom_Card.Rate.Value = 1;
            Visibility = Visibility.Visible;
        }
        public void Open_Edit(SkillCardsModel skillCardsModel)
        {
            Custom_Card.SkillCardsModel = skillCardsModel;
            Custom_Card.DataContext = skillCardsModel.SkillCards[0];
            Custom_Card.Rate.Value = 1;
            Visibility = Visibility.Visible;
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
            this.Visibility = Visibility.Hidden;
            DateTime dateTime = DateTime.Now;
            foreach (SkillCard item in Origin_Custom_Card.SkillCardsModel.SkillCards)
            {
                item.Date_Latest = dateTime;
            }
            GeneralControl.Skill_Card_Date = dateTime;
            Origin_Custom_Card.SkillCardsModel.Save();
            GeneralControl.Menu_Person_Information_Class.Instance.Author.Upgrade_num++;
            GeneralControl.Menu_Person_Information_Class.Instance.Author.Save();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            this.Visibility = Visibility.Hidden;
            Origin_Custom_Card.SkillCardsModel.Delete();
            GeneralControl.Skill_Cards.Remove(Origin_Custom_Card.SkillCardsModel);
            GeneralControl.Skill_Cards_ID.Remove(Origin_Custom_Card.SkillCardsModel.ID);
            foreach(SkillCard item in Origin_Custom_Card.SkillCardsModel.SkillCards)
            {
                GeneralControl.Skill_Card_ID_Skllcard.Remove(item.ID);
                GeneralControl.Skill_Card_Name_Skllcard.Remove(item.Name);
            }
            GeneralControl.Skill_Card_Date = dateTime;
            Pack_General.MainWindow.CardPanle.CardsPanel.Children.Remove(Origin_Custom_Card);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Custom_Card.SkillCardsModel.SkillCards[Custom_Card.Rate.Value - 1].Effect_States.Remove((State)(sender as Button).DataContext);
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (Custom_Card.SkillCardsModel.SkillCards[Custom_Card.Rate.Value - 1].Effect_States.Count >= Make.MODEL.GeneralControl.MaxStates)
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
            Custom_Card.SkillCardsModel.SkillCards[Custom_Card.Rate.Value - 1].Effect_States.Add(state);      
            States_Select.Visibility = Visibility.Hidden;
        }
    }
}
