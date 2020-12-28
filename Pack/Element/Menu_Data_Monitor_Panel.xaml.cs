using Make.BLL;
using Make.MODEL;
using Pack.MODEL;
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

namespace Pack.Element
{
    /// <summary>
    /// Menu_Data_Monitor.xaml 的交互逻辑
    /// </summary>
    public partial class Menu_Data_Monitor_Panel : UserControl
    {
        Adventure origin_Adventure, new_Adventure;
        SkillCardsModel origin_SkillcardsModel, new_SkillCardsModel;
        public Menu_Data_Monitor_Panel()
        {
            InitializeComponent();
            Pubmit_Skill.ItemsSource = GeneralControl.Menu_Data_Monitor_Class.Instance.Pubmit_SkillCardsModel;
            Pubmit_Adventures.ItemsSource = GeneralControl.Menu_Data_Monitor_Class.Instance.Pubmit_Adventures;
            EditCard_Adventure.Button_Clear();
            EditCard_SkillCard.Button_Clear();
            EditCard_Adventure.Delete.Click += Adventure_Switch_Click;
            EditCard_SkillCard.Delete.Click += SkillCardsModel_Switch_Click;
            EditCard_Adventure.Close.Click += Close_Click;
            EditCard_SkillCard.Close.Click += Close_Click1;
        }

        private void Close_Click1(object sender, RoutedEventArgs e)
        {
            EditCard_SkillCard.Visibility = Visibility.Hidden;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {   
            EditCard_Adventure.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Custom_Card_Adventure custom_Card_Adventure = new Custom_Card_Adventure((sender as Button).DataContext as Adventure);
            new_Adventure = custom_Card_Adventure.Adventure;
            if (GeneralControl.Adventures_ID.TryGetValue(custom_Card_Adventure.Adventure.ID, out Adventure adventure))
            {
                origin_Adventure = adventure;
                EditCard_Adventure.Delete.Content = "原始";
                EditCard_Adventure.Delete.Visibility = Visibility.Visible;
            }
            else
            {
                EditCard_Adventure.Delete.Visibility =  Visibility.Hidden;
            }
            EditCard_Adventure.Open_Edit(new_Adventure);
        }


        private void Adventure_Switch_Click(object sender, RoutedEventArgs e)
        {

            if (origin_Adventure == EditCard_Adventure.Custom_Card_Adventure.Adventure)
            {
                EditCard_Adventure.Open_Edit(new_Adventure);
                EditCard_Adventure.Delete.Content = "原始";
            }
            else
            {
                EditCard_Adventure.Open_Edit(origin_Adventure);
                EditCard_Adventure.Delete.Content = "全新";
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Adventure adventure = (sender as Button).DataContext as Adventure;
            Custom_Card_Adventure custom_Card_Adventure = (from Custom_Card_Adventure item in Pack_General.MainWindow.AdventurePanle.AdventurePanel.Children where item.Adventure.ID == adventure.ID select item).FirstOrDefault();
            Make.MODEL.User author = Make.MODEL.User.Load(adventure.UserName);
            if (custom_Card_Adventure != null)
            {
                custom_Card_Adventure.Adventure.Delete();
                custom_Card_Adventure.Adventure = adventure;
                custom_Card_Adventure.DataContext = adventure;
                author.Upgrade_num++;
                author.Money += 50;
                author.Save();
            }
            else
            {
                Pack_General.MainWindow.AdventurePanle.Add_Adventure(adventure);
                author.Create_num++;
                author.Money += 100;
                author.Save();
            }
            adventure.Add_To_General();
            adventure.Save();
            GeneralControl.Menu_Data_Monitor_Class.Instance.Pubmit_Adventures.Remove(adventure);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Adventure adventure = (sender as Button).DataContext as Adventure;
            GeneralControl.Menu_Data_Monitor_Class.Instance.Pubmit_Adventures.Remove(adventure);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Custom_Card_SkillCard custom_Card_SkillCard = new Custom_Card_SkillCard((sender as Button).DataContext as SkillCardsModel);
            new_SkillCardsModel = custom_Card_SkillCard.SkillCardsModel;
            if(GeneralControl.Skill_Cards_ID.TryGetValue(custom_Card_SkillCard.SkillCardsModel.ID,out SkillCardsModel skillCardsModel))
            {
                origin_SkillcardsModel = skillCardsModel;
                EditCard_SkillCard.Delete.Content = "原始";
                EditCard_SkillCard.Delete.Visibility = Visibility.Visible;
            }
            else EditCard_SkillCard.Close.Visibility = Visibility.Hidden;
            EditCard_SkillCard.Open_Edit(new_SkillCardsModel);
        }
        private void SkillCardsModel_Switch_Click(object sender, RoutedEventArgs e)
        {
            if (origin_SkillcardsModel == EditCard_SkillCard.Custom_Card.SkillCardsModel)
            {
                EditCard_SkillCard.Open_Edit(new_SkillCardsModel);
                EditCard_SkillCard.Delete.Content = "原始";
            }
            else
            {
                EditCard_SkillCard.Open_Edit(origin_SkillcardsModel);
                EditCard_SkillCard.Delete.Content = "全新";
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            SkillCardsModel skillCards = (sender as Button).DataContext as SkillCardsModel;
            Custom_Card_SkillCard custom_Card_SkillCard = (from Custom_Card_SkillCard item in Pack_General.MainWindow.CardPanle.CardsPanel.Children where item.SkillCardsModel.ID == skillCards.ID select item).FirstOrDefault();
            Make.MODEL.User author = Make.MODEL.User.Load(skillCards.UserName);
            if (custom_Card_SkillCard!=null)
            {
                custom_Card_SkillCard.SkillCardsModel.Delete();
                custom_Card_SkillCard.SkillCardsModel = skillCards;
                custom_Card_SkillCard.DataContext = skillCards.SkillCards[custom_Card_SkillCard.Rate.Value - 1];
                author.Upgrade_num++;
                author.Save();
            }
            else
            {
                Pack_General.MainWindow.CardPanle.Add_Card(skillCards);
                author.Create_num++;
                author.Save();
            }
            skillCards.Add_To_General();
            skillCards.Save();
            GeneralControl.Menu_Data_Monitor_Class.Instance.Pubmit_SkillCardsModel.Remove(skillCards);
            Pubmit_Skill.ItemsSource = null;
            Pubmit_Skill.ItemsSource = GeneralControl.Menu_Data_Monitor_Class.Instance.Pubmit_SkillCardsModel;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            SkillCardsModel skillCards = (sender as Button).DataContext as SkillCardsModel;
            GeneralControl.Menu_Data_Monitor_Class.Instance.Pubmit_SkillCardsModel.Remove(skillCards);
            Pubmit_Skill.ItemsSource = null;
            Pubmit_Skill.ItemsSource = GeneralControl.Menu_Data_Monitor_Class.Instance.Pubmit_SkillCardsModel;
        }
    }
}
