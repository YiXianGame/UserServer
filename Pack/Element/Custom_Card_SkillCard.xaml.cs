using Make.MODEL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Pack;
using System.Globalization;
using System.IO;
using Make.BLL;

namespace Pack.Element
{   
    /// <summary>
    /// Custom_Card.xaml 的交互逻辑
    /// </summary>
    public partial class Custom_Card_SkillCard : UserControl
    {
        public SkillCardsModel SkillCardsModel;
        public Custom_Card_SkillCard(SkillCardsModel skillCards)
        {
            InitializeComponent();         
            DataContext = skillCards.SkillCards[0];
            SkillCardsModel = skillCards;
            InitializeComponent();
        }
        public Custom_Card_SkillCard()
        {
            InitializeComponent();
        }
        private void Rate_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            if (SkillCardsModel != null)
            {              
                DataContext = SkillCardsModel.SkillCards[e.NewValue-1];
            }
        }
    }
}
