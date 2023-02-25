using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzer
{
    public class Bestellung
    {
        public string objectId { get; set; }
        private int bestellNummer;
        private DateTime bestellDatum;
        private Kunde kunde;
        private List<Pizza> pizza;
        private double total;

        

        public string DisplayString { get; set; }

        public double Total
        {
            get { return total; }
            set { total = value; }
        }

        public List<Pizza> Pizza
        {
            get { return pizza; }
            set { pizza = value; }
        }
        
        public int BestellNummer
        {
            get { return bestellNummer; }
            set { bestellNummer = value; }
        }

        public DateTime BestellDatum
        {
            get { return bestellDatum; }
            set
            {
                bestellDatum = value;
            }
        }

        public Kunde Kunde
        {
            get { return kunde; }
            set { kunde = value;}
        }

        public Bestellung(string objectId, int bestellNummer, DateTime bestellDatum, Kunde kunde, List<Pizza> pizza, double total)
        {
            this.bestellNummer = bestellNummer;
            this.bestellDatum = bestellDatum;
            this.kunde = kunde;
            this.pizza = pizza;
            this.objectId= objectId;
            this.total = total;

            DisplayString = "Nr: " + bestellNummer + " " + kunde.Vorname + " " + kunde.Nachname;
        }
    }
}
