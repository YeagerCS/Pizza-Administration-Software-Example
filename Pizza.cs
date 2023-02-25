using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pizzer
{
    public class Pizza
    {
        public string ObjectId { get; set; }
        private string name;
        public List<string> zutaten;
        private double einzelpreis;
        private double kcal;
        private double durchmesser;
        private string groesse;

        public string DisplayString { get; set; }

        public Pizza(string objectId, string name, List<string> zutaten, double einzelpreis, double kcal, double durchmesser, string groesse)
        {
            ObjectId = objectId;
            Name = name;
            Zutaten = zutaten;
            Einzelpreis = einzelpreis;
            Kcal = kcal;
            Durchmesser = durchmesser;
            Groesse = groesse;

            string zutatenString = string.Join(", ", zutaten);
            DisplayString = $"{name}: {zutatenString}\t {einzelpreis:N2}CHF";
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<string> Zutaten
        {
            get { return zutaten; }
            set { zutaten = value; }
        }

        public double Einzelpreis
        {
            get { return einzelpreis; }
            set { einzelpreis = value; }
        }

        public double Kcal
        {
            get { return kcal; }
            set { kcal = value; }
        }

        public double Durchmesser
        {
            get { return durchmesser; }
            set { durchmesser = value; }
        }

        public string Groesse
        {
            get { return groesse; }
            set { groesse = value; }
        }

    }
}
