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
using DevExpress.ExpressApp.ConditionalAppearance;
using Auftragserfassung_Blazor.Module.BusinessObjects.Ordner_Lager;
using System.Diagnostics;
using DevExpress.XtraEditors;

namespace Auftragserfassung_Blazor.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class LieferantenLieferungPosition : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public LieferantenLieferungPosition(Session session)
            : base(session)
        {
        }

        //---------------------------- Klasse ------------------------------------
        //---------------------------- Override Methoden -------------------------------

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

            if (IsLoading == false && IsSaving == false)
            {
                if (propertyName == nameof(Artikel) && Artikel != null)
                {
                    if (Positionsnummer == 0)
                    {
                        foreach (LieferantenLieferungPosition lieferantenLieferungPosition in LieferantenLieferung.LieferantenLieferungPositionenListe)
                        {
                            if (lieferantenLieferungPosition.Artikel != null)
                            {
                                _Positionsnummer++;
                            }
                        }
                        Positionsnummer = _Positionsnummer;
                    }
                }
                else if (propertyName == nameof(LieferantenLieferung) && LieferantenLieferung != null)
                {
                    Lieferant = LieferantenLieferung.Lieferant;
                }
                else if (propertyName == nameof(Artikel) && Artikel != null)
                {
                    if (Positionsnummer == 0)    //bestimmt bei neu angelegten Positionen die neue Positionsnummer
                    {
                        int positionsnummer = 0;
                        foreach (LieferantenLieferungPosition position in LieferantenLieferung.LieferantenLieferungPositionenListe)
                        {
                            if (position.Artikel != null)
                            {
                                positionsnummer++;
                            }
                        }
                        Positionsnummer = positionsnummer;
                    }
                }
            }
        }


        protected override void OnDeleting()
        {
            base.OnDeleting();
            foreach (LieferantenLieferungPosition lieferantenLieferungPosition in LieferantenLieferung.LieferantenLieferungPositionenListe)
            {
                if (lieferantenLieferungPosition.Positionsnummer > Positionsnummer)
                {
                    lieferantenLieferungPosition.Positionsnummer--;
                }
            }
        }

        protected override void OnSaving()
        {
            base.OnSaving();

            if (IsDeleted == false && PositionWurdeCommited == false)
            {
                Waren_Eingang waren_Eingang = new Waren_Eingang(Session)
                {
                    Lieferant = this.Lieferant,
                    LieferantenLieferung = this.LieferantenLieferung,
                    LieferantenLieferungPosition = this,
                    Artikel = this.Artikel,
                    Liefermenge_Soll = this.Liefermenge,
                    Wareneingang_Lager = this.LieferantenLieferung.Wareneingang_Lager,
                    Positionsnummer = this.Positionsnummer
                };

                //Artikel.AnzahlBestelltBeimLieferanten += Liefermenge;
                Artikel.BerechneAnzahlAnVerfuegbarenArtikeln();
                PositionWurdeCommited = true;
            }
        }

        #region alt aber funktioniert
        //int anzahlProStellplatz = 0;
        //foreach (Lager_ArtikeLager_Zugehoerigkeit zugehoerigkeit in Artikel.Lager_ArtikeLager_ZugehoerigkeitsListe)
        //{
        //    if (zugehoerigkeit.Lager == LieferantenLieferung.Wareneingang_Lager)
        //    {
        //        anzahlProStellplatz = zugehoerigkeit.AnzahlVonArtikelnAufEinenStellplatz;
        //        break;
        //    }
        //}
        //if(anzahlProStellplatz == 0)
        //{
        //    throw new UserFriendlyException("Dieser Artikel ist nicht für diesen Wareneingang vorgesehen!");
        //}

        //double anzahlAnGebrauchtenStellplaetzen = Math.Ceiling((double)Liefermenge / (double)anzahlProStellplatz);
        //int verbleibendeMenge = Liefermenge;

        //while (verbleibendeMenge != 0)
        //{



        //    //Kontrolle auf angefangene Lagerplaetze
        //    CriteriaOperator criteria = CriteriaOperator.And(new BinaryOperator("Lager", waren_Eingang.Wareneingang_Lager.Oid), new BinaryOperator("Artikel", Artikel),
        //        CriteriaOperator.Parse($"AnzahlDerArtikel < {anzahlProStellplatz}"), new BinaryOperator("LagerplatzIstGesperrt", false));
        //    waren_Eingang.Lagerplatz = Session.FindObject<Lagerplatz>(criteria);
        //    if(waren_Eingang.Lagerplatz != null)
        //    {
        //        int verfuegbareLagermenge = BestimmeVerfuegbareMenge(anzahlProStellplatz, waren_Eingang, verbleibendeMenge);
        //        verbleibendeMenge -= verfuegbareLagermenge;

        //        waren_Eingang.Lagerplatz.Artikel = waren_Eingang.Artikel;
        //        waren_Eingang.Lagerplatz.AnzahlDerArtikel += verfuegbareLagermenge;
        //    }
        //    //Kontrolle auf leere Lagerplaetze
        //    else if (waren_Eingang.Lagerplatz == null)
        //    {
        //        CriteriaOperator criteria2 = CriteriaOperator.And(new BinaryOperator("Lager", waren_Eingang.Wareneingang_Lager.Oid), new NullOperator("Artikel"),
        //                                CriteriaOperator.Parse($"AnzahlDerArtikel < {anzahlProStellplatz}"), new BinaryOperator("LagerplatzIstGesperrt", false));
        //        waren_Eingang.Lagerplatz = Session.FindObject<Lagerplatz>(criteria2);
        //        if(waren_Eingang.Lagerplatz != null)
        //        {
        //            int verfuegbareLagermenge = BestimmeVerfuegbareMenge(anzahlProStellplatz, waren_Eingang, verbleibendeMenge);
        //            verbleibendeMenge -= verfuegbareLagermenge;

        //            waren_Eingang.Lagerplatz.Artikel = waren_Eingang.Artikel;
        //            waren_Eingang.Lagerplatz.AnzahlDerArtikel += verfuegbareLagermenge;
        //        }
        //        else
        //        {
        //            throw new UserFriendlyException("Es konnten keine freien Lagerplätze ermittelt werden!");
        //        }
        #endregion

        //---------------------------- Override Methode -------------------------------
        //-------------------------------- Methoden ---------------------------------------------

        //private XPCollection<Artikel> BestimmeErlaubteArtikel()
        //{
        //    Stopwatch stopwatch = new Stopwatch();
        //    stopwatch.Start();
        //    XPCollection<Artikel> artikelListe = new XPCollection<Artikel>(Session);

        //    //CriteriaOperator criteria = CriteriaOperator.And(new BinaryOperator("IstStandart", "True"), new NullOperator("GCRecord"));
        //    //var abc = ((Steuer)Session.FindObject(typeof(Steuer), criteria)).HöheDesSteuersatzes;


        //    for (int i = artikelListe.Count - 1; i >= 0; i--)
        //    {
        //        foreach (Lager_ArtikeLager_Zugehoerigkeit zugehoerigkeit in artikelListe[i].Lager_ArtikeLager_ZugehoerigkeitsListe)
        //        {
        //            foreach (Lieferant lieferant in artikelListe[i].LieferantenListe)
        //            {
        //                if (zugehoerigkeit.Lager != LieferantenLieferung.Wareneingang_Lager || lieferant != LieferantenLieferung.Lieferant)
        //                {
        //                    artikelListe.Remove(artikelListe[i]);
        //                }
        //            }
        //        }
        //    }

        //    stopwatch.Stop();
        //    double time = (double)(stopwatch.ElapsedMilliseconds) / 1000;
        //    XtraMessageBox.Show($"benötigte Zeit: {time} s");
        //    return artikelListe;
        //}


        //-------------------------------- Methoden ---------------------------------------------
        //-------------------------------- Properties ---------------------------------------------


        private Lieferant _Lieferant;
        [VisibleInDashboards(false)]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        [ImmediatePostData]
        public Lieferant Lieferant
        {
            get { return _Lieferant; }
            set { SetPropertyValue<Lieferant>(nameof(Lieferant), ref _Lieferant, value); }
        }


        private bool _PositionIstOk;
        [VisibleInDashboards(false)]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        public bool PositionIstVollständig
        {
            get { return _PositionIstOk; }
            set { SetPropertyValue<bool>(nameof(PositionIstVollständig), ref _PositionIstOk, value); }
        }


        private Artikel _Artikel;
        //[Appearance("Sperre Artikel", Enabled = false, Criteria = nameof(Lieferant) + " IS NULL")]
        [DataSourceProperty("Lieferant.ArtikelListe")]
        //[DataSourceProperty("ErlaubteArtikel")]
        [RuleRequiredField]
        [ImmediatePostData]
        public Artikel Artikel
        {
            get { return _Artikel; }
            set { SetPropertyValue<Artikel>(nameof(Artikel), ref _Artikel, value); }
        }


        //XPCollection<Artikel> _ErlaubteArtikel;
        //[NonPersistent]
        //[VisibleInListView(false)]
        //public XPCollection<Artikel> ErlaubteArtikel
        //{
        //    get
        //    {
        //        if (_ErlaubteArtikel == null)
        //        {
        //            _ErlaubteArtikel = BestimmeErlaubteArtikel();
        //        }
        //        return _ErlaubteArtikel;
        //    }
        //}


        private bool _PositionWurdeCommited;
        [Browsable(false)]
        [ModelDefault("AllowEdit", "false")]
        public bool PositionWurdeCommited
        {
            get { return _PositionWurdeCommited; }
            set { SetPropertyValue<bool>(nameof(PositionWurdeCommited), ref _PositionWurdeCommited, value); }
        }


        private int _Liefermenge;
        [ImmediatePostData]
        public int Liefermenge
        {
            get { return _Liefermenge; }
            set { SetPropertyValue<int>(nameof(Liefermenge), ref _Liefermenge, value); }
        }


        private LieferantenLieferung _LieferantenLieferung;
        [Browsable(false)]
        [Association("LieferantenLieferung-LieferantenLieferungPosition")]
        public LieferantenLieferung LieferantenLieferung
        {
            get { return _LieferantenLieferung; }
            set { SetPropertyValue<LieferantenLieferung>(nameof(LieferantenLieferung), ref _LieferantenLieferung, value); }
        }


        private int _Positionsnummer;
        [ModelDefault("AllowEdit", "false")]
        public int Positionsnummer
        {
            get { return _Positionsnummer; }
            set { SetPropertyValue<int>(nameof(Positionsnummer), ref _Positionsnummer, value); }
        }





        //-------------------------------- Properties ---------------------------------------------
        //-------------------------------- Listen ---------------------------------------------



        //-------------------------------- Listen ---------------------------------------------
        //-------------------------------- Non Persistent Properties ---------------------------------------------



        //-------------------------------- Non Persistent Properties ---------------------------------------------
    }
}