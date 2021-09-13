using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Forms;
using DisplayNameAttribute = System.ComponentModel.DisplayNameAttribute;

namespace Auftragserfassung_Blazor.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DefaultProperty("Bestellnummer")]

    #region//Appearance Rules
    [Appearance("Verstecke Lieferort", TargetItems = nameof(Lieferort_Straße) + ", " + nameof(Lieferort_Postleitzahl) + ", " + nameof(Lieferort_Ort),
            Criteria = nameof(BelegWurdeCommitted) + " == false", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]

    [Appearance("Verstecke Kunden Daten", TargetItems = nameof(Kunde_LieferAdresse_Straße) + ", " + nameof(Kunde_LieferAdresse_Postleitzahl) + ", " +
           nameof(Kunde_LieferAdresse_Ort) + ", " + nameof(Kunde_RechnungsAdresse_Straße) + ", " + nameof(Kunde_RechnungsAdresse_Postleitzahl) + ", "
        + nameof(Kunde_RechnungsAdresse_Ort) + ", " + nameof(Kunde_LieferungZurRechnungsadresse),
        Criteria = nameof(BelegWurdeCommitted) + " == true", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]

    [Appearance("ieferungZurRechnungsadresse", TargetItems = nameof(Kunde_LieferAdresse_Straße) + ", " + nameof(Kunde_LieferAdresse_Postleitzahl) + ", " +
           nameof(Kunde_LieferAdresse_Ort), Criteria = nameof(Kunde_LieferungZurRechnungsadresse) + " == true",
        Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]

    [Appearance("LieferungZurLieferadresse", TargetItems = nameof(Kunde_RechnungsAdresse_Straße) + ", " + nameof(Kunde_RechnungsAdresse_Postleitzahl) + ", " +
           nameof(Kunde_RechnungsAdresse_Ort), Criteria = nameof(Kunde_LieferungZurRechnungsadresse) + " == false",
        Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]

    //[Appearance("test_verstecken", Visibility = ViewItemVisibility.Hide, Context = "ListView", Criteria = "BelegWurdeCommitted == false",
    //    TargetItems = nameof(BestellungsPosition.AnzahlGeliefert) + ", " + nameof(BestellungsPosition.AnzahlOffeneRestmenge))]
    //[Appearance("test_zeigen", Visibility = ViewItemVisibility.Show, Context = "ListView",
    //    Criteria = "Bestellung.BelegWurdeCommitted == true", TargetItems = nameof(BestellungsPosition.AnzahlGeliefert) + ", " + nameof(BestellungsPosition.AnzahlOffeneRestmenge))]

    #endregion



    public class Bestellung : Beleg
    { 
        public Bestellung(Session session)
            : base(session)
        {
        }

        #region Override Methoden

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if(IsLoading == false && IsSaving == false)
            {
                if (propertyName == nameof(Summenrabatt))
                {
                    // setzt den letzten Summenrabatt zurück und wendet dann den neuen Summenrabatt an
                    BildeRechnungsSummen();
                }
                else if(propertyName == nameof(Kunde) && Kunde != null)
                {
                    BesorgeKundenStammdaten();
                }
                else if(propertyName == nameof(BelegWurdeCommitted) && BelegWurdeCommitted == true)
                {
                    BestimmeAnzahlDerOffenenBestellungenDesKunden();

                    if(Bestellnummer < 1)
                    {
                        SelectedData Maxnummer = Session.ExecuteQuery("SELECT MAX(Bestellnummer) FROM Bestellung");
                        if ((Maxnummer.ResultSet[0].Rows[0].Values[0] != null))
                        {
                            Bestellnummer = int.Parse(Maxnummer.ResultSet[0].Rows[0].Values[0].ToString()) + 1;
                        }
                        else
                        {
                            Bestellnummer = 1;
                        }
                    }

                    foreach (BestellungsPosition bestellungsPosition in BestellungsPositionenListe)
                    {
                        bestellungsPosition.ZugehörigeBestellungWurdeCommitted = true;
                    }

                    //schreibt beim Speichern in die DB, ob die Lieferung zur Rechnungsadresse oder Lieferadresse ging
                    if (Kunde_LieferungZurRechnungsadresse == false)
                    {
                        Lieferort_Straße = Kunde_LieferAdresse_Straße;
                        Lieferort_Postleitzahl = Kunde_LieferAdresse_Postleitzahl;
                        Lieferort_Ort = Kunde_LieferAdresse_Ort;
                        Lieferort_Landkreis = Kunde_LieferAdresse_Landkreis;
                        Lieferort_Bundesland = Kunde_LieferAdresse_Bundesland;
                        Lieferort_Land = Kunde_LieferAdresse_Land;
                    }
                    else
                    {
                        Lieferort_Straße = Kunde_RechnungsAdresse_Straße;
                        Lieferort_Postleitzahl = Kunde_RechnungsAdresse_Postleitzahl;
                        Lieferort_Ort = Kunde_RechnungsAdresse_Ort;
                        Lieferort_Landkreis = Kunde_RechnungsAdresse_Landkreis;
                        Lieferort_Bundesland = Kunde_RechnungsAdresse_Bundesland;
                        Lieferort_Land = Kunde_RechnungsAdresse_Land;
                    }
                }
            }
        }


        public override void AfterConstruction()
        {
            base.AfterConstruction();
           
            BestellDatum = DateTime.Today;
        }

        protected override void OnDeleting()
        {
            base.OnDeleting();
            if(BestellungIstAbgearbeitet == false && BelegWurdeCommitted == true)
            {
                BestimmeAnzahlDerOffenenBestellungenDesKunden();
            }
        }


        [Association("Bestellung-Bestellung", typeof(Bestellung))]
        public XPCollection<Bestellung> BestellungListe
        {
            get { return GetCollection<Bestellung>(nameof(BestellungListe)); }
        }

        [Association("Bestellung-Bestellung", typeof(Bestellung))]
        public XPCollection<Bestellung> BestellungListe2
        {
            get { return GetCollection<Bestellung>(nameof(BestellungListe2)); }
        }


        #endregion
        //----------------------------------------------
        #region Methoden

        public void BestimmeAnzahlDerOffenenBestellungenDesKunden()
        {
            int dummy = 0;
            foreach (Bestellung bestellung in Kunde.Bestellungen)
            {
                if(bestellung.BestellungIstAbgearbeitet == false)
                {
                    dummy++;
                }
            }
            Kunde.AnzahlOffenerBestellungen = dummy;
        }

        public void BildeRechnungsSummen() //Summiert die einzelnen Positionen Summen auf und wendet denn den optionalen Summenrabatt an
        {
            SummeRechnungsNetto = 0;
            SummeRechnungsBrutto = 0;

            foreach (BestellungsPosition posi in BestellungsPositionenListe)
            {
                posi.SummeDerPositionNetto_NP = posi.Artikel_Preis * posi.AnzahlBestellteMenge * (1 + (posi.Zeilenrabatt + Summenrabatt) / 100);
                posi.SummeDerPositionBrutto_NP = posi.SummeDerPositionNetto_NP * (1 + posi.Artikel_Steuersatz / 100);

                posi.SummeDerPositionNetto_NP = Math.Round(posi.SummeDerPositionNetto_NP, 4);
                posi.SummeDerPositionBrutto_NP = Math.Round(posi.SummeDerPositionBrutto_NP, 4);

                SummeRechnungsNetto += posi.SummeDerPositionNetto_NP;
                SummeRechnungsBrutto += posi.SummeDerPositionBrutto_NP;
            }
        }

        private void BesorgeKundenStammdaten() //Wenn sich der Kunde geändert hat, werden dessen Stammdaten aus der DB besorgt 
        {
            Kunde_LieferAdresse_Straße = Kunde.Lieferadresse_Straße;
            Kunde_LieferAdresse_Postleitzahl = Kunde.Lieferadresse_Postleitzahl;
            Kunde_LieferAdresse_Ort = Kunde.Lieferadresse_Ort;
            Kunde_LieferAdresse_Landkreis = Kunde.Lieferadresse_Landkreis;
            Kunde_LieferAdresse_Bundesland = Kunde.Lieferadresse_Bundesland;
            Kunde_LieferAdresse_Land = Kunde.Lieferadresse_Land;

            Kunde_RechnungsAdresse_Straße = Kunde.Rechnungsadresse_Straße;
            Kunde_RechnungsAdresse_Postleitzahl = Kunde.Rechnungsadresse_Postleitzahl;
            Kunde_RechnungsAdresse_Ort = Kunde.Rechnungsadresse_Ort;
            Kunde_RechnungsAdresse_Landkreis = Kunde.Rechnungsadresse_Landkreis;
            Kunde_RechnungsAdresse_Bundesland = Kunde.Rechnungsadresse_Bundesland;
            Kunde_RechnungsAdresse_Land = Kunde.Rechnungsadresse_Land;

            Kunde_LieferungZurRechnungsadresse = Kunde.LieferungZurRechnungsadresse;
        }


        #endregion
        //---------------------------------------------------
        #region Listen

        [DevExpress.Xpo.Aggregated, Association("Bestellung-BestellungsPosition")]
        public XPCollection<BestellungsPosition> BestellungsPositionenListe
        {
            get { return GetCollection<BestellungsPosition>(nameof(BestellungsPositionenListe)); }
        }

        #endregion

        //--------------------------------

        #region Properties


        private int _Bestellnummer;
        [ModelDefault("AllowEdit", "false")]
        [Appearance("Verstecke Bestellnummer", criteria: nameof(Bestellnummer) + " == 0", Visibility = ViewItemVisibility.Hide)]
        public int Bestellnummer
        {
            get { return _Bestellnummer; }
            set { SetPropertyValue<int>(nameof(Bestellnummer), ref _Bestellnummer, value); }
        }



        private Kunde _Kunde;
        [RuleRequiredField]
        [ImmediatePostData]
        [Association("Kunde-Bestellung")]
        public Kunde Kunde
        {
            get { return _Kunde; }
            set { SetPropertyValue<Kunde>(nameof(Kunde), ref _Kunde, value); }
        }


        private bool _BestellungIstAbgearbeitet;
        [DisplayNameAttribute("Bestellung ist abgearbeitet")]
        [ModelDefault("AllowEdit", "false")]
        [VisibleInDashboards(false)]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        public bool BestellungIstAbgearbeitet
        {
            get { return _BestellungIstAbgearbeitet; }
            set { SetPropertyValue<bool>(nameof(BestellungIstAbgearbeitet), ref _BestellungIstAbgearbeitet, value); }
        }


        private double _Summenrabatt;
        [ImmediatePostData]
        public double Summenrabatt
        {
            get { return _Summenrabatt; }
            set { SetPropertyValue<double>(nameof(Summenrabatt), ref _Summenrabatt, value); }
        }


        private DateTime _BestellDatum;
        [ModelDefault("AllowEdit", "false")]
        public DateTime BestellDatum
        {
            get { return _BestellDatum; }
            set { SetPropertyValue<DateTime>(nameof(BestellDatum), ref _BestellDatum, value); }
        }



        //private Lieferschein _Lieferschein;
        //[Association("Lieferschein-Bestellung")]
        //[Browsable(false)]
        //public Lieferschein Lieferschein
        //{
        //    get { return _Lieferschein; }
        //    set { SetPropertyValue<Lieferschein>(nameof(Lieferschein), ref _Lieferschein, value); }
        //}

        #endregion


    }
}