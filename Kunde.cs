using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzer
{
    public class Kunde
    {
        public ObjectId _id { get; set; }
        private string nachname; 
        private string vorname;
        public Adresse Adresse { get; set; }
        private string telefon;
        private string email;
        private DateTime geburtsdatum;
        public string DisplayString { get; set; }

        

        public string Nachname
        {
            get { return nachname; }
            set { nachname = value; }
        }

        public string Strasse
        {
            get { return Adresse.Strasse; }
            set { Adresse.Strasse = value; }
        }

        public string Ort
        {
            get { return Adresse.Ort; }
            set { Adresse.Ort = value; }
        }

        public string Plz
        {
            get { return Adresse.Plz; }
            set { Adresse.Plz = value; }
        }

        public string Vorname
        {
            get { return vorname; }
            set { vorname = value; }
        }


        public string Telefon
        {
            get { return telefon; }
            set { telefon = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public DateTime Geburtsdatum        
        {
            get { return geburtsdatum; }
            set { geburtsdatum = value; }
        }

        public Kunde()
        {

        }

        public Kunde(ObjectId _id, string nachname, string vorname, Adresse adresse, string telefon, string email, DateTime geburtsdatum)
        {
            this._id = _id;
            Nachname = nachname;
            Vorname = vorname;
            Adresse= adresse;
            Telefon = telefon;
            Email = email;
            Geburtsdatum = geburtsdatum;

            DisplayString= $"{vorname} {nachname}";
        }
    }
    public class Adresse
    {
        public string Strasse { get; set; }
        public string Plz { get; set; }
        public string Ort { get; set; }
    }
}
