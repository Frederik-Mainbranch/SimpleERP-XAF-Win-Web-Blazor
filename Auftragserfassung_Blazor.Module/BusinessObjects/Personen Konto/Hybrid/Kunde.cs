using System;
using System.Linq;
using System.Text;
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
using DevExpress.XtraEditors;
using System.IO;
using DevExpress.Xpo.DB;

namespace Auftragserfassung_Blazor.Module.BusinessObjects

{
    [DefaultClassOptions]
    //[DefaultProperty("VollerName")]

    public class Kunde : Hybrid
    {
        public Kunde(Session session)
            : base(session)
        {
        }

        //---------------------------- Klasse ------------------------------------
        //---------------------------- Override Methoden -------------------------------

        protected override void OnSaving()
        {
            base.OnSaving();

            if(IsDeleted == false && BearbeitungDurchViewController == false)
            {
                if(Kundennummer == 0)
                {
                    SelectedData Maxnummer = Session.ExecuteQuery("SELECT MAX(Kundennummer) FROM Kunde");
                    if ((Maxnummer.ResultSet[0].Rows[0].Values[0] != null))
                    {
                        Kundennummer = int.Parse(Maxnummer.ResultSet[0].Rows[0].Values[0].ToString()) + 1;
                    }
                    else
                    {
                        Kundennummer = 1;
                    }

                    BestimmeVollerName();
                }
            }
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

            if(IsLoading == false && IsSaving == false && BearbeitungDurchViewController == false)
            {
                if(Kundennummer != 0)
                {
                    if(propertyName == nameof(Titel) || propertyName == nameof(Vorname) || propertyName == nameof(Name))
                    {
                        BestimmeVollerName();
                    }
                }
            }
        }

        //---------------------------- Override Methode -------------------------------
        //---------------------------- Methoden -------------------------------

        public void BestimmeVollerName()
        {
            if (Vorname != "")
            {
                VollerName = $"{Titel} {Vorname} {Name}";
            }
            else
            {
                VollerName = $"{Titel} {Name}";
            }
        }

        //----------------------------  Methoden -------------------------------
        //-------------------------------- Properties ---------------------------------------------


        private int _Kundennummer;
        [ModelDefault("AllowEdit", "false")]
       // [Appearance("Verstecke Kundennummer", criteria: nameof(Kundennummer) + "= 0", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace )]
        public int Kundennummer
        {
            get { return _Kundennummer; }
            set { SetPropertyValue<int>(nameof(Kundennummer), ref _Kundennummer, value); }
        }

        private int _AnzahlOffenerBestellungen;
        [ModelDefault("AllowEdit", "false")]
        public int AnzahlOffenerBestellungen
        {
            get { return _AnzahlOffenerBestellungen; }
            set { SetPropertyValue<int>(nameof(AnzahlOffenerBestellungen), ref _AnzahlOffenerBestellungen, value); }
        }


        private string _VollerName;
        [DisplayNameAttribute("Kunde")]
        public string VollerName
        {
            get { return _VollerName; }
            set { SetPropertyValue<string>(nameof(VollerName), ref _VollerName, value); }
        }



        //-------------------------------- Properties ---------------------------------------------
        //-------------------------------- Listen ---------------------------------------------


        [Association("Kunde-Lieferschein")]
        public XPCollection<Lieferschein> Lieferscheine
        {
            get { return GetCollection<Lieferschein>(nameof(Lieferscheine)); }
        }


        [Association("Kunde-Bestellung")]
        public XPCollection<Bestellung> Bestellungen
        {
            get { return GetCollection<Bestellung>(nameof(Bestellungen)); }
        }


        //-------------------------------- Listen ---------------------------------------------
        //-------------------------------- Non Persistent Properties ---------------------------------------------


        //-------------------------------- Non Persistent Properties ---------------------------------------------

    }
}