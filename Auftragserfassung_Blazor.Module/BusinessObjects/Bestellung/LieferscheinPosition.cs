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
using System.ComponentModel.DataAnnotations.Schema;
using DisplayNameAttribute = DevExpress.Xpo.DisplayNameAttribute;
using Auftragserfassung_Blazor.Module.Helpers;
using DevExpress.ExpressApp.ConditionalAppearance;
using Auftragserfassung_Blazor.Module.BusinessObjects.Ordner_Lager;

namespace Auftragserfassung_Blazor.Module.BusinessObjects
{
    [DefaultClassOptions]
    [ExpandObjectMembers(ExpandObjectMembers.Always)]

    public class LieferscheinPosition : BelegPosition
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public LieferscheinPosition(Session session)
            : base(session)
        {
        }


        //---------------------------- Klasse ------------------------------------
        #region Override Methoden


        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

            if (IsLoading == false && IsSaving == false && IsDeleted == false)
            {
                if (propertyName == nameof(Lieferschein) && Lieferschein != null)
                {
                    if (Artikel != null)
                    {
                        BinaryOperator criteria = new BinaryOperator("Artikel", Artikel);
                        XPCollection<Lagerplatz> dummy_lagerplatzListe = new XPCollection<Lagerplatz>(PersistentCriteriaEvaluationBehavior.InTransaction, Session, criteria);
                        VerfuegbareMenge = dummy_lagerplatzListe.Sum(x => x.AnzahlDerArtikel);
                    }
                }

                if (Lieferschein != null)
                {
                    if (propertyName == nameof(AnzahlLiefermenge))
                    {
                        if (AnzahlLiefermenge > VerfuegbareMenge)
                        {
                            AnzahlLiefermenge = 0;
                            throw new UserFriendlyException("Die Liefermenge darf nicht größer sein als die Menge, die im Warenausgangslager vorrätig ist!");
                        }

                        Lieferschein.BildeRechnungsSummen();
                    }
                }
            }
        }



        protected override void OnDeleting()
        {
            base.OnDeleting();
            int AnzahlLiefermengePuffer = AnzahlLiefermenge;

            //aktualisieren der Positionen, sollange der Lieferschein nicht gelöscht wird
            if (Lieferschein != null)
            {
                //Rechnungssumme
                AnzahlLiefermenge = 0; //löst die OnChanged Methode aus, welche die neue Rechnungssumme bildet

                //Positionsnummer
                foreach (LieferscheinPosition position in Lieferschein.LieferscheinPositionenListe)
                {
                    if (position.Positionsnummer > Positionsnummer)
                    {
                        position.Positionsnummer--;
                    }
                }
            }


            //Updaten der BestellungsPositionen
            if (PositionWurdeCommited == true)
            {
                UrsprungsPosition.AnzahlGeliefert -= AnzahlLiefermengePuffer;
                UrsprungsPosition.AnzahlOffeneRestmenge = UrsprungsPosition.AnzahlBestellteMenge - UrsprungsPosition.AnzahlGeliefert;

                if (UrsprungsPosition.AnzahlOffeneRestmenge <= 0)
                {
                    UrsprungsPosition.PositionIstVollständig = true;
                }
                else
                {
                    UrsprungsPosition.PositionIstVollständig = false;
                }
            }
        }


        protected override void OnSaving()
        {
            base.OnSaving();
            if (AnzahlLiefermenge > VerfuegbareMenge)
            {
                AnzahlLiefermenge = 0;
                throw new UserFriendlyException("Die Liefermenge darf nicht größer sein als die Menge, die im Warenausgangslager vorrätig ist!");
            }

            if (AnzahlLiefermenge == 0)
            {
                LeerePositionVorCommit = true;
                this.Delete();
                return;
            }

            if (IsDeleted == false && PositionWurdeCommited == false)
            {
                PositionWurdeCommited = true;
                BinaryOperator criteria = new BinaryOperator("Artikel", Artikel);
                XPCollection<Lagerplatz> dummy_lagerplatzListe = new XPCollection<Lagerplatz>(PersistentCriteriaEvaluationBehavior.InTransaction, Session, criteria);

                int restmenge = AnzahlLiefermenge;
                foreach (Lagerplatz lagerplatz in dummy_lagerplatzListe)
                {
                    if (restmenge <= lagerplatz.AnzahlDerArtikel)
                    {
                        //Artikel.Anzahl im Lager wird beim onsaving des Artikels automatisch angepasst
                        lagerplatz.AnzahlDerArtikel -= restmenge;
                        //Artikel.AnzahlReserviert -= restmenge;
                        //Artikel.AnzahlImVersandprozess -= restmenge;
                        restmenge = 0;
                        if (lagerplatz.AnzahlDerArtikel == 0)
                        {
                            lagerplatz.Artikel = null;
                        }
                        break;
                    }
                    else //restmenge > lagerplatz.AnzahlDerArtikel
                    {
                        lagerplatz.AnzahlDerArtikel = 0;
                        lagerplatz.Artikel = null;
                        //Artikel.AnzahlReserviert -= lagerplatz.AnzahlDerArtikel;
                        //Artikel.AnzahlImVersandprozess -= lagerplatz.AnzahlDerArtikel;
                    }
                }
            }

            //Lieferschein.Bestellung.ÜberprüfeZustandDerBestellung();
        }


        #endregion


        #region Methoden


        //public void BesorgeAktuelleSteuer() //WIP
        //{
        //    //Ermittlung der Aktionssteuer, wenn es eine gibt
        //    //verwendeteSteuerErmittler verwendeteSteuerErmittler = new verwendeteSteuerErmittler(Session);
        //    //BestellterArtikelSteuer = verwendeteSteuerErmittler.ErmittlePositionsSteuer(Artikel, RelevantesDatum);
        //    //BestellterArtikelSteuer = Artikel.Steuersatz.AktuellerSteuersatz;
        //}

        #region //alt
        //BestellterArtikelSteuer = Artikel.verwendeteSteuer;
        //foreach (AktionsSteuer aktSteuer in Artikel.Aktionssteuern)
        //{
        //    if (RelevantesDatum > aktSteuer.AktionStart && RelevantesDatum < aktSteuer.AktionEnde)
        //    {
        //        BestellterArtikelSteuer = aktSteuer.AktionsSteuer;
        //        break;
        //    }
        //}

        //bool aktionssteuerGefunden = false;

        //foreach (vorübergehendeSteuer vorübergehendeSteuer in Artikel.Steuersatz.vorübergehendeSteuerListe)
        //{
        //    if (RelevantesDatum > vorübergehendeSteuer.AktionStart && RelevantesDatum < vorübergehendeSteuer.AktionEnde)
        //    {
        //        BestellterArtikelSteuer = vorübergehendeSteuer.AktionsSteuer;
        //        aktionssteuerGefunden = true;
        //        break;
        //    }
        //}


        //if (aktionssteuerGefunden == false)
        //{
        //    BestellterArtikelSteuer = Artikel.verwendeteSteuer;
        //}
        #endregion


        #endregion


        #region Properties


        private BestellungsPosition _UrsprungsPosition; //Debug, ist association wichtig???
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        public BestellungsPosition UrsprungsPosition
        {
            get { return _UrsprungsPosition; }
            set { SetPropertyValue<BestellungsPosition>(nameof(UrsprungsPosition), ref _UrsprungsPosition, value); }
        }


        private Lieferschein _Lieferschein;
        [Association("Lieferschein-LieferscheinPosition")]
        public Lieferschein Lieferschein
        {
            get { return _Lieferschein; }
            set { SetPropertyValue<Lieferschein>(nameof(Lieferschein), ref _Lieferschein, value); }
        }


        private int _AnzahlLiefermenge;
        [Appearance("")]
        [ImmediatePostData]
        [DisplayName("Liefermenge")]
        [ToolTip("Die Menge, die in dieser Lieferung von diesen Artikel geliefert werden")]
        public int AnzahlLiefermenge
        {
            get { return _AnzahlLiefermenge; }
            set { SetPropertyValue<int>(nameof(AnzahlLiefermenge), ref _AnzahlLiefermenge, value); }
        }


        private int _VerfuegbareMenge;
        [ModelDefault("AllowEdit", "false")]
        [ToolTip("menge, welche aktuell im Warenausgangslager ist und an den Kunden gesendet werden kann")]
        public int VerfuegbareMenge
        {
            get { return _VerfuegbareMenge; }
            set { SetPropertyValue<int>(nameof(VerfuegbareMenge), ref _VerfuegbareMenge, value); }
        }


        private int _PositionsnummerDerBestellung;
        [ModelDefault("AllowEdit", "false")]
        public int PositionsnummerDerBestellung
        {
            get { return _PositionsnummerDerBestellung; }
            set { SetPropertyValue<int>(nameof(PositionsnummerDerBestellung), ref _PositionsnummerDerBestellung, value); }
        }

        #region alt

        //private double _BestellterArtikelPreis;
        //[ImmediatePostData]
        //[System.ComponentModel.DisplayName("Listenpreis")]
        //[ModelDefault("AllowEdit", "false")]
        //public double BestellterArtikelPreis
        //{
        //    get { return _BestellterArtikelPreis; }
        //    set { SetPropertyValue<double>(nameof(BestellterArtikelPreis), ref _BestellterArtikelPreis, value); }
        //}


        //private double _BestellterArtikelSteuer;
        //[ImmediatePostData]
        //[ModelDefault("AllowEdit", "false")]
        //public double BestellterArtikelSteuer
        //{
        //    get { return _BestellterArtikelSteuer; }
        //    set { SetPropertyValue<double>(nameof(BestellterArtikelSteuer), ref _BestellterArtikelSteuer, value); }
        //}


        //private double _AktionspreisBestellterArtikel;
        //[ModelDefault("AllowEdit", "false")]
        //public double AktionspreisBestellterArtikel
        //{
        //    get { return _AktionspreisBestellterArtikel; }
        //    set { SetPropertyValue<double>(nameof(AktionspreisBestellterArtikel), ref _AktionspreisBestellterArtikel, value); }
        //}


        //private double _AktionsrabattBestellterArtikel;
        //[ModelDefault("AllowEdit", "false")]
        //public double AktionsrabattBestellterArtikel
        //{
        //    get { return _AktionsrabattBestellterArtikel; }
        //    set { SetPropertyValue<double>(nameof(AktionsrabattBestellterArtikel), ref _AktionsrabattBestellterArtikel, value); }
        //}


        //private int _AnzahlSchonGeliefert;
        //[ImmediatePostData]
        //[ModelDefault("AllowEdit", "false")]
        //[DisplayName("schon geliefert")]
        //public int AnzahlSchonGeliefert
        //{
        //    get { return _AnzahlSchonGeliefert; }
        //    set { SetPropertyValue<int>(nameof(AnzahlSchonGeliefert), ref _AnzahlSchonGeliefert, value); }
        //}

        #endregion

        #endregion
        //--------------------------------
        #region alt
        //private double _SummeDerPositionNetto;
        //[ModelDefault("AllowEdit", "false")]
        //public double SummeDerPositionNetto
        //{
        //    get { return _SummeDerPositionNetto; }
        //    set { SetPropertyValue<double>(nameof(SummeDerPositionNetto), ref _SummeDerPositionNetto, value); }
        //}

        //private double _SummeDerPositionBrutto;
        //[ModelDefault("AllowEdit", "false")]
        //public double SummeDerPositionBrutto
        //{
        //    get { return _SummeDerPositionBrutto; }
        //    set { SetPropertyValue<double>(nameof(SummeDerPositionBrutto), ref _SummeDerPositionBrutto, value); }
        //}

        #endregion
    }
}