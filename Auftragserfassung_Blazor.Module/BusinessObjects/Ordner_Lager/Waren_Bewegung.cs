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
using DevExpress.Xpo.DB;

namespace Auftragserfassung_Blazor.Module.BusinessObjects.Ordner_Lager
{
    [DefaultClassOptions]
    [DefaultProperty(nameof(Lagerplatz_Ziel))]
    public class Waren_Bewegung : BaseObject
    { 
        public Waren_Bewegung(Session session)
            : base(session)
        {
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if(IsDeleted == false && WarenbewegungWurdeCommitted == false)
            {
                Datum = DateTime.Now;
                //SelectedData query = Session.ExecuteQuery($"SELECT MAX({nameof(Bewegungsnummer)}) FROM {nameof(Waren_Bewegung)}");
                //if ((query.ResultSet[0].Rows[0].Values[0] != null))
                //{
                //    Bewegungsnummer = int.Parse(query.ResultSet[0].Rows[0].Values[0].ToString()) + 1;
                //}
                //else
                //{
                //    Bewegungsnummer = 1;
                //}
                WarenbewegungWurdeCommitted = true;
            }
        }
        //public bool RuleMethod()
        //{
        //    if (UeberschreibeAppearenceRUle == true)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //private bool _UeberschreibeAppearenceRUle;
        //public bool UeberschreibeAppearenceRUle
        //{
        //    get { return _UeberschreibeAppearenceRUle; }
        //    set { SetPropertyValue<bool>(nameof(UeberschreibeAppearenceRUle), ref _UeberschreibeAppearenceRUle, value); }
        //}


        //private int _Bewegungsnummer;
        //[ModelDefault("AllowEdit", "false")]
        //public int Bewegungsnummer
        //{
        //    get { return _Bewegungsnummer; }
        //    set { SetPropertyValue<int>(nameof(Bewegungsnummer), ref _Bewegungsnummer, value); }
        //}


        private bool _WarenbewegungWurdeCommitted;
        [Browsable(false)]
        public bool WarenbewegungWurdeCommitted
        {
            get { return _WarenbewegungWurdeCommitted; }
            set { SetPropertyValue<bool>(nameof(WarenbewegungWurdeCommitted), ref _WarenbewegungWurdeCommitted, value); }
        }




        private DateTime _Datum;
        [ModelDefault("DisplayFormat", "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime Datum
        {
            get { return _Datum; }
            set { SetPropertyValue<DateTime>(nameof(Datum), ref _Datum, value); }
        }


        private Artikel _Artikel;
        [ModelDefault("AllowEdit", "false")]
        public Artikel Artikel
        {
            get { return _Artikel; }
            set { SetPropertyValue<Artikel>(nameof(Artikel), ref _Artikel, value); }
        }


        private int _Anzahl;
        [ModelDefault("AllowEdit", "false")]
        public int Anzahl
        {
            get { return _Anzahl; }
            set { SetPropertyValue<int>(nameof(Anzahl), ref _Anzahl, value); }
        }


        private Lagerplatz _Lagerplatz_Herkunft;
        [DataSourceProperty("Lager_Herkunft.LagerplatzListe")]
        public Lagerplatz Lagerplatz_Herkunft
        {
            get { return _Lagerplatz_Herkunft; }
            set { SetPropertyValue<Lagerplatz>(nameof(Lagerplatz_Herkunft), ref _Lagerplatz_Herkunft, value); }
        }


        private Lagerplatz _Lagerplatz_Ziel;
        [DataSourceProperty("Lager_Ziel.LagerplatzListe")]
        public Lagerplatz Lagerplatz_Ziel
        {
            get { return _Lagerplatz_Ziel; }
            set { SetPropertyValue<Lagerplatz>(nameof(Lagerplatz_Ziel), ref _Lagerplatz_Ziel, value); }
        }


        private Lager _Lager_Herkunft;
        [ImmediatePostData]
        public Lager Lager_Herkunft
        {
            get { return _Lager_Herkunft; }
            set { SetPropertyValue<Lager>(nameof(Lager_Herkunft), ref _Lager_Herkunft, value); }
        }


        private Lager _Lager_Ziel;
        [ImmediatePostData]
        public Lager Lager_Ziel
        {
            get { return _Lager_Ziel; }
            set { SetPropertyValue<Lager>(nameof(Lager_Ziel), ref _Lager_Ziel, value); }
        }



        private bool _WareHatZielErreicht;
        [ModelDefault("AllowEdit", "false")]
        public bool WareHatZielErreicht
        {
            get { return _WareHatZielErreicht; }
            set { SetPropertyValue<bool>(nameof(WareHatZielErreicht), ref _WareHatZielErreicht, value); }
        }


    }
}