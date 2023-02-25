using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
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

namespace Pizzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Kunde> Kunden = new List<Kunde>();
        List<Pizza> Pizzas = new List<Pizza>();
        List<Bestellung> Bestellungen = new List<Bestellung>();
        List<string> ExtraZutaten = new List<string>();
        InsertWindow insertWindow = new InsertWindow();
        InsertPizzaWindow insertPizzaWindow = new InsertPizzaWindow();
        BestellungWindow bestellungWindow = new BestellungWindow();
        IMongoDatabase database = new Config().getConnection();



        public MainWindow()
        {
            InitializeComponent();
            ReadDatabase();
            this.ResizeMode = ResizeMode.NoResize;

        }

        public void ReadDatabase()
        {
            Kunden.Clear();
            Pizzas.Clear();
            Bestellungen.Clear();

            //Read all Data

            var collection = database.GetCollection<BsonDocument>("Kunde");

            var filter = new BsonDocument();
            var documents = collection.Find(filter).ToList();
            foreach (var doc in documents)
            {
                Kunden.Add(new Kunde(doc["_id"].AsObjectId, doc["Nachname"] + "", doc["Vorname"] + "", BsonSerializer.Deserialize<Adresse>(doc["Adresse"].AsBsonDocument), doc["Telefon"] + "", doc["Email"] + "", doc["Geburtsdatum"].ToUniversalTime().ToLocalTime()));
            }

            var collectionPizza = database.GetCollection<BsonDocument>("Pizza");

            var documentsPizza = collectionPizza.Find(filter).ToList();
            foreach (var doc1 in documentsPizza)
            {
                string zutat = doc1["Zutaten"]["Zutat"] + "";
                List<string> zutaten = zutat.Split(',').ToList();
                string extrazutat = doc1["Zutaten"]["Extrazutat"] + "";
                ExtraZutaten = extrazutat.Split(",").ToList();

                Pizzas.Add(new Pizza(doc1["_id"] + "", doc1["Name"] + "", zutaten, Convert.ToDouble(doc1["Einzelpreis"]), Convert.ToDouble(doc1["Kcal"]), Convert.ToDouble(doc1["Durchmesser"]), doc1["Groesse"] + ""));
            }

            var collectionBestellung = database.GetCollection<BsonDocument>("Bestellung");

            var documentsBestellung = collectionBestellung.Find(filter).ToList();
            foreach (var doc1 in documentsBestellung)
            {
                List<Pizza> pizzaList =  doc1["Bestellposition"]["Pizza"].AsBsonArray.Select(x => BsonSerializer.Deserialize<Pizza>(x.AsBsonDocument)).ToList();
                Bestellungen.Add(new Bestellung(doc1["_id"] + "", Convert.ToInt32(doc1["Bestellnummer"]), doc1["Bestelldatum"].ToUniversalTime().ToLocalTime(), BsonSerializer.Deserialize<Kunde>(doc1["Kunde"].AsBsonDocument), pizzaList, Convert.ToDouble(doc1["Total"])));
            }

            lbxBestellungen.Items.Clear();
            lbxKunden.Items.Clear();
            lbxPizza.Items.Clear();
            AddRange(lbxKunden, Kunden); //Add List content to listbox
            AddRangePizza(lbxPizza, Pizzas);
            AddRangeBestellung(lbxBestellungen, Bestellungen);
        }


        private void AddRangeBestellung(ListBox listbox, List<Bestellung> list)
        {
            foreach(Bestellung bestellung in list)
            {
                listbox.Items.Add(bestellung);
            }
        }

        private void AddRange(ListBox listbox, List<Kunde> list)
        {
            foreach (Kunde str in list)
            {
                listbox.Items.Add(str);
            }
        }

        private void AddRangePizza(ListBox listbox, List<Pizza> list)
        {
            foreach (Pizza str in list)
            {
                listbox.Items.Add(str);
            }
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            insertWindow.DataInserted -= InsertWindow_DataInserted;

            insertWindow = new InsertWindow();
            insertWindow.DataInserted += InsertWindow_DataInserted;

            insertWindow.Show();
            
        }

        private void btnInsertPizza_Click(object sender, RoutedEventArgs e)
        {
            insertPizzaWindow.DataInserted -= InsertPizzaWindow_DataInserted;

            insertPizzaWindow = new InsertPizzaWindow();
            insertPizzaWindow.DataInserted += InsertPizzaWindow_DataInserted;

            insertPizzaWindow.Show();
        }

        private void updateKunde_Click(object sender, RoutedEventArgs e)
        {
            if (lbxKunden.SelectedIndex != -1)
            {
                insertWindow.DataInserted -= InsertWindow_DataInserted;

                Kunde kunde = Kunden[lbxKunden.SelectedIndex];
                insertWindow = new InsertWindow(kunde);
                insertWindow.DataInserted+= InsertWindow_DataInserted;

                insertWindow.Show();
            }
        }

        private void updatePizza_Click(object sender, RoutedEventArgs e)
        {
            if(lbxPizza.SelectedIndex != -1)
            {
                insertPizzaWindow.DataInserted -= InsertPizzaWindow_DataInserted;

                Pizza pizza = Pizzas[lbxPizza.SelectedIndex];
                insertPizzaWindow = new InsertPizzaWindow(pizza);
                insertPizzaWindow.DataInserted+= InsertPizzaWindow_DataInserted;

                insertPizzaWindow.Show();
            }
        }

        //When this Window is loaded, every other Window gets Added methods which detect a event EventHandler to update the ListBoxes
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            insertPizzaWindow.DataInserted += InsertPizzaWindow_DataInserted;
            insertWindow.DataInserted += InsertWindow_DataInserted;
            bestellungWindow.DataInserted += BestellungWindow_DataInserted;
        }

        private void BestellungWindow_DataInserted(object? sender, EventArgs e)
        {
            ReadDatabase();
        }

        private void InsertWindow_DataInserted(object? sender, EventArgs e)
        {
            ReadDatabase();
        }

        private void InsertPizzaWindow_DataInserted(object? sender, EventArgs e)
        {
            ReadDatabase();
        }

        private void bestellungBtn_Click(object sender, RoutedEventArgs e)
        {
            bestellungWindow.DataInserted -= BestellungWindow_DataInserted;

            bestellungWindow = new BestellungWindow();
            bestellungWindow.DataInserted += BestellungWindow_DataInserted;

            bestellungWindow.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void deleteKunde_Click(object sender, RoutedEventArgs e)
        {
            if(lbxKunden.SelectedIndex != -1)
            {
                MessageBoxResult result = MessageBox.Show($"Löschung von {Kunden[lbxKunden.SelectedIndex].Vorname} {Kunden[lbxKunden.SelectedIndex].Nachname} bestätigen", "Bestätigen", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if(result == MessageBoxResult.Yes)
                {
                    var collection = database.GetCollection<BsonDocument>("Kunde");
                    var fitler = Builders<BsonDocument>.Filter.Eq("_id", Kunden[lbxKunden.SelectedIndex]._id);

                    collection.DeleteOne(fitler);
                    ReadDatabase();
                }
            }
        }

        private void deletePizza_Click(object sender, RoutedEventArgs e)
        {
            if(lbxPizza.SelectedIndex != -1)
            {
                MessageBoxResult result = MessageBox.Show($"Löschung von {Pizzas[lbxPizza.SelectedIndex].Name} bestätigen", "Bestätigen", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    var collection = database.GetCollection<BsonDocument>("Pizza");
                    var fitler = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(Pizzas[lbxPizza.SelectedIndex].ObjectId));

                    collection.DeleteOne(fitler);
                    ReadDatabase();
                }

            }
        }

        private void bestellungLoeschen_Click(object sender, RoutedEventArgs e)
        {
            if(lbxBestellungen.SelectedIndex != -1)
            {
                MessageBoxResult result = MessageBox.Show($"Löschung von Bestellung mit Nr: {Bestellungen[lbxBestellungen.SelectedIndex].BestellNummer} bestätigen", "Bestätigen", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if(result == MessageBoxResult.Yes)
                {
                    var collection = database.GetCollection<BsonDocument>("Bestellung");
                    var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(Bestellungen[lbxBestellungen.SelectedIndex].objectId));

                    collection.DeleteOne(filter);
                    ReadDatabase();
                }
            }
        }


        private void bestellungBearbeiten_Click(object sender, RoutedEventArgs e)
        {
            if(lbxBestellungen.SelectedIndex != -1)
            {
                Bestellung bestellung = Bestellungen[lbxBestellungen.SelectedIndex];
                bestellungWindow = new BestellungWindow(bestellung);
                bestellungWindow.Show();
            }
        }
    }
}
