using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Pizzer
{
    /// <summary>
    /// Interaction logic for InsertPizzaWindow.xaml
    /// </summary>
    public partial class InsertPizzaWindow : Window
    {
        public string _id;
        IMongoDatabase database = new Config().getConnection();
        public event EventHandler DataInserted;
        List<string> extraZutatenList = new List<string>();

        public InsertPizzaWindow()
        {
            InitializeComponent();
            Array values = Enum.GetValues(typeof(Size));
            cbxGroesse.ItemsSource = values;
            this.ResizeMode = ResizeMode.NoResize;

            Array zutaten = Enum.GetValues(typeof(Zutaten));
            cbxZutaten.ItemsSource = zutaten;
            cbxZutaten2.ItemsSource= zutaten;
            cbxZutaten3.ItemsSource= zutaten;
        }

        public InsertPizzaWindow(Pizza pizza)
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
            tbxName.Text = pizza.Name;
            tbxEinzelpreis.Text = pizza.Einzelpreis + "";
            tbxDurchmesser.Text = pizza.Durchmesser + "";
            tbxKcal.Text = pizza.Kcal + "";
            

            _id = pizza.ObjectId;

            Array values = Enum.GetValues(typeof(Size));
            cbxGroesse.ItemsSource = values;

            Array zutaten = Enum.GetValues(typeof(Zutaten));
            cbxZutaten.ItemsSource = zutaten;
            cbxZutaten2.ItemsSource = zutaten;
            cbxZutaten3.ItemsSource = zutaten;

            CheckCheckBoxes(GetExtraZutaten());

            Size size1;
            Zutaten zutaten1, zutaten2, zutaten3;
            Enum.TryParse<Size>(pizza.Groesse, out size1);
            Enum.TryParse<Zutaten>(pizza.Zutaten[0], out zutaten1);
            Enum.TryParse<Zutaten>(pizza.Zutaten[1], out zutaten2);
            Enum.TryParse<Zutaten>(pizza.Zutaten[2], out zutaten3);
            cbxGroesse.SelectedIndex = Array.IndexOf(values, size1);
            cbxZutaten.SelectedIndex = Array.IndexOf(zutaten, zutaten1);
            cbxZutaten2.SelectedIndex = Array.IndexOf(zutaten, zutaten2);
            cbxZutaten3.SelectedIndex = Array.IndexOf(zutaten, zutaten3);

            btnInsertPizza.Content = "Pizza Speichern";
        }

        private void CheckCheckBoxes(string extras)
        {
            CheckBox[] checkBoxes =
            {
                chbx, chbx1, chbx2, chbx3, chbx4
            };

            foreach (CheckBox checkBox in checkBoxes)
            {
                if (extras.Contains(checkBox.Content + "") || extras.Contains(checkBox.Content + " ") || extras.Contains($" {checkBox.Content}"))
                {
                    checkBox.IsChecked = true;
                }
            }
        }

        public string GetExtraZutaten()
        {
            extraZutatenList.Clear();
            string zuts = "";
            var collection = database.GetCollection<BsonDocument>("Pizza");

            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(_id));
            var documents = collection.Find(filter).ToList();
            foreach(var doc in documents)
            {
                string extrazutat = doc["Zutaten"]["Extrazutat"] + "";
                zuts = extrazutat;
                extraZutatenList = extrazutat.Split(",").ToList();
            }


            return zuts;
        }

        private bool TextBoxesFilled()
        {
            TextBox[] textBoxes =
            {
                tbxDurchmesser,
                tbxEinzelpreis,
                tbxKcal,
                tbxName
            };

            foreach(TextBox textBox in textBoxes)
            {
                if (String.IsNullOrEmpty(textBox.Text))
                {
                    return false;
                }
            }

            ComboBox[] comboBoxes =
            {
                cbxGroesse,
                cbxZutaten, 
                cbxZutaten2,
                cbxZutaten3
            };

            foreach(ComboBox comboBox in comboBoxes)
            {
                if(comboBox.SelectedIndex == -1)
                {
                    return false;
                }
            }

            return true;
        }

        public string BuildExtrazutaten()
        {
            return String.Join(", ", extraZutatenList);
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            string Building = BuildExtrazutaten();


            if (btnInsertPizza.Content != "Pizza Speichern")
            {
                if (TextBoxesFilled())
                {
                    var collection = database.GetCollection<BsonDocument>("Pizza");

                    if (Building != "")
                    {
                        var document = new BsonDocument
                        {
                            {"Name", tbxName.Text },
                            {"Zutaten", new BsonDocument
                                {
                                    {"Zutat", $"{cbxZutaten.Text}, {cbxZutaten2.Text}, {cbxZutaten3.Text}"},
                                    {"Extrazutat", Building }
                                }
                            },
                            {"Einzelpreis", tbxEinzelpreis.Text},
                            {"Kcal", tbxKcal.Text},
                            {"Durchmesser", tbxDurchmesser.Text},
                            {"Groesse", cbxGroesse.SelectedItem.ToString() }
                        };

                        collection.InsertOne(document);
                    }
                    else
                    {
                        var document = new BsonDocument
                        {
                            {"Name", tbxName.Text },
                            {"Zutaten", new BsonDocument
                                {
                                    {"Zutat", $"{cbxZutaten.Text}, {cbxZutaten2.Text}, {cbxZutaten3.Text}" },
                                    {"Extrazutat", "" }
                                }
                            },
                            {"Einzelpreis", tbxEinzelpreis.Text},
                            {"Kcal", tbxKcal.Text},
                            {"Durchmesser", tbxDurchmesser.Text},
                            {"Groesse", cbxGroesse.SelectedItem.ToString() }
                        };

                        collection.InsertOne(document);

                    }
                    MessageBox.Show("Pizza Gespeichert", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalider Input", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                if (TextBoxesFilled() && Building != "")
                {
                    var collection = database.GetCollection<BsonDocument>("Pizza");
                    var fitler = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(_id));

                    var update = Builders<BsonDocument>.Update.Set("Name", tbxName.Text).Set("Zutaten", new BsonDocument { { "Zutat", $"{cbxZutaten.Text}, {cbxZutaten2.Text}, {cbxZutaten3.Text}" }, { "Extrazutat", Building } })
                        .Set("Einzelpreis", tbxEinzelpreis.Text)
                        .Set("Kcal", tbxKcal.Text)
                        .Set("Durchmesser", tbxDurchmesser.Text)
                        .Set("Groesse", cbxGroesse.Text);

                    collection.UpdateOne(fitler, update);
                    MessageBox.Show("Pizza Gespeichert", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalider Input", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            DataInserted?.Invoke(this, EventArgs.Empty);
            
        }

        private void chbx_Checked(object sender, RoutedEventArgs e)
        {
            extraZutatenList.Clear();
            CheckBox[] checkBoxes =
            {
                chbx, chbx1, chbx2, chbx3, chbx4
            };


            foreach (var checkBox in checkBoxes)
            {
                if (checkBox.IsChecked == true)
                {
                    extraZutatenList.Add(checkBox.Content + "");
                    
                }
            }


            CalculateTotalPrice(cbxGroesse);
        }

        private void chbx_Unchecked(object sender, RoutedEventArgs e)
        {
            extraZutatenList.Clear();
            CheckBox[] checkBoxes =
            {
                chbx, chbx1, chbx2, chbx3, chbx4
            };

            
            foreach (var checkBox in checkBoxes)
            {
                if (checkBox.IsChecked == true)
                {
                    extraZutatenList.Add(checkBox.Content + "");
                }
            }
            CalculateTotalPrice(cbxGroesse);
        }

        private void CalculateTotalPrice(ComboBox comboBox)
        {
            
            double price = 0;

            if (comboBox.SelectedIndex != -1)
            {
                if ((Size)comboBox.SelectedItem == Size.Small)
                {
                    price = 13;
                }
                else if ((Size)comboBox.SelectedItem == Size.Medium)
                {
                    price = 17;
                }
                else
                {
                    price = 26;
                }
            }
            
            CheckBox[] checkBoxes =
            {
                chbx, chbx1, chbx2, chbx3, chbx4
            };

            foreach (CheckBox checkBox in checkBoxes)
            {
                if(checkBox.IsChecked == true)
                {
                    if(checkBox.Content.ToString() == "Cola")
                    {
                        price += 5;
                    }
                    else if(checkBox.Content.ToString() == "Mais")
                    {
                        price += 1.5;
                    } 
                    else if(checkBox.Content.ToString() == "XTomate")
                    {
                        price += 2;
                    } 
                    else if(checkBox.Content.ToString() == "Orange")
                    {
                        price += 6;
                    }
                    else
                    {
                        price += 2.25;
                    }
                }
            }

            tbxEinzelpreis.Text = "" + price;
        }

        private void cbxGroesse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((Size)cbxGroesse.SelectedItem == Size.Small)
            {
                tbxDurchmesser.Text = "24";
            } else if((Size)cbxGroesse.SelectedItem == Size.Medium)
            {
                tbxDurchmesser.Text = "32";
            }
            else
            {
                tbxDurchmesser.Text = "40";
            }
            CalculateTotalPrice(cbxGroesse);
        }

        private void tbxKcal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsAllowed(e.Text);
        }

        private bool IsAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+");
            return !regex.IsMatch(text);
        }
    }
}
