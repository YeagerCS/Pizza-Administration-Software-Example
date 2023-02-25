using MongoDB.Driver;
using MongoDB.Bson;
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
using System.Windows.Shapes;
using ZstdSharp.Unsafe;
using System.Collections;
using System.Windows.Media.Animation;
using System.Data;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Pizzer
{
    /// <summary>
    /// Interaction logic for InsertWindow.xaml
    /// </summary>
    public partial class InsertWindow : Window
    {
        IMongoDatabase database = new Config().getConnection();
        private ObjectId _id;
        //Detects whether a method in this window was executed to update listboxes in MainWindow
        public event EventHandler DataInserted;
        public InsertWindow()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;

        }

        public InsertWindow(Kunde kunde)
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;

            btnInsertKunde.Content = "Kunde Speichern";
            _id = kunde._id;
            tbxNachname.Text = kunde.Nachname;
            tbxVorname.Text = kunde.Vorname;
            tbxStrasse.Text = kunde.Strasse;
            tbxPlz.Text = kunde.Plz;
            tbxOrt.Text = kunde.Ort;
            tbxTelefon.Text = kunde.Telefon;
            tbxEmail.Text = kunde.Email;
            tbxGebdatum.Text = kunde.Geburtsdatum.ToString("dd.MM.yyyy");
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            if (btnInsertKunde.Content.ToString() != "Kunde Speichern")
            {
                if (TextBoxesFilled())
                {
                    var collection = database.GetCollection<BsonDocument>("Kunde");

                    DateTime dateTime = tbxGebdatum.SelectedDate.Value;

                    var document = new BsonDocument
                    {
                    {"Nachname", tbxNachname.Text },
                    {"Vorname", tbxVorname.Text },
                    {"Adresse", new BsonDocument
                        {
                            {"Strasse", tbxStrasse.Text },
                            { "Plz", tbxPlz.Text },
                            { "Ort", tbxOrt.Text }
                        }
                    },
                    {"Telefon", tbxTelefon.Text },
                    {"Email", tbxEmail.Text },
                    {"Geburtsdatum", dateTime }
                    };

                    collection.InsertOne(document);
                    MessageBox.Show("Kunde Gespeichert", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalider Input", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                if (TextBoxesFilled())
                {
                    var collection = database.GetCollection<BsonDocument>("Kunde");
                    DateTime dateTime = tbxGebdatum.SelectedDate.Value;

                    var filter = Builders<BsonDocument>.Filter.Eq("_id", _id);
                    var update = Builders<BsonDocument>.Update.Set("Nachname", tbxNachname.Text).Set("Vorname", tbxVorname.Text)
                        .Set("Adresse", new BsonDocument
                        {
                            {"Strasse", tbxStrasse.Text },
                            { "Plz", tbxPlz.Text },
                            { "Ort", tbxOrt.Text }
                        }).Set("Telefon", tbxTelefon.Text).Set("Email", tbxEmail).Set("Geburtsdatum", dateTime);


                    collection.UpdateOne(filter, update);
                    MessageBox.Show("Kunde Gespeichert", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalider Input", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            DataInserted?.Invoke(this, EventArgs.Empty);
        }

        private bool EmailMatchesRegex(string text)
        {
            Regex regex = new Regex(@"^\w+(\w|\.|\-|_)+\@+\w+(\w|\-)+\.\w+$");
            return regex.IsMatch(text) ? true : false;
        }

        private bool TextBoxesFilled()
        {
            //Generally validates TextBoxes
            
            TextBox[] textBoxes =
            {
                tbxNachname,
                tbxVorname,
                tbxStrasse,
                tbxPlz,
                tbxOrt,
                tbxTelefon,
                tbxEmail
            };

            foreach (TextBox textBox in textBoxes)
            {
                if (String.IsNullOrEmpty(textBox.Text))
                {
                    return false;
                }
            }

            try
            {
                Regex dateRegex = new Regex(@"^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[012])/\d{4}$");
                Regex dateRegex2 = new Regex(@"^(0[1-9]|[12][0-9]|3[01])\.(0[1-9]|1[012])\.\d{4}$");
                if (tbxGebdatum.SelectedDate.Value == null || !dateRegex.IsMatch(tbxGebdatum.Text) && !dateRegex2.IsMatch(tbxGebdatum.Text))
                {
                    return false;
                }
                if (tbxGebdatum.SelectedDate.Value.CompareTo(DateTime.Today) > 0)
                {
                    return false;
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            
            
            if (!EmailMatchesRegex(tbxEmail.Text))
            {
                return false;
            }

            return true;
        }

        private void tbxTelefon_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
