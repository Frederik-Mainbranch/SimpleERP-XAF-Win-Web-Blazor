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

namespace Auftragserfassung_Blazor.Module.BusinessObjects
{
    [DefaultClassOptions]

    public class BelegPosition : BaseObject
    { 
        public BelegPosition(Session session)
            : base(session)
        {
        }

        //-----------------------------------------------------------------

        #region Override Methoden

        protected override void OnSaving()
        {
            base.OnSaving();
            if (AnzahlBestellteMenge == 0 || Artikel == null)
            {
                LeerePositionVorCommit = true;
                this.Delete();
                return;
            }
        }

        #endregion

        //---------------------------------------------------------

        #region Properties

        private Artikel _Artikel;
        [ImmediatePostData]
        [RuleRequiredField]
        public Artikel Artikel
        {
            get { return _Artikel; }
            set { SetPropertyValue<Artikel>(nameof(Artikel), ref _Artikel, value); }
        }


        private double _Artikel_Preis;
        [DisplayName("Preis")]
        [ModelDefault("AllowEdit", "false")]
        public double Artikel_Preis
        {
            get { return _Artikel_Preis; }
            set { SetPropertyValue<double>(nameof(Artikel_Preis), ref _Artikel_Preis, value); }
        }


        private bool _PositionWirdGeloescht;
        [Browsable(false)]
        public bool LeerePositionVorCommit
        {
            get { return _PositionWirdGeloescht; }
            set { SetPropertyValue<bool>(nameof(LeerePositionVorCommit), ref _PositionWirdGeloescht, value); }
        }




        private double _Artikel_Steuersatz;
        [DisplayName("Preis")]
        [ModelDefault("AllowEdit", "false")]
        public double Artikel_Steuersatz
        {
            get { return _Artikel_Steuersatz; }
            set { SetPropertyValue<double>(nameof(Artikel_Steuersatz), ref _Artikel_Steuersatz, value); }
        }


        private int _AnzahlBestellteMenge;
        [ImmediatePostData]
        [DisplayName("bestellte Menge")]
        [ToolTip("Die Menge, die von diesen Artikel bestellt wurden")]
        public int AnzahlBestellteMenge
        {
            get { return _AnzahlBestellteMenge; }
            set { SetPropertyValue<int>(nameof(AnzahlBestellteMenge), ref _AnzahlBestellteMenge, value); }
        }

        private int _AnzahlOffeneRestmenge;
        [ImmediatePostData]
        [ModelDefault("AllowEdit", "false")]
        [DisplayName("offene Restmenge")]
        [ToolTip("Die Menge, die von diesen Artikel noch geliefert werden müssen")]
        public int AnzahlOffeneRestmenge
        {
            get { return _AnzahlOffeneRestmenge; }
            set { SetPropertyValue<int>(nameof(AnzahlOffeneRestmenge), ref _AnzahlOffeneRestmenge, value); }
        }


        private int _AnzahlGeliefert;
        [ImmediatePostData]
        [ModelDefault("AllowEdit", "false")]
        [DisplayName("schon geliefert")]
        [ToolTip("Die Menge, die bereits von diesen Artikel geliefert wurden")]
        public int AnzahlGeliefert
        {
            get { return _AnzahlGeliefert; }
            set { SetPropertyValue<int>(nameof(AnzahlGeliefert), ref _AnzahlGeliefert, value); }
        }


        private double _Zeilenrabatt;
        [ImmediatePostData]
        public double Zeilenrabatt
        {
            get { return _Zeilenrabatt; }
            set { SetPropertyValue<double>(nameof(Zeilenrabatt), ref _Zeilenrabatt, value); }
        }


        private int _Positionsnummer;
        [ModelDefault("AllowEdit", "false")]
        public int Positionsnummer
        {
            get { return _Positionsnummer; }
            set { SetPropertyValue<int>(nameof(Positionsnummer), ref _Positionsnummer, value); }
        }


        private bool _PositionWurdeCommited;
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public bool PositionWurdeCommited
        {
            get { return _PositionWurdeCommited; }
            set { SetPropertyValue<bool>(nameof(PositionWurdeCommited), ref _PositionWurdeCommited, value); }
        }


        private double _SummeDerPositionNetto;
        [System.ComponentModel.DisplayName("Nettosumme der Position")]
        [ModelDefault("AllowEdit", "false")]
        public double SummeDerPositionNetto_NP
        {
            get { return _SummeDerPositionNetto; }
            set { SetPropertyValue<double>(nameof(SummeDerPositionNetto_NP), ref _SummeDerPositionNetto, value); }
        }


        private double _SummeDerPositionBrutto;
        [System.ComponentModel.DisplayName("Bruttosumme der Position")]
        [ModelDefault("AllowEdit", "false")]
        public double SummeDerPositionBrutto_NP
        {
            get { return _SummeDerPositionBrutto; }
            set { SetPropertyValue<double>(nameof(SummeDerPositionBrutto_NP), ref _SummeDerPositionBrutto, value); }
        }

        #endregion

        //------------------------------------------------------------------------

        #region Non Persistent Properties





        #endregion
    }
}