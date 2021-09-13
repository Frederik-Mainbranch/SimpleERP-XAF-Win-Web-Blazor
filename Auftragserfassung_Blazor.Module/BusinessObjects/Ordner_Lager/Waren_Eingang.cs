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
using DevExpress.XtraEditors;
using System.Windows.Forms;
using DevExpress.Xpo.DB;
using Auftragserfassung_Blazor.Module.Helpers;
using System.Diagnostics;

namespace Auftragserfassung_Blazor.Module.BusinessObjects.Ordner_Lager
{
    [DefaultClassOptions]

    public class Waren_Eingang : BaseObject
    { 
        public Waren_Eingang(Session session)
            : base(session)
        {
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if(IsDeleted == false && IsSaving == false && IsLoading == false)
            {
                if(propertyName == nameof(Liefermenge_Ist))
                {
                    if(Liefermenge_Ist == Liefermenge_Soll)
                    {
                        WareIstVollstaendig = true;
                    }
                    else if(Liefermenge_Ist < Liefermenge_Soll)
                    {
                        WareIstVollstaendig = false;
                    }
                }
                else if(propertyName == nameof(ZumEinlagernFreigeben) && ZumEinlagernFreigeben == true)
                {
                    if (Liefermenge_Ist > Liefermenge_Soll)
                    {
                        XtraMessage_np = $"Die Liefermenge Ist: {Liefermenge_Ist} ist größer" +
                            $" als die Liefermenge Soll: {Liefermenge_Soll}.\n Bitte überprüfen Sie nochmal die Anzahl der Artikel.";
                    }
                }
            }
        }

        protected override void OnSaving()
        {
            base.OnSaving();

            if(IsDeleted == false && WarenEingangWurdeCommited == false)
            {
                if(ZumEinlagernFreigeben == true && Liefermenge_Ist < 1)
                {
                    ZumEinlagernFreigeben = false;
                    Liefermenge_Ist = 0;
                    throw new UserFriendlyException("Die Liefermenge muss größer als 0 sein!");
                }

                if (ZumEinlagernFreigeben == true && Liefermenge_Ist > 0)
                {
                    //Artikel.AnzahlImLager += Liefermenge_Ist;
                    //Artikel.AnzahlBestelltBeimLieferanten -= Liefermenge_Ist;

                    LagerHelper4000 lagerHelper = new LagerHelper4000(Session, Artikel, true);

                    if (lagerHelper.ErstelleLagerBuchung(Wareneingang_Lager, Artikel, Liefermenge_Ist, false, true) == false)
                    {
                        throw new UserFriendlyException("Es konnten keine freien Lagerplätze ermittelt werden!");
                    }

                    //Kontrolliere auf offene Bestellungen mit Fehlmenge und erstelle dann Warenbewegungen zum Warenausgang
                    XPCollection<Artikel> artikelListe = new XPCollection<Artikel>(Session, CriteriaOperator.Parse("AnzahlImLager < AnzahlReserviert"));

                    CriteriaOperator criteria = CriteriaOperator.And(new BinaryOperator("Artikel", Artikel), CriteriaOperator.Parse("Anzahl_Reserviert < AnzahlDerArtikel"), new BinaryOperator("Lager", Wareneingang_Lager));
                    List<Lagerplatz> lagerplaetzeZumUmlagern = lagerHelper.GefundeneLagerplaetzeListe_WE;

                    foreach (Artikel artikel in artikelListe)
                    {
                        if (artikel == Artikel)
                        {
                            int mengeZumReservieren;
                            if (Liefermenge_Ist > artikel.AnzahlFehlmenge)
                            {
                                mengeZumReservieren = artikel.AnzahlFehlmenge;
                            }
                            else
                            {
                                mengeZumReservieren = Liefermenge_Ist;
                            }
                            lagerHelper.ReserviereArtikel_ImWareneingang(mengeZumReservieren, lagerplaetzeZumUmlagern);
                            //artikel.ReserviereArtikelNachBestellung(mengeZumReservieren);
                            break;
                        }
                    }


                    //Warenbewegungsaufträge vom Wareneingang ins Lager
                    lagerHelper.UpdateAnzahlStandardlager();

                    foreach (Lagerplatz lagerplatz in lagerplaetzeZumUmlagern)
                    {
                        int anzahl = lagerplatz.AnzahlDerArtikel - lagerplatz.Anzahl_Reserviert;
                        if(anzahl > 0)
                        {
                            lagerHelper.Erstelle_Waren_Bewegung_WarenEingangZuLager(Wareneingang_Lager, lagerplatz, anzahl);
                        }
                    }

                    WarenEingangWurdeCommited = true;
                    if(ZumEinlagernFreigeben == true && WareIstUnbeschaedigt == true && WareIstVollstaendig == true)
                    {
                        this.LieferantenLieferungPosition.PositionIstVollständig = true;
                    }
                    Wareneingang_Lager.Save();
                    Artikel.Save();
                }
            }
        }

        #region Properties


        //private int _IndexNummer;
        //[ModelDefault("AllowEdit", "false")]
        //public int IndexNummer
        //{
        //    get { return _IndexNummer; }
        //    set { SetPropertyValue<int>(nameof(IndexNummer), ref _IndexNummer, value); }
        //}


        private int _Positionsnummer;
        [ModelDefault("AllowEdit", "false")]
        public int Positionsnummer
        {
            get { return _Positionsnummer; }
            set { SetPropertyValue<int>(nameof(Positionsnummer), ref _Positionsnummer, value); }
        }


        private Lieferant _Lieferant;
        [ModelDefault("AllowEdit", "false")]
        public Lieferant Lieferant
        {
            get { return _Lieferant; }
            set { SetPropertyValue<Lieferant>(nameof(Lieferant), ref _Lieferant, value); }
        }


        private Lager _Wareneingang_Lager;
        [ModelDefault("AllowEdit", "false")]
        public Lager Wareneingang_Lager
        {
            get { return _Wareneingang_Lager; }
            set { SetPropertyValue<Lager>(nameof(Wareneingang_Lager), ref _Wareneingang_Lager, value); }
        }


        //private Lagerplatz _Lagerplatz;
        //[ModelDefault("AllowEdit", "false")]
        //public Lagerplatz Lagerplatz
        //{
        //    get { return _Lagerplatz; }
        //    set { SetPropertyValue<Lagerplatz>(nameof(Lagerplatz), ref _Lagerplatz, value); }
        //}


        private LieferantenLieferung _LieferantenLieferung;
        [ModelDefault("AllowEdit", "false")]
        public LieferantenLieferung LieferantenLieferung
        {
            get { return _LieferantenLieferung; }
            set { SetPropertyValue<LieferantenLieferung>(nameof(LieferantenLieferung), ref _LieferantenLieferung, value); }
        }


        private LieferantenLieferungPosition _LieferantenLieferungPosition;
        [ModelDefault("AllowEdit", "false")]
        [VisibleInDashboards(false)]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        public LieferantenLieferungPosition LieferantenLieferungPosition
        {
            get { return _LieferantenLieferungPosition; }
            set { SetPropertyValue<LieferantenLieferungPosition>(nameof(LieferantenLieferungPosition), ref _LieferantenLieferungPosition, value); }
        }


        private Artikel _Artikel;
        [ModelDefault("AllowEdit", "false")]
        public Artikel Artikel
        {
            get { return _Artikel; }
            set { SetPropertyValue<Artikel>(nameof(Artikel), ref _Artikel, value); }
        }


        private int _Liefermenge_Soll;
        [ModelDefault("AllowEdit", "false")]
        public int Liefermenge_Soll
        {
            get { return _Liefermenge_Soll; }
            set { SetPropertyValue<int>(nameof(Liefermenge_Soll), ref _Liefermenge_Soll, value); }
        }


        private int _Liefermenge_Ist;
        [ImmediatePostData]
        public int Liefermenge_Ist
        {
            get { return _Liefermenge_Ist; }
            set { SetPropertyValue<int>(nameof(Liefermenge_Ist), ref _Liefermenge_Ist, value); }
        }

        //

        //private Lagerplatz _Lagerplatz;
        //[RuleRequiredField]
        //public Lagerplatz Lagerplatz
        //{
        //    get { return _Lagerplatz; }
        //    set { SetPropertyValue<Lagerplatz>(nameof(Lagerplatz), ref _Lagerplatz, value); }
        //}



        private bool _ZumEinlagernFreigeben;
        [ImmediatePostData]
        public bool ZumEinlagernFreigeben
        {
            get { return _ZumEinlagernFreigeben; }
            set { SetPropertyValue<bool>(nameof(ZumEinlagernFreigeben), ref _ZumEinlagernFreigeben, value); }
        }



        private bool _WareIstVollstaendig;
        [ImmediatePostData]
        [ModelDefault("AllowEdit", "false")]
        public bool WareIstVollstaendig
        {
            get { return _WareIstVollstaendig; }
            set { SetPropertyValue<bool>(nameof(WareIstVollstaendig), ref _WareIstVollstaendig, value); }
        }


        private bool _WareIstUnbeschaedigt;
        [ImmediatePostData]
        public bool WareIstUnbeschaedigt
        {
            get { return _WareIstUnbeschaedigt; }
            set { SetPropertyValue<bool>(nameof(WareIstUnbeschaedigt), ref _WareIstUnbeschaedigt, value); }
        }


        private bool _WarenEingangWurdeCommited;
        [VisibleInDashboards(false)]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        public bool WarenEingangWurdeCommited
        {
            get { return _WarenEingangWurdeCommited; }
            set { SetPropertyValue<bool>(nameof(WarenEingangWurdeCommited), ref _WarenEingangWurdeCommited, value); }
        }


        private string _Notizen;
        public string Notizen
        {
            get { return _Notizen; }
            set { SetPropertyValue<string>(nameof(Notizen), ref _Notizen, value); }
        }


        //--------------------------------------------
        private string _XtraMessage_np;
        [NonPersistent]
        [Browsable(false)]
        public string XtraMessage_np
        {
            get { return _XtraMessage_np; }
            set { SetPropertyValue<string>(nameof(XtraMessage_np), ref _XtraMessage_np, value); }
        }

        #endregion
    }
}