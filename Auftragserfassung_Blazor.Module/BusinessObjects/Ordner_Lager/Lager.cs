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
    [RuleCriteria("LagerKuerzel Laenge", DefaultContexts.Save, "Len(LagerKuerzel) == 3", "Das Lagerkürzel muss genau 3 Zeichen lang sein",
            SkipNullOrEmptyValues = false)]
    [DefaultProperty("Bezeichnung")]

    public class Lager : BaseObject
    {
        public Lager(Session session)
            : base(session)
        {
        }

        #region Override Methoden

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

            if (IsLoading == false && IsSaving == false && IsDeleted == false)
            {
                if (propertyName == nameof(AnzahlDerReihen) || propertyName == nameof(AnzahlDerRegaleInEinerReihe) || propertyName == nameof(AnzahlDerEbenenInEinenRegal)
                    || propertyName == nameof(AnzahlDerStellplaetzeInEinerEbene))
                {
                    if ((int)newValue < 0)
                    {
                        SetzteLagerStrukturOnchangedZurück(propertyName);
                    }

                    AnzahlDerStellplaetzeImLager = AnzahlDerReihen * AnzahlDerRegaleInEinerReihe * AnzahlDerEbenenInEinenRegal * AnzahlDerStellplaetzeInEinerEbene;
                }
            }
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if (IsDeleted == false)
            {

                if (LagerWurdeCommited == false)
                {
                    int counter_reihen = 1;
                    int counter_regale = 1;
                    int counter_ebenen = 1;
                    int counter_stellplaetze = 1;

                    for (int neuerArtikelLaufnummer = 0; neuerArtikelLaufnummer < AnzahlDerStellplaetzeImLager; neuerArtikelLaufnummer++)
                    {
                        Lagerplatz lagerplatz = new Lagerplatz(Session)
                        {
                            Lager = this,
                            IndexNummer = neuerArtikelLaufnummer + 1
                        };

 
                        if (AnzahlDerReihen != 1)
                        {
                            lagerplatz.LagerplatzNummer = $"{LagerKuerzel}-{counter_reihen}-{counter_regale}-{counter_ebenen}-{counter_stellplaetze}";
                        }
                        else if (AnzahlDerRegaleInEinerReihe != 1)
                        {
                            lagerplatz.LagerplatzNummer = $"{LagerKuerzel}-{counter_regale}-{counter_ebenen}-{counter_stellplaetze}";
                        }
                        else if (AnzahlDerEbenenInEinenRegal != 1)
                        {
                            lagerplatz.LagerplatzNummer = $"{LagerKuerzel}-{counter_ebenen}-{counter_stellplaetze}";
                        }
                        else
                        {
                            lagerplatz.LagerplatzNummer = $"{LagerKuerzel}-{counter_stellplaetze}";
                        }


                        #region alt
                        // string LagerplatzNummer_format = $"R{counter_regale}";
                        //if (AnzahlDerReihen != 1)
                        //{
                        //    LagerplatzNummer_format = $"{counter_reihen}-{LagerplatzNummer_format}";
                        //}
                        //LagerplatzNummer_format = $"{LagerKuerzel}-{LagerplatzNummer_format}";
                        //if (AnzahlDerEbenenInEinenRegal != 1)
                        //{
                        //    LagerplatzNummer_format = $"{LagerplatzNummer_format}-{counter_ebenen}";
                        //}
                        //if (AnzahlDerStellplaetzeInEinerEbene != 1)
                        //{
                        //    LagerplatzNummer_format = $"{LagerplatzNummer_format}-{counter_stellplaetze}";
                        //}

                        //if (LagerIstInReihenAngeordnet == true)
                        //{
                        //    lagerplatz.LagerplatzNummer = $"{LagerKuerzel}-{counter_reihen}-{counter_regale}-{counter_ebenen}-{counter_stellplaetze}";
                        //}
                        //else
                        //{
                        //    lagerplatz.LagerplatzNummer = $"{LagerKuerzel}-{counter_regale}-{counter_ebenen}-{counter_stellplaetze}";
                        //}
                        #endregion

                        counter_stellplaetze++;

                        if (counter_stellplaetze > AnzahlDerStellplaetzeInEinerEbene)
                        {
                            counter_ebenen++;
                            counter_stellplaetze = 1;
                        }
                        if (counter_ebenen > AnzahlDerEbenenInEinenRegal)
                        {
                            counter_regale++;
                            counter_ebenen = 1;
                            counter_stellplaetze = 1;
                        }
                        if (counter_regale > AnzahlDerRegaleInEinerReihe)
                        {
                            counter_reihen++;
                            counter_regale = 1;
                            counter_ebenen = 1;
                            counter_stellplaetze = 1;
                        }
                    }
                    LagerWurdeCommited = true;
                }

                BestimmeLagerauslastung();
            }
        }

        #endregion
        //-------------------------------------------------------------------
        #region Methoden

        private void BestimmeLagerauslastung()
        {
            int anzahl_frei = 0;
            int anzahl_gesamt = AnzahlDerStellplaetzeImLager;

                foreach (Lagerplatz lagerplatz in LagerplatzListe)
                {
                    if(lagerplatz.Artikel == null)
                    {
                        anzahl_frei++;
                    }
                }
            BelegteStellplaetze = anzahl_gesamt - anzahl_frei;
            LeereStellplaetze = anzahl_frei;
            BelegungInProzent = Math.Round(((double)BelegteStellplaetze / (double)anzahl_gesamt * 100), 1);
        }

        private void SetzteLagerStrukturOnchangedZurück(string propertyName)
        {
            if (propertyName == nameof(AnzahlDerReihen))
            {
                AnzahlDerReihen = 0;
            }
            else if (propertyName == nameof(AnzahlDerRegaleInEinerReihe))
            {
                AnzahlDerRegaleInEinerReihe = 0;
            }
            else if (propertyName == nameof(AnzahlDerEbenenInEinenRegal))
            {
                AnzahlDerEbenenInEinenRegal = 0;
            }
            else if (propertyName == nameof(AnzahlDerStellplaetzeInEinerEbene))
            {
                AnzahlDerStellplaetzeInEinerEbene = 0;
            }
        }
        #endregion
        //--------------------------------------------------------------------
        #region Properties


        private string _LagerKuerzel;
        [ToolTip("Genau 3 Zeichen")]
        public string LagerKuerzel
        {
            get { return _LagerKuerzel; }
            set { SetPropertyValue<string>(nameof(LagerKuerzel), ref _LagerKuerzel, value); }
        }



        private string _Bezeichnung;
        public string Bezeichnung
        {
            get { return _Bezeichnung; }
            set { SetPropertyValue<string>(nameof(Bezeichnung), ref _Bezeichnung, value); }
        }


        //private bool _LagerIstInReihenAngeordnet;
        //[ImmediatePostData]
        //[VisibleInDashboards(false)]
        //[VisibleInListView(false)]
        //[VisibleInLookupListView(false)]
        //public bool LagerIstInReihenAngeordnet
        //{
        //    get { return _LagerIstInReihenAngeordnet; }
        //    set { SetPropertyValue<bool>(nameof(LagerIstInReihenAngeordnet), ref _LagerIstInReihenAngeordnet, value); }
        //}


        private int _AnzahlDerReihen;
        [ImmediatePostData]
        //[Appearance("Verstecke Anzahl der Reihen", criteria: nameof(LagerIstInReihenAngeordnet) + " = false", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace)]
        public int AnzahlDerReihen
        {
            get { return _AnzahlDerReihen; }
            set { SetPropertyValue<int>(nameof(AnzahlDerReihen), ref _AnzahlDerReihen, value); }
        }


        private int _AnzahlDerRegaleInEinerReihe;
        [ImmediatePostData]
       // [Appearance("Verstecke AnzahlDerRegaleInEinerReihe", criteria: nameof(LagerIstInReihenAngeordnet) + " = false", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
        public int AnzahlDerRegaleInEinerReihe
        {
            get { return _AnzahlDerRegaleInEinerReihe; }
            set { SetPropertyValue<int>(nameof(AnzahlDerRegaleInEinerReihe), ref _AnzahlDerRegaleInEinerReihe, value); }
        }


        //private int _AnzahlDerRegale;
        //[ImmediatePostData]
        ////[Appearance("Verstecke AnzahlDerRegale", criteria: nameof(LagerIstInReihenAngeordnet) + " = true", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
        //public int AnzahlDerRegale
        //{
        //    get { return _AnzahlDerRegale; }
        //    set { SetPropertyValue<int>(nameof(AnzahlDerRegale), ref _AnzahlDerRegale, value); }
        //}




        private int _AnzahlDerEbenenInEinenRegal;
        [ImmediatePostData]
        public int AnzahlDerEbenenInEinenRegal
        {
            get { return _AnzahlDerEbenenInEinenRegal; }
            set { SetPropertyValue<int>(nameof(AnzahlDerEbenenInEinenRegal), ref _AnzahlDerEbenenInEinenRegal, value); }
        }


        private int _AnzahlDerStellplaetzeInEinerEbene;
        [ImmediatePostData]
        public int AnzahlDerStellplaetzeInEinerEbene
        {
            get { return _AnzahlDerStellplaetzeInEinerEbene; }
            set { SetPropertyValue<int>(nameof(AnzahlDerStellplaetzeInEinerEbene), ref _AnzahlDerStellplaetzeInEinerEbene, value); }
        }


        private int _AnzahlDerStellplaetzeImLager;
        [ModelDefault("AllowEdit", "false")]
        [RuleValueComparison(DefaultContexts.Save, ValueComparisonType.GreaterThan, 0)]
        public int AnzahlDerStellplaetzeImLager
        {
            get { return _AnzahlDerStellplaetzeImLager; }
            set { SetPropertyValue<int>(nameof(AnzahlDerStellplaetzeImLager), ref _AnzahlDerStellplaetzeImLager, value); }
        }


        private int _LeereStellplaetze;
        [ModelDefault("AllowEdit", "false")]
        public int LeereStellplaetze
        {
            get { return _LeereStellplaetze; }
            set { SetPropertyValue<int>(nameof(LeereStellplaetze), ref _LeereStellplaetze, value); }
        }


        private int _BelegteStellplaetze;
        [ModelDefault("AllowEdit", "false")]
        public int BelegteStellplaetze
        {
            get { return _BelegteStellplaetze; }
            set { SetPropertyValue<int>(nameof(BelegteStellplaetze), ref _BelegteStellplaetze, value); }
        }


        private double _BelegungInProzent;
        [ModelDefault("AllowEdit", "false")]
        //[ModelDefault("DisplayFormat", "{##0.# '%'}")]
        public double BelegungInProzent
        {
            get { return _BelegungInProzent; }
            set { SetPropertyValue<double>(nameof(BelegungInProzent), ref _BelegungInProzent, value); }
        }


        private bool _LagerWurdeCommited;
        [VisibleInDashboards(false)]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        public bool LagerWurdeCommited
        {
            get { return _LagerWurdeCommited; }
            set { SetPropertyValue<bool>(nameof(LagerWurdeCommited), ref _LagerWurdeCommited, value); }
        }


        private bool _Wareneingang;
        [ImmediatePostData]
        [ToolTip("Markiert das Lager als Wareneingang, wo Artikel kurzfristig im Rahmen der Warenannahme zwischengelagert werden." +
            " Es findet keine Lagerplatzverfügbarkeits Prüfung statt")]
        public bool Wareneingang
        {
            get { return _Wareneingang; }
            set { SetPropertyValue<bool>(nameof(Wareneingang), ref _Wareneingang, value); }
        }


        private bool _Warenausgang;
        [ImmediatePostData]
        [ToolTip("Markiert das Lager als Warenausgang, wo Artikel kurzfristig im Rahmen der Warenversendung zwischengelagert werden." +
    " Es findet keine Lagerplatzverfügbarkeits Prüfung statt")]
        public bool Warenausgang
        {
            get { return _Warenausgang; }
            set { SetPropertyValue<bool>(nameof(Warenausgang), ref _Warenausgang, value); }
        }



        #endregion
        //--------------------------------------
        #region Listen


        [DevExpress.Xpo.Aggregated, Association("Lager-Lagerplatz")]
        public XPCollection<Lagerplatz> LagerplatzListe
        {
            get { return GetCollection<Lagerplatz>(nameof(LagerplatzListe)); }
        }


        [DevExpress.Xpo.Aggregated, Association("Lager-Lager_ArtikeLager_Zugehoerigkeit")]
        public XPCollection<Lager_ArtikeLager_Zugehoerigkeit> Lager_ArtikeLager_ZugehoerigkeitsListe
        {
            get { return GetCollection<Lager_ArtikeLager_Zugehoerigkeit>(nameof(Lager_ArtikeLager_ZugehoerigkeitsListe)); }
        }

        //[Association("Artikel-Lager")]
        //public XPCollection<Artikel> ArtikelListe
        //{
        //    get { return GetCollection<Artikel>(nameof(ArtikelListe)); }
        //}


        #endregion
    }
}