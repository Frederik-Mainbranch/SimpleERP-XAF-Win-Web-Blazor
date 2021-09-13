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
using DevExpress.XtraRichEdit.Model;

namespace Auftragserfassung_Blazor.Module.BusinessObjects.Ordner_Lager
{
    [DefaultClassOptions]
    [DefaultProperty("LagerplatzNummer")]
    public class Lagerplatz : BaseObject
    { 
        public Lagerplatz(Session session)
            : base(session)
        {
        }



        private string _LagerplatzNummer;
        [ModelDefault("AllowEdit", "false")]
        public string LagerplatzNummer
        {
            get { return _LagerplatzNummer; }
            set { SetPropertyValue<string>(nameof(LagerplatzNummer), ref _LagerplatzNummer, value); }
        }


        private int _IndexNummer;
        [ModelDefault("AllowEdit", "false")]
        public int IndexNummer
        {
            get { return _IndexNummer; }
            set { SetPropertyValue<int>(nameof(IndexNummer), ref _IndexNummer, value); }
        }


        private Artikel _Artikel;
        [Association("Artikel-Lagerplatz")]
        [ModelDefault("AllowEdit", "false")]
        public Artikel Artikel
        {
            get { return _Artikel; }
            set { SetPropertyValue<Artikel>(nameof(Artikel), ref _Artikel, value); }
        }


        private int _AnzahlDerArtikel;
        [ModelDefault("AllowEdit", "false")]
        public int AnzahlDerArtikel
        {
            get { return _AnzahlDerArtikel; }
            set { SetPropertyValue<int>(nameof(AnzahlDerArtikel), ref _AnzahlDerArtikel, value); }
        }


        private int _Anzahl_Reserviert;
        [ModelDefault("AllowEdit", "false")]
        public int Anzahl_Reserviert
        {
            get { return _Anzahl_Reserviert; }
            set { SetPropertyValue<int>(nameof(Anzahl_Reserviert), ref _Anzahl_Reserviert, value); }
        }


        private Lager _Lager;
        [Association("Lager-Lagerplatz")]
        public Lager Lager
        {
            get { return _Lager; }
            set { SetPropertyValue<Lager>(nameof(Lager), ref _Lager, value); }
        }


        private bool _LagerplatzIstGesperrt;
        [DefaultValue(false)]
        public bool LagerplatzIstGesperrt
        {
            get { return _LagerplatzIstGesperrt; }
            set { SetPropertyValue<bool>(nameof(LagerplatzIstGesperrt), ref _LagerplatzIstGesperrt, value); }
        }


        private bool _LagerplatzIstReserviert;
        [ModelDefault("AllowEdit", "false")]
        [DefaultValue(false)]
        public bool LagerplatzIstReserviert
        {
            get { return _LagerplatzIstReserviert; }
            set { SetPropertyValue<bool>(nameof(LagerplatzIstReserviert), ref _LagerplatzIstReserviert, value); }
        }


        private Waren_Bewegung _ReserviertFuerWarenBewegung;
        [ModelDefault("AllowEdit", "false")]
        public Waren_Bewegung ReserviertFuerWarenBewegung
        {
            get { return _ReserviertFuerWarenBewegung; }
            set { SetPropertyValue<Waren_Bewegung>(nameof(ReserviertFuerWarenBewegung), ref _ReserviertFuerWarenBewegung, value); }
        }


    }
}