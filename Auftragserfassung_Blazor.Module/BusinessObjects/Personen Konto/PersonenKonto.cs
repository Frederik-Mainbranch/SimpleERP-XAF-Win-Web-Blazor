using System;
using System.Linq;
using System.Text;
using System.Resources;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DisplayNameAttribute = DevExpress.Xpo.DisplayNameAttribute;
using DevExpress.ExpressApp.ConditionalAppearance;
using System.IO;
using Auftragserfassung_Blazor.Module.Helpers;
using DevExpress.XtraEditors;

namespace Auftragserfassung_Blazor.Module.BusinessObjects
{
    [DefaultClassOptions]

    public abstract class PersonenKonto : BaseObject
    { 
        public PersonenKonto(Session session)
            : base(session)
        {
        }


        //---------------------------- Klasse ------------------------------------
        //---------------------------- Override Methoden -------------------------------

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

            if (IsLoading == false && IsSaving == false && BearbeitungDurchViewController == false)
            {
                if(newValue != null)
                {
                    if (propertyName == nameof(Lieferadresse_Postleitzahl) && Lieferadresse_Postleitzahl.Length == 5)
                    {
                        //string Postleitzahl_typ = "Lieferadresse";
                        BesorgeAdressdaten((string)newValue, "Lieferadresse");
                    }
                    else if (propertyName == nameof(Rechnungsadresse_Postleitzahl) && Rechnungsadresse_Postleitzahl.Length == 5)
                    {
                        //string Postleitzahl_typ = "Rechnungsadresse";
                        //BesorgeAdressdaten((string)newValue, Postleitzahl_typ);
                        BesorgeAdressdaten((string)newValue, "Rechnungsadresse");
                    }
                }
            }
        }





        //---------------------------- Override Methoden -------------------------------
        //---------------------------- Methoden -------------------------------

        private void BesorgeAdressdaten(string imput, string Postleitzahl_typ)
        {
            txtListenHelper txtListenHelper = new txtListenHelper();
            string[] postleitzahl_zeilen = txtListenHelper.KonvertiereTxtStringzuStringArray(Auftragserfassung_Blazor.Module.Properties.Resources.Postleitzahl);
            string[] ort_zeilen = txtListenHelper.KonvertiereTxtStringzuStringArray(Auftragserfassung_Blazor.Module.Properties.Resources.Ort);
            string[] landkreis_zeilen = txtListenHelper.KonvertiereTxtStringzuStringArray(Auftragserfassung_Blazor.Module.Properties.Resources.landkreis);
            string[] bundesland_zeilen = txtListenHelper.KonvertiereTxtStringzuStringArray(Auftragserfassung_Blazor.Module.Properties.Resources.bundesland);

            bool postleitzahlGefunden = false;
            for (int i = 0; i < postleitzahl_zeilen.Length; i++)
            {
                if (postleitzahl_zeilen[i] == imput) //
                {
                    if (Postleitzahl_typ == "Lieferadresse")
                    {
                        Lieferadresse_Ort = ort_zeilen[i];
                        Lieferadresse_Landkreis = landkreis_zeilen[i];
                        Lieferadresse_Bundesland = bundesland_zeilen[i];
                        Lieferadresse_Land = "Deutschland";
                        postleitzahlGefunden = true;
                        break;
                    }
                    else
                    {
                        Rechnungsadresse_Ort = ort_zeilen[i];
                        Rechnungsadresse_Landkreis = landkreis_zeilen[i];
                        Rechnungsadresse_Bundesland = bundesland_zeilen[i];
                        Rechnungsadresse_Land = "Deutschland";
                        postleitzahlGefunden = true;
                        break;
                    }
                }
            }

                if (postleitzahlGefunden == false && Postleitzahl_typ == "Lieferadresse")
            {
                Lieferadresse_Postleitzahl = "";
                Lieferadresse_Ort = "";
                Lieferadresse_Landkreis = "";
                Lieferadresse_Bundesland = "";
                Lieferadresse_Land = "";
                XtraMessage_np = "Es wurde keine Postleitzahl gefunden!";
            }
            else if(postleitzahlGefunden == false && Postleitzahl_typ == "Rechnungsadresse")
            {
                Rechnungsadresse_Postleitzahl = "";
                Rechnungsadresse_Ort = "";
                Rechnungsadresse_Landkreis = "";
                Rechnungsadresse_Bundesland = "";
                Rechnungsadresse_Land = "";
                XtraMessage_np = "Es wurde keine Postleitzahl gefunden!";
            }
        }


        //---------------------------- Methoden -------------------------------
        //-------------------------------- Properties ---------------------------------------------

        private string _Name;
        [RuleRequiredField]
        public string Name
        {
            get { return _Name; }
            set { SetPropertyValue<string>(nameof(Name), ref _Name, value); }
        }


        private string _Telefonnummer;
        public string Telefonnummer
        {
            get { return _Telefonnummer; }
            set { SetPropertyValue<string>(nameof(Telefonnummer), ref _Telefonnummer, value); }
        }


        private string _Faxnummer;
        public string Faxnummer
        {
            get { return _Faxnummer; }
            set { SetPropertyValue<string>(nameof(Faxnummer), ref _Faxnummer, value); }
        }



        private string _Email;
        public string Email
        {
            get { return _Email; }
            set { SetPropertyValue<string>(nameof(Email), ref _Email, value); }
        }




        private string _IBAN;
        public string IBAN
        {
            get { return _IBAN; }
            set { SetPropertyValue<string>(nameof(IBAN), ref _IBAN, value); }
        }


        private bool _LieferungZurRechnungsadresse;
        [ImmediatePostData]
        public bool LieferungZurRechnungsadresse
        {
            get { return _LieferungZurRechnungsadresse; }
            set { SetPropertyValue<bool>(nameof(LieferungZurRechnungsadresse), ref _LieferungZurRechnungsadresse, value); }
        }


        // Adresse


        private string _RechnungsadresseStraße;
        public string Rechnungsadresse_Straße
        {
            get { return _RechnungsadresseStraße; }
            set { SetPropertyValue<string>(nameof(Rechnungsadresse_Straße), ref _RechnungsadresseStraße, value); }
        }


        private string _RechnungsadressePostleitzahl;
        [ImmediatePostData]
        public string Rechnungsadresse_Postleitzahl
        {
            get { return _RechnungsadressePostleitzahl; }
            set { SetPropertyValue<string>(nameof(Rechnungsadresse_Postleitzahl), ref _RechnungsadressePostleitzahl, value); }
        }


        private string _RechnungsadresseOrt;
        [ModelDefault("AllowEdit", "false")]
        public string Rechnungsadresse_Ort
        {
            get { return _RechnungsadresseOrt; }
            set { SetPropertyValue<string>(nameof(Rechnungsadresse_Ort), ref _RechnungsadresseOrt, value); }
        }


        private string _RechnungsadresseLandkreis;
        [ModelDefault("AllowEdit", "false")]
        public string Rechnungsadresse_Landkreis
        {
            get { return _RechnungsadresseLandkreis; }
            set { SetPropertyValue<string>(nameof(Rechnungsadresse_Landkreis), ref _RechnungsadresseLandkreis, value); }
        }


        private string _RechnungsadresseBundesland;
        [ModelDefault("AllowEdit", "false")]
        public string Rechnungsadresse_Bundesland
        {
            get { return _RechnungsadresseBundesland; }
            set { SetPropertyValue<string>(nameof(Rechnungsadresse_Bundesland), ref _RechnungsadresseBundesland, value); }
        }


        private string _RechnungsadresseLand;
        [ModelDefault("AllowEdit", "false")]
        public string Rechnungsadresse_Land
        {
            get { return _RechnungsadresseLand; }
            set { SetPropertyValue<string>(nameof(Rechnungsadresse_Land), ref _RechnungsadresseLand, value); }
        }


        // 


        private string _LieferadresseStraße;
        public string Lieferadresse_Straße
        {
            get { return _LieferadresseStraße; }
            set { SetPropertyValue<string>(nameof(Lieferadresse_Straße), ref _LieferadresseStraße, value); }
        }


        private string _LieferadressePostleitzahl;
        [ImmediatePostData]
        public string Lieferadresse_Postleitzahl
        {
            get { return _LieferadressePostleitzahl; }
            set { SetPropertyValue<string>(nameof(Lieferadresse_Postleitzahl), ref _LieferadressePostleitzahl, value); }
        }


        private string _LieferadresseOrt;
        [ModelDefault("AllowEdit", "false")]
        public string Lieferadresse_Ort
        {
            get { return _LieferadresseOrt; }
            set { SetPropertyValue<string>(nameof(Lieferadresse_Ort), ref _LieferadresseOrt, value); }
        }


        private string _LieferadresseLandkreis;
        [ModelDefault("AllowEdit", "false")]
        public string Lieferadresse_Landkreis
        {
            get { return _LieferadresseLandkreis; }
            set { SetPropertyValue<string>(nameof(Lieferadresse_Landkreis), ref _LieferadresseLandkreis, value); }
        }


        private string _LieferadresseBundesland;
        [ModelDefault("AllowEdit", "false")]
        public string Lieferadresse_Bundesland
        {
            get { return _LieferadresseBundesland; }
            set { SetPropertyValue<string>(nameof(Lieferadresse_Bundesland), ref _LieferadresseBundesland, value); }
        }


        private string _LieferadresseLand;
        [ModelDefault("AllowEdit", "false")]
        public string Lieferadresse_Land
        {
            get { return _LieferadresseLand; }
            set { SetPropertyValue<string>(nameof(Lieferadresse_Land), ref _LieferadresseLand, value); }
        }

        //-------------------------------- Properties ---------------------------------------------
        //-------------------------------- Non Persistent Properties ---------------------------------------------

        private bool _BearbeitungDurchViewController;
        [NonPersistent]
        [Browsable(false)]
        public bool BearbeitungDurchViewController
        {
            get { return _BearbeitungDurchViewController; }
            set { SetPropertyValue<bool>(nameof(BearbeitungDurchViewController), ref _BearbeitungDurchViewController, value); }
        }


        private string _XtraMessage_np;
        [NonPersistent]
        [Browsable(false)]
        public string XtraMessage_np
        {
            get { return _XtraMessage_np; }
            set { SetPropertyValue<string>(nameof(XtraMessage_np), ref _XtraMessage_np, value); }
        }

        //-------------------------------- Non Persistent Properties ---------------------------------------------

    }
}