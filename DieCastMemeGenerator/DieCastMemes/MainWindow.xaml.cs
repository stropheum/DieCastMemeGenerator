using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace DieCastMemes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<String, List<String>> m_strategyMap;
        Random m_random;

        public MainWindow()
        {
            InitializeComponent();
            InitializeMembers();
            ParseXml();
        }

        /// <summary>
        /// Initializes local members and sets the state for the application to run
        /// </summary>
        private void InitializeMembers()
        {
            m_strategyMap = new Dictionary<String, List<String>>();
            m_random = new Random();
        }

        /// <summary>
        /// Initialize the local member states
        /// </summary>
        private void ParseXml()
        {
            XDocument doc = XDocument.Load(@"config.xml");

            // Grab all of our matchup scopes
            var matchups = 
                from matchup in doc.Descendants("Matchup")
                select new
                {
                    Header = matchup.Attribute("name").Value,
                    Children = matchup.Descendants("Strategy")
                };

            // Populate our map
            foreach (var matchup in matchups)
            {
                m_strategyMap[matchup.Header]  =
                    (from strategy in matchup.Children
                     where strategy.Name == "Strategy"
                     select strategy.Attribute("name").Value)
                     .ToList();

                ComboBoxItem matchupItem = new ComboBoxItem();
                matchupItem.Content = matchup.Header;
                MatchupBox.Items.Add(matchupItem);
            }
        }

        /// <summary>
        /// Callback for GenerateButton click events
        /// </summary>
        /// <param name="sender">The button being clicked</param>
        /// <param name="e">Event arguments being sent through the event</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            
            String key = ((ComboBoxItem)MatchupBox.SelectedValue).Content.ToString();
            int index = m_random.Next() % m_strategyMap[key].Count;

            StrategyText.Text = m_strategyMap[key][index];
        }

        /// <summary>
        /// Callback for combobox item selection events
        /// </summary>
        /// <param name="sender">The combo box being selected</param>
        /// <param name="e">Event arguments being sent through the event</param>
        private void MatchupBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBox box = (ComboBox)sender;

            if (ChooseButton != null && !ChooseButton.IsEnabled && box.SelectedIndex != 0)
            {
                ChooseButton.IsEnabled = true;
            }
        }
    }
}
