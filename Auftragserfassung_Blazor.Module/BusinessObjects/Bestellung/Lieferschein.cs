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
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.ExpressApp.ConditionalAppearance;
using Auftragserfassung_Blazor.Module.BusinessObjects;
using DevExpress.Xpo.DB;
using DevExpress.ExpressApp.Editors;

namespace Auftragserfassung_Blazor.Module.BusinessObjects
{
    [DefaultClassOptions]
    [ExpandObjectMembers(ExpandObjectMembers.Always)]



    public class Lieferschein : Beleg
    {
        private bool AfterConstructionErledigt = false;
        public Lieferschein(Session session)
            : base(session)
        {
        }


        #region Override Methoden


        public override void AfterConstruction()
        {
            base.AfterConstruction();
            LieferscheinDatum = DateTime.Today;
            LieferungZurBestellungsLieferadresse = true;
            AfterConstructionErledigt = true;
        }


        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

            if (IsLoading == false && IsSaving == false && AfterConstructionErledigt == true)
            {
                if (propertyName == nameof(Kunde))
                {
                    if (Kunde != null)
                    {
                        Bestellung = null;
                        BesorgeKundenStammdaten();
                    }
                    else
                    {
                        Bestellung = null;
                    }
                }
                else if (propertyName == nameof(Bestellung))
                {

                    if (oldValue == null && newValue != null)
                    {//importiere neuen Auftrag
                        ImportiereAuftragStammdaten();
                    }
                    else if (oldValue != null && newValue != null)
                    {//es wurde ein anderer Auftrag zum importieren ausgewählt
                        ÄndereImportiertenAuftrag();
                    }
                    else if (oldValue != null && newValue == null)
                    {//es soll kein Auftrag importiert werden
                        LöscheImportiertePositionen();
                    }
                }
                else if (propertyName == nameof(LieferungZurBestellungsLieferadresse) || propertyName == nameof(LieferungZurKundenLieferadresse) || propertyName == nameof(LieferungZurKundenRechnungsadresse))
                {   //Wohin die Lieferung gehen soll
                    if ((bool)newValue == true)
                    {
                        if (propertyName == nameof(LieferungZurBestellungsLieferadresse))
                        {
                            LieferungZurKundenLieferadresse = false;
                            LieferungZurKundenRechnungsadresse = false;
                        }
                        else if (propertyName == nameof(LieferungZurKundenLieferadresse))
                        {
                            LieferungZurKundenRechnungsadresse = false;
                            LieferungZurBestellungsLieferadresse = false;
                        }
                        else if (propertyName == nameof(LieferungZurKundenRechnungsadresse))
                        {
                            LieferungZurBestellungsLieferadresse = false;
                            LieferungZurKundenLieferadresse = false;
                        }
                    }
                }
            }
        }


        protected override void OnSaving()
        {
            base.OnSaving();

            if(IsDeleted == false)
            {
                BelegWurdeCommitted = true;

                //Lieferscheinnummer
                SelectedData Maxnummer = Session.ExecuteQuery("SELECT MAX(Lieferscheinnummer) FROM Lieferschein");
                if ((Maxnummer.ResultSet[0].Rows[0].Values[0] != null))
                {
                    Lieferscheinnummer = int.Parse(Maxnummer.ResultSet[0].Rows[0].Values[0].ToString()) + 1;
                }
                else
                {
                    Lieferscheinnummer = 1;
                }

                //Ist die Bestellung abgearbeitet?
                bool OffenePositionenVorhanden = false;
                foreach (LieferscheinPosition lieferscheinPosition in LieferscheinPositionenListe)
                {
                    if(lieferscheinPosition.LeerePositionVorCommit == false)
                    {
                        lieferscheinPosition.UrsprungsPosition.AnzahlGeliefert += lieferscheinPosition.AnzahlLiefermenge;
                        lieferscheinPosition.AnzahlGeliefert += lieferscheinPosition.AnzahlLiefermenge;
                        lieferscheinPosition.UrsprungsPosition.AnzahlOffeneRestmenge = lieferscheinPosition.UrsprungsPosition.AnzahlBestellteMenge - lieferscheinPosition.UrsprungsPosition.AnzahlGeliefert;
                        lieferscheinPosition.AnzahlOffeneRestmenge = lieferscheinPosition.AnzahlBestellteMenge - lieferscheinPosition.AnzahlGeliefert;
                        if (lieferscheinPosition.UrsprungsPosition.AnzahlOffeneRestmenge > 0)
                        {
                            OffenePositionenVorhanden = true;
                        }
                        else
                        {
                            lieferscheinPosition.UrsprungsPosition.PositionIstVollständig = true;
                        }
                    }
                }

                if (OffenePositionenVorhanden == false)
                {
                    Bestellung.BestellungIstAbgearbeitet = true;
                    Bestellung.BestimmeAnzahlDerOffenenBestellungenDesKunden();
                }

                // Lieferschein
                //setzen des Lieferort
                if (LieferungZurBestellungsLieferadresse == true)
                {
                    Lieferort_Straße = Bestellung_Lieferort_Straße;
                    Lieferort_Postleitzahl = Bestellung_Lieferort_Postleitzahl;
                    Lieferort_Ort = Bestellung_Lieferort_Ort;
                    Lieferort_Landkreis = Bestellung_Lieferort_Landkreis;
                    Lieferort_Bundesland = Bestellung_Lieferort_Bundesland;
                    Lieferort_Land = Bestellung_Lieferort_Land;
                }
                else if (LieferungZurKundenLieferadresse == true)
                {
                    Lieferort_Straße = Kunde_LieferAdresse_Straße;
                    Lieferort_Postleitzahl = Kunde_LieferAdresse_Postleitzahl;
                    Lieferort_Ort = Kunde_LieferAdresse_Ort;
                    Lieferort_Landkreis = Kunde_LieferAdresse_Landkreis;
                    Lieferort_Bundesland = Kunde_LieferAdresse_Bundesland;
                    Lieferort_Land = Kunde_LieferAdresse_Land;
                }
                else if (LieferungZurKundenRechnungsadresse == true)
                {
                    Lieferort_Straße = Kunde_RechnungsAdresse_Straße;
                    Lieferort_Postleitzahl = Kunde_RechnungsAdresse_Postleitzahl;
                    Lieferort_Ort = Kunde_RechnungsAdresse_Ort;
                    Lieferort_Landkreis = Kunde_RechnungsAdresse_Landkreis;
                    Lieferort_Bundesland = Kunde_RechnungsAdresse_Bundesland;
                    Lieferort_Land = Kunde_RechnungsAdresse_Land;
                }

                BelegWurdeCommitted = true;
            }
        }


        protected override void OnDeleting()
        {
            base.OnDeleting();

            for (int i = LieferscheinPositionenListe.Count; i > 0; i--)
            {
                LieferscheinPositionenListe[i - 1].Delete();
            }

            foreach (BestellungsPosition bestellungsPosition in Bestellung.BestellungsPositionenListe)
            {
                if(bestellungsPosition.AnzahlOffeneRestmenge > 0)
                {
                    Bestellung.BestellungIstAbgearbeitet = false;
                    break;
                }
            }

            if(Bestellung != null)
            {
                Bestellung.BestimmeAnzahlDerOffenenBestellungenDesKunden();
            }
        }


        #endregion
        //---------------------------------------------------------
        #region Methoden


        public void ÜberprüfeZustandDerBestellung()
        {
            //überprüfen, ob die Bestellung vollständig ausgeliefert wurde
            bool allePositionenGespeichert = true;
            foreach (LieferscheinPosition lieferscheinPosition in LieferscheinPositionenListe)
            {
                if (lieferscheinPosition.PositionWurdeCommited == false)
                {
                    allePositionenGespeichert = false;
                    break;
                }
            }
            if (allePositionenGespeichert == true)
            {
                bool unvollständig = false;
                foreach (BestellungsPosition bestellungsPosition in Bestellung.BestellungsPositionenListe)
                {
                    if (bestellungsPosition.PositionIstVollständig == false)
                    {
                        unvollständig = true;
                        break;
                    }
                }
                if (unvollständig == false)
                {
                    Bestellung.BestellungIstAbgearbeitet = true;
                    Bestellung.Save();
                }
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

        private void ImportiereAuftragStammdaten()
        {
            if (Bestellung != null)
            {
                //Importieren der Bestellungspositionen
                Bestellung.BestellungsPositionenListe.Sorting.Add(new SortProperty(nameof(BestellungsPosition.Positionsnummer),DevExpress.Xpo.DB.SortingDirection.Ascending));
                foreach (BestellungsPosition posi in Bestellung.BestellungsPositionenListe)
                {
                    if (posi.AnzahlGeliefert < posi.AnzahlBestellteMenge)
                    {
                        LieferscheinPosition neueLieferscheinPosition = new LieferscheinPosition(Session)
                        {
                            Artikel = posi.Artikel,
                            Artikel_Preis = posi.Artikel_Preis,
                            AnzahlBestellteMenge = posi.AnzahlBestellteMenge,
                            AnzahlOffeneRestmenge = posi.AnzahlBestellteMenge - posi.AnzahlGeliefert,
                            AnzahlGeliefert = posi.AnzahlGeliefert,
                            Zeilenrabatt = posi.Zeilenrabatt,
                            Artikel_Steuersatz = posi.Artikel_Steuersatz,
                            PositionsnummerDerBestellung = posi.Positionsnummer,
                            UrsprungsPosition = posi
                        };
                        LieferscheinPositionenListe.Add(neueLieferscheinPosition);

                        //Hier ist der Fehler

                        neueLieferscheinPosition.Positionsnummer = LieferscheinPositionenListe.Count;   //Debug
                    }
                }

                //Importieren der Auftragsstammdaten
                Summenrabatt = Bestellung.Summenrabatt;

                Bestellung_Lieferort_Straße = Bestellung.Lieferort_Straße;
                Bestellung_Lieferort_Postleitzahl = Bestellung.Lieferort_Postleitzahl;
                Bestellung_Lieferort_Ort = Bestellung.Lieferort_Ort;
                Bestellung_Lieferort_Landkreis = Bestellung.Lieferort_Landkreis;
                Bestellung_Lieferort_Bundesland = Bestellung.Lieferort_Bundesland;
                Bestellung_Lieferort_Land = Bestellung.Lieferort_Land;
            }
        }


        private void ÄndereImportiertenAuftrag()
        {
            LöscheImportiertePositionen();
            ImportiereAuftragStammdaten();
        }


        private void LöscheImportiertePositionen()
        {
            for (int i = LieferscheinPositionenListe.Count; i > 0; i--)
            {
                LieferscheinPositionenListe[i - 1].Delete();
            }
        }


        public void BildeRechnungsSummen() //Summiert die einzelnen Positionen Summen auf und wendet denn den optionalen Summenrabatt an
        {
            SummeRechnungsNetto = 0;
            SummeRechnungsBrutto = 0;

            foreach (LieferscheinPosition posi in LieferscheinPositionenListe)
            {
                posi.SummeDerPositionNetto_NP = posi.Artikel_Preis * posi.AnzahlLiefermenge * (1 + (posi.Zeilenrabatt + Summenrabatt) / 100);
                posi.SummeDerPositionBrutto_NP = posi.SummeDerPositionNetto_NP * (1 + posi.Artikel_Steuersatz / 100);

                posi.SummeDerPositionNetto_NP = Math.Round(posi.SummeDerPositionNetto_NP, 4);
                posi.SummeDerPositionBrutto_NP = Math.Round(posi.SummeDerPositionBrutto_NP, 4);

                SummeRechnungsNetto += posi.SummeDerPositionNetto_NP;
                SummeRechnungsBrutto += posi.SummeDerPositionBrutto_NP;
            }
        }


        #endregion

        #region Properties


        private int _Lieferscheinnummer;
        [ModelDefault("AllowEdit", "false")]
        [Appearance("Verstecke Lieferscheinnummer", criteria: nameof(Lieferscheinnummer) + " = 0", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]
        public int Lieferscheinnummer
        {
            get { return _Lieferscheinnummer; }
            set { SetPropertyValue<int>(nameof(Lieferscheinnummer), ref _Lieferscheinnummer, value); }
        }



        private Kunde _Kunde;
        [ImmediatePostData]
        [RuleRequiredField]
        [Association("Kunde-Lieferschein")]
        public Kunde Kunde
        {
            get { return _Kunde; }
            set { SetPropertyValue<Kunde>(nameof(Kunde), ref _Kunde, value); }
        }


        private Bestellung _Bestellung;
        [DataSourceProperty("Kunde.Bestellungen")]
        [DataSourceCriteria("BestellungIstAbgearbeitet = false")]
        [Appearance("Sperre Bestellung", Enabled = false, Criteria = nameof(Kunde) + " = NULL")]
        [ImmediatePostData]
        public Bestellung Bestellung
        {
            get { return _Bestellung; }
            set { SetPropertyValue<Bestellung>(nameof(Bestellung), ref _Bestellung, value); }
        }

        //

        private string _BestellungLieferortStraße;
        [NonPersistent]
        public string Bestellung_Lieferort_Straße
        {
            get { return _BestellungLieferortStraße; }
            set { SetPropertyValue<string>(nameof(Bestellung_Lieferort_Straße), ref _BestellungLieferortStraße, value); }
        }


        private string _BestellungLieferortPostleitzahl;
        [NonPersistent]
        public string Bestellung_Lieferort_Postleitzahl
        {
            get { return _BestellungLieferortPostleitzahl; }
            set { SetPropertyValue<string>(nameof(Bestellung_Lieferort_Postleitzahl), ref _BestellungLieferortPostleitzahl, value); }
        }


        private string _BestellungLieferortOrt;
        [NonPersistent]
        public string Bestellung_Lieferort_Ort
        {
            get { return _BestellungLieferortOrt; }
            set { SetPropertyValue<string>(nameof(Bestellung_Lieferort_Ort), ref _BestellungLieferortOrt, value); }
        }


        private string _BestellungLieferortLandkreis;
        [NonPersistent]
        public string Bestellung_Lieferort_Landkreis
        {
            get { return _BestellungLieferortLandkreis; }
            set { SetPropertyValue<string>(nameof(Bestellung_Lieferort_Landkreis), ref _BestellungLieferortLandkreis, value); }
        }


        private string _BestellungLieferortBundesland;
        [NonPersistent]
        public string Bestellung_Lieferort_Bundesland
        {
            get { return _BestellungLieferortBundesland; }
            set { SetPropertyValue<string>(nameof(Bestellung_Lieferort_Bundesland), ref _BestellungLieferortBundesland, value); }
        }


        private string _BestellungLieferortLand;
        [NonPersistent]
        public string Bestellung_Lieferort_Land
        {
            get { return _BestellungLieferortLand; }
            set { SetPropertyValue<string>(nameof(Bestellung_Lieferort_Land), ref _BestellungLieferortLand, value); }
        }

        ////nach dem Speichern, wohin die Lieferung ging

        //private string _LieferscheinLieferortStraße;
        //[Appearance("Verstecke LieferscheinLieferortStraße", TargetItems = "LieferscheinLieferortStraße", Context = "Oid == -1", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
        //public string LieferscheinLieferortStraße
        //{
        //    get { return _LieferscheinLieferortStraße; }
        //    set { SetPropertyValue<string>(nameof(LieferscheinLieferortStraße), ref _LieferscheinLieferortStraße, value); }
        //}


        //private string _LieferscheinLieferortPostleitzahl;
        //[Appearance("Verstecke LieferscheinLieferortPostleitzahl", TargetItems = "LieferscheinLieferortPostleitzahl", Context = "Oid == -1", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
        //public string LieferscheinLieferortPostleitzahl
        //{
        //    get { return _LieferscheinLieferortPostleitzahl; }
        //    set { SetPropertyValue<string>(nameof(LieferscheinLieferortPostleitzahl), ref _LieferscheinLieferortPostleitzahl, value); }
        //}


        //private string _LieferscheinLieferortOrt;
        //[Appearance("Verstecke LieferscheinLieferortOrt", TargetItems = "LieferscheinLieferortOrt", Context = "Oid == -1", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
        //public string LieferscheinLieferortOrt
        //{
        //    get { return _LieferscheinLieferortOrt; }
        //    set { SetPropertyValue<string>(nameof(LieferscheinLieferortOrt), ref _LieferscheinLieferortOrt, value); }
        //}


        //private string _LieferscheinLieferortLand;
        //[Appearance("Verstecke LieferscheinLieferortLand", TargetItems = "LieferscheinLieferortLand", Context = "Oid == -1", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
        //public string LieferscheinLieferortLand
        //{
        //    get { return _LieferscheinLieferortLand; }
        //    set { SetPropertyValue<string>(nameof(LieferscheinLieferortLand), ref _LieferscheinLieferortLand, value); }
        //}


        private bool _LieferungZurAuftragsLieferadresse;
        [ImmediatePostData]
        [VisibleInListView(false)]
        [Appearance("Sperre LieferungZurBestellungsLieferadresse", criteria: nameof(LieferungZurBestellungsLieferadresse) + " = true", Enabled = false)]
        public bool LieferungZurBestellungsLieferadresse
        {
            get { return _LieferungZurAuftragsLieferadresse; }
            set { SetPropertyValue<bool>(nameof(LieferungZurBestellungsLieferadresse), ref _LieferungZurAuftragsLieferadresse, value); }
        }


        private bool _LieferungZurKundenLieferadresse;
        [ImmediatePostData]
        [VisibleInListView(false)]
        [Appearance("Sperre LieferungZurKundenLieferadresse", criteria: nameof(LieferungZurKundenLieferadresse) + " = true", Enabled = false)]
        public bool LieferungZurKundenLieferadresse
        {
            get { return _LieferungZurKundenLieferadresse; }
            set { SetPropertyValue<bool>(nameof(LieferungZurKundenLieferadresse), ref _LieferungZurKundenLieferadresse, value); }
        }


        private bool _LieferungZurKundenRechnungsadresse;
        [ImmediatePostData]
        [VisibleInListView(false)]
        [Appearance("Sperre LieferungZurKundenRechnungsadresse", criteria: nameof(LieferungZurKundenRechnungsadresse) + " = true", Enabled = false)]
        public bool LieferungZurKundenRechnungsadresse
        {
            get { return _LieferungZurKundenRechnungsadresse; }
            set { SetPropertyValue<bool>(nameof(LieferungZurKundenRechnungsadresse), ref _LieferungZurKundenRechnungsadresse, value); }
        }


        private DateTime _LieferscheinDatum;
        [ModelDefault("AllowEdit", "false")]
        public DateTime LieferscheinDatum
        {
            get { return _LieferscheinDatum; }
            set { SetPropertyValue<DateTime>(nameof(LieferscheinDatum), ref _LieferscheinDatum, value); }
        }


        private double _Summenrabatt;
        [ModelDefault("AllowEdit", "false")]
        [ImmediatePostData]
        public double Summenrabatt
        {
            get { return _Summenrabatt; }
            set { SetPropertyValue<double>(nameof(Summenrabatt), ref _Summenrabatt, value); }
        }


        #endregion
        //-------------------------------- 
        #region Listen

        [Association("Lieferschein-LieferscheinPosition")]
        [ExpandObjectMembers(ExpandObjectMembers.Always)]
        public XPCollection<LieferscheinPosition> LieferscheinPositionenListe
        {
            get { return GetCollection<LieferscheinPosition>(nameof(LieferscheinPositionenListe)); }
        }


        //[DevExpress.Xpo.Aggregated, Association("Lieferschein-Bestellung")]
        //public XPCollection<Bestellung> BestellungenListe
        //{
        //    get { return GetCollection<Bestellung>(nameof(BestellungenListe)); }
        //}

        #endregion
    }
}
