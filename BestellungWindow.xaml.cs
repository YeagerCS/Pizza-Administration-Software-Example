using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
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

namespace Pizzer
{
    /// <summary>
    /// Interaction logic for BestellungWindow.xaml
    /// </summary>
    public partial class BestellungWindow : Window
    {
        List<Kunde> Kunden  = new List<Kunde>();
        List<Pizza> Pizzas= new List<Pizza>();
        List<Pizza> SelectedPizzas = new List<Pizza>();
        IMongoDatabase database = new Config().getConnection();
        double finalPrice = 0;
        public event EventHandler DataInserted;

        public BestellungWindow()
        {
            InitializeComponent();
            ReadDatabase();
            this.ResizeMode = ResizeMode.NoResize;
        }

        public BestellungWindow(Bestellung bestellung)
        {
            InitializeComponent();
            ReadDatabase();
            this.ResizeMode = ResizeMode.NoResize;
            SelectedPizzas = bestellung.Pizza;
            foreach(Pizza pizza in SelectedPizzas)
            {
                string zutatenString = string.Join(", ", pizza.Zutaten);
                pizza.DisplayString = $"{pizza.Name}: {zutatenString}\t {pizza.Einzelpreis:N2}CHF";
            }

            var selectedKunde = Kunden.FirstOrDefault(x => x._id == bestellung.Kunde._id);
            lblPreis.Content += bestellung.Total.ToString() + "CHF";

            lbxKunden.SelectedItem = selectedKunde;
            AddRangePizza(lbxSelectedPizzas, SelectedPizzas);

            string time = bestellung.BestellDatum.TimeOfDay.ToString("hh\\:mm\\:ss");
            lblTime.Content = $"Datum und Uhrzeit: {bestellung.BestellDatum.Date.ToString("dd\\.MM\\.yyyy")} {time}";
            btnAddPizza.IsEnabled = false;
            btnRemovePizza.IsEnabled = false;
            btnBestellung.IsEnabled = false;
            lbxKunden.IsEnabled = false;
        }


        private void ReadDatabase()
        {
            Kunden.Clear();
            Pizzas.Clear();
            var collection = database.GetCollection<BsonDocument>("Kunde");

            var filter = new BsonDocument();
            var documents = collection.Find(filter).ToList();
            foreach (var doc in documents)
            {
                Kunden.Add(new Kunde(new ObjectId(doc["_id"]+""), doc["Nachname"] + "", doc["Vorname"] + "", BsonSerializer.Deserialize<Adresse>(doc["Adresse"].AsBsonDocument), doc["Telefon"] + "", doc["Email"] + "", doc["Geburtsdatum"].ToUniversalTime().ToLocalTime()));
            }

            var collectionPizza = database.GetCollection<BsonDocument>("Pizza");

            var documentsPizza = collectionPizza.Find(filter).ToList();
            foreach (var doc1 in documentsPizza)
            {
                Pizzas.Add(new Pizza(doc1["_id"] + "", doc1["Name"] + "", new List<string>() { doc1["Zutaten"]["Zutat"] + "", doc1["Zutaten"]["Extrazutat"] + "" }, Convert.ToDouble(doc1["Einzelpreis"]), Convert.ToDouble(doc1["Kcal"]), Convert.ToDouble(doc1["Durchmesser"]), doc1["Groesse"] + ""));
            }


            lbxKunden.Items.Clear();
            lbxPizza.Items.Clear();
            AddRange(lbxKunden, Kunden);
            AddRangePizza(lbxPizza, Pizzas);
        }

        private void AddRange(ListBox listBox, List<Kunde> list)
        {
            foreach (Kunde str in list)
            {
                listBox.Items.Add(str);
            }
        }

        private void AddRangePizza(ListBox listBox, List<Pizza> list)
        {
            foreach (Pizza str in list)
            {
                listBox.Items.Add(str);
            }
        }

        private double CalculatePrice(List<Pizza> list)
        {
            finalPrice = 0;
            foreach(Pizza pizza in list) 
            {
                finalPrice += pizza.Einzelpreis;
            }

            return finalPrice;
        }


        private void btnAddPizza_Click(object sender, RoutedEventArgs e)
        {
            int index = lbxPizza.SelectedIndex;
            if(index != -1)
            {
                lbxSelectedPizzas.Items.Clear();
                SelectedPizzas.Add(Pizzas[index]);
                AddRangePizza(lbxSelectedPizzas, SelectedPizzas);
                lblPreis.Content = "Endpreis: ";
                lblPreis.Content += $"{CalculatePrice(SelectedPizzas)}CHF";
            }
        }

        private void lbxPizza_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblDurchmesser.Content = "Durchmesser: ";
            lblEinzelpreis.Content = "Einzelpreis: ";
            lblKcal.Content = "Kcal: ";
            lblName.Content = "Name: ";
            lblSize.Content = "Groesse: ";
            lblZutaten.Content = "Zutaten: ";

            lblDurchmesser.Content += Pizzas[lbxPizza.SelectedIndex].Durchmesser + "cm";
            lblEinzelpreis.Content += Pizzas[lbxPizza.SelectedIndex].Einzelpreis + "CHF";
            lblKcal.Content += Pizzas[lbxPizza.SelectedIndex].Kcal + "";
            lblName.Content += Pizzas[lbxPizza.SelectedIndex].Name;
            lblSize.Content += Pizzas[lbxPizza.SelectedIndex].Groesse;
            lblZutaten.Content += Pizzas[lbxPizza.SelectedIndex].Zutaten[0] + ", " + Pizzas[lbxPizza.SelectedIndex].Zutaten[1];
        }

        private void lbxSelectedPizzas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lbxSelectedPizzas.SelectedIndex != -1)
            {
                lblDurchmesser.Content = "Durchmesser: ";
                lblEinzelpreis.Content = "Einzelpreis: ";
                lblKcal.Content = "Kcal: ";
                lblName.Content = "Name: ";
                lblSize.Content = "Groesse: ";
                lblZutaten.Content = "Zutaten: ";

                lblDurchmesser.Content += SelectedPizzas[lbxSelectedPizzas.SelectedIndex].Durchmesser + "cm";
                lblEinzelpreis.Content += SelectedPizzas[lbxSelectedPizzas.SelectedIndex].Einzelpreis + "CHF";
                lblKcal.Content += SelectedPizzas[lbxSelectedPizzas.SelectedIndex].Kcal + "";
                lblName.Content += SelectedPizzas[lbxSelectedPizzas.SelectedIndex].Name;
                lblSize.Content += SelectedPizzas[lbxSelectedPizzas.SelectedIndex].Groesse;
                lblZutaten.Content += SelectedPizzas[lbxSelectedPizzas.SelectedIndex].Zutaten[0] + ", " + SelectedPizzas[lbxSelectedPizzas.SelectedIndex].Zutaten[1];
            }
        }

        private void btnRemovePizza_Click(object sender, RoutedEventArgs e)
        {
            if(lbxSelectedPizzas.SelectedIndex != -1)
            {
                SelectedPizzas.RemoveAt(lbxSelectedPizzas.SelectedIndex);
                lbxSelectedPizzas.Items.Clear();
                AddRangePizza(lbxSelectedPizzas, SelectedPizzas);

                lblPreis.Content = "Endpreis: ";
                lblPreis.Content += $"{CalculatePrice(SelectedPizzas)}CHF";
            }
        }

        private void btnBestellung_Click(object sender, RoutedEventArgs e)
        {
            if(lbxKunden.SelectedIndex != -1 && SelectedPizzas.Count > 0)
            {
                MessageBox.Show("Bestellung Akzeptiert", "Akzeptiert", MessageBoxButton.OK, MessageBoxImage.Information);
                InsertBestellung();
                this.Close();
            }
            else
            {
                MessageBox.Show("Bestellung Abgelehnt, fehlende Daten", "Abgelehnt", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            DataInserted?.Invoke(this, EventArgs.Empty);
        }

        public int GetOrderNumber()
        {
            int orderNumber = 1;
            List<int> rray = new List<int>();
            var collection = database.GetCollection<BsonDocument>("Bestellung");
            var filter = new BsonDocument();
            var document = collection.Find(filter).ToList();
            if(document.Count > 0)
            {
                foreach (var doc in document)
                {
                    rray.Add(doc["Bestellnummer"].AsInt32);
                }

                orderNumber = rray.Last() + 1;
            }
            
            return orderNumber;

        }

        private void InsertBestellung()
        {
            var collection = database.GetCollection<BsonDocument>("Bestellung");
            Kunde kunde = Kunden[lbxKunden.SelectedIndex];

            var pizzaArray = new BsonArray();

            foreach(var pizza in SelectedPizzas)
            {
                var pizzaDocument = new BsonDocument
                {
                    {"Name", pizza.Name},
                    {"Zutaten", new BsonArray { pizza.zutaten[0], pizza.zutaten[1] } },
                    {"Einzelpreis", pizza.Einzelpreis },
                    {"Kcal", pizza.Kcal },
                    {"Durchmesser", pizza.Durchmesser },
                    {"Groesse", pizza.Groesse }
                };

                pizzaArray.Add(pizzaDocument);
            }

            var document = new BsonDocument
            {
                {"Bestellnummer", GetOrderNumber() },
                {"Bestelldatum", DateTime.Now },
                {"Kunde", new BsonDocument
                    {
                        {"_id", new ObjectId(kunde._id+"") },
                        {"Nachname", kunde.Nachname },
                        {"Vorname", kunde.Vorname },
                        {"Adresse", new BsonDocument
                            {
                                {"Strasse", kunde.Strasse},
                                { "Plz", kunde.Plz },
                                { "Ort", kunde.Ort }
                            }
                        },
                        {"Telefon", kunde.Telefon },
                        {"Email", kunde.Email },
                        {"Geburtsdatum", kunde.Geburtsdatum }
                    }
                },
            };

            document.Add("Bestellposition", new BsonDocument { { "Pizza", pizzaArray } });
            document.Add("Total", CalculatePrice(SelectedPizzas));

            collection.InsertOne(document);
        }
    }
}
