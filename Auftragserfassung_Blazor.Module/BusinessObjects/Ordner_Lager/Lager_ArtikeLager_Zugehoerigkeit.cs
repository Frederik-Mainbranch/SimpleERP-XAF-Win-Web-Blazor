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

namespace Auftragserfassung_Blazor.Module.BusinessObjects.Ordner_Lager
{
    [DefaultClassOptions]
    public class Lager_ArtikeLager_Zugehoerigkeit : BaseObject
    {
        public Lager_ArtikeLager_Zugehoerigkeit(Session session)
            : base(session)
        {
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

            if(IsDeleted == false && IsSaving == false && IsLoading == false)
            {
                if(propertyName == nameof(StandardLager) && Artikel != null && StandardLager == true)
                {
                    foreach (Lager_ArtikeLager_Zugehoerigkeit lager_Zugehoerigkeit in Artikel.Lager_ArtikeLager_ZugehoerigkeitsListe)
                    {
                        if(lager_Zugehoerigkeit.Oid != Oid && lager_Zugehoerigkeit.StandardLager == true)
                        {
                            lager_Zugehoerigkeit.StandardLager = false;
                        }
                    }
                }
                else if(propertyName == nameof(Artikel) && Artikel != null)
                {
                    bool standardVorhanden = false;
                    foreach (Lager_ArtikeLager_Zugehoerigkeit lager_Zugehoerigkeit in Artikel.Lager_ArtikeLager_ZugehoerigkeitsListe)
                    {
                        if (lager_Zugehoerigkeit.Oid != Oid && lager_Zugehoerigkeit.StandardLager == true)
                        {
                            standardVorhanden = true;
                            break;
                        }
                    }

                    if(standardVorhanden == false)
                    {
                        StandardLager = true;
                    }
                }
            }
        }


        private int _AnzahlVonArtikelnAufEinenStellplatz;
        [RuleValueComparison(DefaultContexts.Save, ValueComparisonType.GreaterThan, 0)]
        public int AnzahlVonArtikelnAufEinenStellplatz
        {
            get { return _AnzahlVonArtikelnAufEinenStellplatz; }
            set { SetPropertyValue<int>(nameof(AnzahlVonArtikelnAufEinenStellplatz), ref _AnzahlVonArtikelnAufEinenStellplatz, value); }
        }


        private bool _StandardLager;
        [ImmediatePostData]
        [Appearance("Sperre StandardLager", Criteria = nameof(StandardLager) + " = true", Enabled = false)]
        public bool StandardLager
        {
            get { return _StandardLager; }
            set { SetPropertyValue<bool>(nameof(StandardLager), ref _StandardLager, value); }
        }


        private Artikel _Artikel;
        [Association("Artikel-Lager_ArtikeLager_Zugehoerigkeit")]
        [RuleRequiredField]
        public Artikel Artikel
        {
            get { return _Artikel; }
            set { SetPropertyValue<Artikel>(nameof(Artikel), ref _Artikel, value); }
        }


        private Lager _Lager;
      //  [DataSourceCriteria("Wareneingang = false AND Warenausgang = false")]
        [Association("Lager-Lager_ArtikeLager_Zugehoerigkeit")]
        [RuleRequiredField]
        public Lager Lager
        {
            get { return _Lager; }
            set { SetPropertyValue<Lager>(nameof(Lager), ref _Lager, value); }
        }
    }
}