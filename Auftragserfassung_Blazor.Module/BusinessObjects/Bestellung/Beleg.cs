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

namespace Auftragserfassung_Blazor.Module.BusinessObjects
{
    [DefaultClassOptions]

    public abstract class Beleg : BaseObject
    {
        public Beleg(Session session)
            : base(session)
        {
        }

        #region//Properties
        private bool _BelegWurdeCommitted;
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [ImmediatePostData]
        public bool BelegWurdeCommitted
        {
            get { return _BelegWurdeCommitted; }
            set { SetPropertyValue<bool>(nameof(BelegWurdeCommitted), ref _BelegWurdeCommitted, value); }
        }


        private bool _Kunde_LieferungZurRechnungsadresse;
        [VisibleInListView(false)]
        [NonPersistent]
        [ImmediatePostData]
        public bool Kunde_LieferungZurRechnungsadresse
        {
            get { return _Kunde_LieferungZurRechnungsadresse; }
            set { SetPropertyValue<bool>(nameof(Kunde_LieferungZurRechnungsadresse), ref _Kunde_LieferungZurRechnungsadresse, value); }
        }

        //

        private string _Kunde_LieferAdresse_Straße;
        [NonPersistent]
        [VisibleInListView(false)]
        public string Kunde_LieferAdresse_Straße
        {
            get { return _Kunde_LieferAdresse_Straße; }
            set { SetPropertyValue<string>(nameof(Kunde_LieferAdresse_Straße), ref _Kunde_LieferAdresse_Straße, value); }
        }


        private string _Kunde_LieferAdresse_Postleitzahl;
        [NonPersistent]
        [VisibleInListView(false)]
        //[Appearance("Verstecke " + nameof(Kunde_LieferAdresse_Postleitzahl), TargetItems = nameof(Kunde_LieferAdresse_Postleitzahl), Criteria = nameof(BestellungWurdeKommitet) + " == true",
        //    Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]
        public string Kunde_LieferAdresse_Postleitzahl
        {
            get { return _Kunde_LieferAdresse_Postleitzahl; }
            set { SetPropertyValue<string>(nameof(Kunde_LieferAdresse_Postleitzahl), ref _Kunde_LieferAdresse_Postleitzahl, value); }
        }


        private string _Kunde_LieferAdresse_Ort;
        [NonPersistent]
        [VisibleInListView(false)]
        //[Appearance("Verstecke Kunde_LieferAdresse_Ort", TargetItems = "Kunde_LieferAdresse_Ort", Criteria = "Oid != -1", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]
        public string Kunde_LieferAdresse_Ort
        {
            get { return _Kunde_LieferAdresse_Ort; }
            set { SetPropertyValue<string>(nameof(Kunde_LieferAdresse_Ort), ref _Kunde_LieferAdresse_Ort, value); }
        }


        private string _Kunde_LieferAdresse_Landkreis;
        [NonPersistent]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        // [Appearance("Verstecke Kunde_LieferAdresse_Landkreis", TargetItems = "Kunde_LieferAdresse_Landkreis", Criteria = "Oid != -1", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]
        public string Kunde_LieferAdresse_Landkreis
        {
            get { return _Kunde_LieferAdresse_Landkreis; }
            set { SetPropertyValue<string>(nameof(Kunde_LieferAdresse_Landkreis), ref _Kunde_LieferAdresse_Landkreis, value); }
        }


        private string _Kunde_LieferAdresse_Bundesland;
        [NonPersistent]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        // [Appearance("Verstecke Kunde_LieferAdresse_Bundesland", TargetItems = "Kunde_LieferAdresse_Bundesland", Criteria = "Oid != -1", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]
        public string Kunde_LieferAdresse_Bundesland
        {
            get { return _Kunde_LieferAdresse_Bundesland; }
            set { SetPropertyValue<string>(nameof(Kunde_LieferAdresse_Bundesland), ref _Kunde_LieferAdresse_Bundesland, value); }
        }


        private string _Kunde_LieferAdresse_Land;
        [NonPersistent]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        // [Appearance("Verstecke Kunde_LieferAdresse_Land", TargetItems = "Kunde_LieferAdresse_Land", Criteria = "Oid != -1", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]
        public string Kunde_LieferAdresse_Land
        {
            get { return _Kunde_LieferAdresse_Land; }
            set { SetPropertyValue<string>(nameof(Kunde_LieferAdresse_Land), ref _Kunde_LieferAdresse_Land, value); }
        }

        //

        private string _Kunde_RechnungsAdresse_Straße;
        [NonPersistent]
        [VisibleInListView(false)]
        // [Appearance("Verstecke Kunde_RechnungsAdresse_Straße", TargetItems = "Kunde_RechnungsAdresse_Straße", Criteria = "Oid != -1", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]
        public string Kunde_RechnungsAdresse_Straße
        {
            get { return _Kunde_RechnungsAdresse_Straße; }
            set { SetPropertyValue<string>(nameof(Kunde_RechnungsAdresse_Straße), ref _Kunde_RechnungsAdresse_Straße, value); }
        }


        private string _Kunde_RechnungsAdresse_Postleitzahl;
        [NonPersistent]
        [VisibleInListView(false)]
        // [Appearance("Verstecke Kunde_RechnungsAdresse_Postleitzahl", TargetItems = "Kunde_RechnungsAdresse_Postleitzahl", Criteria = "Oid != -1", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]
        public string Kunde_RechnungsAdresse_Postleitzahl
        {
            get { return _Kunde_RechnungsAdresse_Postleitzahl; }
            set { SetPropertyValue<string>(nameof(Kunde_RechnungsAdresse_Postleitzahl), ref _Kunde_RechnungsAdresse_Postleitzahl, value); }
        }


        private string _Kunde_RechnungsAdresse_Ort;
        [NonPersistent]
        [VisibleInListView(false)]
        // [Appearance("Verstecke Kunde_RechnungsAdresse_Ort", TargetItems = "Kunde_RechnungsAdresse_Ort", Criteria = "Oid != -1", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]
        public string Kunde_RechnungsAdresse_Ort
        {
            get { return _Kunde_RechnungsAdresse_Ort; }
            set { SetPropertyValue<string>(nameof(Kunde_RechnungsAdresse_Ort), ref _Kunde_RechnungsAdresse_Ort, value); }
        }


        private string _Kunde_RechnungsAdresse_Landkreis;
        [NonPersistent]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        // [Appearance("Verstecke Kunde_RechnungsAdresse_Landkreis", TargetItems = "Kunde_RechnungsAdresse_Landkreis", Criteria = "Oid != -1", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]
        public string Kunde_RechnungsAdresse_Landkreis
        {
            get { return _Kunde_RechnungsAdresse_Landkreis; }
            set { SetPropertyValue<string>(nameof(Kunde_RechnungsAdresse_Landkreis), ref _Kunde_RechnungsAdresse_Landkreis, value); }
        }


        private string _Kunde_RechnungsAdresse_Bundesland;
        [NonPersistent]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        // [Appearance("Verstecke Kunde_RechnungsAdresse_Bundesland", TargetItems = "Kunde_RechnungsAdresse_Bundesland", Criteria = "Oid != -1", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]
        public string Kunde_RechnungsAdresse_Bundesland
        {
            get { return _Kunde_RechnungsAdresse_Bundesland; }
            set { SetPropertyValue<string>(nameof(Kunde_RechnungsAdresse_Bundesland), ref _Kunde_RechnungsAdresse_Bundesland, value); }
        }


        private string _Kunde_RechnungsAdresse_Land;
        [NonPersistent]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        // [Appearance("Verstecke Kunde_RechnungsAdresse_Land", TargetItems = "Kunde_RechnungsAdresse_Land", Criteria = "Oid != -1", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]
        public string Kunde_RechnungsAdresse_Land
        {
            get { return _Kunde_RechnungsAdresse_Land; }
            set { SetPropertyValue<string>(nameof(Kunde_RechnungsAdresse_Land), ref _Kunde_RechnungsAdresse_Land, value); }
        }


        //


        private string _Lieferort_Straße;
        [ModelDefault("AllowEdit", "false")]
        public string Lieferort_Straße
        {
            get { return _Lieferort_Straße; }
            set { SetPropertyValue<string>(nameof(Lieferort_Straße), ref _Lieferort_Straße, value); }
        }


        private string _Lieferort_Postleitzahl;
        [ModelDefault("AllowEdit", "false")]
        // [Appearance("Verstecke LieferortDerBestellung_Postleitzahl", TargetItems = "LieferortDerBestellung_Postleitzahl", Criteria = "Oid == -1", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]
        public string Lieferort_Postleitzahl
        {
            get { return _Lieferort_Postleitzahl; }
            set { SetPropertyValue<string>(nameof(Lieferort_Postleitzahl), ref _Lieferort_Postleitzahl, value); }
        }


        private string _Lieferort_Ort;
        [ModelDefault("AllowEdit", "false")]
        // [Appearance("Verstecke LieferortDerBestellung_Ort", TargetItems = "LieferortDerBestellung_Ort", Criteria = "Oid == -1", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]
        public string Lieferort_Ort
        {
            get { return _Lieferort_Ort; }
            set { SetPropertyValue<string>(nameof(Lieferort_Ort), ref _Lieferort_Ort, value); }
        }


        private string _Lieferort_Landkreis;
        [ModelDefault("AllowEdit", "false")]
        // [Appearance("Verstecke LieferortDerBestellung_Landkreis", TargetItems = "LieferortDerBestellung_Landkreis", Criteria = "Oid == -1", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]
        public string Lieferort_Landkreis
        {
            get { return _Lieferort_Landkreis; }
            set { SetPropertyValue<string>(nameof(Lieferort_Landkreis), ref _Lieferort_Landkreis, value); }
        }


        private string _Lieferort_Bundesland;
        [ModelDefault("AllowEdit", "false")]
        // [Appearance("Verstecke LieferortDerBestellung_Bundesland", TargetItems = "LieferortDerBestellung_Bundesland", Criteria = "Oid == -1", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]
        public string Lieferort_Bundesland
        {
            get { return _Lieferort_Bundesland; }
            set { SetPropertyValue<string>(nameof(Lieferort_Bundesland), ref _Lieferort_Bundesland, value); }
        }


        private string _Lieferort_Land;
        [ModelDefault("AllowEdit", "false")]
        // [Appearance("Verstecke LieferortDerBestellung_Land", TargetItems = "LieferortDerBestellung_Land", Criteria = "Oid == -1", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]
        public string Lieferort_Land
        {
            get { return _Lieferort_Land; }
            set { SetPropertyValue<string>(nameof(Lieferort_Land), ref _Lieferort_Land, value); }
        }

        //

        private double _SummeRechnungsNetto;
        [DisplayNameAttribute("Summe Netto")]
        [ModelDefault("AllowEdit", "false")]
        [ImmediatePostData]
        public double SummeRechnungsNetto
        {
            get { return _SummeRechnungsNetto; }
            set { SetPropertyValue<double>(nameof(SummeRechnungsNetto), ref _SummeRechnungsNetto, value); }
        }


        private double _SummeRechnungsBrutto;
        [DisplayNameAttribute("Summe Brutto")]
        [ModelDefault("AllowEdit", "false")]
        [RuleValueComparison(ValueComparisonType.NotEquals, 0)]
        public double SummeRechnungsBrutto
        {
            get { return _SummeRechnungsBrutto; }
            set { SetPropertyValue<double>(nameof(SummeRechnungsBrutto), ref _SummeRechnungsBrutto, value); }
        }
        #endregion
    }
}